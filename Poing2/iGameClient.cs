using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BASeCamp.BASeBlock.Blocks;
using BASeCamp.BASeBlock.Events;
using BASeCamp.BASeBlock.GameStates;

namespace BASeCamp.BASeBlock
{    [Flags]
    public enum ButtonConstants
    {
        Button_None=0,
        Button_Left = 1,
        Button_Right = 2,
        Button_Up = 4,
        Button_Down = 8,
        Button_Shift = 16,
        Button_A = 32,
        Button_B = 64,
        Button_C = 128,
        Button_D = 256,
        Button_E = 512,   //maps to "W" key (up) 
        Button_F = 1024,   //maps to "A" key (Left)
        Button_G = 2048,  //maps to "S" key (Down)
        Button_H=4096,    //maps to "D" key (Right)
        Button_I=8192,
        Button_J=16384,
        Button_K=32768
    }
   
/*public enum GameRunStateConstants
{
    Game_NotRunning,  //the gamethread is null or not running.
    Game_Running,
    Game_Paused,      //GameThread is paused (not really, this is "emulated" via a Application.DoEvents() in the actual thread.
    Game_LevelIntro,
    Game_LevelOutro, //bonus tally screen.. or something.
    Game_LevelOutro_GameOver, //Level outro followed by gameover. (rather then playlevel); note this plays gameovermusic rather then tallymusic.
    Game_ValueInput, // balls,blocks, and particles are "paused" and are not being drawn or having their frame events performed.
    //the drawing code is instead drawing a alpha colour over top of the play area and displaying the contents if ValueInputText (and a prompt)
    Game_Menu,  //as of Fri Aug 5th 2011, this isn't used. Will be used in a manner similar to ValueInput, but showing a menu instead...
    //Data will be stored in a private MenuScreenData structure, defined in frmBaseBlock
    Game_Death, //the paddle and balls are "exploding"... this lasts about 2 seconds.
    Game_Loading //level loading.

}*/




    /// <summary>
    /// interface to be implemented by various classes to accept input from various sources.
    /// </summary>
public interface iGameInput
{
    /// <summary>
    /// initialize this Input device to use the given GameClient; input should be
    /// sent to that implementation (button down and button up and so forth)
    /// </summary>
    /// <param name="gamecli"></param>
    void Initialize(iGameClient gamecli);
    /// <summary>
    /// called to determine if this implementation has changable settings.
    /// </summary>
    /// <returns></returns>
    bool hasConfig();

    void Configure();
    String Name { get; }

}

     
    public interface iGameClient
    {
        //iGameClient; basically, implemented by the main form to allow for gameparticles and whatnot to acquire references to relevant
        //form variables, namely, the background and "playing field" bitmaps.

        IGameState ActiveState { get; set; }
        bool DemoMode { get; }
        bool Dorefreshstats { get; set; }

        
        Bitmap getBackgroundBitmap();
        //Color GetCanvasPixel(int x, int y);
        Level getcurrentLevel();
        LevelSet GetPlayingSet();
        SizeF MeasureString(String stringmeasure, Font fontuse);
        IntPtr Handle { get; }
        //various input events...
        //event Func<ButtonConstants, bool> ButtonDown;
        event EventHandler<ButtonEventArgs<bool>> ButtonDown;
        event EventHandler<ButtonEventArgs<bool>> ButtonUp;
        event EventHandler<MouseEventArgs<bool>> OnMove; 
        event Func<PointF, bool> MoveAbsolute;
        float CurrentFPS { get; set; }
        TimeSpan GetLevelTime();
        void UpdateBlocks();
        void DelayInvoke(TimeSpan Delaytime, BCBlockGameState.DelayedInvokeRoutine routinefunc, object[] parameters);
        void InvokeButtonDown(ButtonEventArgs<bool> Callwith);
        void InvokeButtonUp(ButtonEventArgs<bool> Callwith);
        void InvokeMoveAbsolute(PointF newlocation);
        ButtonConstants getPressedButtons();
        /// <summary>
        /// force the player to die.
        /// </summary>
        void ForceDeath();
        Object Invoke(Delegate routine);

        void RefreshDisplay();
        void DrawShade(Graphics g, Color usecolor);
    }
}
