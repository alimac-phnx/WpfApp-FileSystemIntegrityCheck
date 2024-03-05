using BlakeSharpNG;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Shapes;

namespace WpfApp_FileSystemIntegrityCheck
{
    public class DirectoryChecker
    {
        private static StringBuilder allHash = new StringBuilder();

        public static string CalculateSHA256(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }
            }
        }

        public static async Task CalculateExpectedFileHash(string rootDirectory)
        {
            try
            {
                List<Task> tasks = new List<Task>();
                string[] files = Directory.GetFiles(rootDirectory);

                foreach (string filePath in files)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            //byte[] hash = ComputeSHA3(filePath);
                            using (var stream = new FileStream(filePath, FileMode.Open))
                            {
                                using (SHA256 sha256 = SHA256.Create())
                                {
                                    byte[] hashBytes = sha256.ComputeHash(stream);
                                    allHash.Append($"{filePath} {BitConverter.ToString(hashBytes).Replace("-", "").ToLower()}\n");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }));
                }

                string[] subDirectories = await Task.Run(() => Directory.GetDirectories(rootDirectory));

                foreach (string subDirectory in subDirectories)
                {
                    tasks.Add(CalculateExpectedFileHash(subDirectory));
                }

                await Task.WhenAll(tasks);
            }
            catch (UnauthorizedAccessException ex)
            {
                //$"Отсутствует доступ: {ex.Message}"
            }
            catch (Exception ex)
            {
                //$"Ошибка: {ex.Message}"
            }

            File.WriteAllText("C:\\Users\\alimac\\Desktop\\egorapakpa\\hashes.txt", allHash.ToString());
        }

        public static bool CheckIntegrity(string filePath, string expectedHash)
        {
            string actualHash = CalculateSHA256(filePath);

            return actualHash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
        }

        public static void ErrorMessage(string disp)
        {
            string caption = "Error!";
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxButton button = MessageBoxButton.OK;
            System.Windows.MessageBox.Show(disp, caption, button, icon);
        }

        public static void StartChecking(string path)
        {
            MyFileRepository.MyFiles.Clear();

            string hashFile = null;

            if (path[0] == 'C')
            {
                hashFile = "C:\\Users\\alimac\\Desktop\\egorapakpa\\hashesC.txt";
            }
            else if (path[0] == 'D')
            {
                hashFile = "C:\\Users\\alimac\\Desktop\\egorapakpa\\hashesD.txt";
            }

            //hashFile = "C:\\Users\\alimac\\Desktop\\egorapakpa\\hashes.txt";

            if (!File.Exists(hashFile))
            {
                ErrorMessage("Файл с ожидаемыми хеш-суммами не найден.");
                return;
            }

            string[] expectedHashes = File.ReadAllLines(hashFile);
            try
            {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);

                    int index = Array.FindIndex(expectedHashes, line => line.StartsWith(file, StringComparison.OrdinalIgnoreCase));

                    if (index != -1)
                    {
                        string expectedHash = expectedHashes[index].Substring(file.Length).Trim();
                        if (CheckIntegrity(file, expectedHash))
                        {
                            MyFile myFile = new MyFile(fileInfo, true);
                            MyFileRepository.AddFile(myFile);
                            //System.Windows.MessageBox.Show($"{file}: Целостность файла подтверждена.");
                        }
                        else
                        {
                            MyFile myFile = new MyFile(fileInfo, false);
                            MyFileRepository.AddFile(myFile);
                            //ErrorMessage($"{file}: Файл поврежден или изменен.");
                        }
                    }
                    else
                    {
                        MyFile myFile = new MyFile(fileInfo);
                        MyFileRepository.AddFile(myFile);
                        //ErrorMessage($"{file}: Ожидаемая хеш-сумма не найдена.");
                    }
                }

                string[] subdirectories = Directory.GetDirectories(path);
                foreach (string subdirectory in subdirectories)
                {
                    StartChecking(subdirectory);
                }
            }
            catch (UnauthorizedAccessException)
            {
                //ErrorMessage("Отсутствует доступ: " + path);
            }
            catch (Exception ex)
            {
                //ErrorMessage("Ошибка: " + ex.Message);
            }
        }
    }
}

