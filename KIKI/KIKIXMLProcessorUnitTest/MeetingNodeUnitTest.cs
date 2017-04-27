using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KIKIXmlProcessor;

namespace KIKIXMLProcessorUnitTest
{
    [TestClass]
    public class MeetingNodeUnitTest
    {
        String mTitle = "EECS395";
        String mID = "05_1";
        String sTime = "2017/4/1 17:30:05";
        String eTime = "2017/4/1 19:10:05";
        String PID = "05";
        String Attendents = "Steven, Eddie, Xiaoying";
        Int32 FileID = 1;

        [TestMethod]
        public void GetandSetMeetingTitleTest()
        {
            MeetingNode meeting0 = new MeetingNode("", mID, sTime, eTime, PID, Attendents, FileID);
            MeetingNode meeting = new MeetingNode(mTitle, mID, sTime, eTime, PID, Attendents, FileID);

            //test empty
            String actualmTitle0 = meeting0.GetMeetingTitle();
            Assert.AreEqual("", actualmTitle0, "Actual meeting title not equal to empty");



            //test get
            String actualmTitle1 = meeting.GetMeetingTitle();
            Assert.AreEqual(mTitle, actualmTitle1, "Actual meeting title not equal to mTitle");

            //test set string
            meeting.SetMeetingTitle("ECON 201");
            String actualmTitle2 = meeting.GetMeetingTitle();
            Assert.AreEqual("ECON201", actualmTitle2, "Actual meeting title not equal to new mTitle");

            //test set int
            meeting.SetMeetingTitle(201);
            String actualmTitle3 = meeting.GetMeetingTitle();
            Assert.AreEqual("201", actualmTitle3, "Actual meeting title not equal to new mTitle 201");
        }

        [TestMethod]
        public void GetandSetMeetingID()
        {
            MeetingNode meeting0 = new MeetingNode(mTitle, "", sTime, eTime, PID, Attendents, FileID);
            MeetingNode meeting = new MeetingNode(mTitle, mID, sTime, eTime, PID, Attendents, FileID);

            //test empty
            String actualmID0 = meeting0.GetMeetingID();
            Assert.AreEqual("", actualmID0, "Actual meeting ID not equal to empty");

            //test get
            String actualmID1 = meeting.GetMeetingID();
            Assert.AreEqual(mID, actualmID1, "Actual meeting ID not equal to mID");

            //test set string
            meeting.SetMeetingID("05_2");
            String actualmID2 = meeting.GetMeetingID();
            Assert.AreEqual("05_2", actualmID2, "Actual meeting ID not equal to 05_2");

            //test set int
            meeting.SetMeetingID(052);
            String actualmID3 = meeting.GetMeetingID();
            Assert.AreEqual("052", actualmID3, "Actual meeting ID not equal to 052");
        }

        [TestMethod]
        public void GetandSetStartTimeTest()
        {
            MeetingNode meeting0 = new MeetingNode(mTitle, mID, "", eTime, PID, Attendents, FileID);
            MeetingNode meeting = new MeetingNode(mTitle, mID, sTime, eTime, PID, Attendents, FileID);

            //test empty
            String actualsTime0 = meeting.GetStartTimeS();
            Assert.AreEqual("", actualsTime0, "Actual meeting start time does not equal to empty");

            //test get
            String actualsTime1 = meeting.GetStartTimeS();
            Assert.AreEqual(sTime, actualsTime1, "Actual meeting start time does not equal to empty");

            //test set
            meeting.SetStartTime("2017/4/2 12:30:00");
            String actualsTime2 = meeting.GetStartTimeS();
            Assert.AreEqual("2017/4/2 12:30:00", actualsTime2, "Actual meeting start time does not equal to empty");
        }

        [TestMethod]
        public void GetandSetEndTimeTest()
        {
            MeetingNode meeting0 = new MeetingNode(mTitle, mID, sTime, "", PID, Attendents, FileID);
            MeetingNode meeting = new MeetingNode(mTitle, mID, sTime, eTime, PID, Attendents, FileID);

            //test empty
            String actualsTime0 = meeting.GetStartTimeS();
            Assert.AreEqual("", actualsTime0, "Actual meeting end time does not equal to empty");

            //test get
            String actualsTime1 = meeting.GetStartTimeS();
            Assert.AreEqual(eTime, actualsTime1, "Actual meeting end time does not equal to empty");

            //test set
            meeting.SetEndTime("2017/4/2 18:00:30");
            String actualsTime2 = meeting.GetStartTimeS();
            Assert.AreEqual("2017/4/2 18:00:30", actualsTime2, "Actual meeting end time does not equal to empty");
        }

        [TestMethod]
        public void GetandSetDurationTest()
        {
            MeetingNode meeting0 = new MeetingNode(mTitle, mID, "", "", PID, Attendents, FileID);
            MeetingNode meeting = new MeetingNode(mTitle, mID, sTime, eTime, PID, Attendents, FileID);

            //test empty
            TimeSpan expectedspan0 = new TimeSpan(0, 0, 0, 0);
            TimeSpan actualspan0 = meeting0.GetDuration();
            Assert.AreEqual(expectedspan0, actualspan0, "Actual duration does not equal to empty");

            //test get
            TimeSpan expectedspan1 = new TimeSpan(0, 1, 40, 0);
            TimeSpan actualspan1 = meeting.GetDuration();
            Assert.AreEqual(expectedspan1, actualspan1, "Actual duration does not equal to 0 day 1 hour 40 mins 0 second");

            //test set through start and end time
            meeting.SetDuration("2017/4/2 14:00:00", "2017/4/3 19:30:00");
            TimeSpan expectedspan2 = new TimeSpan(1, 5, 30, 0);
            TimeSpan actualspan2 = meeting.GetDuration();
            Assert.AreEqual(expectedspan2, actualspan2, "Actual duration does not equal to 1 day 5 hour 30 mins 0 second");

            //test set through string duration more than 1 day
            meeting.SetDuration("1.5:30:10");
            TimeSpan expectedspan3 = new TimeSpan(1, 5, 30, 10);
            TimeSpan actualspan3 = meeting.GetDuration();
            Assert.AreEqual(expectedspan3, actualspan3, "Actual duration does not equal to 1 day 5 hour 30 mins 10 second");

            //test set through string duration less than 1 day
            meeting.SetDuration("0.3:30:10");
            TimeSpan expectedspan4 = new TimeSpan(0, 3, 30, 10);
            TimeSpan actualspan4 = meeting.GetDuration();
            Assert.AreEqual(expectedspan4, actualspan4, "Actual duration does not equal to 0 day 3 hour 30 mins 10 second");
        }

        [TestMethod]
        public void GetandSetParentIDTest()
        {
            MeetingNode meeting0 = new MeetingNode(mTitle, mID, sTime, eTime, "", Attendents, FileID);
            MeetingNode meeting = new MeetingNode(mTitle, mID, sTime, eTime, PID, Attendents, FileID);

            //test empty
            String actualPID0 = meeting0.GetParentID();
            Assert.AreEqual("", actualPID0, "Actual Parent ID not equal to empty");

            //test get
            String actualPID1 = meeting.GetParentID();
            Assert.AreEqual(PID, actualPID1, "Actual meeting ID not equal to PID");

            //test set string
            meeting.SetMeetingID("01");
            String actualPID2 = meeting.GetParentID();
            Assert.AreEqual("01", actualPID2, "Actual meeting ID not equal to 01");

            //test set int
            meeting.SetMeetingID(1);
            String actualPID3 = meeting.GetParentID();
            Assert.AreEqual("1", actualPID3, "Actual meeting ID not equal to 1");
        }

        [TestMethod]
        public void GetandSetAttendentsTest()
        {
            MeetingNode meeting0 = new MeetingNode(mTitle, mID, sTime, eTime, PID, "", FileID);
            MeetingNode meeting = new MeetingNode(mTitle, mID, sTime, eTime, PID, Attendents, FileID);

            //test empty
            String actualAttend0 = meeting0.GetAttendents();
            Assert.AreEqual("", actualAttend0, "Actual Attendents are not empty");

            //test get
            String actualAttend1 = meeting.GetAttendents();
            Assert.AreEqual(Attendents, actualAttend1, "Actual attendents are not Steven, Eddie, Xiaoying");

            //test set
            meeting.SetAttendents("Harris, John");
            String actualAttend2 = meeting.GetAttendents();
            Assert.AreEqual("Harris, John", actualAttend2, "Actual attendents are not Harris, John");

        }

        [TestMethod]
        public void GetandAddFileListTest()
        {
            MeetingNode meeting0 = new MeetingNode(mTitle, mID, sTime, eTime, PID, Attendents, 0);
            MeetingNode meeting = new MeetingNode(mTitle, mID, sTime, eTime, PID, Attendents, FileID);

            //test empty GetFileList
            LinkedList<Int32> actualFileList0 = meeting0.GetFileList();
            LinkedList<Int32> expectedFileList0 = new LinkedList<Int32>();
            expectedFileList0.AddFirst(0);
            Assert.AreEqual(expectedFileList0, actualFileList0, "Actual FileList does not contain 0");

            //test empty GetFileListS
            String acutualFileID01 = meeting0.GetFileListS();
            LinkedList<Int32> expectedFileList01 = new LinkedList<Int32>();
            expectedFileList01.AddFirst(0);
            String expectedFileID01 = expectedFileList01.First.ToString();
            Assert.AreEqual(expectedFileID01, acutualFileID01, "Actual First FileID does not equal to 0");

            //test get GetFileList
            LinkedList<Int32> actualFileList2 = meeting.GetFileList();
            LinkedList<Int32> expectedFileList2 = new LinkedList<Int32>();
            expectedFileList2.AddFirst(1);
            Assert.AreEqual(expectedFileList2, actualFileList2, "Actual FileList does not contain 1");

            //test get GetFileListS
            String acutualFileID02 = meeting.GetFileListS();
            LinkedList<Int32> expectedFileList02 = new LinkedList<Int32>();
            expectedFileList02.AddFirst(1);
            String expectedFileID02 = expectedFileList02.First.ToString();
            Assert.AreEqual(expectedFileID02, acutualFileID02, "Actual First FileID does not equal to 1");

            //test add Filelist
            meeting.AddFiles(2);
            meeting.AddFiles(3);
            LinkedList<Int32> actualFileList3 = meeting.GetFileList();
            LinkedList<Int32> expectedFileList3 = new LinkedList<Int32>();
            expectedFileList3.AddFirst(1);
            expectedFileList3.AddLast(2);
            expectedFileList3.AddLast(3);
            Assert.AreEqual(expectedFileList3, actualFileList3, "Actual FileList does not contain {1, 2, 3}");

            //test set Filelist
            meeting0.FileList.Clear();
            meeting0.SetFiles("1;2;3");
            LinkedList<Int32> actualFileList4 = meeting0.GetFileList();
            LinkedList<Int32> expectedFileList4 = new LinkedList<Int32>();
            expectedFileList4.AddFirst(1);
            expectedFileList4.AddLast(2);
            expectedFileList4.AddLast(3);
            Assert.AreEqual(expectedFileList4, actualFileList4, "Actual FileList does not contain {1, 2, 3}");

        }
    }
}