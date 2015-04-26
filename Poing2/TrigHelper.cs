using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BASeBlock
{
    public static class TrigHelper
    {
        private static double[] sinTable = new double[359];
        private static double[] cosTable = new double[359];
        private static bool sinTableInitialized = false;
        private static bool cosTableInitialized = false;
        public static double Sin(double input)
        {
            if(!sinTableInitialized)
            {//build the Sine Table.
                for (int i = 0; i < 360; i++)
                {
                    sinTable[i] = Math.Sin((double)i);
                }
            
            }
            return sinTable[(int)(Math.Floor(Rad2Deg(input)))];

        }
        public static double Cos(double input)
        {
            if (!cosTableInitialized)
            {//build the Sine Table.
                for (int i = 0; i < 360; i++)
                {
                    cosTable[i] = Math.Sin((double)i);
                }

            }
            return cosTable[(int)(Math.Floor(Rad2Deg(input)))];

        }
        public static double Rad2Deg(double angle)
        {
            return Math.PI / (angle / 180.0);
        }
        public static double Deg2Rad(double angle)
        {
            return Math.PI * angle / 180.0;
        }

    }
}
