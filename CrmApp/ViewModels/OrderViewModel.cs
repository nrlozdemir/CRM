using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Crm.Models;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Crm.App.ViewModels
{
    public class OrderViewModel : BindableBase
    {
        public OrderViewModel(Order model)
        {
            Model = model;

            LineItems = new ObservableCollection<LineItem>(Model.LineItems);
            LineItems.CollectionChanged += LineItems_Changed;

            NewLineItem = new LineItemViewModel();

            if (model.Customer == null)
            {
                Task.Run(() => LoadCustomer(Model.CustomerId));
            }
        }

        public async static Task<OrderViewModel> CreateFromGuid(Guid orderId) =>
            new OrderViewModel(await GetOrder(orderId));

        public Order Model { get; }

        private async void LoadCustomer(Guid customerId)
        {
            var customer = await App.Repository.Customers.GetAsync(customerId);
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                Customer = customer;
            });
        }

        private static async Task<Order> GetOrder(Guid orderId) =>
            await App.Repository.Orders.GetAsync(orderId); 

        public bool CanRefresh => Model != null && !IsModified && IsExistingOrder;

        public bool CanRevert => Model != null && IsModified && IsExistingOrder;

        public Guid Id
        {
            get => Model.Id; 
            set
            {
                if (Model.Id != value)
                {
                    Model.Id = value;
                    OnPropertyChanged();
                    IsModified = true;
                }
            }
        }

        bool _IsModified = false;

        public bool IsModified
        {
            get => _IsModified; 
            set
            {
                if (value != _IsModified)
                {
                    if (IsLoaded)
                    {
                        _IsModified = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        public bool IsExistingOrder => !IsNewOrder;

        public bool IsLoaded => Model != null && (IsNewOrder || Model.Customer != null);

        public bool IsNotLoaded => !IsLoaded;

        public bool IsNewOrder => Model.InvoiceNumber == 0; 

        public int InvoiceNumber
        {
            get => Model.InvoiceNumber;
            set
            {
                if (Model.InvoiceNumber != value)
                {
                    Model.InvoiceNumber = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsNewOrder));
                    OnPropertyChanged(nameof(IsLoaded));
                    OnPropertyChanged(nameof(IsNotLoaded));
                    OnPropertyChanged(nameof(IsNewOrder));
                    OnPropertyChanged(nameof(IsExistingOrder));
                    IsModified = true;
                }
            }
        }

        public Customer Customer
        {
            get => Model.Customer;
            set
            {
                if (Model.Customer != value)
                {
                    var isLoadingOperation = Model.Customer == null &&
                        value != null && !IsNewOrder;
                    Model.Customer = value;
                    OnPropertyChanged();
                    if (isLoadingOperation)
                    {
                        OnPropertyChanged(nameof(IsLoaded));
                        OnPropertyChanged(nameof(IsNotLoaded));
                    }
                    else
                    {
                        IsModified = true;
                    }
                }
            }
        }

        private ObservableCollection<LineItem> _lineItems;
        
        public ObservableCollection<LineItem> LineItems
        {
            get => _lineItems; 
            set
            {
                if (_lineItems != value)
                {
                    if (value != null)
                    {
                        value.CollectionChanged += LineItems_Changed;
                    }

                    if (_lineItems != null)
                    {
                        _lineItems.CollectionChanged -= LineItems_Changed;
                    }
                    _lineItems = value;
                    OnPropertyChanged();
                    IsModified = true;
                }
            }
        }

        private void LineItems_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (LineItems != null)
            {
                Model.LineItems = LineItems.ToList();
            }

            OnPropertyChanged(nameof(LineItems));
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(Tax));
            OnPropertyChanged(nameof(GrandTotal));
            IsModified = true;
        }

        private LineItemViewModel _newLineItem;

        public LineItemViewModel NewLineItem
        {
            get => _newLineItem; 
            set
            {
                if (value != _newLineItem)
                {
                    if (value != null)
                    {
                        value.PropertyChanged += NewLineItem_PropertyChanged;
                    }

                    if (_newLineItem != null)
                    {
                        _newLineItem.PropertyChanged -= NewLineItem_PropertyChanged;
                    }

                    _newLineItem = value;
                    UpdateNewLineItemBindings();
                }
            }
        }

        private void NewLineItem_PropertyChanged(object sender, PropertyChangedEventArgs e) => UpdateNewLineItemBindings();

        private void UpdateNewLineItemBindings()
        {
            OnPropertyChanged(nameof(NewLineItem));
            OnPropertyChanged(nameof(HasNewLineItem));
            OnPropertyChanged(nameof(NewLineItemProductListPriceFormatted));
        }

        public bool HasNewLineItem => NewLineItem != null && NewLineItem.Product != null;

        public string NewLineItemProductListPriceFormatted => (NewLineItem?.Product?.ListPrice ?? 0).ToString("c");

        public DateTime DatePlaced
        {
            get => Model.DatePlaced;
            set
            {
                if (Model.DatePlaced != value)
                {
                    Model.DatePlaced = value;
                    OnPropertyChanged();
                    IsModified = true;
                }
            }
        }

        public DateTime? DateFilled
        {
            get => Model.DateFilled;
            set
            {
                if (value != Model.DateFilled)
                {
                    Model.DateFilled = value;
                    OnPropertyChanged();
                    IsModified = true;
                }
            }
        }

        public decimal Subtotal => Model.Subtotal;

        public decimal Tax => Model.Tax;

        public decimal GrandTotal => Model.GrandTotal;

        public string Address
        {
            get => Model.Address; 
            set
            {
                if (Model.Address != value)
                {
                    Model.Address = value;
                    OnPropertyChanged();
                    IsModified = true;
                }
            }
        }

        public List<string> PaymentStatusValues => Enum.GetNames(typeof(PaymentStatus)).ToList();

        public void SetPaymentStatus(object value) => PaymentStatus = value == null ?
            PaymentStatus.Odenmedi : (PaymentStatus)Enum.Parse(typeof(PaymentStatus), value as string);

        public PaymentStatus PaymentStatus
        {
            get => Model.PaymentStatus;
            set
            {
                if (Model.PaymentStatus != value)
                {
                    Model.PaymentStatus = value;
                    OnPropertyChanged();
                    IsModified = true;
                }
            }
        }
        public List<string> TermValues => Enum.GetNames(typeof(Term)).ToList();

        public Term SetTerm(object value) => Term = value == null ?
            Term.Vade : (Term)Enum.Parse(typeof(Term), value as string);

        public Term Term
        {
            get => Model.Term;
            set
            {
                if (Model.Term != value)
                {
                    Model.Term = value;
                    OnPropertyChanged();
                    IsModified = true;
                }
            }
        }

        public List<string> OrderStatusValues => Enum.GetNames(typeof(OrderStatus)).ToList();

        public OrderStatus SetOrderStatus(object value) => OrderStatus = value == null ? 
            OrderStatus.Acik : (OrderStatus)Enum.Parse(typeof(OrderStatus), value as string);

        public OrderStatus OrderStatus
        {
            get => Model.Status;
            set
            {
                if (Model.Status != value)
                {
                    Model.Status = value;
                    OnPropertyChanged();

                    // Update the DateFilled value.
                    DateFilled = Model.Status == OrderStatus.Islemde ? (DateTime?)DateTime.Now : null;
                    IsModified = true;
                }
            }
        }

        public string CustomerName
        {
            get => Model.CustomerName;
            set
            {
                if (Model.CustomerName != value)
                {
                    Model.CustomerName = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task SaveOrderAsync()
        {
            Order result = null;
            try
            {
                result = await App.Repository.Orders.UpsertAsync(Model);
            }
            catch (Exception ex)
            {
                throw new OrderSavingException("Hata:", ex);
            }

            if (result != null)
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => IsModified = false);
            }
            else
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => new OrderSavingException(
                    "Kaydedilemedi"));
            }
        }

        public ObservableCollection<Product> ProductSuggestions { get; } = new ObservableCollection<Product>();

        public async void UpdateProductSuggestions(string queryText)
        {
            ProductSuggestions.Clear();

            if (!string.IsNullOrEmpty(queryText))
            {
                var suggestions = await App.Repository.Products.GetAsync(queryText);

                foreach (Product p in suggestions)
                {
                    ProductSuggestions.Add(p);
                }
            }
        }
    }
}
