using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Media_File_Renamer
{
    class Program
    {
        //FIELDS
        private const int MATCH_LENGTH = 6; //6 for "S##E##"

        private static readonly string[] Extensions =
        {
            ".3g2", ".3gp", ".asf", ".avi", ".drc", ".flv", ".f4v", ".fvp", ".f4a", ".f4b", ".gif",
            ".gifv", ".m4v", ".mkv", ".mng", ".mov", ".qt", ".mp4",".m4p", ".mpg", ".mpeg", ".m2v",
            ".mxf", ".nsv", ".ogv", ".ogg", ".rm",  ".rmvb", ".roq", ".svi", ".vob", ".webm", ".wmv", ".yuv",

            ".srt"
        };

        private static string path; //The directory in which the program operates.

        private static Dictionary<string, string> files = new Dictionary<string, string>(); //The list of filenames and their extensions.


        //METHODS
        private static void Main(string[] args)
        {
            path = args[0] + "\\";

            GetAllFiles();
            if (files.Count() > 0)    
            {
                List<string> renamedFiles = RenameFiles();
    
                Console.WriteLine("Renamed Files:\n--------------");
                for (int i = 0; i < renamedFiles.Count; i += 2)
                {
                    Console.WriteLine("{0} -> {1}", renamedFiles[i], renamedFiles[i + 1]);
                }
            }
            else
            {
                Console.WriteLine("No files to rename.");
            }
            
            Console.ReadLine();
        }

        private static void GetAllFiles()
        {
            Dictionary<string, string> allFiles = new Dictionary<string, string>();
            DirectoryInfo directoryMangaer = new DirectoryInfo(path);
            
            foreach (string extension in Extensions)
            {
                FileInfo[] tempFiles = directoryMangaer.GetFiles("*" + extension);
                foreach (FileInfo file in tempFiles)
                {
                    string fileName = file.Name;
                    Match match = Regex.Match(fileName, "[Ss][0-9][0-9][Ee][0-9][0-9]");
                    if (match.Success && fileName.Length != MATCH_LENGTH + extension.Length) //Avoid renaming files in correct format.
                    {
                        allFiles.Add(fileName, extension);
                    }
                }
            }

            files = allFiles;
        }

        private static List<string> RenameFiles()
        {
            List<string> renamedFiles = new List<string>();

            foreach (string fileName in files.Keys)
            {
                Match match = Regex.Match(fileName, "[Ss][0-9][0-9][Ee][0-9][0-9]");
                if (match.Success)
                {
                    int sIndex = match.Index;

                    renamedFiles.Add(fileName);
                    File.Move(path + fileName, //Original full path
                        path + //Path
                        fileName.Substring(sIndex, MATCH_LENGTH).ToUpper() //S##E## section
                        + files[fileName] //MediaFile extension
                    );

                    renamedFiles.Add(fileName.Substring(sIndex, MATCH_LENGTH) + files[fileName]);
                }
            }

            return renamedFiles;
        }
    }
}
