using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;
using BASeCamp.BASeBlock.Particles;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    [BlockDescription("Block that can spawn a GameCharacterPowerup. This is geared towards the platforming implementation. Some stub powerups exists to spawn things like macguffins.")]
    public class PowerupBlock : ImageBlock,IEditorBlockExtensions 
    {

        /// <summary>
        /// Object used to create the "bounce" animation.
        /// this is better than making the PowerUpblock animated and moving it, because
        /// the animated block stuff is quite terrible.
        /// </summary>
        private class TransitoryBlockObject : SizeableGameObject
        {
            private TimeSpan DefaultPoptime = new TimeSpan(0, 0, 0, 0, 250); //default to half a second.
            private DateTime? StartPopTime=null; //stored at first PerformFrame after construction.
            private Bitmap useDrawImage = null; //created from the block in constructor.
            public delegate void TransitionCompletionFunction(BCBlockGameState gstate,TransitoryBlockObject sender);
            public TransitionCompletionFunction TransitionComplete;
            private Block TransitBlock = null;
            public TransitoryBlockObject(Block blockobj):base(blockobj.CenterPoint(), blockobj.BlockSize)
            {
                
                TransitBlock = blockobj;
                Graphics gcanvas = null; 
                Block.DrawBlock(blockobj, out useDrawImage, out gcanvas);
                gcanvas.Dispose();

                
            }
            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                if (StartPopTime == null)
                {
                    StartPopTime = DateTime.Now;
                    
                }

                //has the time elapsed?
                if ((DateTime.Now - StartPopTime) > DefaultPoptime)
                {
                    //it has; we're done.
                    if (TransitionComplete != null) TransitionComplete(gamestate,this);
                    return true; //true, to destroy ourselves.


                }
                //calculate our "percentage"...
                double elapsedms = (DateTime.Now - StartPopTime).Value.TotalMilliseconds;
                double percentage = elapsedms / DefaultPoptime.TotalMilliseconds;
                offsetamount = (int)(Math.Sin(percentage * (Math.PI)) * TransitBlock.BlockRectangle.Height);

                //Location = new PointF(TransitBlock.Location.X, TransitBlock.Location.Y - offsetamount);


                return base.PerformFrame(gamestate);
            }
            int offsetamount = 0;


          
            public override void Draw(Graphics g)
            {
                g.DrawImageUnscaled(useDrawImage, new Point((int)TransitBlock.BlockRectangle.Left, (int)(TransitBlock.BlockRectangle.Top - offsetamount)));
            }
        }
        //the powerup block has three stages:
        //Normal, where it is a block that still contains a powerup, before it is hit.
        //when it is hit, it enters a short mode where it is invisible, creating a gameobject with it's size and shape
        //that moves up half the height of the block, and back down. the block then changes to "emptyblock", and releases it's powerup.
        //one possible tweak could be to have it so that a block can release a given number of powerups before it changes to being depleted.


        private enum PowerupBlockMode
        {
            /// <summary>
            /// Powerup is still contained in this block.
            /// </summary>
            BlockMode_Filled, 
            /// <summary>
            /// Transitory state between filled and empty.
            /// </summary>
            BlockMode_Transitory,
            /// <summary>
            /// Empty state, powerup has been depleted.
            /// </summary>
            BlockMode_Empty,


        }
        private string emptyimagekey = "emptyblock";
        private int _releaseCount = 1;
        public int ReleaseCount { get { return _releaseCount; } set { _releaseCount = value; hasChanged = true; } }
        private PowerupBlockMode CurrentMode = PowerupBlockMode.BlockMode_Filled;
        private Type _PowerupType = typeof(macGuffinpowerup);
        [Editor(typeof(ItemTypeEditor<GameCharacterPowerup>), typeof(UITypeEditor))]
        public Type PowerupType { get { return _PowerupType; } set { _PowerupType = value; hasChanged = true; } }

        protected override void sethasChanged(bool newvalue)
        {
            base.sethasChanged(newvalue);
            if(newvalue) cachedInnerSize = new SizeF(0, 0);
        }

        #region editor extensions
        public override void EditorDraw(Graphics g, IEditorClient Client)
        {
            //call default implementation first.
            Draw(g);
            //now, the extension for the editor is to also draw our powerup within us.
            //first, get an appropriately sized rect, the width and length is the minimum of our length and width.
            float usexy = Math.Min(BlockRectangle.Width, BlockRectangle.Height);
            //create a new bitmap of the given size.
            SizeF usesize = new SizeF(usexy,usexy);
            if (Math.Abs(cachedInnerSize.Width - usesize.Width) > 0.01f ||
                Math.Abs(cachedInnerSize.Height - usesize.Height) > 0.01f
                || hasChanged)
            {
                //recreate the cached bitmap.
                Bitmap drawinner = new Bitmap((int)usesize.Width, (int)usesize.Height);
                Graphics drawhere = Graphics.FromImage(drawinner);

                //we need to Create our powerup.
                GameCharacterPowerup gcp = (GameCharacterPowerup)Activator.CreateInstance(_PowerupType, this);

                

                gcp.DrawSize = usesize;
                gcp.Location = new PointF(0, 0);
                gcp.Draw(drawhere);
                cachedInnerSize = usesize;
                cachedInnerImage = drawinner;



            }
            //Center cachedInnerSize on our rect.
            RectangleF useDrawLocation = new RectangleF(BlockRectangle.Left+(float)((BlockRectangle.Width / 2) - cachedInnerSize.Width / 2),
                                                        BlockRectangle.Top + (float)((BlockRectangle.Height / 2) - cachedInnerSize.Height / 2), cachedInnerSize.Width, cachedInnerSize.Height);
            //now DRAW.
            if(cachedInnerImage!=null)
                g.DrawImage(cachedInnerImage, useDrawLocation);




        }
        /// <summary>
        /// size of cached inner powerup bitmap
        /// </summary>
        private SizeF cachedInnerSize;
        private Image cachedInnerImage = null;
        public override String GetToolTipInfo(IEditorClient Client)
        {
            String pluralize = ReleaseCount > 1 ? "s" : "";
            return "Contains " + ReleaseCount.ToString() + " " + _PowerupType.Name + pluralize;

        }
        #endregion


        public PowerupBlock(RectangleF Blockrect)
            : base(Blockrect, "powerupblock")
        {


        }
        public PowerupBlock(PowerupBlock clonethis)
            : base(clonethis)
        {
            _PowerupType = clonethis.PowerupType;
            _releaseCount = clonethis.ReleaseCount;

        }
        public override object Clone()
        {
            return new PowerupBlock(this);
        }

        private void TransitFunc(BCBlockGameState gstate, TransitoryBlockObject sender)
        {
            ReleaseCount--;
            if(ReleaseCount==0)
            {
                CurrentMode = PowerupBlockMode.BlockMode_Empty;
                BlockImageKey = emptyimagekey;
            }
            else
            {
                CurrentMode = PowerupBlockMode.BlockMode_Filled;
            }




           
            GameCharacterPowerup gcp = (GameCharacterPowerup)Activator.CreateInstance(_PowerupType, this);
            String emergesound = gcp.GetEmergeSound();
            BCBlockGameState.Soundman.PlaySound(emergesound);
            //gstate.GameObjects.AddLast(gcp);
            gstate.NextFrameCalls.Enqueue
                (new BCBlockGameState.NextFrameStartup((a, b) => { b.GameObjects.AddLast(gcp); return false; }, null));
            gstate.Forcerefresh = true;
            

        }
        public override void Draw(Graphics g)
        {
            if (CurrentMode == PowerupBlockMode.BlockMode_Transitory)
                return;
            else
            {
                base.Draw(g);
            }
        }
        private void BumpObjects(BCBlockGameState gstate)
        {
            //bump gameobjects above this block.
            foreach(GameObject go in gstate.GameObjects)
            {
                if (go is MushroomPower)
                {
                    Debug.Print("mushpower");

                }
                if(go is GameEnemy) 
                {
                    GameEnemy gg = (GameEnemy)go;



                    var grabrect = gg.GetRectangleF();
                    grabrect = new RectangleF(grabrect.Location, new SizeF(grabrect.Width + 2, grabrect.Height + 5));

                    var testoffset = this.BlockRectangle;
                    testoffset.Inflate(-2, -2);
                    
                    testoffset.Offset(-1, -11);

                    if (grabrect.IntersectsWith(testoffset))
                    {
                        if(gg is iBumpable)
                            ((iBumpable)gg).Bump(this);
                        



                    }
                }

            }

        }
        //returns whether _PowerUpType has the  NoEmergePowerupAttribute.
        private bool isEmergePower()
        {
            return !(BCBlockGameState.HasAttribute(_PowerupType, typeof(NoEmergePowerupAttribute)));
            
            
        }
        protected override Particle AddStandardSprayParticle(BCBlockGameState parentstate, cBall ballhit)
        {
            return null;
            //no particles.
            //return base.AddStandardSprayParticle(parentstate, ballhit);
        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            if (CurrentMode == PowerupBlockMode.BlockMode_Filled)
            {
                if (isEmergePower())
                {
                    try
                    {
                        TransitoryBlockObject tbo = new TransitoryBlockObject(this);
                        parentstate.Defer(()=>parentstate.GameObjects.AddLast(tbo));
                        tbo.TransitionComplete = TransitFunc;
                        CurrentMode = PowerupBlockMode.BlockMode_Transitory;

                        BumpObjects(parentstate);
                    }
                    catch (Exception err)
                    {
                        Debug.Print(err.ToString());

                    }
                }
                else
                {
                    //call transitfunc directly.
                    TransitFunc(parentstate, null);

                }
            }
            base.PerformBlockHit(parentstate, ballhit);
            BCBlockGameState.Soundman.PlaySound("bump");
            return false;

        }
        public override bool MustDestroy()
        {
            return false;
        }
        public PowerupBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            String getpowerupname = info.GetString("PowerupType");
            _PowerupType = BCBlockGameState.FindClass(getpowerupname);
            try { _releaseCount = info.GetInt32("ReleaseCount"); }
            catch { _releaseCount = 1; }

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PowerupType", _PowerupType.Name);
            info.AddValue("ReleaseCount", (_releaseCount));

        }



    }
}