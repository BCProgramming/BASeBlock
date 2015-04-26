using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
//using IronPython.Hosting;
//using IronPython.Runtime;
//using Microsoft.Scripting;
//using Microsoft.Scripting.Hosting;
//using Mono.CSharp;
namespace BASeBlock.Scripting
{



    public class ScriptObject
    {
        public String Name { get; set; }
        public String Language { get; set; }
        public String Code { get; set; }



    }

    
    public abstract class BBScriptEngine
    {
        /// <summary>
        /// the file  extensions applicable for  this "engine"..
        /// </summary>
        /// <returns></returns>
        public abstract String[] GetScriptExtensions();
        /// <summary>
        /// Loads a script file into this 'Engine'
        /// return true for success, false otherwise.
        /// </summary>
        /// <param name="ScriptCode"></param>
        public abstract bool LoadScript(String ScriptCode);
        public abstract void AddObject(String Name, Object Value);
        public abstract T Evaluate<T>(String ExpressionRun,IDictionary<String,Object> localsuse);
        public Object Evaluate(String ExpressionRun)
        {
            return Evaluate<Object>(ExpressionRun,null);
            
        }

    }
    /*
    public class PythonBBEngine : BBScriptEngine
    {
        PythonEngine pe;
        private  IronPython.Hosting.CompiledCode ourcode=null;
        private ScriptCode scode =null;
        public static String[] PythonExtensions = new string[] { "py" };
        EngineModule usemodule=null;
        public PythonBBEngine()
        {

            EngineOptions eo = new EngineOptions();
            pe = new PythonEngine(eo);
            usemodule = pe.CreateModule("BASeBlockPy", true);
        }

        public override string[] GetScriptExtensions()
        {
            return PythonExtensions;
//            throw new NotImplementedException();
        }

        public override bool LoadScript(string ScriptCode)
        {
  //          throw new NotImplementedException();
             
            return false;
            
            

        }
        public override void AddObject(string Name, object Value)
        {
            if (usemodule.Globals.ContainsKey(Name))
            {
                usemodule.Globals[Name] = Value;

            }
            else
            {


                usemodule.Globals.Add(Name, Value);
            }
        }
        public override T Evaluate<T>(string ExpressionRun,IDictionary<String,Object> localsuse)
        {
            T returnvalue;
            if (localsuse != null)
                returnvalue = pe.EvaluateAs<T>(ExpressionRun, usemodule, localsuse);
            else
                returnvalue = pe.EvaluateAs<T>(ExpressionRun, usemodule);
                
            

            Debug.Print("Executed Python code:" + ExpressionRun + " result was of type " + returnvalue.GetType().Name);
            return returnvalue;
            //throw new NotImplementedException();
        }
    }

    */

}
