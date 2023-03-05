using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace versioncontrol_ONI
{
    // reads changelog and updates AssemblyInfo and info.json with new version
    public class Program
    {

        public class Data
        {
            public string Path_Log;
            public string Path_MD;
            public string Path_Info;
            public string Path_Assembly;
            public string Path_State;
            public string GameVersion;
            public string GameVersion_Prefix;
            public string AssemblyVersion;
            public string ProjectName;
            public int GameVersion_Int;
            public bool B_exp1;
            public bool Overwrite_state;
            public bool NewVersion;
        }

        public static int Main(string[] args)
        {
            try
            {
                var data = new Data();

                // read args
                ReadArgs(args, data);

                // read current version from log
                ReadLog(data);

                // read assemblyversion and update gameversion to changelog
                UpdateChangelog(data);

                UpdateRevisionVersion(data);

                CreateBackup(data);

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

        /// <summary>
        /// Parse arguments into data class.
        /// </summary>
        private static void ReadArgs(string[] args, Data data)
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

        /// <summary>
        /// Parse game version and extensions from log file.
        /// </summary>
        private static void ReadLog(Data data)
        {
            data.Path_Log ??= (Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\..\LocalLow\Klei\Oxygen Not Included\Player.log");
            if (!File.Exists(data.Path_Log))
                return;

            var rx_build = new Regex("Build: (([0-9A-z]+)-(.+)-(.+))$", RegexOptions.Compiled);

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

                var match = rx_build.Match(line);
                if (match.Success)
                {
                    data.GameVersion = match.Groups[1].Value;
                    data.GameVersion_Prefix = match.Groups[2].Value;
                    _ = int.TryParse(match.Groups[3].Value, out data.GameVersion_Int);
                    Console.WriteLine($"gameversion is {data.GameVersion}, parsed as build: {data.GameVersion_Int}, prefix: {data.GameVersion_Prefix}");
                }
            }
        }

        /// <summary>
        /// Parse assembly version.<br/>
        /// Print game version, if newer.
        /// </summary>
        private static void UpdateChangelog(Data data)
        {
            if (data.Path_MD == null)            
                return;

            var rx = new Regex(@"\[([\d\.]+)\]");
            
            var changelog = File.ReadAllLines(data.Path_MD);
            for (int i = 0; i < changelog.Length; i++)
            {
                var match = rx.Match(changelog[i]);
                if (!match.Success)
                    continue;

                data.AssemblyVersion = match.Groups[1].Value;
                Console.WriteLine("read assembly version as: " + data.AssemblyVersion);
                if (data.GameVersion != null && !changelog[i].Contains(data.GameVersion, StringComparison.Ordinal))
                {
                    changelog[i] += " " + data.GameVersion;
                    data.NewVersion = true;
                }
                break;
            }
            File.WriteAllLines(data.Path_MD, changelog);
        }

        /// <summary>
        /// Bump assembly version, if not newer.
        /// </summary>
        private static void UpdateRevisionVersion(Data data)
        {
            if (data.Path_Info == null)
                return;

            var match = Regex.Match(File.ReadAllText(data.Path_Info), "^version: ([\\d\\.]+)", RegexOptions.Multiline);
            if (!match.Success)
                return;

            var v_changelog = new Version(data.AssemblyVersion);
            var v_info = new Version(match.Groups[1].Value);
            if (v_changelog > v_info)
                return;

            data.AssemblyVersion = $"{v_info.Major}.{v_info.Minor}.{v_info.Build}.{v_info.Revision + 1}";
        }

        /// <summary>
        /// Copy dll and info file into archived_versions folder, if game version is newer.
        /// </summary>
        private static void CreateBackup(Data data)
        {
            if (data.Path_Info == null)
                return;

            string path_mod = new FileInfo(data.Path_Info).Directory.FullName;
            var match = Regex.Match(File.ReadAllText(data.Path_Info), "^minimumSupportedBuild: (\\d+)", RegexOptions.Multiline);
            if (!match.Success)
                return;

            string oldgameversion = match.Groups[1].Value;
            if (data.GameVersion_Int.ToString() == oldgameversion)
                return;

            string path_archive = Path.Combine(path_mod, "archived_versions", oldgameversion);

            Directory.CreateDirectory(path_archive);
            foreach (var dll in Directory.GetFiles(path_mod, "*.dll"))
                File.Copy(dll, Path.Combine(path_archive, new FileInfo(dll).Name), true);
            File.Copy(Path.Combine(path_mod, "mod_info.yaml"), Path.Combine(path_archive, "mod_info.yaml"), true);
            Console.WriteLine("archived old version");
        }

        /// <summary>
        /// Print assembly version and game version into info file.
        /// </summary>
        private static void UpdateInfo(Data data)
        {
            if (data.Path_Info != null && data.GameVersion_Int != 0)
            {
                string[] modinfo;
                if (File.Exists(data.Path_Info))
                {
                    modinfo = File.ReadAllLines(data.Path_Info);
                    for (int i = 0; i < modinfo.Length; i++)
                        if (modinfo[i].StartsWith("minimumSupportedBuild:", StringComparison.Ordinal))
                            modinfo[i] = "minimumSupportedBuild: " + data.GameVersion_Int;
                        else if (modinfo[i].StartsWith("version: ", StringComparison.Ordinal))
                            modinfo[i] = "version: " + data.AssemblyVersion;
                }
                else
                {
                    modinfo = new string[]
                    {
                            "supportedContent: " + (data.B_exp1 ? "EXPANSION1_ID" : "VANILLA_ID"),
                            "minimumSupportedBuild: " + data.GameVersion_Int,
                            "APIVersion: 2",
                            //projectname == null ? "" : "staticID: " + projectname,
                            data.AssemblyVersion == null ? "" : "version: " + data.AssemblyVersion
                    };
                }
                File.WriteAllLines(data.Path_Info, modinfo);
                Console.WriteLine("updated info file");
            }
        }

        /// <summary>
        /// Print assembly version into assembly file.
        /// </summary>
        private static void UpdateAssembly(Data data)
        {
            if (data.Path_Assembly == null || data.AssemblyVersion == null)            
                return;
            
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
            Console.WriteLine("updated assembly file");
        }
    }
}

