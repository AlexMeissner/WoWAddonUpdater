namespace WoWAddonUpdater.CurseForge
{
    public class Category
    {
        public uint CategoryId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string AvatarUrl { get; set; }
        public uint ParentId { get; set; }
        public uint RootId { get; set; }
        public uint ProjectId { get; set; }
        public uint AvatarId { get; set; }
        public uint GameId { get; set; }
    }
}