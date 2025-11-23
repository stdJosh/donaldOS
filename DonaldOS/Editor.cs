using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DonaldOS
{
    // todo:
    // markieren und copy paste ? --
    // scrollen können (pfeiltasten)  -/
    // zurück                         //
    // wieder vor                     //
    // schreiben / editiren           -/
    // beim nach oben drücken länge beachten -/
    // beim del in einer leeren zeile soll sie gelöscht werden bzw wenn wenn an pos 0 soll der "reststring" nach oben geschoben werden -/
    // view machen (falls zu viele zeilen) -/
    // speichern lol -/ 
    //offset mitzählen (oben, unten, links (bei 0), rechts bei length, delete (bei x%80 = 0 und y-offset = 0) und enter -/
    // error handling ? statt abstürzen evt lieber eine meldung ausgeben also immer ein try catch haben in der while(true) und das run beenden ? evt mit speichern


    //aktuelles problem / noch zu machen: 25. zeile nicht angezeigt (also der inhalt) aber editierbar
    // mehrzeilige string werden immer als ganzes angezeigt 
    // programm schließen (automatisch speichern verfügbar machen)
    // error handling ? automatisch gespeichert (real file zeug, cursor position, filesave, beenden, bei strings(wegen index) evt bei (sys. out of range)commandexecutionshelp, ex.tostring statt message)
    //2 letzten zeilen für comands (und evt daten wie x, y und automatisch speichern) keine daten
    //vor zurück mit strg z und strg y und einer liste an changes  (alle lines speichern oder cursorposition und key)
    // wenn ich dann zurück im editor bin sind alle änderungen mit in der anzeige ?



    //user  management (read level, write level pro funktion prüfen und in string [0] speichern ? und nach auslesen abtrennen und extra speichern

    internal class Editor
    {
        List<string> rows = new List<string>();
        string filename;

        int cursorX = 0;
        int cursorY = 0;
        int width = Console.WindowWidth;
        int extra = 0;
        int offset = 0;
        bool saved = true;

        //kostruktor
        public Editor(string filename)
        {
            this.filename = filename;

        }

        //initialisieren der rows
        public void ReadFile()
        {
            System.Console.Clear();
            rows = new List<string>(File.ReadAllLines(filename));

            if (rows.Count == 0)
            {
                rows.Add("");
            }
        }

        public void PrintFile()
        {
            int lang;
            extra = 0;
            int i = 0;
            int max_begrenzer = 0;
            int n = 0;

            //if (offset != 0)
            //{
            //    Console.WriteLine("offset ist " + offset);
            //}

            for (n = 0 + offset; n < rows.Count && n < offset + 24 - max_begrenzer; n++)
            {
                //muss testen ob die row abgeschnitten wird und somit das y beeinflusst (alle vorher)
                if ((lang = rows[n].Length) > width && cursorY - offset > i)
                {
                    extra += (lang / width);
                }

                if ((lang = rows[n].Length) > width)
                {
                    max_begrenzer += (lang / width);
                }

                //der akktuelle string
                if (i == cursorY - offset)
                {
                    //extra dazuzeählen wenn nicht in 1. "zeile"
                    extra += cursorX / width;
                }
                System.Console.WriteLine(rows[n]);

                //Console.WriteLine(rows[n] + "         cursor ist bei " + offset);

                i++;
            }

            //if (offset != 0)
            //{
            //    Console.WriteLine("offset ist " + offset);
            //}
        }



        private void SaveText()
        {
            System.Console.Clear();

            System.Console.WriteLine("versuche speichern");


            try
            {
                using (var fs = File.Open(filename, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    foreach (string line in rows)
                    {
                        sw.WriteLine(line);
                    }
                }
                System.Console.WriteLine("Datei gespeichert!");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Fehler beim Speichern: " + ex.Message);
            }
        }

        bool AskSaveAndDelete()
        {
            string antwort = "";

            System.Console.Clear();

            while (true)
            {
                System.Console.WriteLine("Moechtest du das Programm vorher speichern und dann erst beenden? Ja/Nein");
                antwort = System.Console.ReadLine()?.Trim().ToLower();

                if (antwort == "ja" || antwort == "nein")
                    break; // gültige Eingabe → Schleife verlassen

                System.Console.WriteLine("Bitte gib 'Ja' oder 'Nein' ein!");
                Thread.Sleep(1000);
            }

            if (antwort == "ja")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void editmode()
        {

            while (true)
            {
                //todo: nicht immer alles neu zeichnen
                System.Console.Clear();
                PrintFile();  // Gibt alle logischen Zeilen aus

                System.Console.SetCursorPosition(cursorX % width, (cursorY + extra) - offset); // modulo für physische Zeile

                System.ConsoleKeyInfo key = System.Console.ReadKey();

                if (key.Key == ConsoleKey.S && (key.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    SaveText();
                    System.Threading.Thread.Sleep(3000);
                    saved = true;
                    continue;
                }

                if (key.Key == ConsoleKey.X && (key.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    if (saved)
                    {
                        System.Console.Clear();
                        break;
                    }

                    if (AskSaveAndDelete())
                    {
                        SaveText();
                        System.Threading.Thread.Sleep(3000);
                        System.Console.Clear();
                        break;
                    }
                    else
                    {
                        break;
                    }

                }

                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (cursorX > 0)
                        {
                            cursorX--;
                        }
                        else if (cursorY > 0)
                        {
                            cursorX = rows[cursorY - 1].Length;

                            if (cursorY - offset == 0)
                            {
                                offset--;
                            }

                            cursorY--;
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (cursorX < rows[cursorY].Length)
                        {
                            cursorX++;
                        }
                        else if (cursorY + 1 < rows.Count)
                        {
                            cursorY++;
                            cursorX = 0;

                            if (cursorY - offset >= 23)
                            {
                                offset++;
                            }

                        }
                        break;

                    case ConsoleKey.UpArrow:
                        string line = rows[cursorY];

                        // Welche physische Zeile innerhalb dieser logischen Zeile bin ich gerade?
                        int cursorXrow2 = (cursorX + 1) / width;
                        if ((cursorX + 1) % width != 0)
                        {
                            cursorXrow2++;
                        }

                        if (line.Length > width && cursorXrow2 > 1)
                        {
                            // Noch innerhalb der gleichen logischen Zeile -> eine physische Zeile hoch
                            cursorX -= width;
                            if (cursorX < 0)
                                cursorX = 0;

                            //multilineoffset
                        }
                        else
                        {
                            // In der obersten physischen Zeile oder normale Zeile -> zur vorherigen logischen Zeile
                            if (cursorY > 0)
                            {
                                string prevLine = rows[cursorY - 1];

                                // Wieviele Bildschirmzeilen hat die vorherige row?
                                int prevlinerows = prevLine.Length / width;
                                if (prevLine.Length % width != 0) { prevlinerows++; }

                                if (prevLine.Length == 0)
                                {
                                    cursorX = 0;
                                }
                                else
                                {
                                    // Neue Cursorposition innerhalb der vorherigen logischen Zeile
                                    // gleiche horizontale Position beibehalten, aber eine Zeile höher landen
                                    cursorX = (prevlinerows - 1) * width + (cursorX % width);
                                }


                                // Falls die vorherige Zeile kürzer ist, Cursor ans Ende
                                if (cursorX > prevLine.Length)
                                    cursorX = prevLine.Length;

                                if (cursorY - offset == 0)
                                {
                                    offset--;
                                }
                                cursorY--;
                            }
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        string currentLine = rows[cursorY];
                        if (currentLine.Length > width)
                        {
                            //wie viele volle zeilen (+1 bei cursor weil bei 80 (2. zeile) sonst 1ne zeile nur rauskommt (weil ja von 0 bis 79.
                            int cursorXrow = (cursorX + 1) / width;
                            //wenn eine unfertige zeile
                            if ((cursorX + 1) % width != 0) cursorXrow++;

                            int currentlinerow = currentLine.Length / width;
                            if (currentLine.Length % width != 0) currentlinerow++;

                            //wenn es in dem mehrzeilingen string noch eine zeile gibt 
                            if (cursorXrow < currentlinerow)
                            {
                                // Zeile ist länger als Bildschirmbreite 
                                cursorX += width;
                                //wenn die zeile darunter kürzer ist 
                                if (cursorX > currentLine.Length) cursorX = currentLine.Length;

                                if (cursorY - offset >= 23)
                                {
                                    offset++;
                                }

                            }
                            //das gleiche wie wenn es keine mehrere reihen geben würde
                            else
                            {
                                // Zeile passt auf Bildschirmbreite -> nächste Zeile
                                if (cursorY < rows.Count - 1)
                                {
                                    cursorY++;
                                    //falls es vorher mehrere zeilen waren wäre cursor X zu hoch
                                    cursorX = cursorX % width;
                                    currentLine = rows[cursorY];
                                    if (cursorX > currentLine.Length) cursorX = currentLine.Length;

                                    if (cursorY - offset >= 23)
                                    {
                                        offset++;
                                    }

                                }
                            }
                        }
                        else
                        {
                            // Zeile passt auf Bildschirmbreite -> nächste Zeile
                            if (cursorY < rows.Count - 1)
                            {

                                //falls es vorher mehrere zeilen waren wäre cursor X zu hoch
                                cursorX = cursorX % width;
                                currentLine = rows[cursorY];
                                if (cursorX > currentLine.Length) cursorX = currentLine.Length;

                                if (cursorY - offset >= 23)
                                {
                                    offset++;
                                }

                                cursorY++;

                            }
                        }
                        break;

                    case ConsoleKey.Backspace:

                        saved = false;

                        if (cursorX > 0)
                        {
                            //löscht das zeichen bei string[cursorX - 1] 
                            rows[cursorY] = rows[cursorY].Remove(cursorX - 1, 1);
                            cursorX--;
                        }
                        else if (cursorY != 0)
                        {
                            //hier drinne wenn x = 0 und y != 0
                            int oldLen = rows[cursorY - 1].Length;

                            rows[cursorY - 1] += rows[cursorY];
                            rows.RemoveAt(cursorY);


                            cursorX = oldLen;

                            if (cursorY - offset == 0)
                            {
                                offset--;
                            }

                            cursorY--;
                        }
                        break;

                    case ConsoleKey.Enter:

                        saved = false;

                        string rest = "";
                        if (cursorX < rows[cursorY].Length)
                            rest = rows[cursorY].Substring(cursorX);
                        rows[cursorY] = rows[cursorY].Substring(0, cursorX);

                        if (cursorY + 1 < rows.Count)
                            rows.Insert(cursorY + 1, rest);
                        else
                            rows.Add(rest); // falls du am Ende bist 


                        if (cursorY - offset >= 23)
                        {
                            offset++;
                        }

                        cursorY++;
                        cursorX = 0;


                        break;

                    default:
                        //zwischen 32 " " und 126 sind alle buchstaben zahlen und satzzeichen
                        if (key.KeyChar >= 32 && key.KeyChar <= 126)
                        {
                            rows[cursorY] = rows[cursorY].Insert(cursorX, key.KeyChar.ToString());
                            cursorX++;
                        }

                        saved = false;

                        break;
                }
            }
        }
    }
}