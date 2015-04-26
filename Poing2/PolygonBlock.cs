using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BASeBlock.Blocks
{
    [Serializable]
    public abstract class PolygonBlockDrawMethod:ISerializable,ICloneable 
    {

        public abstract void Draw(Object source,Polygon pb, Graphics g);
        public abstract object Clone();
        protected PolygonBlockDrawMethod()
        {


        }
        protected PolygonBlockDrawMethod(SerializationInfo info,StreamingContext context)
        {


        }
        
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //
        }
    }
    [Serializable]
    [Description("Spokes")]
    public class SpokedPolyDrawMethod : PolygonBlockDrawMethod
    {
        private PolygonBlockDrawMethod _SpokeDraw = new FilledPolyDrawMethod();


        [Editor(typeof(ItemTypeEditor<PolygonBlockDrawMethod>), typeof(UITypeEditor))]
        public Type SectorDrawMethodType { get { return _SpokeDraw.GetType(); } set { _SpokeDraw = (PolygonBlockDrawMethod)Activator.CreateInstance(value); } }

        
        [Editor(typeof(ObjectTypeEditor), typeof(UITypeEditor))]



        public PolygonBlockDrawMethod SectorDrawMethod { get { return _SpokeDraw; } set { _SpokeDraw = value; } }

        public delegate PolygonBlockDrawMethod GetDrawMethod(Object Sender, Polygon pb);

        private GetDrawMethod _DrawRetriever=null;



        private PolygonBlockDrawMethod DefaultDrawMethod(Object sender,Polygon pb)
        {
            return _SpokeDraw;
             
        }


        public override void Draw(Object Source, Polygon pb, Graphics g)
        {
            //
            //first, create a list of Polygons based on the polygon itself.
            Polygon[] subpolys = pb.Split();
            foreach (Polygon pg in subpolys)
            {
                if (_DrawRetriever == null) _DrawRetriever = DefaultDrawMethod;
                var usedraw = _DrawRetriever(Source, pb);
                
                usedraw.Draw(Source,pg, g);

            }


        }
        public SpokedPolyDrawMethod()
        {
            _DrawRetriever = DefaultDrawMethod;
        }
        public override object Clone()
        {
            return new SpokedPolyDrawMethod(this);
        }
        public SpokedPolyDrawMethod(SpokedPolyDrawMethod spoker)
        {
            _SpokeDraw = (PolygonBlockDrawMethod)spoker.SectorDrawMethod.Clone();
            



        }
        public SpokedPolyDrawMethod(SerializationInfo info, StreamingContext context):base(info,context)
        {
            _SpokeDraw = (PolygonBlockDrawMethod)info.GetValue("SectorDraw", typeof(PolygonBlockDrawMethod));

            _DrawRetriever = DefaultDrawMethod;
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SectorDraw", _SpokeDraw);


            base.GetObjectData(info, context);
        }
    }
    [Serializable]
    [Description("PathGradient")]
    public class PathGradientPolyDrawMethod : PolygonBlockDrawMethod
    {
        private Color _CenterColor = Color.Blue;
        private Color[] _OuterColors = new Color[] {Color.Red};

        public Color CenterColor { get { return _CenterColor; } set { _CenterColor = value; } }
        public Color[] OuterColors { get { return _OuterColors; } set { _OuterColors = value; } }
        public Color OuterColor { get { return _OuterColors[0]; } set { _OuterColors = new Color[] { value }; } }

        public PathGradientPolyDrawMethod(SerializationInfo info, StreamingContext context)
        {
            _CenterColor = (Color)info.GetValue("CenterColor",typeof(Color));
            _OuterColors= (Color[])info.GetValue("OuterColors",typeof(Color[]));


        }
        public PathGradientPolyDrawMethod(PathGradientPolyDrawMethod pgp)
        {
            _CenterColor = pgp.CenterColor;
            _OuterColors = pgp.OuterColors;
            
        }
        public override object Clone()
        {
            return new PathGradientPolyDrawMethod(this);
        }
        public PathGradientPolyDrawMethod()
        {


        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("CenterColor", _CenterColor);
            info.AddValue("OuterColors", _OuterColors);
        }
        public override void Draw(Object Source, Polygon pb, Graphics g)
        {
            using (GraphicsPath usepath = new GraphicsPath())
            {
                PointF[] gotpoints = (from p in pb.Points select (PointF)p).ToArray();
                usepath.AddPolygon(gotpoints);
                using (PathGradientBrush pgb = new PathGradientBrush(usepath))
                {
                    pgb.CenterColor = _CenterColor;
                    //pgb.InterpolationColors = new ColorBlend(_OuterColors.Length+1);
                    //pgb.InterpolationColors.Colors = _OuterColors;
                    //Color[] useouter = new Color[pb.Points.Count];
                    Color[] useouter = (from p in pb.Points select _OuterColors[0]).ToArray();
                    pgb.SurroundColors = _OuterColors;
                    pgb.CenterPoint = pb.Center;

                    g.FillPolygon(pgb, gotpoints);


                }



            }
        }
    }



    [Serializable]
    [Description("Fill and Stroke")]
    public class FilledPolyDrawMethod : PolygonBlockDrawMethod
    {
        private Pen _StrokePen;
        private Brush _FillBrush;

        public Pen StrokePen
        {
            get { return _StrokePen; }
            set { _StrokePen = value; }
        }
        public Brush FillBrush
        {
            get { return _FillBrush; }
            set { _FillBrush = value; }

        }

        private Color _StrokeColor;
        private Color _FillColor;

        public Color StrokeColor { get { return _StrokeColor; } set { _StrokeColor = value; StrokePen = new Pen(_StrokeColor,0.5f); } }
        public Color FillColor { get { return _FillColor; } set { _FillColor = value; FillBrush = new SolidBrush(_FillColor); } }

        public override object Clone()
        {
            return new FilledPolyDrawMethod(this);
        }
        public FilledPolyDrawMethod(FilledPolyDrawMethod clonefrom)
        {
            StrokeColor = clonefrom.StrokeColor;
            FillColor = clonefrom.FillColor;


        }
        public FilledPolyDrawMethod(Color pPenColor,Color pFillColor)
        {
            StrokeColor = pPenColor;
            FillColor = pFillColor;

        }
        internal FilledPolyDrawMethod(Pen pPen,Brush pBrush)
        {
            _StrokePen = pPen;
            _FillBrush = pBrush;
        }
        public FilledPolyDrawMethod()
            : this(Color.Black, Color.Red)
        {


        }
        public FilledPolyDrawMethod(SerializationInfo info, StreamingContext context):base(info,context)
        {
            StrokeColor = (Color)info.GetValue("StrokeColor", typeof(Color));
            FillColor = (Color)info.GetValue("FillColor", typeof(Color));


        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("StrokeColor", StrokeColor);
            info.AddValue("FillColor", FillColor);

        }



        public override void Draw(Object  Source, Polygon pb, Graphics g)
        {
            //
            var drawpoints = (from p in pb.Points select (PointF)p).ToArray();
            g.FillPolygon(_FillBrush, drawpoints);
            g.DrawPolygon(_StrokePen, drawpoints);
        }



    }



    [Serializable]
    [PolygonBlockCategory]
    public abstract class PolygonBlock : Block,ISerializable,IGameInitializer
    {
        

        public static void GameInitialize(iManagerCallback datahook)
        {


        }


        protected Polygon _OurPoly;
#if DEBUG
        private PolygonBlockDrawMethod _pdraw = new FilledPolyDrawMethod();
        protected PolygonBlockDrawMethod _Drawer { get { return _pdraw; } set { _pdraw = value; } }
#else

        protected PolygonBlockDrawMethod _Drawer = new FilledPolyDrawMethod();
#endif
        [Editor(typeof (ItemTypeEditor<PolygonBlockDrawMethod>),typeof(UITypeEditor))]
        public Type DrawMethodType { get { return _Drawer.GetType(); } set { _Drawer = (PolygonBlockDrawMethod)Activator.CreateInstance(value); } }

        
        [Editor(typeof(ObjectTypeEditor),typeof(UITypeEditor))]
        public PolygonBlockDrawMethod Drawer { get { return _Drawer; } set { _Drawer = value; } }


        [Browsable(false)]
        public override RectangleF BlockRectangle { get { return base.BlockRectangle; } set { base.BlockRectangle = value; } }

     
        public Polygon Poly
        {
            get { return _OurPoly; }
            set { _OurPoly = value;
            Updaterect();
            
            }


        }
        protected bool ignorerectchange = false;
        protected virtual void Updaterect()
        {
            if (!ignorerectchange)
            {
                ignorerectchange = true;

                BlockRectangle = Poly.GetBounds();
                ignorerectchange = false;
            }
            else
            {
                mBlockrect = Poly.GetBounds();
            }

        }
        protected virtual void RectChanged(RectangleF changerect)
        {
            if (ignorerectchange) return;
            ignorerectchange = true;
            try
            {
                if (PreviousRect == null)
                {
                    PreviousRect = BlockRectangle;

                }
                else
                {
                    PointF getoffset = new PointF(BlockRectangle.X - PreviousRect.Value.X, BlockRectangle.Y - PreviousRect.Value.Y);


                    foreach (var ptloop in Poly.Points)
                    {
                        ptloop.Offset(getoffset);


                    }
                    Poly.BuildEdges();
                }

            }
            finally
            {
                ignorerectchange = false;
            }

        }
        //constructors and clone...
        protected PolygonBlock(RectangleF fromRect):this(new Polygon(fromRect))
        {
            PreviousRect = fromRect;
            base.OnBlockRectangleChange += new Action<RectangleF>(PolygonBlock_OnBlockRectangleChange);
        }

        protected RectangleF? PreviousRect = null;



        void PolygonBlock_OnBlockRectangleChange(RectangleF obj)
        {

            RectChanged(obj);

        }
        
        protected PolygonBlock(Polygon usepoly)
        {
            Poly = usepoly;
            base.OnBlockRectangleChange += new Action<RectangleF>(PolygonBlock_OnBlockRectangleChange);

        }
        public override bool HitTest(PointF position)
        {
            return _OurPoly.PointInPoly(position);
        }
        protected PolygonBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {

            Poly = (Polygon)info.GetValue("OurPoly", typeof(Polygon));
            try { _Drawer = (PolygonBlockDrawMethod)info.GetValue("Drawer", typeof(PolygonBlockDrawMethod)); }
            catch { _Drawer = new FilledPolyDrawMethod(); }
            base.OnBlockRectangleChange += new Action<RectangleF>(PolygonBlock_OnBlockRectangleChange);
            PreviousRect = BlockRectangle;
        }
       
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("OurPoly", _OurPoly);
            info.AddValue("Drawer", _Drawer);
            base.GetObjectData(info, context);
        }
        
        protected PolygonBlock(PolygonBlock clonethis)
            : base(clonethis)
        {
            Poly = clonethis.Poly;
            base.OnBlockRectangleChange += new Action<RectangleF>(PolygonBlock_OnBlockRectangleChange);
            _Drawer = (PolygonBlockDrawMethod)clonethis.Drawer.Clone();
            PreviousRect = BlockRectangle;
        }
        
       public override bool MustDestroy()
       {
           return false ;

       }
        //collision related overrides.

        public override bool DoesBallTouch(cBall hitball)
        {
            Polygon ballpoly = hitball.GetBallPoly();
            Vector Adjustment = new Vector();
            GeometryHelper.PolygonCollisionResult pcr = GeometryHelper.PolygonCollision(GetPoly(), ballpoly, new Vector(hitball.Velocity.X, hitball.Velocity.Y));
            //create a polygon for both the ball and this ellipse.
            // polygon for this ellipse is EllipsePoly.
            //adjustment is amount to move ballpoly to keep it "outside" the ellipse poly.
            //this adjustment vector has the additional helpful task of giving us 
            //the normal of the collision point.

            return pcr.Intersect;

        }
        public override Block.BallRelativeConstants getBallRelative(cBall forball)
        {
            //getBallRelative is only used for the rectangular collision detection code in the base Block class.
            //this class overrides the collision detection logic with it's own, meaning we need to return 0 for this to prevent
            // the base class from screwing about with some of the state information before we are called.
            return (Block.BallRelativeConstants)0;
        }
        
        public override Polygon GetPoly()
        {
            return _OurPoly;

        }
        public override PointF CenterPoint()
        {
            return Poly.Center;
        }
        public override void Draw(Graphics g)
        {
            _Drawer.Draw(this,this.Poly, g);
            //g.DrawPolygon(new Pen(Color.Black, 2), (from p in _OurPoly.Points select new PointF(p.X, p.Y)).ToArray());
        }
        public override void DrawSelection(Brush selectionbrush, Graphics g, IEditorClient Client)
        {
            //base.DrawSelection(selectionbrush, g);
            g.FillPolygon(selectionbrush, (from m in Poly.Points select (PointF)m).ToArray());
        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {

            Polygon ballpoly = ballhit.GetBallPoly();
            Vector Adjustment = new Vector();
            GeometryHelper.PolygonCollisionResult pcr = GeometryHelper.PolygonCollision(GetPoly(), ballpoly, new Vector(ballhit.Velocity.X, ballhit.Velocity.Y));
            Adjustment = pcr.MinimumTranslationVector;
            //minimumtranslationvector will be the normal we want to mirror the ball speed through.
            ballhit.Velocity = ballhit.Velocity.Mirror(pcr.MinimumTranslationVector);
            ballhit.Velocity = new PointF(ballhit.Velocity.X, ballhit.Velocity.Y);
            ballhit.Location = new PointF(ballhit.Location.X - Adjustment.X, ballhit.Location.Y - Adjustment.Y);
            base.PerformBlockHit(parentstate, ballhit);
            BCBlockGameState.Soundman.PlaySound(DefaultHitSound);
            return Destructable;
        }


    }



    /// <summary>
    /// Block that creates a regular polygon with 3 or more sides.
    /// </summary>
    [Serializable]
    public class RegularPolygonBlock : PolygonBlock
    {

        private int _PolySides = 3;
        private PointF _PolyCenter = PointF.Empty;
        private float _PolySize = 5;
        private float  _AngleOffset = 0;
        private static float ToRadians(float degrees)
        {

            return (float)(degrees * (Math.PI / 180));

        }
        private static float ToDegrees(float Radians)
        {
            return (float)(Radians / (Math.PI / 180));


        }

        public int PolySides { get { return _PolySides; } set { _PolySides = value; RebuildPoly(); } }
        public float PolySize { get { return _PolySize; } set { _PolySize = value; RebuildPoly(); } }
        public float PolyAngleOffsetDegrees { get { return (float)(_AngleOffset/(Math.PI/180)); } set { PolyAngleOffset = (float)(value*(Math.PI/180)); } }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public float PolyAngleOffset { get { return _AngleOffset; } set { _AngleOffset = value; RebuildPoly(); } }
        [TypeConverter(typeof(FloatFConverter))]
        public PointF PolyCenter { get { return _PolyCenter; } set { _PolyCenter = value; RebuildPoly(); } }

        private static Polygon CreateRegularPoly(PointF pCenter, int pSides,float pSize,float pAngleoffset)
        {
            PointF[] Polypoints = new PointF[pSides];
            double sideangle = (2d * Math.PI) / (float)pSides;
            for (int i = 0; i < pSides; i++)
            {
                Polypoints[i] = new PointF((float)(pCenter.X + Math.Cos(pAngleoffset+sideangle * i) * pSize),
                    (float)(pCenter.Y + Math.Sin(pAngleoffset+sideangle * i) * pSize));


            }


            return new Polygon(Polypoints);


        }


        private void RebuildPoly()
        {

            Poly = CreateRegularPoly(PolyCenter, PolySides, PolySize, PolyAngleOffset);



        }
        public RegularPolygonBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {

            _PolySides = info.GetInt32("Sides");
            _PolySize = info.GetSingle("Size");
            _PolyCenter = (PointF)info.GetValue("CenterSpot", typeof(PointF));
            _AngleOffset = info.GetSingle("AngleOffset");

            OnBlockRectangleChange += new Action<RectangleF>(RegularPolygonBlock_OnBlockRectangleChange);
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Sides", _PolySides);
            info.AddValue("Size", _PolySize);
            info.AddValue("CenterSpot", _PolyCenter);
            info.AddValue("AngleOffset", _AngleOffset);

        }
        public RegularPolygonBlock(PointF CenterSpot, int Sides,float pSize,float pAngleOffset)
            : base(CreateRegularPoly(CenterSpot, Sides,pSize,pAngleOffset))
        {
            _PolySides = Sides;
            _PolySize = pSize;
            _PolyCenter = CenterSpot;
            _AngleOffset = pAngleOffset;
            OnBlockRectangleChange += new Action<RectangleF>(RegularPolygonBlock_OnBlockRectangleChange);
        }

        void RegularPolygonBlock_OnBlockRectangleChange(RectangleF obj)
        {
            PolyCenter = obj.CenterPoint();
        }
        public RegularPolygonBlock(RegularPolygonBlock clonethis)
            : base(clonethis)
        {
            _PolySize = clonethis.PolySize;
            _PolyCenter = clonethis.PolyCenter;
            _PolySides = clonethis.PolySides;
            PolyAngleOffset = clonethis.PolyAngleOffset;
            OnBlockRectangleChange += new Action<RectangleF>(RegularPolygonBlock_OnBlockRectangleChange);
        }
        public RegularPolygonBlock(RectangleF rectuse):base(rectuse)
        {
            _PolyCenter = rectuse.CenterPoint();
            _PolySides = 4;
            _PolySize = Math.Max(rectuse.Width, rectuse.Height);
            OnBlockRectangleChange += new Action<RectangleF>(RegularPolygonBlock_OnBlockRectangleChange);

        }
        public override object Clone()
        {
            return new RegularPolygonBlock(this);
        }



    }

        
    [Serializable]
    class EllipseBlock : PolygonBlock, ISerializable
    {

        //EllipseBlock:
        //private PointF _CenterPoint;
        //private float _Radius;
        

        
        private float EllipseAngle = 0f;

        private PointF _Foci1, _Foci2;
            private float _Radius;

            [TypeConverter(typeof(FloatFConverter))]
        public PointF Foci1
    {
        get { return _Foci1; }
        set { _Foci1 = value;doUpdate(); }

    }
            [TypeConverter(typeof(FloatFConverter))]
        public PointF Foci2
        {
            get { return _Foci2; }
            set
            {_Foci2 = value;doUpdate();}

        }
        public float Radius
        {
            get { return _Radius; }
            set
            {
                _Radius = value;
                doUpdate();
            }
        }
            protected void doUpdate()
        {

            UpdateEllipse(_Foci1, _Foci2, _Radius);

        }

        public override object Clone()
        {
            return new EllipseBlock(this);
        }
        public EllipseBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            
            _Foci1 = (PointF)info.GetValue("Foci1", typeof(PointF));
            _Foci2 = (PointF)info.GetValue("Foci2", typeof(PointF));
            Radius = info.GetSingle("Radius");

            //base.OnBlockRectangleChange += rectchange;
            

            
            


        }
        public override bool MustDestroy()
        {
            return false;
        }
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {

            base.GetObjectData(info, context);
            info.AddValue("Foci1", Foci1);
            info.AddValue("Foci2", Foci2);
            info.AddValue("Radius", Radius);


            
        }
        protected Polygon getBlockPoly()
        {
            RectangleF outrect;
            float outellipseangle;
            return new Polygon(BCBlockGameState.GetEllipsePoints(_Foci1, _Foci2, _Radius, out outellipseangle, out outrect));
            



        }
        
        protected void Collide(cBall hitball,PointF NormalVector)
        {
            
            NormalVector = NormalVector.Normalize();
            Polygon ballpoly = hitball.GetBallPoly();
            Vector Adjustment = new Vector();


            hitball.Velocity = hitball.Velocity.Mirror(NormalVector);

            //rotate Velocity by the angle of Adjustment.
            /*
             vectN is direction of ball before hitting object
             Vect2 is after collision
             WallN is normal 
             DOT is dot product
             * 
             
             * */
            






        }
        public override bool DoesBallTouch(cBall hitball)
        {
            //return base.DoesBallTouch(hitball);

            //merely determine of the ball touches.

            Polygon ballpoly = hitball.GetBallPoly();
            Vector Adjustment= new Vector();
            GeometryHelper.PolygonCollisionResult pcr = GeometryHelper.PolygonCollision(_OurPoly,ballpoly, new Vector(hitball.Velocity.X,hitball.Velocity.Y));
            //create a polygon for both the ball and this ellipse.
            // polygon for this ellipse is EllipsePoly.
            //adjustment is amount to move ballpoly to keep it "outside" the ellipse poly.
            //this adjustment vector has the additional helpful task of giving us 
            //the normal of the collision point.

            return pcr.Intersect;



            return false; //stub.


        }
        public override void Draw(Graphics g)
        {
            base.Draw(g);
            //g.DrawEllipse(new Pen(Color.Black, 2), BlockRectangle);
        }
        public override void EditorDraw(Graphics g, IEditorClient Client)
        {
            base.EditorDraw(g,Client);

            //draw foci points.
            //
            Font usefont = BCBlockGameState.GetScaledFont(new Font(BCBlockGameState.GetMonospaceFont(), 10), 12);

            //draw a dot on the focus, and the numeric indicator.
            foreach (int i in new int[] { 1, 2 })
            {
                PointF drawpoint = i == 1 ? Foci1 : Foci2;
                GraphicsPath gp = new GraphicsPath();

                gp.AddRectangle(new RectangleF(drawpoint.X,drawpoint.Y,2,2));
                
                gp.AddString(i.ToString(),usefont,drawpoint,StringFormat.GenericDefault);

                g.FillPath(new SolidBrush(Color.Black), gp);
                g.DrawPath(new Pen(Color.White, 0.5f), gp);
                gp.Dispose();


            }




        }

        public EllipseBlock(RectangleF EllipseRect)
            : base(EllipseRect)
        {
            
            //OnBlockRectangleChange+=new Action<RectangleF>(EllipseBlock_OnBlockRectangleChange);
            
            EllipseBlock_OnBlockRectangleChange(EllipseRect);
            //base.OnBlockRectangleChange += rectchange;
        }

        public EllipseBlock(EllipseBlock clonethis)
            : base(clonethis)
        {

            //OnBlockRectangleChange += new Action<RectangleF>(EllipseBlock_OnBlockRectangleChange);
            _Foci1 = clonethis.Foci1;
            _Foci2 = clonethis.Foci2;
            Radius = clonethis.Radius;
            //base.OnBlockRectangleChange += rectchange;

        }
        public EllipseBlock(PointF pfoci1, PointF pfoci2, float pRadius):base((Polygon)null)
        {

            UpdateEllipse(pfoci1, pfoci2, pRadius);
            //OnBlockRectangleChange += new Action<RectangleF>(EllipseBlock_OnBlockRectangleChange);

        }
       
        /// <summary>
        /// Given two foci and the radius, updates the other values to match (Polygon, rectangle, etc)
        /// </summary>
        /// <param name="pFoci1"></param>
        /// <param name="pFoci2"></param>
        /// <param name="pRadius"></param>
        protected void UpdateEllipse(PointF pFoci1, PointF pFoci2, float pRadius)
        {
            ignorerectchange=true;

            _Foci1 = pFoci1;
            _Foci2 = pFoci2;
            _Radius=pRadius;
            RectangleF mBlockRectangle;
            PointF[] epoints = BCBlockGameState.GetEllipsePoints(pFoci1, pFoci2, pRadius, out EllipseAngle,out mBlockRectangle);
            //BlockRectangle = mBlockRectangle;

            Poly = new Polygon(epoints);
            
            ignorerectchange=false;
        }
        

        private RectangleF? oldrect;

    
        protected override void RectChanged(RectangleF changerect)
        {
            Debug.Print("EllipseBlock::RectChanged");
            if (ignorerectchange) return;
            ignorerectchange = true;
            if (oldrect == null) oldrect = BlockRectangle;

            PointF diff = new PointF(BlockRectangle.X - oldrect.Value.X, BlockRectangle.Y - oldrect.Value.Y);


            _Foci1 = new PointF(_Foci1.X + diff.X, _Foci1.Y + diff.Y);
            _Foci2 = new PointF(_Foci2.X + diff.X, _Foci2.Y + diff.Y);
            //_Radius = Radius;
            UpdateEllipse(_Foci1, _Foci2, Radius);
            oldrect = BlockRectangle;
            ignorerectchange = false;
        }
        /// <summary>
        /// assigned as an event but does the "opposite" of updateellipse; that is, it updates from rectangle info to foci and radius and whatnot.
        /// </summary>
        /// <param name="obj"></param>
        void EllipseBlock_OnBlockRectangleChange(RectangleF obj)
        {
            if(ignorerectchange) return;
            //throw new NotImplementedException();

            PointF[] ellipsepoints = BCBlockGameState.GetEllipsePoints(obj);
            _OurPoly = new Polygon(ellipsepoints);
            EllipseAngle = 0;
            PointF centerspot = new PointF(obj.Left + (obj.Width/2),obj.Top + (obj.Height/2));
            float majoraxis,minoraxis;
            BCBlockGameState.Coordinates2D majorcoord;
            BCBlockGameState.GetEllipseAxes(obj,out majoraxis,out minoraxis,out majorcoord);

            //majoraxis ^2 - Minoraxis ^2 = 

            float offset = (float)Math.Sqrt(Math.Pow(majoraxis, 2) - Math.Pow(minoraxis, 2));
            
            if(majorcoord == BCBlockGameState.Coordinates2D.COORD_X)
            {
                _Foci1 = new PointF(centerspot.X -offset,centerspot.Y);
                _Foci2 = new PointF(centerspot.X + offset,centerspot.Y);

            }
            else
            {
                _Foci1 = new PointF(centerspot.X,centerspot.X-offset);
                _Foci2 = new PointF(centerspot.X,centerspot.X+offset);
            }
            _Radius = Math.Abs((majoraxis/2)-offset);
        }

        

    }
}
