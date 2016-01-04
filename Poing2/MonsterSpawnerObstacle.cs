using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BASeCamp.BASeBlock.Blocks;

namespace BASeCamp.BASeBlock
{
    public class MonsterSpawnerObstacle : PolygonObstacle
    {
        public class MonsterPolyDraw : PolygonBlockDrawMethod
        {
            private Pen _LinePen;
            public Pen LinePen { get { return _LinePen; } set { _LinePen = value; } }
            private Brush _FillBrush;
            public Brush FillBrush { get { return _FillBrush;} set {_FillBrush = value;}}

            public MonsterPolyDraw(Pen pPen, Brush pFill)
            {
                _LinePen = pPen;
                _FillBrush = pFill;
            }
            public MonsterPolyDraw(MonsterPolyDraw CloneThis)
            {
                _LinePen = CloneThis.LinePen;
                _FillBrush = CloneThis.FillBrush;
            }
            public MonsterPolyDraw(Pen pPen) : this(pPen, Brushes.Transparent) { }
            public MonsterPolyDraw() : this(new Pen(Color.LightSeaGreen, 2)) { }
            public override void Draw(object source, Polygon pb, Graphics g)
            {

                var Grabpoints = pb.getPoints().ToArray();
                g.FillPolygon(_FillBrush, Grabpoints);
                //g.DrawPolygon(_LinePen, Grabpoints);
                foreach (var iterateA in Grabpoints)
                {
                    foreach (var iterateB in Grabpoints)
                    {
                        if (iterateA != iterateB)
                        {
                            //draw a line.
                            g.DrawLine(LinePen, iterateA, iterateB);
                        }
                    }
                }


            }

            public override object Clone()
            {
                return new MonsterPolyDraw(this);
            }
        }
        private Type _SpawnType;
        public Type SpawnType { get { return _SpawnType; } 
            set {
                if (!(value.GetType().IsSubclassOf(typeof(GameEnemy))))
                {
                    throw new ArgumentException("SpawnType must be a GameEnemy Subclass.");
                }    
                _SpawnType = value;
                GrabIcon();

            } }

        #region constructors
        protected virtual void GrabIcon()
        {

        }
        public MonsterSpawnerObstacle(IEnumerable<PointF> Points)
            : this(typeof(EyeGuy), Points)
        {

        }
        public MonsterSpawnerObstacle(Type pSpawnType,IEnumerable<PointF> Points) : base(Points)
        {
            SpawnType = pSpawnType;
        }

        
        
        #endregion

        public override void Draw(Graphics g)
        {
            //Draw the Icon of the thing we spawn.

        }
    }


}
