using KIKIXmlProcessor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    public partial class Window1 : Window { 
        private XMLProcessor processor;
        private XMLSearcher searcher;

    
        public Window1()
        {
            InitializeComponent();
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            processor = new XMLProcessor();
            searcher = new XMLSearcher(processor.GetWorkingPath());
            string keyword = textBox.Text;
            
            if (tabControl.SelectedIndex == 0)
            {
                LinkedList<FileNode> fileList = searcher.FindFilesByFileNameKeywords(keyword);
                if (fileList.Count != 0) { 
                    initializeFileInfo(fileList);
                    Debug.Print(tabControl.SelectedIndex.ToString());
                }else
                {
                    MessageBox.Show("No Results");
                }
            }else if( tabControl.SelectedIndex == 1)
            {
                LinkedList<MeetingNode> meetingList = searcher.FindMeetingsByMeetingTitleKeywords(keyword);
                if (meetingList.Count != 0)
                {
                    initializeMeetingInfo(meetingList);
                    Debug.Print(tabControl.SelectedIndex.ToString());
                }
                else
                {
                    MessageBox.Show("No Results");
                }
            }
            

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

        private void Hyperlink_RequestNavigate(object sender,
                                 System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(e.Uri.AbsoluteUri);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("The file cannot be found.");
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
               // clickFileShowMeeting newWindow = new clickFileShowMeeting();
                //newWindow.Show();
            }

        }

        private void initializeFileInfo(LinkedList<FileNode> fileList)
        {
            Debug.Print(fileList.Count+"");
            processor = new XMLProcessor();
            searcher = new XMLSearcher(processor.GetWorkingPath());
            LinkedList<MeetingNode> MeetingData = new LinkedList<MeetingNode>();
            ObservableCollection<recentFile> items = new ObservableCollection<recentFile>();
            foreach (FileNode item in fileList)
            {

                items.Add(new recentFile() { Name = item.GetFileName(), URL = item.GetFilePath(), Meetings = item.GetMeetingListS() });
                RecentFile.ItemsSource = items;
                MeetingData = searcher.FindMeetingsByMeetingIDs(item.GetMeetingListS());
                Debug.Print(MeetingData + "");

           
            if (MeetingData.Count != 0)
            {
                foreach (MeetingNode meeting in MeetingData)
                {
                    items.Add(new recentFile() { Time = meeting.GetStartTimeS(), Title = meeting.GetMeetingTitle(), Attendee = meeting.GetAttendents(), Files = meeting.GetFileListS() });
                    RecentFile.ItemsSource = items;

                }
            }
            else
            {
                items.Add(new recentFile() { Time = "No Records", Title = "  ", Attendee = "    " });
                RecentFile.ItemsSource = items;
            }

            }


        }

        private void initializeMeetingInfo(LinkedList<MeetingNode> meetingList)
        {
            processor = new XMLProcessor();
            searcher = new XMLSearcher(processor.GetWorkingPath());
            ObservableCollection<previousMeeting> items = new ObservableCollection<previousMeeting>();
            foreach (MeetingNode item in meetingList)
            {
                items.Add(new previousMeeting() { Time = item.GetStartTime().ToString(), Name = item.GetMeetingTitle(), Attendee = item.GetAttendents(), Docs = item.GetFileListS()});
                

            }
           

           
        }

    }

}
