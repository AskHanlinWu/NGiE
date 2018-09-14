using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGiE
{
    public class Utils
    {
        public static void WriteErrorMessageToFile(string ErrorMessage, bool EnableLogging)
        {
            string sourceFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string currentDirectory = Environment.CurrentDirectory + "\\ErrorLog";

            string path = Path.Combine(currentDirectory, "ErrorLog.txt");

            if (!Directory.Exists(currentDirectory))
            {
                Directory.CreateDirectory(currentDirectory);
            }

            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine("\n" + ErrorMessage);
                sw.Close();
            }
        }

        /// <summary>
        /// Reference: https://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
        /// </summary>
        /// <param name="fileSizeInByte"></param>
        /// <returns></returns>
        public static string FileSizeConvertToHumanReadableFormat(long fileSizeInByte)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = fileSizeInByte;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0.##} {1}", len, sizes[order]);

            return result;
        }

        /// <summary>
        /// This checks potential corrupted PDF by checking it's header
        /// https://stackoverflow.com/questions/3108201/detect-if-pdf-file-is-correct-header-pdf
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsPDFPossibleCorrupted(string fileName)
        {
            byte[] buffer = null;
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            //buffer = br.ReadBytes((int)numBytes);
            buffer = br.ReadBytes(5);

            var enc = new ASCIIEncoding();
            var header = enc.GetString(buffer);

            //%PDF−1.0
            // If you are loading it into a long, this is (0x04034b50).
            if (buffer[0] == 0x25 && buffer[1] == 0x50
                && buffer[2] == 0x44 && buffer[3] == 0x46)
            {
                return header.StartsWith("%PDF-");
            }
            return false;

        }

    }
}
