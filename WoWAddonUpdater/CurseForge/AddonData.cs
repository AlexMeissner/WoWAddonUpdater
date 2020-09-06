using System.Collections.Generic;

namespace WoWAddonUpdater.CurseForge
{
    public class AddonData
    {
        public Author Author { get; set; }
        public Attachment Attachment { get; set; }
        public LatestFile LatestFile { get; set; }
        public Category Category { get; set; }
        public Section Section { get; set; }
        public GameVersionLatestFile GameVersionLatestFile { get; set; }
        public uint Id { get; set; }
        public string Name { get; set; }
        public List<Author> Authors { get; set; }
        public List<Attachment> Attachments { get; set; }
        public string WebsiteUrl { get; set; }
        public uint GameId { get; set; }
        public string Summary { get; set; }
        public uint DefaultFileId { get; set; }
        public uint DonwloadCount { get; set; }
        public List<LatestFile> LatestFiles { get; set; }
        public List<Category> Categories { get; set; }
        public uint Status { get; set; }
        public uint PrimaryCategoryId { get; set; }
        public Section CategorySection { get; set; }
        public string Slug { get; set; }
        public List<GameVersionLatestFile> GameVersionLatestFiles { get; set; }
        public bool IsFeatured { get; set; }
        public float PopularityRank { get; set; }
        public uint GamePopularityRank { get; set; }
        public string PrimaryLanguage { get; set; }
        public string GameSlug { get; set; }
        public string GameName { get; set; }
        public string PortalName { get; set; }
        public string DataModified { get; set; }
        public string DataCreated { get; set; }
        public string DataReleased { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsExperimental { get; set; }
    }
}
