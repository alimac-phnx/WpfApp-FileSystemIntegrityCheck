using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_FileSystemIntegrityCheck
{
    public class MyFileRepository
    {
        private static List<MyFile> myFiles = new List<MyFile>();

        public static List<MyFile> MyFiles
        {
            get { return myFiles; }
            set { myFiles = value; }
        }

        public static void AddFile(MyFile myFile)
        {
            MyFiles.Add(myFile);
        }
    }
}
