using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.Serialization;
using BASeBlock.Particles;

namespace BASeBlock.Blocks
{
    [Serializable]
    public class BuilderBlock : ImageBlock
    {
        


        //the "Builder" Block creates new blocks, moving existing blocks away from it.
        public enum BuilderBlockDirection
        {
            Left,
            Up,
            Right,
            Down

        }
        private Type _BuildBlock = typeof(StrongBlock);
        private int _MaxBuilds = 12;
        private int _Built = 0;
        private bool _Initialized = false;
        


        private BuilderBlockDirection _BuildDirection = BuilderBlockDirection.Up;
        [Editor(typeof(ItemTypeEditor<Block>),typeof(UITypeEditor))]
        public Type BuildBlock { get { return _BuildBlock; } set { _BuildBlock = value; } }
        

        /// <summary>
        /// sets/returns the Maximum builds this block can perform. If 0, this block
        /// will never 'run out' of blocks to build.
        /// </summary>
        public int MaxBuilds { get { return _MaxBuilds; } set { _MaxBuilds = value; } }
        /// <summary>
        /// Number of blocks this block has Built, or emitted. This does not count blocks that were in the builddirection
        /// when the block initialized.
        /// </summary>
        public int Built { get { return _Built; } set { _Built = value; } }


        public BuilderBlockDirection BuildDirection { get { return _BuildDirection; } set { _BuildDirection = value; } }
        public bool Initialized { get { return _Initialized; } }
        public BuilderBlock(RectangleF Blockrect)
            : base(Blockrect,"BUILDER")
        {

        }
        public BuilderBlock(BuilderBlock clonethis):base(clonethis)
        {
            _MaxBuilds = clonethis.MaxBuilds;
            _Built = clonethis.Built;
            _BuildDirection = clonethis.BuildDirection;
            _Initialized = clonethis.Initialized;
        }

        public BuilderBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            _MaxBuilds = info.GetInt32("MaxBuilds");
            _Built = info.GetInt32("Built");
            _BuildDirection = info.GetValue<BuilderBlockDirection>("BuildDirection");
            try {_BuildBlock= BCBlockGameState.FindClass(info.GetString("BuildBlock"));} catch(Exception exx) { _BuildBlock = typeof(NormalBlock);}
        }
        public override object  Clone()
        {
            return new BuilderBlock(this);
        }
        public override void  GetObjectData(SerializationInfo info, StreamingContext context)
        {
 	     base.GetObjectData(info, context);
         info.AddValue("MaxBuilds", _MaxBuilds);
         info.AddValue("Built", _Built);
         info.AddValue("BuildDirection", _BuildDirection);
         info.AddValue("BuildBlock", _BuildBlock.Name);
        }

        public override bool MustDestroy()
        {
            //true for when will break.
            return _MaxBuilds > 0; //bigger than 0 will break.
        }
     
      
        /// <summary>
        /// determine if the given block is relevant... or pushable.
        /// </summary>
        /// <param name="ourmover"></param>
        /// <param name="testblock"></param>
        /// <returns></returns>
        private bool isRelevant(Block ourmover, Block testblock,PointF offset)
        {
            PointF OurPoint = ourmover.BlockRectangle.CenterPoint();
            PointF OtherPoint = testblock.BlockRectangle.CenterPoint();
            //the new block has to be in the x or y direction of our offset.
            PointF diff = new PointF(OtherPoint.X - OurPoint.X, OtherPoint.Y - OurPoint.Y);
            Point Signs = new Point(Math.Sign(diff.X), Math.Sign(diff.Y));

            return ((Math.Sign(offset.X)!=0 && Signs.X == Math.Sign(offset.X)) || 
                   (Math.Sign(offset.Y)!=0 && Signs.Y == Math.Sign(offset.Y)));


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gstate"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="offset"></param>
        /// <returns>true if this block "konga line" hit a wall. false otherwise.</returns>
        private bool OffsetBlock(BCBlockGameState gstate, Block target, PointF offset)
        {
            //FIRST: get a set of blocks that are intersecting target.
            
            var wasintersecting = from b in gstate.Blocks where target.BlockRectangle.IntersectsWith(b.BlockRectangle) select b;

            var notus = from bb in gstate.Blocks where target.BlockRectangle.IntersectsWith(bb.BlockRectangle) && bb != target select bb;
            if (!notus.Any())
            {
            //    target.BlockRectangle = new RectangleF(target.BlockRectangle.X + offset.X, target.BlockRectangle.Y + offset.Y, target.BlockRectangle.Width, target.BlockRectangle.Height);
                return true;
             
            }
            //offset the block's position
            target.BlockRectangle = new RectangleF(target.BlockRectangle.X + offset.X, target.BlockRectangle.Y + offset.Y, target.BlockRectangle.Width, target.BlockRectangle.Height);
            
            //if we are touching the edge of the gameplay area, return true to indicate we did so.
            if (!gstate.GameArea.Contains(target.BlockRectangle.ToRectangle()))
                return true;
            //now get another set of intersections. However, we
            //don't want intersections that we were already intersecting with.
            var newintersections = from b in gstate.Blocks
                                   where target.BlockRectangle.IntersectsWith(b.BlockRectangle) &&
                                       !wasintersecting.Contains(b)
                                   select b;

            if (newintersections.Any())
            {
                Debug.Print("intersection");

            }
            //interesting note: newintersections will not contain any blocks that are "behind" us, because we won't
            //be touching them after moving anymore. It also won't include the target block.
            bool hadany = false;
            bool buildresult = false;
            foreach (var iterate in newintersections)
            {
                hadany = true;
                buildresult= buildresult || OffsetBlock(gstate, iterate, offset);
            }
            for (int i = 0; i < 2; i++)
            {
                DustParticle dp = new DustParticle(target.BlockRectangle.RandomSpot(BCBlockGameState.rgen),3,90,Color.Yellow);
                gstate.Particles.Add(dp);

            }

            //return true if no elements, buildresult otherwise.
            return hadany || buildresult;
        }
        public override string GetToolTipInfo(IEditorClient Client)
        {
            String b = base.GetToolTipInfo(Client);
            b = b + "\nDirection:" + _BuildDirection.ToString() + "\n" +
                "Type:" + _BuildBlock.Name + "\n" +
                "Number:" +( _MaxBuilds == 0 ? "(Infinite)" : _MaxBuilds.ToString());
            return b;
        }
        
        private bool ProxyFrame(ProxyObject me, BCBlockGameState gstate)
        {
            bool valuereturn = false;
            //if _CurrentMoving is null, we are finished with movement now.
            if ((_CurrentMoving == null) )
            
            {
                MovingProxy = null;
                immune = false;
                return true;
                
            }
            else
            {
                
                //move CurrentMoving in the appropriate direction.
                PointF useoffset = GetOffsetSpeed();
                



                gstate.Forcerefresh = true;
                //task: we need to move _CurrentMoving, 
                //and push any other blocks upward as it goes, unless a pushed block hits the edge.

                valuereturn = OffsetBlock(gstate, _CurrentMoving, useoffset);
                _CurrentMoving.hasChanged = true;

                

                if (valuereturn)
                {
                    _CurrentMoving = null;
                    immune = false;
                    

                }
                
                


                
            }

            

          


            return valuereturn;
            
        }
        private PointF GetOffsetSpeed()
        {
            if (_BuildDirection == BuilderBlockDirection.Left)
                return new PointF(-1, 0);
            if (_BuildDirection == BuilderBlockDirection.Up)
                return new PointF(0, -1);
            if (_BuildDirection == BuilderBlockDirection.Right)
                return new PointF(1, 0);
            if (_BuildDirection == BuilderBlockDirection.Down)
                return new PointF(0, 1);

            return new PointF(0, -1);
        }
        private bool immune = false;
        private Block _CurrentMoving; //block we are currently moving.
        private ProxyObject MovingProxy = null;
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            //create a new instance of the buildblock at our location with our size.
            //set it to start moving via a ProxyGameObject. Each frame we will Move the first block; if the firstblock touches a block on the side it is moving, we move that block the same amount, if that one is touching a block, we move that, etc.
            //We keep going until we try to move a block outside the gamearea (so it doesn't contains() it) or until the first block no longer intersects with the factory block.
            //while we are doing this, we set the ignore hits to true.
            if (immune) return false;
            immune = true;
          
                Built++;
                if (Built == MaxBuilds)
                {
                    base.PerformBlockHit(parentstate, ballhit);
                    return true;

                }
                //create a buildblock.
                _CurrentMoving = (Block)Activator.CreateInstance(BuildBlock, BlockRectangle);
                //activate our proxy Object.
                MovingProxy = new ProxyObject(ProxyFrame, null);
                parentstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                {
                    parentstate.Blocks.AddLast(_CurrentMoving);
                    parentstate.GameObjects.AddFirst(MovingProxy);

                }));
                BCBlockGameState.Soundman.PlaySound("REVEL"); // go with revel for now.
                return false;
                
                //add code to play special building sound.

      
        }
    }
}
