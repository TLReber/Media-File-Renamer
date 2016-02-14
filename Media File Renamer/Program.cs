using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Media_File_Renamer
{
    class Program
    {
        //TODO factor out common code (regex in GetAllFiles() + RenameFiles())
        //TODO add support for file extensions of any length, consider dictionary <string extName, int extLength>

        private static readonly string[] FileExtensions = { ".3g2", ".3gp", ".asf", ".avi", ".drc",
            ".flv", ".f4v", ".fvp", ".f4a", ".f4b", ".gif", ".gifv", ".m4v", ".mkv", ".mng",
            ".mov", ".qt", ".mp4", ".m4p", ".mpg", ".mpeg", ".m2v", ".mxf", ".nsv", ".ogv",
            ".ogg", ".rm", ".rmvb", ".roq", ".svi", ".vob", ".webm", ".wmv", ".yuv",

            ".srt"
        };

        private static string path;
        private static List<FileInfo> files;

        static void Main(string[] args)
        {
            Console.WriteLine("Specify the location of the files you would like to rename.");
            path = Console.ReadLine();

            while (!Directory.Exists(path))
            {
                Console.WriteLine("Please enter a valid path.");
                path = Console.ReadLine();
            }

            if (path[path.Length - 1] != '\\') path = path + "\\";
            files = GetAllFiles();

            Console.WriteLine("\nList of files to be renamed:");
            Console.WriteLine("----------------------------");
            foreach (FileInfo mediaFile in files)
            {
                Console.WriteLine(mediaFile.Name);
            }

            Console.WriteLine("\nProceed with renaming? (Y/N)");
            if (Console.ReadLine().ToUpper().Equals("Y"))
            {
                Console.WriteLine("\nRenaming files.");
                List<string> renamedFiles = RenameFiles();

                Console.WriteLine("\nRenamed Files:");
                Console.WriteLine("--------------");
                for (int i = 0; i < renamedFiles.Count; i += 2)
                {
                    Console.WriteLine(renamedFiles[i] + " -> " + renamedFiles[i + 1]);
                }
            }
            else //TODO add support for passing new directory instead of exiting.
            {
                Console.WriteLine("\nExiting application.");
            }

            Console.ReadLine();
        }

        private static List<FileInfo> GetAllFiles()
        {
            List<FileInfo> allFiles = new List<FileInfo>();
            DirectoryInfo directoryMangaer = new DirectoryInfo(path);

            foreach (string extension in FileExtensions)
            {
                FileInfo[] tempFiles = directoryMangaer.GetFiles("*" + extension);
                for (int i = 0; i < tempFiles.Length; i++)
                {
                    string currentName = tempFiles[i].Name.ToUpper();
                    Match match = Regex.Match(currentName, "S[0-9][0-9]E[0-9][0-9]");
                    if (match.Success)
                    {
                        allFiles.Add(tempFiles[i]);
                    }
                }
            }

            return allFiles;
        }

        private static List<string> RenameFiles() //TODO remove magic numbers.
        {
            List<string> renamedFiles = new List<string>();

            foreach (FileInfo file in files)
            {
                string currentName = file.Name.ToUpper();
                Match match = Regex.Match(currentName, "S[0-9][0-9]E[0-9][0-9]");
                if (match.Success)
                {
                    int sIndex = match.Index;

                    renamedFiles.Add(file.Name);
                    File.Move(path + file.Name, //Original fully qualified name
                        path + //Path
                        currentName.Substring(sIndex, 6) //SxxExx section
                        + file.Name.Substring(file.Name.Length - 4) //File extension
                    );

                    renamedFiles.Add(currentName.Substring(sIndex, 6) + file.Name.Substring(file.Name.Length - 4));
                }
            }

            return renamedFiles;
        }
    }
}