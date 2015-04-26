using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IronPython.Hosting;

namespace BASeBlock
{

    

    /// <summary>
    /// IronPython/Python script manager
    /// </summary>
    public class IPScriptManager
    {
        IronPython.Hosting.PythonEngine pyengine = new PythonEngine();

        public IPScriptManager(String[] ScriptFolders)
            : this(ScriptFolders, new Nullcallback())
        {


        }

        public IPScriptManager(String[] ScriptFolders, iManagerCallback datahook)
        {
            
            foreach (String loopfolder in ScriptFolders)
            {
                if(Directory.Exists(loopfolder))
                {
                    datahook.ShowMessage("IPScriptManager:Enumerating contents of " + loopfolder);


                }
                else
                {
                    datahook.ShowMessage("IPScriptManager:Folder " + loopfolder + " not found.");
                }


            }

        }

    }
}
