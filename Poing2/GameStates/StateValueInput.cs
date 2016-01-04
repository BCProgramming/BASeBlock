using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BASeCamp.BASeBlock.GameStates
{
    public delegate void ValueInputProcedure(ref String itementered);
    /// <summary>
    /// Routine called to override default paint when acquiring a string from the user.
    /// </summary>
    /// <param name="g">graphics object.</param>
    /// <param name="inputdata">current "inputdata" class reference.</param>
    /// <param name="CurrentText">currently entered text.</param>
    public delegate void ValuePaintProcedure(Graphics g, ValueInputData inputdata, ValueInputTextData CurrentTextData);
    /// <summary>
        /// called for each iteration during the gameproc while the ValueInput "dialog" is open.
        /// </summary>
        public delegate void ValueGameProcRoutine();
    public class StateValueInput:GameState
    {

        public Color TitleColour { get; set; }
        private ValueInputTextData _currentValue;
        private ValueInputData _InputData = null;
        public ValueInputData InputData { get { return _InputData; } set { _InputData = value; } }
        public ValueInputTextData CurrentValue { get { return _currentValue; } set{ _currentValue = value; }}
       

        public StateValueInput(ValueInputData InputData,ValueInputTextData InitText)
        {
            _InputData = InputData;
            _currentValue = InitText;
        }

        public override IGameState Run(BCBlockGameState GameInfo)
        {
            Application.DoEvents();
            Thread.Sleep(100);
            if (InputData.GameProcRoutine != null)
            {

                InputData.GameProcRoutine();

            }
            GameInfo.ClientObject.RefreshDisplay();

            //throw new NotImplementedException();
            return null;
        }
    
        private void BaseInputDraw(BCBlockGameState state,Graphics g)
        {
            state.ClientObject.DrawShade(g, Color.FromArgb(75, Color.DarkSlateGray));

            String measureme = "################################";
            //draw a white box near the center...
            //RectangleF Inputboxcoordsf = new RectangleF(PicGame.ClientRectangle.Width/4,PicGame.ClientRectangle.Height/4,PicGame.ClientRectangle.Width/2,PicGame.ClientRectangle.Height/2);
            Font cheatinputfont = BCBlockGameState.GetScaledFont(new Font(BCBlockGameState.GetMonospaceFont(), 12), 32);
            SizeF InputboxSize;
            if (CurrentValue.Text.Length > 0)
                InputboxSize = BCBlockGameState.MeasureString(measureme, cheatinputfont);
            else
                InputboxSize = BCBlockGameState.MeasureString(measureme, cheatinputfont);


            Rectangle cli = state.GameArea ;
            RectangleF IBox = new RectangleF(new PointF((cli.Width / 2f) - InputboxSize.Width / 2, (cli.Height / 2f) - (InputboxSize.Height / 2)), InputboxSize);

            RectangleF titleBox = new RectangleF(IBox.Location, IBox.Size);
            titleBox.Offset(0, -IBox.Height);

            Brush useTitleBrush = new LinearGradientBrush(titleBox, TitleColour, TitleColour.Darken(64), LinearGradientMode.Horizontal);


            g.FillRectangle(useTitleBrush, titleBox);
            //g.DrawString("Enter Cheat Code:", new Font(BCBlockGameState.GetMonospaceFont(), 14), new SolidBrush(Color.White), titleBox, new StringFormat() { Alignment = StringAlignment.Center });
            g.DrawString(InputData.TitleString, BCBlockGameState.GetScaledFont(new Font(BCBlockGameState.GetMonospaceFont(), 14), 20), new SolidBrush(Color.White), titleBox, new StringFormat() { Alignment = StringAlignment.Center });
            g.FillRectangle(new SolidBrush(Color.FromArgb(230, Color.White)), IBox);

            //base string to draw on currentcheatcharpos.
            //if the length of the string is longer then the length of the box...
            if (g.MeasureString(CurrentValue.Text, cheatinputfont).Width > InputboxSize.Width)
            {
                //the length of the string to draw is longer then the length of the box to draw in.
                //not sure what to do here yet....



            }
            Font usethisfont = new Font(BCBlockGameState.GetMonospaceFont(), 12);
            String CursorChar = DateTime.Now.Millisecond < 500 ? "_" : " ";
            //Region[] charranges = g.MeasureCharacterRanges(VInput.Text, usethisfont, IBox, StringFormat.GenericDefault);
            if (CurrentValue.SelStart > CurrentValue.Text.Length) 
                CurrentValue.SelStart = CurrentValue.Text.Length;
            if (CurrentValue.SelStart < 0) CurrentValue.SelStart = 0;
            String DrawThisText = CurrentValue.Text.Substring(0, CurrentValue.SelStart) + CursorChar + CurrentValue.Text.Substring(CurrentValue.SelStart);
            //Debug.Print("drawing the string..." + VInput.Text);
            g.DrawString(DrawThisText, usethisfont, new SolidBrush(Color.Black), IBox);

            if (state.PlayerPaddle != null)
                state.PlayerPaddle.Draw(g);
        }
        public override void DrawFrame(BCBlockGameState GameInfo, Graphics g, Size AreaSize)
        {
            if (InputData.PaintRoutine == null)
                BaseInputDraw(GameInfo,g);
            else
            {

                if (InputData.PaintType == ValueInputPaintTypeConstants.paint_Pre)
                {
                    InputData.PaintRoutine(g, InputData, CurrentValue);
                    BaseInputDraw(GameInfo,g);
                }
                else if (InputData.PaintType == ValueInputPaintTypeConstants.paint_Override)
                {

                    InputData.PaintRoutine(g, InputData, CurrentValue);
                }
                else if (InputData.PaintType == ValueInputPaintTypeConstants.paint_Post)
                {
                    BaseInputDraw(GameInfo,g);
                    InputData.PaintRoutine(g, InputData, CurrentValue);


                }
            }  
        }
        public override bool IsLoopingState
        {
            get
            {
                return true;
            }
        }
    }

    public enum ValueInputPaintTypeConstants
    {
        paint_Pre, //call the routine before performing the default drawing stuff.
        paint_Override, //call the routine, but perform no default behaviour.
        paint_Post //call the routine after performing default drawing.


    }
    public class ValueInputTextData
    {
        public String Text;
        public int SelStart;
    }
    public class ValueInputData
    {
        //private ValueInputProcedure CompletionRoutine;
        //private String TitleString;
        public ValueInputProcedure CompletionRoutine { get; set; }

        public String TitleString { get; set; }
        public ValuePaintProcedure PaintRoutine { get; set; }
        public ValueInputPaintTypeConstants PaintType { get; set; }
        public ValueGameProcRoutine GameProcRoutine { get; set; }
        public ValueInputData(ValueInputProcedure pCompletionRoutine, String pTitleString, ValuePaintProcedure pPaintRoutine, ValueInputPaintTypeConstants pPaintType, ValueGameProcRoutine pGameProc)
        {
            CompletionRoutine = pCompletionRoutine;
            TitleString = pTitleString;
            PaintRoutine = pPaintRoutine;
            PaintType = pPaintType;
            GameProcRoutine = pGameProc;
        }


    }
}
