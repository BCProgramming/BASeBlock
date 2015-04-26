using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using BASeBlock.Blocks;
using BASeBlock.Events;
using BASeBlock.GameObjects.Orbs;
using BASeBlock.Particles;

namespace BASeBlock
{




    public class GameCharacter : PlatformObject, iBumpable
    {
        private List<GameCharacterAbility> _Abilities = new List<GameCharacterAbility>();
        private bool flInit = false;

        public List<GameCharacterAbility> Abilities { get { return _Abilities; } set { _Abilities = value; } }


        public GameCharacter(PointF pPosition)
            : this(pPosition, new SizeF(16, 16))
        {
        }
        public GameCharacter(PointF pPosition, SizeF pSize)
            : this(pPosition, new Nullable<SizeF>(pSize))
        { }
        public GameCharacter(PointF pPosition, SizeF? pSize)
            : base(pPosition, new PointF(0, 0), null, 6, null)
        {

            foreach (var iterate in Enum.GetValues(typeof(ButtonConstants)))
            {
                Buttonspressed[(ButtonConstants)iterate] = false;

            }

            
            StateFrameImageKeys = new Dictionary<string, string[]>();
            FrameDelayTimes = new Dictionary<string, int[]>();
            StateFrameImageKeys.Add("IDLELEFT", new string[] { "FLIPX:CHAR_STAND" });
            StateFrameImageKeys.Add("IDLERIGHT", new string[] { "CHAR_STAND" });
            StateFrameImageKeys.Add("WALKLEFT", new string[] { "FLIPX:CHAR_STAND", "FLIPX:CHAR_WALK" });
            StateFrameImageKeys.Add("WALKRIGHT", new string[] { "CHAR_STAND", "CHAR_WALK" });
            StateFrameImageKeys.Add("JUMPLEFT", new string[] { "FLIPX:CHAR_JUMP" });
            //StateFrameImageKeys.Add("JUMPLEFTUP", new string[] { "FLIPX:CHAR_JUMP" });
            StateFrameImageKeys.Add("JUMPRIGHT", new string[] { "CHAR_JUMP" });
            //StateFrameImageKeys.Add("JUMPRIGHTUP", new string[] { "CHAR_JUMP" });
            StateFrameImageKeys.Add("STOPLEFT", new string[] { "FLIPX:CHAR_TURN" });
            StateFrameImageKeys.Add("STOPRIGHT", new string[] { "CHAR_TURN" });
            StateFrameImageKeys.Add("DYING", new string[] { "CHAR_DEAD" }); //shows dead, waits half a second then advanced to "DEATH" stage.
            StateFrameImageKeys.Add("DEATH", new string[] { "CHAR_DEAD" });

            StateFrameIndex = new Dictionary<string, int>();
            StateFrameIndex.Add("IDLELEFT", 0);
            StateFrameIndex.Add("IDLERIGHT", 0);
            StateFrameIndex.Add("WALKLEFT", 0);
            StateFrameIndex.Add("WALKRIGHT", 0);
            StateFrameIndex.Add("JUMPLEFT", 0);
            //StateFrameIndex.Add("JUMPLEFTUP", 0);
            StateFrameIndex.Add("JUMPRIGHT", 0);
            //StateFrameIndex.Add("JUMPRIGHTUP", 0);
            StateFrameIndex.Add("STOPLEFT", 0);
            StateFrameIndex.Add("STOPRIGHT", 0);
            StateFrameIndex.Add("DYING", 0);
            StateFrameIndex.Add("DEATH", 0);

            FrameDelayTimes = new Dictionary<string, int[]>();
            FrameDelayTimes.Add("IDLELEFT", new int[] { 2 });
            FrameDelayTimes.Add("IDLERIGHT", new int[] { 2 });
            FrameDelayTimes.Add("WALKLEFT", new int[] { 2, 2 });
            FrameDelayTimes.Add("WALKRIGHT", new int[] { 2, 2 });
            FrameDelayTimes.Add("JUMPLEFT", new int[] { 2 });
            //FrameDelayTimes.Add("JUMPLEFTUP", new int[] { 2 });
            FrameDelayTimes.Add("JUMPRIGHT", new int[] { 2 });
            //FrameDelayTimes.Add("JUMPRIGHTUP", new int[] { 2 });
            FrameDelayTimes.Add("STOPLEFT", new int[] { 2 });
            FrameDelayTimes.Add("STOPRIGHT", new int[] { 2 });
            FrameDelayTimes.Add("DYING", new int[] { 2 });
            FrameDelayTimes.Add("DEATH", new int[] { 2 });
            EnemyAction = "IDLELEFT";
            if (pSize != null) useSize = pSize.Value;

            InitActions();
            //_ActionIndex.Add("JUMPLEFTUP", "JUMPLEFT");
            //_ActionIndex.Add("JUMPRIGHTUP", "JUMPRIGHT");



        }
        private DateTime? DeadTime = null;

     
        public bool GetPowerup(BCBlockGameState gstate, GameCharacterPowerup power)
        {
            return power.GivePowerTo(gstate, this);


        }
        public override void Bump(Block bumpedby)
        {
            base.Bump(bumpedby);


        }
        public override bool Die(BCBlockGameState gstate)
        {
            //play death x (mardeathx)
            if (_EnemyState != "DEATH" && _EnemyState != "DYING")
            {

                if (ptd.TrackBlock != null)
                {
                    if (ptd.TrackBlock is iPlatformBlockExtension)
                        ((iPlatformBlockExtension)ptd.TrackBlock).Standon(gstate, this, false);


                }

                ptd.TrackBlock = null; //set to null to stop tracking if we were on a block.
                BCBlockGameState.Soundman.PlaySound("MARDEATHX");
                Velocity = new PointF(0, 0);
                GravityEffect = new PointF(0, 0);
                //set dead time
                DeadTime = DateTime.Now;
                //change state to DYING
                EnemyAction = "DYING";


            }
            return true;
        }
        protected override void hitside(BCBlockGameState gamestate, List<Block> hitblocks)
        {
            //do nothing.
        }
        private bool isonground { get { return onground || ongroundlastframe; } }


        public override CollideTypeConstants Collide(BCBlockGameState gstate, PlatformObject otherobject)
        {
            bool currreturn = false;
            foreach (GameCharacterAbility gca in _Abilities)
            {
                currreturn = gca.Collide(gstate,this, otherobject);



            }

            if (!currreturn) return base.Collide(gstate,otherobject);
            else return CollideTypeConstants.Collide_Nothing;


        }
        public override void Draw(Graphics g)
        {
            //save current draw properties.
            //call each powerups beforedraw routine.
            //draw.
            //reset draw properties.

            ImageAttributes cacheAttributes = DrawAttributes;
            if (_Abilities.Any())
            {
                foreach (GameCharacterAbility gma in _Abilities)
                {

                    gma.BeforeDraw(this, g);

                }


            }


            base.Draw(g);
            DrawAttributes = cacheAttributes;
        }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            if (_Abilities.Any())
            {
                List<GameCharacterAbility> addabilities = new List<GameCharacterAbility>();
                List<GameCharacterAbility> removeabilities = new List<GameCharacterAbility>();

                foreach (GameCharacterAbility gma in _Abilities)
                {
                    if (gma.PerformFrame(this, gamestate))
                        removeabilities.Add(gma);


                }
                foreach (var removeit in removeabilities)
                {
                    Abilities.Remove(removeit);
                }
                foreach (var addit in addabilities)
                {
                    Abilities.Add(addit);
                }

            }
            bool nobasecall = false;
            if (!flInit)
            {
                flInit = true;
                //hook the GameClient events...
                gamestate.ClientObject.ButtonDown += ClientObject_ButtonDown;
                gamestate.ClientObject.ButtonUp += ClientObject_ButtonUp;

            }
            if (EnemyAction == "DEATH") return base.PerformFrame(gamestate);
            if (EnemyAction == "DYING")
            {
                if (DateTime.Now - DeadTime.Value > new TimeSpan(0, 0, 0, 1))
                {
                    EnemyAction = "DEATH";
                    BCBlockGameState.Soundman.PlaySound("MARDEATH2");
                    Velocity = new PointF(0, -4);
                    _GravityEffect = new PointF(0, 8f);
                    Nocollisions = true;
                    nobasecall = true;

                }

                return base.PerformFrame(gamestate);

            }

            /*    case Keys.W:
         return ButtonConstants.Button_E;
     case Keys.A:
         return ButtonConstants.Button_F;
     case Keys.S:
         return ButtonConstants.Button_G;
     case Keys.D:
         return ButtonConstants.Button_H;*/
            if (EnemyAction == "IDLELEFT" || EnemyAction == "IDLERIGHT")
            {
                if (Buttonspressed[ButtonConstants.Button_F])
                {
                    EnemyAction = "WALKLEFT";


                }
                else if (Buttonspressed[ButtonConstants.Button_H])
                {
                    EnemyAction = "WALKRIGHT";

                }

            }



            switch (EnemyAction)
            {
                case "IDLELEFT":
                    if (!isonground) EnemyAction = "JUMPLEFT";
                    break;
                case "IDLERIGHT":
                    if (!isonground) EnemyAction = "JUMPRIGHT";
                    break;
                case "JUMPLEFT":
                    if (isonground)
                        EnemyAction = "WALKLEFT";
                    Decelerate();
                    break;
                case "JUMPRIGHT":
                    if (isonground)
                        EnemyAction = "WALKRIGHT";
                    Decelerate();
                    break;
                case "WALKLEFT":

                    Decelerate();
                    if (Math.Abs(Velocity.X) < 0.1) EnemyAction = "IDLELEFT";
                    if (!isonground) EnemyAction = "JUMPLEFT";
                    break;
                case "WALKRIGHT":
                    Decelerate();
                    if (Math.Abs(Velocity.X) < 0.1) EnemyAction = "IDLERIGHT";
                    if (!isonground) EnemyAction = "JUMPRIGHT";
                    break;



            }

            //check for ball collisions.
            foreach (var loopball in gamestate.Balls)
            {
                Polygon ppoly = loopball.GetBallPoly();
                Polygon boundbox = new Polygon(this.GetRectangleF());
                var result = GeometryHelper.PolygonCollision(ppoly, boundbox, new Vector(0, 0));
                if (result.Intersect)
                {
                    Die(gamestate);
                    break;

                }



            }




            if (!nobasecall) return base.PerformFrame(gamestate); else return false;
        }
        private void Decelerate()
        {
            Decelerate(false);

        }
        private float maxXSpeed = 7;
        /// <summary>
        /// managed acceleration/deceleration when keys are pressed/unpressed.
        /// </summary>
        /// <param name="fast"></param>
        private void Decelerate(bool fast)
        {



            float reductionfactor = fast ? 0.6f : 0.9f;
            //decelerate.

            //if idle and a button is pressed, change state to that direction walking.





            if (_EnemyState == "IDLELEFT" || _EnemyState == "WALKLEFT" || _EnemyState == "JUMPLEFT")
            {
                if (Buttonspressed[ButtonConstants.Button_H])
                {
                    if (Math.Abs(Velocity.X) < 0.1f)
                    {
                        _EnemyState = _EnemyState.Replace("LEFT", "RIGHT");
                        //swap to right facing...

                    }
                    else
                    {
                        reductionfactor = 0.6f;
                    }
                }
                if ((!Buttonspressed[ButtonConstants.Button_F]))
                {
                    Velocity = new PointF(Velocity.X * reductionfactor, Velocity.Y);
                    if (Math.Abs(Velocity.X) < 0.5) Velocity = new PointF(0, Velocity.Y);
                }
                else
                {
                    //if it is pressed, increase to a maximum.
                    if (Math.Abs(Velocity.X) < maxXSpeed)
                    {
                        Velocity = new PointF(Velocity.X - 0.4f, Velocity.Y);
                    }
                }



            }
            else if (_EnemyState == "IDLERIGHT" || _EnemyState == "WALKRIGHT" || _EnemyState == "JUMPRIGHT")
            {
                if (Buttonspressed[ButtonConstants.Button_F])
                {
                    if (Velocity.X < 0.1f)
                    {
                        _EnemyState = _EnemyState.Replace("RIGHT", "LEFT");
                        //swap to Left facing...
                    }
                    else
                    {
                        reductionfactor = 0.6f;
                    }
                }


                if ((!Buttonspressed[ButtonConstants.Button_H]))
                {

                    Velocity = new PointF(Velocity.X * reductionfactor, Velocity.Y);
                    if (Math.Abs(Velocity.X) < 0.5) Velocity = new PointF(0, Velocity.Y);
                }
                else
                {
                    if (Math.Abs(Velocity.X) < maxXSpeed)
                    {
                        Velocity = new PointF(Velocity.X + 0.4f, Velocity.Y);
                    }

                }
            }

        }
        private Dictionary<ButtonConstants, bool> Buttonspressed = new Dictionary<ButtonConstants, bool>();
        void ClientObject_ButtonUp(Object sender,ButtonEventArgs<bool> e )
        {
            Buttonspressed[e.Button] = false;
            if (e.Button == ButtonConstants.Button_A)
            {
                //GravityEffect = new PointF(0, 1f);

            }
            e.Result = true;
        }
        public override void TouchLeft(BCBlockGameState gamestate, List<Block> touched, Block mainblock)
        {
            Velocity = new PointF(0, Velocity.Y);
        }
        public override void TouchRight(BCBlockGameState gamestate, List<Block> touched, Block mainblock)
        {
            Velocity = new PointF(0, Velocity.Y);
        }
        protected override void TouchLevelSide(BCBlockGameState gamestate, ref List<GameObject> Addobjects, ref List<GameObject> removeobjects, bool LeftSide)
        {
            Velocity = new PointF(0, Velocity.Y);
            //base.TouchLevelSide(gamestate, ref Addobjects, ref removeobjects, LeftSide);
        }
        public override bool TouchPaddle(BCBlockGameState gamestate, Paddle PlayerPaddle)
        {
            
            Velocity = new PointF(Velocity.X, 0);
            /*if (EnemyAction == "JUMPLEFT" || EnemyAction == "JUMPRIGHT")
                EnemyAction = EnemyAction.Replace("JUMP", "IDLE");*/
            return false;
        }
        public override void TouchTop(BCBlockGameState gamestate, List<Block> touched, Block mainblock)
        {
            Velocity = new PointF(Velocity.X, Math.Abs(Velocity.Y * 0.5f));
            Block smackit = touched.First();
            BCBlockGameState.Block_Hit(gamestate, smackit, Velocity); //smack it with the player's velocity.
            gamestate.Forcerefresh = true;
            gamestate.Defer(() => gamestate.Forcerefresh = true);
            
        }
        protected override void standingon(BCBlockGameState gamestate, List<Block> standingon, Block Mainblock)
        {
            onground = true;
        }
        /*               case Keys.W:
                    return ButtonConstants.Button_E;
                case Keys.A:
                    return ButtonConstants.Button_F;
                case Keys.S:
                    return ButtonConstants.Button_G;
                case Keys.D:
                    return ButtonConstants.Button_H; */
        void  ClientObject_ButtonDown(Object sender,ButtonEventArgs<bool> e)
        {
            lock (this)
            {

                Buttonspressed[e.Button] = true;
                if (_EnemyState == "DYING" || _EnemyState == "DEATH") { e.Result = false; return; }

                if (e.Button == ButtonConstants.Button_D)
                {
                    //GravityEffect = new PointF(0, 0.5f);
                    //jump...
                    if (_EnemyState != "JUMPLEFT" && _EnemyState != "JUMPRIGHT" && _EnemyState != "DYING" && _EnemyState != "DEATH")
                    {

                        if (_EnemyState == "IDLELEFT" || _EnemyState == "WALKLEFT")
                            _EnemyState = "JUMPLEFT";
                        else if (_EnemyState == "IDLERIGHT" || _EnemyState == "WALKRIGHT")
                            _EnemyState = "JUMPRIGHT";


                        BCBlockGameState.Soundman.PlaySound("SMBJUMP");
                        Velocity = new PointF(Velocity.X, -8 - (Math.Abs(Velocity.X / 3))); //jump!
                        Location = new PointF(Location.X, Location.Y - 3);
                        onground = false;
                        ongroundlastframe = false; //we need to set these otherwise jumping would suck.

                    }

                }
            }
            e.Result = true;
            return;
        }
        public override float GetGroundOffset()
        {
            return 3;
        }

    }

    //analogous to the coins in Mario
    public class macGuffinpowerup : GameCharacterPowerup
    {
            static String addedkey =  "";
            static readonly TimeSpan Poptime = new TimeSpan(0, 0, 0, 0,500);
            private GameStopwatch gsw = null;

        private static String getOrbKey()
        {
            if (addedkey == "")
            {
                addedkey = "ORB";
                BCBlockGameState.Imageman.AddImage(addedkey, BCBlockGameState.GetSphereImage(Color.Blue,new Size(10,10)));
                
            }
            return addedkey;
        }
        public override string GetEmergeSound()
        {
            return "COIN";
        }
        public override void Bump(Block bumpedby)
        {
            return;
        }
        public override void TouchTop(BCBlockGameState gamestate, List<Block> touched, Block mainblock)
        {
            return;
        }
        public override void TouchLeft(BCBlockGameState gamestate, List<Block> touched, Block mainblock)
        {
            return;
        }
        public override void TouchRight(BCBlockGameState gamestate, List<Block> touched, Block mainblock)
        {
            return;
        }
        protected override void TouchLevelSide(BCBlockGameState gamestate, ref List<GameObject> Addobjects, ref List<GameObject> removeobjects, bool LeftSide)
        {
            return;
        }
        protected override void standingon(BCBlockGameState gamestate, List<Block> standingon, Block Mainblock)
        {
            return;
        }
        public macGuffinpowerup(Block EmergeFrom):base(EmergeFrom,getOrbKey(),20)
        {
            Emerging = false;
            GravityEffect = new PointF(0, 2);
            Velocity = new PointF(0, -5);
            DrawSize = new SizeF(10, 10);
            Location = new PointF(EmergeFrom.CenterPoint().X-DrawSize.Width/2,EmergeFrom.BlockRectangle.Top-DrawSize.Height);
        }

        public override bool GivePowerTo(BCBlockGameState gamestate, GameCharacter gamechar)
        {
            return false;
        }
        private Particle Defaultemitterroutine(BCBlockGameState gstate, EmitterParticle source, int framenum, int ttl)
        {


            FireParticle p = new FireParticle(new PointF(source.Location.X, source.Location.Y));
            p.TTL = 60;
            p.Velocity = BCBlockGameState.GetRandomVelocity(0, 2);
            return p;
        }
        public override bool Powerup_PerformFrame(BCBlockGameState gamestate)
        {
            GravityEffect = new PointF(0, 0);
            if (gsw == null) gsw = new GameStopwatch(gamestate);
            //throw new NotImplementedException();
            //Location = new PointF(Location.X + Velocity.X, Location.Y + Velocity.Y);

            //get current time...
            long elapsedticks = gsw.Elapsed.Ticks;
            long lengthticks = Poptime.Ticks;

            //first, if our time is up, we're done.
            if (elapsedticks > lengthticks || Math.Abs(Velocity.Y) < 0.1f)
            {
                //spawn some particles.
                for (int i = 0; i < 10; i++)
                {
                    EmitterParticle createdemitter = new EmitterParticle(this.GetRectangleF().CenterPoint(), Defaultemitterroutine);
                    createdemitter.Velocity = BCBlockGameState.GetRandomVelocity(0, 5);
                    gamestate.Particles.Add(createdemitter);

                }
                gamestate.SpawnOrbs(45, GetRectangleF().CenterPoint(), typeof(MacGuffinOrb));
                gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => gamestate.GameObjects.Remove(this)));
                return true; //destroy us NAU...
            }
            else
            {
                
                //get percentage through...
                float percentcomplete = ((float)elapsedticks) / ((float)lengthticks);
                
                //set velocity.Y to -(1-Sin(percentcomplete)...
                //Velocity = new PointF(Velocity.X, -2*(float)(Math.Abs(1 - Math.Sin(percentcomplete))));
                if(Velocity.Y < 0)
                    Velocity = new PointF(Velocity.X, Velocity.Y +0.5f);

                Debug.Print("percentage through:" + percentcomplete + Velocity);
            }
            Debug.Print("Velocity is " + Velocity);
           
            return false;
        }
        
    }
        
    public class InvinciblePowerup : GameCharacterPowerup
    {

        public InvinciblePowerup(Block EmergeFrom)
            : base(EmergeFrom, BCBlockGameState.Imageman.getImageFramesString("vc"), 3)
        {

            GravityEffect = new PointF(0, 30f);


        }
        public InvinciblePowerup(PointF pPosition, SizeF psize)
            : base(pPosition, new PointF(1, -3), BCBlockGameState.Imageman.getImageFramesString("vc"), 3)
        {
            GravityEffect = new PointF(0, 30f);

        }
        public override void EmergeComplete(Block EmergedFrom, BCBlockGameState gstate)
        {
            Velocity = new PointF(0.75f, -2f);
        }
        public override bool GivePowerTo(BCBlockGameState gamestate, GameCharacter gamechar)
        {
            bool founditem = false;
            foreach (GameCharacterAbility gca in gamechar.Abilities)
            {

                if (gca is InvinciblePowerupAbility)
                {
                    InvinciblePowerupAbility ipa = gca as InvinciblePowerupAbility;
                    ipa.AbilityStart = DateTime.Now; //reset.
                    ipa.ChangeDelayTime(gamestate, gamestate.GetDelayData(ipa.DelayIdentifier).waitdelay,true);
                    founditem = true;
                    break;

                }


            }
            if (!founditem)
            {
                //if there is no invincible powerup, we add it.
                InvinciblePowerupAbility newpower = new InvinciblePowerupAbility();
                gamechar.Abilities.Add(newpower);

            }
            BCBlockGameState.Soundman.PlaySound("grow");
            Block.AddScore(gamestate, 1000, Location);
            return true;
        }

        public override bool Powerup_PerformFrame(BCBlockGameState gamestate)
        {
            Random rg = BCBlockGameState.rgen;
            Color usecolor = new HSLColor(BCBlockGameState.rgen.NextDouble() * 240, 240, 120);
            //select a random position.
            RectangleF baserect = GetRectangleF();
            PointF randompos = new PointF(baserect.Left + (float)(baserect.Width * rg.NextDouble()), baserect.Top + (float)(baserect.Height * rg.NextDouble()));

            PointF RandomVelocity = new PointF((float)rg.NextDouble() * 0.5f - 0.25f, (float)rg.NextDouble() * 0.5f - 0.25f);

            Sparkle sp = new Sparkle(randompos, RandomVelocity, usecolor);
            sp.MaxRadius = (float)(1 + rg.NextDouble() * 4);
            gamestate.Particles.Add(sp);


            return false;
        }
        public override void TouchTop(BCBlockGameState gamestate, List<Block> touched, Block mainblock)
        {

            base.TouchTop(gamestate, touched, mainblock);
        }
        protected override void standingon(BCBlockGameState gamestate, List<Block> standingon, Block Mainblock)
        {
            onground = false;
            ongroundlastframe = false;
            Location = new PointF(Location.X, Location.Y - 4);
            Velocity = new PointF(Velocity.X, -4);
        }
        
        


    }

    /// <summary>
    /// abstract class, gives GameCharacter an ability when touched.
    /// </summary>
    /*
    public abstract class CharacterAbilityPowerup : GameCharacterPowerup
    {
        private Type _PowerupAbilityType = null;
        protected CharacterAbilityPowerup(Block EmergeFrom, Dictionary<String, String[]> pStateFrameData, int pFrameDelay,Type pPowerupAbilityType)
            : base(EmergeFrom, pStateFrameData, pFrameDelay)
        {
            if (!pPowerupAbilityType.IsSubclassOf(typeof(GameCharacterAbility)))
                throw new ArgumentException("Argument must be a subclass of BASeBlock.GameCharacterAbility", "PowerupAbilityType");

            _PowerupAbilityType = pPowerupAbilityType;
            

        }
        protected CharacterAbilityPowerup(Block EmergeFrom, String[] StateFrameData, int pFrameDelay, Type pPowerupAbility)
            : this(EmergeFrom, new Dictionary<string, string[]>() { { "IDLE", StateFrameData } }, pFrameDelay, pPowerupAbility)
        {


        }
        protected CharacterAbilityPowerup(Block EmergeFrom, String StateFrameData, int pFrameDelay, Type pPowerupAbility)
            : this(EmergeFrom, new string[] { StateFrameData }, pFrameDelay, pPowerupAbility)
        {


        }


    }
     
    */

    public class ExtraLifeItem : GameCharacterPowerup
    {
        //Extra life item collectable by gamecharacter.
        private static ImageAttributes extralifedraw()
        {

            ImageAttributes dAttrib = new ImageAttributes();
            dAttrib.SetColorMatrix(ColorMatrices.GetColourizer(Color.Lime));
            return dAttrib;

        }
        public ExtraLifeItem(Block EmergeFrom)
            : base(EmergeFrom, "Mushroom", 2,extralifedraw())
        {
  

        }
        public ExtraLifeItem(PointF pPosition, SizeF psize)
            :base(pPosition,new PointF(2,0),new string[] { "Mushroom"},2,extralifedraw())

        {
            useSize = psize;

        }
        public override bool GivePowerTo(BCBlockGameState gamestate, GameCharacter gamechar)
        {
            BCBlockGameState.Soundman.PlaySound("1up");
            BasicFadingText bft = new BasicFadingText("1-Up", Location, new PointF(0, -2), new Font(BCBlockGameState.GetMonospaceFont(), 14), new Pen(Color.Black,2), new SolidBrush(Color.Green), 500);
            
            gamestate.playerLives++;
            gamestate.Forcerefresh = true;
            gamestate.Defer(() => gamestate.GameObjects.AddLast(bft));
            
            return true;
        }

        public override bool Powerup_PerformFrame(BCBlockGameState gamestate)
        {
            return false;
        }
    }



    public class MushroomPower : GameCharacterPowerup
    {
        public MushroomPower(Block EmergeFrom)
            : base(EmergeFrom, "Mushroom", 2)
        {

        }
        public MushroomPower(PointF pPosition, SizeF psize)
            : base(pPosition, new PointF(2, 0), new string[] { "Mushroom" }, 0)
        {
            useSize = psize;


        }
        public override void Bump(Block bumpedby)
        {
            base.Bump(bumpedby);
            onground = false;
            ongroundlastframe = false;
            Velocity = new PointF(Velocity.X, -5);
        }

        public override bool GivePowerTo(BCBlockGameState gamestate, GameCharacter gamechar)
        {
            BCBlockGameState.Soundman.PlaySound("grow");
            Block.AddScore(gamestate, 1000, Location);
            return true;
        }

        public override bool Powerup_PerformFrame(BCBlockGameState gamestate)
        {
            //nuttin
            return false;
        }
    }
    /// <summary>
    /// used to indicate a GameCharacterPowerup that does not "emerge".
    /// </summary>
    public class NoEmergePowerupAttribute : Attribute
    {



    }

    /// <summary>
    /// abstract base class for powerups and items acquirable by the GameCharacter.
    /// </summary>
    public abstract class GameCharacterPowerup : PlatformObject
    {
        protected Block EmergingFrom = null; //block this powerup is emerging from. 
        protected bool Emerging = false;

        //moar constructor overloads...
        protected GameCharacterPowerup(Block EmergeFrom, String ImageFrame, int pFrameDelay)
            : this(EmergeFrom, ImageFrame, pFrameDelay, new ImageAttributes())
        {

        }
        protected GameCharacterPowerup(Block EmergeFrom, String ImageFrame, int pFrameDelay, ImageAttributes puseattributes)
            : this(EmergeFrom, new string[] { ImageFrame }, pFrameDelay, puseattributes)
        {

        }

        protected GameCharacterPowerup(Block EmergeFrom, String[] ImageFrames, int pFrameDelay) :
            this(EmergeFrom, ImageFrames, pFrameDelay, new ImageAttributes())
        {

        }
        /// <summary>
        /// Creates a powerup emerging from a block. set of imageframes will be set to the "idle" state
        /// </summary>
        /// <param name="EmergeFrom"></param>
        /// <param name="ImageFrames"></param>
        /// <param name="pFrameDelay"></param>
        /// <param name="puseattributes"></param>
        protected GameCharacterPowerup(Block EmergeFrom, String[] ImageFrames, int pFrameDelay, ImageAttributes puseattributes)
            : this(EmergeFrom, new Dictionary<string, string[]>() { { "IDLE", ImageFrames } }, pFrameDelay, puseattributes)
        {

        }


        protected GameCharacterPowerup(Block EmergeFrom, Dictionary<String, String[]> pStateFrameData, int pFrameDelay)
            : this(EmergeFrom, pStateFrameData, pFrameDelay, new ImageAttributes())
        {

        }
        protected GameCharacterPowerup(Block EmergeFrom, Dictionary<String, String[]> pStateFrameData, int pFrameDelay, ImageAttributes puseattributes)
            : base(EmergeFrom.CenterPoint(), new PointF(0, 0), pStateFrameData, pFrameDelay, puseattributes)
        {
            //set starting position to emerge from the center of the block.
            EnemyAction = "IDLE";
            //Image gotimage = base.GetCurrentImage();
            SizeF grabsize = DrawSize;

            int Xcoord = (int)(EmergeFrom.CenterPoint().X - (grabsize.Width / 2));
            int Ycoord = (int)(EmergeFrom.BlockRectangle.Top);

            Location = new PointF(Xcoord, Ycoord);
            Emerging = true;
            EmergingFrom = EmergeFrom;
            


        }
        public virtual String GetEmergeSound()
        {
            return "EMERGE";

        }
        protected GameCharacterPowerup(PointF pPosition, PointF pVelocity, Dictionary<String, String[]> pStateFrameData, int pFrameDelay, ImageAttributes puseattributes)
            : base(pPosition, pVelocity, pStateFrameData, pFrameDelay, puseattributes)
        {


        }
        protected GameCharacterPowerup(PointF pPosition, PointF pVelocity, Dictionary<String, String[]> pStateFrameData, int pFrameDelay) :
            base(pPosition, pVelocity, pStateFrameData, pFrameDelay, new ImageAttributes())
        {


        }
        private static Dictionary<String, String[]> Powerupstateframedata(String[] powerframes)
        {
            return new Dictionary<string, string[]> { { "IDLE", powerframes } };
        }
        protected GameCharacterPowerup(PointF pPosition, PointF pVelocity, String powerupimagekey, int pFrameDelay, ImageAttributes puseattributes)
            : this(pPosition, pVelocity, Powerupstateframedata(new string[] { powerupimagekey }), pFrameDelay, puseattributes)
        {


        }
        protected GameCharacterPowerup(PointF pPosition, PointF pVelocity, String[] PowerupFrames, int pFrameDelay)
            : this(pPosition, pVelocity, PowerupFrames, pFrameDelay, new ImageAttributes())
        {


        }
        protected GameCharacterPowerup(PointF pPosition, PointF pVelocity, String[] PowerupFrames, int pFrameDelay, ImageAttributes puseattributes)
            : this(pPosition, pVelocity, Powerupstateframedata(PowerupFrames), pFrameDelay, puseattributes)
        {
            EnemyAction = "IDLE";

        }
        public abstract bool GivePowerTo(BCBlockGameState gamestate, GameCharacter gamechar);



        public abstract bool Powerup_PerformFrame(BCBlockGameState gamestate);
        public override void TouchTop(BCBlockGameState gamestate, List<Block> touched, Block mainblock)
        {
            Velocity = new PointF(Velocity.X, Math.Abs(Velocity.Y));
            //base.TouchTop(gamestate, ref AddObjects, ref removeobjects, touched, mainblock);
        }


        public sealed override void Draw(Graphics g)
        {
            //set clip so we only appear above the block.
            //this was changed from previously where we set the clip to the bounds of the block itself,
            //meaning that smaller blocks would show the powerup through the sides.
            var copiedclip = g.Clip;
            if (EmergingFrom != null && Emerging)
            {
                g.SetClip(EmergingFrom.BlockRectangle, CombineMode.Exclude);
                //RectangleF setrect = new RectangleF(float.MinValue, EmergingFrom.BlockRectangle.Top, float.MaxValue, float.MaxValue);
                //g.SetClip(setrect, CombineMode.Exclude);
            }

            base.Draw(g);
            //if (Emerging) EmergingFrom.Draw(g); //make sure the block draws "on top" of us.
            //reset the clip
            if (EmergingFrom != null && Emerging) g.SetClip(copiedclip, CombineMode.Replace);

        }
        public virtual void DrawPowerup(Graphics g)
        {

        }
        public override sealed bool PerformFrame(BCBlockGameState gamestate)
        {

            Rectangle ourrect = this.GetRectangle();

            if (!Emerging)
            {
                foreach (GameCharacter gamechar in
                    (from m in gamestate.GameObjects
                     where
                         m is GameCharacter &&
                         !(new String[] { "DYING", "DEATH" }.Contains(((GameCharacter)m).EnemyAction.ToUpper()))
                     select m))
                {
                    //only give power to living GameCharacters...
                    Rectangle gamecharrect = gamechar.GetRectangle();
                    if (ourrect.IntersectsWith(gamecharrect))
                    {
                        //intersection...
                        return gamechar.GetPowerup(gamestate, this);






                    }


                }
                //if we are emerging, handle that stuff ourselves and return false.
                //otherwise, return the result of PowerUp_PerformFrame || base.PerformFrame.
            }
            else
            {

                //move us up a bit
                Location = new PointF(Location.X, Location.Y - 2);
                //if we are clear of the block, disable the Emerging flag
                //the Location check is to ensure that we don't pop out the bottom of the block is smaller than the powerup.
                if (Location.Y < EmergingFrom.BlockRectangle.Top && !(EmergingFrom.BlockRectangle.IntersectsWith(GetRectangleF())))
                {
                    Emerging = false;
                    //call virtual method
                    EmergeComplete(EmergingFrom, gamestate);
                }
                return false; //DON'T perform default processing, otherwise it will move the powerup via gravity and stuff and we may never emerge at all.
            }
            //only call default if it returns true...
            if(!Powerup_PerformFrame(gamestate))
                return base.PerformFrame(gamestate);
            else
            {
                return true;
            }

        }
        
        public override CollideTypeConstants Collide(BCBlockGameState gstate, PlatformObject otherobject)
        {
            return CollideTypeConstants.Collide_Nothing; //nuffin happens.
        }
        public virtual void EmergeComplete(Block EmergedFrom, BCBlockGameState gstate)
        {
            //set standard speed.
            Velocity = new PointF(3, 0);

        }

    }

    public abstract class GameCharacterAbility
    {
        //abstract base class for the abilities givable to the character.
        //typically a powerup gives the game character an ability.

        bool flInit = false;

        /// <summary>
        /// return true to remove this ability from the character.
        /// </summary>
        /// <param name="gamechar"></param>
        /// <param name="gstatem"></param>
        /// <returns></returns>
        public bool PerformFrame(GameCharacter gamechar, BCBlockGameState gstatem)
        {

            if (!flInit)
            {
                flInit = true;
                AbilityInit(gamechar, gstatem);
                
            }
            return AbilityFrame(gamechar, gstatem);


        }

        

        public abstract void AbilityInit(GameCharacter gamechar, BCBlockGameState gstatem);

        public virtual void BeforeDraw(GameCharacter gamechar, Graphics g)
        {
            //nothing.... well, by default nothing.

        }

        /// <summary>
        /// Called during the gamecharacter frame for each ability.
        /// </summary>
        /// <param name="gamechar"></param>
        /// <param name="gstatem"></param>
        /// <returns>true to remove this ability. False otherwise.</returns>
        public abstract bool AbilityFrame(GameCharacter gamechar, BCBlockGameState gstatem);

        /// <summary>
        /// called when gamecharacter collides with a platformobject.
        /// </summary>
        /// <param name="gstate"></param>
        /// <param name="gamechar"></param>
        /// <param name="otherobject"></param>
        /// <returns>false to continue default processing. true to return immediately without default processing.</returns>
        public abstract bool Collide(BCBlockGameState gstate, GameCharacter gamechar, PlatformObject otherobject);





    }

    /// <summary>
    /// subclass of GameCharacterAbility adding timing constraints.
    /// </summary>
    public abstract class TimedCharacterAbility : GameCharacterAbility
    {
        private static readonly TimeSpan defaultduration = new TimeSpan(0, 0, 0, 10);
        private DateTime _AbilityStart;
        private TimeSpan _Duration = defaultduration; //10 second default.
        private String _AbilityMusic = ""; //music to use; null or empty to not affect music.

        public DateTime AbilityStart { get { return _AbilityStart; } set { 
            _AbilityStart = value;
         
        
        } }

        public String AbilityMusic { get { return _AbilityMusic; } set { _AbilityMusic = value; } }
        protected TimedCharacterAbility()
            : this(defaultduration)
        {


        }
        protected TimedCharacterAbility(TimeSpan Duration)
            : this(Duration, "")
        {


        }
        protected TimedCharacterAbility(TimeSpan Duration,String pAbilityMusic)
        {
            _Duration = Duration;
            _AbilityMusic = pAbilityMusic;
        }
        protected TimedCharacterAbility(String pAbilityMusic):this(defaultduration,pAbilityMusic)
        {

            

        }
        public String DelayIdentifier = null;
        public override void AbilityInit(GameCharacter gamechar, BCBlockGameState gstatem)
        {
            gamechar.OnDeath += gamechar_OnDeath;
            DelayIdentifier = gstatem.DelayInvoke(_Duration, revertroutinefunc, new object[] { gamechar, gstatem });
            AbilityStart = DateTime.Now;
            if (!String.IsNullOrEmpty(_AbilityMusic))
                //BCBlockGameState.Soundman.PushMusic(_AbilityMusic, 1.0f, true);
                BCBlockGameState.Soundman.PlayTemporaryMusic(_AbilityMusic, 1.0f, true);
        }
        public bool ChangeDelayTime(BCBlockGameState gstate,TimeSpan newdelay,bool resetstart)
        {

            return gstate.ChangeDelayData(DelayIdentifier, newdelay, null,resetstart);


        }
        public void revertroutinefunc(object[] parameters)
        {

            BCBlockGameState grabstate = (BCBlockGameState)parameters[1];
            GameCharacter gamechar = (GameCharacter)parameters[0];
            List<GameObject> objectsadd = new List<GameObject>();
            List<GameObject> objectsremove = new List<GameObject>();
            List<GameCharacterAbility> addabilities = new List<GameCharacterAbility>();

            AbilityEnd(gamechar, grabstate, ref objectsadd, ref objectsremove, ref addabilities);

            foreach (var iterate in objectsadd)
                grabstate.GameObjects.AddLast(iterate);

            foreach (var iterate in objectsremove)
                grabstate.GameObjects.Remove(iterate);

            foreach (var iterate in addabilities)
                gamechar.Abilities.Add(iterate);


        }
        void gamechar_OnDeath(Object sender,EnemyDeathEventArgs e)
        {
            //reset the music.
            if (!String.IsNullOrEmpty(_AbilityMusic))
                //BCBlockGameState.Soundman.PopMusic(_AbilityMusic);
                BCBlockGameState.Soundman.StopTemporaryMusic(_AbilityMusic);
        }
        bool AbilityEnded = false;
        public virtual void AbilityEnd(GameCharacter gamechar, BCBlockGameState gstate, ref List<GameObject> addobjects, ref List<GameObject> removeobjects, ref List<GameCharacterAbility> addabilities)
        {
            //called when ability is "over"
           // BCBlockGameState.Soundman.PopMusic(_AbilityMusic);
            AbilityEnded = true;
            BCBlockGameState.Soundman.StopTemporaryMusic(_AbilityMusic);
        }
        public override bool AbilityFrame(GameCharacter gamechar, BCBlockGameState gstatem)
        {
            /*
            if ((DateTime.Now - AbilityStart) > _Duration)
            {
                AbilityEnd(gamechar, gstatem, ref addobjects, ref removeobjects, ref addabilities);
                return true;
            }
            */
            return AbilityEnded;
        }

    }
    public class InvinciblePowerupAbility : TimedCharacterAbility
    {
        private static ImageAttributes IAfromCM(ColorMatrix frommatrix)
        {
            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(frommatrix);
            return ia;



        }
        private static ImageAttributes[] CreateDrawAttributes()
        {

            return new ImageAttributes[] {
                IAfromCM(ColorMatrices.GetColourizer(Color.Red)),
                IAfromCM(ColorMatrices.GetColourizer(Color.Green)),
                IAfromCM(ColorMatrices.GetColourizer(Color.Blue)),
                IAfromCM(ColorMatrices.GetColourizer(Color.Violet)),
                IAfromCM(ColorMatrices.GetColourizer(Color.Orange)),
                IAfromCM(ColorMatrices.GetColourizer(Color.Purple)),
                IAfromCM(ColorMatrices.GetColourizer(Color.Brown)),
                IAfromCM(ColorMatrices.GetColourizer(Color.Yellow))
            };



            }



        
        private static ImageAttributes[] useDrawAttributes = CreateDrawAttributes();


        public InvinciblePowerupAbility(TimeSpan Duration, String Music):base(Duration,Music)
        {


        }
        public InvinciblePowerupAbility()
            : base("INVINCIBLE")
        {


        }

        /// <summary>
        /// called when gamecharacter collides with a platformobject.
        /// </summary>
        /// <param name="gstate"></param>
        /// <param name="gamechar"></param>
        /// <param name="otherobject"></param>
        /// <returns>false to continue default processing. true to return immediately without default processing.</returns>
        public override bool Collide(BCBlockGameState gstate, GameCharacter gamechar, PlatformObject otherobject)
        {
            //kill the other object, and leave us unaffected.
            //but- only for types deriving from PlatformEnemy

            if(otherobject is PlatformEnemy)
            {
                otherobject.Kill();
                
            return true;
          }
                return false; //default processing...
           
        }
        ImageAttributes cached = null;
        public override void AbilityInit(GameCharacter gamechar, BCBlockGameState gstatem)
        {
            base.AbilityInit(gamechar, gstatem);
            //cached = gamechar.DrawAttributes;
        }
        public override void AbilityEnd(GameCharacter gamechar, BCBlockGameState gstate, ref List<GameObject> addobjects, ref List<GameObject> removeobjects, ref List<GameCharacterAbility> addabilities)
        {
            base.AbilityEnd(gamechar, gstate, ref addobjects, ref removeobjects, ref addabilities);
            //gamechar.DrawAttributes = cached; 
        }


        public override void BeforeDraw(GameCharacter gamechar, Graphics g)
        {

            gamechar.DrawAttributes = usetheseattributes;

            base.BeforeDraw(gamechar, g);
        }
        ImageAttributes usetheseattributes = null;
        int delayfactor = 2;
        int delayvalue = 0;
        public override bool AbilityFrame(GameCharacter gamechar, BCBlockGameState gstatem)
        {
            delayvalue++;
            if (delayvalue == delayfactor)
            {
                usetheseattributes = BCBlockGameState.Choose(useDrawAttributes);
                delayvalue = 0;
            }
            //also spawn a sparkly. One sparkley per frame.
            //choose a random colour...
            Random rg = BCBlockGameState.rgen;
            Color usecolor = new HSLColor(BCBlockGameState.rgen.NextDouble()*240, 240, 120);
            //select a random position.
            RectangleF baserect = gamechar.GetRectangleF();
            PointF randompos = new PointF(baserect.Left + (float)(baserect.Width * rg.NextDouble()), baserect.Top + (float)(baserect.Height * rg.NextDouble()));

            PointF RandomVelocity = new PointF((float)rg.NextDouble()*0.5f-0.25f,(float)rg.NextDouble()*0.5f-0.25f);

            Sparkle sp = new Sparkle(randompos, RandomVelocity, usecolor);
            sp.MaxRadius = 5;
            gstatem.Particles.Add(sp);

            return base.AbilityFrame(gamechar, gstatem);
        }
    }

}
