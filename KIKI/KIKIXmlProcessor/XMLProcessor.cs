using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace KIKIXmlProcessor
{
    public class XMLProcessor
    {
        private String mfile = "meetings.xml";
        private String ffile = "files.xml";
        private String sfile = "Settings.xml";
        private String workingPath = "";
        private String tempPath = "temp.xml";
        private String RSFile = "records.xml";
        private LinkedList<FileNode> fileList = new LinkedList<FileNode>();
        private LinkedList<MeetingNode> meetingList = new LinkedList<MeetingNode>();
        private LinkedList<FileNode> AttachmentList = new LinkedList<FileNode>();
        private LinkedList<FileNode> fileFinalList = new LinkedList<FileNode>();
        private DateTime lastUpdate = new DateTime();
        private int lastID = 0;
        private TimeSpan minDuration = new TimeSpan(0, 1, 0);

        public XMLProcessor()
        {
            if (!File.Exists(sfile))
            {
                WriteSettings();
            }
            else
            {
                GetInfoFromSettings();
            }
        }

        #region Basic settings read, update and get
        // ---------------------------Read Basic Settings -------------------------------------

        public void GetInfoFromSettings()
        {
            XElement settingsFile = XElement.Load(sfile);
            workingPath = settingsFile.Element("Working_Path").Value;
            if (settingsFile.Element("Last_Update").Value != "Uninitialized")
            {
                lastUpdate = StringToTime(settingsFile.Element("Last_Update").Value);
            }
            lastID = Convert.ToInt32(settingsFile.Element("Last_File_ID").Value);
            if (settingsFile.Element("Minimum_Duration").Value != "")
            {
                String[] s1 = settingsFile.Element("Minimum_Duration").Value.Split(';');
                TimeSpan newDuration = new TimeSpan(Convert.ToInt32(s1[0]),
                    Convert.ToInt32(s1[1]), Convert.ToInt32(s1[2]));
                minDuration = newDuration;
            }
            UpdateFilePath();
        }

        public TimeSpan GetMinimumDuration()
        {
            return minDuration;
        }

        public String GetWorkingPath()
        {
            return workingPath;
        }

        public int GetLastUpdateId()
        {
            return lastID;
        }

        public DateTime GetLastUpdateTime()
        {
            return lastUpdate;
        }


        public void UpdateFilePath()
        {
            mfile = workingPath + "meetings.xml";
            ffile = workingPath + "files.xml";
            tempPath = workingPath + "temp.xml";
            RSFile = workingPath + "records.xml";
        }

        public DateTime StringToTime(String s)
        {
            if (s == "")
            {
                return new DateTime();
            }
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

        public Boolean FirstUse()
        {
            if (!((File.Exists(mfile)) || (File.Exists(ffile))))
            {
                if (!File.Exists(ffile))
                {
                    WriteNewFilesFile();
                }
                return true;
            }
            return false;
        }
        //-----------------------------------------------------------------------------------------
        #endregion

        #region Read file list data from recent files view
        // ------------Read Recent File Records From the System----------------------------------
        // Post condition: A linked list of roughly sorted file data
        public void Read()
        {
            GetRFXML();
            FileInfo i = new FileInfo(tempPath);
            while (IsFileLocked(i)) { };
            RemoveXMLdeclaration();
            FileInfo i2 = new FileInfo(RSFile);
            while (IsFileLocked(i2)) { };
            ReadRecords();
        }

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
                int filter = 0; //the bit used to filter out entries
                reader.WhitespaceHandling = WhitespaceHandling.None;
                String fileName = "";
                String extension = "";
                String filePath = "";
                String mTime = "";
                String cTime = "";
                String eTime = "";
                String missing = "";
                String stored_in = "";
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
                            if (tag == 5) { missing = reader.Value; } //missing?
                            if (tag == 6) { stored_in = reader.Value; } //stored_in
                            //Console.WriteLine(reader.Value);
                            break;
                        case XmlNodeType.EndElement:
                            if ((filter == 0) && (reader.Name == "item"))
                            {
                                if ((IsValid(fileName, filePath, extension, stored_in)) && (IsValidTimeRange(mTime, cTime, eTime, lastUpdate, DateTime.Now) != -1))
                                {
                                    FileNode tempNode = new FileNode();
                                    tempNode.SetFileName(fileName);
                                    tempNode.SetCreatedTime(cTime);
                                    tempNode.SetExecuteTime(eTime);
                                    tempNode.SetModifiedTime(mTime);
                                    tempNode.SetFilePath(filePath);
                                    tempNode.SetMissing(missing);
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
            Console.WriteLine("Reading Finished...");
        }

        public void RemoveXMLdeclaration()
        {
            Console.WriteLine("Program start formatting raw recent file data...");
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

        #endregion

        #region Check if the data is valid
        //filter, checking the validity of the information
        public Boolean IsValid(String fName, String fPath, String ext, String stored_in)
        {
            if (fName == "")
            {
                return false;
            }
            if (ext == "")
            {
                return false;
            }
            if (stored_in == "Registry")
            {
                return false;
            }
            return true;
        }

        //-1: The file is invalid for this time range
        //1: The file is created in this time range
        //0: The file is modified in this time range
        public int IsValidTimeRange(String mTime, String cTime, String eTime, DateTime sTime, DateTime edTime)
        {
            DateTime mt = new DateTime();
            DateTime ct = new DateTime();
            DateTime et = new DateTime();
            if ((mTime != "N / A") && (mTime != ""))
            {
                mt = StringToTime(mTime);
            }
            if ((cTime != "N / A") && (cTime != ""))
            {
                ct = StringToTime(cTime);
            }
            if ((eTime != "N / A") && (eTime != ""))
            {
                et = StringToTime(eTime);
            }
            // if all three time not within the range, return invalid
            if (((DateTime.Compare(edTime, ct) < 0) || (DateTime.Compare(ct, sTime) < 0))
                && ((DateTime.Compare(edTime, et) < 0) || (DateTime.Compare(et, sTime) < 0))
                && ((DateTime.Compare(edTime, mt) < 0) || (DateTime.Compare(mt, sTime) < 0)))
            {
                return -1;
            }
            //if the file is created after the start time and before the end time
            //Count the file as created in the meeting 
            if ((DateTime.Compare(sTime, ct) < 0) && (DateTime.Compare(ct, edTime) < 0))
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }

        public int IsValidTimeRange(DateTime ct, DateTime et, DateTime mt, DateTime sTime, DateTime edTime)
        {
            // if all three time not within the range, return invalid
            if (((DateTime.Compare(edTime, ct) < 0) || (DateTime.Compare(ct, sTime) < 0))
                && ((DateTime.Compare(edTime, et) < 0) || (DateTime.Compare(et, sTime) < 0))
                && ((DateTime.Compare(edTime, mt) < 0) || (DateTime.Compare(mt, sTime) < 0)))
            {
                return -1;
            }
            //if the file is created after the start time and before the end time
            //Count the file as created in the meeting 
            if ((DateTime.Compare(sTime, ct) < 0) && (DateTime.Compare(ct, edTime) < 0))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        //test if the event is a meeting
        //Condition: The duration of the meeting should exceed 1 minute
        public Boolean IsValidMeeting(MeetingNode m)
        {
            TimeSpan duration = m.GetDuration();
            if (duration.CompareTo(minDuration) < 0)
            {
                return false;
            }
            return true;
        }


        #endregion
        //---------------------------------------------------------------------------------------------

        //-------------------------------File and Meeting Process ---------------------------------
        //Previous Condition: List of File information from recent file viewer, list of file information from google attachments, and meeting list from google calendar
        //Post Condition: List of New Files with sorted ID, List of Old Files with previous IDs, List of meetings. All filelist and meetinglist in nodes updated

        public void ProcessFileWithMeetingNode(MeetingNode mNode)
        {
            //check if the meeting node is valid or not first
            if (!IsValidMeeting(mNode))
            {
                return;
            }

            String meetingID = mNode.GetMeetingID();
            if (meetingID == "")
            {
                Console.WriteLine("Meeting ID for Meeting " + mNode.GetMeetingTitle() + "is missing.");
                return;
            }
            LinkedList<FileNode> filteredFileList = new LinkedList<FileNode>();
            DateTime startTime = mNode.GetStartTime();
            DateTime endTime = mNode.GetEndTime();
            foreach (FileNode item in fileList)
            {
                if (IsValidTimeRange(item.GetCreatedTime(), item.GetExecuteTime(), item.GetModifiedTime(), startTime, endTime) != -1)
                {
                    item.AddMeetings(meetingID);
                    filteredFileList.AddLast(item);
                }
            }
            foreach (FileNode Aitem in AttachmentList)
            {
                if (Aitem.GetMeetingListS() == mNode.GetMeetingID())
                {
                    filteredFileList.AddLast(Aitem);
                }
            }
            filteredFileList = AssignFileIDs(filteredFileList);
            foreach (FileNode node in filteredFileList)
            {
                fileFinalList.AddLast(node);
                mNode.AddFiles(node.GetFileID());
            }
            meetingList.AddLast(mNode);
        }

        public LinkedList<FileNode> AssignFileIDs(LinkedList<FileNode> fList)
        {
            int llid = lastID;
            LinkedList<FileNode> nList = fList;
            XElement filesFile = XElement.Load(ffile);
            IEnumerable<XElement> fileNodes = filesFile.Elements();
            //deal with nodes already exist
            int nodeCount = nList.Count();
            int i = 0;
            foreach (XElement xEle in fileNodes)
            {
                String xEleName = xEle.Element("File_Name").Value;
                String xElePath = xEle.Element("File_Path").Value;
                foreach (FileNode node in nList)
                {
                    if (node.GetFileID() == 0)
                    {
                        String nodePath = node.GetFilePath();
                        String nodeName = node.GetFileName();
                        //if the node name and node path matches with each other -- same file
                        if ((xEleName == nodeName) && (xElePath == nodePath))
                        {
                            node.SetFileID(xEle.Attribute("ID").Value);
                            i = i + 1;
                            break;
                        }
                    }
                }
                if (i == nodeCount)
                {
                    break;
                }
            }

            if (i != nodeCount)
            {
                //check if the node is in final list 
                foreach (FileNode node in nList)
                {
                    foreach (FileNode nNode in fileFinalList)
                    {
                        //if the node is in the final list
                        if ((node.GetFileName() == nNode.GetFileName()) && (node.GetFilePath() == nNode.GetFilePath()))
                        {
                            node.SetFileID(nNode.GetFileID());
                        }
                    }
                    if (node.GetFileID() == 0)
                    {
                        node.SetFileID(llid + 1);
                        llid++;
                    }
                }
            }
            return nList;
        }

        /*
        public Boolean IsRename(FileNode oldFile, FileNode newFile)
        {
            if (oldFile.GetCreatedTime() != newFile.GetCreatedTime())
            {
                return false;
            }
            //if the old file is not missing, the file is not renamed
            if (!oldFile.GetMissing())
            {
                return false;
            }
            return true;
        }*/

        public void ProcessFileWithMeetingList(LinkedList<MeetingNode> mList, LinkedList<FileNode> attachmentList)
        {
            AttachmentList = attachmentList;
            lastUpdate = DateTime.Now;
            foreach (MeetingNode meeting in mList)
            {
                ProcessFileWithMeetingNode(meeting);             
            }
        }

        //----------------------------------------------------------------------------------------------------------------------


        //--------------------Write/Update Data To the XML files--------------------------
        //Previous Condition: The file list and meeting list are processed
        //Previous Condition: File list: New list(new files) assigned with sorted id/ Old list(new file access for previously accessed files)

        //test if the user has previously used the software and generate the data
        //return true if neither of mfile and ffile is generated

        #region Settings.xml Write and Update

        public void WriteSettings()
        {
            XElement s = new XElement("Settings",
                new XElement("Working_Path", ""),
                new XElement("Last_Update", "Uninitialized"),
                new XElement("Last_File_ID", "0"),
                new XElement("Minimum_Duration", ""));
            XDocument xDoc = new XDocument(
            new XDeclaration("1.0", "UTF-16", null), s);
            StringWriter sw = new StringWriter();
            XmlWriter xWrite = XmlWriter.Create(sw);
            xDoc.Save(xWrite);
            xWrite.Close();

            // Save to Disk
            xDoc.Save("Settings.xml");
            Console.WriteLine("New Settings.xml Created...");
        }

        //----------------------------------Update Settings File------------------------------
        public void UpdateSettingsTime(DateTime time)
        {
            String year = time.Year.ToString("0000");
            String month = time.Month.ToString("00");
            String day = time.Day.ToString("00");
            String hour = time.Hour.ToString("00");
            String minute = time.Minute.ToString("00");
            String second = time.Second.ToString("00");

            String timeS = year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second;

            XElement settingsFile = XElement.Load(sfile);
            settingsFile.Element("Last_Update").ReplaceNodes(timeS);
            settingsFile.Save(sfile);
        }

        //WAITING TO BE RESOLVED
        public void UpdateSettingsPath(String WP)
        {
            workingPath = WP;
            //UpdateFilePath();
            XElement settingsFile = XElement.Load("Settings.xml");
            settingsFile.Element("Working_Path").ReplaceNodes(WP);
            settingsFile.Save(sfile);
        }

        public void UpdateLastID(int lastID)
        {

            XElement settingsFile = XElement.Load("Settings.xml");
            settingsFile.Element("Last_File_ID").ReplaceNodes(lastID.ToString());
            settingsFile.Save(sfile);
        }

        public void UpdateSettingsDuration(TimeSpan duration)
        {

            minDuration = duration;
            XElement settingsFile = XElement.Load("Settings.xml");
            String s = duration.Hours.ToString() + ";" +
                duration.Minutes.ToString() + ";" +
                duration.Seconds.ToString();
            settingsFile.Element("Minimum_Duration").ReplaceNodes(s);
            settingsFile.Save(sfile);
        }

        //-------------------------------------------------------------------------------------
        #endregion

        public void Write()
        {
            FileInfo i = new FileInfo(mfile);
            FileInfo i2 = new FileInfo(ffile);
            FileInfo i3 = new FileInfo(sfile);

            while (IsFileLocked(i)) { };
            WriteMeetings(meetingList);

            while (IsFileLocked(i2)) { };
            WriteFiles(fileFinalList);

            while (IsFileLocked(i3)) { };
            UpdateSettingsTime(lastUpdate);
            UpdateLastID(lastID);
            Console.WriteLine("Finished...");
        }

        #region Meetings.xml Write and Update
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
            foreach (MeetingNode temp in mts)
            {
                XElement mt = new XElement("Meeting_Title", temp.GetMeetingTitle()); //meeting title element
                XElement st = new XElement("Start_Time", temp.GetStartTimeS());
                XElement et = new XElement("End_Time", temp.GetEndTimeS());
                XElement pID = new XElement("Parent_ID", temp.GetParentID());
                XElement atds = new XElement("Attendents", temp.GetAttendents());
                XElement f = new XElement("Files", temp.GetFileListS());
                String mID = temp.GetMeetingID();
                XElement item = new XElement("Item", mt, st, et, pID, atds, f, new XAttribute("ID", mID));
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
            Console.WriteLine("New Meeting File Saved");
        }

        //Update the current xml file with name specified in mfile with input list of meeting nodes
        //preassumption: all the attributes are stored in the meeting nodes, and empty string implies null value
        public void UpdateMeetings(LinkedList<MeetingNode> mts)
        {
            XElement meetingFile = XElement.Load(mfile);
            foreach (MeetingNode temp in mts)
            {
                XElement mt = new XElement("Meeting_Title", temp.GetMeetingTitle()); //meeting title element
                XElement st = new XElement("Start_Time", temp.GetStartTimeS());
                XElement et = new XElement("End_Time", temp.GetEndTimeS());
                XElement pID = new XElement("Parent_ID", temp.GetParentID());
                XElement atds = new XElement("Attendents", temp.GetAttendents());
                XElement f = new XElement("Files", temp.GetFileListS());
                String mID = temp.GetMeetingID();
                XElement item = new XElement("Item", mt, st, et, pID, atds, f, new XAttribute("ID", mID));
                meetingFile.Add(item);
            }
            meetingFile.Save(mfile);
            Console.WriteLine("Meeting File Updated");
        }

        #endregion

        #region Files.xml Write and Update
        public void WriteFiles(LinkedList<FileNode> fts)
        {
            if (!File.Exists(ffile))
            {
                WriteNewFilesFile();
            }
            UpdateFiles(fts);
        }

        //Write a new xml file with name specified in ffile with input list of file nodes
        //preassumption: all the information are stored in filelist and the id of filenodes are sorted from smallest to largest
        public void WriteNewFilesFile()
        {
            XElement f = new XElement("File");
            XDocument xDoc = new XDocument(
                        new XDeclaration("1.0", "UTF-16", null), f);
            StringWriter sw = new StringWriter();
            XmlWriter xWrite = XmlWriter.Create(sw);
            xDoc.Save(xWrite);
            xWrite.Close();
            // Save to Disk
            xDoc.Save(ffile);
            Console.WriteLine("New Files.xml Created...");
        }

        //Update the current xml file with name specified in ffile with input list of new file nodes and list of file nodes already existed
        //preassumption: all the attributes are stored in the file nodes, the new file node list has increasing file id
        public void UpdateFiles(LinkedList<FileNode> fts)
        {
            XElement filesFile = XElement.Load(ffile);
            int currentLast = lastID;
            //deal with nodes already exist
            foreach (FileNode temp in fts)
            {

                if (temp.GetFileID() <= currentLast)
                {
                    Console.WriteLine(currentLast);
                    //previous node
                    var previousNode = from id in filesFile.Elements("Item")
                                       where (string)id.Attribute("ID") == Convert.ToString(temp.GetFileID())
                                       select id;
                    foreach (XElement pN in previousNode)
                    {

                        pN.Element("File_Name").ReplaceNodes(temp.GetFileName());
                        pN.Element("File_Path").ReplaceNodes(temp.GetFilePath());
                        pN.Element("Modified_Time").ReplaceNodes(temp.GetModifiedTimeS());
                        pN.Element("Execute_Time").ReplaceNodes(temp.GetExecuteTimeS());
                        pN.Element("Missing").ReplaceNodes(temp.GetMissing().ToString());
                        if (pN.Element("Meetings").Value == "")
                        {
                            pN.Element("Meetings").ReplaceNodes(temp.GetMeetingListS());
                        }
                        else if (temp.GetMeetingListS() != "")
                        {
                            String s = pN.Element("Meetings").Value + ";" + temp.GetMeetingListS();
                            pN.Element("Meetings").ReplaceNodes(s);
                        }
                    }
                }
                else
                {
                    //new Node
                    XElement fn = new XElement("File_Name", temp.GetFileName()); //meeting title element
                    XElement fp = new XElement("File_Path", temp.GetFilePath());
                    XElement ct = new XElement("Created_Time", temp.GetCreatedTimeS());
                    XElement mt = new XElement("Modified_Time", temp.GetModifiedTimeS());
                    XElement et = new XElement("Execute_Time", temp.GetExecuteTimeS());
                    XElement ext = new XElement("Extension", temp.GetExtension());
                    XElement mis = new XElement("Missing", temp.GetMissing().ToString());
                    XElement ms = new XElement("Meetings", temp.GetMeetingListS());
                    XAttribute fID = new XAttribute("ID", temp.GetFileID());
                    XElement item = new XElement("Item", fn, fp, ct, mt, et, ext, mis, ms, fID);
                    currentLast = temp.GetFileID();
                    filesFile.Add(item);
                }
            }
            filesFile.Save(ffile);
            lastID = currentLast;
            Console.WriteLine("Files.xml Updated...");
        }
        #endregion

        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------

        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        //------------------------------Testing Area--------------------------------------------------
        //----------------------------For Developer Only -------------------------------------------------

        public void SetFileList(LinkedList<FileNode> testList)
        {
            fileList = testList;
        }
        public void XmlSample()
        {
            XElement m = new XElement("Meeting");
            XElement mt = new XElement("Meeting_Title", "test");

            XElement st = new XElement("StartTime", "2017/01/01 01:01:01");
            XElement et = new XElement("EndTime", "2017/01/01 01:01:01");
            XElement pID = new XElement("ParentID", "345");
            XElement atds = new XElement("Attendents", "Stupid Eddie");
            XElement f = new XElement("Files", "1;2;3;4;5");
            XElement mID = new XElement("Item", mt, st, et, pID, atds, f, new XAttribute("ID", "4567"));
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

            XElement s1 = new XElement("Settings", new XElement("Working_Path", ""), new XElement("Last_Update", "Uninitialized"));
            XDocument xDoc2 = new XDocument(
            new XDeclaration("1.0", "UTF-16", null), s1);
            StringWriter sw2 = new StringWriter();
            XmlWriter xWrite2 = XmlWriter.Create(sw2);
            xDoc2.Save(xWrite2);
            xWrite2.Close();

            // Save to Disk
            xDoc2.Save("Settings.xml");
            Console.WriteLine("SettingsSaved");

            XElement settingsFile = XElement.Load("Settings.xml");
            Console.WriteLine(settingsFile.Element("Working_Path").Value);
            Console.WriteLine(settingsFile.Element("Last_Update").Value);
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
            
            XMLProcessor x = new XMLProcessor();
            x.Read();
            foreach (FileNode node in x.fileList)
            {
                Console.WriteLine(node.GetFileName());
            }
            Uri.IsWellFormedOriginalString()
        }
    }
}
/*
FileNode a = new FileNode();
a.SetFileName("HELLO");
FileNode b = new FileNode();
b.SetFileName("WORLD");
LinkedList<FileNode> k = new LinkedList<FileNode>();
k.AddLast(a);
k.AddLast(b);
foreach (FileNode item in k)
{
    Console.WriteLine(item.GetFileName());
}
//x.XmlSample();*/
/*
XMLProcessor test = new XMLProcessor();
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
Console.WriteLine(LID);
Console.WriteLine(TS.Minutes);
Console.WriteLine("Path is " + WP);
*/
/*
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
            XMLProcessor processor = new XMLProcessor();
            processor.FirstUse();
            processor.SetFileList(fn);
            processor.ProcessFileWithMeetingNode(node);
            processor.Write();
*/
