using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Gabriel.Cat.S.Check;
using CheckPuya.Net;
using System.IO;

namespace CheckPuya.Xamarin
{
    public partial class MainPage : ContentPage
    {
        public const string VERSION = "1.0";
        public Check Check { get; set; }
        public Cancelation Cancelation { get; set; }
        public Task TaskPublish { get; set; }
        public List<string> ListLog { get; set; }
        public MainPage()
        {


            InitializeComponent();
            ListLog = new List<string>();
            lstLog.ItemsSource = ListLog;
            Cancelation = new Cancelation();
            Check = new Check();
            Check.Log = (s) =>
            {
                Dispatcher.BeginInvokeOnMainThread(new Action(() => ListLog.Add(s)));

            };
            Check.Load(new string[] { Properties.Resources.Web,
                                      Properties.Resources.Channel,
                                      Properties.Resources.KeyApi
                });
        }
        public async Task Start()
        {
            Check.Log($"CheckPuya V{VERSION} Telegram Bot!");
            Cancelation.Continue = true;
            Check.Publicar((web) => Capitulo.GetCapitulos(web), cancelation: Cancelation);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Start();
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Cancelation.Continue = false;
        }
    }
}
