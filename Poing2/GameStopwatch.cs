using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BASeCamp.BASeBlock
{
    //works with BCBlockGameState (both static and instance) to provide functionality that allows a 
    //piece of code to deal with time delays using game time, instead of real time.
    public class GameStopwatch
    {
        private BCBlockGameState _stateobject = null;
        TimeSpan StartingLevelTime;
        public GameStopwatch(BCBlockGameState stateobject)
        {
            _stateobject = stateobject;
            Reset();
        }
        public void Reset()
        {
            StartingLevelTime = _stateobject.ClientObject.GetLevelTime();

        }
        public TimeSpan Elapsed { get { return _stateobject.ClientObject.GetLevelTime() - StartingLevelTime; } }


    }
}
