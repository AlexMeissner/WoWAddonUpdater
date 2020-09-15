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
                            LatestFile latestFile = GetValidVersion(addonData.LatestFiles);

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

        private LatestFile GetValidVersion(List<LatestFile> latestFiles)
        {
            // TODO: Get "newest" valid LatestFile

            foreach (var latestFile in latestFiles)
            {
                if (latestFile.GameVersionFlavor.Equals("wow_retail") &&
                    DoesMajorMatch(latestFile))
                {
                    return latestFile;
                }
            }

            return null;
        }

        private bool DoesMajorMatch(LatestFile latestFile)
        {
            foreach (var gameVersion in latestFile.GameVersion)
            {
                var addonVersion = new FileVersion(gameVersion);

                if (WowVersion.Major == addonVersion.Major)
                {
                    return true;
                }
            }

            foreach (var gameVersion in latestFile.SortableGameVersion)
            {
                var addonVersion = new FileVersion(gameVersion.GameVersion);

                if (WowVersion.Major == addonVersion.Major)
                {
                    return true;
                }
            }

            return latestFile.GameVersion.Count() == 0 && latestFile.SortableGameVersion.Count() == 0;
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

            const string defaultIcon = "https://blznav.akamaized.net/img/games/logo-wow-3dd2cfe06df74407.png";
            return defaultIcon;
        }
    }
}