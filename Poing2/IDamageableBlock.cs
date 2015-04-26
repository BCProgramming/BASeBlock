using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BASeBlock.Blocks
{
    public interface IDamageableBlock
    {
        /// <summary>
        /// Total amount of Damage this block has received.
        /// </summary>
        int Damage { get; set; }
        /// <summary>
        /// Total health of this block.
        /// </summary>
        int Health { get; set; }
    }
}
