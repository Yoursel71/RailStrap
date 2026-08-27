using RailStrap.UI.ViewModels.Dialogs;

namespace RailStrap.UI.Elements.Dialogs
{
    /// <summary>
    /// Interaction logic for FriendActivityWindow.xaml
    /// </summary>
    public partial class FriendActivityWindow
    {
        public FriendActivityWindow()
        {
            DataContext = new FriendActivityViewModel();
            InitializeComponent();
        }
    }
}
