using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RailStrap.UI.ViewModels.Settings;

namespace RailStrap.UI.Elements.Settings.Pages
{
    /// <summary>
    /// Interaction logic for RailStrapPage.xaml
    /// </summary>
    public partial class RailStrapPage
    {
        public RailStrapPage()
        {
            DataContext = new RailStrapViewModel();
            InitializeComponent();
        }
    }
}
