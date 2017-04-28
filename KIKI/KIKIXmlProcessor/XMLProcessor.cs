using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace KIKIXmlProcessor
{
    class XMLProcessor
    {
        private String mfile = "meetings.xml";
        private String ffile = "files.xml";
        private String workingPath = "";
        private String tempPath = "temp.xml";
        private String RSFile = "records.xml";
        private LinkedList<FileNode> fileList = new LinkedList<FileNode>();
        Byte sync = 0;

        public XMLProcessor()
        {
        }

        // ------------Read Recent File Records From the System----------------------------------
        // Post condition: A linked list of roughly sorted file data
        public Boolean GetRFXML()
        {
            Console.WriteLine("Program start fetching recent file history from the system...");
            String command = "/sxml temp.xml";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "RecentFilesView.exe";
            startInfo.Arguments = "/C " + command;
            //startInfo.Arguments = command;
            process.StartInfo = startInfo;
            try
            {
                process.Start();
            }
            catch
            {
                sync = 0;
                return false;

            }
            Console.WriteLine("Fetching Finished...");
            return true;
        }

        public void ReadRecords()
        {
            Console.WriteLine("Program start reading recent file history...");
            XmlTextReader reader = null;

            try
            {

                // Load the reader with the data file and ignore all white space nodes.         
                reader = new XmlTextReader(RSFile);
                sync = 1;
                int filter = 0; //the bit used to filter out entries
                reader.WhitespaceHandling = WhitespaceHandling.None;
                String fileName = "";
                String extension = "";
                String filePath = "";
                String mTime = "";
                String cTime = "";
                String eTime = "";
                //the flag used to indicate what is the current tag: 
                int tag = -1;
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            tag = GetFlag(reader.Name);
                            if (tag == 0)
                            {
                                filter = 0;
                                fileName = "";
                                extension = "";
                                filePath = "";
                            }
                            if (tag == -2)
                            {
                                filter = 1;
                            }
                            //Console.WriteLine("<{0}>", reader.Name);
                            break;
                        case XmlNodeType.Text:
                            if (tag == 1) { filePath = reader.Value; } // full name of the file indicate the path of the file
                            if (tag == 8) { fileName = reader.Value; } //the file_only property indicate the name of the file
                            if (tag == 7) { extension = reader.Value; } // the extension of the file
                            if (tag == 2) { mTime = reader.Value; } //modified Time
                            if (tag == 3) { cTime = reader.Value; } //created Time
                            if (tag == 4) { eTime = reader.Value; } //execute Time
                            //Console.WriteLine(reader.Value);
                            break;
                        case XmlNodeType.EndElement:
                            if ((filter == 0) && (reader.Name == "item"))
                            {
                                if (IsValid(fileName, filePath, extension, mTime, cTime, eTime))
                                {
                                    FileNode tempNode = new FileNode();
                                    tempNode.SetFileName(fileName);
                                    LinkedListNode<FileNode> node = new LinkedListNode<FileNode>(tempNode);
                                    // Console.WriteLine(tempNode.GetFilename());
                                    fileList.AddLast(node);
                                }

                            }
                            // Console.WriteLine("</{0}>", reader.Name);
                            break;
                        default:
                            break;
                            /*
                            case XmlNodeType.CDATA:
                                Console.WriteLine("3");
                                Console.WriteLine("<![CDATA[{0}]]>", reader.Value);
                                break;
                            case XmlNodeType.ProcessingInstruction:
                                Console.WriteLine("4");
                                Console.WriteLine("<?{0} {1}?>", reader.Name, reader.Value);
                                break;
                            case XmlNodeType.Comment:
                                Console.WriteLine("5");
                                Console.WriteLine("<!--{0}-->", reader.Value);
                                break;
                            case XmlNodeType.XmlDeclaration:
                                Console.WriteLine("6");
                                Console.WriteLine("<?xml version='1.0'?>");
                                break;
                            case XmlNodeType.Document:
                                Console.WriteLine("7");
                                break;
                            case XmlNodeType.DocumentType:
                                Console.WriteLine("8");
                                Console.WriteLine("<!DOCTYPE {0} [{1}]", reader.Name, reader.Value);
                                break;
                            case XmlNodeType.EntityReference:
                                Console.WriteLine("9");
                                Console.WriteLine(reader.Name);
                                break;*/

                    }
                }
            }

            finally
            {
                if (reader != null)
                    reader.Close();
            }
            sync = 0;
            Console.WriteLine("Reading Finished...");
        }

        public void RemoveXMLdeclaration()
        {
            Console.WriteLine("Program start formatting raw recent file data...");
            sync = 1;
            StreamReader sr = new StreamReader(tempPath);
            sr.ReadLine();

            string body = null;
            string line = sr.ReadLine();
            while (line != null) // read file into body string
            {
                body += line + "\n";
                line = sr.ReadLine();
            }
            sr.Close(); //close file

            //Write all of the "body" to the same text file
            System.IO.File.WriteAllText(RSFile, body);
            File.Delete(tempPath);
            Console.WriteLine("Fomatting Finished...");
            sync = 0;
        }

        //The flag value of corresponding string tag
        public int GetFlag(String value)
        {
            if (value == "item") { return 0; }
            if (value == "filename") { return 1; }
            if (value == "modified_time") { return 2; }
            if (value == "created_time") { return 3; }
            if (value == "execute_time") { return 4; }
            if (value == "missing_file") { return 5; }
            if (value == "stored_in") { return 6; }
            if (value == "extension") { return 7; }
            if (value == "file_only") { return 8; }
            return -2;
        }

        //filter, checking the validity of the information
        public Boolean IsValid(String fName, String fPath, String ext, String mTime, String cTime, String eTime)
        {
            if (fName == "")
            {
                return false;
            }
            if (ext == "")
            {
                return false;
            }
            return true;
        }
        //----------------------------------------------------------------------------------------------

        //-------------------------------File and Meeting Process ---------------------------------
        //Previous Condition: List of File information from recent file viewer, list of file information from google attachments, and meeting list from google calendar
        //Post Condition: List of New Files with sorted ID, List of Old Files with previous IDs, List of meetings. All filelist and meetinglist in nodes updated





        public int GetLastFileID()
        {
            int last = -1;
            if (File.Exists(mfile))
            {
                XElement fileFile = XElement.Load(ffile);
                var node = fileFile.Descendants("Item").Last();
                last = Convert.ToInt32(node.Attribute("ID").Value);
            }
            return last;
        }

        //----------------------------------------------------------------------------------------------------------------------


        //--------------------Write/Update Data To the XML files--------------------------
        //Previous Condition: The file list and meeting list are processed
        //Previous Condition: File list: New list(new files) assigned with sorted id/ Old list(new file access for previously accessed files)
        public Boolean FirstUse()
        {
            if (!((File.Exists(mfile)) || (File.Exists(ffile))))
            {
                return true;
            }
            return false;
        }

        //A method store the linked list of meetings into the database
        public void WriteMeetings(LinkedList<MeetingNode> mts)
        {
            if (File.Exists(mfile))
            {
                UpdateMeetings(mts);
            }
            else
            {
                WriteNewMeetingsFile(mts);
            }

        }

        //Write a new xml file with name specified in mfile with input list of meeting nodes
        //preassumption: all the attributes are stored in the meeting nodes, and empty string implies null value
        //FOR TEST: PRINT SAVED AFTER THE METHOD DONE
        public void WriteNewMeetingsFile(LinkedList<MeetingNode> mts)
        {
            XElement m = new XElement("Meeting");
            for (LinkedListNode<MeetingNode> node = mts.First; node != mts.Last; node = node.Next)
            {
                MeetingNode temp = node.Value;
                XElement mt = new XElement("Meeting_Title", temp.GetMeetingTitle()); //meeting title element
                XElement st = new XElement("Start_Time", temp.GetStartTimeS());
                XElement et = new XElement("End_Time", temp.GetEndTimeS());
                XElement d = new XElement("Duration", temp.GetDuration().ToString());
                XElement pID = new XElement("Parent_ID", temp.GetParentID().ToString());
                XElement atds = new XElement("Attendents", temp.GetAttendents());
                XElement f = new XElement("Files", temp.GetFileListS());
                String mID = temp.GetMeetingID();
                XElement item = new XElement("Item", mt, et, d, pID, atds, f, new XAttribute("ID", mID));
                m.Add(item);
            }
            XDocument xDoc = new XDocument(
                        new XDeclaration("1.0", "UTF-16", null), m);
            StringWriter sw = new StringWriter();
            XmlWriter xWrite = XmlWriter.Create(sw);
            xDoc.Save(xWrite);
            xWrite.Close();

            // Save to Disk
            xDoc.Save(mfile);
            Console.WriteLine("Saved");
        }


        //Write a new xml file with name specified in ffile with input list of file nodes
        //preassumption: all the information are stored in filelist and the id of filenodes are sorted from smallest to largest
        //FOR TEST: PRINT SAVED AFTER THE METHOD DONE
        public void WriteNewFilesFile(LinkedList<FileNode> fs)
        {
            XElement f = new XElement("File");
            for (LinkedListNode<FileNode> node = fs.First; node != fs.Last; node = node.Next)
            {
                FileNode temp = node.Value;
                XElement fn = new XElement("File_Name", temp.GetFileName()); //meeting title element
                XElement fp = new XElement("File_Path", temp.GetFilePath());
                XElement ct = new XElement("Created_Time", temp.GetCreatedTimeS());
                XElement mt = new XElement("Modified_Time", temp.GetModifiedTimeS());
                XElement et = new XElement("Execute_Time", temp.GetExecuteTimeS());
                XElement ext = new XElement("Extension", temp.GetExtension());
                XElement ms = new XElement("Meetings", temp.GetMeetingListS());
                XAttribute fID = new XAttribute("ID", temp.GetFileID());
                XElement item = new XElement("Item", fn, fp, ct, mt, et, ext, ms, fID);
                f.Add(item);
            }
            XDocument xDoc = new XDocument(
                        new XDeclaration("1.0", "UTF-16", null), f);
            StringWriter sw = new StringWriter();
            XmlWriter xWrite = XmlWriter.Create(sw);
            xDoc.Save(xWrite);
            xWrite.Close();
            // Save to Disk
            xDoc.Save(ffile);
            Console.WriteLine("Saved");
        }

        //Update the current xml file with name specified in ffile with input list of new file nodes and list of file nodes already existed
        //preassumption: all the attributes are stored in the file nodes, the new file node list has increasing file id
        //FOR TEST: PRINT SAVED AFTER THE METHOD DONE
        public void UpdateFiles(LinkedList<FileNode> ftsN, LinkedList<FileNode> ftsO)
        {
            XElement filesFile = XElement.Load(ffile);
            //deal with nodes already existexecute
            for (LinkedListNode<FileNode> node = ftsO.First; node != ftsO.Last; node = node.Next)
            {
                FileNode temp = node.Value;
                var previousNode = from id in filesFile.Elements("Item")
                                   where (string)id.Attribute("ID") == Convert.ToString(temp.GetFileID())
                                   select id;
                foreach (XElement pN in previousNode)
                {
                    pN.Element("File_Name").ReplaceNodes(temp.GetFileName());
                    pN.Element("File_Path").ReplaceNodes(temp.GetFilePath());
                    pN.Element("Modified_Time").ReplaceNodes(temp.GetModifiedTimeS());
                    pN.Element("Execute_Time").ReplaceNodes(temp.GetExecuteTimeS());
                    pN.Element("Meetings").ReplaceNodes(temp.GetMeetingListS());
                }
            }
            //deal with new nodes
            for (LinkedListNode<FileNode> node = ftsN.First; node != ftsN.Last; node = node.Next)
            {
                FileNode temp = node.Value;
                XElement fn = new XElement("File_Name", temp.GetFileName()); //meeting title element
                XElement fp = new XElement("File_Path", temp.GetFilePath());
                XElement ct = new XElement("Created_Time", temp.GetCreatedTimeS());
                XElement mt = new XElement("Modified_Time", temp.GetModifiedTimeS());
                XElement et = new XElement("Execute_Time", temp.GetExecuteTimeS());
                XElement ext = new XElement("Extension", temp.GetExtension());
                XElement ms = new XElement("Meetings", temp.GetMeetingListS());
                XAttribute fID = new XAttribute("ID", temp.GetFileID());
                XElement item = new XElement("Item", fn, fp, ct, mt, et, ext, ms, fID);
                filesFile.Add(item);
            }
            filesFile.Save(ffile);
        }


        //Update the current xml file with name specified in mfile with input list of meeting nodes
        //preassumption: all the attributes are stored in the meeting nodes, and empty string implies null value
        //FOR TEST: PRINT SAVED AFTER THE METHOD DONE
        public void UpdateMeetings(LinkedList<MeetingNode> mts)
        {
            XElement meetingFile = XElement.Load(mfile);
            for (LinkedListNode<MeetingNode> node = mts.First; node != mts.Last; node = node.Next)
            {
                MeetingNode temp = node.Value;
                XElement mt = new XElement("Meeting_Title", temp.GetMeetingTitle()); //meeting title element
                XElement st = new XElement("Start_Time", temp.GetStartTimeS());
                XElement et = new XElement("End_Time", temp.GetEndTimeS());
                XElement d = new XElement("Duration", temp.GetDuration().ToString());
                XElement pID = new XElement("Parent_ID", temp.GetParentID().ToString());
                XElement atds = new XElement("Attendents", temp.GetAttendents());
                XElement f = new XElement("Files", temp.GetFileListS());
                String mID = temp.GetMeetingID();
                XElement item = new XElement("Item", mt, et, d, pID, atds, f, new XAttribute("ID", mID));
                meetingFile.Add(item);
            }
            meetingFile.Save(mfile);
        }

        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------


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
                    //currentNode.SetDuration(meeting.Element("Duration").Value);
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
                    //currentNode.SetDuration(meeting.Element("Duration").Value);
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
                    //currentNode.SetDuration(meeting.Element("Duration").Value);
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

        //------------------------------Testing Area--------------------------------------------------
        //----------------------------For Developer Only -------------------------------------------------

        public void XmlSample()
        {
            XElement m = new XElement("Meeting");
            XElement mt = new XElement("Meeting_Title", "test");

            XElement st = new XElement("StartTime", "2017/01/01 01:01:01");
            XElement et = new XElement("EndTime", "2017/01/01 01:01:01");
            XElement d = new XElement("Duration", "");
            XElement pID = new XElement("ParentID", "345");
            XElement atds = new XElement("Attendents", "Stupid Eddie");
            XElement f = new XElement("Files", "1;2;3;4;5");
            XElement mID = new XElement("Item", mt, et, d, pID, atds, f, new XAttribute("ID", "4567"));
            m.Add(mID);
            XDocument xDoc = new XDocument(
                        new XDeclaration("1.0", "UTF-16", null), m);
            StringWriter sw = new StringWriter();
            XmlWriter xWrite = XmlWriter.Create(sw);
            xDoc.Save(xWrite);
            xWrite.Close();

            // Save to Disk
            xDoc.Save(mfile);
            Console.WriteLine("Saved");

            /*
            //modify meeting file
            XElement xEle = XElement.Load(mfile);
            xEle.Add(new XElement("Item",
                new XElement("EmpId", 5),
                new XElement("Phone", "423-555-4224"), new XAttribute("ID", "1234")));
            xEle.Save(mfile);
            */


            //search meeting by ID           
            XElement xelement1 = XElement.Load(mfile);
            var try1 = from id in xelement1.Elements("Item")
                       where (string)id.Attribute("ID") == "3456"
                       select id;
            Console.WriteLine("Details:");
            foreach (XElement xEle1 in try1)
            {
                Console.WriteLine(xEle1);
            }


            // add file id
            XElement xelement = XElement.Load(mfile);
            foreach (XElement xEle1 in try1)
            {
                String s = xEle1.Element("Files").Value;
                s = s + ";13";
                xEle1.Element("Files").ReplaceNodes(s);
                Console.Write(xEle1);
            }

            XElement meetingFile = XElement.Load(mfile);
            var node = meetingFile.Descendants("Item").Last();
            Console.Write("test");
            Console.Write(node.Attribute("ID").Value);

            Console.Write("test2");
            XElement meetings = XElement.Load(mfile);
            IEnumerable<XElement> meetingNodes = meetings.Elements();
            // Read the entire XML
            foreach (var meeting in meetingNodes)
            {
                //String currentID = meeting.Attribute("ID").Value;

                Console.WriteLine(meeting.Element("Duration").Value);
            }

            /*
                XElement xelement = XElement.Load("Employees.xml");
                var name = from nm in xelement.Elements("Employee")
                           where (string)nm.Element("Sex") == "Female"
                           select nm;
                Console.WriteLine("Details of Female Employees:");
                foreach (XElement xEle1 in name)
                    Console.WriteLine(xEle1);
                //Console.Write(xEle);
                */
        }

        //for test main method
        public static void Main(string[] args)
        {

            //XMLProcessor x = new XMLProcessor();
            /*
            x.GetRFXML(); //date and time not implemented 
            while (!File.Exists(x.tempPath)) ;
            Thread.Sleep(1000); //temporary solution for process conflicts
            x.RemoveXMLdeclaration();
            while (x.sync == 1) ;
            x.ReadRecords();

            for (LinkedListNode<FileNode> node = x.fileList.First; node != x.fileList.Last.Next; node = node.Next)
            {
                Console.WriteLine(node.Value.GetFileName());

            }*/

            /*-----------------------------------------------------------------------------------------------
            String mTitle = "EECS395";
            String mID = "05_1";
            String sTime = "2017/4/1 17:30:05";
            String eTime = "2017/4/1 19:10:05";
            String PID = "05";
            String Attendents = "Steven, Eddie, Xiaoying";
            Int32 FileID = 1;

            MeetingNode meeting = new MeetingNode(mTitle, mID, sTime, eTime, PID, Attendents);

            Console.WriteLine(meeting.GetMeetingTitle());
            Console.WriteLine(meeting.GetMeetingID());
            Console.WriteLine(meeting.GetStartTime());
            Console.WriteLine(meeting.GetEndTime());

            Console.WriteLine(meeting.GetFileList());

            meeting.AddFiles(2);
            meeting.AddFiles(3);
            meeting.AddFiles(5);
            meeting.AddFiles(6);
            Console.WriteLine(meeting.GetFileList());
            Console.WriteLine(meeting.GetFileListS());
            -----------------------------------------------------------------------------------------------*/

            String fileName = "test.txt";
            String fileID = "1";
            String modifiedTime = "2017/04/01 03:00:00";
            String createdTime = "2017/04/01 01:00:00";
            String executeTime = "2017/04/01 05:00:00";
            Boolean missing = true;
            String extension = ".txt";
            String filePath = "";
            String MeetingID = "01";

            FileNode file2 = new FileNode(fileName, fileID, modifiedTime, createdTime, executeTime, extension, filePath, MeetingID);
            //file.AddMeetings("01");
            Console.WriteLine(file2.MeetingList.Count);
            Console.WriteLine(file2.GetMeetingList());
            Console.WriteLine(file2.GetMeetingListS());
            Console.WriteLine(missing.ToString());

            //LinkedList<Stirng> list = new Linked


            //x.XmlSample();


        }
    }
}