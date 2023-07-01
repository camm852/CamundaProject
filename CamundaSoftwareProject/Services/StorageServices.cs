using Camunda.Api.Client.ExternalTask;
using Camunda.Api.Client;
using CamundaSoftwareProject.Models;
using CamundaSoftwareProject.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CamundaSoftwareProject.Services
{
    public class StorageServices
    {


        public static async Task FindInStorage(CamundaClient camunda, ExternalTaskInfo task, DatabaseContext databaseContext)
        {
            Dictionary<string, VariableValue> camundaVariables = await camunda.Executions[task.ExecutionId].LocalVariables.GetAll();

            var article = new Storage
            {
                Name = camundaVariables["articulo"].GetValue<string>(),
                Color = camundaVariables["color"].GetValue<string>(),
            };

            var articleInStorage  = await databaseContext.Storage.FirstOrDefaultAsync(x => x.Name.Equals(article.Name) && x.Color.Equals(article.Color));
         

            FetchExternalTasks fetch = new FetchExternalTasks();
            fetch.WorkerId = "worker";
            fetch.MaxTasks = 1;
            fetch.UsePriority = true;
            fetch.Topics = new List<FetchExternalTaskTopic>();
            fetch.Topics.Add(new FetchExternalTaskTopic(task.TopicName, 1));

            List<LockedExternalTask> lockedTasks = await camunda.ExternalTasks.FetchAndLock(fetch);

            CompleteExternalTask request = new CompleteExternalTask();
            request.WorkerId = "worker";
            request.Variables = new Dictionary<string, VariableValue>();
            if(articleInStorage != null) request.Variables.Add("stock", VariableValue.FromObject(articleInStorage.Amount));
            else request.Variables.Add("stock", VariableValue.FromObject(0));
            await camunda.ExternalTasks[task.Id].Complete(request);
        }


    }
}
