var builder = DistributedApplication.CreateBuilder(args);

// Add services
var identityApi = builder.AddProject<Projects.IdentityService>("identity-api");
var orderApi = builder.AddProject<Projects.OrderService>("order-api");
var paymentApi = builder.AddProject<Projects.PaymentService>("payment-api");
var productApi = builder.AddProject<Projects.ProductService>("product-api");
var blazorApp = builder.AddProject<Projects.BlazorWebApp>("web-app");

// Add Kafka (container)
// var kafka = builder.AddContainer("kafka", "confluentinc/cp-kafka:latest")
//                    .WithPort(9092, 9092)
//                    .WithEnvironment("KAFKA_ZOOKEEPER_CONNECT", "zookeeper:2181");

// Add Gateway
builder.AddProject<Projects.YarpGateway>("gateway")
       .WithReference(identityApi)
       .WithReference(orderApi)
       .WithReference(paymentApi)
       .WithReference(productApi)
       .WithReference(blazorApp)
       ;



builder.Build().Run();
