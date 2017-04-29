using System;
using System.Collections.Generic;
using System.Windows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System.IO;
using System.Threading;
using KIKIXmlProcessor;

namespace KIKI
{
    public partial class App : Application
    {

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";
        static List<string> bufferGoogle = new List<string>();
        public static List<string> bufferMeeting = new List<string>();
        static List<string> bufferFile = new List<string>();
        static UserCredential credential;
        public static LinkedList<FileNode> fileList = new LinkedList<FileNode>();
        public static LinkedList<MeetingNode> meetingList = new LinkedList<MeetingNode>();
        public static async void revoke() {
            try
            {
                await credential.RevokeTokenAsync(CancellationToken.None);
            }catch(Exception e)
            {
                MessageBox.Show("Network connection lost");
            }
        }

        public static void Initialize()
        {
            InitializeGoogle();
            InitializeCalendar();
            InitializeCore();
            InitializeMeetingTab();
            InitializeFileTab();
      

        }

        public static void InitializeGoogle()
            {
                using (var stream =
                  new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                string credPath = System.Environment.GetFolderPath(
                      System.Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");
               
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                      GoogleClientSecrets.Load(stream).Secrets,
                      Scopes,
                      "user",
                      CancellationToken.None
                      ).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
               
            }
          
        }

        public static void InitializeCalendar()
        {
            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.TimeMax = DateTime.Today.AddHours(24);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.\
            try {
                Events events = request.Execute();
                if (events.Items != null && events.Items.Count > 0)
                {

                    foreach (var eventItem in events.Items)
                    {
                        string attendee = "";
                        string when = eventItem.Start.DateTime.ToString();
                        if (eventItem.Attendees != null)
                        {
                            EventAttendee[] attendeeData = new EventAttendee[eventItem.Attendees.Count];
                            string[] attendeeString = new string[eventItem.Attendees.Count];
                            eventItem.Attendees.CopyTo(attendeeData, 0);
                            for (int i = 0; i < eventItem.Attendees.Count; i++)
                            {
                                attendee = attendee + attendeeData[i].DisplayName.ToString() + ", ";
                            }
                            if (eventItem.Attendees.Count < 2)
                            {
                                attendee = "N/A";
                            }
                        }
                        else
                        {
                            attendee = "N/A";
                        }

                        if (String.IsNullOrEmpty(when))
                        {
                            when = eventItem.Start.Date;
                        }

                        bufferGoogle.Add(when);
                        bufferGoogle.Add(eventItem.Summary);
                        bufferGoogle.Add(attendee);

                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("No Internet Connection");
            }
      
            
            Console.Read();
        }

        public static void InitializeMeetingTab()
        {
            LinkedList<MeetingNode> MeetingList = returnMeeting();
            foreach (MeetingNode item in MeetingList)
            {
                string date = item.GetStartTime().ToString().Split(' ')[0];
                string start = item.GetStartTime().ToString().Split(' ')[1];
                string end = item.GetEndTime().ToString().Split(' ')[1];
                bufferMeeting.Add(date);
                bufferMeeting.Add(start + "-" + end);
                bufferMeeting.Add(item.GetMeetingTitle());
                bufferMeeting.Add(item.GetAttendents());   
                bufferMeeting.Add(item.GetFileListS()+"");

            }
        }

        public static void InitializeFileTab()
        {
            LinkedList<FileNode> FileList = returnFile();
            foreach (FileNode item in FileList)
            {

                bufferFile.Add(item.GetFileName());
                bufferFile.Add(item.GetFilePath());
                bufferFile.Add(item.GetMeetingListS());
            }
        }

        public static void InitializeCore()
        {
            XMLProcessor p = new XMLProcessor();
            p.Read();
            fetchFromGoogle(p.GetLastUpdateTime());
            p.ProcessFileWithMeetingList(meetingList, fileList);
            p.Write();
        }

        public static void UpdateCore()
        {
            XMLProcessor p = new XMLProcessor();
            p.Read();
            p.ProcessFileWithMeetingList(meetingList, fileList);
            p.Write();
        }

        public static void fetchFromGoogle(DateTime minTime)
        {
            MeetingNode meeting = new MeetingNode();
            FileNode file = new FileNode();

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = minTime;
            request.TimeMax = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;


            // List events.
            try
            {
                Events events = request.Execute();
                if (events.Items != null && events.Items.Count > 0)
                {

                    foreach (var eventItem in events.Items)
                    {
                        meeting = new MeetingNode();
                        string attendee = "";
                        string when = eventItem.Start.DateTime.ToString();
                        if (eventItem.Attendees != null)
                        {
                            EventAttendee[] attendeeData = new EventAttendee[eventItem.Attendees.Count];
                            eventItem.Attendees.CopyTo(attendeeData, 0);
                            for (int i = 0; i < eventItem.Attendees.Count; i++)
                            {
                                attendee = attendee + attendeeData[i].DisplayName.ToString() + ", ";
                            }
                            if (eventItem.Attendees.Count < 2)
                            {
                                attendee = "N/A";
                            }
                        }
                        else
                        {
                            attendee = "N/A";
                        }
                        meeting.SetAttendents(attendee);
                        meeting.SetMeetingID(eventItem.Id);
                        meeting.SetParentID(eventItem.ICalUID.ToString());
                        meeting.SetStartTime(when);
                        meeting.SetEndTime(eventItem.End.DateTime.ToString());
                        meeting.SetMeetingTitle(eventItem.Summary);
                        if (DateTime.Compare(meeting.GetEndTime(), DateTime.Now) <= 0)
                        {
                            meetingList.AddLast(meeting);
                        }
                        if (eventItem.Attachments != null)
                        {

                            for (int i = 0; i < eventItem.Attachments.Count; i++)
                            {
                                file = new FileNode();
                                file.SetModifiedTime(eventItem.Start.DateTime.ToString());
                                file.SetFileID(" ");
                                file.SetExtension("GoogleDrive");
                                file.SetFileName(eventItem.Attachments[i].Title);
                                file.SetFilePath(eventItem.Attachments[i].FileUrl);
                                file.AddMeetings(eventItem.Id);
                                file.SetMissing("No");
                                fileList.AddLast(file);
                            }
                        }
                    }
                }
            }catch(Exception e)
            {
                MessageBox.Show("Network connection lost");
            }
        }

        public static LinkedList<MeetingNode> getGoogleMeetingList()
        {
            return meetingList;
        }

        public static LinkedList<FileNode> getGoogleFileList()
        {
            return fileList;
        }

        public static List<string> getGoogleBuffer()
        {
            return bufferGoogle;
        }

        public static List<string> getMeetingBuffer()
        {
            return bufferMeeting;
        }

        public static List<string> getFileBuffer()
        {
            return bufferFile;
        }

        public static void Clean()
        {
            bufferGoogle = new List<string>();
            credential = null;
        }

        public static void UpdateMeetingList()
        {
            bufferMeeting= new List<string>();
        }

        public static void UpdateFileList()
        {
            bufferFile = new List<string>();
        }

        public static void UpdateGoogleList()
        {
            bufferGoogle = new List<string>();
        }

        public static LinkedList<MeetingNode> returnMeeting()
        {
            LinkedList<MeetingNode> k = new LinkedList<MeetingNode>();
            XMLProcessor processor = new XMLProcessor();
            XMLSearcher searcher = new XMLSearcher(processor.GetWorkingPath());
            k = searcher.FindMeetingsByMeetingTitleKeywords("");
            return k;

        }

        public static LinkedList<FileNode> returnFile()
        {
            LinkedList<FileNode> k = new LinkedList<FileNode>();
            XMLProcessor processor = new XMLProcessor();
            XMLSearcher searcher = new XMLSearcher(processor.GetWorkingPath());
            k = searcher.FindFilesByFileNameKeywords("");
            return k;

        }

        }
    }
    