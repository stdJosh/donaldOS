using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;

namespace DonaldOS
{
    enum FileSystemElementTypes
    {
        Dirs,
        Files,
        All
    }
    internal class FileSystem
    {
        public FileStream createFile(string path, string filename)
        {
            if (path.Last()  != '\\')
            {
                path.Append('\\');
            }

            FileStream file_stream = null;
            try
            {
                 file_stream = File.Create(path + filename);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return file_stream;
        }

        public void listDir(string path, int recursionLevel = 0, bool recursive = false, FileSystemElementTypes elementsToShow = FileSystemElementTypes.All, string filterString = "")
        { // TODO: rekursive Aufrufe geben nicht gleich alle Elemente aus, sondern befüllen nur nach und nach ein Array
          // von einer eigenen FileSystemElement-Klasse, in dem die auszugebenden Elemente drinstehen
          // Vorteil: bessere Ausgabe bei Angabe eines FilterStrings (würde Zurückgehen im Array und Löschen ermöglichen
            if (!Directory.Exists(path))
            {
                throw new Exception("Path does not exist");
            }

            if (!path.EndsWith('\\'))
            {
                path.Append('\\');
            }

            if (elementsToShow == FileSystemElementTypes.All || elementsToShow == FileSystemElementTypes.Files)
            { 
                string[] fileNames = Directory.GetFiles(path);
                foreach (string fileName in fileNames)
                {
                    if (filterString == "" || fileName.Contains(filterString))
                    {
                        Console.WriteLine(path + fileName);
                    }
                }
            }
            if (elementsToShow == FileSystemElementTypes.All || elementsToShow == FileSystemElementTypes.Dirs)
            {
                string[] dirNames = Directory.GetDirectories(path);
                foreach (string dirName in dirNames)
                {
                    if (filterString == "" || dirName.Contains(filterString))
                    {
                        Console.WriteLine(path + dirName + '\\');
                    }
                    if (recursive)
                    {
                        listDir(path + dirName, recursionLevel + 1, recursive, elementsToShow, filterString);
                    }
                }
            }
        }

        public void remove(string path)
        {
            if (Directory.Exists(path))
            {
                //
            }
            else if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
