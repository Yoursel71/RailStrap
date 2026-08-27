using RailStrap.UI.ViewModels.Settings;

namespace RailStrap.UI.Elements.Settings.Pages
{
    /// <summary>
    /// Interaction logic for StatsPage.xaml
    /// </summary>
    public partial class StatsPage
    {
        public StatsPage()
        {
            DataContext = new StatsViewModel();
            InitializeComponent();
        }
    }
}
