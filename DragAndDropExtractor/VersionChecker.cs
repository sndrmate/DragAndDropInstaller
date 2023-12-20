﻿using System.Xml;

namespace DragAndDropInstaller;

internal class VersionChecker
{
    private VersionChecker() { }

    public static async Task CheckVersionAsync(string localVersion)
    {
        try
        {
            string xmlUrl = "https://sndrmate.github.io/docs/ddi_version.xml";

            string currentVersion = await Task.Run(() =>
            {
                using (XmlReader reader = XmlReader.Create(xmlUrl))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "CurrentVersion")
                        {
                            reader.Read();
                            return reader.Value;
                        }
                    }
                    return string.Empty;
                }
            });

            if (!string.Equals(localVersion, currentVersion, StringComparison.OrdinalIgnoreCase))
            {
                await UserInterface.UpdateReminderAsync(currentVersion, localVersion);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // Handle exceptions as needed
            return;
        }
    }

}
