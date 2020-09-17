using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.IO.Compression;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using WoWAddonUpdater.CurseForge;
using WoWAddonUpdater.ViewModels;

namespace WoWAddonUpdater.Functions
{
    public class AddonUpdater
    {
        private readonly FileVersion WowVersion = new FileVersion(FileVersionInfo.GetVersionInfo(Path.Combine(Properties.Settings.Default.BaseDirectory, "Wow.exe")).FileVersion);
        private readonly string AddonBaseDirectory = Path.Combine(Properties.Settings.Default.BaseDirectory, @"Interface\Addons");
        private readonly string BaseAddress = "https://addons-ecs.forgesvc.net";

        public ObservableCollection<AddonViewModel> GetViewModel()
        {
            var addonDirectories = GetAddonDirectoryNames();
            var curseData = GetCurseForgeData(addonDirectories);
            return new ObservableCollection<AddonViewModel>(curseData);
        }

        public void UpdateAddon(string downloadURL)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    string temporaryZipPath = Path.Combine(AddonBaseDirectory, @"\temp.zip");
                    webClient.DownloadFile(downloadURL, temporaryZipPath);

                    using (ZipArchive archive = ZipFile.OpenRead(temporaryZipPath))
                    {
                        foreach (ZipArchiveEntry zipEntry in archive.Entries)
                        {
                            string zipEntryPath = Path.GetFullPath(Path.Combine(AddonBaseDirectory, zipEntry.FullName));

                            if (zipEntry.Name.Length == 0)
                            {
                                Directory.CreateDirectory(zipEntryPath);
                            }
                            else
                            {
                                zipEntry.ExtractToFile(zipEntryPath, true);
                            }
                        }
                    }

                    File.Delete(temporaryZipPath);
                }
                catch (Exception exception)
                {
                    Logger.Exception(exception);
                }
            }
        }

        private HashSet<string> GetAddonDirectoryNames()
        {
            HashSet<string> addonDirectories = new HashSet<string>();

            try
            {
                if (Directory.Exists(AddonBaseDirectory))
                {
                    foreach (string addonDirectory in Directory.GetDirectories(AddonBaseDirectory))
                    {
                        if (Directory.GetFiles(addonDirectory, "*.toc").Length > 0)
                        {
                            addonDirectories.Add(Path.GetFileName(addonDirectory));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
            }

            return addonDirectories;
        }

        private HashSet<AddonViewModel> GetCurseForgeData(HashSet<string> addonDirectories)
        {
            HashSet<AddonViewModel> data = new HashSet<AddonViewModel>();

            using (HttpClient restAPI = new HttpClient())
            {
                restAPI.BaseAddress = new Uri(BaseAddress);
                restAPI.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                const uint pageSize = 10000;
                uint startIndex = 0;

                while (true)
                {
                    string request = "api/v2/addon/search?gameId=1&pageSize=" + pageSize + "&index=" + startIndex;

                    HttpResponseMessage response = restAPI.GetAsync(request).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        break;
                    }

                    try
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        List<AddonData> addons = JsonConvert.DeserializeObject<List<AddonData>>(json);

                        foreach (var addonData in addons)
                        {
                            LatestFile latestFile = GetHighestUsableLatestFile(addonData.LatestFiles);

                            if (!addonData.IsExperimental && latestFile != null)
                            {
                                foreach (var module in latestFile.Modules)
                                {
                                    if (addonDirectories.Contains(module.Foldername))
                                    {
                                        data.Add(new AddonViewModel()
                                        {
                                            CurseID = addonData.Id,
                                            Name = addonData.Name,
                                            Icon = GetIcon(addonData),
                                            AvailableVersionDate = DateTime.Parse(latestFile.FileDate),
                                            DownloadUrl = latestFile.DownloadUrl,
                                            Blacklisted = false
                                        });

                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.Exception(exception);
                    }

                    startIndex += pageSize;
                }
            }

            return data;
        }

        private LatestFile GetHighestUsableLatestFile(List<LatestFile> latestFiles)
        {
            LatestFile bestMatch = null;
            FileVersion bestMatchVersion = new FileVersion();
            DateTime bestMatchDate = new DateTime();

            foreach (var latestFile in latestFiles)
            {
                if (latestFile.GameVersionFlavor.Equals("wow_retail"))
                {
                    var version = GetHighestUsableVersion(latestFile);
                    var date = DateTime.Parse(latestFile.FileDate);

                    if (version >= bestMatchVersion && date > bestMatchDate)
                    {
                        bestMatch = latestFile;
                        bestMatchDate = date;
                        bestMatchVersion = version;
                    }
                }
            }

            return bestMatch;
        }

        private FileVersion GetHighestUsableVersion(LatestFile latestFile)
        {
            FileVersion fileVersion = new FileVersion();

            foreach (var gameVersion in latestFile.GameVersion)
            {
                var version = new FileVersion(gameVersion);

                if (version > fileVersion && version <= WowVersion)
                {
                    fileVersion = version;
                }
            }

            foreach (var gameVersion in latestFile.SortableGameVersion)
            {
                var version = new FileVersion(gameVersion.GameVersion);

                if (version > fileVersion && version <= WowVersion)
                {
                    fileVersion = version;
                }
            }

            return fileVersion;
        }

        private string GetIcon(AddonData addonData)
        {
            try
            {
                if (addonData.Attachments.Count() > 0)
                {
                    return addonData.Attachments.First().ThumbnailUrl;
                }
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
            }

            const string defaultIcon = "https://www.macupdate.com/images/icons512/38553.png";
            return defaultIcon;
        }
    }
}