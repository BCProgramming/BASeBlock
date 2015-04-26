using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using BASeBlock;
namespace OggAdapter
{
    class OggAdapterSound : iSoundSourceObject
    {

        private Stream OggFileStream = null;
        OggDecodeStream oggstream = null;
        public OggAdapterSound(String sSource)
        {
            OggFileStream = new FileStream(sSource,FileMode.Open,FileAccess.Read);
            oggstream = new OggDecodeStream(OggFileStream);
            SoundPlayer sp = new SoundPlayer(oggstream);
            
        }


        public iActiveSoundObject Play(bool playlooped)
        {
            throw new NotImplementedException();
        }

        public iActiveSoundObject Play(bool playlooped, float volume)
        {
            throw new NotImplementedException();
        }

        public float getLength()
        {
            
        }
    }
}
