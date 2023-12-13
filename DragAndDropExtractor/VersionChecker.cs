using System.Xml;

namespace DragAndDropInstaller;

internal class VersionChecker
{
    public VersionChecker() {}

    public static void CheckVersion(string localVersion, string url)
    {
        try
        {
            using (XmlReader reader = XmlReader.Create(url))
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

                if (!string.Equals(localVersion, currentVersion, System.StringComparison.OrdinalIgnoreCase))
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
