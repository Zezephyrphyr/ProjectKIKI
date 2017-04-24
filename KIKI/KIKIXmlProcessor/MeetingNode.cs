using System;

namespace KIKIXmlProcessor
{
    public class MeetingNode
    {
        private string MeetingTitle = "";
        private DateTime StartTime;
        private DateTime EndTime;
        private TimeSpan Duration;
        private Int32 ParentID;
        private string Attendents;
        private FileNode Files;

        public MeetingNode() { }
        public MeetingNode(String MT, String sTime, String eTime, Int32 PID, String Attend, Int32 FileID)
        {
            MeetingTitle = MT;
            Attendents = Attend;
            ParentID = PID;
            StartTime = this.StringToTime(sTime);
            EndTime = this.StringToTime(eTime);
            Files = new FileNode("", FileID, "", "", "", "", "");
            Duration = this.computeDuration(StartTime, EndTime);
        }

        public void SetMeetingTitle(String MT)
        {
            MeetingTitle = MT;
        }

        public void SetStartTime(String sTime)
        {
            StartTime = this.StringToTime(sTime);
        }

        public void SetEndTime(String eTime)
        {
            EndTime = this.StringToTime(eTime);
        }

        public void SetParentID(Int32 PID)
        {
            ParentID = PID;
        }

        public void SetAttendents(String Attend)
        {
            Attendents = Attend;
        }

        public void SetFiles(Int32 FileID)
        {
            Files = new FileNode("", FileID, "", "", "", "", "");
        }

        public void SetDuration(String sTime, String eTime)
        {
            Duration = this.computeDuration(this.StringToTime(sTime), this.StringToTime(eTime));
        }

        public String GetMeetingTitle()
        {
            return MeetingTitle;
        }

        public DateTime GetStartTime()
        {
            return StartTime;
        }

        public DateTime GetEndTime()
        {
            return EndTime;
        }

        public TimeSpan GetDuration()
        {
            return Duration;
        }

        public Int32 GetParentID()
        {
            return ParentID;
        }

        public String GetAttendents()
        {
            return Attendents;
        }

        public FileNode GetFile()
        {
            return Files;
        }

        public DateTime StringToTime(String s)
        {
            String[] s1 = s.Split(' ');
            String[] s2 = s1[0].Split('/');
            String[] s3 = s1[1].Split(':');
            int year = Convert.ToInt32(s2[0]);
            int month = Convert.ToInt32(s2[1]);
            int day = Convert.ToInt32(s2[2]);
            int hour = Convert.ToInt32(s3[0]);
            int minute = Convert.ToInt32(s3[1]);
            int second = Convert.ToInt32(s3[2]);

            DateTime x = new DateTime(year, month, day, hour, minute, second);
            return x;
        }

        public TimeSpan computeDuration(DateTime StartTime, DateTime EndTime)
        {
            TimeSpan duration = EndTime - StartTime;
            return duration;
        }
    }
}
