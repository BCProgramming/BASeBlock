using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BASeBlock.Powerups
{
    public class EnemySpawnerPowerUp : GamePowerUp
    {
        public Type[] spawntypes = new Type[] { typeof(EyeGuy),typeof(SpinnerGuy),typeof(BouncerGuy) }; 
        public float[] spawnchance = new float[] { 1,1 }; //equal chance for either kind.
        public float Spawnchancesum = 0;
        public static float PowerupChance()
        {

            return 1f;
        }
        private int RandomValueToIndex(float randvalue, IEnumerable<float> chancearray)
        {
            float currentaccum = 0;
            float prevaccum = 0;
            int i = 0;
            foreach (float loopvalue in chancearray)
            {

                currentaccum += loopvalue;
                if (currentaccum > randvalue)
                    return i;

                i++;


            }

            return i;


        }
        public bool PowerUpCallback(BCBlockGameState gamestate)
        {

            float randgen = (float)BCBlockGameState.rgen.NextDouble() * Spawnchancesum;
            int useindex = RandomValueToIndex(randgen, spawnchance);
            Type grabtype = spawntypes[useindex];

            SizeF useSize = new SizeF(16, 16);
            PointF useposition =
                new PointF(gamestate.GameArea.Width * (float)BCBlockGameState.rgen.NextDouble(), (gamestate.GameArea.Height - 72) * (float)BCBlockGameState.rgen.NextDouble());


            GameEnemy genenemy = (GameEnemy)Activator.CreateInstance(grabtype, useposition, useSize);
            gamestate.Defer(() =>
                                {
                                    gamestate.GameObjects.AddLast(genenemy);

                                    AddScore(gamestate, 10);
                                });
            return true;
        }
        public EnemySpawnerPowerUp(PointF Location, SizeF ObjectSize)



            : base(Location, ObjectSize, new Image[] { BCBlockGameState.Imageman.getLoadedImage("enemyspawn") }, 1, null)
        {
            usefunction = PowerUpCallback;

            Spawnchancesum = spawnchance.Sum();

        }


    }
}