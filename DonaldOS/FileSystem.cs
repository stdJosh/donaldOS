using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        // Zwischenablage für copy/cut
        private string lastCopied = null;
        private bool cut = false;

        private string GetFileNameFromPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;
            int idx = path.LastIndexOf('\\');
            if (idx < 0) return path;
            return path.Substring(idx + 1);
        }

        // createFile: legt eine (leere) Datei an
        public FileStream createFile(string path, string filename)
        {
           
            if (string.IsNullOrEmpty(path)) path = @"0:\";
            if (!path.EndsWith("\\"))
                path = path + "\\";

           
            FileStream fs = null;
            try
            {
                string full = path + filename;
                fs = File.Create(full);
                Console.WriteLine("Created file: " + full);
            }
            catch (Exception e)
            {
                Console.WriteLine("createFile error: " + e.Message);
            }
            return fs;
        }

        // listDir: Verzeichnisse und Dateien listen
        public void listDir(string path, int recursionLevel = 0, bool recursive = false,
    FileSystemElementTypes elementsToShow = FileSystemElementTypes.All, string filterString = "")
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    path = @"0:\";

                
                path = Path.GetFullPath(path);
                if (!path.EndsWith("\\")) path += "\\";

                if (!Directory.Exists(path))
                {
                    Console.WriteLine("Path does not exist: " + path);
                    return;
                }

                
                if (elementsToShow == FileSystemElementTypes.All || elementsToShow == FileSystemElementTypes.Files)
                {
                    string[] files = Directory.GetFiles(path);

                    foreach (string f in files)
                    {
                        
                        string full = Path.Combine(path, Path.GetFileName(f));

                        if (!string.IsNullOrEmpty(filterString) && !full.Contains(filterString))
                            continue;

                        Console.WriteLine(full);
                    }
                }

                
                if (elementsToShow == FileSystemElementTypes.All || elementsToShow == FileSystemElementTypes.Dirs)
                {
                    string[] dirs = Directory.GetDirectories(path);

                    foreach (string d in dirs)
                    {
                        string display = Path.Combine(path, Path.GetFileName(d));

                        string full = Path.Combine(path, Path.GetFileName(d));

                        Console.WriteLine(full + "\\");

                        if (recursive)
                            listDir(full, recursionLevel + 1, recursive, elementsToShow, filterString);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("listDir error: " + e.Message);
            }
        }



        // Entfernen: Datei oder Ordner (rekursiv) 
        public void remove(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    Console.WriteLine("remove: path empty");
                    return;
                }

                
                if (Directory.Exists(path))
                {
                    
                    DeleteDirectoryRecursive(path);
                    Console.WriteLine("Directory removed: " + path);
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
                    Console.WriteLine("File removed: " + path);
                }
                else
                {
                    Console.WriteLine("remove: not found: " + path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("remove error: " + e.Message);
            }
        }

        private void DeleteDirectoryRecursive(string path)
        {
            try
            {
                // Dateien löschen
                string[] files = Directory.GetFiles(path);
                foreach (var f in files)
                {
                    try { File.Delete(path + '\\' + f); }
                    catch (Exception e) { Console.Write(e.ToString()); }
                }

                // Unterordner rekursiv löschen
                string[] dirs = Directory.GetDirectories(path);
                foreach (var d in dirs)
                {
                    DeleteDirectoryRecursive(d);
                }

                // Ordner selbst löschen
                try { Directory.Delete(path); }
                catch (Exception) { /* ignorieren */ }
            }
            catch (Exception e)
            {
                Console.WriteLine("DeleteDirectoryRecursive error: " + e.Message);
            }
        }

        // mkdir
        public void MakeDir(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) path = @"0:\";
                if (!path.EndsWith("\\")) path = path + "\\";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Console.WriteLine("Folder created: " + path);
                }
                else
                {
                    Console.WriteLine("Folder already exists: " + path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("mkdir error: " + e.Message);
            }
        }

       
        public string NormalizePath(string currentPath, string input)
        {
            if (string.IsNullOrEmpty(input)) return currentPath;
            
            if (input.Contains(@":\"))
            {
                
                return input;
            }

            
            if (input == "..")
            {
                int lastSlash = currentPath.LastIndexOf('\\');
                if (lastSlash > 2) return currentPath.Substring(0, lastSlash);
                return @"0:\";
            }

            
            string basePath = currentPath;
            if (!basePath.EndsWith("\\")) basePath += "\\";
            return basePath + input;
        }

        public bool DirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            return Directory.Exists(path);
        }

        // File copy
        public void CopyFile(string source, string dest)
        {
            try
            {
                if (!File.Exists(source))
                {
                    Console.WriteLine("copy: source not found: " + source);
                    return;
                }

                string destDir = Path.GetDirectoryName(dest);
                if (!Directory.Exists(destDir))
                {
                    Console.WriteLine("copy: destination folder not found: " + destDir);
                    return;
                }

                File.Copy(source, dest, true); 
                Console.WriteLine("Copied to: " + dest);
            }
            catch (Exception e)
            {
                Console.WriteLine("copy error: " + e.Message);
            }
        }


        // Copy/Cut buffer
        public void CopyBufferSet(string path, bool isCut)
        {
            lastCopied = path;
            cut = isCut;
            Console.WriteLine((isCut ? "Cut set: " : "Copy set: ") + path);
        }

        // Paste
        public void PasteIntoDir(string directory)
        {
            try
            {
                if (string.IsNullOrEmpty(lastCopied))
                {
                    Console.WriteLine("Nothing to paste.");
                    return;
                }

                if (string.IsNullOrEmpty(directory)) directory = @"0:\";
                if (!directory.EndsWith("\\")) directory = directory + "\\";

                if (!File.Exists(lastCopied))
                {
                    Console.WriteLine("paste: source missing: " + lastCopied);
                    return;
                }

                string filename = GetFileNameFromPath(lastCopied);
                string target = directory + filename;

                CopyFile(lastCopied, target);

                if (cut)
                {
                    try { File.Delete(lastCopied); }
                    catch (Exception) { /* ignore */ }
                    cut = false;
                }

                Console.WriteLine("Pasted: " + target);
            }
            catch (Exception e)
            {
                Console.WriteLine("paste error: " + e.Message);
            }
        }

        // Move: implementiert als CopyFile + Delete source
        public void MoveFile(string source, string dest)
        {
            try
            {
                
                if (!File.Exists(source))
                {
                    Console.WriteLine("move: source not found: " + source);
                    return;
                }

                
                bool destIsDir = Directory.Exists(dest);

                if (destIsDir)
                {
                    
                    string filename = GetFileNameFromPath(source);

                    if (!dest.EndsWith("\\"))
                        dest += "\\";

                    dest = dest + filename;
                }
                else
                {
                    
                    int lastSlash = dest.LastIndexOf('\\');
                    if (lastSlash < 0)
                    {
                        Console.WriteLine("move: invalid destination path: " + dest);
                        return;
                    }

                    string destFolder = dest.Substring(0, lastSlash);

                    if (!Directory.Exists(destFolder))
                    {
                        Console.WriteLine("move: destination folder does not exist: " + destFolder);
                        return;
                    }
                }

                
                CopyFile(source, dest);

                try { File.Delete(source); }
                catch { }

                Console.WriteLine("Moved: " + source + " -> " + dest);
            }
            catch (Exception e)
            {
                Console.WriteLine("move error: " + e.Message);
            }
        }

    }
}

