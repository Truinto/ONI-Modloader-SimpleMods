using System;
using System.IO;
using System.Text.RegularExpressions;

namespace versioncontrol_ONI
{
    // reads changelog and updates AssemblyInfo and info.json with new version
    public class Program2
    {
        public static int versionLength = 6;

        public class Data
        {
            public string Path_Log;
            public string Path_MD;
            public string Path_Info;
            public string Path_Assembly;
            public string Path_State;
            public string GameVersion;
            public string GameVersionPrefix;
            public string AssemblyVersion;
            public string ProjectName;
            public int Int_GameVersion;
            public bool B_exp1;
            public bool Overwrite_state;
            public bool NewVersion;
        }

        public static int Main2(string[] args)
        {
            try
            {
                var data = new Data();

                // read args
                UpdateArgs(args, data);

                // read current version from log
                UpdateLog(data);

                // read assemblyversion and update gameversion to changelog
                UpdateChangelog(data);

                // update version to mod_info.yaml
                UpdateInfo(data);

                // update assemblyversion to assembly
                UpdateAssembly(data);

                // auto complete state source
                LanguageFillOut.Run(data.Path_State, data.Overwrite_state);

                Console.WriteLine("versioncontrol done!");
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 1;
            }
        }

        private static void UpdateArgs(string[] args, Data data)
        {
            for (int i = 0; i + 1 < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-log":
                        data.Path_Log = args[i + 1];
                        break;
                    case "-md":
                        data.Path_MD = args[i + 1];
                        break;
                    case "-info":
                        data.Path_Info = args[i + 1];
                        break;
                    case "-asbly":
                        data.Path_Assembly = args[i + 1];
                        break;
                    case "-state":
                        data.Path_State = args[i + 1];
                        break;
                    case "-projectname":
                        data.ProjectName = args[i + 1];
                        break;
                    case "-stateoverwrite":
                        data.Overwrite_state = true;
                        break;
                }
            }
        }

        private static void UpdateLog(Data data)
        {
            data.Path_Log ??= (Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\..\LocalLow\Klei\Oxygen Not Included\Player.log");
            if (!File.Exists(data.Path_Log))            
                return;
            
            //Console.WriteLine("Reading log file...");
            using var log = File.OpenText(data.Path_Log);
            int counter = 0;
            while (!log.EndOfStream && counter++ < 50)
            {
                var line = log.ReadLine();
                if (line.Contains("[INFO] Expansion1: True", StringComparison.Ordinal))
                {
                    data.B_exp1 = true;
                    break;
                }

                int index = line.IndexOf("[INFO] release Build: ", StringComparison.Ordinal);
                if (index < 0) index = line.IndexOf("[INFO] preview Build: ", StringComparison.Ordinal);
                if (index >= 0)
                {
                    index += 22;
                    data.GameVersion = line[index..];
                    data.GameVersion = data.GameVersion.Trim();
                    data.GameVersionPrefix = data.GameVersion[..data.GameVersion.IndexOf('-')];
                    _ = int.TryParse(HelperStrings.GetQuotationString(data.GameVersion, 1, '-'), out data.Int_GameVersion);
                    Console.WriteLine($"gameversion is {data.GameVersion}, parsed as build: {data.Int_GameVersion}, prefix: {data.GameVersionPrefix}");
                }
            }
        }

        private static void UpdateChangelog(Data data)
        {
            if (data.Path_MD != null)
            {
                //Console.WriteLine("Reading changelog...");
                var changelog = File.ReadAllLines(data.Path_MD);
                for (int i = 0; i < changelog.Length; i++)
                {
                    if (changelog[i].Contains('[', StringComparison.Ordinal))
                    {
                        changelog[i] = changelog[i].TrimEnd();
                        int indexopen = changelog[i].IndexOf('[') + 1;
                        int indexclose = changelog[i].IndexOf(']');
                        if (indexclose > indexopen && indexopen > 0)
                        {
                            data.AssemblyVersion = changelog[i][indexopen..indexclose];
                            Console.WriteLine("read assembly version as: " + data.AssemblyVersion);
                            if (data.GameVersion != null)
                            {
                                if (changelog[i].IndexOf(data.GameVersion, StringComparison.Ordinal) < 0)
                                {
                                    changelog[i] += (" " + data.GameVersion);
                                    data.NewVersion = true;
                                }
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
                File.WriteAllLines(data.Path_MD, changelog);
            }
        }

        private static void UpdateInfo(Data data)
        {
            if (data.Path_Info != null && data.Int_GameVersion != 0)
            {
                string[] modinfo;
                if (File.Exists(data.Path_Info))
                {
                    if (data.ProjectName != null /* && newVersion*/) // archive old version, if gameversion was printed into changelog
                    {
                        string path_mod = new FileInfo(data.Path_Info).Directory.FullName;
                        string path_dll = Path.Combine(path_mod, data.ProjectName + ".dll");
                        if (File.Exists(path_dll))
                        {
                            string oldgameversion = Regex.Match(File.ReadAllText(Path.Combine(path_mod, "mod_info.yaml")), "minimumSupportedBuild: (\\d+)").Groups[1].Value;
                            oldgameversion = oldgameversion.Trim();

                            if (data.GameVersion == oldgameversion)
                            {
                                string path_archive = Path.Combine(path_mod, "archived_versions", oldgameversion);
                                string path_archdll = Path.Combine(path_archive, data.ProjectName + ".dll");
                                //if (!File.Exists(path_archdll))
                                {
                                    Directory.CreateDirectory(path_archive);
                                    File.Copy(path_dll, path_archdll, true);
                                    File.Copy(data.Path_Info, Path.Combine(path_archive, "mod_info.yaml"), true);
                                    Console.WriteLine("archived old version");
                                }
                            }
                            else
                                Console.WriteLine("didn't archive");
                        }
                        else
                            Console.WriteLine("archiving failed");
                    }

                    modinfo = File.ReadAllLines(data.Path_Info);
                    for (int i = 0; i < modinfo.Length; i++)
                        if (modinfo[i].StartsWith("minimumSupportedBuild:", StringComparison.Ordinal))
                            modinfo[i] = "minimumSupportedBuild: " + data.Int_GameVersion;
                        else if (modinfo[i].StartsWith("version: ", StringComparison.Ordinal))
                            modinfo[i] = "version: " + data.AssemblyVersion;
                }
                else
                {
                    modinfo = new string[]
                    {
                            "supportedContent: " + (data.B_exp1 ? "EXPANSION1_ID" : "VANILLA_ID"),
                            "minimumSupportedBuild: " + data.Int_GameVersion,
                            "APIVersion: 2",
                            //projectname == null ? "" : "staticID: " + projectname,
                            data.AssemblyVersion == null ? "" : "version: " + data.AssemblyVersion
                    };
                }
                File.WriteAllLines(data.Path_Info, modinfo);
            }
        }

        private static void UpdateAssembly(Data data)
        {
            if (data.Path_Assembly != null && data.AssemblyVersion != null)
            {
                //Console.WriteLine("Reading assembly...");
                var assembly = File.ReadAllLines(data.Path_Assembly);
                for (int i = 0; i < assembly.Length; i++)
                {
                    if (assembly[i].StartsWith("[assembly: AssemblyVersion(\"", StringComparison.Ordinal))
                        assembly[i] = "[assembly: AssemblyVersion(\"" + data.AssemblyVersion + "\")]";
                    else if (assembly[i].StartsWith("[assembly: AssemblyFileVersion(\"", StringComparison.Ordinal))
                        assembly[i] = "[assembly: AssemblyFileVersion(\"" + data.AssemblyVersion + "\")]";
                }
                File.WriteAllLines(data.Path_Assembly, assembly);
            }
        }
    }
}

