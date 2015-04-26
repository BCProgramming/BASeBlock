using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BASeCamp.CommandLineParser
{
    public class CmdParser
    {
        private List<CommandLineElement> _Elements = new List<CommandLineElement>();

        /// <summary>
        /// private dictionary used to index each switch. Each element is a List of all the switches of that kind passed. 
        /// </summary>
        private Dictionary<String, List<Switch>> storedSwitches =
            new Dictionary<string, List<Switch>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// parameterless constructor. This will construct the Parser with the current Environment's Command Line.
        /// </summary>
        public CmdParser()
            : this(Environment.CommandLine)
        {
        }

        /// <summary>
        /// Constructs a CmdParser object with the given commandline.
        /// </summary>
        /// <param name="cmdLine">CommandLine to Parse.</param>
        public CmdParser(String cmdLine)
        {
            ParseCommandLine(cmdLine);
        }

        public bool hasSwitch(String testfor)
        {
            return storedSwitches.ContainsKey(testfor);
        }

        /// <summary>
        /// Enumeration of all Switches that have the given switch String.
        /// </summary>
        /// <param name="SwitchFind"></param>
        /// <returns></returns>
        public IEnumerable<Switch> getSwitches(String SwitchFind)
        {
            if (hasSwitch(SwitchFind))
            {
                return storedSwitches[SwitchFind];
            }
            //no switch found, return an empty enumeration.
            return Enumerable.Empty<Switch>();
        }

        public IEnumerable<Switch> getSwitches()
        {
            return from p in _Elements where p is Switch select p as Switch;
        }

        public IEnumerable<ArgumentItem> getArguments()
        {
            return from a in _Elements where a is ArgumentItem select a as ArgumentItem;
        }

        /// <summary>
        /// Wrapper factory that simply returns a new instance of a CmdParser. 
        /// </summary>
        /// <param name="cmdLine"></param>
        /// <returns></returns>
        public static CmdParser Parse(String cmdLine)
        {
            return new CmdParser(cmdLine);
        }

        private void ParseCommandLine(String cmdLine)
        {
            int currpos = 0;
            int initialpos = 0;
            while (currpos < cmdLine.Length)
            {
                while (char.IsWhiteSpace(cmdLine.ElementAt(currpos))) currpos++;
                initialpos = currpos;

                if (Switch.SwitchAtPos(cmdLine, currpos))
                {
                    Switch sw = new Switch(cmdLine, ref currpos);
                    _Elements.Add(sw);
                    if (!storedSwitches.ContainsKey(sw.SwitchValue))
                        storedSwitches.Add(sw.SwitchValue, new List<Switch>());
                    storedSwitches[sw.SwitchValue].Add(sw);
                }
                else
                {
                    _Elements.Add(new ArgumentItem(cmdLine, ref currpos));
                }
                if (initialpos == currpos) break;
            }
        }

        /// <summary>
        /// Represents an Argument. This includes arguments that exist bare on the command line as well as arguments used with a given switch.
        /// </summary>
        public class ArgumentItem : CommandLineElement
        {
            public static ArgumentItem Empty = new ArgumentItem();
            private String _Argument;

            protected internal ArgumentItem()
            {
                _Argument = "";
            }

            /// <summary>
            /// Construct an Instance from the given string, assuming the start of an Argument element at the given position.
            /// </summary>
            /// <param name="strParse">Command Line to parse.</param>
            /// <param name="Position">Location to start. This variable will be updated with the ending position of the argument that was discovered upon return.</param>
            public ArgumentItem(String strParse, ref int Position)
            {
                int startpos = Position;
                int sloc = startpos;
                while (char.IsWhiteSpace(strParse.ElementAt(sloc))) sloc++;
                if (strParse.ElementAt(sloc) == '"')
                {
                    sloc++;
                    while (true)
                    {
                        if (sloc >= strParse.Length) break;
                        bool doublequote = strParse.Length > sloc + 2 && strParse.Substring(sloc, 2).Equals("\"");
                        //if we found a quote and it's not a double quote...
                        if (strParse.ElementAt(sloc) == '"' && !doublequote)
                        {
                            sloc++;
                            break;
                        }
                        if (doublequote) sloc++; //add an extra spot for the dual quote.

                        sloc++;
                    }
                }
                else
                {
                    sloc = strParse.IndexOfAny(new char[] {'/', ' '}, sloc);
                }
                _Argument = strParse.Substring(Position, sloc - startpos);
                Position = sloc;
            }

            /// <summary>
            /// returns the Argument this Object represents. This will include quotation marks if they were used in the originally parsed string.
            /// </summary>
            public String Argument
            {
                get { return _Argument; }
            }

            /// <summary>
            /// implicitly converts an ArgumentItem to a String.
            /// </summary>
            /// <param name="value">ArgumentItem to implicitly convert.</param>
            /// <returns>the result from calling Chomp() on the given instance.</returns>
            public static implicit operator String(ArgumentItem value)
            {
                return value.Chomp();
            }

            /// <summary>
            /// returns the Argument value. If it starts with and endswith quotation marks, they will be removed.
            /// </summary>
            /// <returns></returns>
            public String Chomp()
            {
                if (_Argument.StartsWith("\"") && Argument.EndsWith("\""))
                    return _Argument.Substring(1, _Argument.Length - 2);
                else return _Argument;
            }

            public override string ToString()
            {
                if (Argument.Any(Char.IsWhiteSpace))
                    return "\"" + Argument + "\"";
                else return Argument;
            }
        }

        public abstract class CommandLineElement
        {
            public abstract override string ToString();
        }

        public class Switch : CommandLineElement
        {
            internal static String[] SwitchPreceders = new string[] {"--", "-", "/"};
            private ArgumentItem _Argument = ArgumentItem.Empty;
            private String _SwitchValue;

            public Switch(String strParse, ref int StartLocation)
            {
                while (String.IsNullOrWhiteSpace(strParse.ElementAt(StartLocation).ToString())) StartLocation++;
                var sLoc = StartLocation;
                var retrieved = SwitchPreceders.
                    FirstOrDefault((s) => !(sLoc + s.Length > strParse.Length) &&
                                          strParse.Substring(sLoc, s.Length)
                                                  .Equals(s, StringComparison.OrdinalIgnoreCase));

                if (retrieved == null)
                {
                    throw new ArgumentException("Passed String " + strParse +
                                                " Does not have a switch preceder at position " + StartLocation);
                }

                var NextSpace = strParse.IndexOfAny(new char[] {' ', '\t', '/', ':'}, sLoc + 1);
                //if(((NextSpace-sLoc)-sLoc+1) <= 0) throw new ArgumentException("Error Parsing Switch");
                _SwitchValue = strParse.Substring(sLoc + 1, NextSpace - sLoc - 1);
                sLoc += retrieved.Length; //we don't want the switch itself.
                //now we need to determine where the Switch ends. colon or space seems reasonable. If a colon, the next entity will be an argument.
                StartLocation = NextSpace;
                //if the char at NextSpace is a Colon...
                if (strParse.ElementAt(NextSpace) == ':')
                {
                    //interpret as an argument
                    NextSpace++;
                    _Argument = new ArgumentItem(strParse, ref NextSpace);
                }
                StartLocation = NextSpace;
            }

            public String SwitchValue
            {
                get { return _SwitchValue; }
            }

            public String Argument
            {
                get { return _Argument; }
            }

            //Constructs an instance of a switch from the given location.

            public static bool SwitchAtPos(String strParse, int Location)
            {
                var retrieved = SwitchPreceders.
                    FirstOrDefault((s) => !(Location + s.Length > strParse.Length) &&
                                          strParse.Substring(Location, s.Length)
                                                  .Equals(s, StringComparison.OrdinalIgnoreCase));
                return retrieved != null;
            }

            public override string ToString()
            {
                return "//" + _SwitchValue + ":" + _Argument.ToString();
            }

            public bool HasArgument()
            {
                return _Argument.Equals(ArgumentItem.Empty);
            }
        }
    }
}