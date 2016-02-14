using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using BASeCamp.BASeBlock.Blocks;

namespace BASeCamp.BASeBlock.Projectiles
{
    public class LaserShot   : Projectile
    {

        protected long TimeAlive = 0;
        protected PointF _ShotPoint; //location it was shot from.
        public PointF ShotPoint { get { return _ShotPoint; } set { _ShotPoint = value; } }
        protected int numpoints = 0;
        protected LinkedList<PointF> LaserPoints = new LinkedList<PointF>();
        protected bool _DamagesPaddle = false;
        protected Pen _LaserPen = null;
        //if maxbounces is 0,(default) behaviour is to go straight through all "non weak" blocks.
        //if maxbounces is non-zero, it will go through weak blocks, but will bounce off of other blocks.

        private int _MaxBounces = 3;
        private int _NumBounces = 0;
        public int MaxBounces { get { return _MaxBounces; } set { _MaxBounces = value; } }
        public delegate void LaserShotFrameFunction(BCBlockGameState gstate, LaserShot shotobject, LinkedList<PointF> LaserPoints);
        public event LaserShotFrameFunction LaserShotFrame;
        protected static Pen DefaultPen = new Pen(Color.Blue, 3);

        public Pen LaserPen
        {
            get
            {
                if (_LaserPen == null)
                {
                    _LaserPen = new Pen(Color.Blue, 5);

                }

                return _LaserPen;
            }
            set { _LaserPen = value; }
        }



        public static int DefaultLength = 72;
        protected float _MaxLength = DefaultLength;
        public bool _Weak = false;

        public bool Weak
        {
            get { return _Weak; }
            set { _Weak = value; }
        }

        public float MaxLength { get { return _MaxLength; } set { _MaxLength = value; } }

        public bool DamagesPaddle { get { return _DamagesPaddle; } set { _DamagesPaddle = value; } }



        protected override PointF getLocation()
        {
            return LaserPoints.First();
        }
        protected override void setLocation(PointF newLocation)
        {
            LaserPoints.RemoveFirst();
            LaserPoints.AddFirst(newLocation);
        }

        public LaserShot(PointF Origin)
            : this(Origin, DefaultPen)
        {


        }
        public LaserShot(PointF Origin, int MaxLength)
            : this(Origin, DefaultPen, MaxLength)
        {
        }

        public LaserShot(PointF Origin, Pen LaserPen)
            : this(Origin, LaserPen, DefaultLength)
        {
        }

        public LaserShot(PointF Origin, Pen LaserPen, int MaxLength)
            : base(Origin, new PointF(0, -5))
        {
            _MaxLength = MaxLength;
            _LaserPen = LaserPen;
            ShotPoint = Origin;
            numpoints = 1;
            LaserPoints = new LinkedList<PointF>();
            LaserPoints.AddFirst(Origin);
            //LaserPoints[0] = LaserPoints[1] = Origin;

        }
        public LaserShot(PointF Origin, PointF pVelocity, int MaxLength)
            : this(Origin, pVelocity)
        {
            _MaxLength = MaxLength;
        }

        public LaserShot(PointF Origin, PointF pVelocity)
            : this(Origin, pVelocity, DefaultPen)
        {
        }


        public LaserShot(PointF Origin, PointF pVelocity, Pen LaserPen)
            : this(Origin, pVelocity, LaserPen, DefaultLength)
        {
        }

        public LaserShot(PointF Origin, PointF pVelocity, Pen LaserPen, int pMaxLength)
            : this(Origin, pMaxLength)
        {

            //set our velocity...

            _LaserPen = LaserPen;
            Velocity = pVelocity;
        }
        public LaserShot(PointF Origin, double pAngle, double pSpeed, Pen LaserPen)
            : this(Origin, pAngle, pSpeed, pSpeed, LaserPen)
        {



        }
        public LaserShot(PointF Origin, double Angle, double Speed)
            : this(Origin, Angle, Speed, DefaultPen)
        {

        }

        public LaserShot(PointF Origin, double Angle, double minSpeed, double maxSpeed)
            : this(Origin, Angle, minSpeed, maxSpeed, DefaultPen)
        {
        }



        public LaserShot(PointF Origin, double Angle, double minSpeed, double maxSpeed, Pen LaserPen)
            : this(Origin, BCBlockGameState.GetRandomVelocity((float)minSpeed, (float)maxSpeed, Angle), LaserPen)
        {



        }

        private List<Block> intersecting = null;
        private bool ProxyPerform(BCBlockGameState gamestate)
        {
            foreach (var removeit in intersecting)
            {

                cBall tempball = new cBall(removeit.CenterPoint(), Velocity);
                tempball.Behaviours.Add(new TempBallBehaviour());
                //add a proxy behaviour to remove it as well.
                //tempball.Behaviours.Add(new ProxyBallBehaviour("ExplosionEffect", null, proxyperformframe, null, null, null, null, null, null));
                //gamestate.Balls.AddLast(tempball);

                List<cBall> discardlist = new List<cBall>();
                try
                {
                    //this is... well, cheating...

                    //we cannot add GameObjects to the List, except by plopping them in the ref AddedObject parameter.
                    //however, we cannot "force" the PerformBlockHit of a block (say a destruction block) to add a GameObject (another ExplosionEffect, in that case)

                    //we cheat. we swap out the entire gamestate.GameObject LinkedList with a new one, call the routine, and then
                    //we add any added GameObjects to our ref parameter, and swap the old list back in, hoping nobody notices our
                    //audacity to fiddle with core game state objects...



                    var copiedref = gamestate.GameObjects;
                    gamestate.GameObjects = new LinkedList<GameObject>();

                    removeit.PerformBlockHit(gamestate, tempball);
                    if (removeit.MustDestroy()) removeit.StandardSpray(gamestate);
                    var tempadded = gamestate.GameObjects;
                    gamestate.GameObjects = copiedref;
                    //now we add the ones we need to add to our ref array. I feel dirty.
                    gamestate.Defer(() =>
                                        {
                                            foreach (var doiterate in tempadded)
                                            {
                                                gamestate.GameObjects.AddLast(doiterate);
                                            }


                                        });
                        


                }
                catch
                {

                }
                //we don't add the ball to our GameState, so we don't have to worry about it persisting :D

                //the idea is that we want to invoke the actions of the block (which for many blocks will simply be destroyed).
                //at the same time, some blocks might react weirdly to having temporary balls tossed into their centers, so we make it so the ball will only live for 
                //two frames by adding a proxyballbehaviour that ensure that.
                try
                {
                    if (removeit.MustDestroy() && removeit.Destructable)
                    {
                        Debug.Print("Removing Block of type:" + removeit.GetType().Name);
                        gamestate.Blocks.Remove(removeit);

                    }



                }
                catch (Exception erroroccur)
                {
                    Debug.Print("Exception:" + erroroccur.Message + " occured while removing block via LaserShot.");
                    gamestate.Forcerefresh = true; //force a refresh...
                }
                finally
                {

                }


            }
            gamestate.Forcerefresh = true; //force a refresh...
            return true;

        }
        private void InvokeLaserFrame(BCBlockGameState gstate)
        {
            var copied = LaserShotFrame;
            if (copied != null)
                copied.Invoke(gstate, this, LaserPoints);



        }
        private bool BlockStopsLaser(Block testblock)
        {

            if (testblock is PolygonBlock)
            {
                PolygonBlock Pb = testblock as PolygonBlock;
                return _Weak || !Pb.Destructable || !testblock.MustDestroy();
            }
            else {
                return _Weak || !testblock.MustDestroy();
            }

        }
        private bool LaserIntersects(RectangleF checkrect)
        {

            //return (from p in LaserPoints let o = LaserPoints.Find(p) where o.Next != null select o)
            //    .Any((p) => BCBlockGameState.IntersectLine(p.Value, p.Next.Value, checkrect)!=null);

            return (from p in LaserPoints let o = LaserPoints.Find(p) where o.Next != null select o)
                .Any((p) => BCBlockGameState.LiangBarsky(checkrect, p.Value, p.Next.Value) != null);
        }
        private int MaxPoints = 10;
        Type[] DamagableEnemies = new Type[]{typeof(EyeGuy)};
        private bool stoppedadvance = false;
        public override bool PerformFrame(BCBlockGameState gamestate)
        {

            //only advances if it isn't currently "inside" a block whose MustDestroy returns false.
            InvokeLaserFrame(gamestate);
            //if there are no points, "destroy" us.
            PointF firstpoint = LaserPoints.First();
            var nextpoint = LaserPoints.Find(firstpoint).Next;
            //we use the second LaserPoint, rather than the first. This is because the "collision" will occur when the first point touches it, which will usually mean the second
            //point is not inside the block, so the normal value will be more reliable for reflecting the vector.
            PointF? secondpoint = nextpoint != null ? nextpoint.Value : (PointF?)null;
            PointF testpoint = secondpoint == null ? firstpoint : secondpoint.Value;

            //check other objects.
            var resultcheck = (from b in gamestate.GameObjects where !(b is LaserShot) select b);
            foreach (var iterate in resultcheck)
            {
                if (iterate is PolygonObstacle)
                {
                    PolygonObstacle po = iterate as PolygonObstacle;
                    if (po.Poly.Contains(firstpoint))
                    {
                        Polygon grabpoly = po.Poly;
                        LineSegment ls;
                        PointF closestPoint = BCBlockGameState.GetClosestPointOnPoly(grabpoly, testpoint, out ls);
                        PointF diffPoint = new PointF(closestPoint.X - nextpoint.Value.X, closestPoint.Y - nextpoint.Value.Y);
                        Velocity = Velocity.Mirror(diffPoint);
                    }
                }
                else if(iterate is GameEnemy)
                {
                    GameEnemy ge = (GameEnemy)iterate;
                    if(DamagableEnemies.Contains(ge.GetType()) && LaserIntersects(ge.GetRectangle()))
                    {
                        ge.HitPoints -= 1;
                    }
                }
            }

            if (numpoints == 0) return true;
            var hitresult = (from b in gamestate.Blocks where LaserIntersects(b.BlockRectangle) select b).ToList();
            if (hitresult.Count > 0)
            {
                Debug.Print("Break");

            }
            Block firstblocker = hitresult.FirstOrDefault((w) => w.BlockRectangle.Contains(firstpoint) && BlockStopsLaser(w));
            if (firstblocker !=null)
            {

                if (_MaxBounces == 0 || _MaxBounces <= _NumBounces)
                {
                    Debug.Print("Point 1 is not advancing");
                    stoppedadvance = true;
                }
                else
                {
                    
                    //otherwise, we bounce off firstblocker.
                    Polygon grabpoly = firstblocker.GetPoly();
                    LineSegment ls;
                    PointF closestPoint = BCBlockGameState.GetClosestPointOnPoly(grabpoly,testpoint,out ls);
                    PointF diffPoint = new PointF(closestPoint.X - nextpoint.Value.X, closestPoint.Y - nextpoint.Value.Y);
                    Velocity = Velocity.Mirror(diffPoint);
                    //PointF normal = new PointF(firstpoint.X - closestPoint.X, firstpoint.Y - closestPoint.Y);
                    //Velocity = BCBlockGameState.ReflectVector(Velocity,ls.Magnitude());
                        


                    /*
                              Polygon ballpoly = ballhit.GetBallPoly();
            Vector Adjustment = new Vector();
            GeometryHelper.PolygonCollisionResult pcr = GeometryHelper.PolygonCollision(GetPoly(), ballpoly, new Vector(ballhit.Velocity.X, ballhit.Velocity.Y));
            Adjustment = pcr.MinimumTranslationVector;
            //minimumtranslationvector will be the normal we want to mirror the ball speed through.
            ballhit.Velocity = ballhit.Velocity.Mirror(pcr.MinimumTranslationVector);
            ballhit.Velocity = new PointF(ballhit.Velocity.X, ballhit.Velocity.Y);
            ballhit.Location = new PointF(ballhit.Location.X - Adjustment.X, ballhit.Location.Y - Adjustment.Y);
            base.PerformBlockHit(parentstate, ballhit, ref ballsadded);
            BCBlockGameState.Soundman.PlaySound(DefaultHitSound);
            return Destructable;
                         * */



                    _NumBounces++;

                }
            }





            intersecting = hitresult;
            //AddObjects.Add(new ProxyObject(ProxyPerform, null));
            //question to past self: Why was I using a Proxy Object?...
               
            bool retval = ProxyPerform(gamestate);
            // if(retval) return retval;


            gamestate.Forcerefresh = gamestate.Forcerefresh || (intersecting.Any());
            //foreach(var destroyblock in intersecting)

            //defer destruction of those blocks.





            //check for paddle damage.
            var ppaddle = gamestate.PlayerPaddle;
            if (DamagesPaddle && ppaddle != null)
            {
                //make sure both points are low enough.
                //if (BCBlockGameState.LiangBarsky(ppaddle.Getrect(), LaserPoints[0], LaserPoints[1]) != null)
                if(LaserIntersects(ppaddle.Getrect()))
                {
                    //do some damage.
                    ppaddle.HP -= 2;
                }




            }

            else
            {
                //an additional check for all eyeguys on the playing field...
                foreach (EyeGuy loopguy in (from n in gamestate.GameObjects where n is EyeGuy select n))
                {
                    if(LaserIntersects(loopguy.GetRectangleF()))
                    {
                        loopguy.HitPoints -= 1;

                    }


                }
            }

            TimeAlive++;
            //final check: if we can bounce, we will bounce off all sides of the playfield but the bottom too.
            if (_MaxBounces > 0)
            {
                if (firstpoint.X < gamestate.GameArea.Left)
                {
                    Velocity = new PointF(-Velocity.X, Velocity.Y);

                }
                else if (firstpoint.X > gamestate.GameArea.Right)
                {
                    Velocity = new PointF(-Velocity.X, Velocity.Y);

                }

                if (firstpoint.Y < gamestate.GameArea.Top)
                {

                    Velocity = new PointF(Velocity.X, -Velocity.Y);


                }
            }
            //advance.
            //LaserPoints[1] = new PointF(LaserPoints[1].X + Velocity.X, LaserPoints[1].Y + Velocity.Y);
            //we advance by adding a new point from the current "head" point
            if (!stoppedadvance)
            {
                PointF newpoint = new PointF(LaserPoints.First().X + Velocity.X, LaserPoints.First().Y + Velocity.Y);
                LaserPoints.AddFirst(newpoint);
                numpoints++;
                //remove the last element too, if we have reached the max.
            }
            if (numpoints > MaxPoints || stoppedadvance)
            {
                LaserPoints.RemoveLast();
                numpoints--;

            }


            //this return will not have a problem since it would only occur when the entire shot leaves the playfield.
            return !LaserPoints.Any((w) => gamestate.GameArea.Contains(new Point((int)w.X, (int)w.Y)));
                



        }
        public override void Draw(Graphics g)
        {
            //g.DrawLine(_LaserPen, LaserPoints[0], LaserPoints[1]);
            LinkedListNode<PointF> currelement = LaserPoints.First;
            while (currelement.Next != null)
            {
                g.DrawLine(_LaserPen, currelement.Value, currelement.Next.Value);
                currelement = currelement.Next;



            }
        }




    }
}