using Camunda.Api.Client;
using Camunda.Api.Client.ExternalTask;
using CamundaSoftwareProject.Persistence;
using CamundaSoftwareProject.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddNpgsql<DatabaseContext>(builder.Configuration.GetConnectionString("PostgresConnection"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();



CamundaClient camunda = CamundaClient.Create("http://localhost:8080/engine-rest");

var timer = new System.Timers.Timer(2000); // El intervalo se especifica en milisegundos (2 segundos)
timer.Elapsed += async (sender, e)=>
{
    List<ExternalTaskInfo> allTask = await camunda.ExternalTasks.Query().List();
    DatabaseContext databaseContext = new DatabaseContext(builder.Services.BuildServiceProvider().GetService<DbContextOptions<DatabaseContext>>());

    foreach (ExternalTaskInfo task in allTask)
    {
        var nameTask = task.TopicName;

        await Console.Out.WriteLineAsync("Timer");

        switch (nameTask)
        {
            case "CreateUser":
                timer.Stop();
                await UserServices.CreateUser(camunda, task, databaseContext);
                timer.Start();
                break;
            case "Login":
                timer.Stop();
                await UserServices.Login(camunda, task, databaseContext);
                timer.Start();
                break;
            case "ReviewStorage":
                timer.Stop();
                await StorageServices.FindInStorage(camunda, task, databaseContext);
                timer.Start();
                break;
            default:
                break;
        }
    }
};
timer.AutoReset = true; // Establece en true si deseas que el temporizador se reinicie automáticamente después de cada evento de tiempo transcurrido
timer.Start();

app.Run();