using System.Xml;

namespace DragAndDropInstaller;

internal class VersionChecker
{
    public VersionChecker() {}

    public static void CheckVersion(string localVersion)
    {
        try
        {
            using (XmlReader reader = XmlReader.Create("https://sndrmate.github.io/docs/ddi_version.xml"))
            {
                string currentVersion = string.Empty;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "CurrentVersion")
                    {
                        reader.Read();
                        currentVersion = reader.Value;
                        break;
                    }
                }

                if (!string.Equals(localVersion, currentVersion, StringComparison.OrdinalIgnoreCase))
                {
                    UserInterface.UpdateReminder(currentVersion, localVersion);
                }
            }
        }
        catch (Exception)
        {
            return;
        }
    }
}
