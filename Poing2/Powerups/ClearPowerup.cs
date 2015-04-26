using System.Drawing;
using BASeBlock.Particles;

namespace BASeBlock.Powerups
{
    public class ClearPowerup : GamePowerUp
    {
        private bool clearpowercallback(BCBlockGameState gamestate)
        {
            BCBlockGameState.Soundman.PlaySound("deflectoron");
            BCBlockGameState.Soundman.PlaySound("deflectoroff");
            if (gamestate.PlayerPaddle != null)
                gamestate.PlayerPaddle.Behaviours.Clear();
            return true;
        }
        public ClearPowerup(Point Location,SizeF ObjectSize)
            :base(Location,ObjectSize,new Image[]{BCBlockGameState.Imageman.getLoadedImage("powerclear")},10,null)
        {
            usefunction = clearpowercallback;
        }
        int frameoffset = 0;
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            if (frameoffset++ > 5)
            { 
                frameoffset = 0;
                gamestate.Defer(() => gamestate.Particles.Add(new LightOrb(Location, Color.Red, this.Size.Width)));
            }
            return base.PerformFrame(gamestate);
        }
    }
}