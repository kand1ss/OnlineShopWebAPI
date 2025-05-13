using System.Text;
using System.Text.Json;
using CatalogManagementService.Application.DTO;
using Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQClient.Consumers;
using Xunit;

namespace RabbitMQClient.tests;

public class MessageHandlerConsumerTests : TestWhichUsingRabbitMQ
{
    private readonly MessageHandlerConsumer<UpdateProductRequest, CreateProductRequest> _consumer;
    private readonly string _testQueue = "test-queue";

    public MessageHandlerConsumerTests()
    {
        var processorMock = new Mock<IRequestProcessor<UpdateProductRequest, CreateProductRequest>>();
        processorMock.Setup(x => 
                x.Process(It.IsAny<UpdateProductRequest>()))
            .ReturnsAsync(new CreateProductRequest("test", null, 0));

        var deserializerMock = new Mock<IMessageDeserializer<byte[], UpdateProductRequest>>();
        deserializerMock.Setup(x => x.Deserialize(It.IsAny<byte[]>()))
            .Returns<byte[]>(x =>
            {
                var body = Encoding.UTF8.GetString(x);
                return JsonSerializer.Deserialize<UpdateProductRequest>(body)
                    ?? throw new ArgumentNullException(nameof(x));
            });

        var scopeMock = new Mock<IServiceScope>();
        
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(processorMock.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        scopeMock.Setup(x => x.ServiceProvider).Returns(serviceProvider);
        
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(x => x.CreateScope()).Returns(scopeMock.Object);
        
        var loggerMock = new Mock<ILogger<MessageHandlerConsumer<UpdateProductRequest, CreateProductRequest>>>();

        _consumer = new(
            RabbitClientMock.Object, scopeFactoryMock.Object, deserializerMock.Object, loggerMock.Object);
    }

    private async Task<TaskCompletionSource<CreateProductRequest>> InitializeProceesingConsumer(string message)
    {
        var tcs = new TaskCompletionSource<CreateProductRequest>();
        _consumer.OnProcessed = result =>
        {
            tcs.SetResult(result);
            return Task.CompletedTask;
        };
        
        await InitializeConsumer(message);
        return tcs;
    }

    private async Task InitializeConsumer(string message)
    {
        var consumer = new AsyncEventingBasicConsumer(Channel);
        consumer.ReceivedAsync += _consumer.ProcessConsumeAsync;
        await Channel.BasicConsumeAsync(_testQueue, false, consumer);
        await Channel.BasicPublishAsync("", _testQueue, true, body: Encoding.UTF8.GetBytes(message));
    }

    private static async Task WaitTCS(TaskCompletionSource<CreateProductRequest> tcs)
    {
        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(5000));
        if (completedTask != tcs.Task)
            throw new TimeoutException();
    }

    [Fact]
    public async Task ProcessConsumeAsync_CorrectMessage_MessageConsumed()
    {
        var request = new UpdateProductRequest(Guid.NewGuid(), "test", "description", 100);
        var json = JsonSerializer.Serialize(request);

        var tcs = await InitializeProceesingConsumer(json);
        await WaitTCS(tcs);

        var result = tcs.Task.Result;
        
        Assert.NotNull(result);
        Assert.Equal("test", result.Title);
        Assert.Null(result.Description);
        Assert.Equal(0, result.Price);
    }

    [Fact]
    public async Task ProcessConsumeAsync_IncorrectMessage_MessageConsumedButNotProcessed()
    {
        Exception? exception = null;
        var tcs = new TaskCompletionSource<CreateProductRequest?>();
        _consumer.OnError = ex =>
        {
            exception = ex;
            tcs.SetResult(null);
            return Task.CompletedTask;
        };
        
        await InitializeConsumer("fake");
        await WaitTCS(tcs);
        
        var request = tcs.Task.Result;
        Assert.NotNull(exception);
        Assert.Null(request);
    }
}