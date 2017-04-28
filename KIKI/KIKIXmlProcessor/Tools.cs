using System;
using System.IO;

namespace KIKIXmlProcessor
{
    public static class Tools
    {
        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static int IsValidTimeRange(DateTime ct, DateTime et, DateTime mt, DateTime sTime, DateTime edTime)
        {
            // if all three time not within the range, return invalid
            if (((DateTime.Compare(edTime, ct) < 0) || (DateTime.Compare(ct, sTime) < 0))
                && ((DateTime.Compare(edTime, et) < 0) || (DateTime.Compare(et, sTime) < 0))
                && ((DateTime.Compare(edTime, mt) < 0) || (DateTime.Compare(mt, sTime) < 0)))
            {
                return -1;
            }
            //if the file is created after the start time and before the end time
            //Count the file as created in the meeting 
            if ((DateTime.Compare(sTime, ct) < 0) && (DateTime.Compare(ct, edTime) < 0))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


        //test if the event is a meeting by exam its duration
        public static Boolean IsValidMeeting(MeetingNode m, TimeSpan minDuration)
        {
            TimeSpan duration = m.GetDuration();
            if (duration.CompareTo(minDuration) < 0)
            {
                return false;
            }
            return true;
        }

        public static Boolean IsValid(String fName, String fPath, String ext, String stored_in)
        {
            if (fName == "")
            {
                return false;
            }
            if (ext == "")
            {
                return false;
            }
            if (stored_in == "Registry")
            {
                return false;
            }
            return true;
        }

        //-1: The file is invalid for this time range
        //1: The file is created in this time range
        //0: The file is modified in this time range
        public static int IsValidTimeRange(String mTime, String cTime, String eTime, DateTime sTime, DateTime edTime)
        {
            DateTime mt = new DateTime();
            DateTime ct = new DateTime();
            DateTime et = new DateTime();
            if ((mTime != "N / A") && (mTime != ""))
            {
                mt = StringToTime(mTime);
            }
            if ((cTime != "N / A") && (cTime != ""))
            {
                ct = StringToTime(cTime);
            }
            if ((eTime != "N / A") && (eTime != ""))
            {
                et = StringToTime(eTime);
            }
            // if all three time not within the range, return invalid
            if (((DateTime.Compare(edTime, ct) < 0) || (DateTime.Compare(ct, sTime) < 0))
                && ((DateTime.Compare(edTime, et) < 0) || (DateTime.Compare(et, sTime) < 0))
                && ((DateTime.Compare(edTime, mt) < 0) || (DateTime.Compare(mt, sTime) < 0)))
            {
                return -1;
            }
            //if the file is created after the start time and before the end time
            //Count the file as created in the meeting 
            if ((DateTime.Compare(sTime, ct) < 0) && (DateTime.Compare(ct, edTime) < 0))
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }

        public static DateTime StringToTime(String s)
        {
            if (s == "N / A")
            {
                DateTime na = DateTime.MinValue;
                return na;
            }
            if (s == "")
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

        public static String TimeToString(DateTime dt)
        {
            if (dt == DateTime.MinValue)
            {
                return "";
            }
            String year = dt.Year.ToString("0000");
            String month = dt.Month.ToString("00");
            String day = dt.Day.ToString("00");
            String hour = dt.Hour.ToString("00");
            String minute = dt.Minute.ToString("00");
            String second = dt.Second.ToString("00");

            String dtString = year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second;
            return dtString;
        }

        public static TimeSpan ComputeDuration(DateTime sTime, DateTime eTime)
        {
            TimeSpan duration = eTime - sTime;
            return duration;
        }
    }
}
