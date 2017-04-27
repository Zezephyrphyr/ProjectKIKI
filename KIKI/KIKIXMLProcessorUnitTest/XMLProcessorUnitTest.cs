using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KIKIXmlProcessor;

namespace KIKIXMLProcessorUnitTest
    {
        [TestClass]
        public class XMLProcessorUnitTest
        {
            [TestMethod]
            public void TestMethod1()
            {
                String mTitle = "EECS395";
                String mID = "05_1";
                String sTime = "2017/4/1 17:30:05";
                String eTime = "2017/4/1 19:10:01";
                String PID = "05";
                String Attendents = "Steven, Eddie, Xiaoying";
                Int32 FileID = 10;
                MeetingNode meeting = new MeetingNode(mTitle, mID, sTime, eTime, PID, Attendents, FileID);

                // act
                String actualmTitle = meeting.GetMeetingTitle();
                String actualsTime = meeting.GetStartTime().ToString();
                String actualeTime = meeting.GetEndTime().ToString();
                TimeSpan actualDuration = meeting.GetDuration();
                Int32 actualPID = Convert.ToInt32(meeting.GetParentID());
                String actualAttendents = meeting.GetAttendents();
                LinkedList<Int32> actualFile = meeting.GetFileList();

                // test
                //Console.WriteLine(actualmTitle);
                //Console.WriteLine(actualsTime);
                //Console.WriteLine(actualeTime);
                //Console.WriteLine(actualDuration.ToString());
                //Console.WriteLine(actualPID);
                //Console.WriteLine(actualAttendents);
                //actualFile.Equals(10);
                //Console.WriteLine();
                //StringAssert.Contains(mTitle, actualmTitle);
                Assert.AreEqual(Convert.ToDouble(actualPID), 4, 0.001, "Not Equal");
            }
        }
    }