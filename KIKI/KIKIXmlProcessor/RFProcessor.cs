using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace KIKIXmlProcessor
{
    class RFProcessor : XMLProcessor
    {
        private String tempPath = "temp.xml";
        private String RSFile = "records.xml";
        private LinkedList<FileNode> fileList;
        Byte sync = 0;

        public Boolean GetRFXML(String start, String end)
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


        public void ReadRecords() {
            Console.WriteLine("Program start reading recent file history...");
            XmlTextReader reader = null;

            try
            {

                // Load the reader with the data file and ignore all white space nodes.         
                reader = new XmlTextReader(RSFile);
                sync = 1;
                int filter = 0; //the bit used to filter out entries
                reader.WhitespaceHandling = WhitespaceHandling.None;
                FileNode tempNode = new FileNode();
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "item")
                            {
                            }
                            Console.WriteLine("1");
                            Console.WriteLine("<{0}>", reader.Name);
                            break;
                        case XmlNodeType.Text:
                            Console.WriteLine("2");
                            Console.WriteLine(reader.Value);
                            break;
                        case XmlNodeType.EndElement:
                            Console.WriteLine("10");
                            Console.WriteLine("</{0}>", reader.Name);
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
            Console.WriteLine(line);
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

        public static void Main(string[] args)
        {
            
            RFProcessor x = new RFProcessor();
            x.GetRFXML("hello","world"); //date and time not implemented 
            while (!File.Exists(x.tempPath)) ;
            Thread.Sleep(1000); //temporary solution for process conflicts
            x.RemoveXMLdeclaration();
            while (x.sync == 1) ;
            x.ReadRecords();
            
         
        }
    }
}
