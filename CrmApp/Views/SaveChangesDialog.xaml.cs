using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Crm.App.Views
{
    public sealed partial class SaveChangesDialog : ContentDialog
    {
        public SaveChangesDialog()
        {
            InitializeComponent();
        }

        public SaveChangesDialogResult Result { get; set; } = SaveChangesDialogResult.Cancel;

        public string Message { get; set; } = "Henüz kaydedilmemiş değişiklikleriniz var. " + 
            "Değişiklikleri kaydetmek istiyor musunuz?"; 

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = SaveChangesDialogResult.Save;
            Hide();
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            Result = SaveChangesDialogResult.DontSave;
            Hide();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = SaveChangesDialogResult.Cancel;
            Hide();
        }
    }

    public enum SaveChangesDialogResult
    {
        Save,
        DontSave,
        Cancel
    }
}
