using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;


namespace KIKI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.method();

            List<string> eventData = App.getBuffer();

            List<todayEvent> items = new List<todayEvent>();
            for (int i = 0; i < eventData.Count; i = i + 3)
            {
                items.Add(new todayEvent() { Time = eventData[i], Name = eventData[i+1], Attendee =  eventData[i+2]});
                mlistView.ItemsSource = items;

            }

        }

        private void listView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }

    public class todayEvent
    {
        public string Time { get; set; }

        public string Name { get; set; }

        public string Attendee { get; set; }
    }


}
