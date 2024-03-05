using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;

namespace WpfApp_FileSystemIntegrityCheck
{
    public partial class Page2 : Page
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]

        static extern bool GetDiskFreeSpace(string lpRootPathName,

        out uint lpSectorsPerCluster,
        out uint lpBytesPerSector,
        out uint lpNumberOfFreeClusters,
        out uint lpTotalNumberOfClusters);

        public Page2(string directoryPath)
        {
            InitializeComponent();

            foreach (var myFile in MyFileRepository.MyFiles)
            {
                dataGridXAML.Items.Add(myFile);
            }

            GetSystemParameters(directoryPath);
        }

        public void GetSystemParameters(string directoryPath)
        {
            DriveInfo drive = new DriveInfo(System.IO.Path.GetPathRoot(directoryPath));

            fileSystemTypeAnswer.Content = drive.DriveFormat;

            driveSizeAnswer.Content = $"{Math.Round(drive.TotalSize / Math.Pow(10, 9), 0)} ГБ";

            uint sectorsPerCluster, bytesPerSector, numberOfFreeClusters, totalNumberOfClusters;

            if (GetDiskFreeSpace(directoryPath, out sectorsPerCluster, out bytesPerSector, out numberOfFreeClusters, out totalNumberOfClusters))
            {
                sectorSizeAnswer.Content = $"{bytesPerSector} байт";
            }
        }

        public void Serialization(List<MyFile> myFiles)
        {

            string jsonFilePath = "inegrity-verivication-result.json";

            string jsonString = JsonConvert.SerializeObject(myFiles, Formatting.Indented);

            File.WriteAllText(jsonFilePath, jsonString);
        }

        private void ToJSONButton_Click(object sender, RoutedEventArgs e)
        {
            Serialization(MyFileRepository.MyFiles);
        }



        private void dataGridXAML_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
