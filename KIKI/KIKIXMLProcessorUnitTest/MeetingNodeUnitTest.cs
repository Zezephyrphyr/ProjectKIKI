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
        String eTime = "2017/4/1 19:10:01";
        String PID = "05";
        String Attendents = "Steven, Eddie, Xiaoying";
        Int32 FileID = 10;

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
            //String actualsTime = meeting.GetStartTime().ToString();
            //String actualeTime = meeting.GetEndTime().ToString();
            //TimeSpan actualDuration = meeting.GetDuration();
            //Int32 actualPID = Convert.ToInt32(meeting.GetParentID());
            //String actualAttendents = meeting.GetAttendents();
            //LinkedList<Int32> actualFile = meeting.GetFileList();

            //test set
            meeting.SetMeetingTitle("ECON 201");
            String actualmTitle2 = meeting.GetMeetingTitle();
            Assert.AreEqual(mTitle, actualmTitle2, "Actual meeting title not equal to new mTitle");
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

            //test set
            meeting.SetMeetingID("05_2");
            String actualmID2 = meeting.GetMeetingID();
            Assert.AreEqual(mID, actualmID2, "Actual meeting title not equal to new mID");
        }

        [TestMethod]
        public void GetandSetStartTimeTest()
        {
            MeetingNode meeting0 = new MeetingNode(mTitle, mID, "", eTime, PID, Attendents, FileID);
            MeetingNode meeting = new MeetingNode(mTitle, mID, sTime, eTime, PID, Attendents, FileID);

            //test empty
            String actualsTime0 = meeting.GetStartTimeS();
            Assert.AreEqual("", actualsTime0, "Actual meeting start time does not equal to empty");



        }

        [TestMethod]
        public void GetandSetEndTimeTest()
        {

        }

        [TestMethod]
        public void GetandSetDurationTest()
        {

        }

        [TestMethod]
        public void GetandSetParentIDTest()
        {

        }

        [TestMethod]
        public void GetandSetAttendentsTest()
        {

        }

        [TestMethod]
        public void GetandAddFileListTest()
        {

        }
    }
}
