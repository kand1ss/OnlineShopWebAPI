using System.Text;
using System.Text.Json;
using APIGate.Application;
using CatalogManagementService.Application.DTO;
using CatalogManagementService.Application.Replies;
using Core.Contracts;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;

namespace RabbitMQClient.tests;

public class MessageRequestClientTests : TestWhichUsingRabbitMQ
{
    private readonly MessageRequestClient<CreateProductRequest> _requestClient;
    private readonly string _testQueue = "test-queue";
    private readonly string _testQueueReply = "test-queue-reply";

    public MessageRequestClientTests()
    {
        var deserializerMock = new Mock<IMessageDeserializer<byte[], RequestReply<CreateProductRequest>>>();
        deserializerMock
            .Setup(x => x.Deserialize(It.IsAny<byte[]>()))
            .Returns<byte[]>(x =>
            {
                var body = Encoding.UTF8.GetString(x);
                var request = JsonSerializer.Deserialize<CreateProductRequest>(body)
                              ?? throw new ArgumentNullException(nameof(x));

                return RequestReply<CreateProductRequest>.Ok(request);
            });

        var loggerMock = new Mock<ILogger<MessageRequestClient<CreateProductRequest>>>();
        _requestClient = new(RabbitClientMock.Object, deserializerMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task PublishMessageAndConsumeReply_CorrectMessageAndQueues_SuccessfulResponseReceived()
    {
        await InitializeQueues();
        await CreateConsumer();
        var (request, reply) = await CreateProductAndSendMessage();

        Assert.NotNull(reply);
        Assert.Null(reply.Error);
        Assert.True(reply.Success);
        Assert.Equal(reply.Result.Title, request.Title);
        Assert.Equal(reply.Result.Description, request.Description);
        Assert.Equal(reply.Result.Price, request.Price);
    }

    private async Task<(CreateProductRequest request, RequestReply<CreateProductRequest> reply)> CreateProductAndSendMessage()
    {
        var request = new CreateProductRequest("Product", "Description", 100);
        var json = JsonSerializer.Serialize(request);
        var body = Encoding.UTF8.GetBytes(json);
        var reply = await _requestClient.PublishMessageAndConsumeReply(body, _testQueue, _testQueueReply);
        return (request, reply);
    }

    private async Task CreateConsumer()
    {
        var consumer = new AsyncEventingBasicConsumer(Channel);
        consumer.ReceivedAsync += InitializeConsumer;
        await Channel.BasicConsumeAsync(_testQueue, false, consumer);
    }
    
    private async Task InitializeConsumer(object model, BasicDeliverEventArgs ea)
    {
        var props = new BasicProperties
        {
            ReplyTo = ea.BasicProperties.ReplyTo,
            CorrelationId = ea.BasicProperties.CorrelationId
        };

        await Channel.BasicPublishAsync("", props.ReplyTo ?? _testQueueReply,
            true, props, body: ea.Body.ToArray());
        await Channel.BasicAckAsync(ea.DeliveryTag, false);
    }

    private async Task InitializeQueues()
    {
        await Channel.QueueDeclareAsync(_testQueue, false, false, false);
        await Channel.QueueDeclareAsync(_testQueueReply, false, false, false);
    }

    [Fact]
    public async Task PublishMessageAndConsumeReply_IncorrectMessage_ErrorResponseReceived()
    {
        await InitializeQueues();
        await CreateConsumer();

        var reply = await _requestClient.PublishMessageAndConsumeReply([], _testQueue, _testQueueReply);

        Assert.NotNull(reply);
        Assert.False(reply.Success);
    }

    [Fact]
    public async Task PublishMessageAndConsumeReply_NoReply_ReceivedNegativeReplyAfterTimeout()
    {
        await InitializeQueues();
        var (request, reply) = await CreateProductAndSendMessage();

        Assert.NotNull(reply);
        Assert.False(reply.Success);
        Assert.Equal(reply.Error, "No response was received within the defined timeout period.");
    }

    [Fact]
    public async Task PublishMessageAndConsumeReply_ManyMessagesInReplyQueue_ReceivedReplyWithCorrectMessage()
    {
        await InitializeQueues();

        await Channel.BasicPublishAsync("", _testQueueReply, true, body: Array.Empty<byte>());
        await Channel.BasicPublishAsync("", _testQueueReply, true, body: Array.Empty<byte>());

        await CreateConsumer();
        var (request, reply) = await CreateProductAndSendMessage();

        Assert.NotNull(reply);
        Assert.True(reply.Success);
        Assert.Equal(reply.Result.Title, request.Title);
        Assert.Equal(reply.Result.Description, request.Description);
        Assert.Equal(reply.Result.Price, request.Price);
    }

    [Fact]
    public async Task PublishMessageAndConsumeReply_ReceivingReply_ConsumerUnsubscribesAfterReplyReceiving()
    {
        await InitializeQueues();
        await CreateConsumer();
        
        Assert.Equal(0, (int)await Channel.ConsumerCountAsync(_testQueueReply));
        
        var (request, reply) = await CreateProductAndSendMessage();
        
        Assert.Equal(0, (int)await Channel.ConsumerCountAsync(_testQueueReply));
    }
}