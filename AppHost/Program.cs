var builder = DistributedApplication.CreateBuilder(args);

// Add services
var identityApi = builder.AddProject<Projects.IdentityService>("identity-api");
// var orderApi = builder.AddProject<Projects.OrderService>("order-api");
var paymentApi = builder.AddProject<Projects.PaymentService>("payment-api");
// var productApi = builder.AddProject<Projects.ProductService>("product-api");
var blazorApp = builder.AddProject<Projects.BlazorWebApp>("web-app");

// Add Kafka (container)
// Add Zookeeper (trước)
// var zookeeper = builder.AddContainer("zookeeper", "confluentinc/cp-zookeeper:7.3.2")
//     .WithPort(2181, 2181)
//     .WithEnvironment("ZOOKEEPER_CLIENT_PORT", "2181");

// // Add Kafka (sau)
// var kafka = builder.AddContainer("kafka", "confluentinc/cp-kafka:7.3.2")
//     .WithPort(9092, 9092)
//     .WithEnvironment("KAFKA_BROKER_ID", "1")
//     .WithEnvironment("KAFKA_ZOOKEEPER_CONNECT", "zookeeper:2181")
//     .WithEnvironment("KAFKA_ADVERTISED_LISTENERS", "PLAINTEXT://localhost:9092")
//     .WithEnvironment("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1")
    // .DependsOn(zookeeper); // 🟢 OK vì zookeeper đã được khai báo
// Add Gateway
builder.AddProject<Projects.YarpGateway>("gateway")
       .WithReference(identityApi)
    //    .WithReference(orderApi)
       .WithReference(paymentApi)
    //    .WithReference(productApi)
       .WithReference(blazorApp);



builder.Build().Run();
