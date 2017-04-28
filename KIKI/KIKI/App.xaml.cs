using System;
using System.Collections.Generic;
using System.Windows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Diagnostics;
using KIKIXmlProcessor;

namespace KIKI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    /// //////////////////////////////////////
    public partial class App : Application
    {

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";
        static List<string> bufferGoogle = new List<string>();
        static List<string> bufferMeeting = new List<string>();
        static List<string> bufferFile = new List<string>();
        static UserCredential credential;
        static LinkedList<FileNode> fileList;
        static LinkedList<MeetingNode> meetingList;

        public static async void revoke() { 
        await credential.RevokeTokenAsync(CancellationToken.None);
        }

        public static void Initialize()
        {
            InitializeGoogle();
            InitializeCalendar();
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
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            Debug.WriteLine("Upcoming events:");
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
                            attendee = "Unknown";
                        }
                    }
                    else
                    {
                        attendee = "Unknown";
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
            else
            {
                Debug.WriteLine("No upcoming events found.");
            }
            Console.Read();
        }

        public static void InitializeMeetingTab()
        {
            LinkedList<MeetingNode> MeetingList = returnMeeting();
            foreach (MeetingNode item in MeetingList)
            {
                
                bufferMeeting.Add(item.GetStartTime().ToString());
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
                Debug.Print(item.GetMeetingListS());
            }
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
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
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
                        eventItem.Attendees.CopyTo(attendeeData, 0);
                        for (int i = 0; i < eventItem.Attendees.Count; i++)
                        {
                            attendee = attendee + attendeeData[i].DisplayName.ToString() + ", ";
                        }
                        if (eventItem.Attendees.Count < 2)
                        {
                            attendee = "Unknown";
                        }
                    }
                    else
                    {
                        attendee = "Unknown";
                    }
                    meeting.SetAttendents(attendee);
                    meeting.SetMeetingID(eventItem.Id);
                    meeting.SetParentID(Convert.ToInt32(eventItem.ICalUID));
                    meeting.SetStartTime(when);
                    meeting.SetEndTime(eventItem.End.DateTime.ToString());
                    meeting.SetMeetingTitle(eventItem.Summary);
                    meetingList.AddLast(meeting);

                    for(int i = 0; i < eventItem.Attachments.Count; i++)
                    {
                        file.SetModifiedTime(eventItem.Start.DateTime.ToString());
                        file.SetFileID(Convert.ToInt32(eventItem.Attachments[i].FileId));
                        file.SetExtension("GoogleDrive");
                        file.SetFileName(eventItem.Attachments[i].Title);
                        file.SetFilePath(eventItem.Attachments[i].FileUrl);
                        fileList.AddLast(file);
                    }
                }
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

        public static LinkedList<MeetingNode> returnMeeting()
        {
            LinkedList<MeetingNode> k = new LinkedList<MeetingNode>();

            String mTitle = "EECS395";
            String mID = "05_1";
            String sTime = "2017/4/1 17:30:05";
            String eTime = "2017/4/1 19:10:05";
            String PID = "05";
            String Attendents = "Steven, Eddie, Xiaoying";
            Int32 FileID = 1;

            String mTitleh = "EECS395h";
            String mIDh = "05_1h";
            String sTimeh = "2017/4/1 15:30:05";
            String eTimeh = "2017/4/1 14:10:05";
            String PIDh = "051";
            String Attendentsh = "Stevenh, Eddie, Xiaoying";
            Int32 FileIDh = 1;

            String mTitle2 = "EECS3952";
            String mID2 = "05_2";
            String sTime2 = "2017/4/2 17:30:05";
            String eTime2 = "2017/4/2 19:10:05";
            String PID2 = "052";
            String Attendents2 = "Steven, Eddie, Xiaoying2";
            Int32 FileID2 = 2;

            MeetingNode meeting = new MeetingNode(mTitle, mID, sTime, eTime, PID, Attendents);
            meeting.AddFiles(2);
            meeting.AddFiles(3);
            k.AddLast(meeting);
            meeting = new MeetingNode(mTitle2, mID2, sTime2, eTime2, PID2, Attendents2);
            k.AddLast(meeting);
            meeting = new MeetingNode(mTitleh, mIDh, sTimeh, eTimeh, PIDh, Attendentsh);
            k.AddLast(meeting);
            return k;
        }

        public static LinkedList<FileNode> returnFile()
        {
            LinkedList<FileNode> k = new LinkedList<FileNode>();
            String fileName = "test.txt";
            String fileID = "1";
            String modifiedTime = "2017/04/01 03:00:00";
            String createdTime = "2017/04/01 01:00:00";
            String executeTime = "2017/04/01 05:00:00";
            Boolean missing = true;
            String extension = ".txt";
            String filePath = "";
            String MeetingID = "1";

            FileNode file = new FileNode(fileName, fileID, modifiedTime, createdTime, executeTime, extension, filePath, MeetingID);
            file.AddMeetings("2");
            file.AddMeetings("3");
            k.AddLast(file);

            String fileName2 = "test2.txt";
            String fileID2 = "12";
            String modifiedTime2 = "2017/04/02 03:00:00";
            String createdTime2 = "2017/04/02 01:00:00";
            String executeTime2 = "2017/04/02 05:00:00";
            String extension2 = ".txt";
            String filePath2 = "";
            String MeetingID2 = "1";

            file = new FileNode(fileName2, fileID2, modifiedTime2, createdTime2, executeTime2, extension2, filePath2, MeetingID2);
            k.AddLast(file);


            return k;
        }

        }
    }
    