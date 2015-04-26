using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace BASeBlock
{
    /// <summary>
    /// interface implemented by explodable things.
    /// </summary>
    public interface iImagable 
    {
        void Draw(Graphics g);
        Size Size { get; set; }
        Point Location { get; set; }
        Rectangle getRectangle();


      

    }
}
