using GongSolutions.Wpf.DragDrop;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace NGiE
{
    #region Release the application into one .exe file

    /* 
     * What is this about: Release the application into one .exe file
     *  
     * How: Instal both Fody 3.1.3 and Costure.Fody 3.1.0 and create FodyWeavers.xml
     * PM Manager: 
     * Install-Package Costura.Fody -Version 3.1.0
     * Install-Package Fody -Version 3.1.3
     *
     * Reference: https://github.com/Fody/Costura/issues/236 (search "I ran into this same issue yesterday (6/28/2018)..." in the thread)
     *
     */

    #endregion

    #region Animated GIF in WPF
    // https://stackoverflow.com/questions/210922/how-do-i-get-an-animated-gif-to-work-in-wpf
    #endregion

    #region Listbox Drag-drop (GONG SOLUTIONS) 
    // https://github.com/punker76/gong-wpf-dragdrop
    #endregion


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static bool EnableErrorLog = true;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }


        private void btnAddPDFs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Step 01. Get list of selected files
                List<CustomPDFDocumentDetails> lstPDFs = ImportSelectedPDFFiles();

                // Step 02 (OPTIONAL). Get PDF details (using iTextSharp)
                //lstPDFs = GetSelectedPDFFileDetails(lstPDFs);

                // Step 03. Add items to listbox
                AddSelectedPDFsToListBox(lstPDFs);
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }

        private void AddSelectedPDFsToListBox(List<CustomPDFDocumentDetails> lstPDFs)
        {
            try
            {
                if (lstPDFs != null)
                {
                    //lstbSelectedPDFs.ItemsSource = lstPDFs;

                    for (int i = 0; i < lstPDFs.Count; i++)
                    {
                        //ListBoxItem item = new ListBoxItem();
                        //item.Content = lstPDFs[i].fileName;
                        //item.ToolTip = lstPDFs[i].fileName;

                        lstPDFs[i].iconFullPath = @"..\Images\pdf.PNG";

                        dgPDFs.Items.Add(lstPDFs[i]);

                    }



                }
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }



        public List<CustomPDFDocumentDetails> GetSelectedPDFFileDetails(List<CustomPDFDocumentDetails> lstPDFs)
        {
            try
            {
                PdfReader.unethicalreading = true;

                for (int i = 0; i < lstPDFs.Count; i++)
                {
                    string filePath = lstPDFs[i].fullPath;

                    if (File.Exists(filePath))
                    {
                        PdfReader pdfReader = new PdfReader(filePath);
                        // TO DO: get PDF details with iTextSharp

                        pdfReader.Close();
                    }
                }

                return lstPDFs;
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
                return lstPDFs;
            }
        }

        private List<CustomPDFDocumentDetails> ImportSelectedPDFFiles()
        {
            try
            {
                List<CustomPDFDocumentDetails> lstPDFs = new List<CustomPDFDocumentDetails>();

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true; // Allow user to select multiple PDF files 

                ofd.Filter = "PDF Files|*.pdf"; // strictly limit selection to PDF files

                bool? result = ofd.ShowDialog();

                if (result.HasValue && result.Value)
                {
                    // This will handle single select as well as multiple select
                    for (int i = 0; i < ofd.FileNames.Length; i++)
                    {
                        CustomPDFDocumentDetails pdf = new CustomPDFDocumentDetails();

                        pdf.fullPath = ofd.FileNames[i]; // this is FULL file path
                        pdf.fileName = System.IO.Path.GetFileNameWithoutExtension(ofd.SafeFileNames[i]); // this is only file name WITHOUT extension               
                        pdf.extension = System.IO.Path.GetExtension(pdf.fullPath); // this will be: .pdf
                        pdf.fileSizeInByte = new System.IO.FileInfo(pdf.fullPath).Length; // this will get actual file size in Byte

                        if (pdf.extension.ToLower() == ".pdf")
                        {
                            lstPDFs.Add(pdf);
                        }
                    }

                    return lstPDFs;
                }

                return lstPDFs;
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
                return null;
            }
        }


        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //The files that we are working with
                string sourceFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                string sourceFile = lblPDF1FullPath.Content.ToString();
                string destFile = System.IO.Path.Combine(sourceFolder, "temp.pdf");
                // action 1. split
                KeepSelectedPages(lblPDF1FullPath.Content.ToString(), "1-8", destFile);

                string finalFile = System.IO.Path.Combine(sourceFolder, "final.pdf");

                // action 2. merge
                IEnumerable<string> filenames = new string[] { destFile, "full path of file that will be combined" };
                MergePDFs(filenames, finalFile);
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }


        /// <summary>
        /// PDF Page Spliter/Remover/Breaker
        /// 
        /// Purpose: Get rid of selected pages in PDF file. This feature can be understood as "remove pages from PDF"
        /// 
        /// In real life, user can just use Adobe PDF reader and print selected pages as PDF file.
        /// 
        /// Reference: https://stackoverflow.com/questions/7246137/itextsharp-trimming-pdf-documents-pages  (Mathew Leger's answer)
        /// </summary>
        /// <param name="inputPdf">Needs to be fullPath</param>
        /// <param name="pageSelection">"1-4" or "1-4,6" or "1-6,!5"</param>
        /// <param name="outputPdf">Needs to be fullPath</param>
        public void KeepSelectedPages(string inputPDFFullPath, string pageSelection, string outputPDFFullPath)
        {
            try
            {
                using (PdfReader reader = new PdfReader(inputPDFFullPath))
                {
                    reader.SelectPages(pageSelection);

                    using (PdfStamper stamper = new PdfStamper(reader, File.Create(outputPDFFullPath)))
                    {
                        stamper.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }

        /// <summary>
        /// https://stackoverflow.com/questions/6573069/initializing-ienumerablestring-in-c-sharp
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="targetPdf"></param>
        /// <returns></returns>
        public static bool MergePDFs(IEnumerable<string> fileNames, string targetPdf)
        {
            bool mergeSuccess = true;
            using (FileStream stream = new FileStream(targetPdf, FileMode.Create))
            {
                Document document = new Document();
                PdfCopy pdf = new PdfCopy(document, stream);
                PdfReader reader = null;
                try
                {
                    document.Open();
                    document.SetPageSize(PageSize.A4);

                    foreach (string file in fileNames)
                    {
                        reader = new PdfReader(file);
                        pdf.AddDocument(reader);
                        reader.Close();
                    }
                }
                catch (Exception)
                {
                    mergeSuccess = false;
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
                finally
                {
                    if (document != null)
                    {
                        document.Close();
                    }
                }
            }
            return mergeSuccess;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //EnableWindowBackgroundBlur();
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }

        #region AERO Effect (Blur transparent background)

        /// <summary>
        /// This is part of AERO effect (blurry + transparent background)
        /// 
        /// Reference https://stackoverflow.com/questions/31151539/native-aero-blur-without-glass-effect-on-borderless-wpf-window?rq=1
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
        internal enum AccentState
        {
            ACCENT_DISABLED = 1,
            ACCENT_ENABLE_GRADIENT = 0,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }
        internal void EnableWindowBackgroundBlur()
        {
            try
            {
                WindowInteropHelper windowHelper = new WindowInteropHelper(this);

                AccentPolicy accent = new AccentPolicy();
                accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

                int accentStructSize = Marshal.SizeOf(accent);

                IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
                Marshal.StructureToPtr(accent, accentPtr, false);

                WindowCompositionAttributeData data = new WindowCompositionAttributeData();
                data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
                data.SizeOfData = accentStructSize;
                data.Data = accentPtr;

                SetWindowCompositionAttribute(windowHelper.Handle, ref data);

                Marshal.FreeHGlobal(accentPtr);
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }

        #endregion


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

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }

        private void btnDeletePDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RemoveSelectedPDFFromDataGrid();
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }

        private void RemoveSelectedPDFFromDataGrid()
        {
            try
            {
                if (dgPDFs.SelectedItem != null)
                {
                    int gridItemIndex = 0;

                    while (dgPDFs.SelectedItems.Count > 0)
                    {
                        gridItemIndex = dgPDFs.Items.IndexOf(dgPDFs.SelectedItem);
                        dgPDFs.Items.RemoveAt(gridItemIndex);
                    }

                    // Select the previous item if there is still item left in the grid
                    if (dgPDFs.Items.Count > 0)
                    {
                        if (gridItemIndex != 0)
                        {
                            gridItemIndex = gridItemIndex - 1;
                        }
                        SelectDataGridItemByRowIndex(gridItemIndex);
                    }

                }
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }

        /// <summary>
        /// Programitically select DataGrid Row
        /// Reference - https://stackoverflow.com/questions/1976087/wpf-datagrid-set-selected-row
        /// </summary>
        /// <param name="gridItemIndex"></param>
        private void SelectDataGridItemByRowIndex(int gridItemIndex)
        {
            try
            {
                DataGridRow row = (DataGridRow)dgPDFs.ItemContainerGenerator.ContainerFromIndex(gridItemIndex);
                object item = dgPDFs.Items[gridItemIndex];
                dgPDFs.SelectedItem = item;
                dgPDFs.ScrollIntoView(item);
                row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessageToFile(ex.ToString(), EnableErrorLog);
            }
        }

        private void dgPDFs_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Delete)
                {
                    RemoveSelectedPDFFromDataGrid();
                    e.Handled = true;
                }

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

    }
}
