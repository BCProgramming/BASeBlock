using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using System.Windows.Forms;
namespace BASeCamp.BASeBlock
{

    public class TrackBarEditor:UITypeEditor
    {
        public class TrackBarEditorData:Attribute
        {
            public double Minimum = 0;
            public double Maximum = 200;
            public TrackBarEditorData(double minimum, double maximum)
            {
                Minimum = minimum;
                Maximum = maximum;

            }




        }
        
        private IWindowsFormsEditorService mserv;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        const int minvalueset = 0;
        const int maxvalueset = short.MaxValue;
        private TrackBar trackbarobject = null;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            //look for a TrackBarEditorData Attribute.
            TrackBarEditorData usedata = null;
            foreach (Attribute iterate in context.PropertyDescriptor.Attributes)
            {
                if (iterate.GetType() == typeof(TrackBarEditorData))
                {
                    usedata = (TrackBarEditorData)iterate;
                    break;
                }

            }
            if (usedata == null) usedata = new TrackBarEditorData(0, 200);
            var returntype = value.GetType(); //type to return.
            Debug.Print(("Type of context.instance-" + context.Instance.GetType().Name));
            if (provider != null)
            {
                
                mserv = ((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService)));

                if (mserv != null)
                {
                    // This is an internal Microsoft class representing the PropertyGrid entry for our component.
                    PictureBox DropDownBox = new PictureBox();
                    DropDownBox.BorderStyle = BorderStyle.None;
                    DropDownBox.Resize += new EventHandler(DropDownBox_Resize);
                    TrackBar tb = new TrackBar();
                    //set the minimum to min, and maximum to maximum value.
                    tb.Minimum = minvalueset;
                    tb.Maximum=maxvalueset;
                    //scale the value as needed.
                    double currentvalue = (double)(Convert.ChangeType(value, TypeCode.Double)??0);
                    tb.LargeChange = 4096;
                    tb.SmallChange = 256;
                    tb.TickFrequency = 256;
                    tb.TickStyle = TickStyle.BottomRight;
                    
                    
                    //scale it to between Int.MinValue and int.MaxValue.

                    double Percentage = currentvalue/(usedata.Maximum - usedata.Minimum);
                    if (double.IsInfinity(Percentage)) Percentage = 0;
                    int scaled = (int)((((double)maxvalueset - (double)minvalueset) * Percentage) + (double)minvalueset);
                    

                    tb.Value = scaled;

                    // Drop the trackbar
                    
                    mserv.DropDownControl(tb);

                    scaled = tb.Value;
                    //now we need to work backwards.
                    Percentage = ((double)scaled) / ((double)maxvalueset - (double)minvalueset);
                    double usethisvalue = Percentage * (usedata.Maximum - usedata.Minimum) + usedata.Minimum;

                    value = Convert.ChangeType(usethisvalue, returntype);

                    // Close the list control after selection.
                    mserv.CloseDropDown();
                }
            }
            return value;
        }

        void DropDownBox_Resize(object sender, EventArgs e)
        {
            //resize TrackBarObject and other constituent controls.
        }


    }
}
