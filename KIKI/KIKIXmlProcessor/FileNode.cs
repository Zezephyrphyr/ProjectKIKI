using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIKIXmlProcessor
{
    class FileNode
    {
        private string fileName = "";
        private DateTime modifiedTime;
        private DateTime createdTime;
        private DateTime executeTime;
        private Boolean missing = true;
        private String extension = "";
        private String filePath = "";

        public FileNode() { }
        public FileNode(String fN, String mTime, String cTime, String eTime, String ext, String fPath) {
            fileName = fN;
            filePath = fPath;
            extension = ext;
            modifiedTime = this.StringToTime(mTime);
            createdTime = this.StringToTime(cTime);
            executeTime = this.StringToTime(eTime);
        }

        public void SetFileName(String fN)
        {
            fileName = fN;
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
