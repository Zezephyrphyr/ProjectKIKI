using System;

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

        public FileNode() { }
        public FileNode(String fN, Int32 ID, String mTime, String cTime, String eTime, String ext, String fPath)
        {
            fileName = fN;
            filePath = fPath;
            extension = ext;
            fileID = ID;
            modifiedTime = this.StringToTime(mTime);
            createdTime = this.StringToTime(cTime);
            executeTime = this.StringToTime(eTime);
        }

        public void SetFileName(String fN)
        {
            fileName = fN;
        }

        public void SetFileID(Int32 ID)
        {
            fileID = ID;
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

        public void SetMissing(int ms)
        {
            if (ms == 1)
            {
                missing = true;
            }
            else
            {
                missing = false;
            }
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

        public DateTime GetCreatedTime()
        {
            return createdTime;
        }

        public DateTime GetExecutedTime()
        {
            return executeTime;
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
    }

}

