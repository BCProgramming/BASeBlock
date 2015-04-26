// Type: DragonOgg.MediaPlayer.OggFile
// Assembly: DragonOgg, Version=1.0.4123.19930, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\BC_Programming\Documents\Visual Studio 2008\Projects\BASeBlocks\Poing2\bin\Debug\DragonOgg.dll

using System;

namespace DragonOgg.MediaPlayer
{
    public class OggFile : IDisposable
    {
        public OggFile(string Filename);
        public OpenTK.Audio.OpenAL.ALFormat Format { get; }

        #region IDisposable Members

        public void Dispose();

        #endregion

        public string GetQuickTag(OggTags TagID);
        public OggTagWriteCommandReturn SetQuickTag(OggTags TagID, string Value);
        public OggTag GetTag(string TagName);
        public OggTag[] GetTags();
        public OggTagWriteCommandReturn SetTag(OggTag Tag);
        public OggTagWriteCommandReturn SetTags(OggTag[] Tags);
        public OggTagWriteCommandReturn SetTags(OggTag[] Tags, bool AbortOnError);
        public OggTagWriteCommandReturn RemoveTag(string TagName);
        public OggTagWriteCommandReturn RemoveTag(OggTag Tag);
        public OggTagWriteCommandReturn RemoveAllTags();
        public OggBufferSegment GetBufferSegment(int SegmentLength);
        public void ResetFile();
        public OggPlayerCommandReturn SeekToTime(float Seconds);
        public float GetTime();
    }
}
