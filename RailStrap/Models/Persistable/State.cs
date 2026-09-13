namespace RailStrap.Models.Persistable
{
    public class State
    {
        public bool PromptWebView2Install { get; set; } = true;

        public bool ForceReinstall { get; set; } = false;

        public string LastSeenVersion { get; set; } = "";

        /// <summary>
        /// Set once the connection-helper offer has been put to the user, so it is never asked twice
        /// regardless of what they answered.
        /// </summary>
        public bool DpiBypassPromptShown { get; set; } = false;

        public WindowState SettingsWindow { get; set; } = new();
    }
}
