using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.Elementizer;

namespace BASeCamp.BASeBlock.Blocks
{
    /// <summary>
    /// BlockShotBlock, shoots a BoxDestructor when impacted in the given direction.
    /// </summary>
    [Serializable]
    [ImpactEffectBlockCategory]
    [BlockDescription("Shoots off in a given direction when hit by a ball.")]
    public class BlockShotBlock : ImageBlock
    {
        private static readonly PointF DefaultShotVelocity = new PointF(0, -10);
        private PointF _ShotVelocity = DefaultShotVelocity;

        public PointF ShotVelocity
        {
            get { return _ShotVelocity; }
            set { _ShotVelocity = value; }
        }

        public BlockShotBlock(BlockShotBlock clonethis):base(clonethis)
        {
            _ShotVelocity = clonethis._ShotVelocity;

        }
        public override object Clone()
        {
            return new BlockShotBlock(this);
        }
        public BlockShotBlock(RectangleF BlockRect, String Imagekey)
            : this(BlockRect, Imagekey,DefaultShotVelocity)
        {


        }
        public BlockShotBlock(RectangleF BlockRect, String Imagekey, PointF pShotDirection):base(BlockRect,Imagekey)
        {
            _ShotVelocity = pShotDirection;
        }

        public BlockShotBlock(RectangleF BlockRect)
            : this(BlockRect, "BlockDestructor")
        {

        }
        public BlockShotBlock(XElement Source,Object pPersistenceData):base(Source, pPersistenceData)
        {
            _ShotVelocity = Source.ReadElement<PointF>("ShotVelocity", PointF.Empty);
        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            var Result = base.GetXmlData(pNodeName,pPersistenceData);
            Result.Add(StandardHelper.SaveElement(_ShotVelocity,"ShotVelocity",pPersistenceData ));
            return Result;
        }

        #region ISerializable stuff
        public BlockShotBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            _ShotVelocity = (PointF)info.GetValue("ShotVelocity", typeof(PointF));

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ShotVelocity", _ShotVelocity);

        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            //spawn a new BoxDestructor.
            
            PointF useaccel = new PointF(_ShotVelocity.X / 100, _ShotVelocity.Y / 100);
            BoxDestructor bd = new BoxDestructor(this, _ShotVelocity);
            //add it to the game.
            parentstate.GameObjects.AddLast(bd);



            return base.PerformBlockHit(parentstate, ballhit);
        }
        #endregion
    }
}