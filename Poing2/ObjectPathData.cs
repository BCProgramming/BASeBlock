using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using BASeCamp.Elementizer;

namespace BASeCamp.BASeBlock
{
    /// <summary>
    /// Class that holds a collection of ObjectPathData objects, and manages them in the context of the game and editor.
    
    /// </summary>
    
    [Serializable]
    
    public class ObjectPathDataManager:Dictionary<String,ObjectPathData>,  ISerializable,IXmlPersistable
    {
       // private Dictionary<String, ObjectPathData> _PathData = new Dictionary<string, ObjectPathData>();
        

        public new ObjectPathData this[String Key]
        {
            get { return base[Key]; }
            set { base[Key] = value;}
        }

        public ObjectPathDataManager()
        {

        }
        public ObjectPathDataManager(SerializationInfo info, StreamingContext context):base(info,context)
        {
            
           //_PathData = (Dictionary<String, ObjectPathData> ) info.GetValue("DataDictionary", typeof(Dictionary<String, ObjectPathData>));
            //autocorrect ourselves.
         




        }
        public ObjectPathDataManager(XElement Source)
        {

        }
        public XElement GetXmlData(String pNodeName)
        {
            List<KeyValuePair<String,ObjectPathData>> Contents = this.AsEnumerable().ToList();
            XElement ResultNode = new XElement(pNodeName);
            foreach(var kvpitem in Contents)
            {
                //<PathData Key="Name"><ObjectPathData /></PathData>
                XElement BuildNode = new XElement("PathData",new XAttribute("Key",kvpitem.Key));
                BuildNode.Add(kvpitem.Value.GetXmlData("PathPoints"));
            }
            return ResultNode;
            //return StandardHelper.SaveList<ObjectPathData>()
        }
        public override void OnDeserialization(object sender)
        {
            Debug.Print("ObjectPathDataManager::OnDeserialization");
            base.OnDeserialization(sender);
            List<ObjectPathData> readditems = new List<ObjectPathData>();


            foreach (var loopitem in this)
            {
                readditems.Add(loopitem.Value);


            }

            Clear(); //clear ourself.

            foreach (var addit in readditems)
            {
                Add(addit.Name, addit);


            }
        }
        public void Remove(ObjectPathData removethis)
        {
            String findkey;
            var foundenum = (from q in this where q.Value == removethis select q.Key);
            if (foundenum.Count() > 0)
            {
                findkey = foundenum.First();
                base.Remove(findkey);
            }

        }

        public void ToAllPoints(Action<ObjectPathDataPoint> actiondelegate)
        {
            foreach (var looppath in this)
            {
                foreach (var looppoint in looppath.Value.PathPoints)
                {
                    actiondelegate(looppoint);


                }


            }


        }

        public String Add(ObjectPathData itemadd)
        {
            if (itemadd.Name==null||itemadd.Name == "")
            {
                itemadd.Name = Guid.NewGuid().ToString();



            }
            Add(itemadd.Name, itemadd);
            return itemadd.Name;


        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);


        }
        /// <summary>
        /// hittests a given point, finding any appropriate point and it's corresponding pathdata/
        /// </summary>
        /// <param name="Location">PointF location to test.</param>
        /// <param name="returnpathdata">parameter which receives the pathdata for the matched point</param>
        /// <param name="returnpathpoint">parameter which receives the closest pathpoint. </param>
        /// <param name="tolerance">distance the given point must be within.</param>
        /// <returns>true to indicate a point was found, false otherwise.</returns>
        /// <remarks>this routine will only find a single point and it's associated pathdata; if multiple points from different paths are on the exact same place, behaviour is undefined.</remarks>
        public bool HitTest(PointF Location,out ObjectPathData returnpathdata,out ObjectPathDataPoint returnpathpoint,float tolerance)
        {
            foreach (var iteratedata in this)
            {
                ObjectPathDataPoint testpoint = iteratedata.Value.HitTest(Location, tolerance);
                if (testpoint != null)
                {
                    returnpathdata=iteratedata.Value;
                    returnpathpoint=testpoint;
                    return true;


                }


            }
            returnpathdata=null;
            returnpathpoint=null;
            return false;

        }
        public List<ObjectPathDataPoint> HitTest(PointF CenterPoint, float Radius)
        {
            List<ObjectPathDataPoint> returnvalue = new List<ObjectPathDataPoint>();
            foreach (var iteratedata in this)
            {
                //loop through all it's points...
                foreach (var looppoint in iteratedata.Value.PathPoints)
                {

                    if (BCBlockGameState.Distance(CenterPoint.X, CenterPoint.Y, looppoint.Location.X, looppoint.Location.Y) < Radius)
                    {
                        returnvalue.Add(looppoint);

                    }

                }


            }



            return returnvalue;

        }

        public List<ObjectPathDataPoint> HitTest(RectangleF testrectangle)
        {
            List<ObjectPathDataPoint> createlist = new List<ObjectPathDataPoint>();
            foreach (var iteratedata in this)
            {
                createlist.AddRange(iteratedata.Value.HitTest(testrectangle));


            }
            return createlist;



        }

        public ObjectPathData FindDataForPoint(ObjectPathDataPoint findthis)
        {
            foreach (var loopvalue in this)
            {
                if(loopvalue.Value.PathPoints.Contains(findthis))
                    return loopvalue.Value;


            }

            return null;

        }

    }
    
    [Serializable]
    public class ObjectPathDataPoint : ISerializable, ICloneable,IXmlPersistable
    {
        
        private PointF _Location;
        private String _Label;
        private bool _Selected;
        //public PointF Location { get { return _Location; } set { _Location=value; Raise
        internal event ObjectPathData.ObjectPathChangeEventDelegate PropertyChanged;
        [Browsable(false)]
        public bool Selected { get { return _Selected; } set
        {
            _Selected = value;
            RaiseChange("Selected", _Selected);
        }}
            public String Label
        {
            get { return _Label; }
            set
            {
                _Label = value;
                RaiseChange("Label", _Label);
            }
        }
            public PointF Location
        {
            get { return _Location; }
            set
            {
                _Location = value;
                RaiseChange("Location", _Location);
            }
        }
            public void Draw(Graphics g)
            {
                //current: just draw a small, 5x5 box around the location, fill it if we are selected.
                g.DrawRectangle(new Pen(Color.Black), Location.X - 3, Location.Y - 3, 6, 6);
                if (_Selected) g.FillRectangle(new SolidBrush(Color.Black), Location.X - 3, Location.Y - 3, 6, 6);



            }

        private void RaiseChange(String PropertyName, Object newValue)
        {
            var invokethis = PropertyChanged;
            if (invokethis != null)
            {
                invokethis.Invoke(PropertyName, newValue);


            }

        }
        public ObjectPathDataPoint(PointF initpoint)
        {
            _Location=initpoint;

        }
        public ObjectPathDataPoint(ObjectPathDataPoint opdp)
        {
            _Location = opdp._Location;
            Label=opdp.Label;


        }

        public ObjectPathDataPoint(SerializationInfo info, StreamingContext context)
        {
            Location = (PointF)info.GetValue("Location", typeof(PointF));
            Label = info.GetString("Label");


        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Location", _Location);
            info.AddValue("Label", _Label);


        }
        public ObjectPathDataPoint(XElement Source)
        {
            XElement LocationNode = (XElement)Source.FirstNode;
            _Location = StandardHelper.ReadElement<PointF>(LocationNode);
            _Label = Source.Attribute("Label").Value;
            
        }
        public XElement GetXmlData(String pNodeName)
        {
            XElement result = new XElement(pNodeName);
            result.Add(StandardHelper.SaveElement(_Location,"Location",false));
            result.Add(new XAttribute("Label",_Label));
            return result;
        }
        public static explicit operator PointF(ObjectPathDataPoint source)
        {
            return source._Location;


        }

        public object Clone()
        {
            return new ObjectPathDataPoint(this);

        }


    }



    /// provides methods for hittesting, acquiring all selected points, unselecting, etc.
    [Serializable]
    public class ObjectPathData :ISerializable,ICloneable ,IXmlPersistable 
    {
        internal delegate void ObjectPathChangeEventDelegate(String PropertyName, Object newValue);

        internal event ObjectPathChangeEventDelegate PropertyChanged;


        private bool _Visible;
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                _Visible = value;
                RaiseChange("Visible", _Visible);
            }
        }
            private String _Name="";
            private List<ObjectPathDataPoint> _PathPoints = new List<ObjectPathDataPoint>();

        public PointF[] PointData
        {
            get { return (from n in _PathPoints select n.Location).ToArray(); }

            set { 
            
                _PathPoints = new List<ObjectPathDataPoint>();
                foreach(var loopitem in value)
                {
                    _PathPoints.Add(new ObjectPathDataPoint (loopitem));


                }

                

                }

            }

        


        public String Name
        {
            get { return _Name; }
            set
        {
            _Name = value;
            RaiseChange("Name", _Name);
        }}

            private void RaiseChange(String PropertyName,Object newValue)
        {
            var invokethis = PropertyChanged;
            if (invokethis != null)
            {
                invokethis.Invoke(PropertyName, newValue);


            }

        }
            public ObjectPathDataPoint AppendPoint(ObjectPathDataPoint appendthis)
            {
                _PathPoints.Add(appendthis);

                return appendthis;
            }
            public ObjectPathDataPoint AppendPoint(PointF Location)
            {
                ObjectPathDataPoint createpoint = new ObjectPathDataPoint(Location);
                
                createpoint.PropertyChanged += hookit_PropertyChanged;
                _PathPoints.Add(createpoint);

                return createpoint;
            }

        //hittesting, acquiring all selected values, etc

            public ObjectPathDataPoint[] getSelected()
            {

                return (from n in PathPoints where n.Selected select n).ToArray();

            }

        //and hitTesting...

        /// <summary>
        /// Hittests for points in and around the given point. Only returns the closest point.
        /// </summary>
        /// <param name="testpoint">Point to test.</param>
        /// <param name="tolerance">Allowable distance.</param>
        /// <returns></returns>
            public ObjectPathDataPoint HitTest(PointF testpoint, float tolerance)
            {
                var grabit = (from n in PathPoints
                              where
                                  BCBlockGameState.Distance(testpoint.X, testpoint.Y, n.Location.X, n.Location.Y) < tolerance
                              select n);
                if (!grabit.Any()) return null; else return grabit.First();



            }
            public ObjectPathDataPoint HitTest(PointF testpoint)
            {

                return HitTest(testpoint, 5f);

            }
        //and hittesting for rectangular areas.
            public ObjectPathDataPoint[] HitTest(RectangleF testarea)
            {

                return (from n in PathPoints where testarea.Contains(n.Location) select n).ToArray();



            }


        public void Draw(Graphics g)
            {
                //draw this set of points.
                bool doflipflop=false;
                for (int i = 1; i < _PathPoints.Count; i++)
                {
                    doflipflop=!doflipflop;
                    PointF prevpoint = _PathPoints[i - 1].Location;
                    PointF currpoint = _PathPoints[i].Location;
                    LinearGradientBrush LineDrawBrush;
                    if(doflipflop)
                        LineDrawBrush = new LinearGradientBrush(prevpoint, currpoint, Color.Red, Color.Black);
                    else
                        LineDrawBrush = new LinearGradientBrush(prevpoint, currpoint, Color.Black, Color.Red);
                        
                    
                    //draw it
                    Pen drawlinepen = new Pen(LineDrawBrush,1);
                    g.DrawLine(drawlinepen, prevpoint, currpoint);

                }



            //g.DrawLines(new Pen(Color.Black), PointData);

                foreach (var looppoint in _PathPoints)
                {
                    looppoint.Draw(g);


                }
            }

            public List<ObjectPathDataPoint> PathPoints { get { return _PathPoints; } set { _PathPoints = value; } }


            public ObjectPathData(List<ObjectPathDataPoint> pathdata)
            {
                _PathPoints=pathdata;

                foreach (var hookit in pathdata)
                {
                 
                    hookit.PropertyChanged += new ObjectPathChangeEventDelegate(hookit_PropertyChanged);


                }
            }

        public ObjectPathData(ObjectPathDataPoint[] pathdata)
        {

           // _PathPoints=pathdata;
            foreach (var hookit in pathdata)
            {
                _PathPoints.Add(hookit);
                hookit.PropertyChanged += new ObjectPathChangeEventDelegate(hookit_PropertyChanged);


            }
        }

        void hookit_PropertyChanged(string PropertyName, object newValue)
        {
            RaiseChange("PathPoints", _PathPoints);
        }

        public ObjectPathData()
        {


        }

        public ObjectPathData(GraphicsPath gpath)
        {
            gpath.Flatten();
            PointData = gpath.PathPoints;



        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Debug.Print("Saving path named " + _Name);
            info.AddValue("Name", _Name);
            info.AddValue("PathPoints", _PathPoints);


        }public XElement GetXmlData(String pNodeName)
        {
            XElement resultnode = new XElement(pNodeName,new XAttribute("Name",_Name));
            resultnode.Add(StandardHelper.SaveList<ObjectPathDataPoint>(_PathPoints,"PathPoints",false));
            return resultnode;
        }
        public ObjectPathData(XElement Source)
        {
            _Name = Source.Attribute("Name").Value;
            _PathPoints = StandardHelper.ReadList<ObjectPathDataPoint>((XElement)Source.FirstNode);

        }

        public ObjectPathData(SerializationInfo info, StreamingContext context)
        {
            _Name = info.GetString("Name");
            Debug.Print("Loading path named " + _Name);
            _PathPoints = (List<ObjectPathDataPoint>)info.GetValue("PathPoints", typeof(List<ObjectPathDataPoint>));

        }
        public Object Clone()
        {

            return new ObjectPathData((List<ObjectPathDataPoint>)_PathPoints.Clone());


        }
    }
}
