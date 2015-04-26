using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using BASeBlock.Blocks;
using BASeBlock.Events;

namespace BASeBlock.Projectiles
{
    /// <summary>
    /// "Bullet" object, which hits blocks. However, it doesn't actually impact the block (call the block's PerformBlockHit()) until
    /// a static "health" value stored for each block exceeds a given value. At which point it does, and resets that counter.
    /// </summary>
    public class Bullet : GameObject, IMovingObject, iLocatable
    {

        private class BulletBlockDamageData
        {

            private Block _BlockObject = null;
            private int _Damage = 0;
            private int _HP = 10; //takes ten hits before we call the blocks hit routine normally.

            public Block BlockObject
            {
                get { return _BlockObject; }
                private set { _BlockObject = value; }
            }

            public int Damage
            {
                get { return _Damage; }
                set { _Damage = value; }
            }

            public int HP
            {
                get { return _HP; }
                set { _HP = value; }
            }



            public BulletBlockDamageData(Block pBlockObject, int pHP)
            {
                _BlockObject = pBlockObject;
                _HP = pHP;


            }

        }

        private static readonly Brush DefaultBulletBrush = new SolidBrush(Color.Yellow);
        private static BCBlockGameState LastState;
        private static Dictionary<Block, BulletBlockDamageData> DamageData = new Dictionary<Block, BulletBlockDamageData>();
        protected Object _Owner = null; //set to whatever or whoever shot this.
        protected PointF _Location;
        protected PointF _Velocity;
        protected Brush _BulletBrush = DefaultBulletBrush;
        private bool _DamagePaddle = false;
        public PointF Velocity { get { return _Velocity; } set { _Velocity = value; } }
        public PointF Location { get { return _Location; } set { _Location = value; } }
        public bool DamagePaddle { get { return _DamagePaddle; } set { _DamagePaddle = value; } }
        public Brush BulletBrush { get { return _BulletBrush; } set { _BulletBrush = value; } }
        public Object Owner { get { return _Owner; } set { _Owner = value; } }
        /// <summary>
        /// used to perform a single frame of this gameobjects animation.
        /// </summary>
        /// <param name="gamestate">Game State object</param>
        /// <param name="AddObjects">out parameter, populate with any new GameObjects that will be added to the game.</param>
        /// <param name="removeobjects">otu parameter, populate with gameobjects that should be deleted.</param>
        /// <returns>true to indicate that this gameobject should be removed. False otherwise.</returns>
        /// 
        public Bullet(PointF pPosition, PointF pVelocity)
            : this(pPosition, pVelocity, false)
        {

        }
        public Bullet(PointF pPosition, PointF pVelocity, bool pDamagePaddle)
        {
            Location = pPosition;
            Velocity = pVelocity;
            _DamagePaddle = pDamagePaddle;
        }
        private static readonly int Default_HP = 10;
        private BulletBlockDamageData getDamageData(Block forblock)
        {
            //returns or creates the bulletBlockDamage data for the given block.
            if (DamageData.ContainsKey(forblock))
                return DamageData[forblock];
            else
            {
                BulletBlockDamageData returnthis = new BulletBlockDamageData(forblock, Default_HP);
                DamageData.Add(forblock, returnthis);
                return returnthis;
            }



        }
        private void AttachToBlock(Block attachto)
        {
            AttachedBlock = attachto;
            AttachedBlock.OnBlockDestroy += AttachedBlock_OnBlockDestroy;

        }

        void AttachedBlock_OnBlockDestroy(Object Sender,BlockHitEventArgs<bool> e)
        {
            AttachedBlock = null;
            isdestroyed = true;
            e.Result = true;
        }
        private int _DamageAmount = 2; //amount of damage this bullet will do if it hits the paddle.
        private int frameinc = 0;
        private bool isdestroyed = false;
        private Block AttachedBlock = null; //The bullet's "stick" to the block they hit, but don't do damage.

        protected virtual bool Impact(BCBlockGameState gamestate,Block smackblock)
        {

            BulletBlockDamageData bbd = getDamageData(smackblock);

            bbd.Damage++;
            if (bbd.Damage >= bbd.HP)
            {
                //destroy(or rather, perform a "standard" block hit, which may or may not destroy it.
                BCBlockGameState.Block_Hit(gamestate, smackblock);
                bbd.Damage = 0; //reset to zero.
            }
            else
            {
                //don't destroy. We still make a sound though.

                BCBlockGameState.EmitBlockSound(gamestate, smackblock);
                //attach...
                if (smackblock.MustDestroy())
                    AttachToBlock(smackblock);
                else
                {
                    return true;
                }
            }
            return false;

        }

        //The point being that you can't see how much damage you've dne to them so just seeing how many spots are stuck to the side can help.
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            frameinc++;
            if (frameinc == 21) frameinc = 0;
            if (AttachedBlock != null)
            {
                //TODO: add code to move the bullet if the block moves.
                if (frameinc >= 20)
                {
                    if (!gamestate.Blocks.Contains(AttachedBlock))
                        return true;
                }

                return false;


            }
            if (_DamagePaddle)
            {

                if (gamestate.PlayerPaddle != null)
                {
                    if (gamestate.PlayerPaddle.Getrect().Contains(new Point((int)Location.X, (int)Location.Y)))
                    {
                        //damage.
                        gamestate.PlayerPaddle.HP -= _DamageAmount;
                        return true;
                    }

                }


            }
            if (isdestroyed) return true;
            if (gamestate != LastState)
            {
                LastState = gamestate;
                DamageData = new Dictionary<Block, BulletBlockDamageData>();


            }
                
            BCBlockGameState.IncrementLocation(gamestate, ref _Location, Velocity);
            //use hittest to see if there are blocks...
            List<Block> hittest = BCBlockGameState.Block_HitTest(gamestate.Blocks.ToList(), Location);

             
            if (hittest.Count > 0)
            {
                gamestate.Forcerefresh = true;
                foreach (Block smackblock in hittest)
                {
                    if (smackblock != Owner)
                    {
                        try
                        {

                            if (Impact(gamestate, smackblock)) return true;
                        }
                        catch (Exception exx)
                        {

                            Debug.Print(exx.ToString());

                        }
                    } //smackblock!=owner
                }
                //return true; //destroy the bullet.

            }

                
            var alleyeguys = (from m in gamestate.GameObjects where m is EyeGuy select m);
            var allcharacters = (from m in gamestate.GameObjects where m is PlatformObject select m);
            if (alleyeguys.Count() > 0)
            {
                foreach (EyeGuy iterateguy in alleyeguys)
                {
                    if (iterateguy.GetRectangleF().Contains(Location))
                    {
                        if (iterateguy != Owner)
                        {
                            iterateguy.HitPoints -= 5;
                        }
                    }
                    

                }



            }
            if (allcharacters.Count() > 0)
            {
                foreach (PlatformObject iteratechar in allcharacters)
                {
                    if (iteratechar.GetRectangleF().Contains(Location))
                        iteratechar.Die(gamestate);


                }


            }


            if (!gamestate.GameArea.Contains(new Point((int)Location.X, (int)Location.Y)))
            {
                return true;

            }
            else
            {
                return false;
            }
            //move the bullet...




        }

        public override void Draw(Graphics g)
        {
            //throw new NotImplementedException();
            g.FillRectangle(BulletBrush, Location.X - 1, Location.Y - 1, 2, 2);
        }
    }
}