using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIKIXmlProcessor
{
    class FileNode
    {
        private string fileName = "";
        private DateTimeOffset modifiedTime;
        private DateTimeOffset createdTime;
        private DateTimeOffset executeTime;
        private Boolean missing = true;
        private String extension = "";
        private String filePath = "";

    }
}
