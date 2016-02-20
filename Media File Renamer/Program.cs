using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Media_File_Renamer
{
    class MediaFile
    {
        internal string path;
        internal string extension;

        public MediaFile(string initPath, string initExt)
        {
            path = initPath;
            extension = initExt;
        }
    }

    class Program
    {
        //FIELDS
        private static readonly Dictionary<string, int> ExtensionPairs = new Dictionary<string, int>()
        {
            {".3g2", 4}, {".3gp",  4},  {".asf",  4},  {".avi",  4},  {".drc", 4},  {".flv",  4},
            {".f4v", 4}, {".fvp",  4},  {".f4a",  4},  {".f4b",  4},  {".gif", 4},  {".gifv", 5},
            {".m4v", 4}, {".mkv",  4},  {".mng",  4},  {".mov",  4},  {".qt",  3},  {".mp4",  4},
            {".m4p", 4}, {".mpg",  4},  {".mpeg", 5},  {".m2v",  4},  {".mxf", 4},  {".nsv",  4},
            {".ogv", 4}, {".ogg",  4},  {".rm",   3},  {".rmvb", 5},  {".roq", 4},  {".svi",  4},
            {".vob", 4}, {".webm", 5},  {".wmv",  4},  {".yuv",  4},

            {".srt", 4}
        };

        private static string path; //The directory in which the program operates.

        private static List<MediaFile> files = new List<MediaFile>(); //The list of filenames and their extensions.

        //METHODS
        private static void Main(string[] args)
        {
            path = args[0] + "\\";

            files = GetAllFiles();
            List<string> renamedFiles = RenameFiles();
        }

        private static List<MediaFile> GetAllFiles()
        {
            List<MediaFile> allFiles = new List<MediaFile>();
            DirectoryInfo directoryMangaer = new DirectoryInfo(path);


            foreach (string extension in ExtensionPairs.Keys)
            {
                FileInfo[] tempFiles = directoryMangaer.GetFiles("*" + extension);
                foreach (FileInfo file in tempFiles)
                {
                    string fileName = file.Name;
                    Match match = Regex.Match(fileName, "[Ss][0-9][0-9][Ee][0-9][0-9]");
                    if (match.Success && fileName.Length != 6 + ExtensionPairs[extension]) //7: 6 for "S##E##"
                    {
                        allFiles.Add(new MediaFile(path, extension));
                    }
                }
            }

            return allFiles;
        }

        private static List<string> RenameFiles() //TODO remove magic numbers.
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
                        fileName.Substring(sIndex, 6).ToUpper() //S##E## section
                        + files[fileName] //MediaFile extension
                    );

                    renamedFiles.Add(fileName.Substring(sIndex, 6) + files[fileName]);
                }
            }

            return renamedFiles;
        }
    }
}