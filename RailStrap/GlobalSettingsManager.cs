using System.Xml.Linq;

namespace RailStrap
{
    // Manages Roblox's own %LocalAppData%\Roblox\GlobalBasicSettings_13.xml. Properties in this
    // file (e.g. FramerateCap) aren't subject to Roblox's Sept 2025 FastFlag allowlist, unlike
    // DFIntTaskSchedulerTargetFps, so this is now the reliable way to manage the frame rate cap.
    //
    // A real launched Roblox client writes many more properties into this file than the ones we
    // manage (mouse sensitivity, VR, accessibility, etc.), so this only ever edits the specific
    // <Properties> child nodes it owns via LINQ-to-XML - it never overwrites the whole document.
    public class GlobalSettingsManager
    {
        private const string TemplateXml =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<roblox xmlns:xmime=\"http://www.w3.org/2005/05/xmlmime\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"http://www.roblox.com/roblox.xsd\" version=\"4\">" +
            "<External>null</External>" +
            "<External>nil</External>" +
            "<Item class=\"UserGameSettings\" referent=\"RBX17633EFA9BBD402199582E10A8C7E8F2\">" +
            "<Properties>" +
            "</Properties>" +
            "</Item>" +
            "</roblox>";

        public string ClassName => nameof(GlobalSettingsManager);

        public string FileLocation => Path.Combine(Paths.Roblox, "GlobalBasicSettings_13.xml");

        private XDocument? _document;

        private XElement? PropertiesElement =>
            _document?
                .Descendants("Item")
                .FirstOrDefault(x => (string?)x.Attribute("class") == "UserGameSettings")?
                .Element("Properties");

        private void Load()
        {
            const string LOG_IDENT = "GlobalSettingsManager::Load";

            try
            {
                _document = File.Exists(FileLocation) ? XDocument.Load(FileLocation) : XDocument.Parse(TemplateXml);
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to load, falling back to template");
                App.Logger.WriteException(LOG_IDENT, ex);

                _document = XDocument.Parse(TemplateXml);
            }
        }

        private void SetValue(string elementType, string propertyName, string value)
        {
            var properties = PropertiesElement;

            if (properties is null)
                return;

            var existing = properties.Elements().FirstOrDefault(x => (string?)x.Attribute("name") == propertyName);

            if (existing is not null)
                existing.Value = value;
            else
                properties.Add(new XElement(elementType, new XAttribute("name", propertyName), value));
        }

        private void Save()
        {
            const string LOG_IDENT = "GlobalSettingsManager::Save";

            if (_document is null)
                return;

            Directory.CreateDirectory(Path.GetDirectoryName(FileLocation)!);

            try
            {
                // keep a copy of the first Roblox-authored file we ever touch, so there's always
                // a true baseline to restore if something goes wrong
                if (File.Exists(FileLocation) && !File.Exists(FileLocation + ".bak"))
                    File.Copy(FileLocation, FileLocation + ".bak");

                _document.Save(FileLocation);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to save");
                App.Logger.WriteException(LOG_IDENT, ex);

                throw;
            }
        }

        // frameRateCap <= 0 means "don't manage this" - leaves the file untouched entirely
        public void ApplyFrameRateCap(int frameRateCap)
        {
            if (frameRateCap <= 0)
                return;

            Load();
            SetValue("int", "FramerateCap", frameRateCap.ToString());
            Save();
        }
    }
}
