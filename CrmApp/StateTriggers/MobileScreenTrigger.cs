using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Crm.App.StateTriggers
{
    public class MobileScreenTrigger : StateTriggerBase
    {
        private UserInteractionMode _interactionMode;


        public MobileScreenTrigger()
        {
            Window.Current.SizeChanged += Window_SizeChanged;
        }

        private void Window_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            UpdateTrigger();
        }

        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            UpdateTrigger();
        }

        public UserInteractionMode InteractionMode
        {
            get { return _interactionMode; }
            set { _interactionMode = value; UpdateTrigger(); }
        }

        private void UpdateTrigger()
        {
            var _currentDeviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            var _currentInteractionMode = UIViewSettings.GetForCurrentView().UserInteractionMode;

            SetActive(InteractionMode == _currentInteractionMode && _currentDeviceFamily == "Windows.Mobile");
        }
    }
}
