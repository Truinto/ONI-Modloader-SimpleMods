using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

#pragma warning disable SYSLIB1045 // GeneratedRegexAttribute

namespace Shared.PathsNS
{
    public enum FileType
    {
        Undefined,
        Directory,
        File,
    }

    /// <summary>
    /// WIP. Tool to handle path operations.
    /// </summary>
    public class PathInfo
    {
        public static char PathSeparator => System.IO.Path.DirectorySeparatorChar;

        public static bool IsValidPath(string path)
        {
            try
            {
                System.IO.Path.GetFullPath(path);
                return true;
            }
            catch
            {
                return false;
            }
            //return name.IndexOfAny(Path.GetInvalidPathChars()) == -1;
        }

        private bool isDirty;

        public PathInfo(string path)
        {
            if (string.IsNullOrEmpty(path))
                path = ".";
            path = path.Replace(PathSeparator == '/' ? '\\' : '/', PathSeparator);

            int indexSlash = path.LastIndexOf(PathSeparator);
            int indexDot = path.LastIndexOf('.');
            bool hasNoExtension = indexDot <= indexSlash + 1;

            this.FullName = path;
            this.Root = System.IO.Path.GetPathRoot(path);
            this.IsAbsolute = this.Root.Length > 0;
            this.Directory = path.Substring(0, indexSlash + 1);
            if (hasNoExtension)
            {
                this.FileNameNoExtension = path.Substring(indexSlash + 1);
                if (this.FileNameNoExtension.EndsWith("."))
                    this.FileNameNoExtension = this.FileNameNoExtension.Substring(1);
                this.Extension = "";
                this.FileName = this.FileNameNoExtension;
            }
            else
            {
                this.FileNameNoExtension = path.Substring(indexSlash + 1, indexDot - indexSlash - 1);
                this.Extension = path.Substring(indexDot + 1);
                this.FileName = $"{this.FileNameNoExtension}.{this.Extension}";
            }
        }

        public bool IsValid { get; } //WIP

        public string FullName { get; }

        public string Root { get; }

        public string Directory { get; }

        public string FileNameNoExtension { get; }

        public string Extension { get; }

        public string FileName { get; }

        public FileType Type { get; } //WIP

        public bool IsAbsolute { get; } //WIP

        public string PathAbsolute(string workingDirectory = null)
        {
            if (this.IsAbsolute && !string.IsNullOrEmpty(workingDirectory))
                Trace.WriteLine($"[Warning] {typeof(PathInfo).FullName}.PathAbsolute trying to set working directory on an absolute path '{this.FullName}'");

            if (this.IsAbsolute)
                return this.FullName;

            workingDirectory ??= System.IO.Directory.GetCurrentDirectory(); //System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
            return workingDirectory + this.FullName;

            throw new NotImplementedException();
        }

        private static void Sandbox()
        {
            System.IO.Directory.Delete("");
            System.IO.Directory.Exists("");
            System.IO.Directory.CreateDirectory("");

            System.IO.File.Delete("");
            System.IO.File.Exists("");
            System.IO.File.CreateText("");

            System.IO.Path.ChangeExtension("", "");
            System.IO.Path.Combine("", "");
            System.IO.Path.GetDirectoryName(""); // warning, this is inconsistent
            System.IO.Path.GetExtension("");
            System.IO.Path.GetFileName("");
            System.IO.Path.GetFileNameWithoutExtension("");
            System.IO.Path.GetFullPath("");
            System.IO.Path.GetInvalidFileNameChars();
            System.IO.Path.GetPathRoot("");
            System.IO.Path.GetRandomFileName();
            System.IO.Path.GetTempFileName();
            System.IO.Path.GetTempPath();
            System.IO.Path.HasExtension("");
            System.IO.Path.IsPathRooted("");

            var di = new System.IO.DirectoryInfo("");
            _ = di.Parent;
            _ = di.Exists;
            _ = di.FullName;
            di.Create();
            di.Delete();
            di.EnumerateDirectories();
            di.EnumerateFiles();
            di.GetDirectories();
            di.GetFiles();
            di.MoveTo("");
            di.CreateSubdirectory("");

            var fi = new System.IO.FileInfo("");
            _ = fi.Exists;
            fi.CreateText();
            fi.Delete();

        }
    }

    /// <summary>
    /// Collection of special folders.
    /// </summary>
    public static class Paths
    {
        /// <summary>
        /// Splits a path into dir, file, and ext.
        /// </summary>
        /// <remarks>
        /// @"(?&lt;dir&gt;.*)(?&lt;file&gt;[^\\\/]*)(?&lt;ext&gt;\.[^\\\/]+?)?$"
        /// </remarks>
        public static Regex Rx_Path => _Rx_Path;
        private static readonly Regex _Rx_Path = new(@"^(?<dir>.*)(?<file>[^\\\/]*)(?<ext>\.[^\\\/]+?)?$", RegexOptions.Compiled | RegexOptions.RightToLeft);

        /// <summary>True if paths are equal. Resolves relative paths. Ignores closing path separator.</summary>
        public static bool AreEqual(this FileInfo path1, FileInfo path2)
        {
            return AreEqual(path1.FullName, path2.FullName);
        }

        /// <summary>True if paths are equal. Resolves relative paths. Ignores closing path separator.</summary>
        public static bool AreEqual(string path1, string path2)
        {
            if (path1 is null or "")
                return path1 == path2;

            path1 = Path.GetFullPath(path1);
            path2 = Path.GetFullPath(path2);

            int length1 = path1.Length;
            int length2 = path2.Length;

            int length;
            if (length1 == length2)
                length = length1;
            else if (length1 - 1 == length2 && path1[length2] is '/' or '\\')
                length = length2;
            else if (length1 == length2 - 1 && path2[length1] is '/' or '\\')
                length = length1;
            else
                return false;

            return string.Compare(
                path1, 0,
                path2, 0,
                length,
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal
                ) == 0;
        }

        /// <summary>Folder path of the currently running executable. Same as <seealso cref="AppContext.BaseDirectory"/>.</summary>
        public static string AssemblyDirectory => AppContext.BaseDirectory;

        /// <summary>Folder path of the working directory. Same as <seealso cref="Environment.CurrentDirectory"/>.</summary>
        public static string WorkingDirectory => Environment.CurrentDirectory;

        /// <summary>%username%</summary>
        public static string Username => Environment.UserName;

        /// <summary>C:\Users\%username%</summary>
        public static string UserProfile => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.DoNotVerify);

        /// <summary>C:\Users\%username%\Desktop</summary>
        public static string DesktopDirectory => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory, Environment.SpecialFolderOption.DoNotVerify);

        /// <summary>C:\Program Files</summary>
        public static string ProgramFiles => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles, Environment.SpecialFolderOption.DoNotVerify);

        /// <summary>C:\Program Files (x86)</summary>
        public static string ProgramFilesX86 => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86, Environment.SpecialFolderOption.DoNotVerify);

        /// <summary>C:\Users\%username%\OneDrive - Bruker Physik GmbH\Documents</summary>
        public static string MyDocuments => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.DoNotVerify);

        /// <summary>C:\Users\%username%\Pictures</summary>
        public static string MyPictures => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures, Environment.SpecialFolderOption.DoNotVerify);

        /// <summary>C:\Users\%username%\Music</summary>
        public static string MyMusic => Environment.GetFolderPath(Environment.SpecialFolder.MyMusic, Environment.SpecialFolderOption.DoNotVerify);

        /// <summary>C:\Users\%username%\Videos</summary>
        public static string MyVideos => Environment.GetFolderPath(Environment.SpecialFolder.MyVideos, Environment.SpecialFolderOption.DoNotVerify);

        /// <summary>C:\Users\%username%\AppData\Roaming</summary>
        public static string ApplicationData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.DoNotVerify);

        /// <summary>C:\Users\%username%\AppData\Local</summary>
        public static string LocalApplicationData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify);

        /// <summary>C:\ProgramData</summary>
        public static string CommonApplicationData => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData, Environment.SpecialFolderOption.DoNotVerify);

        /// <summary>C:\WINDOWS\system32</summary>
        public static string System => Environment.GetFolderPath(Environment.SpecialFolder.System, Environment.SpecialFolderOption.DoNotVerify);

        /// <summary>C:\WINDOWS\SysWOW64</summary>
        public static string SystemX86 => Environment.GetFolderPath(Environment.SpecialFolder.SystemX86, Environment.SpecialFolderOption.DoNotVerify);

        /// <summary>C:\Users\%username%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup</summary>
        public static string Startup => Environment.GetFolderPath(Environment.SpecialFolder.Startup, Environment.SpecialFolderOption.DoNotVerify);

        /// <summary>C:\Users\%username%\Desktop<br/>Prefer DesktopDirectory instead.</summary>
        public static string Desktop => Environment.GetFolderPath(Environment.SpecialFolder.Desktop, Environment.SpecialFolderOption.DoNotVerify);
    }
}
