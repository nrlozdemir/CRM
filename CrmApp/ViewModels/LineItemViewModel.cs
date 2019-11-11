using Crm.Models;

namespace Crm.App.ViewModels
{
    public class LineItemViewModel : BindableBase
    {
        public LineItemViewModel(LineItem model = null) => Model = model ?? new LineItem();

        public LineItem Model { get; }

        public Product Product
        {
            get => Model.Product;
            set
            {
                if (Model.Product != value)
                {
                    Model.Product = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Quantity
        {
            get => Model.Quantity;
            set
            {
                if (Model.Quantity != value)
                {
                    Model.Quantity = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
