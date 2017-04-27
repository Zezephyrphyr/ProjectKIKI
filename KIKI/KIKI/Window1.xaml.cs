using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KIKI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void MeetingClick(object sender, RoutedEventArgs e)
        {
            clickMeetingShowFile newWindow = new clickMeetingShowFile();
            newWindow.Show();
        }

        private void listView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void listView_SelectionChanged_1(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        public void fileClick(object sender, EventArgs e)
        {

            if (SystemParameters.SwapButtons) // Or use SystemInformation.MouseButtonsSwapped
            {
                // openfile
            }
            else
            {
               // clickFileShowMeeting newWindow = new clickFileShowMeeting();
                //newWindow.Show();
            }

        }
    }
}
