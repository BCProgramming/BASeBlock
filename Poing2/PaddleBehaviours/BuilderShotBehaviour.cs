using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BASeBlock.Blocks;
using BASeBlock.Events;
using BASeBlock.GameStates;

namespace BASeBlock.PaddleBehaviours
{
    /// <summary>
    /// the "BuilderShot" is a projectile that is fired and spawns a block of a specific type and size at the destination location.
    /// 
    /// </summary>
    public class BuilderShotBehaviour : BasePaddleBehaviour
    {
        //the buildershot itself.


        private BuilderShot CurrentShot = null;
        private Type[] _BlockBuildTypes = new Type[] {typeof (StrongBlock), typeof (InvincibleBlock)};
        private int _PowerLevel = 0;
        private TimeSpan cooldown = new TimeSpan(0, 0, 0, 1);
        private BCBlockGameState gstate = null;
        private DateTime lastshot = DateTime.Now;
        private Paddle ownerpaddle = null;

        public BuilderShotBehaviour(BCBlockGameState stateobject)
        {
            gstate = stateobject;
            //stateobject.ClientObject.ButtonDown += new Func<ButtonConstants, bool>(ClientObject_ButtonDown);
            //stateobject.ClientObject.ButtonUp += new Func<ButtonConstants, bool>(ClientObject_ButtonUp);

            gstate.ClientObject.ButtonDown += ClientObject_ButtonDown;
        }


        public int PowerLevel
        {
            get { return _PowerLevel; }
            set
            {
                _PowerLevel = BCBlockGameState.ClampValue(value, 0, _BlockBuildTypes.Length - 1);
                RefreshAntecedent();
            }
        }

        public Type[] BlockBuildTypes
        {
            get { return _BlockBuildTypes; }
            set
            {
                _BlockBuildTypes = value;
                RefreshAntecedent();
            }
        }

        public Type BlockBuildType
        {
            get { return _BlockBuildTypes[_PowerLevel]; }
            set { _BlockBuildTypes[_PowerLevel] = value; }
        }

        public String GetDescription()
        {
            return BlockBuildType.Name;
        }

        private void RefreshAntecedent()
        {
            //change state of CurrentShot to reflect changes in the "Behaviour" object. (us)
            if (CurrentShot != null)
            {
                CurrentShot.BuildBlock = BlockBuildType;
            }
        }

        /// <summary>
        /// acquires the icon for this behaviour.
        /// </summary>
        /// <returns>
        /// an image to represent the behaviour.
        /// </returns>
        public override Image GetIcon()
        {
            return BCBlockGameState.Imageman.getLoadedImage("INVINCIBLE");
        }

        public override string getName()
        {
            return "Builder";
        }

        public override void Draw(Paddle onPaddle, Graphics g)
        {
            PointF Position = onPaddle.Position;
            SizeF PaddleSize = onPaddle.PaddleSize;
            RectangleF drawrect = new RectangleF(Position.X - PaddleSize.Width/2, Position.Y - PaddleSize.Height/2,
                                                 PaddleSize.Width, PaddleSize.Height);
            //g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Green)), drawrect);
            //draw the "sticky" overlay...
            g.DrawImage(BCBlockGameState.Imageman.getLoadedImage("BUILDPADDLE"), drawrect);
        }

        public override bool Impact(Paddle onPaddle, cBall withBall)
        {
            return false;
        }

        public override void BehaviourAdded(Paddle toPaddle, BCBlockGameState gamestate)
        {
            ownerpaddle = toPaddle;

            //remove any existing terminator behaviour.
            foreach (var loopbeh in toPaddle.Behaviours)
            {
                if (loopbeh is TerminatorBehaviour)
                    gstate.GameObjects.AddLast(new BehaviourRemoverProxy(toPaddle, loopbeh));
            }

            base.BehaviourAdded(toPaddle, gamestate);
        }

        /// <summary>
        /// determines if there is a shot being... er... shot.
        /// </summary>
        /// <returns></returns>
        private bool CheckCurrentShot()
        {
            if (CurrentShot == null) return false;
            if (CurrentShot.ShotState != BuilderShot.BuilderShotState.BSS_Projectile)
                CurrentShot = null;

            return CurrentShot != null;
        }

        private void ClientObject_ButtonDown(Object sender, ButtonEventArgs<bool> e)
        {
            if (gstate.PlayerPaddle != null && !gstate.PlayerPaddle.Behaviours.Contains(this))
            {
                //unhook
                gstate.ClientObject.ButtonDown -= ClientObject_ButtonDown;

                return;
            }
            //don't shoot if we are not in the "run" state.
            if (gstate.ClientObject.ActiveState is StateRunning)
            {
                if (e.Button == ButtonConstants.Button_B)
                {
                    ShootBuilder();
                }
            }
            e.Result = true;
        }

        private void ShootBuilder()
        {
            if (CheckCurrentShot())
            {
                //only allow for growification when it get's higher than 128.
                if (CurrentShot.Location.Y < gstate.GameArea.Height - 128)
                {
                    CurrentShot.ExpandPhase();
                    lastshot = DateTime.Now;
                }
            }
            //shoot a BuilderShot.
            if (DateTime.Now - lastshot > cooldown)
            {
                if (!CheckCurrentShot())
                {
                    BuilderShot shootthis = new BuilderShot(ownerpaddle.Getrect().CenterPoint(), new PointF(0, -2),
                                                            BlockBuildType);
                    gstate.GameObjects.AddLast(shootthis);
                    CurrentShot = shootthis;
                }
            }
        }

        public override void BehaviourRemoved(Paddle fromPaddle, BCBlockGameState gamestate)
        {
            if (gstate != null)
            {
                gstate.ClientObject.ButtonDown -= ClientObject_ButtonDown;
                base.BehaviourRemoved(fromPaddle, gamestate);
            }
        }

        public class BuilderShot : Projectile, iSizedProjectile
        {
            public enum BuilderShotState
            {
                BSS_Projectile = 0,
                BSS_Expand = 1 //expanding phase, before it turns into a block.
            }

            private const int increments = 32;
            private SizeF? Growincrement = null;
            private Type[] _BuildBlocks = new Type[] {typeof (StrongBlock), typeof (InvincibleBlock)};
            private SizeF _BuildSize = new SizeF(32, 16);
            private int _PowerLevel = 0;
            private SizeF _ShotSize = new SizeF(8, 8);

            private BuilderShotState _ShotState = BuilderShotState.BSS_Projectile;

            private Image useDrawImage = null;

            public BuilderShot(PointF pLocation, PointF pVelocity, Type pBuildBlock)
                : base(pLocation, pVelocity)
            {
                BuildBlock = pBuildBlock;
            }

            public BuilderShotState ShotState
            {
                get { return _ShotState; }
                set { _ShotState = value; }
            }

            public SizeF ShotSize
            {
                get { return _ShotSize; }
                set
                {
                    _ShotSize = value;
                    BuildBlock = BuildBlock; //invoke routine... 
                }
            }

            public SizeF BuildSize
            {
                get { return _BuildSize; }
                set { _BuildSize = value; }
            }


            public int PowerLevel
            {
                get { return _PowerLevel; }
                set { _PowerLevel = BCBlockGameState.ClampValue(value, 0, _BuildBlocks.Length); }
            }

            public Type BuildBlock
            {
                get { return _BuildBlocks[_PowerLevel]; }
                set
                {
                    _BuildBlocks[_PowerLevel] = value;
                    Bitmap buildbitmap = null;
                    Graphics buildcanvas = null;
                    //create and draw the block.
                    Block.DrawBlock(_BuildBlocks[_PowerLevel], out buildbitmap, out buildcanvas, _ShotSize);
                    useDrawImage = buildbitmap;
                }
            }

            public SizeF Size
            {
                get { return _ShotSize; }
                set { _ShotSize = value; }
            }

            public void ExpandPhase()
            {
                //enter expansion phase.
                _ShotState = BuilderShotState.BSS_Expand;
                _Velocity = new PointF(0, 0); //full stop, ensign.
            }

            private bool forcerefresher(ProxyObject po, BCBlockGameState gs)
            {
                gs.Forcerefresh = true;
                return true; //destroy.
            }

            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                bool returnvalue = base.PerformFrame(gamestate);


                switch (_ShotState)
                {
                    case BuilderShotState.BSS_Projectile:

                        List<Block> resulthittest = BCBlockGameState.Block_HitTest(gamestate.Blocks, getfullsize(),
                                                                                   false);
                        returnvalue = !returnvalue || resulthittest.Any();
                        if (returnvalue)
                        {
                            _ShotState = BuilderShotState.BSS_Expand;
                            Velocity = new PointF(0, 0.01f);
                            gamestate.Forcerefresh = true;
                        }
                        return false;

                    case BuilderShotState.BSS_Expand:
                        //if our size is the desired size of the block, create that block and return true.
                        //otherwise, change out size and location to emulate "growing".
                        if (this.ShotSize.Width >= BuildSize.Width &&
                            ShotSize.Height >= BuildSize.Height)
                        {
                            //grow phase completed.
                            //create the block in the desired location.
                            RectangleF desiredlocation = new RectangleF(Location.X, Location.Y, BuildSize.Width,
                                                                        BuildSize.Height);
                            Block builtblock = (Block) Activator.CreateInstance(this.BuildBlock, desiredlocation);
                            //add it to the game...
                            gamestate.Blocks.AddLast(builtblock);
                            //make sure to force a refresh, too.
                            gamestate.Forcerefresh = true;
                            gamestate.Defer(
                                () => gamestate.GameObjects.AddLast(new ProxyObject(forcerefresher, null)));
                            //return true to destroy outselves.
                            //todo: maybe add "effects" here, too?
                            return true;
                        }
                        else
                        {
                            //otherwise, we are in the growing phase.
                            //growincrement could be null, if so, initialize it...
                            if (Growincrement == null)
                            {
                                //initialize it to the difference between the final size and the current shot size, divided
                                //by increments.
                                Growincrement = new SizeF((BuildSize.Width - ShotSize.Width)/increments,
                                                          (BuildSize.Height - ShotSize.Height)/increments);
                            }
                            //change size by growincrement.
                            Location = new PointF(Location.X - Growincrement.Value.Width,
                                                  Location.Y - Growincrement.Value.Height);
                            ShotSize = new SizeF(ShotSize.Width + Growincrement.Value.Width*2,
                                                 ShotSize.Height + Growincrement.Value.Height*2);
                        }
                        return false;
                }
                return false;
            }

            public override void Draw(Graphics g)
            {
                g.DrawImage(useDrawImage, _Location.X, _Location.Y, _ShotSize.Width, _ShotSize.Height);
            }

            /// <summary>
            /// returns the rectangle that would hold the full sized block on this position.
            /// </summary>
            /// <returns></returns>
            private RectangleF getfullsize()
            {
                PointF gotcenter = new PointF(_Location.X + _ShotSize.Width/2, _Location.Y + _ShotSize.Height/2);
                return new RectangleF(gotcenter.X - BuildSize.Width/2, gotcenter.Y - BuildSize.Height/2,
                                      BuildSize.Width, BuildSize.Height);
            }
        }
    }
}