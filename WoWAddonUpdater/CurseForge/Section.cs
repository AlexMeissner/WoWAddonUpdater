namespace WoWAddonUpdater.CurseForge
{
    public class Section
    {
        public uint Id { get; set; }
        public uint GameId { get; set; }
        public string Name { get; set; }
        public uint PackageType { get; set; }
        public string Path { get; set; }
        public string InitialInclusionPattern { get; set; }
        public string ExtraIncludePattern { get; set; }
        public uint GameCategoryId { get; set; }
    }
}