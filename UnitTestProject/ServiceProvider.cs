using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WPFSampleApplication.Model;
using WPFSampleApplication.ViewModel;
using WPFSampleApplication.View;

using BackgroundProcessSample;
using UnitTestProject.TestModels;

namespace UnitTestProject {
    public static class ServiceProvider {
        public static IServiceProvider Service { get; private set; }

        static ServiceProvider() {
            var services = new ServiceCollection();

            services.AddSingleton<ISettingsProcess, SettingsProcess>();
            services.AddSingleton<ISettingsWindowsUI, SettingsWindowsUI>();
            services.AddSingleton<IGlobalSettings, GlobalSettings>();
            
            services.AddSingleton<IGlobalSettings, GlobalSettings>();
            services.AddSingleton<IBackgroundProcessManager, TestBackgroundProcess>();
            services.AddSingleton<IBackgroundProcessModel, BackgroundProcessModel>();
        
            Service = services.BuildServiceProvider();
        }


    }
}
