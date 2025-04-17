
using System.Threading;
using Confluent.Kafka;
using System.Text.Json;
using ProductService.Models;
public class KafkaConsumer(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => ComsumeAsync("order-topic", stoppingToken), stoppingToken);
    }

    public async Task ComsumeAsync(string topic, CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = "order-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(topic); // ✅ Bắt buộc!

        Console.WriteLine($"✅ Subscribed to topic {topic}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                Console.WriteLine($"📥 Received message: {result.Message.Value}");

                var order = JsonSerializer.Deserialize<Order>(result.Message.Value);

                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ProductServiceContext>();

                foreach (var item in order.OrderItems)
                {
                    var product = await dbContext.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock -= item.Quantity;
                    }
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ KafkaConsumer error: {ex.Message}");
            }
        }
    }
}


public class Order
{

    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

}

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Order Order { get; set; } = null!;
}