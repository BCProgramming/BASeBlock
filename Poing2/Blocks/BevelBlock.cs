using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;

namespace BASeBlock.Blocks
{
    [BlockDescription("Block which draws as a bevel shape.")]
    [StandardBlockCategory]
    public class BevelBlock:Block
    {
        //BevelBlock
        public enum BlockSideEnumeration
        {
            LeftSide=3,
            TopSide=0,
            RightSide=1,
            BottomSide=2



        }
        private Polygon[] _tris = new Polygon[4];
        private Brush[] _FillBrushes = new Brush[]{new SolidBrush(Color.White),
                                                   new SolidBrush(Color.Blue),
                                                   new SolidBrush(Color.SkyBlue),
                                                   new SolidBrush(Color.DodgerBlue) };
        public Brush[] FillBrushes { get { return _FillBrushes; } set { _FillBrushes = value; } }

        private Color[] _FillColors = new Color[4];

        protected Color[] FillColors { get { return _FillColors; } set { _FillColors = value; FillBrushes = (from p in _FillColors select (Brush)(new SolidBrush(p))).ToArray(); } }
        private void setcolors()
        {
            FillBrushes = (from p in _FillColors select (Brush)(new SolidBrush(p))).ToArray();

        }
        public Color LeftColor
        {
            get { return _FillColors[(int)BlockSideEnumeration.LeftSide]; }
            set { _FillColors[(int)BlockSideEnumeration.LeftSide] = value; setcolors(); }
        }
        public Color TopColor
        {
            get { return _FillColors[(int)BlockSideEnumeration.TopSide]; }
            set { _FillColors[(int)BlockSideEnumeration.TopSide] = value; setcolors(); }
        }
        public Color RightColor
        {
            get { return _FillColors[(int)BlockSideEnumeration.RightSide]; }
            set { _FillColors[(int)BlockSideEnumeration.RightSide] = value; setcolors(); }
        }
        public Color BottomColor
        {
            get { return _FillColors[(int)BlockSideEnumeration.BottomSide]; }
            set { _FillColors[(int)BlockSideEnumeration.BottomSide] = value; setcolors(); }

        }

        public Brush getFillBrush(BlockSideEnumeration side)
        {
            return _FillBrushes[(int)side];

        }
        public void setFillBrush(BlockSideEnumeration side,Brush value)
        {
            _FillBrushes[(int)side] = value;

        }
        public BevelBlock(BevelBlock clonethis)
            : base(clonethis)
        {
            _FillBrushes = (from p in clonethis.FillBrushes select (Brush)p.Clone()).ToArray();
            
            RefreshPolies();

        }
        public override object Clone()
        {
            return new BevelBlock(this);
        }
        protected void RefreshPolies()
        {
            _tris = new Polygon[] {
                                      new Polygon(BlockRectangle.TopLeft(),BlockRectangle.TopRight(),BlockRectangle.CenterPoint()),
                                      new Polygon(BlockRectangle.TopRight(),BlockRectangle.BottomRight(),BlockRectangle.CenterPoint()),
                                      new Polygon(BlockRectangle.BottomRight(),BlockRectangle.BottomLeft(),BlockRectangle.CenterPoint()),
                                      new Polygon(BlockRectangle.BottomLeft(),BlockRectangle.TopLeft(),BlockRectangle.CenterPoint())};



            


        }
        private Pen _DrawPen = new Pen(Color.Black, 1);
        public override void Draw(Graphics g)
        {
            for (int i = 0; i < _tris.Length;i++ )
            {
                g.FillPolygon(_FillBrushes[i], _tris[i]);
                g.DrawPolygon(_DrawPen, _tris[i]);

            }
        }
        public BevelBlock(RectangleF blockrect)
            : base()
        {
            base.BlockRectangle = blockrect;
            base.OnBlockRectangleChange += new Action<RectangleF>(BevelBlock_OnBlockRectangleChange);
            RefreshPolies();
        }
      
        public BevelBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            info.AddValue("FillColors", FillColors);

        }

        void BevelBlock_OnBlockRectangleChange(RectangleF obj)
        {
            RefreshPolies();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            FillColors = (Color[])info.GetValue("FillColors",typeof(Color[]));
        }
        


    }
}