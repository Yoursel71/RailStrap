using System.Xml;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;

using RailStrap.UI.Elements.Base;

namespace RailStrap.UI.Elements.Dialogs
{
    /// <summary>
    /// Interaction logic for LogViewerWindow.xaml
    /// </summary>
    public partial class LogViewerWindow : WpfUiWindow
    {
        public LogViewerWindow(string logFilePath)
        {
            InitializeComponent();

            LoadHighlightingTheme();
            SearchPanel.Install(UIXML);

            UIXML.Text = File.Exists(logFilePath)
                ? File.ReadAllText(logFilePath)
                : Strings.LogViewer_NotFound;

            UIXML.ScrollToEnd();
        }

        private void LoadHighlightingTheme()
        {
            string name = $"Editor-Theme-{App.Settings.Prop.Theme.GetFinal()}.xshd";
            using Stream xmlStream = Resource.GetStream(name);
            using XmlReader reader = XmlReader.Create(xmlStream);
            UIXML.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

            UIXML.TextArea.TextView.SetResourceReference(ICSharpCode.AvalonEdit.Rendering.TextView.LinkTextForegroundBrushProperty, "NewTextEditorLink");
        }
    }
}
