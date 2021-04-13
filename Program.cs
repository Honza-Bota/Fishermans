using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.IO;

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

            Console.WriteLine( lake.Fill(rnd, fishCount) );

            lake.GetMap(fileName);
            lake.GetBitmap(fileName);

            Console.WriteLine( lake.GetPosition() );

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
            int x = rnd.Next(Size);
            int y = rnd.Next(Size);
            Stopwatch s = new Stopwatch();

            s.Start();
            for (int i = 0; i < fishCount; i++)
            {
                while (Fields[x, y] == true)
                {
                    x = rnd.Next(Size);
                    y = rnd.Next(Size);
                }

                Fields[x, y] = true;
            }
            s.Stop();

            Debug.WriteLine("Naplnění pole hodnotami trvalo: " + s.ElapsedMilliseconds + " ms");
            return true;
        }

        public Point GetPosition(int size = 30)
        {
            //kód pro nalezení optimálních souřadnic

            return new Point(0,0);
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
