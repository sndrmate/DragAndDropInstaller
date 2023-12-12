using System.Xml;

namespace DragAndDropInstaller;

internal class VersionChecker
{
    public VersionChecker() {}

    public static void CheckVersion(string localVersion, string url)
    {
        try
        {
            XmlDocument currentVersionXML = new XmlDocument();
            currentVersionXML.Load(url);
            string currentVersion = currentVersionXML.SelectSingleNode("/CurrentVersion")?.InnerText ?? string.Empty;
            if (localVersion != currentVersion)
            {
                UserInterface.UpdateReminder(currentVersion, localVersion);
            }
        }
        catch (Exception)
        {

            return;
        }
    }
}
