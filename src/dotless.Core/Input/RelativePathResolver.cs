using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;
namespace dotless.Core.Input
{
    class RelativePathResolver : IPathResolver
    {
        private string CurrentDirectory;

        private List<string> Roots = new List<string>();

        public RelativePathResolver(string curDir)
        {
            CurrentDirectory = curDir;

            foreach (var drive in DriveInfo.GetDrives())
            {
                Roots.Add(drive.RootDirectory.Name);
            }
        }

        public string GetFullPath(string path)
        {
            // Path.IsRooted or whatever doesn't do what you think it does, use this instead
            if (Roots.Any(r => path.StartsWith(r))) return path;

            return Path.Combine(CurrentDirectory, path);
        }
    }
}