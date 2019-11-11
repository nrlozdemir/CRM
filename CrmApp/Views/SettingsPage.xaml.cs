using Crm.Repository.Sql;
using System;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Crm.App.Views
{
    public sealed partial class SettingsPage : Page
    {
        public const string DataSourceKey = "data_source"; 

        public SettingsPage()
        {
            InitializeComponent();

            if (App.Repository.GetType() == typeof(SqlCrmRepository))
            {
                SqliteRadio.IsChecked = true;
            }
            else
            {
                RestRadio.IsChecked = true; 
            }
        }

        private void OnDataSourceChanged(object sender, RoutedEventArgs e)
        {
            var radio = (RadioButton)sender; 
            switch (radio.Tag)
            {
                case "Lokal Sqlite Sunucusu": App.UseSqlite(); break;
                case "Azure Rest API": App.UseRest(); break;
                default: throw new NotImplementedException(); 
            }
            ApplicationData.Current.LocalSettings.Values[DataSourceKey] = radio.Tag; 
        }
    }
}
