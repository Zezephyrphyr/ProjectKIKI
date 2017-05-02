using KIKIXmlProcessor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace KIKI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private XMLProcessor processor;
        private XMLSearcher searcher;

        // Window for searching
        public Window1()
        {
            InitializeComponent();
        }

        // handle search button events
        private void Search(object sender, RoutedEventArgs e)
        {
            processor = new XMLProcessor(App.id);
            searcher = new XMLSearcher(processor.GetWorkingPath(), App.id);
            string keyword = textBox.Text;

            if (tabControl.SelectedIndex == 0)
            {
                LinkedList<FileNode> fileList = searcher.FindFilesByFileNameKeywords(keyword);
                if (fileList.Count != 0)
                {
                    initializeFileInfo(fileList);
                    Debug.Print(tabControl.SelectedIndex.ToString());
                }
                else
                {
                    MessageBox.Show("No Results");
                }
            }
            else if (tabControl.SelectedIndex == 1)
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

        // handle events for clicking meeting
        private void MeetingClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            if (button.Tag.ToString() != "")
            {
                clickShowFiles newWindow = new clickShowFiles(button.Tag.ToString());
                newWindow.Show();
            }
            else
            {
                MessageBox.Show("No file modified in this meeting.");
            }
        }

        // handle events for clicking hyperlink
        private void Hyperlink_RequestNavigate(object sender,
                                 System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(e.Uri.AbsoluteUri);
            }
            catch
            {
                MessageBox.Show("The file may be removed or moved to another path.");
            }
        }

        // initialize file tab after searching
        private void initializeFileInfo(LinkedList<FileNode> fileList)
        {
            Debug.Print(fileList.Count + "");
            processor = new XMLProcessor(App.id);
            searcher = new XMLSearcher(processor.GetWorkingPath(), App.id);
            LinkedList<MeetingNode> MeetingData = new LinkedList<MeetingNode>();
            ObservableCollection<recentFile> items = new ObservableCollection<recentFile>();
            foreach (FileNode item in fileList)
            {
                items.Add(new recentFile() { Name = item.GetFileName(), URL = item.GetFilePath(), Meetings = item.GetMeetingListS() });
                RecentFile.ItemsSource = items;
                MeetingData = searcher.FindMeetingsByMeetingIDs(item.GetMeetingListS());

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

        // initialize meeting tab after searching
        private void initializeMeetingInfo(LinkedList<MeetingNode> meetingList)
        {
            processor = new XMLProcessor(App.id);
            searcher = new XMLSearcher(processor.GetWorkingPath(), App.id);
            ObservableCollection<previousMeeting> items = new ObservableCollection<previousMeeting>();
            foreach (MeetingNode item in meetingList)
            {
                items.Add(new previousMeeting() { Time = item.GetStartTime().ToString(), Name = item.GetMeetingTitle(), Attendee = item.GetAttendents(), Docs = item.GetFileListS() });
                mlistView5.ItemsSource = items;
            }
        }

        // initialize event for clicking file title
        public void fileClick(object sender, EventArgs e)
        {

            if (SystemParameters.SwapButtons) // Or use SystemInformation.MouseButtonsSwapped
            { }
            else
            {
                string str = sender.ToString();
                str = str.Substring(str.LastIndexOf(' ') + 1);
                if (str != "System.Windows.Controls.Button")
                {
                    clickShowFiles newWindow = new clickShowFiles(str);
                    newWindow.Show();
                }
                else
                {
                    MessageBox.Show("No file modified in this meeting.");
                }
            }
        }
    }

}
