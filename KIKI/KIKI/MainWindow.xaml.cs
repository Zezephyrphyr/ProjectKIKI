using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Windows.Media;
using Hardcodet.Wpf.TaskbarNotification;
using System.Reflection;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Collections.ObjectModel;

namespace KIKI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool login = true;

        public MainWindow()
        {
            InitializeComponent();
            App.Initialize();
            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/Resources/kiki.ico");
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = System.Windows.WindowState.Normal;
                };
            initializeInfo();
    }
       
        private void initializeInfo()
        {
            List<string> eventData = App.getBuffer();
            ObservableCollection<todayEvent> items = new ObservableCollection<todayEvent>();

            for (int i = 0; i < eventData.Count; i = i + 3)
            {
                items.Add(new todayEvent() { Date = eventData[i], Time = eventData[i], Name = eventData[i + 1], Attendee = eventData[i + 2] });
                mlistView.ItemsSource = items;
                mlistView4.ItemsSource = items;
                var template = mlistView4.ItemTemplate;

            }
        }

        private void listView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        public void OnClick1(object sender, RoutedEventArgs e)
        {
            if (login == true)
            {
                App.revoke();
                App.Clean();
                ObservableCollection<todayEvent> items2 = new ObservableCollection<todayEvent>();
                mlistView.ItemsSource = items2;
                mlistView4.ItemsSource = null;
                login = false;
            }
            else
            {
                App.Initialize();
                initializeInfo();
                login = true; 

            }   
        }

        private void listView_SelectionChanged_1(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void textBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void tabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();
            base.OnStateChanged(e);
        }
    }

    public class todayEvent
    {
        public string Time { get; set; }
        public string Name { get; set; }
        public string Attendee { get; set; }
        public string Date { get; set; }
    }

}
