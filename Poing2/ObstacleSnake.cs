using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using BASeBlock.Blocks;

namespace BASeBlock
{
    class ObstacleSnake : GameEnemy,IMovingObject
    {
        private LinkedList<PolygonObstacle> _Segments = new LinkedList<PolygonObstacle>();
        public PointF Velocity
        {
            get
            {
                return HeadSegment.Value.Velocity;
            }
            set
            {
                HeadSegment.Value.Velocity = value;
            }
        }
        public LinkedList<PolygonObstacle> Segments { get { return _Segments; } set { _Segments = value; } }
        public LinkedListNode<PolygonObstacle> HeadSegment { get { return _Segments != null?_Segments.Find(_Segments.First()):null; } set { _Segments.RemoveFirst(); _Segments.AddBefore(HeadSegment, new LinkedListNode<PolygonObstacle>(value.Value)); } }
        public LinkedListNode<PolygonObstacle> TailSegment { get { return _Segments != null ? _Segments.Last : null; } set { _Segments.RemoveLast(); _Segments.AddLast(value); } } 



        private bool _Preinit = false;
        public int HeadDistance(PolygonObstacle test)
        {
            int currcount = 0;
            var current = HeadSegment;
            while (current.Value != test)
            {
                current = current.Next;
                currcount++;
            }
            return currcount;
        }
        public int TailDistance(PolygonObstacle test)
        {
            int currcount = 0;
            var current = TailSegment;
            while(current.Value!=test)
            {
                current=current.Previous;
                currcount++;
            }
            return currcount;
        }
      public ObstacleSnake(PointF pLocation,int pSegmentCount,Func<int,PointF,PolygonObstacle> Initializer):base(pLocation,new Dictionary<string, string[]>(),100)
        {
          //create the Segments.
            PointF CurrentSpot = pLocation;
            for (int i = 0; i < pSegmentCount; i++)
            {
                PolygonObstacle generated = Initializer(i, pLocation);
                generated.Location = CurrentSpot;
                generated.ObstacleDestroy += DestroyEvent;
                generated.ObstacleHit += generated_ObstacleHit;
                Segments.AddLast(generated);
                CurrentSpot = new PointF(CurrentSpot.X + 10, CurrentSpot.Y + 10);
            }
            HeadSegment.Value.Velocity = new PointF(3, 3);
            base.DrawSize = HeadSegment.Value.DrawSize;

        }
      public ObstacleSnake(LinkedList<PolygonObstacle> pSegments):base(pSegments.First.Value.Location,new Dictionary<string, string[]>(),100)
      {
          foreach (var iterate in pSegments)
              Segments.AddLast(iterate);



      }
      void generated_ObstacleHit(object sender, PolygonObstacle.PolygonObstacleHitEventArgs e)
      {
          if (e.Obstacle != HeadSegment.Value)
          {
              //split the snake at this segment.
              //the segment we hit becomes the tail of the original snake.
              //and the next obstacle is made the head of a new snake.

              
              LinkedListNode<PolygonObstacle> HitNode = Segments.Find(e.Obstacle);
              if (HitNode == null) return;
              LinkedList<PolygonObstacle> buildlist = new LinkedList<PolygonObstacle>();
              var currnode = HitNode.Next;
              while (currnode != null)
              {
                  buildlist.AddLast(currnode);
                  _Segments.Remove(currnode);
                  currnode = currnode.Next;
              }
              ObstacleSnake newsnake = new ObstacleSnake(buildlist);
              //add the new snake.
              e.GameState.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => e.GameState.GameObjects.AddLast(newsnake)));

              e.Cancel = true;
          }
          else
          {
              //death.
              //release all the segments.
              
              base.HitPoints--;
              _Dying = base.HitPoints == 0;
              
          } 

      }
      public ObstacleSnake(PointF pLocation,SizeF pSize): 
          this(pLocation, 12, (i, p) => 
      { 
              var polyobstacle =  new PolygonObstacle(p, 3, 12, 3, 10, new FilledPolyDrawMethod(Color.Black, HSLColor.RandomHue(240, 120)));
              polyobstacle.VelocityDecay = new PointF(1, 1);
              polyobstacle.CurrAngle = BCBlockGameState.rgen.NextDouble() * Math.PI * 2;
              polyobstacle.AngleSpeed = Math.PI * 2 / 4 * BCBlockGameState.rgen.NextDouble();
              polyobstacle.Location = pLocation;
              polyobstacle.MinimumSize = 25;
          

              return polyobstacle;
          })
        {
            
        }
        public void DestroyEvent(Object sender,PolygonObstacle.PolygonObstacleDestroyEventArgs e)
          {
            //remove that Obstacle from our LinkedList.
              if (e.Obstacle != HeadSegment.Value)
              {
                  Segments.Remove(e.Obstacle);
                  _Dying = Segments.Count == 0;
              }
          }
        private double SlitherDirection = 0; //slither angle.
      public ObstacleSnake() : this(PointF.Empty,new SizeF(16,16)) { }
      private bool _Dying = false;
      public override void Draw(Graphics g)
      {
          //do nothing, since we are just a "controller" for other components.
         /* var CurrPos = HeadSegment;
          for (CurrPos = CurrPos.Next; CurrPos != null; CurrPos = CurrPos.Next)
          {
              PointF prevLocation = CurrPos.Previous.Value.Location;
              PointF currLocation = CurrPos.Value.Location;
              g.DrawLine(new Pen(Color.Black, 1), prevLocation, currLocation);


          }
          */

      }
      private void SlitherAI(BCBlockGameState gamestate)
      {
          double maximumangle = (Math.PI / 16);
          if ((DateTime.Now.Second % 3) == 0)
          {
              SlitherDirection += (BCBlockGameState.rgen.NextDouble() * (maximumangle)-(maximumangle/2));
              if (Math.Abs(SlitherDirection) > maximumangle)
              {
                  SlitherDirection = maximumangle * Math.Sign(SlitherDirection);
              }

          }
          //if we are within
          //our length to any side...
          var ourlength = getLength();
          if(HeadSegment.Value.Location.X < ourlength ||
              HeadSegment.Value.Location.Y < ourlength ||
              HeadSegment.Value.Location.X > gamestate.GameArea.Width-ourlength ||
              HeadSegment.Value.Location.Y > gamestate.GameArea.Height-ourlength)
          {
              HeadSegment.Value.Velocity = BCBlockGameState.NudgeTowards(HeadSegment.Value.Location, HeadSegment.Value.Velocity, gamestate.GameArea.CenterPoint(), (float)maximumangle);
          }

          //additional check, make sure the angle of our movement is at least 90 degrees
          //difference from our angle toward all the segments of the snake.
          //retrieve the angle of our movement direction.
          double moveangle = BCBlockGameState.GetAngle(PointF.Empty, HeadSegment.Value.Velocity);
          double[] segmentangles = (from x in Segments where x != HeadSegment.Value select BCBlockGameState.GetAngle(HeadSegment.Value.Location, x.Location)).ToArray();
          
          if (segmentangles.Min() < Math.PI / 2)
          {
              double currentVector = moveangle;
              double currentspeed = HeadSegment.Value.Velocity.Magnitude();
              //now, offset this angle by our slither direction.
              currentVector += (Math.PI / 2) * ((BCBlockGameState.rgen.NextDouble() > 0.5) ? -1 : 1);
              //and convert it back to a X,Y pair.
              HeadSegment.Value.Velocity = new PointF((float)(Math.Cos(currentVector) * currentspeed), (float)(Math.Sin(currentVector) * currentspeed));

          }



      }
        public double getLength()
        {
            var currnode = HeadSegment;
            double curraccum = 0;
            while (currnode.Next != null)
            {

                curraccum += BCBlockGameState.Distance(currnode.Value.Location, currnode.Next.Value.Location);
                currnode = currnode.Next;
            }


            return curraccum;
        }
      private void Slither()
      {
          //changes the Head Segments current velocity by incrementing it angularly.
          //first retrieve the current Angle.
          double currentVector = BCBlockGameState.GetAngle(PointF.Empty, HeadSegment.Value.Velocity);
          double currentspeed = HeadSegment.Value.Velocity.Magnitude();
          //now, offset this angle by our slither direction.
          currentVector += SlitherDirection;
          //and convert it back to a X,Y pair.
          HeadSegment.Value.Velocity = new PointF((float)(Math.Cos(currentVector) * currentspeed), (float)(Math.Sin(currentVector) * currentspeed));



      }
      private PointF Elastic(PointF Source, PointF Neighbour, float relaxedLength, float stiffness = .3f)
      {
          /*
           * F = springvector / length * (length - normal_length) * k 
           * springvector = (position of neighbour point) - (position of point)
           * length = actual length of spring
           * 
           * normal_length = default length of spring when no force is applied on it (constant)
           * k = spring stiffness (constant)
           * */


          PointF springvector = new PointF(Neighbour.X - Source.X, Neighbour.Y - Source.Y);
            //get actual length...
          float lengthof = BCBlockGameState.Distance(Neighbour, Source);
          float FX = springvector.X / lengthof * (lengthof - relaxedLength) * stiffness;
          float FY = springvector.Y / lengthof * (lengthof - relaxedLength) * stiffness;
          return new PointF(FX, FY);
      }

        private PointF getForce(PointF Source,PointF? NeighbourA,PointF? NeighbourB,float relaxedLength,float stiffness)
      {
            //retrieve the force on each point, then take the difference and return
            //the force in one direction.

          PointF ForceA = NeighbourA != null ? Elastic(Source, NeighbourA.Value, 10) : PointF.Empty;
          PointF ForceB = NeighbourB != null ? Elastic(Source, NeighbourB.Value, 10) : PointF.Empty;

          PointF difference = new PointF(ForceB.X - ForceA.X, ForceB.Y - ForceA.Y);
          return difference;











      }
        

      public override bool PerformFrame(BCBlockGameState gamestate)
      {
          if (Frozen) return false ;
          if (HeadSegment == null) return true;
          if (!_Preinit)
          {
              _Preinit = true;
              gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
              {
                  foreach (var iterate in Segments)
                  {
                      gamestate.GameObjects.AddLast(iterate);
                  }
                  HeadSegment.Value.fShoot = true;
              }));
          }

          //apply logic to all segments, in order.
          //really we just need to tweak their speeds.
          SlitherAI(gamestate);
          Slither();
          var firstnode = Segments.First;
          var HeadSpeed = HeadSegment.Value.Velocity.Magnitude();
          for (var currnode = firstnode.Next; currnode != null; currnode = currnode.Next)
          {
              
              //it is happening in reverse order.
              if (currnode.Previous != null)
              {
                  var previouselement = currnode.Previous;
                  var currentelement = currnode.Value;
                  var nextelement = currnode.Next;
                  //the previous element is the next 'segment' forward on the snake.
                  
                  PointF? PrevPt = (previouselement != null ? (PointF?)previouselement.Value.Location : null);
                  //PointF? NextPt = (nextelement != null ? (PointF?)nextelement.Value.Location : null);


                  //new logic:
                  //instead of "elasticity"; just go with a difference.
                  //each point attaches to the Previous point
                  //the desired distance is the radius of this element plus the radius of the previous element.
                  float DesiredDistance = (currnode.Previous.Value.Poly.AverageRadius() + currnode.Value.Poly.AverageRadius())/2;
                  //get the distance between the current node and the previous node.
                  float currentdistance = BCBlockGameState.Distance(currnode.Value.Location, previouselement.Value.Location);
                  //is the distance More than the Desired Difference?
                  if (currentdistance > DesiredDistance)
                  {

                      //if it is, we need to calculate the angle between the Previous node and the current node, then use Trig and
                      //set the location of the current node to DesiredDistance at that angle.
                      var retrieveangle = BCBlockGameState.GetAngle(currnode.Previous.Value.Location, currnode.Value.Location);
                      PointF useoffset = new PointF((float)(Math.Cos(retrieveangle) * DesiredDistance), (float)(Math.Sin(retrieveangle) * DesiredDistance));
                      //that is the offset, add it to the Previous value's location, and we have where we want this item to be.
                      PointF desiredlocation = new PointF(currnode.Previous.Value.Location.X + useoffset.X, currnode.Previous.Value.Location.Y + useoffset.Y);

                      PointF usespeed = new PointF(desiredlocation.X - currnode.Value.Location.X, desiredlocation.Y - currnode.Value.Location.Y);
                      float totalspeed = usespeed.Magnitude();
                      float usevector = (float)(BCBlockGameState.GetAngle(PointF.Empty, usespeed));

                      //max out the speed at the speed of the Head Segment.
                      totalspeed = Math.Min(totalspeed, HeadSpeed);
                      currnode.Value.Velocity = new PointF((float)(Math.Cos(usevector) * totalspeed), (float)(Math.Sin(usevector) * totalspeed));


                      
                  }
                  //if the distance is less than the desired distance, we don't care, and just do nothing.





                  /*//retrieve the force on the current point.
                  PointF useforce = getForce(currentelement.Location, PrevPt, NextPt, 1, 0.3f);
                  useforce = new PointF(Math.Min(useforce.X, 5), Math.Min(useforce.Y, 5));
                  Debug.Print("Force:" + useforce);
                  currentelement.Velocity = useforce;*/
                  

               

              }


          }




              return  _Dying;
      }
        
    }
}
