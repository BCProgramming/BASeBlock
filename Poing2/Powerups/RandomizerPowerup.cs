using System;
using System.Drawing;
using BASeBlock.Particles;

namespace BASeBlock.Powerups
{
    public class RandomizerPowerup : GamePowerUp
    {
        static Color[] chosencolors = new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };
        public bool Callback(BCBlockGameState gamestate)
        {
            //iterate through all balls...
            BCBlockGameState.Soundman.PlaySound("RANDOMY");
            
            foreach (cBall iterateball in gamestate.Balls)
            {
                var gotspeed = iterateball.getMagnitude();
                //set a new velocity with that speed.
                iterateball.Velocity = BCBlockGameState.GetRandomVelocity(gotspeed);
                //add some randomly coloured lightorbs...
                foreach (Color addcolor in chosencolors)
                {
                    LightOrb lo = new LightOrb(iterateball.Location, addcolor, 17);
                    PointF usevelocity = BCBlockGameState.GetRandomVelocity(2, 5, BCBlockGameState.GetAngle(new PointF(0, 0), iterateball.Velocity));
                    BCBlockGameState.VaryVelocity(usevelocity, Math.PI / 4);
                    lo.Velocity = usevelocity;
                    gamestate.Particles.Add(lo);

                }

                

            }

            return true;
        }
        public static float PowerupChance()
        {
            return 1f;
        }
        public RandomizerPowerup(PointF pLocation, SizeF psize)
            : base(pLocation, psize, new Image[] { BCBlockGameState.Imageman.getLoadedImage("COLOURIZE(128,255,55,255):RANDOMIZER") }, 10, null)
        {

            usefunction = Callback;
        }

    }
}