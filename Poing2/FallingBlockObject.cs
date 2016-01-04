using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using BASeCamp.BASeBlock.Blocks;

namespace BASeCamp.BASeBlock
{
    /// <summary>
    /// represents a Block that is Falling.
    
    /// </summary>
    public class FallingBlockObject :SizeableGameObject,iSizedProjectile,iImagable,IExplodable
    {
        //public PointF Location { get; set; }
        Rectangle iImagable.getRectangle()
        {
            return getRectangle().ToRectangle();
        }

        public PointF Velocity { get; set; }
        private readonly PointF MaxSpeed = new PointF(5, 5);
        
        Point iImagable.Location { get; set; }
        private Image doDrawImage = null;
        private Block _FallingBlock = null;
        private PointF _Gravity = new PointF(0, 0.5f);
        //public RectangleF getRectangle() { return new RectangleF(Location, Size); }
        public FallingBlockObject(Block source):base(source.BlockRectangle.TopLeft(),source.BlockSize)
        {
            //create the image, make it so the block in question can not be hit, and then carry on.
            
            _FallingBlock = source;
            Location = source.BlockRectangle.TopLeft();
            Velocity = new PointF(0, 0);
            Size = source.BlockSize;

            doDrawImage = source.getImage();
            //to prevent the block from being hit, we simply move it...
            _FallingBlock.BlockLocation = new PointF(-32767, -32767);
            _FallingBlock.hasChanged = true;


        }
        /*
        public static IEnumerable<Block> GetRestingBlocks(BCBlockGameState gstate,Block blocktest)
        {
            var touchingblocks = from b in gstate.Blocks where b!=blocktest && b.BlockRectangle.IntersectsWith(blocktest.BlockRectangle) select b;
            return touchingblocks;
           // return from b in touchingblocks where b.CenterPoint().Y > blocktest.BlockRectangle.Bottom select b;

        }*/
        public static IEnumerable<Block> GetRestingBlocks(BCBlockGameState gstate,RectangleF testrect)
        {
            var touchingblocks = from b in gstate.Blocks where b.BlockRectangle.IntersectsWith(testrect) select b;

            return from bb in touchingblocks
                   where bb.CenterPoint().Y > testrect.CenterPoint().Y && 
                   Math.Abs(bb.CenterPoint().Y - testrect.CenterPoint().Y) > 
                   Math.Abs(bb.CenterPoint().X-testrect.CenterPoint().X) select bb;

        }
        public static IEnumerable<Block> getAboveBlocks(BCBlockGameState gstate, RectangleF testrect)
        {
            var aboveblocks = from b in gstate.Blocks where b.BlockRectangle.IntersectsWith(testrect) select b;
            return from bb in aboveblocks
                   where Math.Abs(bb.CenterPoint().Y - testrect.CenterPoint().Y) >
                       Math.Abs(bb.CenterPoint().X - testrect.CenterPoint().Y)
                   select bb;

        }
       


        int countoff = 0;
        public override bool PerformFrame(BCBlockGameState gamestate)
        {

            //check to make sure we are in the game bounds, and bounce off the sides.

            if (getRectangle().Top < gamestate.GameArea.Top)
            {

                Location = new PointF(Location.X, gamestate.GameArea.Top + 1);

            }
            if (getRectangle().Left < gamestate.GameArea.Left)
            {
                Location = new PointF(gamestate.GameArea.Left + 1, Location.Y);
                Velocity = new PointF(-Velocity.X, Velocity.Y);
            }

            else if (getRectangle().Right > gamestate.GameArea.Right)
            {
                Location = new PointF(gamestate.GameArea.Right - getRectangle().Width, Location.Y);
                Velocity = new PointF(-Velocity.X, Velocity.Y);
            }

            //next, see if we are touching a block beneath us.

            countoff++;
            if (countoff == 5)
            {
                //FadingImageObject fio = new FadingImageObject(this, new TimeSpan(0, 0, 0, 0, 500), null);
                //gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => gamestate.GameObjects.AddLast(fio)));
                
            }
            
            
            //we only want blocks whose center is below the FallingBlock's top.
            var belowblocks = GetRestingBlocks(gamestate,getRectangle());

            if (belowblocks.Any() && Math.Sign(Velocity.Y) > 0)
            {
                //grab the highest item from belowblocks.
                Block highestBlock = (from getlow in belowblocks orderby getlow.BlockRectangle.Top ascending select getlow).First();
                //we want to be on top, so align our bottom to equal the top of the highest block.
                RectangleF userectangle = new RectangleF(getRectangle().Left, highestBlock.BlockRectangle.Top - getRectangle().Height + 1,
                    getRectangle().Width, getRectangle().Height);

                //stop falling, move the original block to our current location, and remove ourselves.
                _FallingBlock.BlockRectangle = userectangle;
                _FallingBlock.hasChanged = true;
                gamestate.Forcerefresh = true;
                Trace.WriteLine("FallingBlockObject moving block to " + getRectangle());

                if (_FallingBlock is FallingBlock)
                    ((FallingBlock)_FallingBlock).Faller = null;
                return true;


            }
            else
            {

                //check for blocks that are simply intersecting.
                /*
                var intersecting = from b in gamestate.Blocks where b.BlockRectangle.IntersectsWith(getRectangle()) select b;
                if (intersecting.Any())
                {
                    Block firstblock = intersecting.First();
                    //which direction is it? Get the angle between our center and the center of the first block.
                    var angledirection = BCBlockGameState.GetAngle(getRectangle().CenterPoint(), firstblock.BlockRectangle.CenterPoint());

                    Block.BallRelativeConstants relative = Block.getMainDirection(angledirection);

                    //cancel the movement in the correct direction.

                    if (relative == Block.BallRelativeConstants.Relative_Up)
                    {
                        //cancel upward movement if we are going up.
                        if (Velocity.Y < 0) Velocity = new PointF(Velocity.X, 0);

                    }
                    else if (relative == Block.BallRelativeConstants.Relative_Down)
                    {
                        //ignore.
                        //other codepath would have captured this.
                    }
                    else if (relative == Block.BallRelativeConstants.Relative_Right)
                    {
                        //cancel rightward movement if we are moving right.
                        if (Velocity.X > 0) Velocity = new PointF(0, Velocity.Y);

                    }
                    else if (relative == Block.BallRelativeConstants.Relative_Left)
                    {
                        //cancel leftward movement if we are moving left.
                        if (Velocity.X < 0) Velocity = new PointF(0, Velocity.Y);


                    }

                



                




                }

*/
                float lowestpoint = gamestate.GameArea.Bottom;
                if (gamestate.PlayerPaddle != null) lowestpoint = gamestate.PlayerPaddle.Getrect().Top;

                if (getRectangle().Bottom > lowestpoint)
                {
                    RectangleF userectangle = new RectangleF(getRectangle().Left, lowestpoint- getRectangle().Height + 1,
                 getRectangle().Width, getRectangle().Height);
                    _FallingBlock.BlockRectangle=userectangle;
                    _FallingBlock.hasChanged = true;
                    gamestate.Forcerefresh = true;
                    if (_FallingBlock is FallingBlock)
                        ((FallingBlock)_FallingBlock).Faller = null;
                    return true;
                }


            }


            //also check for left and right-side obstructions.

            //if we hit something, we'll "bounce" with this multiplier.
            float bouncemultiplier = 0f; //right now we stop dead horizontally.

            //check above first.






            //otherwise, no obstructions- fall.


            Velocity = new PointF(Velocity.X, Velocity.Y + _Gravity.Y);
            if (Math.Abs(Velocity.Y) > 5) Velocity = new PointF(Velocity.X, Math.Sign(Velocity.Y) * 5);
            Location = new PointF(Location.X + Velocity.X, Location.Y + Velocity.Y);
            Trace.WriteLine("FallingBlockObject Velocity:" + Velocity + " rect:" + getRectangle());
            return false;

            //return base.PerformFrame(gamestate, ref AddObjects, ref removeobjects);
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(doDrawImage, new RectangleF(Location, Size));
            //g.DrawRectangle(new Pen(Color.Black, 2), new RectangleF(Location, Size).ToRectangle());
        }

        Size iImagable.Size { get; set; }
        public void ExplosionInteract(object sender, PointF Origin, double Strength)
        {
            //same as normal, get angle and nudge us in the appropriate direction.

            double usea = BCBlockGameState.GetAngle(Origin, CenterPoint());
            PointF usevector = new PointF((float)(Math.Sin(usea) * Strength), (float)(Math.Cos(usea)));
            Velocity = new PointF(Velocity.X + usevector.X, Velocity.Y + usevector.Y);

        }
    }
}
