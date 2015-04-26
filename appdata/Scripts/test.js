//test javascript for BASeBlock
//and basic "sample"...
//this sample flashes the block as it is hit.
var ourblock=self;
var usepen = System.Drawing.SystemPens.Highlight;
//the GetName() function is called to determine the name of the block as it appears in 
//the Editor Drop-downs.
var GetName = function(){
   return "JavaScript Test Block";
   

}
//called after the script has been loaded, before it has been associated with a block.
var scriptload= function(datahook){

    datahook.ShowMessage("test.js has initialized!");


}

//draw. g is a .NET System.Graphics object.
var draw=function(g){

//usepen = new System.Drawing.Pen(usecolor,2);

if(ourblock!=null)
{
g.DrawRectangle(usepen, 
self.BlockRectangle.Left,self.BlockRectangle.Top,
self.BlockRectangle.Width,self.BlockRectangle.Height);
}

}
//public override bool 
//PerformBlockHit(BCBlockGameState parentstate, cBall ballhit, ref List<cBall> ballsadded)
var PerformBlockHit= function(parentstate,ballhit,ballsadded){
	if(usepen==System.Drawing.SystemPens.Highlight)
	    {
	    usepen=System.Drawing.SystemPens.ButtonShadow;
	    
	    }
	else
	{
	usepen==System.Drawing.SystemPens.Highlight
	}
	//also set the Redraw value.
	self.Redraw=true;
	return false;
}
var MustDestroy=function(){
//return true to indicate the block must be destroyed to finish the level.
//this is also sometimes used by various parts of the game to decide whether to treat a block as "invincible" or not.
return false; 

}

