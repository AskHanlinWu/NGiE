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
    /// Interaction logic for SinglePDFDetailsWindow.xaml
    /// </summary>
    public partial class SinglePDFDetailsWindow : Window
    {

        public string SelectedPages = string.Empty;

        public int originalTotalPages = 0;

        private bool EnableErrorLog = true;

        public SinglePDFDetailsWindow()
        {
            InitializeComponent();
        }

        public SinglePDFDetailsWindow(bool _EnableErrorLog)
        {
            try
            {
                EnableErrorLog = _EnableErrorLog;

                if (originalTotalPages > 0)
                {
                    tbSelectedPages.Text = string.Format("1-{0}", originalTotalPages);
                }
                else
                {
                    // Doc has no pages... 
                    tbSelectedPages.Text = "Error";
                    tbSelectedPages.IsEnabled = false;
                }

                InitializeComponent();
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
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

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedPages = tbSelectedPages.Text;

                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DialogResult = false;
                Close(); // close the window
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }
    }
}
