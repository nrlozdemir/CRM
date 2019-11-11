using System;
using System.Linq;
using Crm.App.ViewModels;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Crm.App.Views
{
    public sealed partial class CustomerListPage : Page
    {
        public CustomerListPage()
        {
            InitializeComponent();
            Window.Current.SizeChanged += CurrentWindow_SizeChanged;
        }

        public MainViewModel ViewModel => App.ViewModel;

        private void CurrentWindow_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile" && 
                e.Size.Width >= (double)App.Current.Resources["MediumWindowSnapPoint"])
            {
                mainCommandBar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
            }
            else
            {
                mainCommandBar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom;
            }
        }

        private void CustomerSearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (CustomerSearchBox != null)
            {
                CustomerSearchBox.AutoSuggestBox.QuerySubmitted += CustomerSearchBox_QuerySubmitted;
                CustomerSearchBox.AutoSuggestBox.TextChanged += CustomerSearchBox_TextChanged;
                CustomerSearchBox.AutoSuggestBox.PlaceholderText = "Müşteri ara";
            }
        }

        private async void CustomerSearchBox_TextChanged(AutoSuggestBox sender,
            AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (String.IsNullOrEmpty(sender.Text))
                {
                    await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                        await ViewModel.GetCustomerListAsync());
                    sender.ItemsSource = null;
                }
                else
                {
                    string[] parameters = sender.Text.Split(new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries);
                    sender.ItemsSource = ViewModel.Customers
                        .Where(customer => parameters.Any(parameter =>
                            customer.Address.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                            customer.FirstName.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                            customer.LastName.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                            customer.Company.StartsWith(parameter, StringComparison.OrdinalIgnoreCase)))
                        .OrderByDescending(customer => parameters.Count(parameter =>
                            customer.Address.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                            customer.FirstName.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                            customer.LastName.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                            customer.Company.StartsWith(parameter, StringComparison.OrdinalIgnoreCase)))
                        .Select(customer => $"{customer.FirstName} {customer.LastName}"); 
                }
            }
        }

        private async void CustomerSearchBox_QuerySubmitted(AutoSuggestBox sender,
            AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (String.IsNullOrEmpty(args.QueryText))
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(async () => 
                    await ViewModel.GetCustomerListAsync());
            }
            else
            {
                string[] parameters = sender.Text.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries);

                var matches = ViewModel.Customers.Where(customer => parameters
                    .Any(parameter =>
                        customer.Address.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                        customer.FirstName.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                        customer.LastName.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                        customer.Company.StartsWith(parameter, StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(customer => parameters.Count(parameter =>
                        customer.Address.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                        customer.FirstName.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                        customer.LastName.StartsWith(parameter, StringComparison.OrdinalIgnoreCase) ||
                        customer.Company.StartsWith(parameter, StringComparison.OrdinalIgnoreCase)))
                    .ToList(); 

                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    ViewModel.Customers.Clear(); 
                    foreach (var match in matches)
                    {
                        ViewModel.Customers.Add(match); 
                    }
                });
            }
        }

        private void CommandBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                (sender as CommandBar).DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom;
            }
            else
            {
                (sender as CommandBar).DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
            }
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedCustomer != null)
            {
                Frame.Navigate(typeof(CustomerDetailPage), ViewModel.SelectedCustomer.Model.Id,
                    new DrillInNavigationTransitionInfo());
            }
        }

        private void CreateCustomer_Click(object sender, RoutedEventArgs e) =>
            Frame.Navigate(typeof(CustomerDetailPage), null, new DrillInNavigationTransitionInfo());

        private void DataGrid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape &&
                ViewModel.SelectedCustomer != null &&
                ViewModel.SelectedCustomer.IsModified &&
                !ViewModel.SelectedCustomer.IsInEdit)
            {
                (sender as DataGrid).CancelEdit(DataGridEditingUnit.Row);
            }
        }

        private void DataGrid_RightTapped(object sender, RightTappedRoutedEventArgs e) =>
            ViewModel.SelectedCustomer = (e.OriginalSource as FrameworkElement).DataContext as CustomerViewModel;

        private void AddOrder_Click(object sender, RoutedEventArgs e) =>
            Frame.Navigate(typeof(OrderDetailPage), ViewModel.SelectedCustomer.Model.Id);

        private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e) =>
            (sender as DataGrid).Sort(e.Column, ViewModel.Customers.Sort);
    }
}
