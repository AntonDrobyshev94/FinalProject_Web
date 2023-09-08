using Microsoft.AspNetCore;
using FinalProject_Web.Vars;
using FinalProject_Web.Data;

namespace FinalProject_Web
{
    public class Program
    { 
        public static void Main(string[] args)
        {
            var init = BuildWebHost(args);
            using (var scope = init.Services.CreateScope())
            {
                var s = scope.ServiceProvider;
            }
            AddTitlesAndContacts();
            init.Run();
        }

        /// <summary>
        /// Метод запроса к API для сохранения переменных титульного листа и
        /// контактов в глобальную переменную (перед запуском Web приложения)
        /// с целью минимизации запросов к API.
        /// </summary>
        public static void AddTitlesAndContacts()
        {
            ApplicationDataApi applicationData = new ApplicationDataApi();

            Variables.titleModelVars = applicationData.GetTitle();
            Variables.contacts = applicationData.GetContacts();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(log => log.AddConsole())
                .Build();
    }
}