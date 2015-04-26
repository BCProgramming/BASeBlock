using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BASeBlock
{
    
    /// <summary>
    /// stores both the time index, message, and  display attributes
    /// of a message that will be shown. These are stored per-level.
    /// </summary>
    [Serializable]
    public class PlayMessageData:ISerializable,IComparable<PlayMessageData>,IComparable<float>,ICloneable
    {
        public enum MessageDataStartPositions
        {
            [Description("Message will scroll from the bottom upwards.")]
            MDSP_Bottom=0,
            [Description("Message will scroll from the Left.")]
            MDSP_Left=1,
            [Description("Message will scroll from the Top.")]
            MDSP_Top=2,
            [Description("Message will scroll from the Right.")]
            MDSP_Right=3



        }

        public enum MessageImagePlacementConstants
        {
            MIP_Left=0,
            MIP_Top=1,
            MIP_Right=2,
            MIP_Bottom=3



        }

        private int? _TriggerID=null;
        private float _TimeIndex; //in seconds, for  simplicity of creation.
        protected int _TotalMS;
        public int TotalMS { get { return _TotalMS; } set { _TotalMS = value; } }
            private String _Message;
        private MessageDataStartPositions _StartPosition = MessageDataStartPositions.MDSP_Bottom;
        private Font _MessageFont=new Font("Arial",24);
        private Color _StrokeColor=Color.White;
        private Color _FillColor = Color.Black;
        private MessageImagePlacementConstants _MessageImagePlacement;
        private String _MessageImageKey = null; //null for no image.
        private Bitmap _SavedMessageBitmap = null; //we save this when.. we are saved... (GetObjectData)
        public MessageImagePlacementConstants MessageImagePlacement
        {
            get { return _MessageImagePlacement; }
            set { _MessageImagePlacement = value; }
        }

        public String MessageImageKey
        {
            get { return _MessageImageKey; }
            set { _MessageImageKey = value; }
        }

        public Font MessageFont { get { return _MessageFont; } set { _MessageFont = value; } }
        public Color StrokeColor { get { return _StrokeColor; } set { _StrokeColor = value; } }
        public Color FillColor { get { return _FillColor; } set { _FillColor = value; } }
        public int? TriggerID { get { return _TriggerID; } set { _TriggerID = value; } }
            public float TimeIndex { get { return _TimeIndex; } set { _TimeIndex = value;


        _TotalMS = calcMS();
        
        } }
        public String Message { get { return _Message; } set { _Message = value; } }

        public Object Clone()
        {

            return new PlayMessageData(this);
        }
        
        public MessageDataStartPositions StartPosition { get { return _StartPosition; } set { _StartPosition = value; } }

            private int calcMS()
        {
            //1000 ms to a second...
            return (int)(_TimeIndex * 1000);
            //float floored = (int)(Math.Floor(_TimeIndex));
            //float fractional = (float)(_TimeIndex-floored);
            //return (int)((fractional * 1000) + floored);



        }
            public PlayMessageData()
            {


            }
            public PlayMessageData(PlayMessageData clonethis)
            {
                
                _StartPosition = clonethis.StartPosition;
                _StrokeColor = clonethis.StrokeColor;
                _FillColor = clonethis.FillColor;
                _MessageFont = clonethis.MessageFont;
                _Message = clonethis.Message;
                TimeIndex = clonethis.TimeIndex;
                _MessageImagePlacement = clonethis.MessageImagePlacement;
                _MessageImageKey = clonethis.MessageImageKey;

            }
            public PlayMessageData(int TriggerID)
            {
                _TriggerID=TriggerID;

            }

        public PlayMessageData(float pTimeIndex, String pMessage)
        {
            _TimeIndex = pTimeIndex;
            Message=pMessage;

        }
        private Image GetMessageImage()
        {
            return BCBlockGameState.Imageman.getLoadedImage(_MessageImageKey);


        }

        private Bitmap DrawMessage()
        {
            //draw this Message to a bitmap, and return that bitmap.
            //Bitmap sizingbitmap = new Bitmap(1, 1);
            //Graphics sizeg = Graphics.FromImage(sizingbitmap);
            //SizeF TextSize = sizeg.MeasureString(_Message, _MessageFont);
            SizeF TextSize = BCBlockGameState.MeasureString(_Message, _MessageFont);
            //inflate the size as needed for the messagebitmap.
            float inflateX = 0, inflateY = 0;
            PointF TextOrigin, ImageOrigin;
            Image messimage = GetMessageImage();
            if (messimage != null)
            {
                TextSize = new SizeF(Math.Max(TextSize.Width, messimage.Width), Math.Max(TextSize.Height, messimage.Height));


                switch (MessageImagePlacement)
                {
                    case MessageImagePlacementConstants.MIP_Left:
                        inflateX = messimage.Width;
                        break;
                    case MessageImagePlacementConstants.MIP_Right:
                        inflateX = messimage.Width;
                        break;
                    case MessageImagePlacementConstants.MIP_Top:
                        inflateY = messimage.Height;
                        break;
                    case MessageImagePlacementConstants.MIP_Bottom:
                        inflateY = messimage.Height;
                        break;

                }

            }


            return null;

        }

        public void GetObjectData(SerializationInfo info,StreamingContext context)
        {
            
            info.AddValue("TimeIndex", _TimeIndex);
            info.AddValue("Message", _Message);
            info.AddValue("Font", _MessageFont);
            info.AddValue("Stroke", _StrokeColor);
            info.AddValue("Fill", _FillColor);
            info.AddValue("StartPosition", _StartPosition);
            info.AddValue("TriggerID", _TriggerID);
            info.AddValue("MessageImageKey", _MessageImageKey);
            info.AddValue("MessageImagePlacement", _MessageImagePlacement);
        }

        public PlayMessageData(SerializationInfo info, StreamingContext context)
        {

            _Message = info.GetString("Message");
            _MessageFont = (Font) info.GetValue("Font", typeof (Font));
            _StrokeColor = (Color) info.GetValue("Stroke", typeof (Color));
            _FillColor = (Color) info.GetValue("Fill", typeof (Color));
            TimeIndex = info.GetSingle("TimeIndex");
            try{_StartPosition =(MessageDataStartPositions) info.GetValue("StartPosition", typeof (MessageDataStartPositions));}catch{}
            try{_TriggerID = (int?) info.GetValue("TriggerID", typeof (int?));}catch{}
            try { _MessageImageKey = info.GetString("MessageImageKey"); }catch{_MessageImageKey = null;}
            try
            {
                _MessageImagePlacement = (MessageImagePlacementConstants)info.GetValue("MessageImagePlacement", typeof(MessageImagePlacementConstants));
            }
            catch
            {
                _MessageImagePlacement = MessageImagePlacementConstants.MIP_Left;
            }

        }

        /// <summary>
        /// creates a GameObject (text) that will show  this message.
        /// </summary>
        /// <param name="tostate"></param>
        public virtual void Invoke(BCBlockGameState tostate)
        {
            if (_TriggerID != null)
            {

                tostate.TriggerEvent(_TriggerID.Value, tostate);

                return;
            }
            PointF startingPos=PointF.Empty;
            PointF useSpeed = PointF.Empty;
            Rectangle ga = tostate.GameArea;
            SizeF messagesize = BCBlockGameState.MeasureString(_Message, _MessageFont);
            switch (_StartPosition)
            {
            case MessageDataStartPositions.MDSP_Bottom:
                    startingPos = new PointF((float)ga.Width/2,ga.Height);
                    useSpeed = new PointF(0,-1);
            break;
            case MessageDataStartPositions.MDSP_Left:
                    startingPos = new PointF(-messagesize.Width/2,(float)ga.Height/2);
                    useSpeed = new PointF(1,0);
            break;
            case MessageDataStartPositions.MDSP_Right:
                    startingPos = new PointF(messagesize.Width/2 + ga.Width,(float)ga.Height/2);
                    useSpeed = new PointF(-1,0);
            break;
            case MessageDataStartPositions.MDSP_Top:
                    startingPos = new PointF((float)ga.Width/2,-messagesize.Height/2);
                    useSpeed = new PointF(0,1);
            break;


            }
            Pen TextPen = new Pen(_StrokeColor);
            Brush TextBrush = new SolidBrush(_FillColor);

            BasicFadingText bft = new BasicFadingText(_Message, startingPos, useSpeed, _MessageFont, TextPen, TextBrush, 750);


            tostate.GameObjects.AddLast(bft);


        }

        #region IComparable<PlayMessageData> Members

        public int CompareTo(PlayMessageData other)
        {
            return _TimeIndex.CompareTo(other.TimeIndex);
        }

        #endregion

        #region IComparable<float> Members

        public int CompareTo(float other)
        {
            return _TimeIndex.CompareTo(other);
        }

        #endregion
    }
}
