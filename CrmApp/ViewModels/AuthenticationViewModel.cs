using System;
using System.IO;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Graph;
using Windows.ApplicationModel.Core;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Crm.App.ViewModels
{
    public class AuthenticationViewModel : BindableBase
    {
        public AuthenticationViewModel()
        {
            Task.Run(PrepareAsync);
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += BuildAccountsPaneAsync;
        }

        private string _name;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _email;

        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        private string _title;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value); 
        }

        private string _domain;

        public string Domain
        {
            get => _domain;
            set => Set(ref _domain, value);
        }

        private BitmapImage _photo;

        public BitmapImage Photo
        {
            get => _photo;
            set => Set(ref _photo, value);
        }

        private string _errorText;

        public string ErrorText
        {
            get => _errorText;
            set => Set(ref _errorText, value);
        }

        private bool _showWelcome;

        public bool ShowWelcome
        {
            get => _showWelcome;
            set => Set(ref _showWelcome, value);
        }

        private bool _showLoading; 

        public bool ShowLoading
        {
            get => _showLoading;
            set => Set(ref _showLoading, value);
        }

        private bool _showData;

        public bool ShowData
        {
            get => _showData;
            set => Set(ref _showData, value); 
        }

        private bool _showError; 

        public bool ShowError
        {
            get => _showError;
            set => Set(ref _showError, value);
        }

        public async Task PrepareAsync()
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("IsLoggedIn") &&
                (bool)ApplicationData.Current.RoamingSettings.Values["IsLoggedIn"])
            {
                await SetVisibleAsync(vm => vm.ShowLoading);
                await LoginAsync();
            }
            else
            {
                await SetVisibleAsync(vm => vm.ShowWelcome);
            }
        }

        public async Task LoginAsync()
        {
            try
            {
                await SetVisibleAsync(vm => vm.ShowLoading);
                string token = await GetTokenAsync();
                if (token != null)
                {
                    ApplicationData.Current.RoamingSettings.Values["IsLoggedIn"] = true;
                    await SetUserInfoAsync(token);
                    await SetUserPhoto(token);
                    await SetVisibleAsync(vm => vm.ShowData);
                }
                else
                {
                    await SetVisibleAsync(vm => vm.ShowError);
                }
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                await SetVisibleAsync(vm => vm.ShowError);
            }
        }

        private async Task<string> GetTokenAsync()
        {
            var provider = await GetAadProviderAsync();
            var request = new WebTokenRequest(provider, "User.Read", 
                Repository.Constants.AccountClientId);
            request.Properties.Add("resource", "https://graph.microsoft.com");
            var result = await WebAuthenticationCoreManager.GetTokenSilentlyAsync(request);
            if (result.ResponseStatus != WebTokenRequestStatus.Success)
            {
                result = await WebAuthenticationCoreManager.RequestTokenAsync(request);
            }
            return result.ResponseStatus == WebTokenRequestStatus.Success ?
                result.ResponseData[0].Token : null;
        }

        private async Task SetUserInfoAsync(string token)
        {
            var users = await Windows.System.User.FindAllAsync();
            var graph = new GraphServiceClient(new DelegateAuthenticationProvider(message =>
            {
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return Task.CompletedTask;
            }));

            var me = await graph.Me.Request().GetAsync();

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
            {
                Name = me.DisplayName;
                Email = me.Mail;
                Title = me.JobTitle;
                Domain = (string)await users[0].GetPropertyAsync(KnownUserProperties.DomainName);
            });
        }

        private async Task SetUserPhoto(string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string url = "https://graph.microsoft.com/beta/me/photo/$value";
                var result = await client.GetAsync(url);
                if (!result.IsSuccessStatusCode)
                {
                    return;
                }
                using (Stream stream = await result.Content.ReadAsStreamAsync())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal, async () =>
                        {
                            Photo = new BitmapImage();
                            await Photo.SetSourceAsync(memoryStream.AsRandomAccessStream());
                        });
                    }
                }
            }
        }

        private async void BuildAccountsPaneAsync(AccountsSettingsPane sender,
            AccountsSettingsPaneCommandsRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();
            var command = new WebAccountProviderCommand(await GetAadProviderAsync(), async (x) =>
                await LoginAsync());
            args.WebAccountProviderCommands.Add(command);
            deferral.Complete();
        }

        public async Task<WebAccountProvider> GetAadProviderAsync() =>
            await WebAuthenticationCoreManager.FindAccountProviderAsync(
                "https://login.microsoft.com", "organizations");


        public async void LoginClick()
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("IsLoggedIn") &&
                (bool)ApplicationData.Current.RoamingSettings.Values["IsLoggedIn"])
            {
                await LoginAsync();
            }
            else
            {
                AccountsSettingsPane.Show();
            }
        }

        public async void LogoutClick()
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("IsLoggedIn") &&
                (bool)ApplicationData.Current.RoamingSettings.Values["IsLoggedIn"])
            {
                ContentDialog SignoutDialog = new ContentDialog()
                {
                    Title = "Sign out",
                    Content = "Sign out?",
                    PrimaryButtonText = "Sign out",
                    SecondaryButtonText = "Cancel"

                };
                await SignoutDialog.ShowAsync();
            }
        }

        private async Task SetVisibleAsync(Expression<Func<AuthenticationViewModel, bool>> selector)
        {
            var prop = (PropertyInfo)((MemberExpression)selector.Body).Member;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ShowWelcome = false;
                ShowLoading = false;
                ShowData = false;
                ShowError = false;
                prop.SetValue(this, true);
            });
        }
    }
}
