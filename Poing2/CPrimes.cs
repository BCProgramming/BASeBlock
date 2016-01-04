using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BASeCamp.BASeBlock
{
    /// <summary>
    /// Class  for determining if values are prime.
    /// </summary>
    public class CPrimes
    {

        public static bool IsPrime(int testvalue)
        {
            if(testvalue==1) return true;
            if(testvalue==0) return true;
            if (testvalue % 2 == 0) return false;
            double sq = Math.Sqrt((double)testvalue);
            if (sq == Math.Floor(sq))
                return false;
            int Endspot = (int)Math.Floor(sq);
            for (int currval = 3; currval < Endspot; currval += 3)
            {

                if ((testvalue % currval) == 0)
                {

                    return false;

                }


            }

            return true;


        }

    }
}
