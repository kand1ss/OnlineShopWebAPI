using Moq;
using RabbitMQ.Client;
using Testcontainers.RabbitMq;
using Xunit;

namespace RabbitMQClient.tests;

public class TestWhichUsingRabbitMQ : IAsyncLifetime
{
    private RabbitMqContainer _rabbitContainer;
    private IConnection _connection;
    protected IChannel Channel { get; private set; }
    
    protected Mock<IRabbitMQClient> RabbitClientMock { get; } = new();
    
    
    public async Task InitializeAsync()
    {
        _rabbitContainer = new RabbitMqBuilder().Build();
        await _rabbitContainer.StartAsync();
        
        var connectionFactory = new ConnectionFactory
        {
            HostName = _rabbitContainer.Hostname
        };
        
        _connection = await connectionFactory.CreateConnectionAsync();
        Channel = await _connection.CreateChannelAsync();
        
        RabbitClientMock.Setup(x => x.Channel).Returns(Channel);
    }

    public async Task DisposeAsync()
    {
        await Channel.DisposeAsync();
        await _connection.DisposeAsync();
        await _rabbitContainer.StopAsync();
        await _rabbitContainer.DisposeAsync();
    }
}