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
            // Pfad-String korrekt machen
            if (string.IsNullOrEmpty(path)) path = @"0:\";
            if (!path.EndsWith("\\"))
                path = path + "\\";

            // FileStream erzeugen
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
                if (string.IsNullOrEmpty(path)) path = @"0:\";
                // Keine Path.GetFullPath - einfach sicherstellen:
                if (!path.EndsWith("\\")) path = path + "\\";

                if (!Directory.Exists(path))
                {
                    throw new Exception("Path does not exist: " + path);
                }

                // Dateien
                if (elementsToShow == FileSystemElementTypes.All || elementsToShow == FileSystemElementTypes.Files)
                {
                    string[] fileNames = Directory.GetFiles(path);
                    foreach (string fileName in fileNames)
                    {
                        // fileName ist möglicherweise schon komplett; wir geben lesbare Form aus
                        string display = path + fileName;
                        if (!string.IsNullOrEmpty(filterString) && !display.Contains(filterString))
                            continue;
                        Console.WriteLine(display);
                    }
                }

                // Ordner
                if (elementsToShow == FileSystemElementTypes.All || elementsToShow == FileSystemElementTypes.Dirs)
                {
                    string[] dirNames = Directory.GetDirectories(path);
                    foreach (string dirName in dirNames)
                    {
                        string display = path + dirName;
                        if (!string.IsNullOrEmpty(filterString) && !display.Contains(filterString))
                            continue;

                        Console.WriteLine(display + "\\");

                        if (recursive)
                        {
                            // rekursiv aufrufen: Pfad bereits komplett
                            listDir(dirName, recursionLevel + 1, recursive, elementsToShow, filterString);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("listDir error: " + e.Message);
            }
        }

        // Entfernen: Datei oder Ordner (rekursiv) - manuell implementiert
        public void remove(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    Console.WriteLine("remove: path empty");
                    return;
                }

                // Wenn Pfad kein TrailingSlash und Directory.Exists(path) false, könnte es eine Datei sein
                if (Directory.Exists(path))
                {
                    // Rekursiv löschen: Dateien löschen, dann Unterordner rekursiv, dann Ordner selbst
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

        // NormalizePath: sehr simpel, keine Path.GetFullPath
        public string NormalizePath(string currentPath, string input)
        {
            if (string.IsNullOrEmpty(input)) return currentPath;
            // Absoluter Pfad: enthält ":\"
            if (input.Contains(@":\"))
            {
                // ensure trailing slash if dir
                return input;
            }

            // Sonderfall ..
            if (input == "..")
            {
                int lastSlash = currentPath.LastIndexOf('\\');
                if (lastSlash > 2) return currentPath.Substring(0, lastSlash);
                return @"0:\";
            }

            // Relativ: zusammenbauen
            string basePath = currentPath;
            if (!basePath.EndsWith("\\")) basePath += "\\";
            return basePath + input;
        }

        public bool DirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            return Directory.Exists(path);
        }

        // File copy: Byte-für-Byte über FileStreams (robust)
        public void CopyFile(string source, string dest)
        {
            try
            {
                if (!File.Exists(source))
                {
                    Console.WriteLine("copy: source not found: " + source);
                    return;
                }

                // Zielordner prüfen
                int last = dest.LastIndexOf('\\');
                if (last > 0)
                {
                    string folder = dest.Substring(0, last);
                    if (!Directory.Exists(folder))
                    {
                        Console.WriteLine("copy: destination folder does not exist: " + folder);
                        return;
                    }
                }

                // Byte-Kopie
                using (FileStream inFs = File.Open(source, FileMode.Open, FileAccess.Read))
                using (FileStream outFs = File.Create(dest))
                {
                    byte[] buffer = new byte[4096];
                    int read;
                    while ((read = inFs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outFs.Write(buffer, 0, read);
                    }
                }

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
                // 1. Quelle prüfen
                if (!File.Exists(source))
                {
                    Console.WriteLine("move: source not found: " + source);
                    return;
                }

                // 2. Wenn dest ein Ordner ist → finalen Pfad bauen
                bool destIsDir = Directory.Exists(dest);

                if (destIsDir)
                {
                    // Ordner → Dateiname anhängen
                    string filename = GetFileNameFromPath(source);

                    if (!dest.EndsWith("\\"))
                        dest += "\\";

                    dest = dest + filename;
                }
                else
                {
                    // dest ist KEIN Ordner → wir müssen prüfen, ob der Ordner existiert
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

                // 3. Datei kopieren (Byte für Byte)
                CopyFile(source, dest);

                // 4. Original löschen
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

