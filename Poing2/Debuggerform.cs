using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BASeCamp.BASeBlock.Blocks;
using BASeCamp.BASeBlock.Controls;

namespace BASeCamp.BASeBlock
{
    public partial class Debuggerform : Form
    {
        public class DebuggerObject
        {
            [TrackBarEditor.TrackBarEditorData(0,200)]
            [Editor(typeof(TrackBarEditor), typeof(UITypeEditor))]
            public float FloatValue { get; set; }



        }
        public Debuggerform()
        {
            InitializeComponent();
        }
        DebuggerObject objuse = new DebuggerObject();
        private void Debuggerform_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = objuse;
            var blocktsitem = new BlockToolStripButton();
            
            blocktsitem.BlockSelected += new EventHandler<BlockToolStripButton.BlockToolStripButtonBlockSelectedArgs>(blocktsitem_BlockSelected);
            toolStrip1.Items.Add(blocktsitem);
        }

        void blocktsitem_BlockSelected(object sender, BlockToolStripButton.BlockToolStripButtonBlockSelectedArgs e)
        {
            propertyGrid1.SelectedObject = e.SelectedBlock;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PictureBox usebox = Pictestellipse;

            cBall ballhit = new cBall(new PointF(100, 100), new PointF(2, 7));
            EllipseBlock eb = new EllipseBlock(new RectangleF(90, 105, 32, 16));

            var Ballpoly = ballhit.GetBallPoly();
            var EllipsePoly = eb.GetPoly();
            /*
                   Polygon ballpoly = ballhit.GetBallPoly();
            Vector Adjustment = new Vector();
            GeometryHelper.PolygonCollisionResult pcr = GeometryHelper.PolygonCollision(EllipsePoly, ballpoly, new Vector(ballhit.Velocity.X,ballhit.Velocity.Y));
            
            //minimumtranslationvector will be the normal we want to mirror the ball speed through.
            ballhit.Velocity = ballhit.Velocity.Mirror(pcr.MinimumTranslationVector);
            ballhit.Velocity = new PointF(-ballhit.Velocity.X, -ballhit.Velocity.Y);
            ballhit.Location = new PointF(ballhit.Location.X + Adjustment.X, ballhit.Location.Y + Adjustment.Y);
              
             * */

            GeometryHelper.PolygonCollisionResult pcr = 
                GeometryHelper.PolygonCollision(EllipsePoly, Ballpoly, new Vector(ballhit.Velocity.X, ballhit.Velocity.Y));
            PointF reflectedVelocity = ballhit.Velocity;
            
            //only if an intersection occurs or will occur.
            if (pcr.Intersect || pcr.WillIntersect)
            {
                //minimumtranslationvector should work to be used as the normal to reflect across.

                



            }


            //calculate Normal of ellipse at that position. 

            


            var addevent = new PaintEventHandler((g, pea) =>
            {
                ballhit.Draw(pea.Graphics);
                eb.Draw(pea.Graphics);



            });
            usebox.Paint += addevent;

            //force update.
            usebox.Invalidate();
            usebox.Update();





        }
    }
}
