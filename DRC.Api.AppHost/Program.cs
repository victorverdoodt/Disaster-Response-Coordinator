var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

var api = builder.AddProject<Projects.DRC_Api>("api")
    .WithReference(redis);


builder.AddProject<Projects.DRC_App>("app")
    .WithReference(api);

builder.Build().Run();
