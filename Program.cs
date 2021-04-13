using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;

namespace Fishermans
{
    class Program
    {
        static Random rnd;
        static Lake lake;
        static int size = 1_000;
        static int fishCount = 100_000;
        static string fileName = "test";

        static void Main(string[] args)
        {
            rnd = new Random();
            lake = new Lake(size);

            Console.WriteLine(lake.Fill(rnd, fishCount) ? "Pole naplněno." : "Pole nenaplněno.");

            lake.GetMap(fileName);
            lake.GetBitmap(fileName);

            int count;
            Point position = lake.GetPosition(out count);
            Console.WriteLine("Optimální souřadnice: " + position );
            Console.WriteLine("Výskyt ryb ve výběru: " + count);
            lake.DrawArea(position, "area");

            Console.ReadLine();
        }
    }

    public class Lake
    {
        public int Size { get; private set; }
        public bool[,] Fields { get; private set; }

        public Lake(int size)
        {
            Size = size;
            Fields = new bool[Size, Size];
        }

        public bool Fill(Random rnd, int fishCount = 100_000)
        {
            Point location = new Point(rnd.Next(Size), rnd.Next(Size));
            Stopwatch s = new Stopwatch();

            s.Start();
            for (int i = 0; i < fishCount; i++)
            {
                while (Fields[location.X, location.Y] == true)
                {
                    location = new Point(rnd.Next(Size), rnd.Next(Size));
                }
                Fields[location.X, location.Y] = true;
            }
            s.Stop();

            Debug.WriteLine("Naplnění pole hodnotami trvalo: " + s.ElapsedMilliseconds + " ms");
            return true;
        }

        public Point GetPosition(out int pocet, int netSize = 30)
        {
            int count = 0;
            Dictionary<Point, int> vyskyty = new Dictionary<Point, int>();
            Stopwatch s = new Stopwatch();

            s.Start();
            for (int height = 0; height < Fields.GetLength(0) - netSize; height++)
            {
                for (int width = 0; width < Fields.GetLength(1) - netSize; width++)
                {
                    for (int i = height; i < netSize + height; i++)
                    {
                        for (int j = width; j < netSize + width; j++)
                        {
                            if (Fields[i, j]) count++;
                        }
                    }
                    vyskyty.Add(new Point(height, width), count);
                    count = 0;
                }
            }
            s.Stop();
            Debug.WriteLine("Čas potřebný pro nalezení ideální polohy: " + s.ElapsedMilliseconds + " ms");

            s.Restart();
            pocet = vyskyty.Values.Max();
            //Point positionOfMax = vyskyty.FirstOrDefault(x => x.Value == vyskyty.Values.Max()).Key;
            Point positionOfMax = vyskyty.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            s.Stop();
            Debug.WriteLine("Čas potřebný pro zjištění ideální polohy: " + s.ElapsedMilliseconds + " ms");

            return positionOfMax;
        }

        public string GetMap(string fileName = null)
        {
            StringBuilder map = new StringBuilder();

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    map.Append(Fields[i, j] ? "1" : "0");
                }
                map.Append(Environment.NewLine);
            }

            if (fileName != null) File.WriteAllText(fileName + ".txt", map.ToString());
            return map.ToString();
        }

        public Bitmap GetBitmap(string fileName = null)
        {
            Bitmap b = new Bitmap(Size, Size);
            
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    b.SetPixel(i, j, Fields[i, j] ? Color.Red : Color.Blue);
                }
            }

            if(fileName != null) b.Save(fileName + ".bmp");
            return b;
        }

        public Bitmap DrawArea(Point position, string fileName = null, int areaSize = 30)
        {
            Bitmap b = GetBitmap();
            for (int i = 0; i <= areaSize; i++)
            {
                //b.SetPixel(position.X + i, position.Y, b.GetPixel(position.X + i, position.Y) == Color.Red ? Color.Red : Color.Green);
                //b.SetPixel(position.X, position.Y + i, b.GetPixel(position.X, position.Y + i) == Color.Red ? Color.Red : Color.Green);
                //b.SetPixel(position.X + i, position.Y + areaSize, b.GetPixel(position.X + i, position.Y + areaSize) == Color.Red ? Color.Red : Color.Green);
                //b.SetPixel(position.X + areaSize, position.Y + i, b.GetPixel(position.X + areaSize, position.Y + i) == Color.Red ? Color.Red : Color.Green);

                b.SetPixel(position.X + i, position.Y, Color.Green);
                b.SetPixel(position.X, position.Y + i, Color.Green);
                b.SetPixel(position.X + i, position.Y + areaSize, Color.Green);
                b.SetPixel(position.X + areaSize, position.Y + i, Color.Green);
            }
        
            if (fileName != null) b.Save(fileName + ".bmp");
            return b;
        }
    }
}
