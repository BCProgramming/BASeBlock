using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.BASeBlock.Particles;
using BASeCamp.Elementizer;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable()]
    [StandardBlockCategory]
    [BlockDescription("Your standard, everyday, boring block. Breaks quickly. Set <i>gummy</i> to false to make it more boring.")]
    public class NormalBlock : Block
    {
        private bool _Gummy = true;
        public bool Gummy { get { return _Gummy; } set { _Gummy = value; } }
        private Image UseGummyImage = null;
        [NonSerialized]
        private Brush mBlockBrush = new SolidBrush(Color.Blue);
        [NonSerialized]
        private Pen mBlockPen = new Pen(new SolidBrush(Color.Gray), 2);

        public Brush BlockBrush
        {
            get { return mBlockBrush; }
            set
            {
                mBlockBrush = value;
                hasChanged = true;
            }


        }
        public Pen BlockPen
        {
            get
            {
                return mBlockPen;
            }
            set { mBlockPen = value; hasChanged = true; }




        }
        [Flags]
        public enum BlockOutline
        {
            Outline_None,
            Outline_Left=2,
            Outline_Right=4,
            Outline_Top=8,
            Outline_Bottom=16,
            Outline_All = Outline_Left | Outline_Right | Outline_Top | Outline_Bottom
        }
        private BlockOutline _BlockOutlineSides = BlockOutline.Outline_All;


        public BlockOutline BlockOutlineSides { get { return _BlockOutlineSides; } set { _BlockOutlineSides = value; } }
            
        private Color _Color;
        private Color _PenColor;

        public Color BlockColor
        {
            get { return _Color; }
            set
            {
                _Color = value;
                mBlockBrush = new SolidBrush(_Color);
                //only retrieve gummy image if INI file is not set to OldNormalBlock...
                //if(!BCBlockGameState.GameSettings["game"]["OldNormalBlock"].GetValue(false))
                UseGummyImage = BCBlockGameState.GetGummyImage(_Color,this.BlockSize.ToSize());

            }
        }
        public Color PenColor
        {
            get { return _PenColor; }
            set
            {
                _PenColor = value;
                mBlockPen = new Pen(_PenColor);
            }


        }
        
        public NormalBlock()
            : this(new RectangleF(0, 0, 32, 16))
        {
            
        }
        private Brush createBlockBrush()
        {

            PathGradientBrush pgb = new PathGradientBrush(BlockRectangle.Corners(), WrapMode.Clamp);
            pgb.CenterColor = BlockColor;
            pgb.CenterPoint = CenterPoint();

            return pgb;
        }
        public NormalBlock(RectangleF pBlockRectangle)
        {
            BlockRectangle = pBlockRectangle;
            Random qgen = BCBlockGameState.rgen;
            if (qgen == null) qgen = new Random();
            BlockColor = Color.FromArgb(qgen.Next(255), qgen.Next(255), qgen.Next(255));
            //BlockBrush = new SolidBrush(gotcolor);
            PenColor = Color.Black;
            //BlockPen = new Pen (Color.Black,2);   
            OnBlockRectangleChange += new Action<RectangleF>(NormalBlock_OnBlockRectangleChange);
        }
        public NormalBlock(RectangleF pBlockRectangle, Brush pBlockBrush, Pen pBlockPen)
            : this(pBlockRectangle)
        {
            BlockBrush = pBlockBrush;
            BlockPen = pBlockPen;

        }
        protected NormalBlock(NormalBlock makecloneof)
            : base(makecloneof)
        {
            mBlockBrush = (Brush)makecloneof.BlockBrush.Clone();
            mBlockPen = (Pen)makecloneof.BlockPen.Clone();
            BlockColor = makecloneof.BlockColor;
            PenColor = makecloneof.PenColor;
            //base.BlockRectangle = makecloneof.BlockRectangle;
            //TriggerID = makecloneof.TriggerID;
            OnBlockRectangleChange += new Action<RectangleF>(NormalBlock_OnBlockRectangleChange);


        }

        void NormalBlock_OnBlockRectangleChange(RectangleF obj)
        {
            UseGummyImage = BCBlockGameState.GetGummyImage(_Color, this.BlockSize.ToSize());
        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            XElement baseNode = base.GetXmlData(pNodeName,pPersistenceData);
            baseNode.Add(StandardHelper.SaveElement(_Color,"Fill",pPersistenceData));
            baseNode.Add(StandardHelper.SaveElement(_PenColor, "Line",pPersistenceData));
            baseNode.Add(new XAttribute("BlockOutline", (int)BlockOutlineSides));
            baseNode.Add(new XAttribute("Gummy",Gummy));
            return baseNode;
        }

        public NormalBlock(XElement Source,Object pPersistenceData) :base(Source,pPersistenceData)
        {
            
            _PenColor = StandardHelper.ReadElement<Color>(Source.Element("Line"),pPersistenceData);
            BlockOutlineSides = (BlockOutline)Source.GetAttributeInt("BlockOutline", (int)BlockOutline.Outline_All);
            Gummy = Source.GetAttributeBool("Gummy", true);
            BlockColor = StandardHelper.ReadElement<Color>(Source.Element("Fill"),pPersistenceData);
            OnBlockRectangleChange += new Action<RectangleF>(NormalBlock_OnBlockRectangleChange);
        }
        public NormalBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            BlockColor = (Color)info.GetValue("Fill", typeof(Color));
            Debug.Print("Read Colour with Alpha=" + _Color.A);
            PenColor = (Color)info.GetValue("Line", typeof(Color));
            try { BlockOutlineSides = (BlockOutline)info.GetValue("BlockOutline", typeof(BlockOutline)); }
            catch { }
            try { _Gummy = info.GetBoolean("Gummy"); }catch{}
            OnBlockRectangleChange += new Action<RectangleF>(NormalBlock_OnBlockRectangleChange);
        }
        protected override Particle AddStandardSprayParticle(BCBlockGameState parentstate, cBall ballhit)
        {
            // Debug.Print("AddStandardSprayParticle");
            PointF middlespot;
            if (ballhit != null)
                middlespot = ballhit.Location;
            else
            {
                middlespot = CenterPoint();
            }
            PolyDebris debris = null;
            if (UseGummyImage == null)
                debris = ballhit != null ? new PolyDebris(ballhit) : new PolyDebris(middlespot, new PointF(0, 0), ((NormalBlock)this).BlockColor);
            else
                debris = ballhit != null ? new PolyDebris(ballhit, UseGummyImage) : new PolyDebris(middlespot, new PointF(0, 0), UseGummyImage);


            // debris.DrawBrush = new SolidBrush(((NormalBlock)this).BlockColor);
            debris.PenColor = ((NormalBlock)this).PenColor;
            debris.BrushColor = ((NormalBlock)this).BlockColor;
            return debris;

        }
        //public Brush BlockBrush =
        public override bool MustDestroy()
        {
            return true;
        }
        public override void Draw(Graphics g)
        {
            base.Draw(g);

           
            g.DrawImage(UseGummyImage, BlockRectangle);

            if (BlockOutlineSides == BlockOutline.Outline_All)
            {
                g.DrawRectangle(BlockPen, BlockRectangle.Left, BlockRectangle.Top, BlockRectangle.Width, BlockRectangle.Height);

            }
            else
            {
                if ((BlockOutlineSides | BlockOutline.Outline_Left) == BlockOutline.Outline_Left)
                {
                    g.DrawLine(BlockPen, BlockRectangle.Left, BlockRectangle.Top, BlockRectangle.Left, BlockRectangle.Bottom);

                }
                if ((BlockOutlineSides | BlockOutline.Outline_Right) == BlockOutline.Outline_Right)
                {
                    g.DrawLine(BlockPen, BlockRectangle.Right, BlockRectangle.Top, BlockRectangle.Right, BlockRectangle.Bottom);

                }
                if ((BlockOutlineSides | BlockOutline.Outline_Top) == BlockOutline.Outline_Top)
                {
                    g.DrawLine(BlockPen, BlockRectangle.Left, BlockRectangle.Top, BlockRectangle.Right, BlockRectangle.Top);

                }
                if ((BlockOutlineSides | BlockOutline.Outline_Bottom) == BlockOutline.Outline_Bottom)
                {
                    g.DrawLine(BlockPen, BlockRectangle.Left, BlockRectangle.Bottom, BlockRectangle.Right, BlockRectangle.Bottom);

                }
            }



            
        }


        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            
            try
            {
                //BCBlockGameState.Soundman.PlaySound("BBOUNCE",1.0f);
                bool nodef = false;
                Block.PlayDefaultSound(ballhit);
                base.PerformBlockHit(parentstate, ballhit);
                base.RaiseBlockDestroy(parentstate, ballhit, ref nodef);
                AddScore(parentstate, 30);

                //temporary: test of the powerups:
                //parentstate.GameObjects.AddLast(new GamePowerUp(CenterPoint(), new Size(24, 12), BCBlockGameState.Imageman.getLoadedImage("paddlePlus"), PaddlePlusImpact));
                //Level gotlevel=parentstate.ClientObject.getcurrentLevel();
                RandomSpawnPowerup(parentstate);
            }
            catch
            {
                return true;
            }
            Debug.Print("NormalBlock returning true for PerformBlockHit()");
            return true;
        }



        public override object Clone()
        {
            //create a new NormalBlock...
            return new NormalBlock(this);
        }

        #region ISerializable Members

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //we need to manually persist the brush and pen data, unfortunately...

            base.GetObjectData(info, context);
            
            Debug.Print("Saving colour, Alpha=" + _Color.A);
            info.AddValue("Fill", _Color);
            info.AddValue("Line", _PenColor);
            info.AddValue("BlockOutline", BlockOutlineSides);
            info.AddValue("Gummy", Gummy);

        }

        #endregion
    }
}