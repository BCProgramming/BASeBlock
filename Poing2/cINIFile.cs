

/*
BASeCamp Software Solutions INIFile Handling Utility class
Copyright (c) 2011, Michael Burgwin
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer 
in the documentation and/or other materials provided with the distribution.

Neither the name of BASeCamp Corporation nor the names of its contributors may be used to endorse or promote products 
derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,
BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

//CONDITIONAL; I've added new features to this class that take advantage of dynamics.
//as a result- (Naturally) those features require .NET 4.0. However, I still have many projects written in and for .NET 3.5 in Visual Studio 2008, and 
//don't want to break those. So this can be set or unset as needed.

//#define CS4



using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.IO;
using System.Threading;
using System.Xml;
using BASeCamp.Configuration;
using Microsoft.Win32;

#if CS4
using System.Dynamic;
#endif


namespace BASeCamp.Configuration
{

    //Storage consolidation...
    //Data storage will be perceivably heirarchal, similar to the Registry.
    //Different implementations will of course... differ... on the implementation

    //The interface itself is relatively simple, and only allows direct access to keys and values.

    public interface ISettingsStorage
    {
        /// <summary>
        /// retrieves a value from the settings storage.
        /// </summary>
        /// <param name="SectionName">name of the section containing this setting.</param>
        /// <param name="ValueName">Name of the value</param>
        /// <param name="DefaultValue">Default Value</param>
        /// <returns>Current value of this setting, or the Default value if none is found.</returns>
        String GetValue(String SectionName, String ValueName, String DefaultValue);
        /// <summary>
        /// Sets Value of a Setting.
        /// </summary>
        /// <param name="SectionName">Name of the section containing this setting.</param>
        /// <param name="ValueName">Name of the value to set</param>
        /// <param name="newValue">Value to assign to this setting</param>
        /// <returns>Previous value of this setting</returns>
        String SetValue(String SectionName, String ValueName, String newValue);
        /// <summary>
        /// Retrieves the Names of all values within the given section.
        /// </summary>
        /// <param name="Section">Name of section to get information on.</param>
        /// <returns>Enumeration of names of the values within this section.</returns>
        IEnumerable<String> GetValueNames(String Section);
        /// <summary>
        /// Retrieves the names of all sections within the given section.
        /// </summary>
        /// <param name="Section">Name of section for which to get child sections.</param>
        /// <returns>Enumeration of the names of all sections within the specified Section.</returns>
        IEnumerable<String> GetSectionNames(String Section);


    }
    public class BaseSettingsValue
    {
        private BaseSettings _Parent;
        private String _SectionName;
        private String _ValueName;

        public BaseSettings Parent { get { return _Parent; } set { _Parent = value; } }
        public String SectionName { get { return _SectionName; } set { _SectionName = value; } }
        public String ValueName { get { return _ValueName; } set { _ValueName = value; } }

        public String Value
        {
            get { return _Parent.GetValue(_SectionName, _ValueName, ""); }
            set { _Parent.SetValue(_SectionName, _ValueName, value); }
        }

        public BaseSettingsValue(BaseSettings pParent, String pSection, String pValueName)
        {
            _Parent = pParent;
            _SectionName = pSection;
            _ValueName = pValueName;

        }

        public static implicit operator String(BaseSettingsValue val)
        {
            return val.Value;

        }


    }
    public class BaseSettingsSection
    {
        private BaseSettings _Parent;
        private String _Section;

        public BaseSettingsSection(BaseSettings settings, String section)
        {
            _Parent = settings;
            _Section = section;

        }
        public BaseSettingsValue this[String ValueName]
        {
            get
            {
                return new BaseSettingsValue(_Parent, _Section, ValueName);
            }

        }
    }

    public class SettingsCategory
    {
        private BaseSettings _Owner = null;
        private String _Category = "";
        //represents a category, section, etcetera.
        public SettingsCategory(BaseSettings pOwner, String pCategory)
        {
            _Owner = pOwner;
            _Category = pCategory;
        }



    }

    /// <summary>
    /// base class  that implements ISettingsStorage and takes a ISettingsStorage implementor as an argument.
    /// it provides simpler access to the various components of a settings implementation.
    /// </summary>

    public class BaseSettings : ISettingsStorage
    {
        private ISettingsStorage _Storage;
        public BaseSettings(ISettingsStorage owner)
        {
            _Storage = owner;

        }
        public string GetValue(string SectionName, string ValueName, string DefaultValue)
        {
            return _Storage.GetValue(SectionName, ValueName, DefaultValue);
        }
        public string SetValue(string SectionName, string ValueName, string newValue)
        {
            return _Storage.SetValue(SectionName, ValueName, newValue);
        }
        public IEnumerable<string> GetValueNames(string Section)
        {
            return _Storage.GetValueNames(Section);
        }
        public IEnumerable<string> GetSectionNames(string Section)
        {
            return _Storage.GetSectionNames(Section);
        }



    }

    public class MRULists
    {
        //manages a list of MRUList classes. well, not really. sorta does.
        private Dictionary<String, INIMRUList> _Lists = new Dictionary<string, INIMRUList>();
        private INIFile _Settings = null;
        public MRULists(INIFile Settings)
        {
            _Settings = Settings;


        }

        public INIMRUList this[String name]
        {
            get
            {
                if (!_Lists.ContainsKey(name))
                {

                    //add it by creating it.
                    INIMRUList createlist = new INIMRUList(_Settings, name);
                    _Lists.Add(name, createlist);


                }
                return _Lists[name];

            }


            set
            {
                _Lists[name] = value;


            }


        }




    }





    /// <summary>
    /// class that works with a INIFile class and manages an MRU list within it.
    /// the MRU list section is Recent.name, where name is the value given in the constructor.
    /// </summary>
    public class INIMRUList
    {


        private String _Name;
        private INIFile _Settings;
        public String Name { get { return _Name; } private set { _Name = value; } }
        private Queue<String> MRUList;
        private const int maxsize = 8;
        private INISection oursection = null;
        public INIFile Settings { get { return _Settings; } private set { _Settings = value; } }

        public List<String> Names
        {
            get { return MRUList.ToList(); }
            set
            {
                MRUList.Clear();
                foreach (String loopadd in value)
                {
                    MRUList.Enqueue(loopadd);


                }


            }




        }
        public bool AddToList(String filename)
        {
            if (filename == null) return false;
            foreach (String loopit in MRUList)
            {

                if (loopit.Equals(filename, StringComparison.OrdinalIgnoreCase))
                {
                    //we will change it's date...

                    return false; //don't add



                }


            }
            //otherwise, add it.
            MRUList.Enqueue(filename);
            return true;

        }

        public INIMRUList(INIFile INISettings, String name)
        {
            Name = name;
            Settings = INISettings;
            MRUList = new Queue<string>();
            oursection = INISettings["recent." + name];

            foreach (var loopitem in oursection.getValues())
            {
                MRUList.Enqueue(loopitem.Value);




            }
            //remove excess items.
            while (MRUList.Count > maxsize)
            {
                MRUList.Dequeue();

            }

        }
        ~INIMRUList()
        {

            //deconstructor/finalizer.
            //Task is to save the queue to INISettings.
            int currentitem = 1;
            //first clear the current settings...
            oursection.INIItems.Clear();
            while (MRUList.Any())
            {
                String dequeued = MRUList.Dequeue();
                //save it to the settings...
                oursection["item" + currentitem.ToString()].Value = dequeued;
                currentitem++;

            }

        }




    }



    public static class INItemValueExtensions
    {
        //extensions for INIDataItem

        //normally, INIDataItem is a Name/Value Pair; More Specifically, because of the way INI files are, they are
        //naturally typeless. However, most configuration options are mapped to a different type by the application.
        //and I've found it to be a gigantic pain to have to write the same TryParse() handling code over and over.
        //so I added these handy extensions to the INIDataItem class, which provide some functions for setting.
        //I keep them out of the main code simply because that way it doesn't clutter it up. It's already cluttered enough as-is.



        /// <summary>
        /// Attempts to use Convert.ChangeType() to change the Value of this INIDataItem to the specified type parameter.
        /// If this fails, it will attempt to call a static "TryParse(String, out T)" method on the generic type parameter.
        /// If THAT fails, it will return the passed in DefaultValue parameter.
        /// </summary>
        /// <typeparam name="T">Parameter Type to retrieve and act on in Static context.</typeparam>
        /// <param name="dataitem">INIDataItem instance whose value is to be parsed to the given type.</param>
        /// <param name="DefaultValue">Default value to return</param>
        /// <returns>Result of the parse/Conversion, or the passed in DefaultValue</returns>
        public static T GetValue<T>(this INIDataItem dataitem, T DefaultValue)
        {

            //first check if it is a Type that uses Base64...
            if (dataitem.Value == "") return DefaultValue;
            if (Base64Applicable<T>())
            {
                // T grabvalue = DeserializeFromBase64<T>(dataitem.Value);
                //sigh, darn constraints... now we need to use reflection to create and invoke the generic method with our type T.

                var Searchmethods = typeof(INItemValueExtensions).GetMethods();
                foreach (MethodInfo iteratemethod in Searchmethods)
                {
                    if (iteratemethod.Name.Equals("DeserializeFromBase64", StringComparison.OrdinalIgnoreCase))
                    {
                        MethodInfo DeserializeMethod = iteratemethod.MakeGenericMethod(typeof(T));
                        //now, invoke it...
                        T grabvalue = (T)DeserializeMethod.Invoke(null,
                            BindingFlags.Static | BindingFlags.InvokeMethod, null,
                            new object[] { dataitem.Value }, null);
                        return grabvalue;

                    }


                }





            }


            //Generic method, attempts to call a static "TryParse" argument on the given class type, passing in the dataitem's value.
            try
            {
                return (T)Convert.ChangeType(dataitem.Value, typeof(T));
            }
            catch (Exception ece)
            {
                //attempt to call TryParse. on the static class type.
                //TryParse(String, out T)

                Type usetype = typeof(T);
                T result = default(T);
                Object[] passparams = new object[] { dataitem.Value, result };
                try
                {
                    MethodInfo TryParseMethod = null;
                    foreach (MethodInfo iteratemember in usetype.GetMethods())
                    {
                        if (iteratemember.Name.Equals("TryParse", StringComparison.OrdinalIgnoreCase))
                        {
                            TryParseMethod = iteratemember;
                            break;

                        }


                    }

                    if (TryParseMethod != null)
                    {
                        bool tpresult = (bool)TryParseMethod.Invoke(result, BindingFlags.InvokeMethod, null, passparams, Thread.CurrentThread.CurrentCulture);


                        if (tpresult)
                        {
                            //tryparse succeeded!
                            return (T)passparams[1]; //second index was out parameter...
                        }
                        else
                        {
                            return DefaultValue;
                        }
                    }
                    else
                    {
                        return DefaultValue;
                    }
                }
                catch (Exception xx)
                {
                    //curses...
                    return DefaultValue;


                }



            }
            return DefaultValue;

        }
        public static void SetValue<T>(this INIDataItem dataitem)
        {
            dataitem.SetValue<T>(default(T));


        }

        /// <summary>
        /// Logical inverse of the getValue routine... a bit faster to implement...
        /// </summary>
        /// <typeparam name="T">Type of the parameter being set</typeparam>
        /// <param name="dataitem">INIDataItem class instance</param>
        /// <param name="newvalue">New value to set.</param>
        /// <remarks>
        /// The SetValue Extension method will save the object to the INI value. This is achieved by converting the object to a String. 
        /// If the object doesn't have a static TryParse() method, but implements the ISerializable interface, the routine will use a BinaryFormatter 
        /// to serialize the object, and then take that byte stream and create a Base64 encoded string from it, and set that as the value.
        /// If the class type implements a static TryParse() routine, however, the routine will simply use .ToString() on the given reference and save that.
        /// </remarks>
        public static void SetValue<T>(this INIDataItem dataitem, T newvalue)
        {
            //if T implements ISerializable and doesn't have a TryParse, we'll serialize to a 
            //Base64 string, and save that instead.
            if (Base64Applicable<T>())
            {
                String useb64 = SerializeToBase64((ISerializable)newvalue);
                dataitem.Value = useb64;
            }
            else
            {



                dataitem.Value = newvalue.ToString();
            }

        }
        private static bool Base64Applicable<T>()
        {
            //doesn't actually have any params, lol...

            //does it have a TryParse() method?
            Type ttype = typeof(T);
            bool hastryparse = false;
            foreach (MethodInfo itmethod in ttype.GetMethods())
            {
                if (itmethod.Name.Equals("TryParse", StringComparison.OrdinalIgnoreCase))
                {
                    hastryparse = true;
                    break;


                }


            }
            //return that it's base64 applicable only if it doesn't have a TryParse method and it implements ISerializable.
            //the assumption being that if it Implements TryParse, that reverses toString(). 
            //We don't check the parameters for TryParse, (requiring a string and an out parameter) though.


            if (!hastryparse) return (ttype is ISerializable);
            return false;
        }

        private static void GetTypeDefault<T>(out T result)
        {
            Type tt = typeof(T);

            //basic idea: call default, empty constructor using reflection.
            ConstructorInfo defaultconstructor = tt.GetConstructor(new Type[] { });

            result = (T)defaultconstructor.Invoke(null);





        }
        private static String SerializeToBase64(ISerializable objserialize)
        {
            //write the object graph to a Memory stream...
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream WriteTo = new MemoryStream();
            bf.Serialize(WriteTo, objserialize);
            //grab the bytes...
            WriteTo.Seek(0, SeekOrigin.Begin);
            byte[] grabbytes = new byte[WriteTo.Length];
            WriteTo.Read(grabbytes, 0, grabbytes.Length);

            //convert the bytes to a Base64 string, and return it..
            String base64 = Convert.ToBase64String(grabbytes);

            return base64;

        }

        private static T DeserializeFromBase64<T>(String fromstring) where T : ISerializable
        {

            //get the bytes in the string.
            char[] chararray = fromstring.ToCharArray();
            byte[] byteformat = Convert.FromBase64CharArray(chararray, 0, chararray.Length);

            //create a MemoryStream on this sequence of bytes.
            MemoryStream Deserializefrom = new MemoryStream(byteformat);

            //get a BinaryFormatter...
            BinaryFormatter bf = new BinaryFormatter();
            object result = bf.Deserialize(Deserializefrom);

            Deserializefrom.Dispose();
            if (!(result is T))
            {
                throw new SerializationException("Deserialized Object was not a " + typeof(T).Name + ".");

            }
            else
            {
                return (T)result;
            }






        }




    }


    /// <summary>
    /// class used to represent any INI value, section, or comment.
    /// </summary>
    public abstract class INIItem
    {
        public abstract override string ToString();
    }

    public class INIDataItem : INIItem
    {
        static IDictionary environvars = Environment.GetEnvironmentVariables();

        public INISection OwnerSection;

        public String Name { get; set; }

        String _Value;

        //Operator overloads.
        public static implicit operator string(INIDataItem dataitem)
        {
            return dataitem.Value;

        }

        public String Value
        {
            get
            {




                String expandedstring = _Value;
                expandenviron(ref expandedstring);

                String reffed = expandedstring;
                if (OwnerSection != null)
                    if (OwnerSection.OwnerINI != null)
                        OwnerSection.OwnerINI.FireBeforeRetrieve(OwnerSection.Name, Name, ref reffed);

                return reffed;
            }
            set
            {

                if (OwnerSection != null)
                    if (OwnerSection.OwnerINI != null)
                        OwnerSection.OwnerINI.FireBeforeSet(OwnerSection.Name, Name, ref value);

                _Value = value;


            }
        }




        public bool GetBoolean()
        {
            return GetBoolean(false);

        }

        public bool GetBoolean(bool defaultval)
        {
            bool tparse;
            if (bool.TryParse(Value, out tparse))
                return true;
            else
            {
                return defaultval;
            }


        }

        public String[] getArrayValue()
        {
            return getArrayValue("|");

        }

        public String[] getArrayValue(String separator)
        {

            return this.Value.Split(new String[] { separator }, StringSplitOptions.RemoveEmptyEntries);


        }

        public override string ToString()
        {
            return Name + "=" + Value;



        }
        /// <summary>
        /// examines the given string and returns true if any environment variables are found (enclosed in % signs)
        /// </summary>
        /// <param name="checkstring">The string to check.</param>
        /// <returns>True if the specified string contains a environment variable enclosed in percent signs- false otherwise.</returns>
        private bool expandenviron(ref String checkstring)
        {

            //if no APPDATA key exists, add one.
            IDictionary envvars = environvars;
            if (!envvars.Contains("APPDATA"))
                envvars.Add("APPDATA", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

            foreach (String loopenv in envvars.Keys)
            {
                if (checkstring.IndexOf("%" + loopenv + "%", StringComparison.OrdinalIgnoreCase) > -1)
                    checkstring = checkstring.Replace("%" + loopenv + "%", (String)envvars[loopenv]);





            }
            checkstring = checkstring.Replace('\\', Path.DirectorySeparatorChar);
            return false;


        }

        internal INIDataItem(String pName, String pValue, INISection ownersection)
        {
            Name = pName;
            //expandenviron(ref pValue);
            OwnerSection = ownersection;
            if (pValue.StartsWith("\""))
            {
                pValue = pValue.Substring(1, pValue.IndexOf("\"", 1) - 1);



            }


            Value = pValue;


        }
    }
    /// <summary>
    /// INIItem derived class that represents a Comment.
    /// </summary>
    public class INIComment : INIItem
    {
        public String Comment { get; set; }

        public INIComment(String pComment)
        {
            Comment = pComment;
        }


        public override string ToString()
        {
            return Comment;
        }
    }
    //if .NET4 is available, we derive from DynamicObject, and override parts of it.
#if CS4
    public class INISection : DynamicObject
#else
    //otherwise, we don't.
    public class INISection : IEnumerable<INIItem>
#endif
    {
        public INIFile OwnerINI = null;
        public List<INIItem> INIItems;
        public String Name { get; set; }
        public String eolComment { get; set; }
        //Code specific to .NET Framework 4.0.

        #region Base Class (.NET) Framework 4 dynamic object support code
#if CS4

        //CS4 code for dynamic: override some DynamicObject methods.
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return (from m in getValues() select m.Name);
        }
        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            String deleteItem = binder.Name;
            StringComparison usecompare = binder.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.CurrentCulture;
            //get the count...
            int currcount = INIItems.Count;
            //remove all where they have that name.
            foreach (var iteratevar in (from m in INIItems
                                        where (m is INIDataItem) && (((INIDataItem)m).Name.Equals(binder.Name, usecompare))
                                        select ((INIDataItem)m)))
            {
                INIItems.Remove(iteratevar);

            }
            int newcount = INIItems.Count;
            //return true of the count has changed, false otherwise.
            if (newcount < currcount) return true; else return false;


        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this[binder.Name];
            return true;



        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {

            //if it is a dataitem, set it directly.
            if (value is INIDataItem)
            {
                this[binder.Name] = (INIDataItem)value;
                return true;
            }
            else if (value is Tuple<String, Object>)
            {
                Tuple<String, Object> theTuple = (Tuple<String, Object>)value;
                INIDataItem getitem = this[binder.Name];
                getitem.Name = theTuple.Item1;
                getitem.Value = theTuple.Item2.ToString();
                return true;

            }
            else if (value is Tuple<String, String>)
            {
                Tuple<String, Object> theTuple = (Tuple<String, Object>)value;
                INIDataItem getitem = this[binder.Name];
                getitem.Name = theTuple.Item1;
                getitem.Value = theTuple.Item2.ToString();
                return true;
            }

            else if (value is KeyValuePair<String, Object>)
            {

                //Allow a KeyValuePair<String,Object> to be passed to set Name and Value.
                KeyValuePair<String, Object> castedval = (KeyValuePair<String, Object>)value;
                INIDataItem getitem = this[binder.Name];
                getitem.Name = castedval.Key;
                getitem.Value = castedval.Value.ToString();

                return true;
            }
            else if (value is KeyValuePair<String, String>)
            {
                //Allow a KeyValuePair<String,String> to be passed to set Name and Value.
                KeyValuePair<String, String> castedval = (KeyValuePair<String, String>)value;
                INIDataItem getitem = this[binder.Name];
                getitem.Name = castedval.Key;
                getitem.Value = castedval.Value;

                return true;

            }

            else
            {
                this[binder.Name].Value = value.ToString();
                return true;
            }
        }
#endif
        #endregion
        public INISection(String pName, string peolComment, List<INIItem> Values, INIFile pOwnerINI)
        {
            Name = pName;
            INIItems = Values;
            if (peolComment == null) peolComment = "";
            eolComment = peolComment;
            OwnerINI = pOwnerINI;

        }
        public INIDataItem this[String index, String defaultvalue]
        {
            get
            {
                INIDataItem returnthis = getValues().FirstOrDefault((w) => w.Name.Equals(index, StringComparison.OrdinalIgnoreCase));
                if (returnthis == null)
                {
                    returnthis = new INIDataItem(index, defaultvalue, this);


                }
                return returnthis;

            }


        }
        public INIDataItem this[params String[] entries]
        {
            get
            {
                foreach (var iterate in entries)
                {
                    var result = this[iterate];
                    if (!String.IsNullOrEmpty(result)) return result;


                }
                //return the first item if it isn't there.
                return this[entries.First()];
            }
            set
            {
                //set only the first entry...
                this[entries.First()] = value;


            }
        }
        public INIDataItem this[String index]
        {
            get
            {
                INIDataItem returnthis =
                    getValues().FirstOrDefault((w) => w.Name.Equals(index, StringComparison.OrdinalIgnoreCase));

                if (returnthis == null)
                {
                    //returnthis = new INISection(index, null, new List<INIItem>());
                    returnthis = new INIDataItem(index, "", this);
                    INIItems.Add(returnthis);
                }
                return returnthis;
            }
            set
            {
                //remove any existing value with the given name...
                INIItem itemfound =
                    getValues().FirstOrDefault(w => w.Name.Equals(index, StringComparison.OrdinalIgnoreCase));
                if (itemfound != null) INIItems.Remove(itemfound);
                INIItems.Add(value);
            }
        }

        public IEnumerable<INIDataItem> getValues()
        {
            foreach (INIItem loopitem in INIItems)
            {
                INIDataItem casted = loopitem as INIDataItem;
                if (casted != null)
                    yield return casted;
            }
        }
        /// <summary>
        /// Adds a INIDataItem to this Section.
        /// </summary>
        /// <param name="addto">Section to add to.</param>
        /// <param name="additem">item to add to the section.</param>
        /// <returns></returns>
        public static INISection operator +(INISection addto, INIDataItem additem)
        {
            addto[additem.Name] = additem;
            //addto.INIItems.Add(additem);
            return addto;


        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<INIItem> GetEnumerator()
        {
            return INIItems.GetEnumerator();
        }
        /// <summary>
        /// Retrieves Sections that can be called "children" if this section.
        /// A "Child" section would be something like Settings\Core compared to Settings, or Settings\Core\main compared to Settings\Core.
        /// </summary>
        /// <param name="checksection">Enumeration to examine. Usually acquired from a INIFile.Sections property.</param>
        /// <returns>Listing of Child sections of this section, or an empty enumerator.</returns>

        public IEnumerable<INISection> getChildSections(IEnumerable<INISection> checksection)
        {
            //first split our name.
            String[] splitus = Name.Split('\\');

            foreach (var iterate in checksection)
            {
                String[] splitem = iterate.Name.Split('\\');
                //a child section would have exactly one extra element.
                if (splitem.Length != splitus.Length + 1) continue;
                //and all elements up to that would be equal.
                for (int i = 0; i < splitus.Length; i++)
                {
                    if (!splitus[i].Equals(splitem[i], StringComparison.OrdinalIgnoreCase)) continue;
                }

                //if we get here, yield this section.
                yield return iterate;
            }

        }
        /// <summary>
        /// Searches the given enumeration to try to find a Section that could be called the "Parent" of this one.
        /// </summary>
        /// <param name="checksections">Sections to check.</param>
        /// <returns>INISection of the section that could be considered the parent of this one. null if none are found in the given enumeration.</returns>
        public INISection getParentSection(IEnumerable<INISection> checksections)
        {
            String[] grabparts = Name.Split('\\');
            String findname = String.Join("\\", (from y in grabparts where y != grabparts.Last() select y));

            INISection founditem = (from y in checksections
                                    where y.Name.Equals(findname, StringComparison.OrdinalIgnoreCase)
                                    || y.Name.Equals(findname + "\\", StringComparison.OrdinalIgnoreCase)

                                    select y).FirstOrDefault();

            return founditem;

        }
        public override string ToString()
        {
            return "[" + Name + "] (" + getValues().Count().ToString() + " Values, " +
                   (INIItems.Count() - getValues().Count()).ToString() + " Comments.";

        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


#if CS4
    public class INIFile : DynamicObject, ISettingsStorage
#else
    public class INIFile : IEnumerable<INISection>
#endif
    {
        public event Action<INIFile> FileEvent;
        public List<INISection> Sections { get; set; }
        private String _CategoryPrefix = "";
        private string _FileName = "";
        private bool _AutoSave = false;
        //idea behind this event being to make it easier to have multiple sources for INI data.

        public delegate void RetrieveValueFunc(String SectionName, String ValueName, ref String val);
        public delegate void SetValueFunc(String SectionName, String ValueName, ref String val);
        public event RetrieveValueFunc BeforeRetrieveValue;
        public event SetValueFunc BeforeSetValue;
        private FileSystemWatcher fsw = null;
        /// <summary>
        /// If set to true, the INIFile will try to save in it's finalizer.
        /// </summary>
        public bool AutoSave { get { return _AutoSave; } set { _AutoSave = value; } }
        /// <summary>
        /// When set, requests directed at Sections will be prefixed with this value. This can be useful for setting a category at
        /// load time and "forgetting" about it thereafter.
        /// </summary>
        public String CategoryPrefix { get { return _CategoryPrefix; } set { _CategoryPrefix = value; } }

        internal void FireBeforeRetrieve(String sSection, String sValue, ref String val)
        {
            var copied = BeforeRetrieveValue;
            if (copied != null)
                copied.Invoke(sSection, sValue, ref val);

        }
        internal void FireBeforeSet(String sSection, String sValue, ref String val)
        {
            var copied = BeforeSetValue;
            if (copied != null)
                copied.Invoke(sSection, sValue, ref val);



        }


        #region Base Class (.NET) Framework 4 dynamic object support code
#if CS4

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {

            if (value is String)
            {
                this[binder.Name].Name = (String)value;
                return true;

            }
            else if (value is List<INIItem>)
            {
                INISection getsection = this[binder.Name];
                getsection.INIItems = (List<INIItem>)value;
                return true;

            }
            else
            {
                return false;

            }




        }



#endif
        #endregion

        public INIFile()
        {
            Sections = new List<INISection>();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<INISection> GetEnumerator()
        {
            return Sections.GetEnumerator();
        }

        ~INIFile()
        {
            if (AutoSave) Save();


        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public INIFile(String filename, bool WatchFS)
            : this(filename)
        {

            if (WatchFS)
            {
                String fullpath = Path.GetFullPath(filename);
                String ININame = Path.GetFileName(fullpath);
                String pathonly = fullpath.Substring(0, fullpath.Length - ININame.Length);

                //create a FileSystemWatcher to watch the fs for changes.
                fsw = new FileSystemWatcher(pathonly, ININame); //we only care about the INI File.

                fsw.Changed += fsw_Changed;


            }

        }

        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            //the file changed.
            var copyevent = FileEvent;
            if (copyevent != null) copyevent.Invoke(this);

        }



        //CS4 code here
        #region .NET Framework 4 Code- dynamic objects





        #endregion

        public INIFile(String filename)
            : base()
        {
            LoadINI(filename);
        }

        public bool HasSection(String Name, StringComparison comparisonmode = StringComparison.OrdinalIgnoreCase)
        {
            if (Sections == null) return false;
            return Sections.Any((e) => e.Name.Equals(Name, comparisonmode));
        }
        /// <summary>
        /// 'special' indexer, accepts multiple parameters. The first item that is found as a valid section is returned.
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public INISection this[params String[] indices]
        {
            get
            {

                foreach (var iterate in indices)
                {
                    //if the section exists, return it...
                    if (HasSection(iterate)) return this[iterate];
                }
                return this[indices.First()];
            }
            set
            {
                //only set first item.
                this[indices.First()] = value;
            }
        }

        //Indexer...
        public INISection this[String index]
        {
            get
            {
                if (Sections == null) Sections = new List<INISection>();
                if (!String.IsNullOrEmpty(CategoryPrefix)) index = CategoryPrefix + "." + index;
                INISection returnthis =
                    Sections.FirstOrDefault((w) => w.Name.Equals(index, StringComparison.OrdinalIgnoreCase));

                if (returnthis == null)
                {
                    if (!String.IsNullOrEmpty(CategoryPrefix)) index = CategoryPrefix + "." + index;
                    returnthis = new INISection(index, "", new List<INIItem>(), this);
                    Sections.Add(returnthis);
                }
                return returnthis;
            }
            set
            {
                //remove any existing value with the given name...
                INISection itemfound =
                    Sections.FirstOrDefault(w => w.Name.Equals(index, StringComparison.OrdinalIgnoreCase));
                if (itemfound != null) Sections.Remove(itemfound);
                Sections.Add(value);
            }
        }

        private static INIDataItem ParseINIValue(String valueline, INISection parentSection)
        {
            int equalspos = valueline.IndexOf('=');
            String valuename = "", valuedata = "";

            if (equalspos == -1) return null;

            valuename = valueline.Substring(0, equalspos).Trim();
            valuedata = valueline.Substring(equalspos + 1).Trim();

            return new INIDataItem(valuename, valuedata, parentSection);
        }

        public void LoadINI(String Filename)
        {
            _FileName = Filename;

            if (!File.Exists(Filename))
                return;

            using (var newreader = new StreamReader(File.OpenRead(Filename), true))
            {
                LoadINI(newreader);
                newreader.Close();
            }

        }

        public void LoadINI(String Filename, Encoding pEncoding)
        {
            using (var newreader = new StreamReader(File.OpenRead(Filename), pEncoding))
            {
                LoadINI(newreader);
                newreader.Close();
            }
        }
        private static readonly int MaxFrames = 500;
        private void ProcessMetacommand(String CommandName, String parameter)
        {
            try
            {
                CommandName = CommandName.ToUpper();
                if (CommandName == "INCLUDE")
                {

                    //if parameter is our filename...
                    //before we call this, get a StackTrace. we will define a maximum of 500 as the depth.
                    StackTrace st = new StackTrace();
                    if (st.FrameCount > MaxFrames)
                    {
                        Trace.WriteLine("Cyclical INCLUDE's detected in INI file.");
                        //return.
                        return;

                    }

                    INIFile subINIFile = new INIFile(parameter);
                    //add it's sections to our own... take "ownership" of them...
                    foreach (var section in subINIFile.Sections)
                    {
                        Sections.Add(section);
                        section.OwnerINI = subINIFile;



                    }


                }
            }
            catch (Exception exx)
            {
                Debug.Print("Exception processing metacommand");


            }

        }

        public void LoadINI(StreamReader fromstream)
        {

            String currentline = null;
            Sections = new List<INISection>();
            INISection globalsection = new INISection("cINIFilecsGlobals", "", new List<INIItem>(), this);
            INISection currentSection = globalsection;
            //while there is still text to read.

            while ((currentline = fromstream.ReadLine()) != null)
            {
                //trim the read in line...
                currentline = currentline.Trim();


                if (currentline.StartsWith("$"))
                {
                    //Special metacommand.
                    String Metacommand = currentline.Substring(1);

                    String CommandName = Metacommand.Substring(0, Metacommand.IndexOf(' '));
                    String parameter = Metacommand.Substring(CommandName.Length + 1);
                    if (parameter.StartsWith("\"") && parameter.EndsWith("\""))
                        parameter = parameter.Substring(1, parameter.Length - 2);

                    ProcessMetacommand(CommandName, parameter);


                }
                else if (currentline.StartsWith("["))
                {
                    //if it starts with a square bracket, it's a section.
                    //parse out the section name...

                    String newsectionname = currentline.Substring(1, currentline.IndexOf(']') - 1);
                    String eolComment = "";
                    if (currentline.IndexOf(';') > -1)
                        eolComment = currentline.Substring(currentline.IndexOf(';'));


                    //Added May 2012: What if we have duplicates of the same section? coalesce them :P.
                    //we first see if there is a section with this name.
                    var foundsection = Sections.Find((w) => w.Name.ToUpper() == newsectionname.ToUpper());
                    if (foundsection != null)
                    {
                        currentSection = foundsection; //set to the existing section.
                        //add to it's comment. why not.
                        currentSection.eolComment += eolComment.Substring(1);
                    }
                    else
                        currentSection = new INISection(newsectionname, eolComment, new List<INIItem>(), this);
                    Sections.Add(currentSection);
                }
                else if (currentline.StartsWith(";"))
                {
                    //add a new Comment INIItem to the current section.
                    INIItem newitem = new INIComment(currentline);
                    currentSection.INIItems.Add(newitem);
                }
                else
                {
                    INIDataItem createitem = ParseINIValue(currentline, currentSection);


                    if (createitem != null)
                    {
                        INIDataItem found = null;
                        //look for an existing item in the current section.
                        if (null != (found = (INIDataItem)(currentSection.INIItems.Find((t) =>
                            (t is INIDataItem && ((INIDataItem)t).Name.ToUpper() == createitem.Name.ToUpper())))))
                        {
                            //set the value of found.
                            found.Value = createitem.Value;

                        }
                        else
                        {
                            currentSection.INIItems.Add(createitem);
                            //createitem.OwnerSection=currentSection;
                        }
                    }
                }
            }
            if (globalsection.INIItems.Count() > 0)
                Sections.Add(globalsection);
        }

        public void SaveINI(String filename)
        {

            using (StreamWriter swriter = new StreamWriter(new FileStream(filename, FileMode.Create), Encoding.ASCII))
            {
                SaveINI(swriter);


                swriter.Close();
            }
        }


        public void SaveINI(String filename, Encoding pEncoding)
        {
            using (StreamWriter swriter = new StreamWriter(new FileStream(filename, FileMode.Truncate), pEncoding))
            {
                SaveINI(swriter);

                swriter.Close();
            }
        }

        public void SaveINI(StreamWriter tostream)
        {
            //save to the given stream.
            foreach (INISection loopsection in Sections)
            {
                //don't write out "[global]" for the global section, if present.
                if (!loopsection.Name.Equals("cINIFilecsGlobals", StringComparison.OrdinalIgnoreCase))
                {
                    tostream.Write("[" + loopsection.Name + "]");
                    if (loopsection.eolComment.Length > 0)
                        tostream.WriteLine("  " + loopsection.eolComment);
                    else
                        tostream.WriteLine();
                }
                foreach (INIItem itemloop in loopsection.INIItems)
                {
                    tostream.WriteLine(itemloop.ToString());
                }
            }
        }

        #region ISettingsStorage Members

        public void Save()
        {
            SaveINI(_FileName);

        }

        public void Load()
        {
            //nothing here either.
        }

        public void AddValue(string Category, string ValueName, string Value)
        {
            this[Category][ValueName].Value = Value;
        }

        public string GetValue(string Category, string ValueName)
        {
            return this[Category][ValueName].Value;
        }
        public IEnumerable<String> GetCategories()
        {

            return (from m in Sections select m.Name);


        }
        #endregion
    }





    #region Extra ISettingsStorage implementors





    #endregion




}