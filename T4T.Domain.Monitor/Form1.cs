using SKYPE4COMLib;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace T4T.Domain.Monitor
{
    public partial class Form1 : Form
    {
        private Skype _skype;

        private string _currentStatus = string.Empty;

        public Form1()
        {
            InitializeComponent();

            _skype = new Skype();

            _skype.Attach(Protocol: 7, Wait: true);

            _skype.OnlineStatus += OnlineStatusChanged;

            _currentStatus = this.ConvertStatusToString(_skype.CurrentUser.OnlineStatus);

            PostToService(_currentStatus);
        }

        private void OnlineStatusChanged(User pUser, TOnlineStatus Status)
        {
            var user = _skype.CurrentUser.OnlineStatus;
            _currentStatus = this.ConvertStatusToString(user);

            PostToService(_currentStatus);
        }

        private string ConvertStatusToString(TOnlineStatus status)
        {
            var resultStatus = string.Empty;
            switch (status)
            {
                case TOnlineStatus.olsOnline:
                    resultStatus = "Online";
                    break;
                case TOnlineStatus.olsOffline:
                    resultStatus = "Offline";
                    break;
                case TOnlineStatus.olsAway:
                    resultStatus = "Away";
                    break;
                case TOnlineStatus.olsDoNotDisturb:
                    resultStatus = "Do Not Disturb";
                    break;
                case TOnlineStatus.olsNotAvailable:
                    resultStatus = "Not Available";
                    break;
                default:
                    resultStatus = "Unknown";
                    break;
            }
            return resultStatus;
        }

        private async void PostToService(string status)
        {
            using (var http = new HttpClient())
            {
                var response = await http.PostAsync("http://t4t-skype.azurewebsites.net/api/skype", new StringContent("{\"Status\":\"" + status + "\",}", Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
