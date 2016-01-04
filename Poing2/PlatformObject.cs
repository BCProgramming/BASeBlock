using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using BASeCamp.BASeBlock.Blocks;
using BASeCamp.BASeBlock.Events;

namespace BASeCamp.BASeBlock
{
    //this interface is implemented by blocks that want to "see" when a PlatformObject touches them.
    public interface iPlatformBlockExtension
    {
        /// <summary>
        /// called when a PlatformObject starts to "track" the block. This means it is standing on it, typically.
        /// </summary>
        /// <param name="gstate"></param>
        /// <param name="objStand"></param>
        /// <param name="standingon">true if the block is now tracking. false otherwise</param>
        void Standon(BCBlockGameState gstate, PlatformObject objStand,bool standingon);


    }


    //generalized class, used to represent any object that acts like a "platform" character; that is, they land on tops of blocks, fall, jump, etc.
    public abstract class PlatformObject : GameEnemy, iLocatable, IMovingObject, iBumpable
    {

        protected struct PlatformTrackingData
        {
            /// <summary>
            /// block we are tracking as a platform.
            /// </summary>
            public Block TrackBlock;
            /// <summary>
            /// position of this tracked object at our last frame.
            /// </summary>
            public PointF PreviousFramePosition;


        }

        protected bool TrackPlatform = true; //true to track when blocks (platforms) move beneath the character.
        protected PlatformTrackingData ptd = new PlatformTrackingData();
        protected bool Nocollisions = false;
        private PointF _Velocity;
        public PointF Velocity { get { return _Velocity; } set { _Velocity = value; } }
        protected float FallSpeedLimit = 5;
        public static PointF DefaultGravityEffect = new PointF(0, 60f);
        public static PointF GetScaledGravityEffect(float factor)
        {

            return new PointF(DefaultGravityEffect.X * factor, DefaultGravityEffect.Y * factor);

        }
        protected PointF _GravityEffect = DefaultGravityEffect;
        public PointF GravityEffect { get { return _GravityEffect; } set { _GravityEffect = value; } }
        /// <summary>
        /// alias for DrawSize
        /// </summary>
        public SizeF useSize { get { return base.DrawSize; } set { base.DrawSize = useSize; } }


        public PlatformObject(PointF pPosition, PointF pVelocity, Dictionary<String, String[]> pStateFrameData, int pFrameDelay, ImageAttributes puseattributes)
            : base(pPosition, pStateFrameData, pFrameDelay, puseattributes)
        {
            Location = pPosition;
            Velocity = pVelocity;
            OnDeath += PlatformObject_OnDeath;

        }

        void PlatformObject_OnDeath(Object sender,EnemyDeathEventArgs e)
        {
            //when we die make sure that we stop tracking...
            if (ptd.TrackBlock is iPlatformBlockExtension)
                ((iPlatformBlockExtension)ptd.TrackBlock).Standon(e.StateObject, this, false);
                
                
        }
        public virtual void BulletHit(BCBlockGameState gstate, Projectile hitbullet)
        {



        }
        protected virtual void standingon(BCBlockGameState gamestate, List<Block> standingon, Block Mainblock)
        {

        }
        public virtual void Bump(Block bumpedby)
        {
            onground = false;
            ongroundlastframe = false;

            Debug.Print(this.GetType().Name + " bumped by " + bumpedby.GetType().Name);
            Velocity = new PointF(Velocity.X, -5);
            Location = new PointF(Location.X, Location.Y - 5);



        }
        public virtual void TouchLeft(BCBlockGameState gamestate, List<Block> touched, Block mainblock)
        {
            Velocity = new PointF(Velocity.X * -1, Velocity.Y);
        }
        //TouchPaddle(gamestate,ref AddObjects,ref removeobjects,gamestate.PlayerPaddle)
        /// <summary>
        /// called when the object touches the paddle.
        /// </summary>
        /// <param name="gamestate"></param>
        /// <param name="PlayerPaddle"></param>
        /// <returns>True to proceed with "stopping" any downward velocity. False otherwise.</returns>
        public virtual bool TouchPaddle(BCBlockGameState gamestate, Paddle PlayerPaddle)
        {
            return true;
            if (ptd.TrackBlock is iPlatformBlockExtension)
                ((iPlatformBlockExtension)ptd.TrackBlock).Standon(gamestate, this, false);
            ptd.TrackBlock = PlayerPaddle;
            ptd.PreviousFramePosition = PlayerPaddle.Location;
            if (ptd.TrackBlock is iPlatformBlockExtension)
                ((iPlatformBlockExtension)ptd.TrackBlock).Standon(gamestate, this, true);
            return true;

        }
        public virtual void TouchRight(BCBlockGameState gamestate, List<Block> touched, Block mainblock)
        {

            Velocity = new PointF(Velocity.X * -1, Velocity.Y);
        }
        public virtual void TouchTop(BCBlockGameState gamestate, List<Block> touched, Block mainblock)
        {
            //
            BCBlockGameState.Block_Hit(gamestate, mainblock);
        }

        /// <summary>
        /// overridden in child classes when the object is being killed.
        /// </summary>
        /// <param name="gstate">gamestate</param>
        /// <returns>True to prevent from dying; false otherwise. Note that true should only be returned when this gameobject needs to stick around
        /// for the death. (to animate falling off the screen, for example)</returns>
        public virtual bool Die(BCBlockGameState gstate)
        {
            if (ptd.TrackBlock is iPlatformBlockExtension)
                ((iPlatformBlockExtension)ptd.TrackBlock).Standon(gstate, this, false);
            gstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => {
                gstate.GameObjects.AddLast(new MarioDeathStyleObject(this));
                BCBlockGameState.Soundman.PlaySound("shelldie");
            
            }));
            
            return false;
        }

        /// <summary>
        /// called when this object hits the side of a block.
        /// </summary>
        /// <param name="gamestate"></param>
        /// <param name="hitblocks"></param>
        protected virtual void hitside(BCBlockGameState gamestate, List<Block> hitblocks)
        {

        }
        public sealed override int GetScoreValue()
        {
            return 0;
        }
        protected virtual void TouchLevelSide(BCBlockGameState gamestate, ref List<GameObject> Addobjects, ref List<GameObject> removeobjects, bool LeftSide)
        {
            //leftside is true if we touched the left side. False if we hit the right.
            //do nothing and the object will go off the side and be destroyed.

        }

        public enum CollideTypeConstants
        {
            Collide_Nothing, //no collision
            Collide_Bounce, //will collide but the two objects will go their own ways. calls PlatformObject's static "bounce" method.
            Collide_Passive, //will collide, but won't destroy the 'attacker'
            Collide_Aggressive, //will collide, destroying both objects.

        }

        protected void DoCollide(BCBlockGameState gamestate, PlatformObject otherenemy)
        {
            PlatformObject platformed = otherenemy;
            PlatformObject checkobject = platformed;
            CollideTypeConstants ctc = platformed.Collide(gamestate, this);


            if (ctc == CollideTypeConstants.Collide_Aggressive)
            {

                //kill us both...
                BCBlockGameState.Soundman.PlaySound("shelldie");
                Block.AddScore(gamestate, 25, Location);
                //GameObject ourdeath = new MarioDeathStyleObject(this);
                if (!Die(gamestate))
                {
                    gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                        gamestate.GameObjects.Remove(this)));
                    
                }
                if (!(checkobject as PlatformObject).Die(gamestate))
                {
                    gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                     gamestate.GameObjects.Remove(checkobject)));
                }
            }
            else if (ctc == CollideTypeConstants.Collide_Passive)
            {
                //destroy the other object, but not us.
                BCBlockGameState.Soundman.PlaySound("shelldie");
                Block.AddScore(gamestate, 30, Location);
                //make them start at the same speed (horizontally) as the "shell" (or us, rather)
                checkobject.Velocity = new PointF(Velocity.X, checkobject.Velocity.Y);

                if (!checkobject.Die(gamestate))
                {
                    gamestate.Defer(() => gamestate.GameObjects.Remove(checkobject));
                    
                }
            }
            else if (ctc == CollideTypeConstants.Collide_Bounce)
            {

                PlatformObject.Bounce(gamestate, this, checkobject as PlatformObject);

            }


        }


        /// <summary>
        /// called by some implementations of PlatformObject; for example, Shell, to determine if it can "kill" the object.
        /// </summary>
        /// <param name="gstate"></param>
        /// <param name="otherobject"></param>
        /// <returns></returns>
        public virtual CollideTypeConstants Collide(BCBlockGameState gstate, PlatformObject otherobject)
        {
            return CollideTypeConstants.Collide_Aggressive;
        }
        public static void Bounce(BCBlockGameState gstate, PlatformObject Enemy1, PlatformObject Enemy2)
        {
            //task: we want to "bounce" the two enemies off one another.
            //knowns: we know they are touching

            //first change the speeds if they aren't moving, 0 gums up the works.
            // if(Enemy1.Velocity.Y==0) Enemy1.Velocity = new PointF(0.001f,0);
            // if (Enemy2.Velocity.Y == 0) Enemy2.Velocity = new PointF(0.001f, 0);
            //however they need to be moving towards one another to count. First get the midpoint between the two.


            //get the difference between their X location and the midpoint. the Sign of their speeds
            //must be the opposite of the Math.Sign of their difference.
            PointF midpoint = new PointF();
            //special consideration: objects that don't support iMovingObject are assumed to be stationary, so we don't want to move those.
            //so we adjust "midpoint" so it isn't reall a midpoint at all but rather a position that will
            //result in the later code in this routine "moving" the stationary object where it already is.
            if ((Enemy1 is IMovingObject) && (Enemy2 is IMovingObject))
            {

                midpoint = BCBlockGameState.MidPoint(Enemy1.Location, Enemy1.Location);

            }
            else if (Enemy1 is IMovingObject)
            {

                midpoint = Enemy2.Location;

            }
            else if (Enemy2 is IMovingObject)
            {
                midpoint = new PointF(Enemy1.Location.X + Enemy1.DrawSize.Width, Enemy1.Location.Y);

            }

            float e1diff = Enemy1.Location.X - midpoint.X;
            float e2diff = Enemy2.Location.X - midpoint.X;
            if (Math.Sign(Enemy1.Velocity.X) != Math.Sign(e1diff)
                && Math.Sign(Enemy2.Velocity.X) != Math.Sign(e2diff))
            {
                //good! they are headed towards one another.

                //we want to adjust their locations. First sort them so we have the one on the left, and the one on the right.
                SortedList<float, GameEnemy> sortlocation = new SortedList<float, GameEnemy>();
                sortlocation.Add(Enemy1.Location.X, Enemy1);
                sortlocation.Add(Enemy2.Location.X, Enemy2);


                GameEnemy LeftEnemy = sortlocation.First().Value;
                GameEnemy RightEnemy = sortlocation.Last().Value;


                //move the left enemy to the midpoint - it's width.
                //move the right enemy to the midpoint.

                LeftEnemy.Location = new PointF(midpoint.X - LeftEnemy.DrawSize.Width, LeftEnemy.Location.Y);
                RightEnemy.Location = new PointF(midpoint.X, LeftEnemy.Location.Y);

                //make their speeds the same Math.Sign as the calculated e1diff and e2diff for each...
                //at least, for those that support iMovingObject.
                if (LeftEnemy is IMovingObject)
                {
                    IMovingObject castleft = (IMovingObject)LeftEnemy;
                    castleft.Velocity = new PointF(Math.Abs(castleft.Velocity.X) * Math.Sign(e1diff), castleft.Velocity.Y);

                }
                if (RightEnemy is IMovingObject)
                {
                    IMovingObject castright = (IMovingObject)RightEnemy;
                    castright.Velocity = new PointF(Math.Abs(castright.Velocity.Y) * Math.Sign(e2diff), castright.Velocity.Y);


                }





            }








        }
        protected bool Killed = false;
        public void Kill()
        {
            Killed = true;


        }
        bool TrackingBlockPositionChanged = false;
        protected bool ongroundlastframe = false;
        protected bool onground = false;
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            try
            {
                gamestate.Blocks.AddLast(gamestate.PlayerPaddle);
                if (Killed)
                {
                    Die(gamestate);
                    return true;

                }

                //our calculations need not our "real" velocity, but our "frame" FrameVel.
                PointF FrameVel = new PointF(Velocity.X * gamestate.GetMultiplier(), Velocity.Y * gamestate.GetMultiplier());


                onground = false;
                //each frame, we check for collisions.
                //Debug.Print("Shell, location:" + Location + " Vel:" + FrameVel);
                //first we need to see if we are "on the ground".
                //we do this by extending our rect downwards a third. if that returns true when we hit test for blocks, than we are on the ground.
                //the "test" rectangle's are half the size of the object itself and extend from one eighth within the object, to the bottom of the object 
                //plus our Y frame speed.
                Rectangle ourrectangle = new Rectangle((int)Location.X, (int)Location.Y, (int)useSize.Width, (int)useSize.Width);
                Polygon ourrectpoly = new Polygon(ourrectangle);
                if (!Nocollisions)
                {

                    Func<float, float, int> PosCalcFunction = ((Loc, size) => ((int)(Loc + size * .875f)));
                    Func<float, int> SizeCalcFunction = ((size) => ((int)size / 4));



                    //iterate through all Blocks.
#if !oldplatform
                    
                    var result = 
                        (from p in gamestate.Blocks 
                         let y = GeometryHelper.PolygonCollision(ourrectpoly,p.GetPoly(),Velocity) 
                         where y.Intersect|y.WillIntersect 
                         orderby y.MinimumTranslationVector.Magnitude ascending
                         select new{Poly=p,CollisionResult=y}).FirstOrDefault()   ;

                    if (result != null)
                    {
                        //result is now the anon type for the nearest Polygon that we touch.

                        //now we base whether we are standing on, being blocked by, or touched our head to the object
                        //based on the adjustment vector. 
                        //note that the adjustment vector indicates how we have to move the 
                        //PlatformObject.

                        if (result.CollisionResult.Intersect)
                        {
                            //are we touching it with our "feet"?
                            if (result.CollisionResult.MinimumTranslationVector.Y <= 0)
                            {
                                //if the adjustment moves us upward, than we are touching from above, so we are considered to be "on the ground"
                                onground = true;
                                Velocity = new PointF(Velocity.X, 0);

                                if (TrackPlatform)
                                {
                                    if (ptd.TrackBlock != result.Poly)
                                    {

                                        if (ptd.TrackBlock is iPlatformBlockExtension)
                                            ((iPlatformBlockExtension)ptd.TrackBlock).Standon(gamestate, this, false);

                                        //if (ptd.TrackBlock != null) ptd.TrackBlock.OnBlockRectangleChange -= TrackBlock_OnBlockRectangleChange;
                                        ptd.TrackBlock = result.Poly;
                                        if (ptd.TrackBlock is iPlatformBlockExtension)
                                        {

                                            ((iPlatformBlockExtension)ptd.TrackBlock).Standon(gamestate, this, true);

                                        }
                                        //ptd.TrackBlock.OnBlockRectangleChange += new Action<RectangleF>(TrackBlock_OnBlockRectangleChange);
                                        ptd.PreviousFramePosition = result.Poly.Location;
                                    }

                                }
                                Location = new PointF(Location.X, Location.Y + GetGroundOffset());
                                standingon(gamestate, ref AddObjects, ref removeobjects, new Block[] { result.Poly }.ToList(), result.Poly);



                            }
                            else if (result.CollisionResult.MinimumTranslationVector.Y > 0)
                            {


                                //Velocity = new PointF(Velocity.X, Math.Abs(Velocity.Y));
                                //touching from below.
                                TouchTop(gamestate, ref AddObjects, ref removeobjects, new Block[] { result.Poly }.ToList(), result.Poly);
                            }
                            if (Math.Abs(result.CollisionResult.MinimumTranslationVector.X) > 0.5f)
                            {

                                if (result.CollisionResult.MinimumTranslationVector.X < 0)
                                    TouchRight(gamestate, ref AddObjects, ref removeobjects, new Block[] { result.Poly }.ToList(), result.Poly);
                                else
                                    TouchLeft(gamestate, ref AddObjects, ref removeobjects, new Block[] { result.Poly }.ToList(), result.Poly);


                            }
                        }

                        foreach (Block iterateblock in gamestate.Blocks)
                        {
                            var pcr = GeometryHelper.PolygonCollision(ourrectpoly, iterateblock.GetPoly(), FrameVel);



                        }


                    
                        Location = new PointF(Location.X + result.CollisionResult.MinimumTranslationVector.X, Location.Y + result.CollisionResult.MinimumTranslationVector.Y);
                    }
                    if (Velocity.Y < FallSpeedLimit && !(onground || ongroundlastframe))
                    {
                        float GravityXEffect = gamestate.ScaleValue(GravityEffect.X);
                        float GravityYEffect = gamestate.ScaleValue(GravityEffect.Y);
                        Velocity = new PointF(Velocity.X + (GravityXEffect), Velocity.Y + (GravityYEffect));

                    }
                    BCBlockGameState.IncrementLocation(gamestate, ref _Location, Velocity);

                    return (!gamestate.GameArea.IntersectsWith(ourrectangle)) || base.PerformFrame(gamestate, ref AddObjects, ref removeobjects);



#endif

                    //old code here...
                    /*
                    // Rectangle TopTestRect = new RectangleF(LocationX,(Location.Y
                    //Rectangle LeftTestRect = new Rectangle((int)(Location.X - (useSize.Width * .125f)),
                    //        (int)Location.Y, SizeCalcFunction(useSize.Width), (int)(useSize.Height * 0.5));
                    float tenpercentw = (useSize.Width * .1f);
                    Rectangle TopTestRect = new Rectangle((int)Location.X + (int)(tenpercentw), (int)(Location.Y - (useSize.Height * .125f)),
                        (int)(useSize.Width) - (int)(tenpercentw * 2), SizeCalcFunction(useSize.Height));

                    RectangleF GroundTestRect = new RectangleF(Location.X + (int)(useSize.Width * .125), (Location.Y + (useSize.Height - (useSize.Height * .125f))),
                        useSize.Width - ((int)(useSize.Width * .125)), SizeCalcFunction(useSize.Height) / 4);

                    //Debug.Print(Location.ToString());
                    List<Block> groundblocks = BCBlockGameState.Block_HitTest(gamestate.Blocks, GroundTestRect, false);
                    */
                    float tenpercentw = (useSize.Width * .1f);
                    //float topsans = Math.Min(Location.Y - useSize.Height * 0.125f, Location.Y - FrameVel.Y);
                    //float bottomadd = Math.Max(Location.Y + useSize.Height * 0.125f, Location.Y + FrameVel.Y);

                    //test rectangle data for hitting stuff above.
                    int TopLeft = (int)(Location.X + (tenpercentw));
                    int TopRight = (int)(Location.X + useSize.Width - tenpercentw);
                    int TopTop = (int)Math.Min(Location.Y - (useSize.Height * 0.125f), Location.Y + FrameVel.Y);
                    int TopBottom = (int)(Location.Y + Math.Max(useSize.Height * 0.125f,FrameVel.Y));

                    Rectangle TopTestRect = new Rectangle(TopLeft, TopTop, TopRight - TopLeft, TopBottom - TopTop);
                    //test rectangle data for the ground.

                    int BottomLeft = TopLeft;
                    int BottomRight = TopRight;
                    int BottomTop = (int)(Location.Y + useSize.Height - (useSize.Height * 0.125f));
                    int BottomBottom = (int)(Math.Max(Location.Y + useSize.Height + (useSize.Height * 0.125f), Location.Y + FrameVel.Y));


                    RectangleF GroundTestRect = new RectangleF(BottomLeft, BottomTop, BottomRight - BottomLeft, BottomBottom - BottomTop);

                    


                    List<Block> groundblocks = null;

                    if (Math.Sign(Velocity.Y) != -1)
                        groundblocks = BCBlockGameState.Block_HitTest(gamestate.Blocks, GroundTestRect, false);
                    else
                        groundblocks = new List<Block>();

                    // Debug.Print(onground?"onground":"!onground");




                    //get blocks in proper direction, adjust our position and "hit" the block.

                    List<Block> hitblocks = null;
                    if (FrameVel.Y < 0)
                    {
                        RectangleF rf = new RectangleF(TopTestRect.Left, TopTestRect.Top, TopTestRect.Width, TopTestRect.Height);
                        hitblocks = BCBlockGameState.Block_HitTest(gamestate.Blocks, rf, false);
                        if (hitblocks.Count > 0)
                        {
                            float currminimum = float.MaxValue;
                            Block foundbottommost = null;
                            foreach (Block findbottommost in hitblocks)
                            {
                                if (foundbottommost == null || findbottommost.BlockRectangle.Bottom > foundbottommost.BlockRectangle.Bottom)
                                    foundbottommost = findbottommost;
                            }
                            Location = new PointF(Location.X, foundbottommost.BlockRectangle.Bottom+1);
                            TouchTop(gamestate, hitblocks, foundbottommost);
                        }




                    }

                    else if (FrameVel.Y > 0)
                    {
                        //check for paddle contact.
                        if (gamestate.PlayerPaddle != null)
                        {
                            Rectangle testrect = gamestate.PlayerPaddle.Getrect();
                            if (GroundTestRect.IntersectsWith(testrect))
                            {
                                if (TouchPaddle(gamestate,  gamestate.PlayerPaddle))
                                    Location = new PointF(Location.X, testrect.Top - useSize.Height + GetGroundOffset());

                                onground = true;
                                ongroundlastframe = true;


                            }



                        }

                    }


                    if (FrameVel.X > 0)
                    {
                        Rectangle RightTestRect = new Rectangle(PosCalcFunction(Location.X, useSize.Width), (int)Location.Y,
                        SizeCalcFunction(useSize.Width), (int)(useSize.Height * 0.5f));
                        //moving right.
                        RectangleF rf = new RectangleF(RightTestRect.Left, RightTestRect.Top, RightTestRect.Width, RightTestRect.Height);

                        hitblocks = BCBlockGameState.Block_HitTest(gamestate.Blocks, rf, false);
                        if (hitblocks.Count > 0)
                        {
                            //move to the left of the leftmost block...
                            float currminimum = float.MaxValue;
                            Block foundleftmost = null;
                            foreach (Block findleftmost in hitblocks)
                            {
                                if (foundleftmost == null || findleftmost.BlockRectangle.Left < foundleftmost.BlockRectangle.Left)
                                    foundleftmost = findleftmost;

                            }

                            Location = new PointF(foundleftmost.BlockRectangle.Left - useSize.Width, Location.Y);

                            TouchRight(gamestate, hitblocks, foundleftmost);


                        }


                    }
                    else if (FrameVel.X < 0)
                    {

                        Rectangle LeftTestRect = new Rectangle((int)(Location.X - (useSize.Width * .125f)),
                            (int)Location.Y, SizeCalcFunction(useSize.Width), (int)(useSize.Height * 0.5));
                        RectangleF rf = new RectangleF(LeftTestRect.Left, LeftTestRect.Top, LeftTestRect.Width, LeftTestRect.Height);
                        hitblocks = BCBlockGameState.Block_HitTest(gamestate.Blocks, rf, false);
                        if (hitblocks.Count > 0)
                        {
                            float currminimum = float.MaxValue;
                            Block foundrightmost = null;
                            foreach (Block findrightmost in hitblocks)
                            {
                                if (foundrightmost == null || findrightmost.BlockRectangle.Right > foundrightmost.BlockRectangle.Right)
                                    foundrightmost = findrightmost;


                            }

                            Location = new PointF(foundrightmost.BlockRectangle.Right, Location.Y);
                            TouchLeft(gamestate, hitblocks, foundrightmost);
                        }







                    }

                    onground = groundblocks.Any();

                    if (onground)
                    {
                        //acquire the highest block.
                        Block currlowest = null;
                        foreach (Block loopblock in groundblocks)
                        {
                            if (currlowest == null || loopblock.BlockRectangle.Top > currlowest.BlockRectangle.Top)
                                currlowest = loopblock;

                        }
                        if (currlowest != null)
                        {
                            if (currlowest.BlockRectangle.Top > Location.Y)
                                Location = new PointF(Location.X, currlowest.BlockRectangle.Top - useSize.Height + GetGroundOffset());


                            if (TrackPlatform)
                            {
                                if (ptd.TrackBlock != currlowest )
                                {

                                    if (ptd.TrackBlock is iPlatformBlockExtension)
                                        ((iPlatformBlockExtension)ptd.TrackBlock).Standon(gamestate, this, false);

                                    //if (ptd.TrackBlock != null) ptd.TrackBlock.OnBlockRectangleChange -= TrackBlock_OnBlockRectangleChange;
                                    ptd.TrackBlock = currlowest;
                                    if (ptd.TrackBlock is iPlatformBlockExtension)
                                    {

                                        ((iPlatformBlockExtension)ptd.TrackBlock).Standon(gamestate, this, true);

                                    }
                                    //ptd.TrackBlock.OnBlockRectangleChange += new Action<RectangleF>(TrackBlock_OnBlockRectangleChange);
                                    ptd.PreviousFramePosition = currlowest.Location;
                                }

                            }
                            if (currlowest is Paddle)
                            {
                                Debug.Print("paddle");

                            }
                            standingon(gamestate, groundblocks, currlowest);
                        }

                    }
                    else
                    {
                        //if we aren't on the ground, clear the trackblock data.
                        //if (ptd.TrackBlock != null) ptd.TrackBlock.OnBlockRectangleChange -= TrackBlock_OnBlockRectangleChange;
                        if (ptd.TrackBlock != null)
                        {
                            if (ptd.TrackBlock is iPlatformBlockExtension)
                                ((iPlatformBlockExtension)ptd.TrackBlock).Standon(gamestate, this, false);

                            Debug.Print("Stopped Tracking a block:" + ptd.TrackBlock.GetType().Name);
                        }
                        
                        ptd.TrackBlock = null;

                        

                    }
                    if (onground || ongroundlastframe)
                    {
                        Velocity = new PointF(Velocity.X, 0);



                    }

                    if ((hitblocks ?? new List<Block>()).Count > 0)
                    {
                        hitside(gamestate, hitblocks);

                    }


                }

                if (Velocity.Y < FallSpeedLimit && !(onground || ongroundlastframe))
                {
                    float GravityXEffect = gamestate.ScaleValue(GravityEffect.X);
                    float GravityYEffect = gamestate.ScaleValue(GravityEffect.Y);
                    Velocity = new PointF(Velocity.X + (GravityXEffect), Velocity.Y + (GravityYEffect));

                }
                BCBlockGameState.IncrementLocation(gamestate, ref _Location, Velocity);

                if (TrackPlatform)
                {

                    //if we are tracking platforms, AND we are currently on one...
                    if (ptd.TrackBlock != null)
                    {

                        if (ptd.TrackBlock.Location != ptd.PreviousFramePosition)
                        {
                            PointF thediff = new PointF(ptd.TrackBlock.Location.X - ptd.PreviousFramePosition.X,
                                ptd.TrackBlock.Location.Y - ptd.PreviousFramePosition.Y);
                            //add this diff to our location.
                            Debug.Print("The difference is " + thediff);
                            _Location = new PointF(_Location.X + thediff.X, _Location.Y + thediff.Y);

                            ptd.PreviousFramePosition = ptd.TrackBlock.Location;
                        }
                    }


                }

                ongroundlastframe = onground;


                //checks to see if it hit the side of the level.
                Rectangle LeftCheck = new Rectangle(-20, 0, 20, gamestate.GameArea.Height);
                Rectangle RightCheck = new Rectangle(gamestate.GameArea.Width, 0, 20, gamestate.GameArea.Height);



                return (!gamestate.GameArea.IntersectsWith(ourrectangle)) || base.PerformFrame(gamestate);
            }
            finally
            {
                gamestate.Blocks.Remove(gamestate.PlayerPaddle);

            }
        }

        void TrackBlock_OnBlockRectangleChange(RectangleF obj)
        {
            TrackingBlockPositionChanged = true;
        }

        public virtual float GetGroundOffset()
        {
            return 2;

        }


    }
    [NoKillIncrement()] 
    //Platform objects will not increment the Statistics Kill counter.
    

    public abstract class PlatformEnemy : PlatformObject
    {
        protected PlatformEnemy(PointF pPosition, PointF pVelocity, Dictionary<string, string[]> pStateFrameData, int pFrameDelay, ImageAttributes puseattributes)
            : base(pPosition, pVelocity, pStateFrameData, pFrameDelay, puseattributes)
        { }
    }


    //Shell Object. Designed to emulate the action of Mario's Koopa shells.

    public class Shell : PlatformEnemy
    {

        //how it works:
        //the shell will need to start with a speed.





        public Shell(PointF pPosition)
            : this(pPosition, new SizeF(16, 16))
        {
            //GravityEffect = new PointF(0, 11f);
            GravityEffect = GetScaledGravityEffect(0.8f);
        }
        public Shell(PointF pPosition, SizeF pusesize)
            : this(pPosition, new Nullable<SizeF>(pusesize))
        { }
        public Shell(PointF pPosition, SizeF? pusesize)
            : base(pPosition, new PointF(4, 0), null, 4, null)
        {

            StateFrameImageKeys = new Dictionary<string, string[]>();
            /*
            StateFrameImageKeys.Add("NORMAL",new string[]{"SHELL"}); //add extra frames here to animate the shell. (or make it possible to stop....)
            StateFrameIndex = new Dictionary<string, int>();
            StateFrameIndex.Add("NORMAL", 0);
            
            FrameDelayTimes = new Dictionary<string, int[]>();
            FrameDelayTimes.Add("NORMAL", new int[] { 25 });
            EnemyState = "NORMAL";
             * */

            StateFrameImageKeys.Add("NORMAL", new string[] { "SHLL1", "SHLL2", "SHLL3", "FLIPX:SHLL2" }); //add extra frames here to animate the shell. (or make it possible to stop....)
            StateFrameImageKeys.Add("STOPPED", new String[] { "SHELL1" });
            StateFrameIndex = new Dictionary<string, int>();
            StateFrameIndex.Add("NORMAL", 0);
            StateFrameIndex.Add("STOPPED", 0);
            FrameDelayTimes = new Dictionary<string, int[]>();
            FrameDelayTimes.Add("NORMAL", new int[] { 2, 2, 2, 2 });
            FrameDelayTimes.Add("STOPPED", new int[] { 2 });
            EnemyAction = "NORMAL";
            if (pusesize != null) useSize = pusesize.Value;

        }



        protected override void hitside(BCBlockGameState gamestate, List<Block> hitblocks)
        {
            Block smackit = hitblocks.First();
            BCBlockGameState.Block_Hit(gamestate, smackit, Velocity); //smack it with the shell's velocity.
            gamestate.Forcerefresh = true;

        }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {

            //additional checks, to see if we collide with another Enemy

            foreach (GameObject checkobject in gamestate.GameObjects)
            {
                if (checkobject is PlatformObject && checkobject != this)
                {
                    PlatformObject platformed = ((PlatformObject)checkobject);
                    if (platformed.GetRectangle().IntersectsWith(GetRectangle()))
                    {

                        this.DoCollide(gamestate, platformed);
                    }


                }



            }


            return base.PerformFrame(gamestate);
        }

    }
    public interface iBumpable
    {

        void Bump(Block bumpedby);

    }
}
