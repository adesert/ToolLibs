using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace imgLib
{
    class Program
    {
        static void Main(string[] args)
        {
            //CropImageManipulator c = new CropImageManipulator();
            //c.Cropping(@"emoji.png", 300, 300);

            CutImageTools t = new CutImageTools();
            t.LayaCutImageByConfigJsonAndAltas();
            Console.WriteLine();
        }
    }
}
