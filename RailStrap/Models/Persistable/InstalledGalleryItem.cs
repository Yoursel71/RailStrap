namespace RailStrap.Models.Persistable
{
    public class InstalledGalleryItem
    {
        public string Name { get; set; } = "";

        public GalleryItemKind Kind { get; set; }

        public List<string> Files { get; set; } = new();
    }

    public class GalleryState
    {
        public List<InstalledGalleryItem> Installed { get; set; } = new();
    }
}
