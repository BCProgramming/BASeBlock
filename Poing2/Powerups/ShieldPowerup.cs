using System.Drawing;
using BASeCamp.BASeBlock.Blocks;

namespace BASeCamp.BASeBlock.Powerups
{
    public class ShieldPowerup : GamePowerUp
    {
        public static float PowerupChance()
        {

            return 1f;
        }
        public bool ShieldCallback(BCBlockGameState gamestate)
        {

            StrongBlock addthis = new StrongBlock(new RectangleF(0, gamestate.GameArea.Bottom - 32, gamestate.GameArea.Width, 32));
            addthis.Strength = 3;
            
            gamestate.Blocks.AddLast(addthis);
            gamestate.Forcerefresh = true;
            AddScore(gamestate, 40);
            return true;
        }

        public ShieldPowerup(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize, new Image[] { BCBlockGameState.Imageman.getLoadedImage("shieldpowerup") }, 10, null)
        {
            usefunction = ShieldCallback;


        }

    }
}