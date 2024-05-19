using BankDataTransformation.ViewModels;
using BankDataTransformation.Windows;
using BankDataTransformationLogic.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using System.Windows;

namespace BankDataTransformation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Container { get; set; }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            MySettings.Default.Save();
            var ExcelModuleClose = Container.GetService<IExcelModule>();
            ExcelModuleClose.KillExcel();
            System.Windows.Application.Current.Shutdown();
            Environment.Exit(Environment.ExitCode);
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ConfigureWindows(serviceCollection);
            ConfigureViewModels(serviceCollection);

            Container = serviceCollection.BuildServiceProvider();


            var MainWindow = Container.GetRequiredService<MainWindow>();
            MainWindow.Show();
          
        }

        private void ConfigureViewModels(IServiceCollection services)
        {
            services.AddSingleton<IMainViewModel,MainViewModel>();
        }

        private void ConfigureWindows(IServiceCollection services)
        {
            services.AddSingleton(typeof(MainWindow));
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IExcelModule, ExcelModule>();


            services.AddTransient<IAccountHistoryReader, AccountHistoryReader>();
            services.AddTransient<IAccountHistoryRebuilder, AccountHistoryRebuilder>();
        }
    }
}
