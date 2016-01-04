using System;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

using System.Runtime.Serialization.Formatters.Binary;

using System.Text;


namespace BASeCamp.BASeBlock
{
    class SerializationHelper
    {
        private static IFormatter GetFormatter(Object forobj)
        {
            Type t = forobj.GetType();
            //retrieve the getFOrmatter Method of BCBlockGameState.
            MethodInfo grabmethod = typeof(BCBlockGameState).GetMethod("GetFormatter");
            MethodInfo callit = grabmethod.MakeGenericMethod(t);
            return callit.Invoke(null, new object[] { false }) as IFormatter;
        }
        public static byte[] Serialize(Object o)
        {
            MemoryStream stream = new MemoryStream();

            //grab the type of the Object...
            IFormatter formatter = GetFormatter(o);

            //IFormatter formatter = BCBlockGameState.getFormatter();
            if (formatter == null) throw new SerializationException("Formatter was null...");
            formatter.Serialize(stream, o);
            return stream.ToArray();

        }
        public static T BinaryDeSerialize<T>(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            IFormatter formatter = BCBlockGameState.getFormatter<T>(BCBlockGameState.DataSaveFormats.Format_Binary);
         
            Object obj = (Object)formatter.Deserialize(stream);
            return (T)obj;

        }
        internal sealed class VersionConfigToNamespaceAssemblyObjectBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                
                try
                {
                    string BaseAssemblyName = assemblyName.Split(',')[0];
                    Assembly[] Assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly loopassembly in Assemblies)
                    {
                        if (loopassembly.FullName.Split(',')[0].Equals(BaseAssemblyName,StringComparison.OrdinalIgnoreCase))
                        {
                            Debug.Print("SerializationBinder found " + loopassembly.FullName);
                            return loopassembly.GetType(typeName);
                            
                        }
                    }
                }
                catch (System.Exception exception)
                {
                    throw exception;
                } return null;
            }
        }

    }
}
