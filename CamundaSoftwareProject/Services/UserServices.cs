using Camunda.Api.Client;
using Camunda.Api.Client.ExternalTask;
using CamundaSoftwareProject.Models;
using CamundaSoftwareProject.Persistence;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CamundaSoftwareProject.Services
{
    public class UserServices
    {

        public static string HashPassword(string password)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(password);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    password = Convert.ToBase64String(ms.ToArray());
                }
            }
            return password;
        }
        
        public static async Task CreateUser(CamundaClient camunda, ExternalTaskInfo task, DatabaseContext databaseContext)
        {
            Dictionary<string, VariableValue> camundaVariables = await camunda.Executions[task.ExecutionId].LocalVariables.GetAll();

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = camundaVariables["user"].GetValue<string>(),
                Email = camundaVariables["email"].GetValue<string>(),
                Address = camundaVariables["address"].GetValue<string>(),
                Password = HashPassword(camundaVariables["password"].GetValue<string>()),
                Telephone = camundaVariables["telephone"].GetValue<string>(),
            };

            await databaseContext.Users.AddAsync(user);
            await databaseContext.SaveChangesAsync();

            FetchExternalTasks fetch = new FetchExternalTasks();
            fetch.WorkerId = "worker";
            fetch.MaxTasks = 1;
            fetch.UsePriority = true;
            fetch.Topics = new List<FetchExternalTaskTopic>();
            fetch.Topics.Add(new FetchExternalTaskTopic(task.TopicName, 1));

            List<LockedExternalTask> lockedTasks = await camunda.ExternalTasks.FetchAndLock(fetch);

            CompleteExternalTask request = new CompleteExternalTask();
            request.WorkerId = "worker";
            request.Variables = new Dictionary<string, VariableValue>
            {
                { "idUser", VariableValue.FromObject(user.Id) }
            };

            await camunda.ExternalTasks[task.Id].Complete(request);
        }

        public static async Task Login(CamundaClient camunda, ExternalTaskInfo task, DatabaseContext databaseContext)
        {
            Dictionary<string, VariableValue> camundaVariables = await camunda.Executions[task.ExecutionId].LocalVariables.GetAll();

            string email = camundaVariables["email"].GetValue<string>();
            string password = camundaVariables["password"].GetValue<string>();
            bool login = false;

            var user = await databaseContext.Users.FirstOrDefaultAsync(x => x.Email.Equals(email) );
            
            if(user != null) { 
                string passwordHash = HashPassword(password);
                await Console.Out.WriteLineAsync(passwordHash);
                if (user.Password.Equals(passwordHash))
                {
                    login = true;
                }
            }


            FetchExternalTasks fetch = new FetchExternalTasks();
            fetch.WorkerId = "worker";
            fetch.MaxTasks = 1;
            fetch.UsePriority = true;
            fetch.Topics = new List<FetchExternalTaskTopic>();
            fetch.Topics.Add(new FetchExternalTaskTopic(task.TopicName, 1));

            List<LockedExternalTask> lockedTasks = await camunda.ExternalTasks.FetchAndLock(fetch);

            CompleteExternalTask request = new CompleteExternalTask();
            request.WorkerId = "worker";
            request.Variables = new Dictionary<string, VariableValue>
            {
                { "login", VariableValue.FromObject(login) }
            };

            await camunda.ExternalTasks[task.Id].Complete(request);
        }


    }
}
