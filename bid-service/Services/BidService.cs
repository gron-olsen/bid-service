using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using bidService.Models;
using Newtonsoft.Json;
using MongoDB.Driver;

namespace bidService;

// Worker Class + BackgroundService from microsoft extension 
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMongoDatabase _database;
    private ConnectionFactory factory = new ConnectionFactory();

    public readonly IConfiguration _configuration;
    public string BidCollection;
    private IConnection connection;
    private IModel channel;
    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        try
        {
            _logger.LogInformation("Connecting to RabbitMQ at {0}:{1}", _configuration["rabbitmqUrl"], _configuration["rabbitMQPort"]);
            
            factory = new ConnectionFactory()
            {
                HostName = _configuration["rabbitmqUrl"] ?? "localhost",
                Port = Convert.ToInt16(_configuration["rabbitMQPort"] ?? "5672"),
                UserName = _configuration["rabbitmqUsername"] ?? "guest",
                Password = _configuration["rabbitmqUserpassword"] ?? "guest"
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "auction",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _logger.LogInformation("Connected to RabbitMQ successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ at {0}:{1}", _configuration["rabbitmqUrl"], _configuration["rabbitMQPort"]);
            throw;
        }


        try
        {
            _logger.LogInformation($"mongodb: {_configuration["mongodb"]}");
            // Create MongoDB database connection using configuration
            var mongoClient = new MongoClient(_configuration["mongodb"]);
            _database = mongoClient.GetDatabase(_configuration["database"]);
            BidCollection = (_configuration["collection"]) ?? string.Empty;
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, $"Failed to access mongodb: {_configuration["mongodb"]}");
            throw;
        }
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Create a consumer for the channel
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            // Process the received message
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation(message);
            var routingKey = ea.RoutingKey;

            // Get the auction collection from the MongoDB database
            var auctionCollection = _database.GetCollection<Bid>(BidCollection);

            // Deserialize the message into a Bid object and insert it into the auction collection
            Bid bid = JsonConvert.DeserializeObject<Bid>(message);
            auctionCollection.InsertOne(bid);
        };
        channel.BasicConsume(queue: "auction",
                             autoAck: true,
                             consumer: consumer);

        // Continuously perform work until cancellation is requested
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Working time: {time}", DateTimeOffset.Now);

            // Delay the execution for 10 seconds
            await Task.Delay(10000, stoppingToken);
        }
    }
}