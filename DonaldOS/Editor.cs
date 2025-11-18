using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaldOS
{
    // todo:
    // markieren und copy paste ? --
    // scrollen können (pfeiltasten)  //
    // zurück                         //
    // wieder vor                     //
    // schreiben / editiren           -/
    // beim nach oben drücken länge beachten -/
    // beim del in einer leeren zeile soll sie gelöscht werden bzw wenn wenn an pos 0 soll der "reststring" nach oben geschoben werden -/
    // view machen (falls zu viele zeilen) //
    // speichern lol //

    //ist scrollbar schon systemweit?
    //vor zurück systemweit? (wahrscheinlich nicht) 
    //copy paste mit makieren schon systemweit ? 

    internal class Editor
    {
        List<string> rows = new List<string>();
        string filename;

        int cursorX = 0;
        int cursorY = 0;
        int width = Console.WindowWidth;
        int extra = 0;

        //kostruktor
        public Editor(string filename)
        {
            this.filename = filename;

        }

        //initialisieren der rows
        public void ReadFile()
        {
            Console.Clear();
            rows = new List<string>(File.ReadAllLines(filename));
        }

        public void PrintFile()
        {
            int lang;
            extra = 0;
            int i = 0;
            foreach (string row in rows)
            {
                //muss testen ob die row abgeschnitten wird und somit das y beeinflusst (alle vorher)
                if((lang = row.Length) > width && cursorY > i)
                {
                    extra += (lang/width);
                }
                //der akktuelle string
                if (i == cursorY)
                {
                    //extra dazuzeählen wenn nicht in 1. "zeile"
                    extra += cursorX / width;
                }
                Console.WriteLine(row);

                i++;
            }
        }

        public void editmode()
        {

            while (true)  
            {
                //todo: nicht immer alles neu zeichnen
                Console.Clear();
                PrintFile();  // Gibt alle logischen Zeilen aus
                Console.SetCursorPosition(cursorX % width, cursorY + extra); // modulo für physische Zeile

                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (cursorX > 0)
                        {
                            cursorX--;
                        }
                        else if (cursorY >0)
                        {
                            cursorY--;
                            cursorX = rows[cursorY].Length;
                        }


                            break; 

                    case ConsoleKey.RightArrow:
                        if (cursorX < rows[cursorY].Length)
                        {
                            cursorX++;
                        }
                        else if (cursorY +1< rows.Count)
                        {
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

                        if (line.Length > width && cursorXrow2 > 1)
                        {
                            // Noch innerhalb der gleichen logischen Zeile -> eine physische Zeile hoch
                            cursorX -= width;
                            if (cursorX < 0)
                                cursorX = 0;
                        }
                        else
                        {
                            // In der obersten physischen Zeile oder normale Zeile -> zur vorherigen logischen Zeile
                            if (cursorY > 0)
                            { 
                                cursorY--;
                                string prevLine = rows[cursorY];

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
                            }
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        string currentLine = rows[cursorY];
                        if (currentLine.Length > width)
                        {
                            //wie viele volle zeilen (+1 bei cursor weil bei 80 (2. zeile) sonst 1ne zeile nur rauskommt (weil ja von 0 bis 79.
                            int cursorXrow = (cursorX +1) / width;
                            //wenn eine unfertige zeile
                            if ((cursorX + 1) %  width != 0) cursorXrow++;

                            int currentlinerow = currentLine.Length / width;
                            if (currentLine.Length % width != 0) currentlinerow++;

                            //wenn es in dem mehrzeilingen string noch eine zeile gibt 
                            if (cursorXrow > currentlinerow)
                            {
                                // Zeile ist länger als Bildschirmbreite 
                                cursorX += width;
                                //wenn die zeile darunter kürzer ist 
                                if (cursorX > currentLine.Length) cursorX = currentLine.Length;
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
                                }
                            }
                        }
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
                            }
                        }
                        break;

                    case ConsoleKey.Backspace:
                        if (cursorX > 0)
                        {
                            //löscht das zeichen bei string[cursorX - 1] 
                            rows[cursorY] = rows[cursorY].Remove(cursorX - 1, 1);
                            cursorX--;
                        }else if (cursorY != 0)
                        {
                            //hier drinne wenn x = 0 und y != 0
                            int oldLen = rows[cursorY-1].Length;

                            rows[cursorY - 1] += rows[cursorY];
                            rows.RemoveAt(cursorY);

                            cursorY--;
                            cursorX = oldLen;
                        }
                            break;

                    case ConsoleKey.Enter:
                        // Neue Zeile einfügen 
                        //zeile teilen rest ist alles ab cursorX
                       // string rest = rows[cursorY].Substring(cursorX);
                        //bis cursor bleibt
                       // rows[cursorY] = rows[cursorY].Substring(0, cursorX);
                        //rest in neue zeile (braucht kein \n)
                       // rows.Insert(cursorY + 1, rest);
                       // cursorY++;
                       // cursorX = 0;


                         
                        string rest = "";
                        if (cursorX < rows[cursorY].Length)
                            rest = rows[cursorY].Substring(cursorX);
                        rows[cursorY] = rows[cursorY].Substring(0, cursorX);

                        if (cursorY + 1 < rows.Count)
                            rows.Insert(cursorY + 1, rest);
                        else
                            rows.Add(rest); // falls du am Ende bist

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
                        break;
                }
            }
        }
    }
}
 