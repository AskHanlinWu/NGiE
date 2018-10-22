using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGiE
{
    public class CustomPDFDocumentDetails
    {
       
        public string fileName { get; set; }
        public string fullPath { get; set; }
        public string extension { get; set; }

        public int pages { get; set; }

        public string iconFullPath { get; set; }
        /// <summary>
        /// Actual file size (length) in byte
        /// </summary>
        public long fileSizeInByte { get; set; }

        /// <summary>
        /// Converted file size from fileSizeInByte for user to read.
        /// </summary>
        public string fileSize
        {
            get
            {
                return Utils.FileSizeConvertToHumanReadableFormat(fileSizeInByte);
            }
        }

        private bool _IsPDFPossibleCorrupted;
        public bool IsPDFPossibleCorrupted
        {
            get
            {
                _IsPDFPossibleCorrupted = Utils.IsPDFPossibleCorrupted(fullPath);
                return _IsPDFPossibleCorrupted;
            }
            set
            {
                _IsPDFPossibleCorrupted = value;
            }
        }


        // Checking if PDF is password protected
        // reference - https://stackoverflow.com/questions/11298651/checking-if-pdf-is-password-protected-using-itextsharp
        public static bool IsPasswordProtected(string pdfFullname)
        {
            try
            {
                PdfReader pdfReader = new PdfReader(pdfFullname);
                return false;
            }
            catch (BadPasswordException)
            {
                return true;
            }
        }

        public static bool IsPasswordValid(string pdfFullname, string password)
        {
            try
            {
                byte[] passwordInBytes = Encoding.ASCII.GetBytes(password);

                PdfReader pdfReader = new PdfReader(pdfFullname, passwordInBytes);
                return false;
            }
            catch (BadPasswordException)
            {
                return true;
            }
        }

    }
}
