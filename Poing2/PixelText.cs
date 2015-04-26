using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BASeBlock
{
    /// <summary>
    /// PixelText: provides methods and data for drawing text using a custom pixel grid.
    /// Or, rather, it -will- provide those methods and data....
    /// </summary>
    class PixelText
    {


        private static Dictionary<char, int[][]> InitializeCharacterData()
        {
            Dictionary<char, int[][]> cd = new Dictionary<char, int[][]>();



            //pixel grid is 8x8.

            //space

            cd.Add(' ', new int[][] {
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,0,0,0,0,0,0,0},
             
            });
            //numbers

            cd.Add('0',new int[][] {
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,1,1,1,0,0,0,0},
             new int[] {1,0,0,0,1,0,0,0},
             new int[] {1,0,0,1,1,0,0,0},
             new int[] {1,0,1,0,1,0,0,0},
             new int[] {1,1,0,0,1,0,0,0},
             new int[] {1,0,0,0,1,0,0,0},
             new int[] {0,1,1,1,0,0,0,0},
             
            });
            cd.Add('1', new int[][] {
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,0,0,1,0,0,0,0},
             new int[] {0,1,1,1,0,0,0,0},
             new int[] {0,0,1,1,0,0,0,0},
             new int[] {0,0,1,1,0,0,0,0},
             new int[] {0,0,1,1,0,0,0,0},
             new int[] {0,0,1,1,0,0,0,0},
             new int[] {1,1,1,1,1,1,0,0},
             
            });

            cd.Add('2', new int[][] {
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,1,1,1,0,0,0,0},
             new int[] {1,0,0,0,1,0,0,0},
             new int[] {0,0,0,0,1,0,0,0},
             new int[] {0,0,1,1,0,0,0,0},
             new int[] {0,1,0,0,0,0,0,0},
             new int[] {1,0,0,0,1,0,0,0},
             new int[] {1,1,1,1,1,0,0,0},
             
            });

            cd.Add('3', new int[][] {
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,1,1,1,0,0,0,0},
             new int[] {1,0,0,0,1,0,0,0},
             new int[] {0,0,0,0,1,0,0,0},
             new int[] {0,0,1,1,0,0,0,0},
             new int[] {0,0,0,0,1,0,0,0},
             new int[] {1,0,0,0,1,0,0,0},
             new int[] {0,1,1,1,0,0,0,0},
             
            });


            cd.Add('4', new int[][] {
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,0,1,1,1,0,0,0},
             new int[] {0,1,0,0,1,0,0,0},
             new int[] {1,0,0,0,1,0,0,0},
             new int[] {1,1,1,1,1,1,0,0},
             new int[] {0,0,0,0,1,0,0,0},
             new int[] {0,0,0,0,1,0,0,0},
             new int[] {0,0,0,0,1,0,0,0},
             
            });
            cd.Add('5', new int[][] {
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {1,1,1,1,1,0,0,0},
             new int[] {1,0,0,0,0,0,0,0},
             new int[] {1,1,1,1,0,0,0,0},
             new int[] {0,0,0,0,1,0,0,0},
             new int[] {0,0,0,0,1,0,0,0},
             new int[] {1,0,0,0,1,0,0,0},
             new int[] {0,1,1,1,0,0,0,0},
             
            });
            cd.Add('6', new int[][] {
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,0,0,0,1,1,0,0},
             new int[] {0,0,0,1,0,0,0,0},
             new int[] {0,0,1,0,0,0,0,0},
             new int[] {0,1,1,1,1,0,0,0},
             new int[] {0,1,0,0,0,1,0,0},
             new int[] {0,1,0,0,0,1,0,0},
             new int[] {0,0,1,1,1,0,0,0},
             
            });
            cd.Add('7', new int[][] {
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {1,1,1,1,1,0,0,0},
             new int[] {1,0,0,0,1,0,0,0},
             new int[] {0,0,0,0,1,0,0,0},
             new int[] {0,1,1,1,1,1,0,0},
             new int[] {0,0,0,1,0,0,0,0},
             new int[] {0,0,1,0,0,0,0,0},
             new int[] {0,0,1,0,0,0,0,0},
             
            });
            cd.Add('8', new int[][] {
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,1,1,1,0,0,0,0},
             new int[] {1,0,0,0,1,0,0,0},
             new int[] {1,0,0,0,1,0,0,0},
             new int[] {0,1,1,1,0,0,0,0},
             new int[] {1,0,0,0,1,0,0,0},
             new int[] {1,0,0,0,1,0,0,0},
             new int[] {0,1,1,1,0,0,0,0},
             
            });

            cd.Add('9', new int[][] {
             new int[] {0,0,0,0,0,0,0,0},
             new int[] {0,0,1,1,0,0,0,0},
             new int[] {0,1,0,0,1,0,0,0},
             new int[] {0,1,0,0,1,0,0,0},
             new int[] {0,0,1,1,1,0,0,0},
             new int[] {0,0,0,0,1,0,0,0},
             new int[] {0,0,0,1,0,0,0,0},
             new int[] {0,1,1,0,0,0,0,0},
             
            });
            return cd;





        }

        private Dictionary<char, int[][]> CharacterData = InitializeCharacterData();
 

        



    }
}
