using FinalProject_Web.AuthFinalProjectApp;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Negotiate;
using System.Text.Json;
using FinalProject_Web.Model;
using FinalProject_Web.Extensions;
using Newtonsoft.Json;
using FinalProject_Web.Vars;
using Azure;
using FinalProject_Web.Interfaces;
using System.Runtime.CompilerServices;
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
        /// ����� ������� � API ��� ���������� ���������� ���������� ����� �
        /// ��������� � ���������� ���������� (����� �������� Web ����������)
        /// � ����� ����������� �������� � API.
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