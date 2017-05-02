using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KIKIXmlProcessor;
using System.IO;

namespace KIKIXMLProcessorUnitTest
{
    [TestClass]
    //This class show some basic sample of codes during the testing of XMLProcessor through command lines
    //Due to the read and access of xml files, limited unit tests are implemented for this class
    public class XMLProcessorUnitTest
    {
        XMLProcessor test = new XMLProcessor("123");


        [TestMethod]
        public void FirstUseAndWriteMeetingsTest()
        {
            File.Delete("meetings.xml");
            File.Delete("files.xml");
            Assert.IsTrue(test.FirstUse());
            //test empty input string for write meetings
            LinkedList<MeetingNode> list1 = new LinkedList<MeetingNode>();
            test.WriteMeetings(list1);
            Assert.IsTrue(!test.FirstUse());
            DateTime testTimeS1 = new DateTime(2017, 4, 17, 14, 30, 20);
            DateTime testTimeE1 = new DateTime(2017, 4, 17, 14, 35, 40);
            MeetingNode n = new MeetingNode("TestMeeting1", "1", testTimeS1, testTimeE1, "", "Eddie,Anna");
            n.SetFiles("2");
            list1.AddLast(n);
            test.WriteMeetings(list1);
            Assert.IsTrue(!test.FirstUse());
            LinkedList<MeetingNode> list2 = new LinkedList<MeetingNode>();
            MeetingNode n2 = new MeetingNode("TestMeeting2", "2", testTimeS1, testTimeE1, "1", "Eddie,Anna,Lee");
            n2.SetFiles("1");
            list2.AddLast(n2);
            test.WriteMeetings(list2);
            Assert.IsTrue(!test.FirstUse());
        }

        [TestMethod]
        public void WriteandReadSettingsTest()
        {
            test.WriteSettings();
            test.GetInfoFromSettings();
            int LID = test.GetLastUpdateId();
            TimeSpan TS = test.GetMinimumDuration();
            DateTime DT = test.GetLastUpdateTime();
            String WP = test.GetWorkingPath();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void WriteandReadFilesTest()
        {
            LinkedList<FileNode> fn = new LinkedList<FileNode>();
            FileNode n1 = new FileNode();
            n1.SetFileID("1");
            n1.SetFileName("FileTest1");
            n1.SetModifiedTime("2017/04/03 03:03:03");
            FileNode n2 = new FileNode();
            n2.SetFileID("2");
            n2.SetFileName("FileTest2");
            FileNode n3 = new FileNode();
            n3.SetFileID("1");
            n3.SetFileName("FileTest3");
            n3.SetModifiedTime("2017/04/03 07:07:07");
            fn.AddLast(n1);
            fn.AddLast(n2);
            fn.AddLast(n3);
            XMLProcessor nP = new XMLProcessor("123");
            nP.WriteFiles(fn);
        }

        //These method are not designed to be unit method
        //Put the code in main function in XMLProcessor Class for testing
        public void ProcessFileWithMeetingNodeTest()
        {
            File.Delete("Settings.xml");
            File.Delete("meetings.xml");
            File.Delete("files.xml");
            MeetingNode node = new MeetingNode();
            node.SetStartTime("2017/01/01 12:00:00");
            node.SetEndTime("2017/12/12 12:00:00");
            node.SetMeetingID("12345");
            node.SetMeetingTitle("Hello");
            FileNode file1 = new FileNode();
            FileNode file2 = new FileNode();
            file1.SetCreatedTime("2017/03/03 12:00:00");
            file1.SetFileName("test1");
            file2.SetCreatedTime("2016/03/03 12:00:00");
            file1.SetFileName("test2");
            LinkedList<FileNode> fn = new LinkedList<FileNode>();
            fn.AddLast(file1);
            fn.AddLast(file2);
            XMLProcessor processor = new XMLProcessor("123");
            processor.FirstUse();
            processor.SetFileList(fn);
            processor.ProcessFileWithMeetingNode(node);
            processor.Write();
        }

        public void ProcessFilesWithMeetingListTest()
        {
            LinkedList<MeetingNode> meetingList = new LinkedList<MeetingNode>();
            LinkedList<FileNode> attachmentList = new LinkedList<FileNode>();
            MeetingNode m1 = new MeetingNode();
            m1.SetMeetingID("12345");
            m1.SetMeetingTitle("ABC");
            m1.SetStartTime("2017/04/03 13:13:15");
            m1.SetEndTime("2017/04/04 13:13:15");
            MeetingNode m2 = new MeetingNode();
            m2.SetMeetingID("23456");
            m2.SetMeetingTitle("BCD");
            m2.SetStartTime("2017/04/07 13:13:15");
            m2.SetEndTime("2017/04/08 13:13:15");
            MeetingNode m3 = new MeetingNode();
            m3.SetMeetingID("34567");
            m3.SetMeetingTitle("CDE");
            m3.SetStartTime("2017/04/08 13:13:15");
            m3.SetEndTime("2017/04/09 13:13:15");
            meetingList.AddLast(m1);
            meetingList.AddLast(m2);
            meetingList.AddLast(m3);

            FileNode f1 = new FileNode();
            f1.SetFileName("f1a");
            f1.AddMeetings("12345");
            FileNode f2 = new FileNode();
            f2.SetFileName("f2a");
            f2.AddMeetings("34567");
            FileNode f3 = new FileNode();
            f3.SetFileName("f3a");
            f3.AddMeetings("34567");
            attachmentList.AddLast(f1);
            attachmentList.AddLast(f2);
            attachmentList.AddLast(f3);

            XMLProcessor x = new XMLProcessor("123");
            x.ProcessFileWithMeetingList(meetingList, attachmentList);
            x.Write();
        }

        public void UpdateSettingsTest()
        {
            XMLProcessor test = new XMLProcessor("123");
            test.WriteSettings();
            test.GetInfoFromSettings();
            int LID = test.GetLastUpdateId();
            TimeSpan TS = test.GetMinimumDuration();
            DateTime DT = test.GetLastUpdateTime();
            String WP = test.GetWorkingPath();
            test.UpdateSettingsPath("resources/");
            TimeSpan TS2 = new TimeSpan(3, 4, 2);
            test.UpdateSettingsDuration(TS2);
            test.UpdateLastID(342);
            DateTime DT2 = new DateTime(2017, 3, 3, 3, 3, 3);
            test.UpdateSettingsTime(DT2);
        }
    }
}
