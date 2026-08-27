namespace RailStrap.Models.Persistable
{
    public class State
    {
        public bool PromptWebView2Install { get; set; } = true;

        public bool ForceReinstall { get; set; } = false;

        public string LastSeenVersion { get; set; } = "";

        public WindowState SettingsWindow { get; set; } = new();
    }
}
