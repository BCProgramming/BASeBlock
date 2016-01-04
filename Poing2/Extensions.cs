using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace BASeCamp.BASeBlock
{
    public static class ObjectExtensions
    {
        public static String GetAdvancedInfo(this Object someobj)
        {
            if(someobj==null) return "Null";
            StringBuilder buildresult = new StringBuilder();
            Type grabtype = someobj.GetType();

            buildresult.Append(grabtype.Name + " {\n");
            buildresult.Append("Fields:\n");
            //iterate through all gettable fields.
            foreach (var fielditerate in grabtype.GetFields())
            {
                
                if (fielditerate.FieldType.IsPrimitive)
                    buildresult.Append(fielditerate.Name + " = " + fielditerate.GetValue(someobj).ToString());
                else
                {
                    buildresult.Append(fielditerate.Name + " = \n");
                    buildresult.Append(fielditerate.GetValue(someobj).GetAdvancedInfo());


                }



            }
            //now, do the same for properties.
            buildresult.Append("}\n");
            buildresult.Append("Properties:\n");

            foreach (var propiterate in grabtype.GetProperties())
            {
                try
                {
                    if (propiterate.PropertyType.IsPrimitive)
                    {
                        buildresult.Append(propiterate.Name + " = " + propiterate.GetValue(someobj, null).ToString());
                    }
                    else
                    {
                        buildresult.Append(propiterate.Name + " = \n");
                        buildresult.Append(propiterate.GetValue(someobj, null).GetAdvancedInfo());

                    }
                }
                catch (TargetInvocationException exx)
                {
                    buildresult.Append(propiterate.Name + " = (Exception Accessing)");


                }

            }


            buildresult.Append("}");

            return buildresult.ToString();
        }


    }
    public static class iImagableExtensions
    {
        public static Image getImage(this iImagable source)
        {
            
                Bitmap sourceimage = new Bitmap(source.Size.Width, source.Size.Height);
                Graphics g = Graphics.FromImage(sourceimage);
                var Copied = (iImagable)(source is ICloneable ? (source as ICloneable).Clone() : source);
                var templocation = Copied.Location;
                var tempsize = Copied.Size;
                try
                {
                    Copied.Location = new Point(0, 0);
                    
                    g.Clear(Color.Transparent);
                    Copied.Draw(g);
                    return sourceimage;
                }
                finally
                {
                    Copied.Location = templocation;
                    Copied.Size = tempsize;

                }





        }

    }
    public static class ColourExtensions
    {

        public static Color Darken(this Color Darkenit, int amount)
        {


            return Color.FromArgb(Darkenit.A, BCBlockGameState.ClampValue(Darkenit.R - amount, 0, 255),
                BCBlockGameState.ClampValue(Darkenit.G - amount, 0, 255),
                BCBlockGameState.ClampValue(Darkenit.B - amount, 0, 255));

        }


    }

    public static class GraphicsPathExtensions
    {
        public static void AddString(this GraphicsPath gp, String addme, Font usefont,PointF origin, StringFormat sformat)
        {
            gp.AddString(addme, usefont.FontFamily, (int)usefont.Style, usefont.Size, origin, sformat);


        }
        public static void AddString(this GraphicsPath gp, String addme, Font usefont, Point origin, StringFormat sformat)
        {
            gp.AddString(addme, usefont.FontFamily, (int)usefont.Style, usefont.Size, origin, sformat);


        }

    }
    public static class RandomExtensions
    {
        public static double NextDouble(this Random r, double Minimum, double Maximum)
        {
            return (r.NextDouble() * (Maximum - Minimum)) + Minimum;



        }


    }
    public static class PointExtensions
    {
        public static PointF ToPointF(this Point pt)
        {

            return new PointF((float)pt.X,(float)pt.Y);

            
            
        }


    }
    public static class PointFExtensions
    {

        public static PointF Rotate90(this PointF target)
        {

            return new PointF(target.Y, -target.X);
        }

        public static PointF Mirror(this PointF target, PointF Normal)
        {
            Normal = Normal.Normalize();
            var dotproduct = -target.X * Normal.X - target.Y * Normal.Y;
            PointF returnvalue =  new PointF(
                target.X + 2 * Normal.X * dotproduct,
                target.Y + 2 * Normal.Y * dotproduct);

#if DEBUG
            if (Single.IsNaN(target.X) || Single.IsNaN(target.Y))
            {
                Debug.Assert(false);

            }

#endif

            return returnvalue;

        }
        public static float DotProduct(this PointF pf,PointF vector)
        {

            return pf.X * vector.X + pf.Y * vector.Y;

        }
        public static PointF Normalize(this PointF param)
        {
            /*
             public float Magnitude {
            get { return (float)Math.Sqrt(X * X + Y * Y); }
        }

        public void Normalize() {
            float magnitude = Magnitude;
            X = X / magnitude;
            Y = Y / magnitude;
        }
             */


            float magnitude = param.Magnitude();
            if (magnitude==0) return new PointF(0, 0);
            return new PointF(param.X / magnitude, param.Y / magnitude);
        }
        public static float Magnitude(this  PointF param)
        {
            return (float)Math.Sqrt(param.X * param.X + param.Y * param.Y);

        }
        private static PointF XYFunction(this PointF A, PointF B, Func<float, float, float> lambdaexpr)
        {

            return new PointF(lambdaexpr(A.X, B.X), lambdaexpr(A.Y, B.Y));



        }
        public static Point ToPoint(this PointF param)
        {
            return new Point((int)param.X, (int)param.Y);

        }
        public static PointF Each(this PointF param,Func<float,float> functioncall)
        {
            return new PointF(functioncall(param.X), functioncall(param.Y));

        }

    }
    public static class BitmapExtensions
    {


        public static IEnumerable<Color> getPixels(this Bitmap src)
        {
            //src.GetPixel
            for (int x = 0; x < src.Width; x++)
                for (int y = 0; y < src.Height; y++)
                    yield return src.GetPixel(x, y);



        }


    }
    public static class ImageExtensions
    {

        public static Image ClipImage(this Image ourimage,Point TopLeft, Size ClipSize)
        {
            Bitmap newimage = new Bitmap(ClipSize.Width, ClipSize.Height);
            
                using (Graphics drawcanvas = Graphics.FromImage(newimage))
                {

                    drawcanvas.DrawImage(ourimage, 0, 0, new Rectangle(TopLeft, ClipSize), GraphicsUnit.Pixel);


                }


            return newimage;
            



        }


    }

    public static class SortedListExtensions
    {

        public static void Add<T>(this SortedList<T, T> addto, T Element)
        {
            addto.Add(Element, Element);
        }


    }

    public static class TimeSpanExtensions
    {

        public static String FriendlyString(this TimeSpan ts)
        {
            StringBuilder buildreturn = new StringBuilder();

            List<String> results = new List<string>();

            if(ts.Days !=0)
                results.Add(ts.Days.ToString() + " Days");
            if (ts.Hours != 0)
                results.Add(ts.Hours.ToString() + " Hours");

            if (ts.Minutes != 0)
                results.Add(ts.Minutes.ToString() + " Minutes");

            if (ts.Seconds != 0)
                results.Add(ts.Seconds.ToString() + " Seconds");


            return String.Join("," , results.ToArray());


        }


    }
  
    public static class RectangleExtensions
    {
        
        public static PointF CenterPoint(this RectangleF rect)
        {
            return new PointF(rect.Left+(rect.Width/2),rect.Top+(rect.Height/2));


        }
        public static PointF TopCenter(this RectangleF rect)
        {

            return new PointF(rect.Left + (rect.Width / 2), rect.Top);


        }
        public static PointF RandomSpot(this RectangleF rect,Random rg)
        {
            return new PointF(rect.Left + (float)(rg.NextDouble() * rect.Width),
                rect.Top + (float)(rg.NextDouble() * rect.Height));

        }
        public static PointF BottomCenter(this RectangleF rect)
        {
            return new PointF(rect.Left + (rect.Width / 2), rect.Bottom-1);


        }
        public static PointF MiddleLeft(this RectangleF rect)
        {

            return new PointF(rect.Left, rect.Top + rect.Height / 2);
        }
        public static PointF MiddleRight(this RectangleF rect)
        {
            return new PointF(rect.Right-1, rect.Top + rect.Height / 2);
        }

       
        public static PointF TopRight(this RectangleF rect)
        {
            return new PointF(rect.Right-1, rect.Top);

        }
        public static PointF TopLeft(this RectangleF rect)
        {
            return new PointF(rect.Left, rect.Top);

        }

        public static PointF BottomLeft(this RectangleF rect)
        {

            return new PointF(rect.Left, rect.Bottom-1);

        }
        public static PointF BottomRight(this RectangleF rect)
        {

            return new PointF(rect.Right-1, rect.Bottom-1);

        }
        public static PointF[] Corners(this RectangleF rect)
        {

            return new PointF[] {
             new PointF(rect.Left,rect.Top),
             new PointF(rect.Right,rect.Top),
             new PointF(rect.Right,rect.Bottom),
            new PointF(rect.Left,rect.Bottom)
                
            };


        }
        public static Point[] Corners(this Rectangle rect)
        {

            return new Point[] {
             new Point(rect.Left,rect.Top),
             new Point(rect.Right,rect.Top),
             new Point(rect.Right,rect.Bottom),
            new Point(rect.Left,rect.Bottom)
                
            };


        }

        public static Rectangle ToRectangle(this RectangleF rect)
        {
            return new Rectangle((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);

        }
      
        public static RectangleF ToRectangleF(this Rectangle rect)
        {
            return new RectangleF(rect.Left,rect.Top,rect.Width,rect.Height);

        }

        public static Point CenterPoint(this Rectangle rect)
        {


            PointF getpoint = rect.ToRectangleF().CenterPoint();
            return new Point((int)getpoint.X,(int)getpoint.Y);
        }


        public static bool Contains(this Rectangle rect, IEnumerable<PointF> Polypoints)
        {

            return (from m in Polypoints where rect.Contains(new Point((int)m.X,(int)m.Y)) select m).Any();

        }
        public static RectangleF Union(this RectangleF rect, RectangleF other)
        {
            float MinLeft = Math.Min(rect.Left, other.Left);
            float MinTop = Math.Min(rect.Top, other.Top);
            float usewidth = Math.Max(rect.Right, other.Right) - MinLeft;
            float useheight = Math.Max(rect.Bottom, other.Bottom) - MinTop;
            return new RectangleF(MinLeft, MinTop, usewidth, useheight);



        }
    }

    public static class StreamExtensions
    {

        public static void WriteObject<T>(this Stream str,T writeobj) where T:ISerializable
        {
            BCBlockGameState.ObjectToStream(writeobj, str);
            


        }
        public static T ReadObject<T>(this Stream str) where T : ISerializable
        {

            return BCBlockGameState.StreamToObject<T>(str);

        }

    }

    public static class ISerializableExtensions
    {

        public static void SaveToStream(this ISerializable serialize, Stream target)
        {

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(target, serialize);


        }
        


    }

    public static class SerializationInfoExtensions
    {
        public static void AddValue(this SerializationInfo info, String key, Image saveimage)
        {
            //create a memory stream, and save the image to it as a PNG...
            using (MemoryStream mstream = new MemoryStream())
            {
                saveimage.Save(mstream, ImageFormat.Png);
                //now seek to the start of the memory stream.
                //mstream.Seek(0, SeekOrigin.Begin);
                byte[] imagedata = mstream.ToArray();
                info.AddValue(key, imagedata);
            }
        }
        public static void AddValueType(this SerializationInfo info, String key, Type Value)
        {

            if (Value == null)
                info.AddValue(key, "null");
            else
            {
                info.AddValue(key, Value.Name);

            }


        }
        public static Type GetValueType(this SerializationInfo info, String key)
        {
            String strresult = info.GetString(key);
            if (strresult.Equals("null", StringComparison.OrdinalIgnoreCase)) 
                return null;

            return BCBlockGameState.FindClass(strresult);



        }
        public static void AddValue<T>(this SerializationInfo info, String key, T Value)
        {
            info.AddValue(key, Value);

        }
        public static T GetValue<T>(this SerializationInfo info, String key)
        {
            return (T)info.GetValue(key, typeof(T));
        }



        public static Image getImage(this SerializationInfo info, String key)
        {

            byte[] imagedata = (byte[])info.GetValue(key,typeof(byte[]));
            MemoryStream mstream = new MemoryStream(imagedata);
            Image readimage = Image.FromStream(mstream);

            return readimage;

        }

    }
    public static class StackExtensions
    {

        public static void TrimTo<T>(this Stack<T> stacktotrim, int maxelements)
        {
            if (stacktotrim.Count < maxelements) return; //nothing to do...
            Stack<T> pushto = new Stack<T>();
            while (stacktotrim.Count > 0 && pushto.Count < maxelements)
                pushto.Push(stacktotrim.Pop());

            stacktotrim.Clear();
            while (pushto.Count > 0)
                stacktotrim.Push(pushto.Pop());




        }
        public static void TestExtension()
        {

            Stack<int> teststack = new Stack<int>();
            for(int i=0;i<50;i++)
            {
                teststack.Push(i);


            }

            teststack.TrimTo(10);
            Debug.Print(teststack.ToString());
        }


    }
    public static class ListExtensions
    {

        public static Queue<T> Clone<T>(this Queue<T> QueueToClone) where T : ICloneable
        {
            return new Queue<T>(QueueToClone.Select(item=>(T)item.Clone()));



        }
            public static List<T> Clone<T>(this IEnumerable<T> listToClone) where T : ICloneable
            {
                return listToClone.Select(item => (T)item.Clone()).ToList();
            }
            public static List<T> ShallowClone<T>(this IEnumerable<T> listtoclone)
            {
                lock (listtoclone)
                {
                    return new List<T>(listtoclone);
                }

            }
            public static LinkedList<T> Clone<T>(this LinkedList<T> listtoClone) where T : ICloneable
            {
                return new LinkedList<T>(listtoClone.Select(item=>(T)item.Clone()));
            }
            public static LinkedList<T> ShallowClone<T>(this LinkedList<T> listtoClone)
            {
                return new LinkedList<T>(listtoClone);

            }

    }
    
}
