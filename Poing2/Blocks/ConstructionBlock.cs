using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.Elementizer;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    [ImpactEffectBlockCategory]
    [BlockDescription("'Explodes', Creating blocks within a given radius.")]
    public class ConstructionBlock : ImageBlock
    {

        private Type CreateBlockType = typeof(NormalBlock);

        private String _CreationType;
        [Editor(typeof(BlockTypeStringEditor), typeof(UITypeEditor))]
        public String CreationType
        {
            get { return _CreationType; }
            set
            {
                _CreationType = value;
                retrievetype();

            }
        }
        private void retrievetype()
        {
            CreateBlockType = BCBlockGameState.FindClass(_CreationType);

        }

        private float _EffectRadius;
        /// <summary>
        /// The Radius of the effect of this block.
        /// </summary>
        //        [TypeConverter(typeof(FloatFConverter))]
        public float EffectRadius { get { return _EffectRadius; } set { _EffectRadius = value; } }

        public ConstructionBlock(RectangleF BlockRectangle, String imagekey)
            : base(BlockRectangle, imagekey)
        {



        }
        public ConstructionBlock(RectangleF BlockRectangle)
            : this(BlockRectangle, "Construct")
        {

        }

        public ConstructionBlock(ConstructionBlock reb)
            : base(reb)
        {
            _EffectRadius = reb.EffectRadius;


        }
        public ConstructionBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _EffectRadius = info.GetSingle("EffectRadius");


        }

        public override object Clone()
        {
            return new ConstructionBlock(this);

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("EffectRadius", _EffectRadius);
        }

        public override XElement GetXmlData(string pNodeName)
        {
            var result =  base.GetXmlData(pNodeName);
            result.Add(new XAttribute("EffectRadius",_EffectRadius));
            return result;
        }
        public ConstructionBlock(XElement Source):base(Source)
        {
            _EffectRadius = Source.GetAttributeFloat("EffectRadius", 64);
        }
        protected Queue<List<Block>> BlocksAdd = new Queue<List<Block>>();
        public Block ConstructionRoutine(BCBlockGameState gstate, RectangleF creationSpot)
        {

            return (Block)Activator.CreateInstance(CreateBlockType, new Object[] { creationSpot });


        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {


            /*
            //return base.PerformBlockHit(parentstate, ballhit, ref ballsadded);
            Debug.Print("ConstructionBlock::PerformBlockHit");
            List<Block> addblocks = new List<Block>();
            for (float createX = BlockRectangle.Left - EffectRadius.X; createX < BlockRectangle.Right + EffectRadius.X; createX += BlockRectangle.Width)
            {
                for (float createY = BlockRectangle.Top - EffectRadius.Y; createY < BlockRectangle.Bottom + EffectRadius.Y; createY += BlockRectangle.Height)
                {
                    //create the block.
                    RectangleF createrect = new RectangleF(createX, createY, BlockSize.Width, BlockSize.Height);
                    Block createdBlock = Activator.CreateInstance(CreateBlockType, new Object[] { createrect }) as Block;
                    //add to game.
                    //do so by adding a Proxy Game Object that will add the blocks.
                    addblocks.Add(createdBlock);



                }
            }
            //finished, create the proxy game object. We need to do this because the Block's PerformBlockHit() Routine is doubtless within a Enumeration of the Blocks in the level,
            //so adding them will cause that loop to break out. We instead defer that by adding a Proxy Object.
            BlocksAdd.Enqueue(addblocks);
            ProxyObject proxobject = new ProxyObject(ProxyPerformFrame, null);
            parentstate.GameObjects.AddLast(proxobject);
            return true;
             * */
            CreationEffect creationeffectobj = new CreationEffect(BlockSize, CenterPoint(), _EffectRadius);
            creationeffectobj.BlockCreateRoutine = ConstructionRoutine;
            parentstate.GameObjects.AddLast(creationeffectobj);
            return true;
        }

        public bool ProxyPerformFrame(BCBlockGameState gamestate, ref List<GameObject> AddObjects, ref List<GameObject> removeobjects)
        {
            while (BlocksAdd.Any())
            {
                var addlist = BlocksAdd.Dequeue();
                gamestate.Blocks.AddRangeAfter(addlist);
                gamestate.Forcerefresh = true;

            }
            return true;

        }





    }
}