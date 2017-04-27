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
using KIKIXmlProcessor;
using System.Collections.ObjectModel;

namespace KIKI
{
    /// <summary>
    /// Interaction logic for clickFileShowMeeting.xaml
    /// </summary>
    public partial class clickFileShowMeeting : Window
    {
        public clickFileShowMeeting(string fileID)
        {
            InitializeComponent();
            XMLProcessor processor = new XMLProcessor();
            LinkedList<MeetingNode> meetingList = processor.FindMeetingsByFileID(fileID);
            FileName.Text = processor.FindFilesByFileIDs(fileID).Last().GetFileName();
            ObservableCollection<clickFile> items = new ObservableCollection<clickFile>();
            foreach (MeetingNode meeting in meetingList)
            {
                items.Add(new clickFile() { Time = meeting.GetStartTimeS(), Name = meeting.GetMeetingTitle(), Attendee = meeting.GetAttendents()});
                MeetingList.ItemsSource = items;
            }
        }

        private void listView_SelectionChanged(Object sender, EventArgs e)
        {

        }

    }

    public class clickFile
    {
        public string Time { get; set; }
        public string Name { get; set; }
        public string Attendee { get; set; }
        public string Docs { get; set; }
    }
}
