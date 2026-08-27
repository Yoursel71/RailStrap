namespace RailStrap.UI.Elements.Dialogs
{
    /// <summary>
    /// Interaction logic for ChangelogDialog.xaml
    /// </summary>
    public partial class ChangelogDialog
    {
        public ChangelogDialog(string version)
        {
            InitializeComponent();

            TitleText.Text = string.Format(Strings.Dialog_Changelog_Title, version);
            BodyMDTextBlock.MarkdownText = Strings.Dialog_Changelog_Loading;

            CloseButton.Click += delegate { Close(); };

            _ = LoadReleaseNotes(version);
        }

        private async Task LoadReleaseNotes(string version)
        {
            var release = await App.GetReleaseByTag($"v{version}");

            string releaseUrl = $"https://github.com/{App.ProjectRepository}/releases/tag/v{version}";

            BodyMDTextBlock.MarkdownText = !string.IsNullOrWhiteSpace(release?.Body)
                ? release.Body
                : string.Format(Strings.Dialog_Changelog_Unavailable, releaseUrl);
        }
    }
}
