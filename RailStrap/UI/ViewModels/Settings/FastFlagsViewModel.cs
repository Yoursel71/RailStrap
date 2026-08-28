using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using RailStrap.Enums.FlagPresets;

namespace RailStrap.UI.ViewModels.Settings
{
    public class FastFlagsViewModel : NotifyPropertyChangedViewModel
    {
        private Dictionary<string, object>? _preResetFlags;

        public event EventHandler? RequestPageReloadEvent;
        
        public event EventHandler? OpenFlagEditorEvent;

        private void OpenFastFlagEditor() => OpenFlagEditorEvent?.Invoke(this, EventArgs.Empty);

        public ICommand OpenFastFlagEditorCommand => new RelayCommand(OpenFastFlagEditor);

        public Visibility CanShowFastFlagEditor => App.IsStudioInstalled ? Visibility.Visible : Visibility.Collapsed;

        public bool UseFastFlagManager
        {
            get => App.Settings.Prop.UseFastFlagManager;
            set => App.Settings.Prop.UseFastFlagManager = value;
        }

        public IReadOnlyDictionary<MSAAMode, string?> MSAALevels => FastFlagManager.MSAAModes;

        public MSAAMode SelectedMSAALevel
        {
            get => MSAALevels.FirstOrDefault(x => x.Value == App.FastFlags.GetPreset("Rendering.MSAA")).Key;
            set => App.FastFlags.SetPreset("Rendering.MSAA", MSAALevels[value]);
        }

        public bool FixDisplayScaling
        {
            get => App.FastFlags.GetPreset("Rendering.DisableScaling") == "True";
            set => App.FastFlags.SetPreset("Rendering.DisableScaling", value ? "True" : null);
        }

        public IReadOnlyDictionary<TextureQuality, string?> TextureQualities => FastFlagManager.TextureQualityLevels;

        public TextureQuality SelectedTextureQuality
        {
            get => TextureQualities.Where(x => x.Value == App.FastFlags.GetPreset("Rendering.TextureQuality.Level")).FirstOrDefault().Key;
            set
            {
                if (value == TextureQuality.Default)
                {
                    App.FastFlags.SetPreset("Rendering.TextureQuality", null);
                }
                else
                {
                    App.FastFlags.SetPreset("Rendering.TextureQuality.OverrideEnabled", "True");
                    App.FastFlags.SetPreset("Rendering.TextureQuality.Level", TextureQualities[value]);
                }
            }
        }
        public int MaxFPS
        {
            get => int.TryParse(App.FastFlags.GetPreset("Performance.MaxFPS"), out int value) ? value : 0;
            set => App.FastFlags.SetPreset("Performance.MaxFPS", value > 0 ? value.ToString() : null);
        }

        // Roblox's Sept 2025 FastFlag allowlist broke DFIntTaskSchedulerTargetFps (MaxFPS above) on
        // most clients, so this writes the frame cap into GlobalBasicSettings_13.xml instead, which
        // isn't subject to that allowlist.
        public int GlobalFrameRateCap
        {
            get => App.Settings.Prop.GlobalFrameRateCap;
            set => App.Settings.Prop.GlobalFrameRateCap = value;
        }

        public bool EnablePingOverlay
        {
            get => App.Settings.Prop.EnablePingOverlay;
            set => App.Settings.Prop.EnablePingOverlay = value;
        }

        public bool ResetConfiguration
        {
            get => _preResetFlags is not null;

            set
            {
                if (value)
                {
                    _preResetFlags = new(App.FastFlags.Prop);
                    App.FastFlags.Prop.Clear();
                }
                else
                {
                    App.FastFlags.Prop = _preResetFlags!;
                    _preResetFlags = null;
                }

                RequestPageReloadEvent?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
