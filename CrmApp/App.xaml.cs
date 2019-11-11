using Crm.App.Views;
using Crm.App.ViewModels;
using Crm.Repository;
using Crm.Repository.Rest;
using Crm.Repository.Sql;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Crm.App
{
    sealed partial class App : Application
    {
        public static MainViewModel ViewModel { get; } = new MainViewModel();

        public static ICrmRepository Repository { get; private set; }

        public App() => InitializeComponent();

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Load the database.
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(
                "data_source", out object dataSource))
            {
                switch (dataSource.ToString())
                {
                    case "Rest API": UseRest(); break;
                    default: UseSqlite(); break; 
                }
            }
            else
            {
                UseSqlite();
            }

            AppShell shell = Window.Current.Content as AppShell ?? new AppShell();
            shell.Language = ApplicationLanguages.Languages[0];
            Window.Current.Content = shell;

            if (shell.AppFrame.Content == null)
            {
                shell.AppFrame.Navigate(typeof(CustomerListPage), null,
                    new SuppressNavigationTransitionInfo());
            }

            Window.Current.Activate();
        }

        public static void UseSqlite()
        {
            string demoDatabasePath = Package.Current.InstalledLocation.Path + @"\Assets\Crm.db";
            string databasePath = ApplicationData.Current.LocalFolder.Path + @"\Crm.db";
            if (!File.Exists(databasePath))
            {
                File.Copy(demoDatabasePath, databasePath);
            }
            var dbOptions = new DbContextOptionsBuilder<CrmContext>().UseSqlite(
                "Veritabanı=" + databasePath);
            Repository = new SqlCrmRepository(dbOptions);
        }

        public static void UseRest() =>
            Repository = new RestCrmRepository("https://nozdemir-api-prod.azurewebsites.net/api/"); 
    }
}