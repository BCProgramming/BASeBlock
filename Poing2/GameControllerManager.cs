using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BASeBlock
{
    class GameControllerManager
    {

        private Dictionary<String, iGameInput> InputPlugins = new Dictionary<string, iGameInput>(); 


        public GameControllerManager(IEnumerable<Type> GameInputTypes,iGameClient ClientObject)
        {
            //iterate through each type, confirm it's a iGameInput, instantiate it, and call Initialize.
            foreach (var iterateinput in GameInputTypes)
            {

                iGameInput currentplugin = iterateinput as iGameInput;
                if (currentplugin != null)
                {
                    currentplugin.Initialize(ClientObject);
                    String gotname = currentplugin.Name;

                    InputPlugins.Add(gotname, currentplugin);


                }



            }



        }



    }
}
