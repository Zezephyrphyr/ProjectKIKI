using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KIKIXmlProcessor
{
    public class XMLSearcher
    {
        private String mfile = "meetings.xml";
        private String ffile = "files.xml";
        public XMLSearcher(String WorkingPath)
        {
            mfile = WorkingPath + mfile;
            ffile = WorkingPath + ffile;
        }
        //-----------------------------------------Search Algorithm ------------------------------------------------
        //------------------Read information from XML and return linked list of desired data ----------------------------

        //find corresponding meeting nodes with a string of meeting IDs
        //previous condition: no repetitive meeting ids in the string
        public LinkedList<MeetingNode> FindMeetingsByMeetingIDs(String meetingIDs)
        {
            if (meetingIDs == "")
            {
                return new LinkedList<MeetingNode>();
            }
            String[] idList = meetingIDs.Split(';');
            XElement meetings = XElement.Load(mfile);
            IEnumerable<XElement> meetingNodes = meetings.Elements();
            LinkedList<MeetingNode> list = new LinkedList<MeetingNode>();
            // Read the entire XML
            foreach (var meeting in meetingNodes)
            {
                String currentID = meeting.Attribute("ID").Value;
                Boolean inList = false;
                for (int i = 0; i < idList.Length && (!inList); i++)
                {
                    if (currentID == idList[i])
                    {
                        inList = true;
                    }
                }
                if (inList)
                {
                    MeetingNode currentNode = new MeetingNode();
                    currentNode.SetMeetingID(currentID);
                    currentNode.SetMeetingTitle(meeting.Element("Meeting_Title").Value);
                    currentNode.SetStartTime(meeting.Element("Start_Time").Value);
                    currentNode.SetEndTime(meeting.Element("End_Time").Value);
                    currentNode.SetAttendents(meeting.Element("Attendents").Value);
                    currentNode.SetFiles(meeting.Element("Files").Value);
                    list.AddLast(currentNode);
                }
            }
            //the count of the list indicate whether the list is empty
            return list;
        }

        //Search file by its path and returns the linked list of realted meeting information 
        public LinkedList<MeetingNode> FindMeetingsByFilePath(String filePath)
        {
            XElement fileList = XElement.Load(ffile);
            IEnumerable<XElement> fileNodes = fileList.Elements();
            String meetingIDs = "";
            foreach (var node in fileNodes)
            {
                if (node.Element("File_Path").Value == filePath)
                {
                    meetingIDs = node.Element("Meetings").Value;
                    break;
                }
            }
            return FindMeetingsByMeetingIDs(meetingIDs);
        }


        public LinkedList<MeetingNode> FindMeetingsByFileID(String fileID)
        {
            XElement fileList = XElement.Load(ffile);
            IEnumerable<XElement> fileNodes = fileList.Elements();
            String meetingIDs = "";
            foreach (var node in fileNodes)
            {
                if (node.Attribute("ID").Value == fileID)
                {
                    meetingIDs = node.Element("Meetings").Value;
                    break;
                }
            }
            return FindMeetingsByMeetingIDs(meetingIDs);
        }

        public LinkedList<FileNode> FindFilesByFileIDs(String fileIDs)
        {
            if (fileIDs == "")
            {
                return new LinkedList<FileNode>();
            }
            String[] idList = fileIDs.Split(';');
            XElement files = XElement.Load(ffile);
            IEnumerable<XElement> fileNodes = files.Elements();
            LinkedList<FileNode> list = new LinkedList<FileNode>();
            // Read the entire XML
            foreach (var file in fileNodes)
            {
                String currentID = file.Attribute("ID").Value;
                Boolean inList = false;
                for (int i = 0; i < idList.Length && (!inList); i++)
                {
                    if (currentID == idList[i])
                    {
                        inList = true;
                    }
                }
                if (inList)
                {
                    FileNode currentNode = new FileNode();
                    currentNode.SetFileID(currentID);
                    currentNode.SetCreatedTime(file.Element("Created_Time").Value);
                    currentNode.SetModifiedTime(file.Element("Modified_Time").Value);
                    currentNode.SetExecuteTime(file.Element("Execute_Time").Value);
                    currentNode.SetFileName(file.Element("File_Name").Value);
                    currentNode.SetFilePath(file.Element("File_Path").Value);
                    currentNode.SetExtension(file.Element("Extension").Value);
                    currentNode.SetMeetings(file.Element("Meetings").Value);
                    list.AddLast(currentNode);
                }
            }
            //the count of the list indicate whether the list is empty
            return list;
        }

        public LinkedList<FileNode> FindFilesByMeetingID(String meetingID)
        {
            XElement meetingList = XElement.Load(mfile);
            IEnumerable<XElement> meetingNodes = meetingList.Elements();
            String fileIDs = "";
            foreach (var node in meetingNodes)
            {
                if (node.Attribute("ID").Value == meetingID)
                {
                    fileIDs = node.Element("Files").Value;
                    break;
                }
            }
            return FindFilesByFileIDs(fileIDs);
        }

        public LinkedList<MeetingNode> FindMeetingsByMeetingPID(String meetingPID)
        {
            XElement meetings = XElement.Load(mfile);
            IEnumerable<XElement> meetingNodes = meetings.Elements();
            LinkedList<MeetingNode> list = new LinkedList<MeetingNode>();
            // Read the entire XML
            foreach (var meeting in meetingNodes)
            {
                if ((meeting.Element("Parent_ID").Value == meetingPID) || (meeting.Attribute("ID").Value == meetingPID))
                {
                    MeetingNode currentNode = new MeetingNode();
                    currentNode.SetMeetingID(meeting.Attribute("ID").Value);
                    currentNode.SetMeetingTitle(meeting.Element("Meeting_Title").Value);
                    currentNode.SetStartTime(meeting.Element("Start_Time").Value);
                    currentNode.SetEndTime(meeting.Element("End_Time").Value);
                    currentNode.SetAttendents(meeting.Element("Attendents").Value);
                    currentNode.SetFiles(meeting.Element("Files").Value);
                    list.AddLast(currentNode);
                }
            }
            //the count of the list indicate whether the list is empty
            return list;
        }

        public LinkedList<FileNode> FindFilesByMeetingPID(String meetingPID)
        {
            XElement meetings = XElement.Load(mfile);
            IEnumerable<XElement> meetingNodes = meetings.Elements();
            String[] fileIDs = new string[0];
            // Read the entire XML
            foreach (var meeting in meetingNodes)
            {
                if ((meeting.Element("Parent_ID").Value == meetingPID) || (meeting.Attribute("ID").Value == meetingPID))
                {
                    String[] files = meeting.Element("Files").Value.Split(';');
                    fileIDs = fileIDs.Concat(files).ToArray();
                }
            }
            fileIDs = fileIDs.Distinct().ToArray();
            String pFileIDs = "";
            if (fileIDs.Length != 0)
            {
                for (int i = 0; i < fileIDs.Length; i++)
                {
                    pFileIDs = pFileIDs + fileIDs[i];
                    pFileIDs = pFileIDs + ";";
                }
                pFileIDs = pFileIDs.Remove(pFileIDs.Length - 1);
            }
            return FindFilesByFileIDs(pFileIDs);
        }

        public LinkedList<MeetingNode> FindMeetingsByMeetingTitleKeywords(String keyword)
        {
            XElement meetings = XElement.Load(mfile);
            IEnumerable<XElement> meetingNodes = meetings.Elements();
            LinkedList<MeetingNode> list = new LinkedList<MeetingNode>();
            // Read the entire XML
            foreach (var meeting in meetingNodes)
            {
                if (meeting.Element("Meeting_Title").Value.Contains(keyword))
                {
                    MeetingNode currentNode = new MeetingNode();
                    currentNode.SetMeetingID(meeting.Attribute("ID").Value);
                    currentNode.SetMeetingTitle(meeting.Element("Meeting_Title").Value);
                    currentNode.SetStartTime(meeting.Element("Start_Time").Value);
                    currentNode.SetEndTime(meeting.Element("End_Time").Value);
                    currentNode.SetAttendents(meeting.Element("Attendents").Value);
                    currentNode.SetFiles(meeting.Element("Files").Value);
                    list.AddLast(currentNode);
                }
            }
            //the count of the list indicate whether the list is empty
            return list;
        }

        //keyword should not be empty string
        public LinkedList<FileNode> FindFilesByFileNameKeywords(String keyword)
        {
            XElement fileList = XElement.Load(ffile);
            IEnumerable<XElement> fileNodes = fileList.Elements();
            LinkedList<FileNode> list = new LinkedList<FileNode>();
            foreach (var file in fileNodes)
            {
                if (file.Element("File_Name").Value.Contains(keyword))
                {
                    FileNode currentNode = new FileNode();
                    currentNode.SetFileID(file.Attribute("ID").Value);
                    currentNode.SetCreatedTime(file.Element("Created_Time").Value);
                    currentNode.SetModifiedTime(file.Element("Modified_Time").Value);
                    currentNode.SetExecuteTime(file.Element("Execute_Time").Value);
                    currentNode.SetFileName(file.Element("File_Name").Value);
                    currentNode.SetFilePath(file.Element("File_Path").Value);
                    currentNode.SetExtension(file.Element("Extension").Value);
                    currentNode.SetMeetings(file.Element("Meetings").Value);
                    list.AddLast(currentNode);
                }
            }
            return list;
        }

        //-----------------------------------------------------------------------------------------------------------------

    }
}
