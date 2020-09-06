using System.Collections.Generic;

namespace WoWAddonUpdater.CurseForge
{
    public class LatestFile
    {
        public Dependency Dependency { get; set; }
        public Module Module { get; set; }
        public GameVersionSortable GameVersionSortable { get; set; }
        public uint Id { get; set; }
        public string DisplayName { get; set; }
        public string FileName { get; set; }
        public string FileDate { get; set; }
        public uint FileLength { get; set; }
        public uint ReleaseType { get; set; }
        public uint FileStatus { get; set; }
        public string DownloadUrl { get; set; }
        public bool IsAlternate { get; set; }
        public uint AlternateFileId { get; set; }
        public List<Dependency> Dependencies { get; set; }
        public bool IsAvailable { get; set; }
        public List<Module> Modules { get; set; }
        public uint PackageFingerprint { get; set; }
        public List<string> GameVersion { get; set; }
        public List<GameVersionSortable> SortableGameVersion { get; set; }
        public string InstallMetaData { get; set; }
        public string Changelog { get; set; }
        public bool HasInstallScript { get; set; }
        public bool IsCompatibleWithClient { get; set; }
        public uint CategorySectionPackageType { get; set; }
        public uint RestrictProjectFileAccess { get; set; }
        public uint ProjectStatus { get; set; }
        public uint? RenderCacheId { get; set; }
        public uint? FileLegacyMappingId { get; set; }
        public uint ProjectId { get; set; }
        public uint? ParentProjectFileId { get; set; }
        public uint? ParentFileLegacyMappingId { get; set; }
        public uint? FileTypeId { get; set; }
        public string ExposeAsAlternative { get; set; }
        public uint PackageFingerprintId { get; set; }
        public string GameVersionDateReleased { get; set; }
        public uint GameVersionMappingId { get; set; }
        public uint GameVersionId { get; set; }
        public uint GameId { get; set; }
        public bool IsServerPack { get; set; }
        public uint? ServerPackFileId { get; set; }
        public string GameVersionFlavor { get; set; }
    }
}
