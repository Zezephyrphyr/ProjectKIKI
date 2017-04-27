using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KIKIXmlProcessor;

namespace KIKI
{
    /// <summary>
    /// Interaction logic for clickShowFiles.xaml
    /// </summary>
   
    public partial class clickShowFiles : Window
    {
        private string[] id;
        public clickShowFiles(string IDList)
        {
            InitializeComponent();
            XMLProcessor processor = new XMLProcessor();
            char[] delimiterChars = { ';' };
            id = IDList.Split(delimiterChars);
            foreach (string s in id)
            {
                Files.Items.Add(processor.FindFilesByFileIDs(s).Last().GetFileName());
                
            }

            
        }

        private void Files_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SystemParameters.SwapButtons) // Or use SystemInformation.MouseButtonsSwapped
            {
                // openfile
            }
            else
            {
                
                clickFileShowMeeting newWindow = new clickFileShowMeeting(id[Files.SelectedIndex]);
                newWindow.Show();
            }

        }
    }

}
