using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Bolareshet.UploadPresignedURClient.MefTools;

namespace Bolareshet.UploadPresignedURClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configure Bootstrapper
            var bootstrapper = new AppInitBootstrapper();
            bootstrapper.Run();
        }
    }
}
