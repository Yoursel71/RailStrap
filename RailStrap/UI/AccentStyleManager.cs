using System.Windows;
using System.Windows.Media;

using Wpf.Ui.Appearance;

namespace RailStrap.UI
{
    // Layers a custom accent + surface palette + corner radius on top of the base Dark theme,
    // by overwriting the same DynamicResource brush keys Wpf.Ui.Appearance.Accent.Apply() uses
    // for the system accent, so every control that already binds to them picks it up live.
    public static class AccentStyleManager
    {
        public static bool UsesDarkBase(AccentStyle style) => style != AccentStyle.System;

        public static void Apply(AccentStyle style)
        {
            switch (style)
            {
                case AccentStyle.RailMono:
                    ApplyAccent(
                        system: Color.FromRgb(0x8A, 0x5B, 0xFF),
                        primary: Color.FromRgb(0x7C, 0x6C, 0xFF),
                        secondary: Color.FromRgb(0x6D, 0x5E, 0xF8),
                        tertiary: Color.FromRgb(0x4F, 0x8C, 0xFF));
                    ApplySurface(
                        background: Color.FromRgb(0x0B, 0x0B, 0x0D),
                        elevated: Color.FromRgb(0x16, 0x16, 0x1A),
                        border: Color.FromRgb(0x26, 0x26, 0x2D));
                    ApplyCornerRadius(new CornerRadius(8));
                    ApplyProportions(
                        baseFontSize: 14d,
                        navIconSize: 18d,
                        navItemMargin: new Thickness(0, 0, 0, 4),
                        navItemContentMargin: new Thickness(16, 8, 16, 8),
                        navSidebarWidth: 248d,
                        sidebarPanelPadding: new Thickness(6, 14, 6, 14));
                    break;

                case AccentStyle.AuroraGlass:
                    ApplyAccent(
                        system: Color.FromRgb(0x22, 0xD3, 0xEE),
                        primary: Color.FromRgb(0x38, 0xC6, 0xE6),
                        secondary: Color.FromRgb(0x4D, 0xB8, 0xE0),
                        tertiary: Color.FromRgb(0xA7, 0x8B, 0xFA));
                    ApplySurface(
                        background: Color.FromArgb(0xE6, 0x14, 0x11, 0x24),
                        elevated: Color.FromArgb(0x40, 0xFF, 0xFF, 0xFF),
                        border: Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF));
                    ApplyCornerRadius(new CornerRadius(12));
                    ApplyProportions(
                        baseFontSize: 15d,
                        navIconSize: 21d,
                        navItemMargin: new Thickness(0, 0, 0, 9),
                        navItemContentMargin: new Thickness(20, 13, 20, 13),
                        navSidebarWidth: 268d,
                        sidebarPanelPadding: new Thickness(12, 20, 12, 20));
                    break;

                case AccentStyle.RailTerminal:
                    ApplyAccent(
                        system: Color.FromRgb(0xF0, 0xA7, 0x42),
                        primary: Color.FromRgb(0xE2, 0x99, 0x3D),
                        secondary: Color.FromRgb(0xD0, 0x8B, 0x34),
                        tertiary: Color.FromRgb(0xB9, 0x7A, 0x2A));
                    ApplySurface(
                        background: Color.FromRgb(0x0C, 0x0E, 0x11),
                        elevated: Color.FromRgb(0x15, 0x19, 0x1F),
                        border: Color.FromRgb(0x26, 0x2B, 0x33));
                    ApplyCornerRadius(new CornerRadius(2));
                    ApplyProportions(
                        baseFontSize: 13d,
                        navIconSize: 16d,
                        navItemMargin: new Thickness(0, 0, 0, 1),
                        navItemContentMargin: new Thickness(12, 6, 12, 6),
                        navSidebarWidth: 226d,
                        sidebarPanelPadding: new Thickness(4, 10, 4, 10));
                    break;

                default:
                    return;
            }
        }

        private static void ApplyAccent(Color system, Color primary, Color secondary, Color tertiary) =>
            Accent.Apply(system, primary, secondary, tertiary);

        private static void ApplySurface(Color background, Color elevated, Color border)
        {
            var backgroundBrush = new SolidColorBrush(background);
            var elevatedBrush = new SolidColorBrush(elevated);
            var borderBrush = new SolidColorBrush(border);

            Application.Current.Resources["ApplicationBackgroundColor"] = background;
            Application.Current.Resources["ApplicationBackgroundBrush"] = backgroundBrush;
            Application.Current.Resources["SolidBackgroundFillColorBaseBrush"] = backgroundBrush;
            Application.Current.Resources["SolidBackgroundFillColorSecondaryBrush"] = elevatedBrush;
            Application.Current.Resources["SolidBackgroundFillColorTertiaryBrush"] = elevatedBrush;
            Application.Current.Resources["CardBackgroundFillColorDefaultBrush"] = elevatedBrush;
            Application.Current.Resources["ControlFillColorDefaultBrush"] = elevatedBrush;
            Application.Current.Resources["ControlStrokeColorDefaultBrush"] = borderBrush;
            Application.Current.Resources["CardStrokeColorDefaultSolidBrush"] = borderBrush;
            Application.Current.Resources["DividerStrokeColorDefaultBrush"] = borderBrush;
        }

        private static void ApplyCornerRadius(CornerRadius radius)
        {
            Application.Current.Resources["ControlCornerRadius"] = radius;
            Application.Current.Resources["OverlayCornerRadius"] = radius;
            Application.Current.Resources["PopupCornerRadius"] = radius;
        }

        // These drive real layout differences (not just color) between styles: base text size,
        // sidebar item density/spacing, icon size and sidebar width/padding.
        private static void ApplyProportions(
            double baseFontSize,
            double navIconSize,
            Thickness navItemMargin,
            Thickness navItemContentMargin,
            double navSidebarWidth,
            Thickness sidebarPanelPadding)
        {
            Application.Current.Resources["ContentControlFontSize"] = baseFontSize;
            Application.Current.Resources["ControlContentThemeFontSize"] = baseFontSize;
            Application.Current.Resources["NavIconSize"] = navIconSize;
            Application.Current.Resources["NavItemMargin"] = navItemMargin;
            Application.Current.Resources["NavItemContentMargin"] = navItemContentMargin;
            Application.Current.Resources["NavSidebarWidth"] = navSidebarWidth;
            Application.Current.Resources["SidebarPanelPadding"] = sidebarPanelPadding;
        }
    }
}
