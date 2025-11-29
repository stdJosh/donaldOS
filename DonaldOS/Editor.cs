using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DonaldOS
{
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

        //ab dem wie vielten 80. ? -> 0=0,1=80,2=160
        int topbegin = 0;

        int n = 0;

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
            //gibt an wie viele zeilen gefüllt sind
            int i = 0;
            int max_begrenzer = 0;

            //0-22(23)  (2 sind Status-Zeilen)
            for (n = 0 + offset; n < rows.Count && n < offset + 23 - max_begrenzer; n++)
            {

                if (i == 0 && (((rows[n].Length - (topbegin * width)) - 1) / width) + 1 > 23 - i)
                {
                    int remaining = 23 - i;
                    int maxChars = remaining * width;
                    int take = Math.Min(rows[n].Length - (topbegin * width), maxChars);
                    System.Console.WriteLine(rows[n].Substring(topbegin * width, take));

                    n = rows.Count; // Rest skippen
                    i = 23;
                    continue;
                }

                //prüfen ob der string zu lang wäre
                if (((rows[n].Length - 1) / width) + 1 > 23 - i)
                {
                    int remaining = 23 - i;
                    int maxChars = remaining * width;
                    int take = Math.Min(rows[n].Length, maxChars);

                    System.Console.WriteLine(rows[n].Substring(0, take));

                    n = rows.Count; // Rest skippen
                    i = 23;
                    continue;
                }

                //muss testen ob die row abgeschnitten wird und somit das y beeinflusst (alle vorher)
                if ((lang = rows[n].Length) > width && (cursorY + extra) - offset > i)
                {
                    extra += ((lang - 1) / width);
                }

                //der akktuelle string
                if (i == (cursorY + extra) - offset)
                {
                    //extra dazuzeählen wenn nicht in 1. "zeile"
                    extra += (cursorX / width);
                }

                if (i == 0 && topbegin > 0)
                {
                    extra -= topbegin;
                }

                if ((lang = rows[n].Length) > width)
                {
                    if (i == 0)
                    {
                        //den ersten string ab topbeginn ausgeben und auch nur diese länge für begrenzer bewerten
                        System.Console.WriteLine(rows[n].Substring(topbegin * width));
                        max_begrenzer += (((lang - (topbegin * width)) - 1) / width);
                        i += (((lang - (topbegin * width))) / width) + 1;
                        continue;
                    }
                    else
                    {
                        max_begrenzer += ((lang - 1) / width);
                    }
                }

                System.Console.WriteLine(rows[n]);
                i += ((rows[n].Length) / width) + 1;
            }

            //die 23 zeilen voll machen 
            while (i < 23)
            {
                System.Console.WriteLine("");
                i++;
            }

            System.Console.WriteLine("-------------------------------------------------------------------------------");
            System.Console.Write("Commands: Ctrl+S = Save || Ctrl+X = End       Saved:");
            //System.Console.Write("cursorX:" + cursorX + " cursorY:"+cursorY + " offset:"+offset+ " extra:"+extra);
            if (saved)
            {
                System.Console.Write(" Yes");
            }
            else
            {
                System.Console.Write(" No");
            }

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
                            //in view ganz oben aber x nicht 0 aber ganz links  
                            if ((cursorY + extra) - offset == 0 && cursorX % width == 0 && topbegin > 0)
                            {
                                topbegin--;
                            }

                            cursorX--;
                        }
                        else if (cursorY > 0)
                        {
                            cursorX = rows[cursorY - 1].Length;

                            if ((cursorY + extra) - offset == 0)
                            {
                                if (topbegin == 0 && rows[offset - 1].Length > width)
                                {
                                    topbegin = (rows[offset - 1].Length - 1) / width;
                                    offset--;
                                }
                                else
                                {
                                    offset--;
                                }
                            }
                            cursorY--;
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (cursorX < rows[cursorY].Length)
                        {
                            //wenn am ende einer zeile und mit cursorY ganz unten am bildschirm
                            if (cursorX % width == 0 && (cursorY + extra) - offset >= 23)
                            {
                                if ((rows[offset].Length - 1) / width > topbegin)
                                {
                                    topbegin++;
                                }
                                else
                                {
                                    topbegin = 0;
                                    offset++;
                                }
                            }
                            cursorX++;
                        }
                        else if (cursorY + 1 < rows.Count)
                        {
                            //scrollen
                            if ((cursorY + extra) - offset >= 22)
                            {
                                if ((rows[offset].Length - 1) / width > topbegin)
                                {
                                    topbegin++;
                                }
                                else
                                {
                                    topbegin = 0;
                                    offset++;
                                }
                            }
                            cursorY++;
                            cursorX = 0;
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

                        //nicht in 1. zeile des Strings
                        if (line.Length > width && cursorXrow2 > 1)
                        {
                            // Noch innerhalb der gleichen logischen Zeile -> eine physische Zeile hoch
                            cursorX -= width;
                            if (cursorX < 0)
                                cursorX = 0;

                            //oberste zeile ist nicht die 1. zeile eines strings
                            if ((cursorY + extra) - offset == 0 && topbegin != 0)
                            {
                                topbegin--;
                            }
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
                                    // gleiche vertikale Position beibehalten, aber eine Zeile höher landen
                                    cursorX = (prevlinerows - 1) * width + (cursorX % width);
                                }


                                // Falls die vorherige Zeile kürzer ist, Cursor ans Ende
                                if (cursorX > prevLine.Length)
                                    cursorX = prevLine.Length;

                                if ((cursorY + extra) - offset == 0)
                                {
                                    if (topbegin == 0 && rows[offset - 1].Length > width)
                                    {
                                        topbegin = (rows[offset - 1].Length - 1) / width;
                                        offset--;
                                    }
                                    else
                                    {
                                        offset--;
                                    }
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
                                cursorX += width;
                                //wenn die zeile darunter kürzer ist 
                                if (cursorX > currentLine.Length) cursorX = currentLine.Length;

                                if ((cursorY + extra) - offset >= 22)
                                {

                                    if ((rows[offset].Length - 1) / width > topbegin)
                                    {
                                        topbegin++;
                                    }
                                    else
                                    {
                                        topbegin = 0;
                                        offset++;
                                    }
                                }
                            }
                            //das gleiche wie wenn es keine mehrere reihen geben würde
                            else
                            {
                                // es gibt noch eine -> nächste Zeile
                                if (cursorY < rows.Count - 1)
                                {

                                    if ((cursorY + extra) - offset >= 22)
                                    {

                                        if ((rows[offset].Length - 1) / width > topbegin)
                                        {
                                            topbegin++;
                                        }
                                        else
                                        {
                                            topbegin = 0;
                                            offset++;
                                        }
                                    }
                                    cursorY++;
                                    //falls es vorher mehrere zeilen waren wäre cursor X zu hoch
                                    cursorX = cursorX % width;
                                    currentLine = rows[cursorY];
                                    if (cursorX > currentLine.Length) cursorX = currentLine.Length;
                                }
                            }
                        }
                        else
                        {
                            // es gibt noch eine -> nächste Zeile
                            if (cursorY < rows.Count - 1)
                            {

                                if ((cursorY + extra) - offset >= 22)
                                {

                                    if ((rows[offset].Length - 1) / width > topbegin)
                                    {
                                        topbegin++;
                                    }
                                    else
                                    {
                                        topbegin = 0;
                                        offset++;
                                    }
                                }
                                cursorY++;
                                //falls es vorher mehrere zeilen waren wäre cursor X zu hoch
                                cursorX = cursorX % width;
                                currentLine = rows[cursorY];
                                if (cursorX > currentLine.Length) cursorX = currentLine.Length;
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

                            if ((cursorY + extra) - offset == 0)
                            {
                                if (topbegin == 0 && rows[offset - 1].Length > width)
                                {
                                    topbegin = (rows[offset - 1].Length - 1) / width;
                                    offset--;
                                }
                                else
                                {
                                    offset--;
                                }
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


                        if ((cursorY + extra) - offset >= 22)
                        {
                            if ((rows[offset].Length - 1) / width > topbegin)
                            {
                                topbegin++;
                            }
                            else
                            {
                                topbegin = 0;
                                offset++;
                            }
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