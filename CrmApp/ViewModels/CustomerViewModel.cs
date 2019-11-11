using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Crm.Models;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Crm.App.ViewModels
{
    public class CustomerViewModel : BindableBase, IEditableObject
    {
        public CustomerViewModel(Customer model = null) => Model = model ?? new Customer();

        private Customer _model;

        public Customer Model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;
                    RefreshOrders();

                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public string FirstName
        {
            get => Model.FirstName;
            set
            {
                if (value != Model.FirstName)
                {
                    Model.FirstName = value;
                    IsModified = true;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string LastName
        {
            get => Model.LastName;
            set
            {
                if (value != Model.LastName)
                {
                    Model.LastName = value;
                    IsModified = true;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Name => IsNewCustomer && string.IsNullOrEmpty(FirstName)
            && string.IsNullOrEmpty(LastName) ? "New customer" : $"{FirstName} {LastName}";

        public string Address
        {
            get => Model.Address;
            set
            {
                if (value != Model.Address)
                {
                    Model.Address = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }

        public string Company
        {
            get => Model.Company;
            set
            {
                if (value != Model.Company)
                {
                    Model.Company = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }

        public string Phone
        {
            get => Model.Phone;
            set
            {
                if (value != Model.Phone)
                {
                    Model.Phone = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }

        public string Email
        {
            get => Model.Email;
            set
            {
                if (value != Model.Email)
                {
                    Model.Email = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsModified { get; set; }

        public ObservableCollection<Order> Orders { get; } = new ObservableCollection<Order>();

        private Order _selectedOrder;

        public Order SelectedOrder
        {
            get => _selectedOrder;
            set => Set(ref _selectedOrder, value);
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }

        private bool _isNewCustomer;

        public bool IsNewCustomer
        {
            get => _isNewCustomer;
            set => Set(ref _isNewCustomer, value);
        }

        private bool _isInEdit = false;

        public bool IsInEdit
        {
            get => _isInEdit;
            set => Set(ref _isInEdit, value);
        }

        public async Task SaveAsync()
        {
            IsInEdit = false;
            IsModified = false;
            if (IsNewCustomer)
            {
                IsNewCustomer = false;
                App.ViewModel.Customers.Add(this);
            }

            await App.Repository.Customers.UpsertAsync(Model);
        }

        public event EventHandler AddNewCustomerCanceled;

        public async Task CancelEditsAsync()
        {
            if (IsNewCustomer)
            {
                AddNewCustomerCanceled?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                await RevertChangesAsync();
            }
        }

        public async Task RevertChangesAsync()
        {
            IsInEdit = false;
            if (IsModified)
            {
                await RefreshCustomerAsync();
                IsModified = false;
            }
        }

        public void StartEdit() => IsInEdit = true;

        public async Task RefreshCustomerAsync()
        {
            RefreshOrders();
            Model = await App.Repository.Customers.GetAsync(Model.Id);
        }

        public void RefreshOrders() => Task.Run(LoadOrdersAsync);

        public async Task LoadOrdersAsync()
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                IsLoading = true;
            });

            var orders = await App.Repository.Orders.GetForCustomerAsync(Model.Id);

            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                Orders.Clear();
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }

                IsLoading = false;
            });
        }

        public void BeginEdit()
        {
           
        }

        public async void CancelEdit() => await CancelEditsAsync();

        public async void EndEdit() => await SaveAsync();
    }
}