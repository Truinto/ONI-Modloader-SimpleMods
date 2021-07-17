//#define DEBUG

using System;
using System.IO;

namespace versioncontrol_ONI
{
    // reads changelog and updates AssemblyInfo and info.json with new version
    public class Program
    {
        public static int versionLength = 6;

        public static int Main(string[] args)
        {
#if DEBUG
            args = new string[]
            {
                "-md",
                @"C:\Users\Fumihiko\Documents\Visual Studio Projects\ONI_Mod\CustomizeBuildings\Changelog.md",
                "-info",
                @"C:\Users\Fumihiko\Documents\Klei\OxygenNotIncluded\mods\dev\CarePackageMod\mod_info.yaml",
                "-asbly",
                @"C:\Users\Fumihiko\Documents\Visual Studio Projects\ONI_Mod\CustomizeBuildings\Properties\AssemblyInfo.cs"
            };
#endif

            try
            {
                string path_log = null;
                string path_md = null;
                string path_info = null;
                string path_assembly = null;
                string path_state = null;
                string gameversion = null;
                string gameversionprefix = null;
                string assemblyversion = null;
                string projectname = null;
                int int_gameversion = 0;
                bool b_exp1 = false;
                bool overwrite_state = false;

                // read args
                #region args
                for (int i = 0; i + 1 < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-log":
                            path_log = args[i + 1];
                            break;
                        case "-md":
                            path_md = args[i + 1];
                            break;
                        case "-info":
                            path_info = args[i + 1];
                            break;
                        case "-asbly":
                            path_assembly = args[i + 1];
                            break;
                        case "-state":
                            path_state = args[i + 1];
                            break;
                        case "-projectname":
                            projectname = args[i + 1];
                            break;
                        case "-stateoverwrite":
                            overwrite_state = true;
                            break;
                    }
                }
                #endregion

                // read current version from log
                #region log
                path_log = path_log ?? (Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\..\LocalLow\Klei\Oxygen Not Included\Player.log");
                using (var log = File.OpenText(path_log))
                {
                    //Console.WriteLine("Reading log file...");
                    int counter = 0;
                    while (!log.EndOfStream && counter++ < 50)
                    {
                        var line = log.ReadLine();
                        if (line.Contains("[INFO] Expansion1: True", StringComparison.Ordinal))
                        {
                            b_exp1 = true;
                            break;
                        }

                        int index = line.IndexOf("[INFO] release Build: ", StringComparison.Ordinal);
                        if (index < 0) index = line.IndexOf("[INFO] preview Build: ", StringComparison.Ordinal);
                        if (index >= 0)
                        {
                            index += 22;
                            gameversion = line[index..];
                            gameversionprefix = gameversion[..gameversion.IndexOf('-')];
                            int.TryParse(HelperStrings.GetQuotationString(gameversion, 1, '-'), out int_gameversion);
                            Console.WriteLine($"gameversion is {gameversion}, parsed as build: {int_gameversion}, prefix: {gameversionprefix}");
                        }
                    }
                }
                #endregion

                // read assemblyversion and update gameversion to changelog
                #region md
                if (path_md != null)
                {
                    //Console.WriteLine("Reading changelog...");
                    var changelog = File.ReadAllLines(path_md);
                    for (int i = 0; i < changelog.Length; i++)
                    {
                        if (changelog[i].Contains("[", StringComparison.Ordinal))
                        {
                            changelog[i] = changelog[i].TrimEnd();
                            int indexopen = changelog[i].IndexOf('[') + 1;
                            int indexclose = changelog[i].IndexOf(']');
                            if (indexclose > indexopen && indexopen > 0)
                            {
                                assemblyversion = changelog[i][indexopen..indexclose];
                                Console.WriteLine("read assembly version as: " + assemblyversion);
                                if (gameversion != null)
                                {
                                    if (changelog[i].IndexOf(gameversion, StringComparison.Ordinal) < 0)
                                        changelog[i] += (" " + gameversion);
                                    //int indexversion = changelog[i].IndexOf(gameversionprefix, StringComparison.Ordinal);
                                    //if (indexversion < 0)
                                    //    changelog[i] += (" " + gameversion);
                                    //else
                                    //    changelog[i] = changelog[i][..indexversion] + gameversion + changelog[i][(indexversion + gameversion.Length)..];
                                }
                            }
                            break;
                        }
                    }
                    File.WriteAllLines(path_md, changelog);
                }
                #endregion

                // update version to mod_info.yaml
                #region info
                if (path_info != null && int_gameversion != 0)
                {
                    string[] modinfo;
                    if (File.Exists(path_info))
                    {
                        modinfo = File.ReadAllLines(path_info);
                        for (int i = 0; i < modinfo.Length; i++)
                            if (modinfo[i].StartsWith("minimumSupportedBuild:", StringComparison.Ordinal))
                                modinfo[i] = "minimumSupportedBuild: " + int_gameversion;
                            else if (modinfo[i].StartsWith("version: ", StringComparison.Ordinal))
                                modinfo[i] = "version: " + assemblyversion;
                    }
                    else
                    {
                        modinfo = new string[]
                        {
                            "supportedContent: " + (b_exp1 ? "EXPANSION1_ID" : "VANILLA_ID"),
                            "minimumSupportedBuild: " + int_gameversion,
                            "APIVersion: 2",
                            //projectname == null ? "" : "staticID: " + projectname,
                            assemblyversion == null ? "" : "version: " + assemblyversion
                        };
                    }
                    File.WriteAllLines(path_info, modinfo);
                }
                #endregion

                // update assemblyversion to assembly
                #region assembly
                if (path_assembly != null && assemblyversion != null)
                {
                    //Console.WriteLine("Reading assembly...");
                    var assembly = File.ReadAllLines(path_assembly);
                    for (int i = 0; i < assembly.Length; i++)
                    {
                        if (assembly[i].StartsWith("[assembly: AssemblyVersion(\"", StringComparison.Ordinal))
                            assembly[i] = "[assembly: AssemblyVersion(\"" + assemblyversion + "\")]";
                        else if (assembly[i].StartsWith("[assembly: AssemblyFileVersion(\"", StringComparison.Ordinal))
                            assembly[i] = "[assembly: AssemblyFileVersion(\"" + assemblyversion + "\")]";
                    }
                    File.WriteAllLines(path_assembly, assembly);
                }
                #endregion

                // auto complete state source
                #region language
                LanguageFillOut.Run(path_state, overwrite_state);
                #endregion

                Console.WriteLine("versioncontrol done!");
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 1;
            }
        }
    }
}

