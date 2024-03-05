using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_FileSystemIntegrityCheck
{
    public class MyFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string CheckState { get; set; }
        public string ErrorType { get; set; }

        public MyFile(FileInfo file, bool flagCheckState) 
        {
            Name = file.Name;   
            Path = file.FullName;

            if (flagCheckState)
            {
                CheckState = "Целостность подтверждена";
                ErrorType = "-";
            }
            else
            {
                CheckState = "Файл был поврежден";
                ErrorType = "Byte sequence error";
            }
        }

        public MyFile(FileInfo file)
        {
            Name = file.Name;
            Path = file.FullName;

            CheckState = "Unknown";

            ErrorType = "Ожидаемая хэш-сумма не найдена";
        }
    }
}
