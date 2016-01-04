using System;
using System.Drawing;
using System.Linq;
using BASeCamp.BASeBlock.PaddleBehaviours;

namespace BASeCamp.BASeBlock.Powerups
{
    public class BuilderPowerUp : PaddlePowerUp<PaddleBehaviours.BuilderShotBehaviour>
    {
        public static float PowerupChance()
        {
            return 1f;

        }
        public BuilderPowerUp(PointF Location, SizeF ObjectSize):base(Location,ObjectSize)
        {
            mScoreValue = 140;

        }
        public override Image[] GetPowerUpImages()
        {
            return new Image[]{BCBlockGameState.Imageman.getLoadedImage("BUILDPWR")};
        }
        protected override bool SupportsMulti()
        {
            return true;
        }
        protected override void ApplyPower(BCBlockGameState gamestate)
        {
            gamestate.PlayerPaddle.Energy = Math.Max(gamestate.PlayerPaddle.Energy, 100);
            BCBlockGameState.Soundman.PlaySound("sgcock");
            BuilderShotBehaviour Buildbehave = (gamestate.PlayerPaddle.Behaviours.First((w) => w.GetType() == typeof(BuilderShotBehaviour))) as BuilderShotBehaviour;
            Buildbehave.PowerLevel++;
            string desc = Buildbehave.GetDescription();
            AddScore(gamestate, 20 + Buildbehave.PowerLevel, " Power up! (" + desc + ")");
            
        }
    }
}