using System;
using System.IO;
using System.Net;
using System.Windows;
using System.IO.Compression;
using System.Collections.Generic;

namespace WoWAddonUpdater.Functions
{
    public class AutoUpdater
    {
        private readonly string UpdateUri = "http://togel.bplaced.net/Updates/WoWAddonUpdater.zip";
        private readonly string UpdateDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_update");
        private readonly string BackupDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_backup");
        private readonly string UpdateArchivePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_update", "update.zip");
        private DateTime UpdateDateTime = new DateTime();

        public void Run()
        {
            if (!IsUpdateAvailable() ||
                !CreateDirectory(UpdateDirectoryPath))
            {
                return;
            }

            if (!DownloadUpdate() ||
                !ExtractUpdate() ||
                !CreateDirectory(BackupDirectoryPath))
            {
                RemoveDirectory(UpdateDirectoryPath);
                return;
            }

            if (!CreateBackup())
            {
                RemoveDirectory(UpdateDirectoryPath);
                RemoveDirectory(BackupDirectoryPath);
                return;
            }

            if (!ApplyUpdate())
            {
                RemoveDirectory(UpdateDirectoryPath);
                RemoveDirectory(BackupDirectoryPath);
                Properties.Settings.Default.LastUpdate = UpdateDateTime;
                Properties.Settings.Default.Save();
                RestartApplication();
            }
            else
            {
                RestoreBackup();
            }
        }

        private bool IsUpdateAvailable()
        {
            try
            {
                HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(UpdateUri);

                using (HttpWebResponse Response = (HttpWebResponse)Request.GetResponse())
                {
                    if (Response.StatusCode == HttpStatusCode.OK)
                    {
                        UpdateDateTime = Response.LastModified;
                        return DateTime.Compare(Properties.Settings.Default.LastUpdate, UpdateDateTime) < 0;
                    }
                }
            }
            catch (Exception Error)
            {
                Logger.Exception(Error);
            }

            return false;
        }

        private bool CreateDirectory(string DirectoryPath)
        {
            try
            {
                if (Directory.Exists(DirectoryPath))
                {
                    Directory.Delete(DirectoryPath, true);
                }

                Directory.CreateDirectory(DirectoryPath);
            }
            catch (Exception Error)
            {
                Logger.Exception(Error);
                return false;
            }

            return true;
        }

        private bool DownloadUpdate()
        {
            try
            {
                using (WebClient Client = new WebClient())
                {
                    Client.DownloadFile(UpdateUri, UpdateArchivePath);
                }
            }
            catch (Exception Error)
            {
                Logger.Exception(Error);
                return false;
            }

            return true;
        }

        private bool ExtractUpdate()
        {
            try
            {
                ZipFile.ExtractToDirectory(UpdateArchivePath, UpdateDirectoryPath);
                File.Delete(UpdateArchivePath);
            }
            catch (Exception Error)
            {
                Logger.Exception(Error);
                return false;
            }

            return true;
        }

        private bool CreateBackup()
        {
            try
            {
                List<string> FilesToMove = new List<string>();

                foreach (string FilePath in Directory.GetFiles(UpdateDirectoryPath, "*", SearchOption.AllDirectories))
                {
                    FilesToMove.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                            Uri.UnescapeDataString(new Uri(UpdateDirectoryPath + Path.DirectorySeparatorChar).MakeRelativeUri(new Uri(FilePath)).ToString().Replace('/', Path.DirectorySeparatorChar))));
                }

                foreach (string FilePath in FilesToMove)
                {
                    if (File.Exists(FilePath))
                    {
                        File.Move(FilePath, Path.Combine(BackupDirectoryPath, Path.GetFileName(FilePath)));
                    }
                }
            }
            catch (Exception Error)
            {
                Logger.Exception(Error);
                return false;
            }

            return true;
        }

        private bool ApplyUpdate()
        {
            try
            {
                foreach (string FilePath in Directory.GetFiles(UpdateDirectoryPath, "*", SearchOption.AllDirectories))
                {
                    string MoveTo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(FilePath));
                    File.Move(FilePath, MoveTo);
                }
            }
            catch (Exception Error)
            {
                Logger.Exception(Error);
                return false;
            }

            return true;
        }

        private bool RemoveDirectory(string DirectoryPath)
        {
            try
            {
                if (Directory.Exists(DirectoryPath))
                {
                    Directory.Delete(DirectoryPath, true);
                }
            }
            catch (Exception Error)
            {
                Logger.Exception(Error);
                return false;
            }

            return true;
        }

        private void RestoreBackup()
        {
            try
            {
                foreach (string FilePath in Directory.GetFiles(BackupDirectoryPath, "*", SearchOption.AllDirectories))
                {
                    string MoveTo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(FilePath));
                    File.Move(FilePath, MoveTo);
                }
            }
            catch (Exception Error)
            {
                Logger.Exception(Error);
            }
        }

        private void RestartApplication()
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }
    }
}