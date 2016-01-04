using System;
using System.Drawing;
using System.Runtime.Serialization;
using BASeCamp.BASeBlock.Particles;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    [BBEditorVisible]
    [StandardBlockCategory]
    [BlockDescription("Breaks into several pieces, which fall to and off the level.")]
    public class BrickBlock : TexturedBlock,IGameInitializer 
    {

        static short[][] BrickMask = new short[][]{
                                                      new short[]{0,0,0,0,0,0,0,1},
                                                      new short[]{0,0,0,0,0,0,0,1},
                                                      new short[]{0,0,0,0,0,0,0,1},
                                                      new short[]{1,1,1,1,1,1,1,1},
                                                      new short[]{0,0,0,1,0,0,0,0},
                                                      new short[]{0,0,0,1,0,0,0,0},
                                                      new short[]{0,0,0,1,0,0,0,0},
                                                      new short[]{1,1,1,1,1,1,1,1}
                                                  };
        public static void GameInitialize(iManagerCallback datahook)
        {
            //add a default image.
            BCBlockGameState.Imageman.AddImage("SMBBRICK", ImageManager.CreateImageFromMatrix(BrickMask, (a, x, y) => a == 1 ? Color.Black : Color.Red));



        }

        public BrickBlock(RectangleF blockrect, String textureid)
            : base(blockrect, textureid)
        {


        }
        
        public BrickBlock(RectangleF blockrect)
            : this(blockrect, "SMBBRICK")
        {

        }
        public BrickBlock(BrickBlock clonethis)
            : base(clonethis)
        {


        }
        public BrickBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {


        }
        public override object Clone()
        {
            return new BrickBlock(this);
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            BCBlockGameState.Soundman.PlaySound("brickbreak");
            AddScore(parentstate, 50);
            base.PerformBlockHit(parentstate, ballhit);
            //StandardSpray(parentstate, ballhit);
            //bool nodef;
            //OnBlockHit(this, parentstate, ballhit, ref ballsadded, ref nodef);
            RandomSpawnPowerup(parentstate);
            return true;
        }
        protected override void StandardSpray(BCBlockGameState parentstate, cBall ballhit)
        {
            PointF passvel = new PointF(0, 3);
            if (ballhit != null) passvel = ballhit.Velocity;
            parentstate.Particles.AddRange(BrickDebris.GenerateQuadBricks(this, passvel));


            //base.StandardSpray(parentstate, ballhit);
        }


    }
}