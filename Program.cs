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
            Console.WriteLine("Optimální souřadnice: " + lake.GetPosition(out count) );
            Console.WriteLine("Výskyt ryb ve výběru: " + count);

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

            for (int height = 0; height < Fields.GetLength(0); height++)
            {
                for (int width = 0; width < Fields.GetLength(1); width++)
                {
                    for (int y = height; y < netSize; y++)
                    {
                        for (int x = width; x < netSize; x++)
                        {
                            if (Fields[x, y]) count++;
                        }
                    }
                    vyskyty.Add(new Point(width, height), count);
                    count = 0;
                } 
            }

            pocet = vyskyty.Values.Max();

            Point positionOfMax = vyskyty.FirstOrDefault(x => x.Value == vyskyty.Values.Max()).Key;

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
    }
}
