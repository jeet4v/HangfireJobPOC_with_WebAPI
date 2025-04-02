using Hangfire;
using Hangfire.SqlServer;
using HangfireJobDemo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Hangfire with SQL Server storage
builder.Services.AddHangfire(config => 
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireDBConnection"))
    );

// Add Hangfire background job server
builder.Services.AddHangfireServer();
builder.Services.AddScoped<IJobTestService, JobTestService>();

var app = builder.Build();


// Enable Hangfire Dashboard (Optional)
app.UseHangfireDashboard(); // can set specific URL by app.UseHangfireDashboard("/MyJobs");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
