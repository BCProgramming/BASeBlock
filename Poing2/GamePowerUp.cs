using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using BASeBlock.Particles;

namespace BASeBlock
{
    /// <summary>
    /// used to assign certain default behaviours to a GamePowerup, specifically 
    /// some visual distinctions between "good" and "bad" powerups.
    /// </summary>
    public abstract class PowerupAttribute : Attribute
    {
        public abstract void PerformFrame(GamePowerUp powerup, BCBlockGameState gamestate);
        public abstract void Draw(GamePowerUp powerup,Graphics g);
        

    }


    public class ColouredGamePowerupAttribute : PowerupAttribute
    {
        private Color _Color;
        private float _Radius;
        private Image drawimage = null;
        public ColouredGamePowerupAttribute(Color usecolor, float Radius)
        {
            _Color = usecolor;
            _Radius = Radius;
            drawimage = LightOrb.DrawLightOrb(new Size((int)(Radius * 2), (int)(Radius * 2)), usecolor);

        }
        public override void Draw(GamePowerUp powerup,Graphics g)
        {
            g.DrawImage(drawimage, powerup.CenterPoint().X - _Radius, powerup.CenterPoint().Y - _Radius);
        }
        public override void PerformFrame(GamePowerUp powerup, BCBlockGameState gamestate)
        {
            //nothing needed.
            
        }

    }

    public class PositivePowerupAttribute : ColouredGamePowerupAttribute
    {

        public PositivePowerupAttribute()
            : base(Color.Green, 32)
        {
        }


    }


    public class PowerupCollectionEditor : BaseMultiEditor
    {
        public PowerupCollectionEditor(Type ptype)
            : base(ptype)
        {


        }
        protected override Type[] CreateNewItemTypes()
        {
            return GamePowerUp.GetPowerUpTypes();
        }



    }



    public class GamePowerUp : AnimatedSprite
    {



        //public delegate
        //public delegate bool PerformFrameFunction(BCBlockGameState bb, ref List<GameObject> AddObjects,ref List<GameObject> removeobjects);
        //public PointF Velocity= new PointF(0,2);
        public static readonly SizeF defaultSize = new SizeF(24, 12);
        public delegate bool CollectPowerupfunction(BCBlockGameState bb);




        protected CollectPowerupfunction usefunction;





        //tweaked for extensibility...

        public static Type[] GetPowerUpTypes()
        {
            //return BCBlockGameState.MTypeManager[typeof(GamePowerUp)].ManagedTypes.ToArray();
            return BCBlockGameState.MTypeManager[typeof(GamePowerUp)].ManagedTypes.ToArray();
            // return new Type[] {typeof(NullPowerUp),typeof(FrustratorPowerup), typeof (PaddlePlusPowerup), typeof (PaddleMinusPowerup),typeof(AddBallPowerup),typeof(StickyPaddlePowerUp),typeof(TerminatorPowerUp),typeof(LifeUpPowerUp),typeof(EnemySpawnerPowerUp),
            //     typeof(MagnetPowerup),typeof(IcePowerUp),typeof(BallSplitterPowerup),typeof(ExtraLifePowerup),typeof(ShieldPowerup)
            // };
        }
        public static float[] GetPowerUpChance()
        {
            Type[] inspecttypes = GetPowerUpTypes();
            float[] returnfloat = new float[inspecttypes.Length];
            for (int i = 0; i < inspecttypes.Length; i++)
            {
                float usechance = 1f;
                //check for static "PowerupChance()" routine.
                MethodInfo getchanceproc = inspecttypes[i].GetMethod("PowerupChance", BindingFlags.Static);
                if (getchanceproc != null)
                {
                    try
                    {
                        usechance = (float)getchanceproc.Invoke(null, new object[0]);
                    }
                    catch
                    {
                        usechance = 1f;
                    }


                }
                returnfloat[i] = usechance;

            }
            return returnfloat;
            /*  return new float[] 
            { 10f, //null
                2f, //Frustrator
                2f,  //PaddlePlus
                3f,  //PaddleMinus
                5f, //AddBallPowerup
                2f, //StickyPaddlePowerUp
                1f, //TerminatorPowerUp 
                1f, //LifeUpPowerUp
                2f, //EnemySpawnerPowerup
                3f, //MagnetPowerup
                0f, //IcePowerup
                2f, //BallSplitterPowerup
                0.4f, //Extra life 
                1f //shield
            };*/
        }
        protected void AddScore(BCBlockGameState ParentGame, int scoreadd)
        {
            AddScore(ParentGame, scoreadd, "");


        }
        public override void Draw(Graphics g)
        {
            Object[] result = GetType().GetCustomAttributes(typeof(PowerupAttribute), true);

            foreach (Object iterate in result)
            {
                if (iterate is PowerupAttribute)
                {
                    ((PowerupAttribute)iterate).Draw(this, g);


                }


            }
            base.Draw(g);




        }
        protected void AddScore(BCBlockGameState ParentGame, int scoreadd, String prefixString)
        {
            //PointF MidPoint = new PointF(mBlockrect.Left + (mBlockrect.Width / 2), mBlockrect.Top + (mBlockrect.Height / 2));
            String usestring;



            PointF MidPoint = new PointF(Location.X + Size.Width / 2, Location.Y + Size.Height / 2);
            int addedscore = scoreadd + (Math.Sign(scoreadd) * ParentGame.GameObjects.Count * 10);
            if (String.IsNullOrEmpty(prefixString))
                usestring = addedscore.ToString();
            else
                usestring = prefixString + "(" + addedscore.ToString() + ")";

            ParentGame.GameScore += addedscore;
            //ParentGame.GameObjects.AddLast(new BasicFadingText(usestring, MidPoint, new PointF(((float)BCBlockGameState.rgen.NextDouble() * 0.2f) * -0.1f, ((float)BCBlockGameState.rgen.NextDouble()) * -0.7f), new Font(BCBlockGameState.GetMonospaceFont(), 16), null, null));
            ParentGame.Defer(() => ParentGame.GameScore += addedscore);
            ParentGame.GameObjects.AddLast(new BasicFadingText(usestring, MidPoint, new PointF(((float)BCBlockGameState.rgen.NextDouble() * 0.2f) * -0.1f, ((float)BCBlockGameState.rgen.NextDouble()) * -0.7f), new Font(BCBlockGameState.GetMonospaceFont(), 16), null, null));
            //ParentGame.EnqueueMessage(usestring);
        }

        public GamePowerUp(PointF Location, SizeF ObjectSize, Image ImageUse, CollectPowerupfunction powerupfunc)
            : base(Location, ObjectSize, new Image[] { ImageUse }, 0, 0, 3)
        {

            NextAttributesFunc = null;
            usefunction = powerupfunc;

        }
        public GamePowerUp(PointF Location, SizeF ObjectSize, Image[] Imagesuse, int framedelay, CollectPowerupfunction powerupfunc)
            : base(Location, ObjectSize, Imagesuse, 0, 0, framedelay)
        {
            //VelocityChange = new VelocityChangerLeafy();
            NextAttributesFunc = null;
            usefunction = powerupfunc;



        }
        public GamePowerUp(PointF Location, SizeF ObjectSize, String[] ImageFrameskeys, int framedelay, CollectPowerupfunction powerupfunc)
            : base(Location, ObjectSize, ImageFrameskeys, 0, 0, framedelay)
        {

            usefunction = powerupfunc;
        }

        private bool TouchesPaddle(Paddle paddlecheck)
        {
            if (paddlecheck == null) return false;
            return new RectangleF(Location, Size).IntersectsWith(paddlecheck.Getrect());


        }


        public override bool PerformFrame(BCBlockGameState gamestate)
        {

            //return base.PerformFrame(gamestate, ref AddObjects, ref removeobjects);
            base.PerformFrame(gamestate);


            if (TouchesPaddle(gamestate.PlayerPaddle))
            {
                //call into each behaviour. If any return true, we want to reject.
                if (!gamestate.PlayerPaddle.Behaviours.Any((b) => b.getPowerup(gamestate, gamestate.PlayerPaddle, this)))
                {






                    bool retval = usefunction(gamestate);
                    // removeobjects.Add(this);
                    return retval;
                }
                else
                {
                    return true;
                }
            }
                if (Location.Y > gamestate.GameArea.Height)
                    return true;

                else if (Location.Y < gamestate.GameArea.Top)
                {
                    Location = new PointF(Location.X, 1);
                    Velocity = new PointF(Velocity.X, Math.Abs(Velocity.Y));
                }

                if (Location.X > gamestate.GameArea.Width)
                {
                    //bounce off right wall.
                    Location = new PointF(getRectangle().Right - getRectangle().Width + 1, Location.Y);
                    Velocity = new PointF(Math.Abs(Velocity.X), Velocity.Y);



                }
                else if (Location.X < gamestate.GameArea.Left)
                {

                    //bounce off left wall.
                }
                Object[] result = GetType().GetCustomAttributes(typeof(PowerupAttribute), true);

                foreach (Object iterate in result)
                {
                    if (iterate is PowerupAttribute)
                    {
                        ((PowerupAttribute)iterate).PerformFrame(this, gamestate);


                    }


                }

            

            return false;
            //  return usefunction(gamestate,ref  AddObjects, ref removeobjects);
        }


    }
}