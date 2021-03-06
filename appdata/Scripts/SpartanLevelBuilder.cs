using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using BASeBlock;
using BASeBlock.Blocks;
using bcHighScores;
using BASeBlock.Particles;
namespace SpartanLevel
{

	public class SpartanLevelBuilder : BaseLevelBuilder
	{
		private static Random mRandom = BASeBlock.BCBlockGameState.rgen;
		private IHighScoreList _SetHighScores = BASeBlock.BCBlockGameState.Scoreman[typeof(SpartanLevelBuilder).Name];
		
		private Color RandomColor()
		{
		 return Color.FromArgb((int)(mRandom.NextDouble()*128+128), (int)(mRandom.NextDouble()*255),
		 (int)(mRandom.NextDouble()*255),
		 (int)(mRandom.NextDouble()*255));
		
		}
		
		private cLevel CreateDefaultLevel(int Levelnumber,RectangleF PicGame)
		{
			cLevel returnlevel = new cLevel();
			returnlevel.Background.Backgroundframekeys = new string[] { "mainbg" };
           	returnlevel.Background.rotatespeed = 0;
            returnlevel.Background.MoveVelocity=new PointF(0,0);


			returnlevel.levelballs.Add(new cBall(new PointF(PicGame.Width / 2,PicGame.Height - 50), new PointF(-2f,-2f)));
			returnlevel.LevelName = "Level #" + Levelnumber.ToString();
			returnlevel.MusicName="ZAARK|COMMANDO|BASESTOMP";
			
			for(int x = 0;x < PicGame.Width-33; x+=33)
			{
				
				for(int y=0; y < PicGame.Height /3; y+=16)
				{
				Block createdblock = new NormalBlock(new RectangleF(x,y,33,16), 
					new SolidBrush(RandomColor()),
					new Pen(Color.Black,1));
				//createdblock.BlockEffects.Add(new ParticleEmanationEffect() {SpawnType=typeof(AnimatedImageParticle)});
				//createdblock.BlockEffects.Add(new LinearGlintEffect());
				returnlevel.levelblocks.Add(createdblock);
				
				}
			
			
			}
		
		  return returnlevel;
		}
		
		public LevelSet CreateDefaultLevelSet(RectangleF Targetrect)
		{

			LevelSet createme = new LevelSet();
			createme.SetName = "Spartan Levels";
			const int numlevels = 10;
			
			for(int i=1;i<numlevels;i++)
			{
			    createme.Levels.Add(CreateDefaultLevel(i,Targetrect));
			
			
			}
			
		
		 return createme;
		}
		
		
		#region iLevelSetBuilder Members
		public override string getName()
		{
		return "Spartan Set";
		
		}
		public override string getDescription()
		{
		return "Spartan Level Builder.";
		
		}
		
		public override  LevelSet BuildLevelSet(RectangleF targetrect,IWin32Window Owner)
		{
		return CreateDefaultLevelSet(targetrect);
		
		}
		
		
		
		
		#endregion
		
	}
	
}