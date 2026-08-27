using RailStrap.UI.ViewModels.Settings;

namespace RailStrap.UI.Elements.Settings.Pages
{
    /// <summary>
    /// Interaction logic for PluginsPage.xaml
    /// </summary>
    public partial class PluginsPage
    {
        public PluginsPage()
        {
            DataContext = new PluginsViewModel();
            InitializeComponent();
        }
    }
}
