using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.BASeCamp;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml.Linq;
using BASeCamp.BASeBlock.Events;
using BASeCamp.BASeBlock.GameStates;
using BASeCamp.BASeBlock.Particles;
using BASeCamp.BASeBlock.Powerups;
using BASeCamp.BASeBlock.Projectiles;
using BASeCamp.Elementizer;

//using System.Xml.Serialization;

namespace BASeCamp.BASeBlock.Blocks
{
    


    [Serializable()]
    
    public abstract class AnimatedBlock : Block,IEditorBlockExtensions 
    {
        private String _blocktype = "ImageBlock";
        [Editor(typeof(BlockTypeStringEditor),typeof(UITypeEditor))]
        public String BlockType
        {
            get { return _blocktype; }
            set
            {
                _blocktype = value;
                recreateblock();

            }
        }
        private bool _Frozen = false;
        public bool Frozen { get { return _Frozen; } set { _Frozen = value; } }
        public new List<BlockTrigger> BlockTriggers { get { return baseBlock.BlockTriggers; } set { base.BlockTriggers = value; baseBlock.BlockTriggers = value; } }
        public new List<BlockEvent> BlockEvents { get { return baseBlock.BlockEvents; } set { base.BlockEvents = value; baseBlock.BlockEvents = value; } }
        public new RectangleF BlockRectangle { get { return baseBlock.BlockRectangle; } set { base.BlockRectangle=value; baseBlock.BlockRectangle = value; } }
        public new PointF BlockLocation { get { return baseBlock.BlockLocation; } set { base.BlockLocation = value; baseBlock.BlockLocation = value; } }
        public new SizeF BlockSize { get { return baseBlock.BlockSize; } set { base.BlockSize = value; baseBlock.BlockSize = value; } }
        
        public override PointF CenterPoint()
        {
            return new PointF(BlockRectangle.Left + (BlockRectangle.Width / 2), BlockRectangle.Top + (BlockRectangle.Height / 2));

        }
        internal override bool RaiseBlockDestroy(BCBlockGameState parentstate, cBall ballhit, ref bool nodefault)
        {
            return baseBlock.RaiseBlockDestroy(parentstate, ballhit, ref nodefault) || base.RaiseBlockDestroy(parentstate,ballhit,ref nodefault);
        }
        internal override bool RaiseBlockHit(BCBlockGameState parentstate, cBall ballhit, ref bool nodefault)
        {

            return baseBlock.RaiseBlockHit(parentstate, ballhit, ref nodefault) | base.RaiseBlockHit(parentstate, ballhit, ref nodefault);
        }
        /// <summary>
        /// Recreates our baseBlock based on the value of _blocktype. uses the "default" RectangleF constructor.
        /// </summary>
        
        protected void recreateblock()
        {
            //reinstantiates baseBlock;
            RectangleF useblockrectangle;

            if(_baseBlock!=null)
                useblockrectangle = BlockRectangle;
            else
            {
                useblockrectangle = new RectangleF(0,0,33,16);
            }
            
            

            Type createit = BCBlockGameState.FindClass(_blocktype);
            if (createit == null)
                throw new ArgumentException("blocktype \"" + _blocktype + "\" does not correspond to existing block type.");
            else
            {
                //we have the Type, and all Blocks by definition ought to support the (RectangleF) constructor.
                Block acquirebblock = (Activator.CreateInstance(createit, useblockrectangle) as Block);
                if (acquirebblock != null) baseBlock = acquirebblock;



            }


        }
        private Block _baseBlock = null;
        [Editor(typeof(ObjectTypeEditor), typeof(UITypeEditor))]
        public Block baseBlock
        {
            get
            {
                if (_baseBlock == null)
                {
                    _baseBlock = new NormalBlock(base.BlockRectangle);
                    
                    
                    Trace.WriteLine("Trace: AnimatedBlock has null baseBlock!");
                }
                return _baseBlock;
            }

            set { _baseBlock = value; }
        }
        protected AnimatedBlock()
        {

            OnBlockRectangleChange += new Action<RectangleF>(AnimatedBlock_OnBlockRectangleChange);

        }
      

        protected AnimatedBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            String bblocktype = info.GetString("bblocktype");
            baseBlock = (Block)info.GetValue("baseblock", Type.GetType(bblocktype));
            BlockRectangle = (RectangleF)info.GetValue("BlockRectangle", typeof(RectangleF));
            baseBlock.BlockRectangle = BlockRectangle;
            if(BlockRectangle.Left==0 && BlockRectangle.Top==0) BlockRectangle = baseBlock.BlockRectangle;

            OnBlockRectangleChange += new Action<RectangleF>(AnimatedBlock_OnBlockRectangleChange);


        }
        private bool currhandlingrectchange=false; //flag to prevent recursive event calls.
        void AnimatedBlock_OnBlockRectangleChange(RectangleF obj)
        {
            if(currhandlingrectchange) return;
            currhandlingrectchange=true;
            
            
            BlockRectangle=base.BlockRectangle=obj;
            //BlockRectangle=obj;
            currhandlingrectchange=false;
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("bblocktype", baseBlock.GetType().FullName);
            info.AddValue("baseblock", baseBlock);
            
        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            XElement result = base.GetXmlData(pNodeName,null);
            result.Add(new XAttribute("BlocKType",baseBlock.GetType().FullName));
            result.Add(StandardHelper.SaveElement(baseBlock,"baseBlock",pPersistenceData));
            return result;
        }
        protected AnimatedBlock(XElement pSource, Object pPersistenceData) :base(pSource,pPersistenceData)
        {
            String BlockType = pSource.Attribute("BlockType").Value;
            Type blocktype = BCBlockGameState.FindClass(BlockType);
            baseBlock = (Block)pSource.ReadElement(blocktype, "baseBlock");
        }
        protected AnimatedBlock(AnimatedBlock pcloneblock)
            : base(pcloneblock)
        {
            baseBlock = (Block)pcloneblock.baseBlock.Clone();
            OnBlockRectangleChange += new Action<RectangleF>(AnimatedBlock_OnBlockRectangleChange);

        }

        protected AnimatedBlock(Block pbaseBlock):base(pbaseBlock)
        {
            baseBlock = pbaseBlock;

            OnBlockRectangleChange += new Action<RectangleF>(AnimatedBlock_OnBlockRectangleChange);

        }
        
        public override bool CheckImpact(BCBlockGameState currentgamestate, cBall hitball, out bool washit, ref List<cBall> ballsadded)
        {
            bool washit1, washit2;
            bool retval = baseBlock.CheckImpact(currentgamestate, hitball, out washit1, ref ballsadded) | base.CheckImpact(currentgamestate,hitball,out washit2,ref ballsadded);
            //aggregate the two return values into one...
            washit=washit1 || washit2;
            if (retval || washit)
            {
                PerformBlockHit(currentgamestate, hitball);


            }
            return retval;
        }
        public override bool DoesBallTouch(cBall hitball)
        {
            return baseBlock.DoesBallTouch(hitball);
        }
        public override void Draw(Graphics g)
        {
           // var TempRect = baseBlock.BlockRectangle;
           // baseBlock.BlockRectangle = this.BlockRectangle;
            
            baseBlock.Draw(g);
            //baseBlock.BlockRectangle=TempRect;
        }
        public override bool MustDestroy()
        {
            return baseBlock.MustDestroy();
        }
        public override bool RequiresPerformFrame()
        {
            return true;
        }
        public abstract override bool PerformFrame(BCBlockGameState gamestate);

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            bool nodefault=false;
            //we want to cause both the events (for hit & destruction) to be invoked; that is, the set that is on us (inherited from our base)
            //as well as the events of our baseBlock aggregate object. 
            return base.PerformBlockHit(parentstate,ballhit)  || baseBlock.PerformBlockHit(parentstate, ballhit) ;




        }


        /// <summary>
        /// called when this block is drawn in the editor. the "normal" implementation of Draw will not be called afterwards.
        /// </summary>
        /// <param name="g"></param>
        public override void EditorDraw(Graphics g, IEditorClient Client)
        {

            IEditorBlockExtensions t = baseBlock as IEditorBlockExtensions;
            if (t != null)
                t.EditorDraw(g,Client);

            
        }

        /// <summary>
        /// retrieves additional tooltip information for a block.
        /// </summary>
        /// <returns></returns>
        public override string GetToolTipInfo(IEditorClient Client)
        {
            IEditorBlockExtensions t = baseBlock as IEditorBlockExtensions;
            if (t != null)
                return t.GetToolTipInfo(Client);
            else
                throw new NotImplementedException();
        }
    }
    /// <summary>
    /// BoundedMovingBlock: allows for any other Block implementation to become a moving block.
    /// </summary>
    /// 
    [Serializable()]
    public class BoundedMovingBlock : AnimatedBlock
    {
        public PointF Velocity { get; set; }
        public RectangleF? MovementBounds { get; set; }
        public BoundedMovingBlock()
        {


        }

        public BoundedMovingBlock(Block blocktomove, PointF pvelocity, RectangleF? pMovementBounds)
            : base(blocktomove)
        {
            if (blocktomove is BoundedMovingBlock)
                throw new ArgumentException("BoundedMovingBlock cannot accept another BoundedMovingBlock as the control block.");


            Velocity = pvelocity;
            MovementBounds = pMovementBounds;


        }
        public BoundedMovingBlock(XElement Source, Object pPersistenceData) :base(Source,pPersistenceData)
        {
            Velocity = Source.ReadElement<PointF>("Velocity");
            MovementBounds = Source.ReadElement<RectangleF>("MovementBounds");
        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            XElement result = base.GetXmlData(pNodeName,pPersistenceData);
            result.Add(StandardHelper.SaveElement(Velocity,"Velocity",pPersistenceData));
            result.Add(StandardHelper.SaveElement(MovementBounds,"MovementBounds",pPersistenceData));
            return result;
        }

        public BoundedMovingBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            Velocity = (PointF)info.GetValue("Velocity", typeof(PointF));
            MovementBounds=(RectangleF?)info.GetValue("MovementBounds",typeof(RectangleF?));



        }
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Velocity", Velocity);
            info.AddValue("MovementBounds", MovementBounds);




        }



        public BoundedMovingBlock(Block blocktomove, PointF pvelocity)
            : this(blocktomove, pvelocity, null)
        {


        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            bool retval = baseBlock.PerformBlockHit(parentstate, ballhit);

            //we need to add speed to the ball based on it's position relative to our speed.
            //or, we can just add our speed and be done with it.
            //ballhit.Velocity = new PointF(ballhit.Velocity.X + (Velocity.X/3), ballhit.Velocity.Y + Velocity.Y);

            BallRelativeConstants ballrel = getBallRelative(ballhit);


            if (((ballrel & BallRelativeConstants.Relative_Left) != 0) || ((ballrel & BallRelativeConstants.Relative_Right) != 0))
            {
                ballhit.Velocity = new PointF(ballhit.Velocity.X + (Velocity.X), ballhit.Velocity.Y/2);

            }
            if (((ballrel & BallRelativeConstants.Relative_Up) != 0) || ((ballrel & BallRelativeConstants.Relative_Down) != 0))
            {
                ballhit.Velocity = new PointF(ballhit.Velocity.X, ballhit.Velocity.Y + (Velocity.Y/2));

            }



            return retval;
        }
        public override object Clone()
        {
            return new BoundedMovingBlock((Block)baseBlock.Clone(), Velocity, MovementBounds);
        }

        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            //first, move the block via the velocity values we have.
            RectangleF userect;
            

            if (MovementBounds != null)
                userect = MovementBounds.Value;
            else
            {
                Rectangle cr = gamestate.TargetObject.ClientRectangle;
                userect = new RectangleF((float)cr.Left, (float)cr.Top, (float)cr.Width, (float)cr.Height);

            }
            
            

            baseBlock.BlockRectangle = new RectangleF(baseBlock.BlockRectangle.Left + Velocity.X, baseBlock.BlockRectangle.Top + Velocity.Y, baseBlock.BlockRectangle.Width, baseBlock.BlockRectangle.Height);

            if (baseBlock.BlockRectangle.Right > userect.Right)
                Velocity = new PointF(-Math.Abs(Velocity.X), Velocity.Y);
            else if (baseBlock.BlockRectangle.Left < userect.Left)
                Velocity = new PointF(Math.Abs(Velocity.X), Velocity.Y);

            if (baseBlock.BlockRectangle.Bottom > userect.Bottom)
                Velocity = new PointF(Velocity.X, -Math.Abs(Velocity.Y));
            else if (baseBlock.BlockRectangle.Top < userect.Top)
                Velocity = new PointF(Velocity.X, Math.Abs(Velocity.Y));

            if (baseBlock.RequiresPerformFrame()) baseBlock.PerformFrame(gamestate);
            return true;
        }






    
    }
    [Serializable]
    public class PathedMovingBlock: AnimatedBlock 
    {
        private String _PathName;
        public String PathName
        {
            get { return _PathName; }
            set
            {
                _PathName = value;
                refreshpoints = true;
            }
        }
            public double speed=2;
        private double pathlength;
        /// <summary>
        /// stores the PointF[] array from the graphics path.
        /// </summary>
        private PointF[] pathpoints;
        /// <summary>
        /// during calculation of the distances, this array will be populated;
        /// each item is the accumulated distance from the start of the graphicspath to the corresponding
        /// pointF element in the pathpoints array.
        /// </summary>
        private double[] distances;
        private double totaldistance;
        private double currentposition; //currentposition indicates the position along the path that the blocks centerpoint is.
        private double initialdist;
        private PointF currentPoint, PreviousPoint;

        public new RectangleF BlockRectangle
        {
            get { return baseBlock.BlockRectangle; }
            set { baseBlock.BlockRectangle = value;base.BlockRectangle=value; }

        }

        //helper routines...
        #region helper routines
        private static double Square(double value)
        {
            return Math.Pow(value, 2);

        }

        private static double Distance(PointF ptA, PointF ptB)
        {
            return Math.Sqrt(Square(ptB.X - ptA.X) + Square(ptA.Y - ptB.Y));


        }
        private static double PathLength(BCBlockGameState gstate,String pathname, out double[] accumdistances)
        {
            ObjectPathData opd = gstate.ClientObject.GetPlayingSet().PathData[pathname];
            return PathLength((from q in opd.PathPoints select q.Location).ToArray() , out accumdistances);


        }

        private static double PathLength(PointF[] ppoints, out double[] accumdistances)
        {
            PointF[] tempout;
            GraphicsPath tempgraph = new GraphicsPath();
            tempgraph.AddLines(ppoints);
            return GraphicsPathLength(tempgraph, out tempout, out accumdistances);




        }

        private static double GraphicsPathLength(GraphicsPath ofPath,out PointF[] ppoints,out double[] accumdistances)
        {
            ppoints = ofPath.PathPoints;
            //get the total length of the entire graphics path.
            accumdistances = new double[ppoints.Length];
            double distanceacc = 0;

            for (int i = 0; i < ppoints.Length - 1; i++)
            {
                //add distance between point i and point i+1 to accumulator.
                distanceacc += Distance(ppoints[i], ppoints[i + 1]);
                accumdistances[i + 1] = distanceacc;


            }
            return distanceacc;

        }
        /// <summary>
        /// returns the point that is a given percentage between the two points.
        /// </summary>
        /// <param name="pointA">First point</param>
        /// <param name="pointB">Second point</param>
        /// <param name="percent">percentage between points to get. 50% will get the midpoint of a imaginary line drawn between the two points, 25 percent will get the midpoint of the first half of a line bisected towards the first point (closer to pointA), etc.</param>
        /// <returns></returns>
        private static PointF Getpercentbetween(PointF pointA, PointF pointB, float percent)
        {
        PointF differencepoint = new PointF(pointB.X-pointA.X,pointB.Y-pointA.Y);
        return new PointF((differencepoint.X * percent)+pointA.X, (differencepoint.Y * percent)+pointA.Y);



        }
        private PointF GetSpeedVector()
        {

            return new PointF(currentPoint.X-PreviousPoint.X,currentPoint.Y-PreviousPoint.Y);



        }

        private PointF PointFromTotalDistance(double ptotaldistance)
        {
            //this is a tricky routine; the task is the get the point thatis "totaldistance" along the graphicspath.
            //this is actually sort of easier given the housekeeping arrays we have.

            //first step: get the first element of distances that is larger then the given distance...
            int indexgot = -1;
            ptotaldistance = ptotaldistance % totaldistance;
            for (int i = 0; i < distances.Length-1; i++)
            {
                
                if (distances[i] > ptotaldistance)
                {
                    indexgot = i;
                    break;
                }


            }
            //subtract one from indexgot to get the last element that is not larger then the value...
            indexgot--;

            //at this point, indexgot now corresponds to the index of the last point that is not larger then the given distance.
            //therefore, we know that the point we are trying to acquire is between pathpoints[indexgot] and pathpoints[indexgot+1].

            //first, find the percentage between the two points our target lies:
            //totaldistance minus our first point will get the "remainder" of the given value; we then get the percentage that is of
            //the distance between the "indexgot" point and the next one.
            double percentbetween = (ptotaldistance - distances[indexgot]) / (distances[indexgot + 1] - distances[indexgot]);


            //lastly, we return it:

            return Getpercentbetween(pathpoints[indexgot], pathpoints[indexgot + 1], (float)percentbetween);

            


        }

        #endregion
        public PathedMovingBlock(RectangleF BlockRect)
        {
            //set baseblock to a Normal block...
            //base.BlockRectangle=BlockRect;
            base.BlockType = "NormalBlock";
            //do that first, otherwise baseBlock would be null.
            BlockRectangle=BlockRect;
            

        }

        private PathedMovingBlock(Block moveblock, GraphicsPath pathmove)
            : this(moveblock, pathmove, 0)
        {


        }
        private bool refreshpoints=false;
        public PathedMovingBlock(Block moveblock, String pathname, float initialdistance):base(moveblock)
        {
            PathName=pathname;
            initialdist = currentposition = initialdistance;
            refreshpoints=true;


        }

        private PathedMovingBlock(Block moveblock, GraphicsPath pathmove,float initialdistance)
            : base(moveblock)
        {
            pathmove.Flatten();
            initialdist = currentposition = initialdistance;
            
            Debug.Print("currentposition=" + currentposition.ToString());
            //initialize some informational stuff...
            totaldistance = GraphicsPathLength(pathmove, out pathpoints, out distances);

        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            bool retval = baseBlock.PerformBlockHit(parentstate, ballhit);

            PointF velocity = GetSpeedVector();
            ballhit.Velocity = new PointF(ballhit.Velocity.X+(velocity.X/2),ballhit.Velocity.Y+(velocity.Y/2));




            return retval;
        }
        private void InitializePath(BCBlockGameState gamestate)
        {
            _PathName = "DEFAULTPATH";
            if (gamestate.ClientObject.GetPlayingSet().PathData.ContainsKey(_PathName))
                return;
            else
            {
                GraphicsPath EllipsePath = new GraphicsPath();

                Rectangle userect = new Rectangle(gamestate.GameArea.Left + 96, gamestate.GameArea.Top + 96, gamestate.GameArea.Width - 192, gamestate.GameArea.Height - 192);
                EllipsePath.AddEllipse(userect);
                GraphicsPathLength(EllipsePath, out pathpoints, out distances);
                

            }



        }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            if (String.IsNullOrEmpty(_PathName) || distances ==null)
            {
                InitializePath(gamestate);
                

            }
            if (refreshpoints)
            {
                totaldistance = PathLength(gamestate, _PathName, out distances);



            }
            //throw new NotImplementedException();
            //add our speed...
            
            PreviousPoint=currentPoint;
            currentposition+=speed;
            //Debug.Print(currentposition.ToString());
            if(currentposition>totaldistance) currentposition = currentposition%totaldistance;
            //now, get the point that is the blocks new position...
            PointF newposition = PointFromTotalDistance(currentposition);

            //now we should have the point; use it as the center point of the block and move the blocks blockrectangle..

            BlockRectangle = new RectangleF(newposition.X-(BlockRectangle.Width/2),newposition.Y - (BlockRectangle.Height/2),BlockRectangle.Width,BlockRectangle.Height);


            currentPoint=newposition;
            if (baseBlock.RequiresPerformFrame()) baseBlock.PerformFrame(gamestate);
            return true;
        }

        #region de/serialization code

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("InitialDistance", initialdist);
            //and of course our path....
            //info.AddValue("PathPoints", pathpoints);
            //as it stands, however, I am "cheating" and creating the graphics path from the given lines...
            info.AddValue("PathName", PathName);





        }
        public PathedMovingBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {

            initialdist = info.GetSingle("InitialDistance");

            /*
            pathpoints = (PointF[])info.GetValue("PathPoints", typeof(PointF[]));
            

            //now, this part is a tad more tricky.


            Byte[] pttypearray = new byte[pathpoints.Length];
            pttypearray[0] = (byte)PathPointType.Start;
            for (int poptype = 1; poptype < pttypearray.Length; poptype++)
            {
                pttypearray[poptype] = (byte)PathPointType.Line;




            }
            GraphicsPath newpath = new GraphicsPath(pathpoints, pttypearray);
            //and now, populate just like with our constructor...

            */

            PathName = info.GetString("PathName");



        }



        #endregion




        public override void Draw(Graphics g)
        {
#if DEBUG
            //g.DrawPath(new Pen(Color.Black, 3), MovingPath);
            if(pathpoints!=null)
                g.DrawPolygon(new Pen(Color.Black), pathpoints);
#endif
            base.Draw(g);
        }
        public PathedMovingBlock(PathedMovingBlock copythis):base(copythis)
        {
            baseBlock = (Block)copythis.baseBlock.Clone();
            PathName = copythis.PathName;
            //MovingPath = (GraphicsPath)copythis.MovingPath.Clone();
            currentposition = copythis.currentposition;
        }

        public override object Clone()
        {
            return new PathedMovingBlock(this);
            //return new PathedMovingBlock((Block)baseBlock.Clone(), (GraphicsPath)MovingPath.Clone(),(float)currentposition);
        }
    }



    /// <summary>
    /// Very similar class to AnimatedImageBlock, but done with textures.
    /// </summary>
    [Serializable]
    
    public class AnimatedTextureBlock : AnimatedImageBlock,IDeserializationCallback 
    {


        TextureBrush[] TextureFrames;
        
        private PointF _InitialTextureOrigin= PointF.Empty;
        private PointF _TextureOriginSpeed= PointF.Empty;
        protected PointF _TextureOrigin = PointF.Empty;
        [TypeConverter(typeof(FloatFConverter))]
        public PointF InitialTextureOrigin { get { return _InitialTextureOrigin; } set { _InitialTextureOrigin = value; } }
        [TypeConverter(typeof(FloatFConverter))]
        public PointF TextureOriginSpeed { get { return _TextureOriginSpeed; } set { _TextureOriginSpeed = value; } }
        

            public AnimatedTextureBlock(RectangleF blockrect, String[] frames, TimeSpan pframeincrementdelay)
            : this(blockrect, frames, pframeincrementdelay, null)
        {


        }
        public AnimatedTextureBlock(RectangleF blockrect):this(blockrect,BCBlockGameState.Imageman.getImageFramesString("GENERIC_"),new TimeSpan(0,0,0,0,250),null)
        {


        }
        private void inittexturebrushes()
        {
            inittexturebrushes((ImageAttributes[])null);

        }
        private void inittexturebrushes(ColorMatrix[] usematrices)
        {
            ImageAttributes[] createattributes = new ImageAttributes[usematrices.Length];
            for (int i = 0; i < usematrices.Length; i++)
            {
                createattributes[i] = new ImageAttributes();
                createattributes[i].SetColorMatrix(usematrices[i]);


            }
            inittexturebrushes(createattributes);

        }

        private void inittexturebrushes(ImageAttributes[] useattributes)
        {
            bool useattribs=false;
            if (useattributes == null || useattributes.Length == 0)  
                useattribs = false;
            

            if (AnimationFrames != null)
            {
                TextureFrames = new TextureBrush[AnimationFrames.Length];
                for (int i = 0; i < AnimationFrames.Length; i++)
                {
                    if(useattribs)
                        TextureFrames[i] = new TextureBrush(AnimationFrames[i], BlockRectangle,useattributes[i%useattributes.Length]);
                    else
                    {
                        TextureFrames[i] = new TextureBrush(AnimationFrames[i]);
                    }


                }
            }

        }
        
        public AnimatedTextureBlock(RectangleF blockrect, String[] frames, TimeSpan pframeincrementdelay, BlockHitFunction[] hitdelegates)
            :base(blockrect,frames,pframeincrementdelay,hitdelegates)
        {

            inittexturebrushes();



        }
        public AnimatedTextureBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            InitialTextureOrigin = (PointF)info.GetValue("InitialTextureOrigin", typeof(PointF));
            TextureOriginSpeed = (PointF)info.GetValue("TextureOriginSpeed", typeof(PointF));
            _TextureOrigin = InitialTextureOrigin;
            inittexturebrushes();
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("InitialTextureOrigin", InitialTextureOrigin);
            info.AddValue("TextureOriginSpeed", TextureOriginSpeed);
        }
        public AnimatedTextureBlock(AnimatedTextureBlock clonethis)
            : base(clonethis)
        {
            InitialTextureOrigin = clonethis.InitialTextureOrigin;
            TextureOriginSpeed = clonethis.TextureOriginSpeed;
            TextureFrames = clonethis.TextureFrames;
        }
        public override object Clone()
        {
            return new AnimatedTextureBlock(this);
        }

        protected TextureBrush getCurrTexture()
        {
            TextureBrush Returnbrush = TextureFrames[currentframe];
            Returnbrush.ResetTransform();
            Returnbrush.TranslateTransform(_TextureOrigin.X, _TextureOrigin.Y);
            return Returnbrush;



        }

        public override void Draw(Graphics g)
        {
            g.FillRectangle(getCurrTexture(), BlockRectangle);
        }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            //animate the texture origin...


            Image currimage = AnimationFrames[currentframe];
                
                _TextureOrigin = new PointF(_TextureOrigin.X + TextureOriginSpeed.X,
                                            _TextureOrigin.Y + TextureOriginSpeed.Y);
                _TextureOrigin = new PointF(_TextureOrigin.X%currimage.Width, _TextureOrigin.Y%currimage.Height);
            
            return base.PerformFrame(gamestate);
        }


        #region IDeserializationCallback Members

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);
            Debug.Print("AnimatedTextureBlock::ONDeserialization");
            inittexturebrushes();
        }

        #endregion
    }
    [Serializable]
    [PassiveEffectCategory]
    public abstract class PassiveEffectBlock : AnimatedTextureBlock
    {
        protected PassiveEffectBlock(RectangleF blockrect, String[] frames, TimeSpan pframeincrementdelay):base(blockrect,frames,pframeincrementdelay)
    {
        Debug.Print("PassiveEffectBlock constructor");
    }
        protected PassiveEffectBlock(PassiveEffectBlock clonethis):base(clonethis)
        {
            //doesn't actually do anything...

        }
        public PassiveEffectBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            Debug.Print("PassiveEffectBlock constructor");


        }
       
        public abstract override object Clone();
        
        public abstract void PassiveEffect(cBall hitball);

        public override bool RequiresPerformFrame()
        {
            return true;
        }
        
        public override bool MustDestroy()
        {
            return false;
        }
        public override bool DoesBallTouch(cBall hitball)
        {
            Debug.Print("PassiveEffectBlock::DoesBallTouch");
            if (baseBlock.DoesBallTouch(hitball))
            {

                //BUGFIX: balls that are inside this block when the game is paused get fiddlewaggled and no longer move afterward (since their behaviour that is added
                //when the game is paused is ripped from them. So, we check to make sure the speed is greater than 0,0, for now.
                //if we are being touched by this ball, perform the passive effect.
                
                
                //and make takevehaviours do the opposite... (new field, obviously)
                if (hitball.Velocity.X != 0 && hitball.Velocity.Y != 0)
                    PassiveEffect(hitball);


            }
            return false;
        }
        public override bool CheckImpact(BCBlockGameState currentgamestate, cBall hitball, out bool washit, ref List<cBall> ballsadded)
        {
            //return base.CheckImpact(currentgamestate, hitball, out washit, ref ballsadded);
            //Fix:  switched base.CheckImpact (which called "PerformImpact" and thus destroys the block) with baseBlock, which shouldn't.
            //... but still might...
            //Debug.Print("PowerAssignmentBlock::CheckImpact");
            
            if (baseBlock.DoesBallTouch(hitball))
            {
                //if we are being touched by this ball, give it the behaviours.
                PassiveEffect(hitball);


            }

            washit = false;
            return false; //never impact
        }


    }

    
///PowerAssignmentBlock: when a ball passes over this block, it will be given the powers (behaviours)
    ///that are stored in this block for that purpose. Note that the ball's direction and speed are unaffected.
    [Serializable]
    [PowerupEffectCategory]
    public class PowerAssignmentBlock : PassiveEffectBlock
    {
        private List<BaseBehaviour> _GiveBehaviours = new List<BaseBehaviour>();
        



        [Editor(typeof(BallBehaviourCollectionEditor), typeof(UITypeEditor))]
        public List<BaseBehaviour> GiveBehaviours { get { return _GiveBehaviours; } set { _GiveBehaviours = value; } }
        

        public PowerAssignmentBlock(PowerAssignmentBlock clonethis):this(clonethis.BlockRectangle)
        {
            _GiveBehaviours = clonethis.GiveBehaviours.Clone();


        }
        public override object Clone()
        {
            return new PowerAssignmentBlock(this);
        }
        public PowerAssignmentBlock(RectangleF BlockRect)
            : base(BlockRect, BCBlockGameState.Imageman.getImageFramesString("field"), new TimeSpan(0, 0, 0, 0, 25))
        {
            base.TextureOriginSpeed= new PointF(0,2);


        }
        public PowerAssignmentBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            _GiveBehaviours = (List<BaseBehaviour>)info.GetValue("GiveBehaviours", typeof(List<BaseBehaviour>));


        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("GiveBehaviours", _GiveBehaviours);
        }
        private void DoGiveBehaviours(cBall giveto)
        {
            List<iBallBehaviour> removeitems = new List<iBallBehaviour>();
            foreach (var loopitem in giveto.Behaviours)
            {

                //remove any item whose type does not have
                //the Assignmentnonremovable attribute.
                if (!BCBlockGameState.HasAttribute(loopitem.GetType(), typeof(AssignmentNonremovableAttribute)))
                {
                    removeitems.Add(loopitem);


                }
                else
                {

                    Debug.Print("not removing behaviour type " + loopitem.GetType().Name);
                    if (loopitem.GetType().Name.ToUpper().IndexOf("pause") != -1)
                        Debug.Print("paused");
                }


            }
            foreach (var removeit in removeitems)
            {
                giveto.Behaviours.Remove(removeit);

            }


            //giveto.Behaviours.Clear();
            foreach (BaseBehaviour loopbeh in GiveBehaviours)
            {
                giveto.Behaviours.Add((BaseBehaviour)loopbeh.Clone());

            }

        }

        public override bool MustDestroy()
        {
            return false;
        }





        public override void PassiveEffect(cBall hitball)
        {
            DoGiveBehaviours(hitball);
        }
    }





    public delegate bool BlockHitFunction(Block OnBlock, BCBlockGameState parentstate, cBall ballhit);
    [Serializable]
    public class AnimatedImageBlock : AnimatedBlock
    {
        //an AnimatedImageBlock is simply an Imageblock with a sequence of images frames through which it cycles.
        protected Image[] AnimationFrames=null;
        protected String[] AniFrameKeys = null;

        public List<String> AnimationKeys
        {
            get { return AniFrameKeys.ToList(); }
            set {
                AniFrameKeys = value.ToArray();
                ReloadImages();
            }


        }
        


        protected int currentframe = 0;
        protected TimeSpan frameincrementdelay;
        //default to last write time...
        static DateTime lastwritetime = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;
        private DateTime lastperf = lastwritetime;
        //public override bool  PerformBlockHit(BCBlockGameState parentstate, cBall ballhit, ref List<cBall> ballsadded)
       
        private Image[] imagetogettype= new Image[5];
        private String[] stringtogettype = new string[5];
        private BlockHitFunction[] FrameHitFunctions=null;
        protected virtual void ReloadImages()
        {
           

        }

        public AnimatedImageBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //AnimationFrames = (Image[])info.GetValue("AnimationFrames", imagetogettype.GetType());
            AniFrameKeys = (String[])info.GetValue("AniFrameKeys", stringtogettype.GetType());
            AnimationFrames = GetKeyImages(ref AniFrameKeys);
            frameincrementdelay=(TimeSpan)info.GetValue("frameincrementdelay",typeof(TimeSpan));

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("AniFrameKeys", AniFrameKeys);
            info.AddValue("frameincrementdelay", frameincrementdelay);



        }
        public AnimatedImageBlock(RectangleF blockrect, String[] frames)
            : this(blockrect, frames, new TimeSpan(0, 0, 0, 0, 100), null)
        {

            
        }
        private void ExpandKeyNames(ref String[] keynames)
        {
            List<String> createlist = new List<string>();
            foreach (string t in keynames)
            {
                String[] entries = BCBlockGameState.Imageman.getImageFramesString(t);
                if (entries.Length > 0)
                {
                    createlist.AddRange(entries);

                }
                else
                {
                    createlist.Add(t);
                    
                }
            }
            keynames = createlist.ToArray();


        }

        /// <summary>
        /// Given a list if String keys, creates and returns an array if Image objects
        /// corresponding to those images. the parameter will also be updated to contain all appropriate elements
        /// </summary>
        /// <param name="keys">String array containing Key values in the Image Manager. Any key that is a base name image frame will be added to the array.</param>
        /// <returns></returns>
        /// 
        //Example: if the image manager has items named GENERIC_1,GENERIC_2,GENERIC_3, BEAR1,BEAR2, and turkey in, then passing in the array:
        //GENERIC,BEAR,TURKEY will change the String[] array to be:
        //GENERIC_1,GENERIC_2,GENERIC_3,BEAR1,BEAR2,turkey 

        private Image[] GetKeyImages(ref String[] keys)
        {
            ExpandKeyNames(ref keys);

            Image[] returnthis = new Image[keys.Length];
            for (int i = 0; i < keys.Length; i++)
                returnthis[i] = BCBlockGameState.Imageman[keys[i]];

            return returnthis;


        }

        public AnimatedImageBlock(RectangleF blockrect, String[] frames, TimeSpan pframeincrementdelay, BlockHitFunction[] hitdelegates)
            : base(new ImageBlock(blockrect, frames[0]))
        {
            AniFrameKeys = frames;
            AnimationFrames = GetKeyImages(ref AniFrameKeys);
            BlockRectangle=blockrect;
            FrameHitFunctions = hitdelegates;
            frameincrementdelay = pframeincrementdelay;
            ((ImageBlock)baseBlock).hasChanged = true;
        }

        


        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            if ((DateTime.Now - lastperf) > frameincrementdelay)
            {
                currentframe++;
                lastperf = DateTime.Now;
            }
            if (currentframe > AnimationFrames.Length-1) currentframe = 0;
            ImageBlock asimage = baseBlock as ImageBlock;
            if (asimage != null)
            {
                asimage.BlockImageKey = AniFrameKeys[currentframe];

                asimage.hasChanged = true;
            }
            return true;
        }
        public override void Draw(Graphics g)
        {
            
            base.Draw(g);
        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            if (FrameHitFunctions != null)
            {
                //if we have framehitfunctions defined...
                int callthis=currentframe;
                if (currentframe > FrameHitFunctions.Length)
                {
                    //was working here; call the appropriate blockhit routine by using the modulus on the current frame.
                    callthis = currentframe % FrameHitFunctions.Length;


                }
                return FrameHitFunctions[callthis](this,parentstate, ballhit);




            }
            else
            {
                //otherwise, go with def behaviour:
                return base.PerformBlockHit(parentstate, ballhit);


            }
        }
        public AnimatedImageBlock(AnimatedImageBlock clonethis):base(clonethis)
        {
            AniFrameKeys = clonethis.AniFrameKeys;
            this.AnimationFrames = clonethis.AnimationFrames;

        }

        public override object Clone()
        {
            return new AnimatedImageBlock(this);
        }
    }
    
    [Serializable]
    public class PulsingBlock : AnimatedBlock
    {
        //a pulsing block has several "phases"
        //1: the block is fully visible.
        //2: the block is "fading out" (can balls hit it? not yet)
        //3: the block is invisible. (balls go straight through)
        //4: the block is fading in (again, not sure whether to have balls hit it at this stage or not

        protected enum PulsingBlockState
        {
            Pulse_Visible=0,
            Pulse_Fadeout=1,
            Pulse_Invisible=2,
            Pulse_Fadein=3
        }
        private PulsingBlockState PulseState = PulsingBlockState.Pulse_Visible;

        //also an array of timespans
        private static TimeSpan halfsecond = new TimeSpan(0,0,0,0,500);
        private static TimeSpan fullsecond = new TimeSpan(0,0,0,1);
        /// <summary>
        /// Pulse times. First element will be the amount of time to stay in Pulse_Visible (fully visible) mode, second element will dictate the length of the fadeout, etc.
        /// </summary>
        protected TimeSpan[] _PulseTimes = new TimeSpan[] { fullsecond, halfsecond, fullsecond, halfsecond };

        [Editor(typeof(GenericCollectionEditor<TimeSpan>),typeof(UITypeEditor))]
        public List<TimeSpan> PulseTimes { get { return _PulseTimes.ToList(); } set { _PulseTimes = value.ToArray(); } }


            private Bitmap ourblockbitmap;
        private Graphics ourblockgraphics;
        private DateTime? currphasestart;



        public PulsingBlock(RectangleF blockrect)
        {
            baseBlock = new NormalBlock(blockrect, new SolidBrush(Color.Red), new Pen(Color.Black));
            BlockRectangle=blockrect;
            

        }

        public PulsingBlock(Block pbaseblock)
            : base((Block)pbaseblock)
        {


        }
        public PulsingBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            PulseTimes = (List<TimeSpan>)info.GetValue("PulseTimes", typeof(List<TimeSpan>));

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PulseTimes", PulseTimes);
        }
        public PulsingBlock(PulsingBlock clonethis):base(clonethis)
        {


        }
        public override object Clone()
        {
            return new PulsingBlock(this);
        }
        public override bool RequiresPerformFrame()
        {
            return true;
        }
        /// <summary>
        /// Advances the "phase" to the next phase.
        /// </summary>
        protected void AdvancePhase()
        {
            //use modulus; 3 (the fading in phase) will advance to 4 and then 4 %4==0 and goes back to the first state (visible).
        PulseState = (PulsingBlockState)((((int)PulseState)+1)%4);
        currphasestart=DateTime.Now;


        }
        private double phasepercentcomplete = 0;
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            //task: change our "phase" depending on the timing values.
            if (currphasestart == null)
            {
                currphasestart = DateTime.Now;
                PulseState = PulsingBlockState.Pulse_Visible;

            }
            else
            {
                //otherwise, we already have a "current" time, so have already "engaged" a phase.
                //so the task here is to compare the difference between Now and phase start time, and if it is larger then 
                //the corresponding PulseTime, advance it to the next "phase", and reset the time to datetime.now.
                var datenow = DateTime.Now;
                TimeSpan currpulsetime = _PulseTimes[(int)PulseState];
                if (datenow - currphasestart.Value > currpulsetime)
                {
                    //advance the current phase.
                    AdvancePhase();

                }
                //assign the double that holds the "percentage" we are through the current phase.
                phasepercentcomplete = ((float)((datenow - currphasestart.Value).TotalMilliseconds)) / ((float)(currpulsetime.TotalMilliseconds));
            }


            return baseBlock.PerformFrame(gamestate);
            //throw new NotImplementedException();
        }
        
        private void RecreateBasebitmap()
        {
            //condition: if either the bitmap or graphics object are null or our BlockRectangle is different from the size of the generated image, 
            if ((ourblockbitmap == null || ourblockgraphics == null) ||
                ((ourblockbitmap.Size.Width != (int)baseBlock.BlockRectangle.Width) ||
                (ourblockbitmap.Size.Height != (int)baseBlock.BlockRectangle.Height)))
            {
                //recreate ourblockbitmap and ourblockgraphics to be the appropriate size. 
                ourblockbitmap = new Bitmap((int)baseBlock.BlockRectangle.Width, (int)baseBlock.BlockRectangle.Height);
                ourblockgraphics = Graphics.FromImage(ourblockbitmap);
                //ourblockgraphics.Clear(Color.Transparent);

            }



        }
        public override bool DoesBallTouch(cBall hitball)
        {

            if (PulseState != PulsingBlockState.Pulse_Visible)
            {
                
                return false; //can't be hit while invisible, or fading in or out.
              
            }
            return base.DoesBallTouch(hitball);
        }
        public override void Draw(Graphics g)
        {
            //this is "tricky"; we create/use
            //clear current image
            if (ourblockgraphics == null || ourblockbitmap == null) RecreateBasebitmap();
            ourblockgraphics.Clear(Color.Transparent);
            //temporarily change our BlockRectangle to be at 0,0...
            RectangleF temprect = BlockRectangle;
            BlockRectangle = new RectangleF(0, 0, BlockRectangle.Width, BlockRectangle.Height);
            //now tell our base to draw to ourblockgraphics...
            base.Draw(ourblockgraphics);
            BlockRectangle=temprect;
            Rectangle intrect = new Rectangle((int)BlockRectangle.Left,(int)BlockRectangle.Top,(int)BlockRectangle.Width,(int)BlockRectangle.Height);
            ImageAttributes AlphaAttributes=null;
            AlphaAttributes = new ImageAttributes();
            //Debug.Print("phasepercent:" + phasepercentcomplete);
            switch (PulseState)
            {
                case PulsingBlockState.Pulse_Fadein:
                    Debug.Print("fadin:" + phasepercentcomplete);
                    AlphaAttributes.SetColorMatrix(ColorMatrices.GetColourizer(1, 1, 1, (float)phasepercentcomplete));
                    break;
                case PulsingBlockState.Pulse_Fadeout:
                    Debug.Print("fadeout:" + (1-phasepercentcomplete));
                    AlphaAttributes.SetColorMatrix(ColorMatrices.GetColourizer(1, 1, 1, (float)(1 - phasepercentcomplete)));
                    break;
                case PulsingBlockState.Pulse_Visible:
                    AlphaAttributes = null;
                    break;
                case PulsingBlockState.Pulse_Invisible:
                    AlphaAttributes.SetColorMatrix(ColorMatrices.GetColourizer(1, 1, 1, 0.01f));
                    break;
            }
            
            
            //now, draw <that> image to the <actual> graphics context...
            g.DrawImage(ourblockbitmap, intrect, 0, 0, ourblockbitmap.Width, ourblockbitmap.Height, GraphicsUnit.Pixel, AlphaAttributes);
            

            
        }

    }
    /// <summary>
    /// invadercontroller: implements "gameobject" but only so it can be given a PerformFrame.
    /// </summary>
    public class InvaderController :GameObject 
    {
        private int InvaderSpeed = 2;

        public enum InvaderHordeDirectionConstants
        {
            Invader_Left,
            Invader_Right,
            Invader_Down


        }
        public InvaderHordeDirectionConstants HordeDirection = InvaderHordeDirectionConstants.Invader_Left;
        private DateTime? lastFrame;
        private TimeSpan HordeAdvanceDelay = new TimeSpan(0, 0, 0, 0, 600);
        private TimeSpan movedowntime = new TimeSpan(0, 0, 0, 0, 300);
        private DateTime startmovedown;
        private int distancetolerance = 15;
        private InvaderHordeDirectionConstants nextdirection;
        
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            if (!(gamestate.ClientObject.ActiveState is StateRunning)) return false;
            Debug.Print("InvaderController::PerformFrame");
            //first, only act if our interval has expired.
            if (lastFrame!=null || (DateTime.Now - lastFrame > HordeAdvanceDelay))
            {
                gamestate.Forcerefresh =true;

                

                //acquire ALL invader blocks in the game...
                List<InvaderBlock> allinvaders = (from q in gamestate.Blocks where q is InvaderBlock select (InvaderBlock)q).ToList();
                float checkycoord;
                if (gamestate.PlayerPaddle != null)
                {
                    checkycoord= gamestate.PlayerPaddle.Position.Y;
                    InvaderSpeed = 2;
                }
                else
                {
                    checkycoord=gamestate.GameArea.Height;
                    InvaderSpeed = 12;
                }
                //first step: check if ANY of the "invaders" have passed the paddle; if so....
                if (allinvaders.Any((y) => y.BlockRectangle.Bottom >checkycoord))
                {

                    //destroy <ALL> balls present in the level, this will cause a life to be lost next frame.
                   // foreach (cBall removeme in gamestate.Balls) gamestate.Balls.Remove(removeme);
                    gamestate.Balls.Clear();
                    
                    //ok, now, how many invader blocks do we have left?
                    int countinvaders = allinvaders.Count();

                    //alright, remove ALL invader blocks...
                    
                    //allinvaders=null;

                    int addnumber = 0;
                    foreach (InvaderBlock cloneblock in (from p in gamestate.ClientObject.getcurrentLevel().levelblocks where p is InvaderBlock select p))
                    {
                        addnumber++;
                        if(addnumber==countinvaders) break; //break out of the foreach...
                        //add this block to the current levels blocks...
                        
                        gamestate.Blocks.AddLast((Block)cloneblock.Clone());

                    }
                    foreach (Block removeinvader in allinvaders) gamestate.Blocks.Remove(removeinvader);


                    return false;

                }
                //acquire the leftmost and rightmost blocks...
                InvaderBlock leftmost=null,rightmost=null;
                float currminleft = 65536, currmaxleft = 0;
                foreach (InvaderBlock loopinvader in allinvaders)
                {
                    //since we are looping to find the min and max for left and right anyway, may as well update their locations as well.
                    switch (HordeDirection)
                    {
                        case InvaderHordeDirectionConstants.Invader_Down:
                            loopinvader.BlockLocation = new PointF(loopinvader.BlockLocation.X,
                                                                   loopinvader.BlockLocation.Y + InvaderSpeed);
                            
                            break;
                        case InvaderHordeDirectionConstants.Invader_Left:
                            loopinvader.BlockLocation = new PointF(loopinvader.BlockLocation.X - InvaderSpeed,
                                                                   loopinvader.BlockLocation.Y);
                            break;
                        case InvaderHordeDirectionConstants.Invader_Right:
                            loopinvader.BlockLocation = new PointF(loopinvader.BlockLocation.X + InvaderSpeed,
                                                                   loopinvader.BlockLocation.Y);
                            break;

                    }
                    loopinvader.baseBlock.BlockRectangle = loopinvader.BlockRectangle;
                    

                    if (loopinvader.BlockLocation.X < currminleft)
                    {
                        leftmost = loopinvader;
                        currminleft = loopinvader.BlockLocation.X;

                    }

                    if (loopinvader.BlockLocation.X > currmaxleft)
                    {
                        rightmost = loopinvader;
                        currmaxleft = loopinvader.BlockLocation.X;

                    }

                 
                    switch (HordeDirection)
                    {
                        case InvaderHordeDirectionConstants.Invader_Left:

                            //if the horde is moving left, allow it to remain undisturbed, 
                            //unless our leftmost block's left side is within distancetolerance..

                            if (leftmost.BlockRectangle.Left < distancetolerance)
                            {
                                //set to move down, and assign the start value  for when we started moving down..
                                HordeDirection = InvaderHordeDirectionConstants.Invader_Down;
                                startmovedown=DateTime.Now;
                                nextdirection = InvaderHordeDirectionConstants.Invader_Right;
                            }


                            break;
                        case InvaderHordeDirectionConstants.Invader_Right:
                            //similar to above, but for the rightmost and the right side...
                            if ((gamestate.GameArea.Width - rightmost.BlockRectangle.Right) < distancetolerance)
                            {
                                HordeDirection = InvaderHordeDirectionConstants.Invader_Down;
                                startmovedown=DateTime.Now;
                                nextdirection = InvaderHordeDirectionConstants.Invader_Left;


                            }
                            break;
                        case InvaderHordeDirectionConstants.Invader_Down:
                            if (DateTime.Now - startmovedown > movedowntime)
                            {
                                HordeDirection = nextdirection;



                            }

                            break;


                    }
                }



                //advance All invader blocks based on our direction



            }
            //throw new NotImplementedException();
            lastFrame = DateTime.Now;
            return false;

        }

        public override void Draw(Graphics g)
        {
            //nothing to draw
            //throw new NotImplementedException();
        }
    }
    public class InvaderPaddleDamage : PaddleBehaviours.BasePaddleBehaviour
    {
        public override Image GetIcon()
        {
            return null;
        }
        public override bool Impact(Paddle onPaddle, cBall withBall)
        {
            if (withBall as InvaderBallShot != null)
            {
                //it's an invader ballshot...
                //err... move the ball like really far away...
                withBall.Location = new PointF(-50,-50);
                //and take some damage...
                onPaddle.HP -= 12;
                    



            }
            return false;
        }
        public override void Draw(Paddle onPaddle, Graphics g)
        {
            //throw new NotImplementedException();
            //do nothing. Although we might make it paint some sort of ship like texture...
        }
        



    }
    public class InvaderBallShot : cBall
    {
        public InvaderBallShot(PointF Location, PointF Velocity, float pRadius)
            : base(Location, Velocity)
        {
            Radius=pRadius;
            DrawColor=Color.White;


        }
        




    }
    [Serializable]
    public class InvaderBlock : AnimatedImageBlock
    {
       
       
       // private InvaderDirectionConstants InvaderDirection=InvaderDirectionConstants.Invader_Left;
        TimeSpan minTimeShoot = new TimeSpan(0, 0, 0, 0, 100);
        TimeSpan maxTimeShoot = new TimeSpan(0, 0, 0, 3, 0);
        TimeSpan? currshootdelay;
        DateTime? LastShot;

        public InvaderBlock(RectangleF blockrect)
            : base(blockrect, BCBlockGameState.Imageman.getImageFramesString("invader"))
        {
            
            

        }
        bool hasInited=false;
        public override bool RequiresPerformFrame()
        {
            return true;
        }
        private TimeSpan MakeShootDelay()
        {
            return new TimeSpan(0,0,0,0,(int)(((maxTimeShoot.TotalMilliseconds-minTimeShoot.TotalMilliseconds)*(float)BCBlockGameState.rgen.NextDouble())+minTimeShoot.TotalMilliseconds));

        }

        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            
            if (currshootdelay == null) currshootdelay = MakeShootDelay();
            if(LastShot==null) LastShot=DateTime.Now;
            //invader behaviour:
            //step one: move based on our direction.
            hasInited=true;
            //add an InvaderController if one isn't already present.
            if (!gamestate.GameObjects.Any((q) => q is InvaderController))
            {
                gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                gamestate.GameObjects.AddLast(new InvaderController())));

                
            }
            if(gamestate.PlayerPaddle!=null)
            {
                if (!gamestate.PlayerPaddle.Behaviours.Any((y) => y is InvaderPaddleDamage))
                {
                    gamestate.PlayerPaddle.Behaviours.Add(new InvaderPaddleDamage());


                }
            }
            if (DateTime.Now - LastShot > currshootdelay)
            {
                Shoot(gamestate);
                currshootdelay = MakeShootDelay();
                LastShot = DateTime.Now;

            }

            return baseBlock.PerformFrame(gamestate);
        }
        public override bool CheckImpact(BCBlockGameState currentgamestate, cBall hitball, out bool washit, ref List<cBall> ballsadded)
        {
            if (((hitball as InvaderBallShot ==null)))
            {
                washit=true;
                return base.CheckImpact(currentgamestate, hitball, out washit, ref ballsadded);
            }
            else
            {
                washit = false;
                return false;
            }
            
        }


        private void Shoot(BCBlockGameState gamestate)
        {
            if (gamestate.ClientObject.ActiveState is StateRunning)
            {

                PointF shootspeed = new PointF(0, 5);
                //InvaderBallShot mInvaderBallShot = new InvaderBallShot(CenterPoint(), shootspeed, 3);
                //mInvaderBallShot.Behaviours.Add(new TempBallBehaviour());
                //gamestate.Balls.AddLast(mInvaderBallShot);
                
                //CHANGE: this now shoots bullets instead of "Invader Balls".
                Bullet invadershot = new Bullet(CenterPoint(), shootspeed, true);
                invadershot.Owner = this;
                gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => gamestate.GameObjects.AddLast(invadershot)));


                BCBlockGameState.Soundman.PlaySound("spitfire", 1.0f);

            }
         



            
        }
        public InvaderBlock(InvaderBlock clonethis)
            : base(clonethis)
        {


        }
        public InvaderBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {


        }

        public override object Clone()
        {
            return new InvaderBlock(this);
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        



    }


    [Serializable]
    public class GrowBlock : StrongBlock 
    {
        private const int growdelaytime=500;
        private int mGrowFrame=0;

        public GrowBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {


        }
        public GrowBlock(GrowBlock copythis):base(copythis)
        {


        }

        public GrowBlock(RectangleF blockrect)
            : base(blockrect)
        {


        }

        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            //return base.PerformFrame(gamestate);
            if(numhits==0) return true;
            mGrowFrame++;
            //Debug.Print("mGrowFrame=" + mGrowFrame.ToString());
            if (mGrowFrame >= growdelaytime)
            {
                numhits--;
                hasChanged=true;
                
                mGrowFrame = 0;

            }
            return true;
        }
        public override object Clone()
        {
            return new GrowBlock(BlockRectangle);
        }
        public override bool RequiresPerformFrame()
        {
           return true;
        }
        
        public override void Draw(Graphics g)
        {
            //if(numhits>0) Debug.Print("GrowBlock::Draw numhits=" + numhits);
            base.Draw(g);
            g.FillRectangle(new SolidBrush(Color.FromArgb(128,Color.Green)), BlockRectangle);
        }





    }
    
    /// <summary>
    /// ControllableBlock: allows control of the block using the keyboard
    /// </summary>
    [Serializable]
    public class ControllableBlock : AnimatedBlock, ISerializable, ICloneable,IDisposable
    {

        private bool hookinit = false; //true if our hook routines are "installed".

        private ButtonConstants Key_Upwards = ButtonConstants.Button_Up;
        private ButtonConstants Key_Rightwards = ButtonConstants.Button_Right;
        private ButtonConstants Key_Leftwards = ButtonConstants.Button_Left;
        private ButtonConstants Key_Downwards = ButtonConstants.Button_Down;
        private WeakReference mgamestate;
        private int moveconstant = 2;
        private int minMillisecondsbetweenmove = 50;
        private DateTime LastFrameTime;
        private PointF MovementVelocity = new PointF(0, 0);
        private bool mControllable = true;


        public bool Controllable { get { return mControllable; } set { mControllable = value; } }

        public ControllableBlock(RectangleF pBlockrect)
            : base(new InvincibleBlock(pBlockrect))
        {
            BlockRectangle = pBlockrect;


        }

        public ControllableBlock(Block baseBlock)
            : base(baseBlock)
        {
            



        }
        public ControllableBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            Debug.Print("ControllableBlock");
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Debug.Print("ControllableBlock::GetObjectData");
            base.GetObjectData(info, context);
        }

        public ControllableBlock(ControllableBlock clonethis)
            : base(clonethis)
        {
            //Don't forget to set values here :P

        }
        public override bool RequiresPerformFrame()
        {
            return !hookinit || (MovementVelocity.X != 0 && MovementVelocity.Y != 0);
        }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {

            if (!hookinit)
            {
                hookinit=true;
                gamestate.ClientObject.ButtonDown += ClientObject_ButtonDown;
                gamestate.ClientObject.ButtonUp += ClientObject_ButtonUp;

                LastFrameTime = DateTime.Now;
                mgamestate = new WeakReference(gamestate);
                Debug.Print("ControllableBlock: key hooks initialized...");
                return true;
            }
            else
            {
                double msbetween = (DateTime.Now - LastFrameTime).TotalMilliseconds;
                if ((int)msbetween > minMillisecondsbetweenmove)
                {
                    BlockLocation = new PointF(BlockLocation.X + MovementVelocity.X,
                                               BlockLocation.Y + MovementVelocity.Y);


                    LastFrameTime = DateTime.Now;
                    return true;
                }
                else
                {
                    return false;
                }
            }





            //          throw new NotImplementedException();
        }

        void ClientObject_ButtonUp(Object sender, ButtonEventArgs<bool> e)
        {
            Debug.Print("ButtonUp caught by ControllableBlock");
            if (!mControllable) { e.Result = false; return; }
            //throw new NotImplementedException();
            if (e.Button == Key_Upwards)
            {
                Debug.Print("Controllableblock:Upwards");
                MovementVelocity = new PointF(MovementVelocity.X, MovementVelocity.Y + 1);

            }
            else if (e.Button== Key_Downwards)
            {
                Debug.Print("Controllableblock:Downwards");
                MovementVelocity = new PointF(MovementVelocity.X, MovementVelocity.Y - 1);

            }
            else if (e.Button  == Key_Leftwards)
            {
                Debug.Print("Controllableblock:Leftwards");
                MovementVelocity = new PointF(MovementVelocity.X+ 1, MovementVelocity.Y);

            }
            else if (e.Button == Key_Rightwards)
            {
                Debug.Print("Controllableblock:Rightwards");
                MovementVelocity = new PointF(MovementVelocity.X - 1, MovementVelocity.Y);

            }
            else
            {
                e.Result = false;
                return;
            }
            e.Result = true;
        }

        void ClientObject_ButtonDown(Object sender, ButtonEventArgs<bool> e)
        {
            Debug.Print("ButtonDown caught by ControllableBlock");
            if (!mControllable) { e.Result = false; return; }
            //throw new NotImplementedException();

            if ((e.Button&Key_Upwards) == Key_Upwards)
            {
                MovementVelocity = new PointF(MovementVelocity.X, MovementVelocity.Y - 1);

            }
            else if ((e.Button&Key_Downwards) == Key_Downwards)
            {
                MovementVelocity = new PointF(MovementVelocity.X, MovementVelocity.Y + 1);

            }
            else if ((e.Button & Key_Leftwards) == Key_Leftwards)
            {
                MovementVelocity = new PointF(MovementVelocity.X-1, MovementVelocity.Y);

            }
            else if ((e.Button & Key_Rightwards) == Key_Rightwards)
            {

                MovementVelocity = new PointF(MovementVelocity.X + 1, MovementVelocity.Y);

            }
            else
            {
                e.Result = false;
                return;
            }
            e.Result = true;
            

        }
       

        public override object Clone()
        {
//            throw new NotImplementedException();
            return new ControllableBlock(this);
        }

        public void Dispose()
        {
            if (hookinit)
            {
                //deinitialize hooks

                if(mgamestate.IsAlive)
                {
                    ((BCBlockGameState)mgamestate.Target).ClientObject.ButtonDown -= ClientObject_ButtonDown;
                    ((BCBlockGameState)mgamestate.Target).ClientObject.ButtonUp -= ClientObject_ButtonUp;


                }
            }

        }
    }

    [Serializable]
    public class MovingPlatformBlock : AnimatedImageBlock, ISerializable, ICloneable,iPlatformBlockExtension
    {
        public enum BlockCollisionEffectMode
        {
            CollisionEffect_Reverse, //reverses direction.
            CollisionEffect_NextSpeed //advances to next speed.



        }
        private System.Collections.Generic.HashSet<PlatformObject> _TrackingObjects = new HashSet<PlatformObject>();
        private bool _Interactive = true;
        private BlockCollisionEffectMode _BlockCollisionEffect = BlockCollisionEffectMode.CollisionEffect_NextSpeed;
        private PointF _BlockVelocity = new PointF(0, 0);
        private List<PointF> _StartSpeedStates = new List<PointF>(new PointF[] {new PointF(2, 0),new PointF(0,-2),new PointF(-2,0),new PointF(0,2)});
        private int NextStartSpeed = 0; //next index to use.
        private PointF _StopSpeed = new PointF(0, 0);
        [TypeConverter(typeof(FloatFConverter))]
        public PointF BlockVelocity { get { return _BlockVelocity; } set { _BlockVelocity = value; } }

        /// <summary>
        /// Determines whether this Platform will be controlled by the player. If true, the platform will only move when a character is on it. Otherwise it will move regardless.
        /// Note that the platform will still remain at stop speed until the player stands on it. 
        /// </summary>
        public bool Interactive { get { return _Interactive; } set { _Interactive = value; } }
        public BlockCollisionEffectMode BlockCollisionEffect { get { return _BlockCollisionEffect; } set { _BlockCollisionEffect = value; } }
        [Editor(typeof(GenericCollectionEditor<PointF>),typeof(UITypeEditor))]
        public List<PointF> StartSpeedStates
        {

            get { return _StartSpeedStates; }
            set { _StartSpeedStates = value; }
        }


        public PointF StartSpeed { get { return _StartSpeedStates[0]; } set { _StartSpeedStates[0] = value; } }
        [TypeConverter(typeof(FloatFConverter))]
        public PointF StopSpeed { get { return _StopSpeed; } set { _StopSpeed = value; } }
        public bool isMoving { get { return Math.Abs(BlockVelocity.X) > 0.1f || Math.Abs(BlockVelocity.Y) > 0.1f; } }
        public MovingPlatformBlock(RectangleF BlockRect)
            : base(BlockRect, BCBlockGameState.Imageman.getImageFramesString("PlatformBlock"))
        {

            
            
        }

        public MovingPlatformBlock(MovingPlatformBlock clonethis):base(clonethis)
        {
            StartSpeed = clonethis.StartSpeed;
            BlockVelocity = clonethis.BlockVelocity;
            BlockRectangle = clonethis.BlockRectangle;
            baseBlock.BlockRectangle = BlockRectangle;
        }
        public MovingPlatformBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            BlockVelocity = (PointF)info.GetValue("BlockVelocity", typeof(PointF));
          //  StartSpeed = (PointF)info.GetValue("StartSpeed", typeof(PointF));
            try { _StartSpeedStates = (List<PointF>)info.GetValue("StartSpeedStates", typeof(List<PointF>)); }catch { }
            try { _Interactive = info.GetBoolean("Interactive"); }catch { }
            try { _BlockCollisionEffect = (BlockCollisionEffectMode)info.GetValue("BlockCollisionEffect", typeof(BlockCollisionEffectMode)); }
            catch { }

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("BlockVelocity", BlockVelocity);
            info.AddValue("StartSpeed", StartSpeed);
            info.AddValue("Interactive", _Interactive);
            info.AddValue("BlockCollisionEffect", _BlockCollisionEffect);
        }
        public override object Clone()
        {
            return new MovingPlatformBlock(this);
        }
        public override bool MustDestroy()
        {
            return false;
        }
        public override bool RequiresPerformFrame()
        {
            
            return isMoving;
        }
        private bool TestWallHorizontal(RectangleF GameArea)
        {

            return BlockRectangle.Left < GameArea.Left || BlockRectangle.Right > GameArea.Right;

        }
        private bool TestWallVertical(RectangleF GameArea)
        {
            return BlockRectangle.Top < GameArea.Top || BlockRectangle.Bottom > GameArea.Bottom;

        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            return false;
        }
        const int CheckTrackingInterval = 50; //only check every 50 frames.
        int CheckTrack = 0;
        public override bool PerformFrame(BCBlockGameState gamestate)
        {

            //check tracking code.
            //this makes sure that a gameobject being removed while on top of a movingplatformblock doesn't "orphan" the block and make it keep going thinking that there is a 
            //object on it.
            CheckTrack++;
            if (CheckTrack == CheckTrackingInterval)
            {
                //get all the trackingobjects that are in our _TrackingObjects list that do not exist in the game.
                LinkedList<GameObject> madelist = new LinkedList<GameObject>();
                foreach (var iterate in from m in _TrackingObjects where !gamestate.GameObjects.Contains(m) select m)
                {
                    madelist.AddLast(iterate);

                }
                //we do it twice because we cannot change the enumerable being enumerated, as would be the case here since Standon will remove the PlatformObject from
                //_TrackingObjects and we were looping through it.
                foreach (var iterate in madelist)
                {
                    //fake it leaving the block.
                    this.Standon(gamestate, (PlatformObject)iterate, false);

                }
                

            }

            //move in the desired direction.
            //use IncrementLocation, too... for frame and all that.
            float gotmult = gamestate.GetMultiplier();
            PointF newlocation = BlockLocation;
            PointF twiggedspeed = new PointF(BlockVelocity.X * gotmult,BlockVelocity.Y*gotmult);
            BCBlockGameState.IncrementLocation(gamestate, ref newlocation, BlockVelocity);


            BlockLocation = newlocation;

            //now we check whether we are touching blocks in the directions we are moving.
            RectangleF toptest = new RectangleF(BlockRectangle.X, BlockRectangle.Y - twiggedspeed.Y, BlockRectangle.Width, twiggedspeed.Y);
            RectangleF bottomtest = new RectangleF(BlockRectangle.X, BlockRectangle.Y + BlockRectangle.Height, BlockRectangle.Width, twiggedspeed.Y);
            RectangleF LeftTest = new RectangleF(BlockRectangle.X - twiggedspeed.X, BlockRectangle.Top, twiggedspeed.X, BlockRectangle.Height);
            RectangleF RightTest = new RectangleF(BlockRectangle.Right, BlockRectangle.Top, twiggedspeed.X, BlockRectangle.Height);


            //first, test X...
            RectangleF? horztest=null;
            if (twiggedspeed.X > 0)
                horztest = RightTest;
            else if (twiggedspeed.X < 0)
                horztest = LeftTest;

            RectangleF? verttest = null;
            if(twiggedspeed.Y > 0)
                verttest = bottomtest;
            else if (twiggedspeed.Y < 0)
            {
                verttest = toptest;
            }


            //check for blocks in those rectangles.
            if (horztest != null)
            {
                List<Block> horztestresults = BCBlockGameState.Block_HitTest(gamestate.Blocks, horztest.Value, false);
                if (horztestresults.Count((w) => w != this) > 0 || TestWallHorizontal(gamestate.GameArea))
                {
                    if (BlockCollisionEffect == BlockCollisionEffectMode.CollisionEffect_Reverse)
                    {
                        BlockVelocity = new PointF(BlockVelocity.X * -1, BlockVelocity.Y);
                    }
                    else
                    {
                        AdvanceState();
                    }
                    BCBlockGameState.Soundman.PlaySound("SWITCH");
                }

            }

            if (verttest != null)
            {
                List<Block> verttestresults = BCBlockGameState.Block_HitTest(gamestate.Blocks, verttest.Value, false);
                if (verttestresults.Count((w) => w != this) > 0 || TestWallVertical(gamestate.GameArea))
                {
                    if (BlockCollisionEffect == BlockCollisionEffectMode.CollisionEffect_Reverse)
                    {
                        BlockVelocity = new PointF(BlockVelocity.X, BlockVelocity.Y * -1);
                        
                    }
                    else
                    {
                        AdvanceState();
                    }
                    BCBlockGameState.Soundman.PlaySound("SWITCH");
                }
            }




            return false;

        }
        bool istracking = false;
        /// <summary>
        /// called when a PlatformObject starts to "track" the block. This means it is standing on it, typically.
        /// </summary>
        /// <param name="gstate"></param>
        /// <param name="objStand"></param>
        public void Standon(BCBlockGameState gstate, PlatformObject objStand, bool standingon)
        {

            if (standingon) _TrackingObjects.Add(objStand);
            else _TrackingObjects.Remove(objStand);

            if (!_Interactive)
            {
                if (BlockVelocity == StopSpeed)
                {
                    AdvanceState();

                }
                return;
            }
            if (istracking == standingon) return;

            if (standingon)
            {
                AdvanceState();
            }
            else
                BlockVelocity = StopSpeed;



            gstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => gstate.Forcerefresh = true));
            BCBlockGameState.Soundman.PlaySound("SWITCH");
            istracking = standingon;
            
        }
        private void AdvanceState()
        {
            BlockVelocity = _StartSpeedStates[NextStartSpeed];
            NextStartSpeed = (NextStartSpeed + 1) % _StartSpeedStates.Count;
        }
    }


    [Serializable]
    [MaliceCategory]
    [BlockDescription("That's a very nice Paddle you have there...")]
    public class CreeperBlock : AnimatedImageBlock, ISerializable, ICloneable
    {
        private int numsprays = 1;
        private PointF CreepVelocity = new PointF(0, 0);
        private bool activated = false;
        private const int MaxHP = 10;
        private int HP = MaxHP; //number of hits to destroy, after being hit the first time.
        public CreeperBlock(RectangleF BlockRect)
            : base(BlockRect, BCBlockGameState.Imageman.getImageFramesString("CreeperBlock"))
        {


        }
        public CreeperBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {


        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        public CreeperBlock(CreeperBlock Clonethis)
            : base(Clonethis)
        {



        }
        public override bool MustDestroy()
        {
            return true;
        }
        
        public override object Clone()
        {
            return new CreeperBlock(this);
        }
        public override bool CheckImpact(BCBlockGameState currentgamestate, cBall hitball, out bool washit, ref List<cBall> ballsadded)
        {
            bool washit1, washit2;
            //bool retval = baseBlock.CheckImpact(currentgamestate, hitball, out washit1, ref ballsadded) | base.CheckImpact(currentgamestate, hitball, out washit2, ref ballsadded);
            bool retval = base.CheckImpact(currentgamestate, hitball, out washit2, ref ballsadded);
            //aggregate the two return values into one...
//            washit = washit1 || washit2;
            washit = washit2;
            if (retval || washit)
            {
                PerformBlockHit(currentgamestate, hitball);


            }
            washit = washit && HP <= 0;
            return retval;
        }
        protected override void StandardSpray(BCBlockGameState parentstate, cBall ballhit)
        {
            //do nothing....
        }
        protected override Particle AddStandardSprayParticle(BCBlockGameState parentstate, cBall ballhit)
        {
            //return base.AddStandardSprayParticle(parentstate, ballhit);
            //mwa haha, do nothing...
            return null;
        }
        private void SprayDust(BCBlockGameState gamestate)
        {
            var rgg = BCBlockGameState.rgen;
            for (int i = 0; i < 30; i++)
            {
                PointF addposition = new PointF(BlockRectangle.Left+(float)(BlockRectangle.Width * rgg.NextDouble()), 
                    BlockRectangle.Top+ (float)(BlockRectangle.Height * rgg.NextDouble()));

                DustParticle spawnparticle = new DustParticle(addposition, 3);
                gamestate.Particles.Add(spawnparticle);
            }


        }
        private void sprayStandard(BCBlockGameState gamestate)
        {

            for (int i = 0; i < 50; i++)
                gamestate.Particles.Add(base.AddStandardSprayParticle(gamestate, null));


        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            //return base.PerformBlockHit(parentstate, ballhit, ref ballsadded);
            bool nodef=false;
            bool retval1 = RaiseBlockHit(parentstate, ballhit, ref nodef);
            if (!activated)
            {
                Debug.Print("PerformBlockHit:: Activated...");
                parentstate.Forcerefresh=true;

                activated = true;

                return false;
            }
            else
            {
                HP--;
                if(HP<=0)
                {
                    BCBlockGameState.Soundman.PlaySoundRnd("creepdeath");
                    SprayDust(parentstate);
                    parentstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                    parentstate.GameObjects.AddLast(new ProxyObject(delegateremoveblock, null))));
                return false;
               // return base.PerformBlockHit(parentstate, ballhit, ref ballsadded);
                }
                else
                {
                    return false;
                }
            }
            



        }
        public override bool RequiresPerformFrame()
        {
            //return base.RequiresPerformFrame();
            return activated;


        }
        private bool delegateremoveblock(ProxyObject sourceobject,BCBlockGameState gamestate)
        {
            gamestate.Forcerefresh = true;
            for(int i =0;i<numsprays;i++)
                StandardSpray(gamestate);

            

            gamestate.Blocks.Remove(this);
            return true;
        }
        public override void Draw(Graphics g)
        {
            base.Draw(g);

            float percentdamage = 1 - ((float)HP / (float)MaxHP);
            int usealpha = (int)(percentdamage * 255);
            if(usealpha < 0) usealpha = 0;
            if (usealpha > 255) usealpha = 255;
            g.FillRectangle(new SolidBrush(Color.FromArgb(usealpha, Color.Red)), BlockRectangle);

        }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            //return base.PerformFrame(gamestate);
            if (activated)
            {
                //move towards the paddle.
               // Debug.Print("Performing active animation...");
                if (gamestate.PlayerPaddle != null)
                {
                    
                    //check if the block is intersecting the paddle.
                    if (gamestate.PlayerPaddle.Getrect().IntersectsWith(BlockRectangle.ToRectangle()))
                    {
                        //do damage to the paddle, and Explode the creeper block.
                        //gamestate.PlayerPaddle.HP -= 30; //commented out, the explosion item will be changed so
                        //that it can do damage instead.

                        //to destroy, create a proxy game object.

                        numsprays=3;
                        //for (int i = 0; i < numsprays; i++)
                            sprayStandard(gamestate);

                        BCBlockGameState.Soundman.PlaySoundRnd("explode");
                        gamestate.CreateExplosion(CenterPoint(), 64);
                        //sprayStandard(gamestate);
                        gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                        gamestate.GameObjects.AddLast(new ProxyObject(delegateremoveblock, null))));




                    }

                    if (gamestate.PlayerPaddle != null)  //can be null if we reduced the HP below zero earlier.
                    {
                        PointF paddlepos = gamestate.PlayerPaddle.Position;
                        PointF ourpos = CenterPoint();

                        PointF newspeed = BCBlockGameState.GoTowardsPoint(ourpos, paddlepos, 1);

                        CreepVelocity = new PointF((newspeed.X + CreepVelocity.X)/2, (newspeed.Y + CreepVelocity.Y)/2);
                    }


                }



                BlockLocation = new PointF(BlockLocation.X + CreepVelocity.X, BlockLocation.Y + CreepVelocity.Y);
                //baseBlock.BlockLocation=BlockLocation;
                baseBlock.BlockRectangle=BlockRectangle;
                return true;
            }
            return false;
        }

    }


    //vomit block:
    //how it works:
    //block hits it, block "eats" the ball. (all other balls hitting it in this period bounce off and do nothing).
    //The block will get redder, until finally it explodes, releasing three balls in random directions.
    [Serializable]
    
    public class VomitBlock : AnimatedImageBlock,ISerializable,ICloneable 
    {
        public enum VomitModeConstants
        {
            Vomit_Once, //fires the balls, and then is destroyed. MustDestroy will return true.
            Vomit_Multi // each time it fires, balls will be "vomited" and the block will "reset" to the standard state.

        }
        private VomitModeConstants mVomitMode = VomitModeConstants.Vomit_Once;
        public VomitModeConstants VomitMode { get { return mVomitMode; } set { mVomitMode = value; } }
            private DateTime? BallHitTime; //time ball hit. If null, no ball hit us. (duh)
        private readonly TimeSpan BallPurgeDelay = new TimeSpan(0, 0, 3); //three seconds to "purge"
        private float mVomitAmount = 3;

        /// <summary>
        /// ballswithin: when we are set to Vomit_Multi mode, we will shoot balls, but not "deactivate" or destroy ourselves.
        /// therefore, we will need to know which balls we spawned inside ourself so that we can effectively force-ignore them and not collide with them.
        /// </summary>
        private List<cBall> ballswithin = new List<cBall>(); 



        internal float PercentPurge { get { if (BallHitTime == null) return 0; else return (float)(DateTime.Now-BallHitTime.Value).TotalMilliseconds / (float)(BallPurgeDelay.TotalMilliseconds); } }


        public override bool DoesBallTouch(cBall hitball)
        {
            if (ballswithin.Contains(hitball))
            {
                //if the ball is no longer touching the block, remove it from the list.
                if (!base.DoesBallTouch(hitball))
                {
                    hitball.Location = new PointF(hitball.Location.X + hitball.Velocity.X, hitball.Location.Y + hitball.Velocity.Y);
                    ballswithin.Remove(hitball);

                }
                return false;


            }
            else
            {
                bool returnvalue = base.DoesBallTouch(hitball);
                if(returnvalue) hitball.DrawColor=Color.Yellow;
                return returnvalue;
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("VomitMode", mVomitMode);
            info.AddValue("VomitAmount", mVomitAmount);
        }
        public override bool MustDestroy()
        {
            //return base.MustDestroy();
            return mVomitMode == VomitModeConstants.Vomit_Once;
        }
        public VomitBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            try{mVomitMode = (VomitModeConstants)info.GetValue("VomitMode", typeof(VomitModeConstants));}catch{mVomitMode = VomitModeConstants.Vomit_Once;}
            try { mVomitAmount = info.GetInt32("VomitAmount"); }
            catch { mVomitAmount = 3; }
        }
            public override Object Clone()
            {

                return new VomitBlock(this);


            }

        public VomitBlock(VomitBlock clonethis):base(clonethis)
        {
            
            baseBlock = (Block)clonethis.baseBlock.Clone();
            ((ImageBlock)baseBlock).BlockImageKey = AniFrameKeys[currentframe];
            ((ImageBlock)baseBlock).hasChanged = true;
            this.VomitMode = clonethis.VomitMode;

        }

        public VomitBlock(RectangleF Blockrect):base(Blockrect,BCBlockGameState.Imageman.getImageFramesString("CrazyBlock"))
        {

            ((ImageBlock)baseBlock).BlockRectangle = Blockrect;
            ((ImageBlock)baseBlock).BlockImageKey = AniFrameKeys[currentframe];
            ((ImageBlock)baseBlock).hasChanged = true;   


        }
        public override bool RequiresPerformFrame()
        {
            return BallHitTime!=null; //we need performframe if our ballhittime has initialized...
          //  return true;
        }
        private void bloodemitter(cBall onball,BCBlockGameState bgamestate)
        {
            for (int i = 0; i < 5; i++)
            {
                Random rg = BCBlockGameState.rgen;
                PointF usetemp = new PointF(((float) rg.NextDouble()*5f) - 2.5f, 0);
                PointF userandomvelocity = new PointF(usetemp.X + onball.Velocity.X/3, usetemp.Y + onball.Velocity.Y/3);
                Particle addedpart = new WaterParticle(onball.Location, userandomvelocity, Color.Red);
                
                bgamestate.Particles.Add(addedpart);
            }

        }

        private void ShootRandom(BCBlockGameState gamestate,PointF pLocation, float TotalSpeed)
        {
            //choose random direction...
            float angleuse = (float)(BCBlockGameState.rgen.NextDouble() * (2 * Math.PI));
            
            //create the vector of that direction...
            PointF usespeed = new PointF((float)(Math.Cos(angleuse) * TotalSpeed), (float)(Math.Sin(angleuse) * TotalSpeed));


            cBall createball = new cBall(pLocation, usespeed);
            createball.Radius = 2.5f;


            //iBallBehaviour createbehaviour = new ParticleEmitterBehaviour(bloodemitter);

            //createball.Behaviours.Add(createbehaviour);
            gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(()=>gamestate.Balls.AddLast(createball)));
            //add the leapfrog behaviour...
            //var lfrog = new LeapFrogExploderBehaviour();
            //lfrog.NumExplosions = 1;
            //createball.Behaviours.Add(lfrog);
            //also add lineargravity...
            createball.Behaviours.Add(new LinearGravityBallBehaviour(0.2));
            createball.Behaviours.Add(new ParticleEmitterBehaviour((p, x) => x.Particles.Add(new WaterParticle(p.Location, p.Velocity, Color.Red))));
            ballswithin.Add(createball);



        }
        private bool delegateremoveblock(ProxyObject sourceobject,BCBlockGameState gamestate)
        {
            gamestate.Forcerefresh =true;
            gamestate.Blocks.Remove(this);
            return true;
        }

        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            //base.PerformFrame(gamestate);
            //return base.PerformFrame(gamestate);
            
            //idiot check...
            if(BallHitTime==null) return true; //true should be "don't destroy me"



            //otherwise, check if DateTime-BallHitTime is larger than BallPurgeDelay...

            if (DateTime.Now - BallHitTime > BallPurgeDelay)
            {
                //it is. blow out three balls in random directions.
                Debug.Print("Vomit block kerploding...");
                for (int i = 0; i < mVomitAmount; i++)
                {
                    ShootRandom(gamestate, CenterPoint(), 3);
                }
                BCBlockGameState.Soundman.PlaySound("blech");

                //add a proxy object to remove this block.
                if (mVomitMode == VomitModeConstants.Vomit_Once)
                {
                    gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                    gamestate.GameObjects.AddLast(new ProxyObject(delegateremoveblock, null))));
                    StandardSpray(gamestate);
                }
                else if (mVomitMode==VomitModeConstants.Vomit_Multi)
                {
                    Debug.Print("Vomit_Multi: PerformFrame is returning false.");
                    BallHitTime=null; //null it out to revert.
                    return false;
                }
                return true;


            }

            return true;

            //task


        }
        protected override Particle AddStandardSprayParticle(BCBlockGameState parentstate, cBall ballhit)
        {
            //return base.AddStandardSprayParticle(parentstate, ballhit);
            //does nothing...
            return null;
        }
        protected override void StandardSpray(BCBlockGameState parentstate, cBall ballhit)
        {
            //base.StandardSpray(parentstate, ballhit);
            //special standardspray.
            
            if(BallHitTime==null) return; 

            //will need to repaint...
            hasChanged = true;
            Random rg = BCBlockGameState.rgen;
            //instead of showing bits of our block, we will explode into a shower of gibs. Gibs for all!
            for (int i = 0; i < 50; i++)
            {
                //choose a random location.
                PointF randomspot = new PointF((float)(rg.NextDouble() * BlockRectangle.Width + BlockRectangle.Left), (float)(rg.NextDouble() * BlockRectangle.Height + BlockRectangle.Top));
                //addparticle = new WaterParticle(
                PointF userandomvelocity = new PointF(((float)rg.NextDouble() * 5f) - 2.5f, 0);
                Particle addpart = new WaterParticle(randomspot, userandomvelocity, Color.Red);
                parentstate.Particles.Add(addpart);
            }



        }
        public override bool CheckImpact(BCBlockGameState currentgamestate, cBall hitball, out bool washit, ref List<cBall> ballsadded)
        {
            washit = false;
            bool tempout;
            return base.CheckImpact(currentgamestate, hitball, out tempout, ref ballsadded);
            
            
        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            Debug.Print("VomitBlock detected blockhit...");
            if (ballswithin.Contains(ballhit))
            {
                return false;


            }
            hasChanged=true;
            bool nodef=false;
            bool retval1 = RaiseBlockHit(parentstate, ballhit, ref nodef);
            //bool retval2 = baseBlock.PerformBlockHit(parentstate, ballhit, ref ballsadded);
            bool retval = retval1;
            if (BallHitTime != null)
            {
                return false;
                    //if we have already been hit, this will have a value; block hits during the time we are hit and the time we finally explode 
                //bounce off harmlessly.
            }
            else
            {
                BallHitTime = new DateTime?(DateTime.Now);
                return false;
                //return retval;
            }  
        }
        protected override void CreateOrbs(PointF Location, BCBlockGameState gstate)
        {
            return; //NOTHING...
        }
        public override void Draw(Graphics g)
        {
            //base.Draw(g);
            base.Draw(g);
            if (BallHitTime != null)
            {
                float percentgreen = PercentPurge;

                int usealpha = (int)(percentgreen * 255f);
                //Debug.Print("VomitBlock: using alpha:" + usealpha);
                usealpha = BCBlockGameState.ClampValue(usealpha, 0, 255);
                g.FillRectangle(new SolidBrush(Color.FromArgb(usealpha, 0, 255, 0)), baseBlock.BlockRectangle);






            }



        }



    }



    [Serializable]
    [MaliceCategory]
    public class DemonBlock : AnimatedImageBlock,ISerializable 
    {
        int frameshootdelay = 50+((int)(BCBlockGameState.rgen.NextDouble()*50));
        int framecount = 0;
        
        public DemonBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {

            frameshootdelay = info.GetInt32("frameshootdelay");
            currentframe = 1;
            ((ImageBlock)baseBlock).BlockImageKey = AniFrameKeys[currentframe];
            ((ImageBlock)baseBlock).hasChanged = true;    

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("frameshootdelay", frameshootdelay);

        }
        public DemonBlock(DemonBlock clonethis):base(clonethis)
        {
            frameshootdelay = clonethis.frameshootdelay;
            baseBlock = (Block)clonethis.baseBlock.Clone();
            currentframe = clonethis.currentframe;
            ((ImageBlock)baseBlock).BlockImageKey = AniFrameKeys[currentframe];
            ((ImageBlock)baseBlock).hasChanged = true;    

        }

        public DemonBlock(RectangleF blockrect)
            : base(blockrect, BCBlockGameState.Imageman.getImageFramesString("DemonBlock"))
        {
            ((ImageBlock)baseBlock).BlockRectangle=blockrect;
            ((ImageBlock)baseBlock).BlockImageKey = AniFrameKeys[currentframe];
            ((ImageBlock)baseBlock).hasChanged = true;    

        }
        public override object Clone()
        {
            return new DemonBlock(this);
        }
        public override bool RequiresPerformFrame()
        {
          return true;
        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            base.PlayBlockSound(ballhit, "blach");
            return base.PerformBlockHit(parentstate, ballhit);
        }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            //base.PerformFrame(gamestate);
            if(gamestate.ClientObject.ActiveState  is StateRunning) framecount++;
            if (framecount == frameshootdelay)
            {
                currentframe = 1;
                ((ImageBlock)baseBlock).BlockImageKey = AniFrameKeys[currentframe];
                ((ImageBlock)baseBlock).hasChanged = true;
                return true;
                
            }
            else if (framecount == (frameshootdelay + (frameshootdelay / 2)))
            {
                Shootfireball(gamestate);
                currentframe = 0;
                framecount = 0;
                ((ImageBlock)baseBlock).BlockImageKey = AniFrameKeys[currentframe];
                ((ImageBlock)baseBlock).hasChanged = true;    
                return true;
            }

            return true;
        }
        private void Shootfireball(BCBlockGameState gamestate)
        {
            //Debug.Print("DemonBlock:Shootfireball()");

            float randomspeed = 2;
            randomspeed += (float)(BCBlockGameState.rgen.NextDouble() * 4);
            double genned=BCBlockGameState.rgen.NextDouble();


            if (genned > 0.6)
            {
                gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                gamestate.GameObjects.AddLast(new Fireball(gamestate, this, randomspeed))));
            }
            else
            {
                gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(()=>gamestate.GameObjects.AddLast(new BlueFireball(gamestate, this, randomspeed))));

            }
            BCBlockGameState.Soundman.PlaySound("spitfire",1.0f);
        }

    }

    [Serializable]
    [PowerupEffectCategory]
    public class SpeedEffectBlock : PassiveEffectBlock, ISerializable
    {

        public enum SpeedEffectTypeConstants
        {
            SpeedEffect_Add,
            SpeedEffect_Multiply,
            SpeedEffect_Direct,
            SpeedEffect_Angle

        }
        private SpeedEffectTypeConstants _effecttype;
        public SpeedEffectTypeConstants EffectType { get { return _effecttype; } set { _effecttype = value; } }


        public static Image DrawArrow(Size DrawSize)
        {

            return DrawArrow(DrawSize, new Pen(Color.Black), new SolidBrush(Color.Red), new SolidBrush(Color.Transparent));

        }

        /// <summary>
        /// Creates Right-pointing arrow of the given size, using the specified border, background, and fill.
        /// </summary>
        /// <param name="DrawSize"></param>
        /// <param name="BorderPen"></param>
        /// <param name="BackgroundBrush"></param>
        /// <param name="FillBrush"></param>
        /// <returns></returns>
        public static Image DrawArrow(Size DrawSize, Pen BorderPen, Brush BackgroundBrush, Brush FillBrush)
        {
            //draws the arrow pointing Right- always. This image is then rotated by the class
            //based on the vector of the Effect...
            int w = DrawSize.Width;
            int h = DrawSize.Height;


            //First: create the bitmap, and the graphics object we will draw to...
            Bitmap arrowimage = new Bitmap(DrawSize.Width, DrawSize.Height);
            Graphics g = Graphics.FromImage(arrowimage);
            //clear it in the background color...
            g.Clear(Color.Transparent);
            g.FillRectangle(BackgroundBrush,0,0,DrawSize.Width,DrawSize.Height);

            //now create the path of the arrow...

            GraphicsPath arrowpath = new GraphicsPath();
            arrowpath.AddLine(w,h/2,0,0);
            arrowpath.AddLine(0,0,w/2,h/2);
            arrowpath.AddLine(w/2,h/2,0,h);
            //arrowpath.AddLine(0,h,w,h/2);
            arrowpath.CloseFigure();

            //now, paint the arrow and draw the outline...
            g.FillPath(FillBrush,arrowpath);
            g.DrawPath(BorderPen,arrowpath);


            //complete! return the image.
            return arrowimage;


        }
        /*
        public static String GetRotatedTexture(double currangle)
        {
            //returns the key of the texture that should be applied to this SpeedEffectBlock.
            //format: "speedF<degree>" where degree is the degree of the vector angle of this blocks "Effect" vector.
            
            //step one: convert to degrees...
            int degreeamount = (int)(currangle / (Math.PI / 180));
            String usekey = "speedF" + degreeamount.ToString();

            //if the image exists, simply return the key.
            if(BCBlockGameState.Imageman.Exists(usekey)) return usekey;
            //otherwise, we need to create a new image.
            Image drawtexture = new Bitmap(128,64);
            Graphics drawgraph = Graphics.FromImage(drawtexture);
            //draw to that graphics object...
            //LinearGradientBrush lgb = new LinearGradientBrush(new PointF(0, 0), new PointF((float)Math.Cos(currangle) * 5, (float)Math.Sin(currangle) * 5), Color.Red, Color.Green);
            LinearGradientBrush lgb = new LinearGradientBrush(new PointF(0, 0), new PointF(50, 50), Color.Red, Color.Blue);
            drawgraph.FillRectangle(lgb, 0, 0, 128, 64);
            

            //done, add the image to the image manager with the proper key...
            BCBlockGameState.Imageman.AddImage(usekey, drawtexture);
            return usekey;

            



        }
         * 
        public static String GetRotatedTexture()
        {
            return GetRotatedTexture(0);

        }
        */
        private PointF _SpeedEffect=PointF.Empty;
        [TypeConverter(typeof(FloatFConverter))]
        public PointF SpeedEffect
        {
            get { return _SpeedEffect; }
            set
            {
                _SpeedEffect = value;
                base.TextureOriginSpeed = _SpeedEffect; //make the texture animate at that speed.
                refreshtexture();
                hasChanged=true;
            }
        }
        public override bool RequiresPerformFrame()
        {
            return true; //no need to reload frames...
        }
        public override object Clone()
        {
            return new SpeedEffectBlock(this);
        }
        private void DoAffectSpeed(cBall balleffect)
        {
            switch (EffectType)
            {
                case SpeedEffectTypeConstants.SpeedEffect_Add:
                    Debug.Print("add");
                    balleffect.Velocity = new PointF(balleffect.Velocity.X + SpeedEffect.X,
                                                     balleffect.Velocity.Y + SpeedEffect.Y);

                    break;
                case SpeedEffectTypeConstants.SpeedEffect_Multiply:
                    Debug.Print("multiply");
                    balleffect.Velocity = new PointF(balleffect.Velocity.X*SpeedEffect.X,
                                                     balleffect.Velocity.Y*SpeedEffect.Y);
                    break;
                case SpeedEffectTypeConstants.SpeedEffect_Direct:
                    Debug.Print("direct");
                    balleffect.Velocity= new PointF(SpeedEffect.X,SpeedEffect.Y);
                    break;
                case SpeedEffectTypeConstants.SpeedEffect_Angle:
                    {
                        double speedangle = BCBlockGameState.GetAngle(new PointF(0, 0), SpeedEffect);
                        float ballspeed = balleffect.getMagnitude();
                        balleffect.Velocity = new PointF((float)Math.Cos(speedangle) * ballspeed, (float)Math.Sin(speedangle) * ballspeed);




                    }
                    break;
            }
            BCBlockGameState.Soundman.PlaySound("Switch");

        }

        public override void Draw(Graphics g)
        {
            //Debug.Print("Draw");

            if (usebrush == null)
            {
                refreshtexture();

            }
            g.FillRectangle(usebrush, BlockRectangle);

            //base.Draw(g);
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("SpeedEffect", SpeedEffect);
            info.AddValue("EffectType", _effecttype);
        }

        private TextureBrush usebrush;

        public void refreshtexture()
        {
            //first calculate angle of vector.
            double useangle = BCBlockGameState.GetAngle(new PointF(0, 0), _SpeedEffect);
            //AniFrameKeys = new String[] {  };
            usebrush = new TextureBrush(DrawArrow(new Size((int)(BlockRectangle.Width / 2), (int)(BlockRectangle.Height / 2))),
                WrapMode.Tile);

            float usea = (float)(useangle/(Math.PI/180));
            
            usebrush.RotateTransform(usea);
            usebrush.TranslateTransform(_TextureOrigin.X, _TextureOrigin.Y);
            //done...



        }
        
        public SpeedEffectBlock(RectangleF Blockrect)
            : base(Blockrect, new String[] {"slimeball"}, new TimeSpan(0, 0, 0, 0, 1000))
        {



        }
        public SpeedEffectBlock(SpeedEffectBlock cloneme):base(cloneme)
        {
            SpeedEffect = cloneme.SpeedEffect;
            EffectType = cloneme.EffectType;

        }
        
        public SpeedEffectBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            SpeedEffect = (PointF)info.GetValue("SpeedEffect", typeof(PointF));

            _effecttype = (SpeedEffectTypeConstants)info.GetValue("EffectType", typeof(SpeedEffectTypeConstants));
            

        }


        public override void PassiveEffect(cBall hitball)
        {
            DoAffectSpeed(hitball);
        }
    }



    [Serializable]
    public class PowerupCycleBlock : AnimatedImageBlock, ISerializable
    {
        public static bool generated=false;
        public static Image GetPowerupImage(Type Poweruptype)
        {
            Bitmap drawbitmap = new Bitmap(128, 64);
            Graphics drawto = Graphics.FromImage(drawbitmap);
            drawto.Clear(Color.Transparent);
            //for now it's just a red box. this ought to have it's own special image though...
            drawto.FillRectangle(new SolidBrush(Color.Red), 0, 0, drawbitmap.Width, drawbitmap.Height);
            drawto.DrawRectangle(new Pen(Color.Black), 0, 0, drawbitmap.Width, drawbitmap.Height);
            //first, attempt to instantiate it...)
            try
            {
                GamePowerUp instantiate = (GamePowerUp)Activator.CreateInstance(Poweruptype,new Object[]{new PointF(32,16),new SizeF(64,32)});
                //
                //instantiated... now we need to draw this as well.
                instantiate.Draw(drawto);
                //well that was easy...
                return drawbitmap;
                
            }
            catch
            {
                return null;
            }




        }
        private static String[] genimages;
        private static Type[] _powertypes = null;
        public static String[] getPowerupImageFrames(out Type[] poweruptypes)
        {
            poweruptypes=null;
            //generate new block frames if necessary.
            if (!generated || _powertypes==null)
            {
                generated = true;
                //generate!
                //our frames will go by the names "Cycle<powerupname>Block".
                LoadedTypeManager powerupmanager = BCBlockGameState.MTypeManager[typeof(GamePowerUp)];
                //using that powerupmanager, iterate through each powerup.
                List<Type> gottypes = new List<Type>();
                List<string> addedimages = new List<string>();
                int i = 0;
                foreach (Type poweruptype in powerupmanager.ManagedTypes)
                {
                    //we need to create an image for this powerup.
                    Image gotpic = GetPowerupImage(poweruptype);
                    if (gotpic != null)
                    {
                        gottypes.Add(poweruptype);
                        String imagekey=("Cycle" + poweruptype.Name + "Block").ToUpper();
                        addedimages.Add(imagekey);
                        //also add to the image manager.
                        BCBlockGameState.Imageman.AddImage(imagekey, gotpic);



                    }
                }
                
                _powertypes = gottypes.ToArray();
                poweruptypes = _powertypes;
                genimages = addedimages.ToArray(); 
                return genimages;
                //pointF Location SizeF size




            }
           //poweruptypes = _powertypes;
            
           return genimages;

        }

        public PowerupCycleBlock(RectangleF rectangle)
            : base(rectangle, getPowerupImageFrames(out _powertypes), new TimeSpan(0, 0, 0, 0, 500), null)
        {

            AniFrameKeys = getPowerupImageFrames(out _powertypes);

        }
        public override void Draw(Graphics g)
        {
            base.Draw(g);
        }
        public PowerupCycleBlock(PowerupCycleBlock cloneit):base(cloneit)
        {
            AniFrameKeys = getPowerupImageFrames(out _powertypes);


        }
        public PowerupCycleBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {

            AniFrameKeys = getPowerupImageFrames(out _powertypes);
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            
        }
        public override object Clone()
        {
            return new PowerupCycleBlock(this);
        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {

            Debug.Print("PerformBlockHit occured in frame #" + currentframe);
            if(_powertypes==null) AniFrameKeys = getPowerupImageFrames(out _powertypes);
            //create _powertypes[currentframe]...
            if (_powertypes[currentframe] == null)
            {
                Debug.Print("break...");

            }

            GamePowerUp createpowerup = (GamePowerUp)(Activator.CreateInstance(_powertypes[currentframe], CenterPoint(), new SizeF(18, 9)));
            parentstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(()=>
            parentstate.GameObjects.AddLast(createpowerup)));

            return true;


        }

    }





    [Serializable]
    [PowerupEffectCategory]
    public class GamePowerupBlock : AnimatedBlock
    {
        protected GamePowerUp mUsePowerup;

       

        public GamePowerupBlock(GamePowerupBlock clonethis):base(clonethis)
        {

            mUsePowerup = clonethis.mUsePowerup;

        }
        public GamePowerupBlock(RectangleF pblockRectangle):this(pblockRectangle,typeof(StickyPaddlePowerUp))
        {


        }
        public GamePowerupBlock(SerializationInfo info,StreamingContext context):base(info,context)
        {
            Type usetype = info.GetValueType("Type");
            Instantiate(usetype);


        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValueType("Type", mUsePowerup.GetType());


        }
        protected void Instantiate(Type pType)
        {
           //otherwise, instantiate it; all GamePowerUp classes are supposed to support a constructor like:
            //(PointF,Size)
            PointF powerupposition = new PointF((BlockRectangle.Left+(BlockRectangle.Width/2))-GamePowerUp.defaultSize.Width/2,
                (BlockRectangle.Top + (BlockRectangle.Height/2)-(GamePowerUp.defaultSize.Height/2)));
            


            Object createit = Activator.CreateInstance(pType, new object[] { (object)powerupposition, (object)GamePowerUp.defaultSize});



        }
        public GamePowerupBlock(RectangleF pblockrectangle,Type usePowerup)
        {
            BlockRectangle=pblockrectangle;
            if(!usePowerup.IsSubclassOf(typeof(GamePowerUp)))
            {
                throw new ArgumentException("Specified Powerup is not a powerup.");

                Instantiate(usePowerup);

            }
         

        }


        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            //
            return false;
        }

        public override object Clone()
        {
            return new GamePowerupBlock(BlockRectangle);
        }
    }
    [Serializable]
    public class WaterBlock : AnimatedImageBlock
    {
        public WaterBlock(WaterBlock clonethis)
            : base(clonethis)
        {


        }

        public WaterBlock(RectangleF blockrect)
            : base(blockrect, BCBlockGameState.Imageman.getImageFramesString("Water"), new TimeSpan(0, 0, 0, 0, 100),null)
        {



        }
        public WaterBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            


        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            //play SPLASH.
            BCBlockGameState.Soundman.PlaySound("SPLASH", 0.9f);
            Random rgen = BCBlockGameState.rgen;        
                PointF middlespot = new PointF((float)BlockRectangle.Left + (BlockRectangle.Width / 2), (float)BlockRectangle.Top + (BlockRectangle.Height / 2));
                
//            for (int i = 0; i < (((BlockRectangle.Width*BlockRectangle.Height)/.75f)*(BCBlockGameState.ParticleGenerationFactor)   ; i++)
            for (int i = 0; i < (((BlockRectangle.Width*BlockRectangle.Height)/.75f)*(0.05f)); i++)
            {
                //old code creates particles... new code uses a shiteload of balls instead.
                
                PointF userandomposition = new PointF(BlockRectangle.Left +(BlockRectangle.Width*(float)rgen.NextDouble()),
                    BlockRectangle.Top + (BlockRectangle.Height*(float)rgen.NextDouble()));
                PointF userandomvelocity = new PointF(((float)rgen.NextDouble()*5f)-2.5f,0);

               // if (BCBlockGameState.rgen.NextDouble() < 0.75)
               // {
               //     parentstate.Particles.Add(new WaterParticle(userandomposition, userandomvelocity));
               // }
               // else
               // {
                    parentstate.Particles.Add(new BubbleParticle(userandomposition, userandomvelocity));
               // }
                /*
                PointF userandomposition = new PointF(BlockRectangle.Left + (BlockRectangle.Width * (float)rgen.NextDouble()),
                    BlockRectangle.Top + (BlockRectangle.Height * (float)rgen.NextDouble()));
                PointF userandomvelocity = new PointF(((float)rgen.NextDouble() * 5f) - 2.5f, 0);
                cBall createball = new cBall(userandomposition, userandomvelocity);
                createball.Behaviours.Add(new WaterBallBehaviour());
                createball.Behaviours.Add(new TempBallBehaviour());
                createball.Behaviours.Add(new LinearGravityBallBehaviour(0.04));
                createball.Radius = 1;
                createball.DrawColor=Color.Blue;
                createball.DrawPen = new Pen(Color.Blue, 1);
                parentstate.Balls.AddLast(createball);*/
            }



            return true;
        }
        public override object Clone()
        {
            return new WaterBlock(BlockRectangle);
        }
        public override bool RequiresPerformFrame()
        {
            if(BCBlockGameState.WaterBlockAnimations)
                return base.RequiresPerformFrame();
            else
                return false;
                
            
        }

    }







}
    