using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using IronPython.Hosting;

namespace BASeBlock
{
    [BBEditorInvisible]
    class PythonBlock : Block 
    {
        public static PythonEngine pyengine= new PythonEngine();
        private EngineModule pythonmodule;
        public PythonBlock(RectangleF pblockrectangle,String PythonCode)
            : base()
        {
            BlockRectangle = pblockrectangle;
            //create a new module for this Block...
            

        }

        public override object Clone()
        {
            
        }
    }
}
