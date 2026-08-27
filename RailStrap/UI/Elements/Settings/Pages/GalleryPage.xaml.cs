using RailStrap.UI.ViewModels.Settings;

namespace RailStrap.UI.Elements.Settings.Pages
{
    /// <summary>
    /// Interaction logic for GalleryPage.xaml
    /// </summary>
    public partial class GalleryPage
    {
        public GalleryPage()
        {
            DataContext = new GalleryViewModel();
            InitializeComponent();
        }
    }
}
