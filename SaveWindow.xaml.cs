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

namespace NGiE
{
    /// <summary>
    /// Interaction logic for SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {

        private bool EnableErrorLog = true;

        public SaveWindow()
        {
            InitializeComponent();
        }

        public SaveWindow(bool _EnableErrorLog)
        {
            EnableErrorLog = _EnableErrorLog;
            InitializeComponent();
        }

        private void stackPnlHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // This allow user to drag application
                if (e.ChangedButton == MouseButton.Left)
                {
                    this.DragMove();
                }
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close(); // close the application           
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }

      
    }
}
