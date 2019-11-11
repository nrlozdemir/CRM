using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Crm.Models;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Crm.App.ViewModels
{
    public class OrderListPageViewModel : BindableBase
    {
        public OrderListPageViewModel() => IsLoading = false;

        private List<Order> MasterOrdersList { get; } = new List<Order>();

        public ObservableCollection<Order> Orders { get; private set; } = new ObservableCollection<Order>();

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value); 
        }

        private Order _selectedOrder;

        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (Set(ref _selectedOrder, value))
                {
                    SelectedCustomer = null;
                    if (_selectedOrder != null)
                    {
                        Task.Run(() => LoadCustomer(_selectedOrder.CustomerId));
                    }
                    OnPropertyChanged(nameof(SelectedOrderGrandTotalFormatted));
                }
            }
        }

        public string SelectedOrderGrandTotalFormatted => (SelectedOrder?.GrandTotal ?? 0).ToString("c");

        private Customer _selectedCustomer;

        public Customer SelectedCustomer
        {
            get => _selectedCustomer; 
            set => Set(ref _selectedCustomer, value);
        }

        private async void LoadCustomer(Guid customerId)
        {
            var customer = await App.Repository.Customers.GetAsync(customerId);
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                SelectedCustomer = customer;
            });
        }
        public async void LoadOrders()
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                IsLoading = true;
                Orders.Clear();
                MasterOrdersList.Clear();
            });

            var orders = await Task.Run(App.Repository.Orders.GetAsync);

            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                foreach (var order in orders)
                {
                    Orders.Add(order);
                    MasterOrdersList.Add(order);
                }

                IsLoading = false;
            });
        }

        public async void QueryOrders(string query)
        {
            IsLoading = true;
            Orders.Clear();
            if (!string.IsNullOrEmpty(query))
            {
                var results = await App.Repository.Orders.GetAsync(query);
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    foreach (Order o in results)
                    {
                        Orders.Add(o);
                    }
                    IsLoading = false;
                });
            }
        }

        public async Task DeleteOrder(Order orderToDelete) =>
            await App.Repository.Orders.DeleteAsync(orderToDelete.Id);

        public ObservableCollection<Order> OrderSuggestions { get; } = new ObservableCollection<Order>();

        public void UpdateOrderSuggestions(string queryText)
        {
            OrderSuggestions.Clear();
            if (!string.IsNullOrEmpty(queryText))
            {
                string[] parameters = queryText.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries);

                var resultList = MasterOrdersList
                    .Where(order => parameters
                        .Any(parameter =>
                            order.Address.StartsWith(parameter) ||
                            order.CustomerName.Contains(parameter) ||
                            order.InvoiceNumber.ToString().StartsWith(parameter)))
                    .OrderByDescending(order => parameters
                        .Count(parameter =>
                            order.Address.StartsWith(parameter) ||
                            order.CustomerName.Contains(parameter) ||
                            order.InvoiceNumber.ToString().StartsWith(parameter)));

                foreach (Order order in resultList)
                {
                    OrderSuggestions.Add(order);
                }
            }
        }
    }
}