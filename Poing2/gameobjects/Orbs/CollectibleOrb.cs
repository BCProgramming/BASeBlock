using System;
using System.Drawing;
using System.Linq;

namespace BASeBlock.GameObjects.Orbs
{
    public abstract class CollectibleOrb : AnimatedImageObject, IMovingObject, iLocatable
    {
        protected PointF _Velocity = new PointF(0, 0);
        
        
        public PointF Velocity { get { return _Velocity; } set { _Velocity = value; } }
        public Image OrbImage { get { return CurrentFrameImage; }  }
        public static SizeF DefaultSize = new SizeF(8, 8);
        [Flags]
        protected enum CollectibleTypeConstants
        {
            Collectible_GameCharacter,
            Collectible_Paddle,
            Collectible_Both = Collectible_GameCharacter | Collectible_Paddle
            
        }



        protected CollectibleOrb(PointF pLocation, SizeF pSize)
            : base(pLocation, pSize,new Image[]{BCBlockGameState.GetGummyImage(Color.Blue,pSize.ToSize())})
        {
            
        }
        protected CollectibleOrb(PointF pLocation, SizeF pSize,Image[] OrbImages)
            :base(pLocation,pSize,OrbImages)
        {
        }
        protected CollectibleOrb(PointF pLocation, SizeF pSize, Image[] OrbImages, int pFrameDelay)
            :base(pLocation,pSize,OrbImages,pFrameDelay)
        {
            
        }
        //ALL CollectibleOrb's must implement this constructor. Or bad stuff will happen. I promise you.
        protected CollectibleOrb(PointF pLocation):this(pLocation, DefaultSize)
        {}
        protected CollectibleOrb(SizeF pSize, PointF pLocation, Image pOrbImage):base(pLocation, pSize,new Image[]{pOrbImage})
        {}
        protected virtual void ReAcquireImage()
        {//by default does nothing.
        }
        
        public override void Draw(Graphics g)
        {//location is center.
            g.DrawImage(OrbImage, Location.X, Location.Y, Size.Width, Size.Height);
        }
        /// <summary>
        /// called when this collectible touches a character. or paddle.
        /// </summary>
        /// <param name="gstate"></param>
        /// <param name="gchar"></param>
        /// <returns></returns>
        protected abstract bool TouchCharacter(BCBlockGameState gstate, GameCharacter gchar);
        protected abstract bool TouchPaddle(BCBlockGameState gstate, Paddle pchar);
        protected virtual CollectibleTypeConstants getCollectibleType()
        {
            return CollectibleTypeConstants.Collectible_Both;
        }
        protected virtual String getCollectionSound()
        {
            return "ORBTING";

        }
        protected static PointF DecayFactor = new PointF(0.98f, 0.98f);
        protected static PointF DecayAdd = new PointF(0, 0.1f);
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            if (gamestate.PlayerPaddle != null)
            {
                Velocity = new PointF(Velocity.X * DecayFactor.X,
                                      Velocity.Y * (DecayFactor.Y) + (DecayAdd.Y));
            }
            
            BCBlockGameState.IncrementLocation(gamestate,ref _Location, Velocity);

            //check for touches between the character and the paddle, based on whether the flag for each is set.
            if ((getCollectibleType() & CollectibleTypeConstants.Collectible_GameCharacter) == CollectibleTypeConstants.Collectible_GameCharacter)
            {
                foreach (GameCharacter gchar in (from j in gamestate.GameObjects where j is GameCharacter select j))
                {
                    //check for collision/touch
                    if (gchar.GetRectangleF().IntersectsWith(getRectangle()))
                    {
                        BCBlockGameState.Soundman.PlaySound(getCollectionSound());
                        return TouchCharacter(gamestate, gchar);


                    }


                }


            }
            if ((getCollectibleType() & CollectibleTypeConstants.Collectible_Paddle) == CollectibleTypeConstants.Collectible_Paddle)
            {
                if (gamestate.PlayerPaddle != null)
                {
                    if (gamestate.PlayerPaddle.Getrect().IntersectsWith(getRectangle().ToRectangle()))
                    {
                        //if any of the behaviours reject it by returning true, do not collect it.
                        if (!gamestate.PlayerPaddle.Behaviours.Any((b) => b.getMacGuffin(gamestate, gamestate.PlayerPaddle, this)))
                        {
                            BCBlockGameState.Soundman.PlaySound(getCollectionSound());
                            return TouchPaddle(gamestate, gamestate.PlayerPaddle);
                        }
                    }

                }

            }

            //if we are off to the left...
            if (Location.X < gamestate.GameArea.Left)
            {
                //move into play...
                Location = new PointF(gamestate.GameArea.Left + 1, Location.Y);
                //adjust speed.
                Velocity = new PointF(Math.Abs(Velocity.X), Velocity.Y);

            }
            else if (Location.X + Size.Width > gamestate.GameArea.Right)
            {
                //move back into play...
                Location = new PointF(gamestate.GameArea.Right - 1 - Size.Width, Location.Y);
                //adjust speed.
                Velocity = new PointF(-Math.Abs(Velocity.X), Velocity.Y);

            }

            //also bounce off the top, but not the bottom!
            if (Location.Y < gamestate.GameArea.Top)
            {
                Location = new PointF(Location.X, gamestate.GameArea.Top + 1);
                Velocity = new PointF(Velocity.X, Math.Abs(Velocity.Y));

            }

            //don't twaddle the velocity if there is no paddle. (idea being that levels that are platform based should have "stationary" macguffins.


            return base.PerformFrame(gamestate) || !gamestate.GameArea.IntersectsWith(getRectangle().ToRectangle());
        }

    }
}