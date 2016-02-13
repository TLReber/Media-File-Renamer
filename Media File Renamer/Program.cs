using System;
using System.Collections.Generic;
using System.IO;

namespace Media_File_Renamer
{
    class Program
    {
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
                List<FileInfo> renamedFiles = RenameFiles();

                Console.WriteLine("\nRenamed Files:");
                Console.WriteLine("--------------");
                foreach (FileInfo file in renamedFiles)
                {
                    Console.WriteLine(file.Name);
                }
            }
            else
            {
                Console.WriteLine("Exiting application.");
            }

            Console.ReadLine();
        }

        private static List<FileInfo> GetAllFiles()
        {
            List<FileInfo> files = new List<FileInfo>();
            DirectoryInfo directoryMangaer = new DirectoryInfo(path);

            foreach (string extension in FileExtensions)
            {
                FileInfo[] tempFiles = directoryMangaer.GetFiles("*" + extension);
                for (int i = 0; i < tempFiles.Length; i++)
                {
                    if (tempFiles[0].Name.Length > 10)
                    {
                        files.Add(tempFiles[i]);
                    }
                }
            }

            return files;
        }

        private static List<FileInfo> RenameFiles()
        {
            List<FileInfo> renamedFiles = new List<FileInfo>();

            foreach (FileInfo file in files)
            {
                string currentName = file.Name.ToUpper();
                int sIndex = currentName.IndexOf('S');
                bool startFound = false;

                while (!startFound && sIndex <= currentName.Length - 10) //-10 as must be able to go 5 spots forward for **E** and then another 4 for *.(extension) + 1 for bounding
                {

                    if (48 <= (int)currentName[sIndex + 1] && (int)currentName[sIndex + 1] <= 57) //[48, 57] are the ASCII values of decimal digits.
                    {
                        if (48 <= (int)currentName[sIndex + 2] && (int)currentName[sIndex + 2] <= 57)
                            if (currentName[sIndex + 3] == 'E')
                                if (48 <= (int)currentName[sIndex + 4] && (int)currentName[sIndex + 4] <= 57)
                                    if (48 <= (int)currentName[sIndex + 5] && (int)currentName[sIndex + 5] <= 57)
                                        startFound = true;
                    }
                    if (!startFound)
                    {
                        sIndex++;
                        sIndex = currentName.IndexOf('S', sIndex);
                    }
                }

                if (startFound)
                {
                    renamedFiles.Add(file);
                    File.Move(path + file.Name, path + currentName.Substring(sIndex, 6) + file.Name.Substring(file.Name.Length - 4));
                }
            }

            return renamedFiles;
        }
    }
}