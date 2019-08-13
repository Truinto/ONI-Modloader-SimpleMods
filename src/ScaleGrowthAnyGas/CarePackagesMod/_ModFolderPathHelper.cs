using ONI_Common.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ModHelper
{
    class ModFolderPathHelper
    {
        public string path;
        public static string sep = Path.DirectorySeparatorChar.ToString();

        public ModFolderPathHelper(string modName, long id)
        {
            //path = "%UserProfile%\\Documents\\Klei\\OxygenNotIncluded\\mods\\Steam\\" + id.ToString();
            path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + sep + "Klei" + sep + "OxygenNotIncluded" + sep + "mods" + sep + "Steam" + sep + id.ToString();

            if (Directory.Exists(path))
                path += sep + modName + "State.json";
            else
                path = ONI_Common.Paths.GetStateFilePath(modName);

            Debug.Log("Settings file located at: " + path);
        }
    }
}
