/*
 * BASeCamp BASeBlock
Copyright (c) 2011, Michael Burgwin
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    Neither the name of BASeCamp Corporation nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BASeBlock;
using BASeBlock.Blocks;

namespace BASeBlock
{

    /// <summary>
    /// Interface implemented by "Block Filters"; these basically perform some task on the Blocks.
    /// </summary>
    /// 
    
    public interface IFilterDataObject : ICloneable,ISerializable
    {
        //no methods... yet...

    }

    public interface iEditBlockFilter 
    {

        Type GetFilterDataType();
        void PerformFilter(Rectangle Area, ref List<Block> OnBlocks, ref List<cBall> OnBalls, IFilterDataObject DataObject);
        String Name { get; }
        String Description { get; }






    }
    public class BlockTypeAggregateFilter : BaseBlockListFilter
    {
        //filters by block type. Balls are not filtered.
        [Editor(typeof(ItemTypeEditor<iEditBlockFilter>),typeof(UITypeEditor))]
        public Type FilterBlockType = typeof(StrongBlock);
        public bool ExcludeType = true;

        public override IEnumerable<Block> Filter(IEnumerable<Block> source)
        {
            return source.Where(blockiterate => (source.GetType() == FilterBlockType) == ExcludeType);
        }

        public override IEnumerable<cBall> Filter(IEnumerable<cBall> source)
        {
            return source;
        }
    }


    public abstract class BaseBlockListFilter : IFilteredEnumerator<Block>,IFilteredEnumerator<cBall>

    {

        public IEnumerable<Block> FilterBlocks(IEnumerable<Block> source)
        {
            return Filter(source);
        }
        public IEnumerable<cBall> FilterBalls(IEnumerable<cBall> source)
        {
            return Filter(source);
        }

        public abstract IEnumerable<Block> Filter(IEnumerable<Block> source);
        public abstract IEnumerable<cBall> Filter(IEnumerable<cBall> source);
    }

    [Serializable]
    public class BlockFilterAggregate : BaseBlockFilter
    {
        [Serializable]
        public class BlockFilterAggregateData : IFilterDataObject
        {

            public Object Clone()
            {
                return new BlockFilterAggregateData(this);

            }
            
            private Type _BlockFilterAggregateType = null;
            public BlockTypeAggregateFilter Aggregate { get; set; }
            /// <summary>
            /// A BlockTypeAggregateFilter Type. This is used to filter the Blocks and balls before passing to a constructed instance of CallFiltered.
            /// 
            /// </summary>
            [Editor(typeof(ItemTypeEditor<BlockTypeAggregateFilter>), typeof(UITypeEditor))]
            public Type BlockFilterAggregateType {
                get { return _BlockFilterAggregateType; }
                set { _BlockFilterAggregateType = value;
                    if(value!=null) Aggregate = Activator.CreateInstance(value) as BlockTypeAggregateFilter;
                }
            }
            
            private Type _PreFilterCall = null;
            /// <summary>
            /// iEditBlockFilter Type. This instance will be called before the filter, with whatever the block and ball listings are that 
            /// was passed to this Filter.
            /// </summary>
            [Editor(typeof(ItemTypeEditor<iEditBlockFilter>), typeof(UITypeEditor))]
            public Type PreFilterCall {
                get { return _PreFilterCall; }
                set { _PreFilterCall = value;
                PreFilteredData = Activator.CreateInstance(_PreFilterCall) as IFilterDataObject;
                
                }
            }
            private Type _CallFiltered = null;
            /// <summary>
            /// iEditBlockFilter type. This instance will be instantiated and called after filters are run.
            /// </summary>
            
            [Editor(typeof(ItemTypeEditor<iEditBlockFilter>), typeof(UITypeEditor))]
            public Type CallFiltered 
            { 
                get { return _CallFiltered; } 
                set { _CallFiltered = value;
                FilteredData = Activator.CreateInstance(_CallFiltered) as IFilterDataObject;
                } 
            }

            /// <summary>
            /// Data for the Filtered iEditBlockFilter call.
            /// </summary>
            public IFilterDataObject FilteredData { get; private set; }
            /// <summary>
            /// Data for the pre-Filtered iEditBlockFilter call.
            /// </summary>
            public IFilterDataObject PreFilteredData { get; private set; }

            public BlockFilterAggregateData() { }
            public BlockFilterAggregateData(BlockFilterAggregateData clonethis)
            {
                BlockFilterAggregateType = clonethis.BlockFilterAggregateType;
                PreFilterCall = clonethis.PreFilterCall;
                CallFiltered = clonethis.CallFiltered;


            }
            public BlockFilterAggregateData(SerializationInfo info, StreamingContext context)
            {
                BlockFilterAggregateType = info.GetValueType("AggregateType");
                PreFilterCall = info.GetValueType("PreFilterCall");
                CallFiltered = info.GetValueType("FilteredCall");
            }
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValueType("AggregateType", BlockFilterAggregateType);
                info.AddValueType("PreFilterCall", PreFilterCall);
                info.AddValueType("FilteredCall", CallFiltered);

            }

            

        }
        public BlockFilterAggregate():base("Filtered Aggregate","Runs given filter, then filters the objects and calls another.")
        {


        }

        
        public override Type GetFilterDataType()
        {
            return typeof(BlockFilterAggregateData);
        }
        public override void PerformFilter(Rectangle Area, ref List<Block> OnBlocks, ref List<cBall> OnBalls, IFilterDataObject DataObject)
        {
            var dataobj = DataObject as BlockFilterAggregateData;
            iEditBlockFilter preFilterObj =null;
            if (dataobj.PreFilterCall != null)
            {
                preFilterObj = Activator.CreateInstance(dataobj.PreFilterCall) as iEditBlockFilter;
                if (preFilterObj != null)
                    preFilterObj.PerformFilter(Area, ref OnBlocks, ref OnBalls, dataobj.PreFilteredData);
            }
            //call aggregate filter class.
            OnBlocks = dataobj.Aggregate.Filter(OnBlocks).ToList();
            OnBalls = dataobj.Aggregate.Filter(OnBalls).ToList();

            //call post filter.
            if (dataobj.CallFiltered != null)
            {
                iEditBlockFilter postFilterObj = Activator.CreateInstance(dataobj.CallFiltered) as iEditBlockFilter;
                if(postFilterObj!=null)
                    postFilterObj.PerformFilter(Area, ref OnBlocks, ref OnBalls, dataobj.FilteredData);
            }
            
            
        }
    }


    [Serializable]
    public abstract class BaseBlockFilter : iEditBlockFilter
    {

        public abstract Type GetFilterDataType();
        public abstract void PerformFilter(Rectangle Area, ref List<Block> OnBlocks, ref List<cBall> OnBalls, IFilterDataObject DataObject);
        protected String _Name;
        protected String _Description;
        public string Name { get { return _Name; } }
        public string Description { get { return _Description; } }

        protected BaseBlockFilter(String pName, String pDescription)
        {
            _Name = pName;
           _Description = pDescription;
        }
        protected BaseBlockFilter(BaseBlockFilter clonethis)
        {
            _Name = clonethis.Name;
            _Description = clonethis.Description;
        }
        protected BaseBlockFilter(SerializationInfo info, StreamingContext context)
        {
            _Name = info.GetString("Name");
            _Description = info.GetString("Description");

        }
     

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", _Name);
            info.AddValue("Description", _Description);


        }
        


    }
    



    public class BasicGradientColourizer : BaseBlockFilter 
    {
        [Serializable]
        public class GradientColourizerData : IFilterDataObject
        {

            public enum GradientStyleConstants
            {
                Gradient_Horizontal,
                Gradient_Vertical



            }
            public GradientStyleConstants GradientStyle { get; set; }
            public Color _FillColor1 = Color.Red;
            public Color _FillColor2 = Color.Blue;
            public Color _PenColor1 = Color.Green;
            public Color _PenColor2 = Color.Yellow;


            public Color FillColor1
            {
                get { return _FillColor1; }
                set { _FillColor1 = value; }
            }
            public Color FillColor2
            {
                get { return _FillColor2; }
                set { _FillColor2 = value; }
            }
            public Color PenColor1
            {
                get { return _PenColor1; }
                set { _PenColor1 = value; }
            }
            public Color PenColor2
            {
                get { return _PenColor2; }
                set { _PenColor2 = value; }
            }


            public GradientColourizerData()
            {
                Debug.Print("GradientColourizerData Constructor");
            }
            public GradientColourizerData(SerializationInfo info, StreamingContext context)
            {
                FillColor1 = (Color)info.GetValue("FillColor1", typeof(Color));
                FillColor2 = (Color)info.GetValue("FillColor2", typeof(Color));
                PenColor1 = (Color)info.GetValue("PenColor1", typeof(Color));
                PenColor2 = (Color)info.GetValue("PenColor2", typeof(Color));


            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("FillColor1", FillColor1);
                info.AddValue("FillColor2", FillColor2);
                info.AddValue("PenColor1", PenColor1);
                info.AddValue("PenColor2", PenColor2);

            }
            public GradientColourizerData(GradientColourizerData clonethis)
            {
                FillColor1 = clonethis.FillColor1;
                FillColor2 = clonethis.FillColor2;
                PenColor1 = clonethis.PenColor1;
                PenColor2 = clonethis.PenColor2;

            }

            #region ICloneable Members

            public object Clone()
            {
                return new GradientColourizerData(this);
            }

            #endregion
        }



        #region iEditBlockFilter Members

        
        public BasicGradientColourizer():base("Basic Gradient","Basic Gradient Block Colourizer")
        {

        }
        public override Type GetFilterDataType()
        {
            return typeof(GradientColourizerData);
        }
        private Color CalcGradient(Color FirstColor, Color SecondColor, float percent)
        {
            int[] firstcp = new int[] { FirstColor.R, FirstColor.G, FirstColor.B };
            int[] secondcp = new int[] { SecondColor.R, SecondColor.G, SecondColor.B };

            BCBlockGameState.ClampValue(percent, 0, 1);
            float[] mixed = new float[]
            {
                FirstColor.R + (((float)(SecondColor.R-FirstColor.R))*percent),
                FirstColor.G + (((float)(SecondColor.G-FirstColor.G))*percent),
                FirstColor.B + (((float)(SecondColor.B-FirstColor.B))*percent)


            };
            return Color.FromArgb((int)mixed[0], (int)mixed[1], (int)mixed[2]);


        }

        public void CalculateColor(RectangleF extents, NormalBlock forblock, GradientColourizerData data)
        {
            //get x and y percentage within the extents.
            float xpercent = Math.Min(1f,(forblock.CenterPoint().X-extents.Left) / extents.Width);
            float ypercent = Math.Min(1f,(forblock.CenterPoint().Y-extents.Left) / extents.Height);
            float usepercent = 0;

            if (data.GradientStyle == GradientColourizerData.GradientStyleConstants.Gradient_Horizontal)
                usepercent = xpercent;
            else if (data.GradientStyle == GradientColourizerData.GradientStyleConstants.Gradient_Vertical)
                usepercent = ypercent;
            //get the RGB values of the colours.
            Color usefillcolor = CalcGradient(data.FillColor1, data.FillColor2, usepercent);
            Color usepencolor = CalcGradient(data.PenColor1, data.PenColor2, usepercent);


            forblock.BlockColor = usefillcolor;
            forblock.PenColor = usepencolor;




        }

        public override void PerformFilter(Rectangle Area, ref List<Block> OnBlocks, ref List<cBall> OnBalls, IFilterDataObject DataObject)
        {
            //throw new NotImplementedException();

            //step one get the extents of the blocks...
            RectangleF blockextents = BCBlockGameState.GetBlockRect(OnBlocks);

            foreach (NormalBlock loopnormal in (from n in OnBlocks where n.GetType() == typeof(NormalBlock) select (NormalBlock)n))
            {


                CalculateColor(blockextents, loopnormal, (GradientColourizerData)DataObject);

            }


        }

        

       
        #endregion
    }

    public class RandomizerFilter : BaseBlockFilter
    {
        [Serializable]
        public class RandomizerFilterData : IFilterDataObject
        {

            [Flags]
            public enum FilterRandomizationType
            {
                Blocks = 1,
                Balls = 2,
                Both = Blocks | Balls


            }
            private bool _AllowIntersection = false;
            private FilterRandomizationType RandomizationType = FilterRandomizationType.Blocks;
            public bool AllowIntersection { get { return _AllowIntersection; } set { _AllowIntersection = value; } }

            public RandomizerFilterData()
            {

            }
            public RandomizerFilterData(RandomizerFilterData clonethis)
            {
                AllowIntersection=clonethis.AllowIntersection;
                RandomizationType = clonethis.RandomizationType;

            }
            public Object Clone()
            {
                return new RandomizerFilterData(this);

            }
            public RandomizerFilterData(SerializationInfo info, StreamingContext context)
            {


            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {


            }


        }



        public RandomizerFilter():base("Randomizer Filter","Object Location Randomizer")
        {



        }
        public override Type GetFilterDataType()
        {
            return typeof(RandomizerFilterData);
        }

        public override void PerformFilter(Rectangle Area, ref List<Block> OnBlocks, ref List<cBall> OnBalls, IFilterDataObject DataObject)
        {
            //randomize!

            RandomizerFilterData rfd = DataObject as RandomizerFilterData;
            if (rfd == null) return;

            List<Block> movedblocks = new List<Block>();
            foreach (Block iterate in OnBlocks)
            {
                int maxcount = 50;
                
                //move it...
                Rectangle userect = new Rectangle(-10, -10, 12, 12);
                while (!Area.Contains(userect)
                    && !(rfd.AllowIntersection && movedblocks.Any((b)=>b.GetPoly().IntersectsWith(iterate.GetPoly()))))
                {
                    maxcount--;
                    if (maxcount < 0) break;
                    float xcoord = (float)(BCBlockGameState.rgen.NextDouble() * Area.Width);
                    float ycoord = (float)(BCBlockGameState.rgen.NextDouble() * Area.Height);
                     userect = new Rectangle((int)xcoord, (int)ycoord, (int)iterate.BlockRectangle.Width,
                        (int)iterate.BlockRectangle.Height);

                     iterate.BlockLocation = new PointF(xcoord, ycoord);

                }



            }

        }

       
    }

    public class BlockReplacer : BaseBlockFilter
    {
        public class BlockReplacerData :IFilterDataObject ,ICloneable 
        {

            public Type SearchForType { get; private set; }
            public Type ReplaceWithType { get; private set; }


            public BlockReplacerData()
            {

            }
            public BlockReplacerData(BlockReplacerData copyfrom)
            {
                SearchFor = copyfrom.SearchFor;
                ReplaceWith = copyfrom.ReplaceWith;


            }
            public BlockReplacerData(SerializationInfo info, StreamingContext context)
            {
                SearchFor = info.GetString("SearchFor");
                ReplaceWith = info.GetString("ReplaceWith");

            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("SearchFor", SearchFor);
                info.AddValue("ReplaceWith", ReplaceWith);
            }
            public Object Clone()
            {
                return new BlockReplacerData(this);

            }
            /// <summary>
            /// Type of block to search for
            /// </summary>
            [Editor(typeof(BlockTypeStringEditor), typeof(UITypeEditor))]
            public String SearchFor
            {
                get { return SearchForType==null?"null":SearchForType.Name; }
                set
                {
                    SearchForType = BCBlockGameState.FindClass(value);

                }
            }
            /// <summary>
            /// type of block to replace with
            /// </summary>
            [Editor(typeof(BlockTypeStringEditor), typeof(UITypeEditor))]
            public String ReplaceWith
            {
                get { return ReplaceWithType == null ? "null" : ReplaceWithType.Name; }
                set
                {
                    ReplaceWithType = BCBlockGameState.FindClass(value);
                }

            }



        }

        public BlockReplacer():base("Block Replacer","Replaces one Block Type with another")
    {

    }

        public override Type GetFilterDataType()
        {
            return typeof(BlockReplacerData);
        }
        public override void PerformFilter(Rectangle Area, ref List<Block> OnBlocks, ref List<cBall> OnBalls, IFilterDataObject DataObject)
        {
            //
            //blocks to be removed.
            List<Block> blockstoremove = new List<Block>();
            List<Block> blockstoadd = new List<Block>();
            BlockReplacerData rpd = (BlockReplacerData)DataObject;

            foreach (Block iterateblock in OnBlocks)
            {
                if (iterateblock.GetType() == rpd.SearchForType)
                {
                    blockstoremove.Add(iterateblock);

                    blockstoadd.Add((Block)Activator.CreateInstance(rpd.ReplaceWithType, iterateblock.BlockRectangle));



                }



            }

            foreach (var loopremove in blockstoremove)
            {
                OnBlocks.Remove(loopremove);

            }
            foreach (var loopadd in blockstoadd)
            {
                OnBlocks.Add(loopadd);

            }



        }

      
    }





    public class MakeColourTest : BaseBlockFilter
    {
        public class ColourTestData : IFilterDataObject
        {
            /// <summary>
            /// Fill Colour Of the block.
            /// </summary>
            public Color UseFillColour { get; set; }
            /// <summary>
            /// Pen (outline) colour of the block.
            /// </summary>
            public Color usePenColour { get; set; }

            
            public ColourTestData()
            {


            }

            public ColourTestData(Color useFill, Color usePen)
            {
                UseFillColour = useFill;
                usePenColour = usePen;

            }

            public ColourTestData(SerializationInfo info, StreamingContext context)
            {
                UseFillColour = (Color)info.GetValue("Fill", typeof(Color));
                usePenColour = (Color)info.GetValue("Pen", typeof(Color));

            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("Fill", UseFillColour, typeof(Color));
                info.AddValue("Pen", usePenColour, typeof(Color));

            }




            public object Clone()
            {
                return new ColourTestData(UseFillColour, usePenColour);
            }


        }
        


        #region iEditBlockFilter Members

        public MakeColourTest():base("Test Colourizer","Colourizes NormalBlocks")
        {
        }
        public override Type GetFilterDataType()
        {
            return typeof(ColourTestData);
        }

        public override void PerformFilter(Rectangle Area, ref List<Block> OnBlocks, ref List<cBall> OnBalls, IFilterDataObject DataObject)
        {
            ColourTestData usedata = (ColourTestData)DataObject;


            foreach (Block loopblock in (from q in OnBlocks where q.GetType() == typeof(NormalBlock) select q))
            {
                NormalBlock useblock = (NormalBlock)loopblock;
                useblock.BlockColor = usedata.UseFillColour;
                useblock.PenColor = usedata.usePenColour;



            }


        }

      
        #endregion
    }

}