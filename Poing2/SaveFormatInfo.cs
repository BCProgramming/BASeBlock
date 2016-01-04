using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BASeCamp.BASeBlock
{
    public interface ISaveFormatInfo
    {
        String[] GetSupportedExtensions();
        IFormatter getFormatter(Type t);
    }
    public abstract class SaveFormatInfo:ISaveFormatInfo
    {
        public static Type[] AllFormats = new Type[] { typeof(BinarySaveFormatInfo), typeof(XmlSaveFormatInfo) };

        
        public abstract String[] GetSupportedExtensions();
        public abstract IFormatter getFormatter(Type t);
        public bool SupportsFile(String sFileName)
        {
            String sExtension = Path.GetExtension(sFileName);
            return (GetSupportedExtensions() ?? new String[] { }).Any((w) => w.Equals(sExtension));
        }
        
    }
    public class BinarySaveFormatInfo:SaveFormatInfo
    {
        public EditorSet.CompressionTypeConstants CompressionType { get; set; }
        
        public override string[] GetSupportedExtensions()
        {
            return new string[] { ".BLF" } ;
        }

        public override IFormatter getFormatter(Type t)
        {
            return BCBlockGameState.getFormatter(t,BCBlockGameState.DataSaveFormats.Format_Binary);

        }
    }
    public class XmlSaveFormatInfo:SaveFormatInfo
    {
        public override string[] GetSupportedExtensions()
        {
            return new string[]{".BLFX"};
        }
        public override IFormatter getFormatter(Type t)
        {
            return BCBlockGameState.getFormatter(t, BCBlockGameState.DataSaveFormats.Format_XML);
        }
    }
}
