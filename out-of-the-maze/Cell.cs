using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace out_of_the_maze
{
    public class Cell
    {
        // Directional weights for the cell
        public double Up { get; set; }
        public double Down { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }

        public Boolean Exit { get; set; }


        public Cell()
        {
            // Initialize all directions as walls (weight 0)
            Up = Down = Left = Right = 0;
        }

        public override string ToString()
        {
            return $"({Up},{Down},{Left},{Right})";
        }
    }

}
