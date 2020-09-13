using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private readonly string AddonBaseDirectory = Path.Combine(Properties.Settings.Default.BaseDirectory, @"Interface\Addons");
        private readonly string BaseAddress = "https://addons-ecs.forgesvc.net";

        public ObservableCollection<AddonViewModel> GetViewModel()
        {
            var curseData = Refresh();
            return new ObservableCollection<AddonViewModel>(curseData.Select(d => new AddonViewModel() { Name = d.Name, Blacklisted = false, CurseID = d.ID, Icon = d.Icon }));
        }

        public HashSet<CurseData> Refresh()
        {
            var addonDirectories = GetAddonDirectoryNames();
            var curseData = GetCurseForgeData(addonDirectories);

            return curseData;
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
                var fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(Properties.Settings.Default.BaseDirectory, "Wow.exe")).FileVersion;

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

        private HashSet<CurseData> GetCurseForgeData(HashSet<string> addonDirectories)
        {
            // Installed Addon Folders as input
            // If modules of current json contains any of the folder names then add to hashset
            // else ignore

            HashSet<CurseData> data = new HashSet<CurseData>();

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
                            if (addonData.LatestFiles.Count > 0)
                            {
                                foreach (var module in addonData.LatestFiles.First().Modules)
                                {
                                    if (addonDirectories.Contains(module.Foldername))
                                    {
                                        data.Add(new CurseData()
                                        {
                                            ID = addonData.Id,
                                            Name = addonData.Name,
                                            Icon = GetIcon(addonData)
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

        private string GetIcon(AddonData addonData)
        {
            try
            {
                if (addonData.Attachments.Count > 0)
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