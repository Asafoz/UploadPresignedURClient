using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Mef;

namespace Bolareshet.UploadPresignedURClient.MefTools
{
    class AppInitBootstrapper : MefBootstrapper
    {
        private const string ModulesSectionPath = "Bolareshet.UploadPresignedURClient/mefModules";


        protected override DependencyObject CreateShell()
        {
            //initialize the Shell here
            return this.Container.GetExportedValue<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Shell)this.Shell;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));
            this.AggregateCatalog.AddMoudlesFromAppConfig(ModulesSectionPath);
        }

    }
}
