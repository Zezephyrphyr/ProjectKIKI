using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KIKIXmlProcessor;

namespace KIKIXMLProcessorUnitTest
{
    [TestClass]
    public class FileNodeUnitTest
    {
        String fileName = "test.txt";
        String fileID = "1";
        String modifiedTime = "2017/04/01 03:00:00";
        String createdTime = "2017/04/01 01:00:00";
        String executeTime = "2017/04/01 05:00:00";
        Boolean missing = true;
        String extension = ".txt";
        String filePath = "D:/EECS395";
        String MeetingID = "01";

        [TestMethod]
        public void GetandSetFileNameTest()
        {
            FileNode file0 = new FileNode("", fileID, modifiedTime, createdTime, executeTime, extension, filePath, MeetingID);
            FileNode file = new FileNode(fileName, fileID, modifiedTime, createdTime, executeTime, extension, filePath, MeetingID);

            //test get empty string
            String actualfileName0 = file0.GetFileName();
            Assert.AreEqual("", actualfileName0, "Actual file name is not an empty string");

            //test get non-empty string
            String actualfileName1 = file.GetFileName();
            Assert.AreEqual("test.txt", actualfileName1, "Actual file name does not equal to text.txt");

            //test set empty string
            file.SetFileName("");
            String actualfileName2 = file.GetFileName();
            Assert.AreEqual("", actualfileName2, "Set empty string file name was not successful");

            //test set non-empty string
            file.SetFileName("exam.pdf");
            String actualfileName3 = file.GetFileName();
            Assert.AreEqual("exam.pdf", actualfileName3, "Set non-empty string file name was not successful");
        }

        [TestMethod]
        public void GetandSetFileIDTest()
        {
            FileNode file0 = new FileNode(fileName, "0", modifiedTime, createdTime, executeTime, extension, filePath, MeetingID);
            FileNode file = new FileNode(fileName, fileID, modifiedTime, createdTime, executeTime, extension, filePath, MeetingID);

            //test get 0
            Int32 actualfileID0 = file0.GetFileID();
            Assert.AreEqual(0, actualfileID0, "Actual file ID does not equal to 0");

            //test get non-0 integer
            Int32 actualfileID1 = file.GetFileID();
            Assert.AreEqual(1, actualfileID1, "Actual file ID does not equal to 1");

            //test set int input of 0
            file.SetFileID(0);
            Int32 actualfileID2 = file.GetFileID();
            Assert.AreEqual(0, actualfileID2, "Set method using 0 as input failed. Actual file ID does not equal to 0");

            //test set int input of non-0 integer
            file.SetFileID(2);
            Int32 actualfileID3 = file.GetFileID();
            Assert.AreEqual(2, actualfileID3, "Set method using non-0 int input failed. Actual file ID does not equal to 2");

            //test set string input of 0 value
            file.SetFileID("0");
            Int32 actualfileID4 = file.GetFileID();
            Assert.AreEqual(0, actualfileID4, "Set method using string input of 0 value failed. Actual file ID does not equal to 0");

            //test set string input of non-0 value
            file.SetFileID("3");
            Int32 actualfileID5 = file.GetFileID();
            Assert.AreEqual(3, actualfileID5, "Set method using string input of non-0 value failed. Actual file ID does not equal to 3");
        }

        [TestMethod]
        public void GetandSetFilePathTest()
        {
            FileNode file0 = new FileNode(fileName, fileID, modifiedTime, createdTime, executeTime, extension, "", MeetingID);
            FileNode file = new FileNode(fileName, fileID, modifiedTime, createdTime, executeTime, extension, filePath, MeetingID);

            //test get empty string
            String actualfilePath0 = file0.GetFilePath();
            Assert.AreEqual("", actualfilePath0, "Actual file path is not an empty string");

            //test get non-empty string
            String actualfilePath1 = file.GetFilePath();
            Assert.AreEqual("D:/EECS395", actualfilePath1, "Actual file path does not equal to D:/EECS395");

            //test set empty string
            file.SetFilePath("");
            String actualfilePath2 = file.GetFilePath();
            Assert.AreEqual("", actualfilePath2, "Set empty string path was not successful");

            //test set non-empty string
            file.SetFilePath("C:/MKMR201");
            String actualfilePath3 = file.GetFilePath();
            Assert.AreEqual("C:/MKMR201", actualfilePath3, "Set non-empty string path was not successful");
        }

        [TestMethod]
        public void GetandSetExtensionTest()
        {
            FileNode file0 = new FileNode(fileName, fileID, modifiedTime, createdTime, executeTime, "", filePath, MeetingID);
            FileNode file = new FileNode(fileName, fileID, modifiedTime, createdTime, executeTime, extension, filePath, MeetingID);

            //test get empty string
            String actualExtension0 = file0.GetExtension();
            Assert.AreEqual("", actualExtension0, "Actual file extension is not an empty string");

            //test get non-empty string
            String actualExtension1 = file.GetExtension();
            Assert.AreEqual(".txt", actualExtension1, "Actual file extension does not equal to D:/EECS395");

            //test set empty string
            file.SetFilePath("");
            String actualExtension2 = file.GetExtension();
            Assert.AreEqual("", actualExtension2, "Set empty string extension was not successful");

            //test set non-empty string
            file.SetFilePath(".pdf");
            String actualExtension3 = file.GetExtension();
            Assert.AreEqual(".pdf", actualExtension3, "Set non-empty string extension was not successful");
        }

        [TestMethod]
        public void GetandSetModifiedTimeTest()
        {
            DateTime min = DateTime.MinValue;
            FileNode file_na = new FileNode(fileName, fileID, "N / A", createdTime, executeTime, extension, filePath, MeetingID);
            FileNode file01 = new FileNode(fileName, fileID, "0001/01/01 00:00:00", createdTime, executeTime, extension, filePath, MeetingID);
            FileNode file02 = new FileNode(fileName, fileID, min, min, min, extension, filePath, MeetingID);
            FileNode file = new FileNode(fileName, fileID, modifiedTime, createdTime, executeTime, extension, filePath, MeetingID);

            //test when input is "N / A"
            String actualmodifiedTime_na = file_na.GetModifiedTimeS();
            Assert.AreEqual("", actualmodifiedTime_na, "Actual modified time is not empty when input is N / A");

            //test get method when input is string-type min value
            String actualmodifiedTime01 = file01.GetModifiedTimeS();
            Assert.AreEqual("", actualmodifiedTime01, "Actual modified time is not empty when input is 0001/01/01 00:00:00");

            //test get method when input is DateTime-type min value
            String actualmodifiedTime02 = file02.GetModifiedTimeS();
            Assert.AreEqual("", actualmodifiedTime02, "Actual modified time is not empty when input is 0001/01/01 00:00:00");

            //test get method when input is not min value or "N / A"
            String actualmodifiedTime1 = file.GetModifiedTimeS();
            Assert.AreEqual("2017/04/01 03:00:00", actualmodifiedTime1, "Actual modified time is not 2017/04/01 03:00:00");

            //test set method when input is empty string
            file.SetModifiedTime("");
            String actualmodifiedTime2 = file.GetModifiedTimeS();
            Assert.AreEqual("", actualmodifiedTime2, "Actual modified time is not empty when the modified time is set to be empty string");

            //test set method when input is "N / A"
            file.SetModifiedTime("N / A");
            String actualmodifiedTime3 = file.GetModifiedTimeS();
            Assert.AreEqual("", actualmodifiedTime3, "Actual modified time is not empty when the modified time is set to be N / A");

            //test set method when input is MinValue
            file.SetModifiedTime("0001/01/01 00:00:00");
            String actualmodifiedTime4 = file.GetModifiedTimeS();
            Assert.AreEqual("", actualmodifiedTime4, "Actual modified time is not empty when the modified time is set to be MinValue");

            //test set method when input is not MinValue
            file.SetModifiedTime("2016/12/01 10:00:00");
            String actualmodifiedTime5 = file.GetModifiedTimeS();
            Assert.AreEqual("2016/12/01 10:00:00", actualmodifiedTime5, "Actual modified time is not 2016/12/01 10:00:00");
        }

        [TestMethod]
        public void GetandSetCreatedTimeTest()
        {
            DateTime min = DateTime.MinValue;
            FileNode file_na = new FileNode(fileName, fileID, modifiedTime, "N / A", executeTime, extension, filePath, MeetingID);
            FileNode file01 = new FileNode(fileName, fileID, modifiedTime, "0001/01/01 00:00:00", executeTime, extension, filePath, MeetingID);
            FileNode file02 = new FileNode(fileName, fileID, min, min, min, extension, filePath, MeetingID);
            FileNode file = new FileNode(fileName, fileID, modifiedTime, createdTime, executeTime, extension, filePath, MeetingID);

            //test when input is "N / A"
            String actualcreatedTime_na = file_na.GetCreatedTimeS();
            Assert.AreEqual("", actualcreatedTime_na, "Actual created time is not empty when input is N / A");

            //test get method when input is string-type min value
            String actualcreatedTime01 = file01.GetCreatedTimeS();
            Assert.AreEqual("", actualcreatedTime01, "Actual created time is not empty when input is 0001/01/01 00:00:00");

            //test get method when input is DateTime-type min value
            String actualcreatedTime02 = file02.GetCreatedTimeS();
            Assert.AreEqual("", actualcreatedTime02, "Actual created time is not empty when input is 0001/01/01 00:00:00");

            //test get method when input is not min value or "N / A"
            String actualcreatedTime1 = file.GetCreatedTimeS();
            Assert.AreEqual("2017/04/01 01:00:00", actualcreatedTime1, "Actual created time is not 2017/04/01 01:00:00");

            //test set method when input is empty string
            file.SetCreatedTime("");
            String actualcreatedTime2 = file.GetCreatedTimeS();
            Assert.AreEqual("", actualcreatedTime2, "Actual created time is not empty when the created time is set to be empty string");

            //test set method when input is "N / A"
            file.SetCreatedTime("N / A");
            String actualcreatedTime3 = file.GetCreatedTimeS();
            Assert.AreEqual("", actualcreatedTime3, "Actual created time is not empty when the created time is set to be N / A");

            //test set method when input is MinValue
            file.SetCreatedTime("0001/01/01 00:00:00");
            String actualcreatedTime4 = file.GetCreatedTimeS();
            Assert.AreEqual("", actualcreatedTime4, "Actual created time is not empty when the created time is set to be MinValue");

            //test set method when input is not MinValue
            file.SetCreatedTime("2016/11/30 18:00:00");
            String actualcreatedTime5 = file.GetCreatedTimeS();
            Assert.AreEqual("2016/11/30 18:00:00", actualcreatedTime5, "Actual created time is not 2016/11/30 18:00:00");
        }

        [TestMethod]
        public void GetandSetExecuteTimeTest()
        {
            DateTime min = DateTime.MinValue;
            FileNode file_na = new FileNode(fileName, fileID, modifiedTime, createdTime, "N / A", extension, filePath, MeetingID);
            FileNode file01 = new FileNode(fileName, fileID, modifiedTime, createdTime, "0001/01/01 00:00:00", extension, filePath, MeetingID);
            FileNode file02 = new FileNode(fileName, fileID, min, min, min, extension, filePath, MeetingID);
            FileNode file = new FileNode(fileName, fileID, modifiedTime, createdTime, executeTime, extension, filePath, MeetingID);

            //test when input is "N / A"
            String actualexecuteTime_na = file_na.GetExecuteTimeS();
            Assert.AreEqual("", actualexecuteTime_na, "Actual execute time is not empty when input is N / A");

            //test get method when input is string-type min value
            String actualexecuteTime01 = file01.GetExecuteTimeS();
            Assert.AreEqual("", actualexecuteTime01, "Actual execute time is not empty when input is 0001/01/01 00:00:00");

            //test get method when input is DateTime-type min value
            String actualexecuteTime02 = file02.GetExecuteTimeS();
            Assert.AreEqual("", actualexecuteTime02, "Actual execute time is not empty when input is 0001/01/01 00:00:00");

            //test get method when input is not min value or "N / A"
            String actualexecuteTime1 = file.GetExecuteTimeS();
            Assert.AreEqual("2017/04/01 01:00:00", actualexecuteTime1, "Actual execute time is not 2017/04/01 01:00:00");

            //test set method when input is empty string
            file.SetExecuteTime("");
            String actualexecuteTime2 = file.GetExecuteTimeS();
            Assert.AreEqual("", actualexecuteTime2, "Actual execute time is not empty when the execute time is set to be empty string");

            //test set method when input is "N / A"
            file.SetExecuteTime("N / A");
            String actualexecuteTime3 = file.GetExecuteTimeS();
            Assert.AreEqual("", actualexecuteTime3, "Actual execute time is not empty when the execute time is set to be N / A");

            //test set method when input is MinValue
            file.SetExecuteTime("0001/01/01 00:00:00");
            String actualexecuteTime4 = file.GetExecuteTimeS();
            Assert.AreEqual("", actualexecuteTime4, "Actual execute time is not empty when the execute time is set to be MinValue");

            //test set method when input is not MinValue
            file.SetExecuteTime("2016/12/10 12:00:00");
            String actualexecuteTime5 = file.GetExecuteTimeS();
            Assert.AreEqual("2016/12/10 12:00:00", actualexecuteTime5, "Actual execute time is not 2016/12/10 12:00:00");

        }

        [TestMethod]
        public void GetandSetMissingTest()
        {
            FileNode file = new FileNode(fileName, fileID, modifiedTime, createdTime, executeTime, extension, filePath, MeetingID);

            //test get initialized true
            Boolean actualMissing1 = file.GetMissing();
            Assert.IsTrue(actualMissing1, "Actual missing is not true");

            //test set No
            file.SetMissing("No");
            Boolean actualMissing2 = file.GetMissing();
            Assert.IsFalse(actualMissing2, "Actual missing is not false");

            //test set Yes
            file.SetMissing("Yes");
            Boolean actualMissing3 = file.GetMissing();
            Assert.IsTrue(actualMissing3, "Actual missing is not true");

            //test set other string
            file.SetMissing("Maybe");
            Boolean actualMissing4 = file.GetMissing();
            Assert.IsFalse(actualMissing4, "Actual missing is not false");
        }

        [TestMethod]
        public void GetandAddMeetingListTest()
        {

        }

        [TestMethod]
        public void SetMeetingListTest()
        {

        }
    }
}