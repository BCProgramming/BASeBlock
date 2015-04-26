using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BASeBlock;

namespace OggAdapter
{
    public class OggAdapterClass : BASeBlock.iSoundEngineDriver 
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public event OnSoundStopDelegate OnSoundStop;
        public event OnSoundPlayDelegate OnSoundPlay;
        public iSoundSourceObject loadSound(byte[] data, string sName, string fileextension)
        {
            throw new NotImplementedException();
        }

        public iSoundSourceObject loadSound(string filename)
        {
            throw new NotImplementedException();
        }

        public string Name { get; private set; }
        public IEnumerable<string> GetSupportedExtensions()
        {
            throw new NotImplementedException();
        }
    }
}
