using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KIKIXmlProcessor;
using System.IO;

namespace KIKIXMLProcessorUnitTest
{
    [TestClass]
    public class XMLProcessorUnitTest
    {
        XMLProcessor test = new XMLProcessor();


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
            /*
            File.Delete("meetings.xml");
            Assert.IsTrue(test.FirstUse());
            test.WriteMeetings(list2);
            Assert.IsTrue(!test.FirstUse());
            */


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
            XMLProcessor nP = new XMLProcessor();
            nP.WriteFiles(fn);
        }

        [TestMethod]
        public void IsValidMeetingTest()
        {


        }
    }
}
