using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BASeBlock
{

    public interface SoundObject
    {

        void Play();
        void Stop();


    }
    public interface iPlaybackEngine
    {
        //implemented by "sound engines"... for example, the irrKlang sound lib will have it's own "engine", as well
        //fmod, and so forth.



    }
    /// <summary>
    /// A more generic version of my other SoundManager, since this one actually abstracts away both the sound objects themselves
    /// as well as how they are played.
    /// </summary>
    class GenericSoundManager
    {


    }
}
