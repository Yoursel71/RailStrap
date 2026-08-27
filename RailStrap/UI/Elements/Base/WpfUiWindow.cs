using System.Windows;
using System.Windows.Interop;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace RailStrap.UI.Elements.Base
{
    public abstract class WpfUiWindow : UiWindow
    {
        private readonly IThemeService _themeService = new ThemeService();

        public WpfUiWindow()
        {
            // FontFamily is an inherited property, so setting it here cascades to every
            // descendant control in the window instead of needing a per-control style
            SetResourceReference(FontFamilyProperty, "Rubik");

            ApplyTheme();
        }

        public void ApplyTheme()
        {
            const int customThemeIndex = 2; // index for CustomTheme merged dictionary

            var accentStyle = App.Settings.Prop.AccentStyle;
            bool customAccentStyle = AccentStyleManager.UsesDarkBase(accentStyle);

            // the custom accent styles are dark-only visual identities, so they always force a dark base
            var themeType = customAccentStyle
                ? ThemeType.Dark
                : (App.Settings.Prop.Theme.GetFinal() == Enums.Theme.Dark ? ThemeType.Dark : ThemeType.Light);

            _themeService.SetTheme(themeType);

            if (customAccentStyle)
                AccentStyleManager.Apply(accentStyle);
            else
                _themeService.SetSystemAccent();

            if (accentStyle == Enums.AccentStyle.AuroraGlass)
                Wpf.Ui.Appearance.Background.Apply(this, BackgroundType.Acrylic);
            else
                Wpf.Ui.Appearance.Background.Remove(this);

            // there doesn't seem to be a way to query the name for merged dictionaries
            string styleFile = customAccentStyle ? "Dark" : Enum.GetName(App.Settings.Prop.Theme.GetFinal())!;
            var dict = new ResourceDictionary { Source = new Uri($"pack://application:,,,/UI/Style/{styleFile}.xaml") };
            Application.Current.Resources.MergedDictionaries[customThemeIndex] = dict;

#if QA_BUILD
            this.BorderBrush = System.Windows.Media.Brushes.Red;
            this.BorderThickness = new Thickness(4);
#endif
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            if (App.Settings.Prop.WPFSoftwareRender || App.LaunchSettings.NoGPUFlag.Active)
            {
                if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
                    hwndSource.CompositionTarget.RenderMode = RenderMode.SoftwareOnly;
            }

            base.OnSourceInitialized(e);
        }
    }
}
