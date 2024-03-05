using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WpfApp_FileSystemIntegrityCheck
{
    public partial class Page1 : System.Windows.Controls.Page
    {
        public Page1()
        {
            InitializeComponent();
        }
        
        private void ClearField(System.Windows.Controls.TextBox fieldName)
        {
            fieldName.Text = null;

            Brush brush = Brushes.White;
            fieldName.Background = brush;
        }

        private void OpenDirectory()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            if (folderBrowser.ShowDialog() == DialogResult.Cancel)
                return;

            folderPath.Text = folderBrowser.SelectedPath;
        }

        private async void OpenDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            ClearField(filePath);

            OpenDirectory();

            CheckIntegrityButton.IsEnabled = false;

            string filePathToCheck = folderPath.Text;
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            //await DirectoryChecker.CalculateExpectedFileHash(filePathToCheck);
            stopwatch.Stop();

            System.Windows.MessageBox.Show($"Время выполнения: {stopwatch.Elapsed}");
            CheckIntegrityButton.IsEnabled = true;
        }

        public void OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text documents (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.FilterIndex = 2;

            dialog.ShowDialog();

            filePath.Text = dialog.FileName;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        public void DamageFile(string filePath)
        {
            Random random = new Random();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                long fileSize = fileStream.Length;

                RandomByteChange(random, fileSize, fileStream);
            }
        }

        public void RandomByteChange(Random random, long fileSize, FileStream fileStream)
        {
            for (int i = 0; i < 4; i++)
            {
                long position = random.Next(0, (int)fileSize);

                byte newByte = (byte)random.Next(0, 256);

                fileStream.Seek(position, SeekOrigin.Begin);
                fileStream.WriteByte(newByte);
            }
        }

        private void DamageFileButton_Click(object sender, RoutedEventArgs e)
        {
            DamageFile(filePath.Text);

            Brush brush = Brushes.Coral;
            filePath.Background = brush;
        }

        private void CheckIntegrityButton_Click(object sender, RoutedEventArgs e)
        {
            DirectoryChecker.StartChecking(folderPath.Text);
            NavigationService.Navigate(new Page2(folderPath.Text));
        }

        private void Label_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
