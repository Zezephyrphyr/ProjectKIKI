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

namespace KIKI
{
    /// <summary>
    /// Interaction logic for clickMeetingShowFile.xaml
    /// </summary>
    public partial class clickMeetingShowFile : Window
    {
        public clickMeetingShowFile()
        {
            InitializeComponent();
            FileList.ItemsSource = null;
        }

        private void listView_SelectionChanged(Object sender, EventArgs e)
        {

        }
        private void textBox_TextChanged(Object sender, EventArgs e)
        {

        }
    }
}
