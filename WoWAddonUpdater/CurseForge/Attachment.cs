namespace WoWAddonUpdater.CurseForge
{
    public class Attachment
    {
        public uint Id { get; set; }
        public uint ProjectId { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public uint Status { get; set; }
    }
}