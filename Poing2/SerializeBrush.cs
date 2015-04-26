using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace BASeBlock
{
    [Serializable]
    public class SerializePen : ISerializable
    {
        private Pen mPen;

        [Editor(typeof(ObjectTypeEditor),typeof(UITypeEditor))]
        public Pen usePen { get { return mPen; } set { mPen = value; } }

        [Editor(typeof(ObjectTypeEditor),typeof(UITypeEditor))]
        public Brush PenBrush { get { return usePen.Brush; } set { usePen.Brush = value; } }

        public SerializePen(Pen PenSerialize)
        {
            mPen = PenSerialize;


        }
        public static implicit operator Pen(SerializePen fromPen)
        {
            return fromPen.GetPen();

        }
        public static explicit operator SerializePen(Pen fromPen)
        {

            return new SerializePen(fromPen);

        }
        public Pen GetPen()
        {
            return mPen;

        }
        public SerializePen(SerializationInfo info, StreamingContext context)
        {
            
            //info.AddValue("EndCap", mPen.EndCap);
            //info.AddValue("StartCap", mPen.StartCap);
            //info.AddValue("MiterLimit", mPen.MiterLimit);
            //info.AddValue("PenType", mPen.PenType);
            //info.AddValue("Transform", mPen.Transform);
            //info.AddValue("Width", mPen.Width);
            Brush brushuse;
            float fwidth;
            brushuse = ((SerializeBrush)(info.GetValue("Brush", typeof(SerializeBrush)))).brushtoserialize;

            fwidth = info.GetSingle("Width");
            mPen = new Pen(brushuse, fwidth);
            mPen.Color = info.GetValue<Color>("Color");
            mPen.CompoundArray = info.GetValue<float[]>("CompoundArray");
            mPen.CustomStartCap = info.GetValue<CustomLineCap>("CustomStartCap");
            mPen.CustomEndCap = info.GetValue<CustomLineCap>("CustomEndCap");
            mPen.DashCap = info.GetValue<DashCap>("DashCamp");

            mPen.DashOffset = info.GetSingle("DashOffset");
            mPen.DashPattern = info.GetValue<float[]>("DashPattern");
            mPen.DashStyle = info.GetValue<DashStyle>("DashStyle");
            mPen.EndCap = info.GetValue<LineCap>("EndCap");
            mPen.StartCap = info.GetValue<LineCap>("StartCap");
            mPen.MiterLimit = info.GetSingle("MiterLimit");
            //mPen.PenType = info.GetValue<PenType>("PenType");
            mPen.Transform = info.GetValue<Matrix>("Transform");
            mPen.Width = info.GetSingle("Width");
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Alignment", mPen.Alignment);
            info.AddValue("Brush", new SerializeBrush(mPen.Brush));
            info.AddValue("Color", mPen.Color);
            info.AddValue("CompoundArray", mPen.CompoundArray);
            //info.AddValue("CustomEndCap", mPen.CustomEndCap.);
            //info.AddValue("CustomStartCap", mPen.CustomStartCap);
            info.AddValue("DashCap", mPen.DashCap);
            info.AddValue("DashOffset", mPen.DashOffset);
            //info.AddValue("DashPattern", mPen.DashPattern);
            info.AddValue("DashStyle", mPen.DashStyle);
            info.AddValue("EndCap", mPen.EndCap);
            info.AddValue("StartCap", mPen.StartCap);
            info.AddValue("MiterLimit", mPen.MiterLimit);
            info.AddValue("PenType", mPen.PenType);
            info.AddValue("Transform", mPen.Transform);
            info.AddValue("Width", mPen.Width);
            
            
        }

        #endregion
    }

    //a quick class to help serialize/deserialize brushes.
    [Serializable]
    public class SerializeBrush :ISerializable
    {
        public static readonly string BrushTypeName = "BrushType";
        

        public Brush brushtoserialize=null;

        private enum BrushTypeConstants
        {
            Brush_Null, //no brush.
            Brush_Solid,  //SolidBrush
            Brush_Hatched, //HatchBrush
            Brush_Texture, //TextureBrush
            Brush_LinearGradient, //LinearGradientBrush
            Brush_PathGradient // PathGradientBrush.


        }

        public Brush getBrush()
        {
            return brushtoserialize;


        }
        public static implicit operator Brush(SerializeBrush source)
        {

            return source.getBrush();


        }
        public static explicit operator SerializeBrush(Brush source)
        {

            return new SerializeBrush(source);

        }
        #region ISerializable Members

        private BrushTypeConstants getBrushType(Brush testbrush)
        {
            if (testbrush == null)
                return BrushTypeConstants.Brush_Null;
            else if (testbrush is SolidBrush)
                return BrushTypeConstants.Brush_Solid;
            else if (testbrush is HatchBrush)
                return BrushTypeConstants.Brush_Hatched;
            else if (testbrush is TextureBrush)
                return BrushTypeConstants.Brush_Texture;
            else if (testbrush is LinearGradientBrush)
                return BrushTypeConstants.Brush_LinearGradient;
            else if (testbrush is PathGradientBrush)
                return BrushTypeConstants.Brush_PathGradient;
            //Unknown...
            return BrushTypeConstants.Brush_Null;

        }

        public SerializeBrush(Brush pBrushtoSerialize)
        {
            brushtoserialize = pBrushtoSerialize;


        }

        public SerializeBrush(SerializationInfo info, StreamingContext context)
        {
            BrushTypeConstants gotbrushtype = (BrushTypeConstants)info.GetInt32(BrushTypeName);


            switch (gotbrushtype)
            {
                case BrushTypeConstants.Brush_Null:
                    brushtoserialize=null;
                    break;
                case BrushTypeConstants.Brush_Solid:
                    Color usecolor = Color.FromArgb(info.GetInt32("Colour"));
                    brushtoserialize = new SolidBrush(usecolor);
                    break;
                case BrushTypeConstants.Brush_Hatched:
                    HatchStyle usehstyle = (HatchStyle)info.GetInt32("HatchStyle");
                    Color forecolor = Color.FromArgb(info.GetInt32("ForeColour"));
                    Color backcolor = Color.FromArgb(info.GetInt32("BackColour"));
                    brushtoserialize = new HatchBrush(usehstyle, forecolor, backcolor);



                    break;
                case BrushTypeConstants.Brush_Texture:
                    
                   
                    SerializableImage grabimage = (SerializableImage)(info.GetValue("ImageData", typeof(SerializableImage)));
                    Image gotimage = grabimage.ActualImage;
                    Matrix readmatrix = (Matrix)(info.GetValue("Transform",typeof(Matrix)));

                    WrapMode readwrapmode = (WrapMode)(info.GetInt32("WrapMode"));

                    brushtoserialize = new TextureBrush(gotimage, readwrapmode);

                   

                   

                    
                    break;
                case BrushTypeConstants.Brush_LinearGradient:
                    break;
                case BrushTypeConstants.Brush_PathGradient:
                    break;





            }




        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //serialize...
            BrushTypeConstants gotbrushtype = getBrushType(brushtoserialize);
            info.AddValue(BrushTypeName, (Int32)gotbrushtype);

            switch (gotbrushtype)
            {
                case BrushTypeConstants.Brush_Null:

                    break;
                case BrushTypeConstants.Brush_Solid:
                    SolidBrush castSolid = (SolidBrush)brushtoserialize;
                    //write the int value....
                    info.AddValue("Colour", castSolid.Color.ToArgb());
                    break;
               default:
                    throw new InvalidOperationException("Cannot serialize a non-solid Brush");


                    
            }




        }

        #endregion
    }
}
