using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using BASeBlock; 
namespace XNAAdapter
{

    //attempts to implement a Sound Adapter Driver for the XNA framework to BASeBlock


    public class XNASound:iSoundSourceObject,iActiveSoundObject
{

}
    public class XNASoundAdapter :iSoundEngineDriver 
    {

        private Microsoft.Xna.Framework.Audio.AudioEngine ae;


        public XNASoundAdapter()
        {
            
            

        }
        public XNASoundAdapter(String plugs):this()
        {


        }



    }
}
