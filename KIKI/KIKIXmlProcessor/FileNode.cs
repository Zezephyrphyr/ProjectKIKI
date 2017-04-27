using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIKIXmlProcessor
{
    public class FileNode
    {
        private String fileName = "";
        private Int32 fileID = 0;
        private DateTime modifiedTime;
        private DateTime createdTime;
        private DateTime executeTime;
        private Boolean missing = true;
        private String extension = "";
        private String filePath = "";
        private LinkedList<String> MeetingList = new LinkedList<String>();

        public FileNode() { }
        public FileNode(String fN, String FID, String mTime, String cTime, String eTime, String ext, String fPath, String MeetingID)
        {
            fileName = fN;
            filePath = fPath;
            extension = ext;
            fileID = Convert.ToInt32(FID);
            modifiedTime = this.StringToTime(mTime);
            createdTime = this.StringToTime(cTime);
            executeTime = this.StringToTime(eTime);
            this.AddMeetings(MeetingID);
        }

        public FileNode(String fN, String FID, DateTime mTime, DateTime cTime, DateTime eTime, String ext, String fPath)
        {
            fileName = fN;
            filePath = fPath;
            extension = ext;
            fileID = Convert.ToInt32(FID);
            modifiedTime = mTime;
            createdTime = cTime;
            executeTime = eTime;
        }

        public void SetFileName(String fN)
        {
            fileName = fN;
        }

        public void SetFileID(Int32 FID)
        {
            fileID = FID;
        }

        public void SetFileID(String FID)
        {
            fileID = Convert.ToInt32(FID);
        }

        public void SetFilePath(String fPath)
        {
            filePath = fPath;
        }

        public void SetModifiedTime(String mTime)
        {
            modifiedTime = this.StringToTime(mTime);
        }

        public void SetCreatedTime(String cTime)
        {
            createdTime = this.StringToTime(cTime);
        }

        public void SetExecuteTime(String eTime)
        {
            executeTime = this.StringToTime(eTime);
        }

        public void SetExtension(String ext)
        {
            extension = ext;
        }

        public void SetMissing(String ms)
        {
            if (ms == "Yes")
            {
                missing = true;
            }
            else
            {
                missing = false;
            }
        }

        //May need adjustment
        public void SetMeetings(String meetingString)
        {
            MeetingList.AddLast(meetingString);
        }

        public void AddMeetings(String MeetingID)
        {
            MeetingList.AddLast(MeetingID);
        }

        public void AddMeetings(Int32 MeetingID)
        {
            MeetingList.AddLast(Convert.ToString(MeetingID));
        }

        public String GetFileName()
        {
            return fileName;
        }

        public Int32 GetFileID()
        {
            return fileID;
        }

        public DateTime GetModifiedTime()
        {
            return modifiedTime;
        }

        public String GetModifiedTimeS()
        {
            String ModifiedTimeS = this.TimeToString(modifiedTime);
            return ModifiedTimeS;
        }

        public DateTime GetCreatedTime()
        {
            return createdTime;
        }

        public String GetCreatedTimeS()
        {
            String CreatedTimeS = this.TimeToString(createdTime);
            return CreatedTimeS;
        }

        public DateTime GetExecuteTime()
        {
            return executeTime;
        }

        public String GetExecuteTimeS()
        {
            String ExecutedTimeS = this.TimeToString(executeTime);
            return ExecutedTimeS;
        }

        public Boolean GetMissing()
        {
            return missing;
        }

        public String GetExtension()
        {
            return extension;
        }

        public String GetFilePath()
        {
            return filePath;
        }

        public LinkedList<String> GetMeetingList()
        {
            return MeetingList;
        }

        public String GetMeetingListS()
        {
            String MeetingID = "";
            for (LinkedListNode<String> meeting = MeetingList.First; meeting != MeetingList.Last; meeting = meeting.Next)
            {
                MeetingID = meeting.Value;
                MeetingID = MeetingID + ";";
            }
            if (MeetingID != "")
            {
                MeetingID = MeetingID.Remove(MeetingID.Length - 1);
            }
            return MeetingID;
        }

        public Boolean AddToMeetinglist(String s)
        {
            if (MeetingList.Find(s) == null)
            {
                MeetingList.AddLast(s);
                return true;
            }
            else
            {
                return false;
            }
        }

        public DateTime StringToTime(String s)
        {
            if (s == "N / A")
            {
                DateTime empty = DateTime.MinValue;
                return empty;
            }
            else
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
        }

        public String TimeToString(DateTime dt)
        {
            String year = Convert.ToString(dt.Year);
            String month = Convert.ToString(dt.Month);
            String date = Convert.ToString(dt.Date);
            String hour = Convert.ToString(dt.Hour);
            String minute = Convert.ToString(dt.Minute);
            String second = Convert.ToString(dt.Second);

            String dtString = year + "/" + month + "/" + date + " " + hour + ":" + minute + ":" + second;
            return dtString;
        }
    }

}
