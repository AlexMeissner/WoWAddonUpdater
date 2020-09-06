namespace WoWAddonUpdater.CurseForge
{
    public class Author
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public uint ProjectId { get; set; }
        public uint Id { get; set; }
        public uint? ProjectTitleId { get; set; }
        public string ProjectTitle { get; set; }
        public uint UserId { get; set; }
        public uint? TwitchId { get; set; }
    }
}