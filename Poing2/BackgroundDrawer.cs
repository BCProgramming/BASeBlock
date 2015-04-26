using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BASeBlock
{
  

    public interface IBackgroundDrawer:ISerializable 
    {
        void DrawBackground(BCBlockGameState gamestate, Graphics g, Rectangle fullarea, bool pforce);
        void PerformFrame(BCBlockGameState gamestate);
        bool RequiresPerformFrame();


    }
    public abstract class BackgroundDrawer : IBackgroundDrawer
    {
        public abstract void DrawBackground(BCBlockGameState gamestate, Graphics g, Rectangle fullarea, bool pforce);
        public abstract bool RequiresPerformFrame();
        public abstract void PerformFrame(BCBlockGameState gamestate);
        public abstract void GetObjectData(SerializationInfo info,StreamingContext context);
        
        
    }
    [Serializable]
    public class BackgroundColourImageDrawer : BackgroundDrawer,ISerializable ,IDeserializationCallback
    {
        bool _iscleared=false;
        private bool BackgroundImage { get { return BackgroundFrameKeys != null && BackgroundFrameKeys.Length > 0; } }
        
        private PointF _moveVelocity= new PointF(0,0f);
        private Color _backgroundColour = Color.Gray;
        public Color BackgroundColour
        {
            get { return _backgroundColour; }
            set { _backgroundColour = value; AcquireBrushes(); }
        }
        [TypeConverter(typeof(FloatFConverter))]
        public PointF MoveVelocity { get { return _moveVelocity; } set { _moveVelocity = value; }}
        private double _rotateSpeed = 0f;
        public double RotateSpeed { get { return _rotateSpeed; } set { _rotateSpeed = value; } }
            private double _currentRotation = 0;
            public double CurrentRotation { get { return _currentRotation; } set { _currentRotation = value; AcquireBrushes(); } }


            private PointF _currentOffset = PointF.Empty;
            [TypeConverter(typeof(FloatFConverter))]
            public PointF CurrentOffset { get { return _currentOffset; } set { _currentOffset = value; AcquireBrushes(); } }
            
            private String[] _backgroundFrameKeys=null;
            [Editor(typeof(ImageKeyEditor),typeof(UITypeEditor))]
            public String FirstFrameKey
            {
                get {

                    if (BackgroundFrameKeys == null || BackgroundFrameKeys.Length < 1) return "";    
                    
                    return _backgroundFrameKeys[0]; 
                }
                set { if(value=="") return;
                    BackgroundFrameKeys = new string[]{value};_imagesacquired=false; }
            }





            public String[] BackgroundFrameKeys { get { return _backgroundFrameKeys; } set { _backgroundFrameKeys = value; AcquireBrushes(); } }

            private PointF _rotateOrigin= PointF.Empty;
            public PointF RotateOrigin { get { return _rotateOrigin; } set { _rotateOrigin = value; AcquireBrushes(); } }

                private bool _imagesacquired=false;
        private Image[] _acquiredImages=null;

        public BackgroundColourImageDrawer(Color backgroundColour)
        {
            
            _backgroundColour = backgroundColour;
            AcquireBrushes();

        }
        public BackgroundColourImageDrawer(String backgroundKey):this(new string[] {backgroundKey})
        {
            

        }
        public BackgroundColourImageDrawer(String[] backgroundKey)
        {
            _backgroundPicKey = backgroundKey;
            
            BackgroundFrameKeys = backgroundKey;


            
        }
        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            BackgroundFrameKeys = _backgroundFrameKeys;
        }

        #endregion
        public BackgroundColourImageDrawer(SerializationInfo info, StreamingContext context)
        {
            _moveVelocity = (PointF)info.GetValue("MoveVelocity", typeof(PointF));
            _backgroundFrameKeys = (String[])info.GetValue("Backgroundframekeys", typeof(String[]));
            _rotateSpeed = info.GetSingle("rotatespeed");
            _currentRotation = info.GetSingle("currentrotation");
            _currentOffset = (PointF)info.GetValue("CurrentOffset",typeof(PointF));
            _rotateOrigin = (PointF)info.GetValue("RotateOrigin", typeof(PointF));

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("MoveVelocity", MoveVelocity);
            info.AddValue("Backgroundframekeys", BackgroundFrameKeys);
            info.AddValue("rotatespeed", RotateSpeed);
            info.AddValue("currentrotation", CurrentRotation);
            info.AddValue("CurrentOffset", CurrentOffset);
            info.AddValue("RotateOrigin", RotateOrigin);
        }


        public Image[] Backgroundframes {

            get {
                if(BackgroundFrameKeys==null) return null;
                if (_imagesacquired) return _acquiredImages;
                List<Image> buildlist = new List<Image>();

                foreach (string t in BackgroundFrameKeys)
                {
                    Image[] gotimages = BCBlockGameState.Imageman.getImageFrames(t);
                    if (gotimages.Length == 0)
                    {
                        Image grabimage = BCBlockGameState.Imageman.getLoadedImage(t);
                        buildlist.Add(grabimage);

                    }
                    buildlist.AddRange(gotimages);
                }
                _imagesacquired=true;
                _acquiredImages = buildlist.ToArray();



                return _acquiredImages;

            }


            
        }

            
            private readonly Stopwatch _callTimer = new Stopwatch();
        private const int AnimationDelayTime=500;
        private int _currentFrame = 0;

        private String[] _backgroundPicKey;


    
        public BackgroundColourImageDrawer()
        {
            _callTimer.Start();
            /*
            LevelDraw=leveldraw;

            if ((BackgroundPicKey!=null) && BackgroundPicKey.Length )
            {
                usingimage=true;
                Backgroundframekeys = new string[] { LevelDraw.BackgroundPicKey };
               
            }
            DrawAttributes.SetColorMatrix(ColorMatrices.GetColourizer(Color.Blue));
            */
        }
        public ImageAttributes DrawAttributes = new ImageAttributes();
        private Brush _useBrush;
        private bool _mustDrawManual=false;
        private void AcquireBrushes()
        {

            if (!BackgroundImage)
            {
                _useBrush = new SolidBrush(BackgroundColour);
            }
            else
            {
                try
                {
                    //RectangleF framerect = new RectangleF(0, 0, Backgroundframes[CurrentFrame].Width, Backgroundframes[CurrentFrame].Height);
                    TextureBrush tbrush = new TextureBrush(Backgroundframes[_currentFrame]);

                    Debug.Print("issueing translatetransform" + CurrentOffset.ToString());
                    if (CurrentRotation != 0)
                    {

                        tbrush.RotateTransform((float)CurrentRotation, MatrixOrder.Append);

                    }



                    tbrush.TranslateTransform(CurrentOffset.X, CurrentOffset.Y);

                    _useBrush = tbrush;
                }
                catch (OutOfMemoryException em)
                {
                    //texturebrush error
                    _mustDrawManual=true;
                    _useBrush=null;
                    
                }
            }

        }

        public override void DrawBackground(BCBlockGameState gamestate,Graphics g,Rectangle fullarea, bool pforce)
        {
            if(_useBrush!=null)
                g.FillRectangle(_useBrush, fullarea);
            else if(_mustDrawManual)
            {
                //draw manually
                g.DrawImage(Backgroundframes[_currentFrame], 0, 0, Backgroundframes[_currentFrame].Width, Backgroundframes[_currentFrame].Height);

                

            }
            
        }

        public override bool RequiresPerformFrame()
        {
            if(Backgroundframes==null) return false;
            return Backgroundframes.Length > 1 || (MoveVelocity.X!=0 || MoveVelocity.Y!=0);

            
        }
        private void IncrementFrame()
        {
            _currentFrame++;
            if (_currentFrame > Backgroundframes.Length)
                _currentFrame = 0;

       


        }

        public override void PerformFrame(BCBlockGameState gamestate)
        {
            if (_callTimer.ElapsedMilliseconds > AnimationDelayTime)
            {
                IncrementFrame();


            }
            Image currimage = Backgroundframes[_currentFrame];
            CurrentOffset = new PointF(CurrentOffset.X + MoveVelocity.X, CurrentOffset.Y + MoveVelocity.Y);
            CurrentOffset = new PointF(CurrentOffset.X % currimage.Width, CurrentOffset.Y % currimage.Height);
            CurrentRotation += RotateSpeed;
            _callTimer.Restart();
            AcquireBrushes();
        }



       
    }

    

}
