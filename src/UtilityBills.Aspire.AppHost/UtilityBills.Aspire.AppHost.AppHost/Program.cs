var builder = DistributedApplication.CreateBuilder(args);

// builder.AddProject<Projects.UtilityBills_Host>("utilitybills-host");

builder.Build().Run();