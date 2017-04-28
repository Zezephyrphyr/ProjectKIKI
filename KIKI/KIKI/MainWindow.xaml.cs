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
using KIKIXmlProcessor;

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
            initializeGoogleInfo();
            
            initializeMeetingInfo();
            initializeFileInfo();
    }
       
        private void initializeGoogleInfo()
        {
           
            List<string> eventData = App.getGoogleBuffer();
            ObservableCollection<todayEvent> items = new ObservableCollection<todayEvent>();
            
            for (int i = 0; i < eventData.Count; i = i + 3)
            {
                items.Add(new todayEvent() { Date = eventData[i], Time = eventData[i], Name = eventData[i + 1], Attendee = eventData[i + 2] });
                mlistView.ItemsSource = items;

            }

        }
        private void initializeMeetingInfo()
        { 
            List<string> meetingData = App.getMeetingBuffer();
            ObservableCollection<previousMeeting> items = new ObservableCollection<previousMeeting>();
            Debug.Print(""+ meetingData.Count);
            for (int i = 0; i < meetingData.Count; i = i + 4)
            {
                items.Add(new previousMeeting() { Time = meetingData[i],  Name = meetingData[i+1], Attendee = meetingData[i + 2], Docs = meetingData[i + 3]});
                
                mlistView4.ItemsSource = items;
          
            }

        }
        private void initializeFileInfo()
        {
            XMLProcessor processor = new XMLProcessor();
            List<string> FileData = App.getFileBuffer();
            LinkedList<MeetingNode> MeetingData = new LinkedList<MeetingNode>();
            ObservableCollection<recentFile> items = new ObservableCollection<recentFile>();
            for (int i = 0; i < FileData.Count; i = i + 3)
            {
                items.Add(new recentFile() { Name = FileData[i], URL = FileData[i+1], Meetings = FileData[i+2]});
                RecentFile.ItemsSource = items;

                
                MeetingData = processor.FindMeetingsByMeetingIDs(FileData[i + 2]);
                Debug.Print(MeetingData+"");
                if (MeetingData.Count != 0)
                {
                    foreach (MeetingNode item in MeetingData)
                    {
                        items.Add(new recentFile() { Time = item.GetStartTimeS(), Title = item.GetMeetingTitle(), Attendee = item.GetAttendents() });
                        RecentFile.ItemsSource = items;

                    }
                }else
                {
                    items.Add(new recentFile() { Time = "No Records", Title = "  ", Attendee = "    " });
                    RecentFile.ItemsSource = items;
                }
                

            }
        }


        public void searchClick(object sender, RoutedEventArgs e)
        {
            Window1 newWindow = new Window1();
            newWindow.Show();
        }

        public void loginClick(object sender, RoutedEventArgs e)
        {
            if (login == true)
            {
                App.revoke();
                App.Clean();
                ObservableCollection<todayEvent> items2 = new ObservableCollection<todayEvent>();
                mlistView.ItemsSource = items2;
                mlistView4.ItemsSource = null;
                login = false;
                loginButton.Content = "Log In";
            }
            else
            {
                App.Initialize();
                initializeGoogleInfo();
                login = true;
                loginButton.Content = "Log Out";

            }   
        }

        public void fileClick(object sender, EventArgs e)
        {
            
            if (SystemParameters.SwapButtons) // Or use SystemInformation.MouseButtonsSwapped
            {
                // openfile
            }
            else
            {
                string str = sender.ToString();
                str = str.Substring(str.LastIndexOf(' ') + 1);
                clickShowFiles newWindow = new clickShowFiles(str);
                Debug.Print("" + str);
                newWindow.Show();
            }
         
        }
        
        private void MeetingClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;

           // Debug.Print("" + button.DataContext);
            //clickShowFiles newWindow = new clickShowFiles();
            //newWindow.Show();
        }

        private void listView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
          
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

        private void Hyperlink_RequestNavigate(object sender,
                                     System.Windows.Navigation.RequestNavigateEventArgs e)
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

    public class previousMeeting
    {
        public string Time { get; set; }
        public string Name { get; set; }
        public string Attendee { get; set;}
        public string Docs { get; set; }
    }

    public class recentFile
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public string Time { get; set; }
        public string Title { get; set; }
        public string Attendee { get; set; }
        public string Meetings { get; set; }
    }
    
    public class searchFile
    {
        public string Time { get; set; }
        public string Title { get; set; }
        public string Attendee { get; set; }
    }

    public class searchMeeting
    {
        public string Time { get; set; }
        public string Name { get; set; }
        public string Attendee { get; set; }
        public string Docs { get; set; }
    }

    public class clickMeeting {
        public string Name { get; set; }
        public string createdTime { get; set; }
        public string lastModified { get; set; }
    }



}
