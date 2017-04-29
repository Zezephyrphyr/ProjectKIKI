using Microsoft.VisualStudio.TestTools.UnitTesting;
using KIKIXmlProcessor;

namespace KIKIXMLProcessorUnitTest
{
    [TestClass]
    public class XMLSearcherUnitTest
    {
        [TestMethod]
        //The test only test the constructor of the searcher
        //The rest are tested through simulation of the user interface
        public void TestConstructor()
        {
            XMLSearcher search = new XMLSearcher("");
            Assert.AreEqual(search.GetFfile(), "files.xml");
            Assert.AreEqual(search.GetMfile(), "meetings.xml");
            XMLSearcher search2 = new XMLSearcher("C:/Downloads/");
            Assert.AreEqual(search2.GetFfile(), "C:/Downloads/files.xml");
            Assert.AreEqual(search2.GetMfile(), "C:/Downloads/meetings.xml");
        }


    }
}

