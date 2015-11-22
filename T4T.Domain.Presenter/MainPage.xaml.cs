using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace T4T.Domain.Presenter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        //Global Variables
        private GpioPin _pin;

        //PI LED Pin Configuration
        private const int _pinNumber = 27;

        //GPIO Constants
        private GpioPinValue LO = GpioPinValue.Low;
        private GpioPinValue HI = GpioPinValue.High;

        //UI Features
        private string _myStatus = string.Empty;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private SolidColorBrush yellowBrush = new SolidColorBrush(Windows.UI.Colors.Yellow);

        private DispatcherTimer timer1;
        private DispatcherTimer timer2;

        public MainPage()
        {
            this.InitializeComponent();

            //setup Timer 1 (Poller for Rest Service)
            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(5000);
            timer1.Tick += Timer_Tick;

            //Setup Timer 2 (UI Refresher)
            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromMilliseconds(500);
            timer2.Tick += Timer2_Tick;

            InitGPIO();

            timer1.Start();
            timer2.Start();
        }

        private void Timer2_Tick(object sender, object e)
        {
            //update UI
            Status.Text = _myStatus;

            switch (_myStatus)
            {
                case "Online":
                    updateLight(LO, greenBrush);
                    break;
                case "Away":
                    updateLight(HI, yellowBrush);
                    break;
                case "Do Not Disturb":
                    updateLight(HI, redBrush);
                    break;
                default:
                    updateLight(LO, grayBrush);
                    break;
            }
        }

        private void updateLight(GpioPinValue pinvalue, SolidColorBrush currentColor)
        {
            if (_pin != null)
            {
                _pin.Write(pinvalue);
            }
            StatusColor.Fill = currentColor;
        }

        private async void Timer_Tick(object sender, object e)
        {
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.IfModifiedSince = DateTime.UtcNow;
                var response = await http.GetAsync(new Uri("http://t4t-skype.azurewebsites.net/api/skype"));
                if (response.StatusCode != HttpStatusCode.Ok)
                {
                    _myStatus = response.StatusCode.ToString();
                    return;
                }

                _myStatus = await response.Content.ReadAsStringAsync();
            }
        }

        private void InitGPIO()
        {
            //setup the Windows 10 GPIO COntroller
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                _pin = null;
                Status.Text = "There is no GPIO controller on this device.";
                return;
            }

            _pin = gpio.OpenPin(_pinNumber);

            _pin.Write(GpioPinValue.Low);

            _pin.SetDriveMode(GpioPinDriveMode.Output);
        }
    }
}
