using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BASeBlock
{



    //this file has the various TypeConverter classes that are used by the PropertyGrid and the classes whose members require conversion.


    


    class FloatFConverter:ExpandableObjectConverter 
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            //Override the CanConvertTo method and return true if the destinationType parameter 
            //is the same type as the class that uses this type converter 
            //(the SpellingOptions class in your example); otherwise, return the value of the base class CanConvertTo method. 
            if (destinationType == typeof(RectangleF) || destinationType==typeof(PointF))
                return true;
            
            else
                return base.CanConvertTo(context, destinationType);

        }
        /*
         * Override the ConvertTo method and ensure that the destinationType parameter is a
         * String and that the value is the same type as the class that uses this type converter 
         * (the SpellingOptions class in your example). If either case is false, 
         * return the value of the base class ConvertTo method; otherwise return a string representation 
         * of the value object. The string representation needs to separate each property of your class with
         * a unique delimiter. Since the whole string will be displayed in the PropertyGrid you will want to
         * choose a delimiter that doesn't detract from the readability; commas usually work well.
         * */
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String))
            {
                if (value.GetType() == typeof(RectangleF))
                {
                    RectangleF castrect = (RectangleF) value;
                    return castrect.Left.ToString() + ", " + castrect.Top.ToString() + ", " + castrect.Width.ToString() +
                           ", " + castrect.Height;
                }
                else if (value.GetType() == typeof(PointF))
                {
                    PointF castpoint = (PointF)value;
                    return castpoint.X + ", " + castpoint.Y;


                }
            }
           
                return base.ConvertTo(context, culture, value, destinationType);
            
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            //(Optional) You can enable editing of the object's string representation in the grid by specifying that your
            //type converter can convert from a string. To do this, first override the CanConvertFrom method and return 
            //true if the source Type parameter is of type String; otherwise,
            //return the value of the base class CanConvertFrom method. 
            if(sourceType==typeof(String)) return true;
           return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            /*To enable editing of the object's base class, you also need to override the ConvertFrom method and ensure 
             * that the value parameter is a String. If it is not a String, return the value of the base class ConvertFrom 
             * method; otherwise return a new instance of your class (the SpellingOptions class in your example) based on 
             * the value parameter. You will need to parse the values for each property of your class from the value parameter.
             * Knowing the format of the delimited string you created in the ConvertTo method will help you perform the parsing. */
            if(value.GetType()!=typeof(String))
                return base.ConvertFrom(context, culture, value);

            String[] parseme = ((String)value).Split(',');
            for (int i = 0; i < parseme.Length; i++)
            {
                parseme[i] = parseme[i].Trim();


            }
            if (parseme.Length>=4)
            {
                return new RectangleF(float.Parse(parseme[0]), float.Parse(parseme[1]),
                                      float.Parse(parseme[2]), float.Parse(parseme[3]));
            }
            else if (parseme.Length==2)
            {
                return new PointF(float.Parse(parseme[0]), float.Parse(parseme[1]));

            }
            return base.ConvertFrom(context, culture, value);
        }

    }
}
