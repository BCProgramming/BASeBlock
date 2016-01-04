using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BASeCamp.BASeBlock.Blocks;

namespace BASeCamp.BASeBlock.Events
{
    //Func<ButtonConstants, bool>
    public class ButtonEventArgs<TResult> : EventArgs
    {
        private TResult _Result;
        private ButtonConstants _Button;

        public TResult Result { get { return _Result; } set { _Result = value; } }
        public ButtonConstants Button { get { return _Button; } set { _Button = value; } }

        public ButtonEventArgs(ButtonConstants pButton)
        {
            _Button = pButton;

        }



    }
    
    public class BlockEventArgs<TResult> : EventArgs
    {
        private TResult _Result;
        private Block _TheBlock;
        private BCBlockGameState _gameState;
        public TResult Result { get { return _Result; } set { _Result = value; } }
        public Block TheBlock { get { return _TheBlock; } set { _TheBlock = value; } }
        public BCBlockGameState GameState { get { return _gameState; } set { _gameState = value; } }
        public BlockEventArgs(BCBlockGameState gstate,Block theblock)
        {
            _TheBlock = theblock;
            _gameState = gstate;

        }
    }
    public class BlockHitEventArgs<TResult> : BlockEventArgs<TResult>
    {
        private cBall _Ball;
        public cBall Ball { get { return _Ball; } set { _Ball = value; } }
        public BlockHitEventArgs(BCBlockGameState gstate,Block theBlock,cBall pBall):base(gstate,theBlock)
        {
            _Ball = pBall;

        }


    }
    //public event Action<TriggerEvent,BCBlockGameState> EventInvoked;
    public class TriggerEventArgs : EventArgs
    {
        private TriggerEvent _TheTrigger;
        private BCBlockGameState _GameState;
        public TriggerEvent TheTrigger { get { return _TheTrigger; } set { _TheTrigger = value; } }
        public BCBlockGameState GameState { get { return _GameState; } set { _GameState = value; } }
        public TriggerEventArgs(BCBlockGameState gstate, TriggerEvent TheTrigger)
        {
            _GameState = gstate;
            _TheTrigger = TheTrigger;

        }
    }
    public class EnemyStateChangeEventArgs : EventArgs
    {
        private String _PreviousState;
        private String _NewState;
        public String PreviousState { get { return _PreviousState; }  }
        public String NewState { get { return _NewState; } set { _NewState = value; } }
        public EnemyStateChangeEventArgs(String pPrevious,String pNew)
        {
            _PreviousState = pPrevious;
            _NewState=pNew;
        
        }

    }
    public class EnemyDeathEventArgs:EventArgs 
    {
        //GameEnemy enemydied,BCBlockGameState stateobject
        private GameEnemy _enemydied = null;
        private BCBlockGameState _stateobject = null;
        public GameEnemy EnemyDied { get { return _enemydied; } set { _enemydied = value; } }
        public BCBlockGameState StateObject { get { return _stateobject; } set { _stateobject = value; } }

        public EnemyDeathEventArgs(GameEnemy penemy, BCBlockGameState gstate)
        {
            _enemydied = penemy;
            _stateobject = gstate;


        }
    }
}
