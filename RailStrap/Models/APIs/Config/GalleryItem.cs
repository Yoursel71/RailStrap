namespace RailStrap.Models.APIs.Config
{
    public enum GalleryItemKind
    {
        Theme,
        Mod
    }

    public class GalleryItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("author")]
        public string Author { get; set; } = "";

        [JsonPropertyName("downloadUrl")]
        public string DownloadUrl { get; set; } = "";

        [JsonIgnore]
        public GalleryItemKind Kind { get; set; }

        [JsonIgnore]
        public bool IsInstalled { get; set; }
    }

    public class GalleryManifest
    {
        [JsonPropertyName("themes")]
        public List<GalleryItem> Themes { get; set; } = new();

        [JsonPropertyName("mods")]
        public List<GalleryItem> Mods { get; set; } = new();
    }
}
