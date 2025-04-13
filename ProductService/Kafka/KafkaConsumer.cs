
using System.Threading;
using Confluent.Kafka;
using System.Text.Json;
using ProductService.Models;
public class KafkaConsumer(IServiceScopeFactory scopeFactory, ProductServiceContext _productContext) : BackgroundService
{

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            _ = ComsumeAsync("order-topic", stoppingToken);
        }, stoppingToken);
    }
    public async Task ComsumeAsync(string topic, CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig()
        {
            GroupId = "order-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);
            var order = JsonSerializer.Deserialize<Order>(consumeResult.Message.Value);
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ProductServiceContext>();

            foreach (var item in order.OrderItems)
            {
                var product = await dbContext.Products.FindAsync(item.Id);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    await dbContext.SaveChangesAsync();
                }
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