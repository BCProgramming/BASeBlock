using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Microsoft.JScript;
using Microsoft.VisualBasic;

namespace BASeBlock
{
    /// <summary>
    /// Does all the work to compile scripts and script groups into assemblies, which are then used by the BCBlockGameState.initGameState() and passed on to LoadedTypeManager.
    /// 
    /// </summary>
    class BBScript
    {
        public class AggregateScriptFailureException : Exception
        {
            public IEnumerable<ScriptCompileFailureException> CompileIssues;
            private int _issueCount = 0;
            public AggregateScriptFailureException(IEnumerable<ScriptCompileFailureException> issues)
            {
                CompileIssues = issues;
                _issueCount = CompileIssues.Count();
            }
            public override string ToString()
            {
                return CompileIssues.Count().ToString() + " Issues compiling Scripts:\n" + from p in CompileIssues select p.ToString() + "\n";
                
            }


        }
        public class ScriptCompileFailureException :Exception 
        {
            private readonly IEnumerable<CompilerError> _errors;
            private readonly IEnumerable<CompilerError> _warnings;
            private readonly int _warningsCount;
            private readonly int _errorsCount;
            private readonly String _sourceFile = "";
            public ScriptCompileFailureException(String pSourceFile, IEnumerable<CompilerError> pErrors, IEnumerable<CompilerError> pWarnings)
                :base()
            {
                _errors = pErrors;
                _warnings = pWarnings;
                _sourceFile = pSourceFile;
                _warningsCount = _warnings.Count();
                _errorsCount = _errors.Count();
                
            }
            public override string ToString()
            {
                StringWriter buildresult = new StringWriter();
                
                buildresult.WriteLine("Compilation of script, \"" + _sourceFile + " resulted in " + _warningsCount + " Warnings & " + _errorsCount + " Errors.");

                if (_warningsCount > 0)
                {
                    buildresult.WriteLine("Warnings:");
                    foreach (CompilerError warning in _warnings)
                    {
                        String linewrite = warning.ErrorText + " #" + warning.ErrorNumber + " Line " + warning.Line + " Column " + warning.Column + " in file: " + warning.FileName;
                        Debug.Print(linewrite);
                        buildresult.WriteLine(linewrite);

                    }
                }
                if (_errorsCount > 0)
                {
                    buildresult.WriteLine("Errors:");
                    foreach (CompilerError error in _errors)
                    {
                        String linewrite = error.ErrorText + " #" + error.ErrorNumber + " Line " + error.Line + " Column " + error.Column + " in file: " + error.FileName;
                        Debug.Print(linewrite);
                        buildresult.WriteLine(linewrite);


                    }
                }
                return buildresult.ToString();
            }


        }

        public class ScriptGroupData
        {
            public String Version="0.0.0";
            public String GroupFilename="";
            public String Language="C#"; //currently supported: C# and VB.NET...
            public List<String> AssemblyNames = new List<string>(); //assemblies to include...
            public List<String> ScriptFilenames = new List<string>(); //filenames
            public List<String> AddImages = new List<string>(); //Images to add to the Image manager.
            public List<String> AddSounds = new List<string>(); //sounds to add to the sound manager.

            public StreamWriter LogStream=null; //initialized in constructor, closed when compiled, or in the destructor.
            //used to log stuff like unrecognized lines in the group file, or warnings/errors during compile.
            private void WriteLog(String message)
            {
                if (LogStream != null) LogStream.WriteLine(message);

            }

            private StreamWriter OpenLogFile()
            {
                //if GroupFilename is null, return null...
                if(String.IsNullOrEmpty(GroupFilename)) return null;

                //first get the name of the logfile; groupfilename + ".log"...
                String logfilename = GroupFilename + ".log";
                
                //now, open it for output.
                FileStream openedlogstream = new FileStream(logfilename, FileMode.Create);
                return new StreamWriter(openedlogstream);



            }

            public CompilerResults Compile(iManagerCallback datahook)
            {
                //compile this scriptgroupdata into an assembly. Or, attempt to, anyway.
                WriteLog("Compiling ScriptGroupData.");
                WriteLog("Parameters:");
                WriteLog("Assemblies:" + BCBlockGameState.joinList(AssemblyNames));
                WriteLog("Files:" + BCBlockGameState.joinList(ScriptFilenames));

                CompilerParameters cp = new CompilerParameters(AssemblyNames.ToArray(), "Script_" + Path.GetFileNameWithoutExtension(GroupFilename));
                cp.GenerateInMemory=true;
                cp.GenerateExecutable=false;

                CodeDomProvider cd = GetCodeProviderForLanguage(Language);
                if (cd == null)
                {
                    datahook.ShowMessage("Error getting CodeProvider for Language:" + Language);

                    return null;

                }
                
                CompilerResults result = cd.CompileAssemblyFromFile(cp, ScriptFilenames.ToArray());


                if (result.Errors.HasErrors)
                {
                    Debug.Print("add more code here dammit to enumerate errors/warnings...");


                }
                else
                {

                    datahook.ShowMessage("Adding " + AddImages.Count + " Images & " + AddSounds.Count + " sounds...");
                    foreach (String loopimage in AddImages)
                    {
                        datahook.ShowMessage("Adding Image \"" + loopimage + "\"");
                        BCBlockGameState.Imageman.AddImage(loopimage);


                    }
                    foreach(String loopsound in AddSounds)
                    {

                        datahook.ShowMessage("Adding Sound \"" + loopsound + "\"");
                        BCBlockGameState.Soundman.AddSound(loopsound);
                    }


                }



                return result;

            }
            ~ScriptGroupData()
            {
                LogStream.Flush();
                LogStream.Close();


            }

            public ScriptGroupData(String fromfile)
            {
                //read the given file and initialize our values.
                GroupFilename=fromfile;
                OpenLogFile();
                WriteLog("Opening ScriptGroupData file, " + fromfile + " Date:" + DateTime.Now);
                FileStream readgroupfile = new FileStream(fromfile,FileMode.Open);
                using (StreamReader groupreader = new StreamReader(readgroupfile))
                {

                    //get the folder the script is in. This will be used as the base for relative paths.

                    String scriptGroupFolder = fromfile.Substring(0, fromfile.Length - Path.GetFileName(fromfile).Length);
                    String versionheader = groupreader.ReadLine();
                    if (!versionheader.StartsWith("VERSION=", StringComparison.OrdinalIgnoreCase))
                    {
                        WriteLog("File format Exception: file does not contain valid VERSION= line.");
                        WriteLog("Make sure the first line in your group file is a VERSION= line specifying the earliest version of BASeBlock");
                        WriteLog("it is compatible with.");
                        throw new FormatException("Given file \"" + fromfile + "\" does not contain valid VERSION= line");


                    }
                    else
                    {
                        //otherwise, well... it does, I guess.
                        this.Version = versionheader.Substring(7).Trim(); //cut off the VERSION= part.
                        WriteLog("Group file specifies version as " + Version);
                    }
                    //continue to parse each line in sequence.
                    while (!groupreader.EndOfStream)
                    {
                        String currentline = groupreader.ReadLine();
                        //valid lines:
                        //ASSEMBLY=<assemblyfilename>
                        //LANGUAGE=C#|VB.NET|other item recognized by getCodeProviderForLanguage()
                        //FILE=<Scriptfile> add that script to the compilation set.
                        //SOUND=<Soundfile> add this sound to our list.
                        //IMAGE=<imagefile> add this image to our list.
                        //filenames (scriptfile, assemblyfilename) need to be canonicalized to the location of the script group.
                        if (currentline.StartsWith("ASSEMBLY=", StringComparison.OrdinalIgnoreCase))
                        {
                            
                            String addassemblyfile = currentline.Substring(9);
                            WriteLog("including assembly, \"" + addassemblyfile + "\"");
                            if (addassemblyfile.Contains(Path.PathSeparator))
                            {
                                //only if it contains the pathseparator. this way we don't munge up references to
                                //assemblies in the GAC. (this has the side effect of requiring things like .\assembly.dll if you want to refer to
                                //an assembly in the 'current' (script group file's) directory.
                                addassemblyfile = BCBlockGameState.CanonicalizePath(addassemblyfile, scriptGroupFolder);
                            }

                            AssemblyNames.Add(addassemblyfile);

                        }
                        else if (currentline.StartsWith("LANGUAGE=", StringComparison.OrdinalIgnoreCase))
                        {
                            Language = currentline.Substring(9);
                            WriteLog("Settings language to " + Language);

                        }
                        else if (currentline.StartsWith("FILE=", StringComparison.OrdinalIgnoreCase))
                        {
                            String addfilename = currentline.Substring(5);
                            WriteLog("File specified:" + addfilename);
                            addfilename = BCBlockGameState.CanonicalizePath(addfilename, scriptGroupFolder);
                            WriteLog("Canonicalized path:" + addfilename);
                            ScriptFilenames.Add(addfilename);




                        }
                        else if (currentline.StartsWith("IMAGE=", StringComparison.OrdinalIgnoreCase))
                        {
                            String addimage = currentline.Substring(6);
                            WriteLog("Image specified:" +addimage);
                            addimage = BCBlockGameState.CanonicalizePath(addimage, scriptGroupFolder);
                            WriteLog("Canonicalized path:" + addimage);
                            AddImages.Add(addimage);

                        }
                        else if (currentline.StartsWith("SOUND=", StringComparison.OrdinalIgnoreCase))
                        {
                            String addsound = currentline.Substring(6);
                            WriteLog("Sound specified:" + addsound);
                            addsound = BCBlockGameState.CanonicalizePath(addsound, scriptGroupFolder);
                            WriteLog("Canonicalized path:" + addsound);
                            AddSounds.Add(addsound);
                        }
                        else
                        {
                            //ignore.
                        }


                        
                    }
                    LogStream.Flush();
                } //using
                //all our values ought to be initialized now.






            }

            public static CodeDomProvider  GetCodeProviderForLanguage(String language)
            {

                switch(language.ToLower())
                {
                    case "VB.NET":
                        return new VBCodeProvider();
                    case "C#":
                        return new CSharpCodeProvider();
                    default:
                        return null;
                }


            }



        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupFile">Script group filename</param>

        /// <returns></returns>
        public static ScriptGroupData CompileScriptGroup(String groupFile)
        {

            //format of a ScriptGroup file:
            
            //this function needs to first find and get the info on all ScriptGroup files (BASeBlock Script Group, bbsg extension). Then it will
            //find other script files, <i>not included by any of those Script Groups</i> in those specified directories.
            ScriptGroupData sgd = new ScriptGroupData(groupFile);
            return sgd;

           




        }
       

        public static Assembly[] CompileScripts(String[] directories,iManagerCallback datahook)
        {

            //What this routine needs to do:

            //FIRST: find all .bbsg (baseblock script group) files in the given directories.
            //with each, create a new ScriptGroupData object.
            //compile the files of each scriptgroup data.
            //iterate through .cs and .vb files. create/compile an assembly only for those not present in the aforementioned list.
            

            
            List<Assembly> buildlist = new List<Assembly>();
            List<String> includedscriptfiles = new List<string>();
            List<ScriptGroupData> sgdList = new List<ScriptGroupData>();
            foreach (String loopdirectory in directories)
            {
                //find all scriptgroups...
                foreach (FileInfo loopgroupfile in new DirectoryInfo(loopdirectory).GetFiles("*.bbsg", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        ScriptGroupData sgdmake = new ScriptGroupData(loopgroupfile.FullName);
                        //add to list.
                        sgdList.Add(sgdmake);
                        //add it's files to the includedscriptfiles list...
                        includedscriptfiles.AddRange(sgdmake.ScriptFilenames);


                    }
                    catch(Exception groupexception)
                    {
                        datahook.ShowMessage("Exception working with ScriptGroup file:" + loopgroupfile.FullName);
                    }

                }



            }

            //now, we search again, finding all .cs and .vb files.
            List<String> allscriptfiles = new List<string>();
            foreach (String loopdirectory in directories)
            {
                DirectoryInfo thisdir = new DirectoryInfo(loopdirectory);
                //three cheers for LINQ....
                allscriptfiles.AddRange(from p in thisdir.GetFiles("*.vb", SearchOption.TopDirectoryOnly) where !includedscriptfiles.Contains(p.FullName)  select p.FullName);
                allscriptfiles.AddRange(from p in thisdir.GetFiles("*.cs", SearchOption.TopDirectoryOnly) where !includedscriptfiles.Contains(p.FullName) select p.FullName);
                allscriptfiles.AddRange(from p in thisdir.GetFiles("*.fs", SearchOption.TopDirectoryOnly) where !includedscriptfiles.Contains(p.FullName) select p.FullName);

            }

            //final step, compile all the scriptGroupData's and the allscriptfiles into assemblies...


            //first, ScriptGroupData's...
            foreach (ScriptGroupData loopgroup in sgdList)
            {
                Assembly groupresult=null;
                try
                {
                    var cr = loopgroup.Compile(datahook);
                    groupresult = cr.CompiledAssembly;
                    if (groupresult!=null) buildlist.Add(groupresult);
                }
                catch(Exception sgdexception)
                {
                    



                }



            }
            List<ScriptCompileFailureException> compileexceptions = new List<ScriptCompileFailureException>();
            //now, the allscriptfiles list.
            foreach (String compileme in allscriptfiles)
            {
                try
                {
                    Assembly compileresult = CompileScriptToAssembly(datahook,compileme);
                    if (compileresult != null) buildlist.Add(compileresult);

                }
                catch (ScriptCompileFailureException exx)
                {
                    compileexceptions.Add(exx);
                }


            }
            if (compileexceptions.Any())
            {
                var createdaggregate = new AggregateScriptFailureException(compileexceptions);
                datahook.FlagError("CompileScripts-" + compileexceptions.Count + " Exceptions.", createdaggregate);
                //datahook.ShowMessage(createdaggregate.ToString());
            }


            return buildlist.ToArray();
        }

        public static Assembly CompileScriptToAssembly(String scriptFile)
        {

            //FileStream readfilein = new FileStream(ScriptFile, FileMode.Open);
            //StreamReader sr = new StreamReader(readfilein);
            //String allcode = sr.ReadToEnd();
            //sr.Close();
            String extensiontest = Path.GetExtension(scriptFile).ToLower();
            String[] assemblyNames = new String[] { "system.dll","System.Design.dll",
                "system.xml.dll",
                "system.data.dll",
                "system.windows.forms.dll","system.drawing.dll",
                Assembly.GetExecutingAssembly().Location};

            CompilerParameters cp = new CompilerParameters(assemblyNames);
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            String outfile = Path.Combine(BCBlockGameState.GetTempPath(), "script_" + Path.GetFileNameWithoutExtension(scriptFile));
            if (File.Exists(outfile)) File.Delete(outfile);
            cp.OutputAssembly = outfile;
            Assembly result;
            CompilerResults cr = null;
            switch (extensiontest)
            {
                case ".cs":
                    CSharpCodeProvider prov = new CSharpCodeProvider();

                    cr = prov.CompileAssemblyFromFile(cp, new string[] { scriptFile });




                    break;
                case ".vb":
                    Microsoft.VisualBasic.VBCodeProvider vbprov = new VBCodeProvider();
                    cr = vbprov.CompileAssemblyFromFile(cp, new string[] { scriptFile });
                    
                    break;
                case ".js":
                    Microsoft.JScript.JScriptCodeProvider jsprov = new JScriptCodeProvider();
                    cr = jsprov.CompileAssemblyFromFile(cp, new string[] { scriptFile });
                    break;
             

            }
            if (cr.Errors.HasErrors || cr.Errors.HasWarnings)
            {

                //create a new file, if the script was scriptfile.csx, create scriptfile.csx.log
                String newfile = scriptFile + ".log";
                FileStream logfsstream = new FileStream(newfile, FileMode.Create);
                StreamWriter logstream = new StreamWriter(logfsstream);

                int warningsCount = 0, errorsCount = 0;
                List<CompilerError> warnings = new List<CompilerError>(), errors = new List<CompilerError>();
                foreach (CompilerError cerror in cr.Errors)
                {
                    if (cerror.IsWarning)
                    {
                        warningsCount++;
                        warnings.Add(cerror);

                    }

                    else
                    {
                        errorsCount++;
                        errors.Add(cerror);
                    }

                }

                ScriptCompileFailureException scf = new ScriptCompileFailureException(scriptFile,
                    errors, warnings);
                throw scf;

            }
            else
            {
                Debug.Print("Compiled \"" + scriptFile + "\" successfully...");
            }
            return cr.CompiledAssembly ?? null;





        }
        public static Assembly CompileScriptToAssembly(iManagerCallback datahook,String scriptFile)
        {

            //FileStream readfilein = new FileStream(ScriptFile, FileMode.Open);
            //StreamReader sr = new StreamReader(readfilein);
            //String allcode = sr.ReadToEnd();
            //sr.Close();
            String extensiontest = Path.GetExtension(scriptFile).ToLower();
            String[] assemblyNames = new String[] { "system.dll","System.Design.dll",
                "system.xml.dll",
                "system.data.dll",
                "system.windows.forms.dll","system.drawing.dll",
                Assembly.GetExecutingAssembly().Location};
            
            CompilerParameters cp = new CompilerParameters(assemblyNames);
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            String outfile = Path.Combine(BCBlockGameState.GetTempPath(), "script_" + Path.GetFileNameWithoutExtension(scriptFile));
            if (File.Exists(outfile)) File.Delete(outfile);
            cp.OutputAssembly = outfile;
            Assembly result;
            CompilerResults cr = null;
            switch (extensiontest)
            {
                case ".cs":
                    CSharpCodeProvider prov = new CSharpCodeProvider();

                    cr = prov.CompileAssemblyFromFile(cp, new string[] { scriptFile });
                    



                    break;
                case ".vb":
                    Microsoft.VisualBasic.VBCodeProvider vbprov = new VBCodeProvider();
                    cr = vbprov.CompileAssemblyFromFile(cp, new string[] { scriptFile });
                    break;
                case ".js":
                    Microsoft.JScript.JScriptCodeProvider jsprov = new JScriptCodeProvider();
                    cr = jsprov.CompileAssemblyFromFile(cp, new string[] { scriptFile });
                    break;
                case ".fs":
                    //fsharp
                    //unused...
                    
                case "bbsg":
                    //bbsg: BASeBlock Script Group
                    ScriptGroupData sgd = new ScriptGroupData(scriptFile);
                    cr = sgd.Compile(datahook);
                    break;


            }
            if (cr.Errors.HasErrors || cr.Errors.HasWarnings)
            {

                //create a new file, if the script was scriptfile.csx, create scriptfile.csx.log
                String newfile = scriptFile + ".log";
                FileStream logfsstream = new FileStream(newfile, FileMode.Create);
                StreamWriter logstream = new StreamWriter(logfsstream);

                int warningsCount = 0, errorsCount = 0;
                List<CompilerError> warnings = new List<CompilerError>(), errors = new List<CompilerError>();
                foreach (CompilerError cerror in cr.Errors)
                {
                    if (cerror.IsWarning)
                    {
                        warningsCount++;
                        warnings.Add(cerror);
                        logstream.WriteLine("Warning:" + cerror.ToString());

                    }

                    else
                    {
                        errorsCount++;
                        errors.Add(cerror);
                        logstream.WriteLine("Error:" + cerror.ToString());
                    }

                }
                
                ScriptCompileFailureException scf = new ScriptCompileFailureException(scriptFile,
                    errors, warnings);
                throw scf;

            }
            else
            {
                Debug.Print("Compiled \"" + scriptFile + "\" successfully...");
            }
            return cr.CompiledAssembly ?? null;





        }

    }
}
