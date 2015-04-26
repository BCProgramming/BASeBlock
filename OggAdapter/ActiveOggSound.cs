using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using BASeBlock;

namespace OggAdapter
{
    class ActiveOggSound:iActiveSoundObject 
    {
        SoundPlayer sp = null;
        internal ActiveOggSound(OggDecodeStream sourceOgg)
        {
            sp = new SoundPlayer(sourceOgg);


        }

        public bool Finished { get; private set; }
        public float Tempo { get; set; }
        public void Stop()
        {
            sp.Stop();
        }

        public void Pause()
        {
            sp.Stop();
        }

        public void UnPause()
        {
            if(!sp.
        }

        public bool Paused { get; set; }
        public void setVolume(float volumeset)
        {
            throw new NotImplementedException();
        }

        public float Progress { get; private set; }
        public iSoundSourceObject Source { get; private set; }
    }
}
