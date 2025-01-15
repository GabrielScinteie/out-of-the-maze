using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace out_of_the_maze
{
    public partial class Maze : Form
    {
        private CentralAgent _ownerAgent;
        private Bitmap _doubleBufferImage;
        private Boolean debug = false;
        public Cell[,] maze { get; set; }

        public Maze(CentralAgent agent)
        {
            SetOwner(agent);
            InitializeComponent();
            
/*            for (int r = 0; r < maze.GetLength(0); r++)
            {
                for (int c = 0; c < maze.GetLength(1); c++)
                {
                    // Print the cell's directional weights
                    Cell cell = maze[r, c];
                    Console.Write(cell);
                }
                Console.WriteLine();
            }*/
            
        }

        public void SetOwner(CentralAgent a)
        {
            _ownerAgent = a;
            maze = _ownerAgent.maze;
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            drawMaze();
        }

        private void pictureBox_Resize(object sender, EventArgs e)
        {
            drawMaze();
        }

        public void UpdateMazeGUI()
        {
            drawMaze();
        }

        private void drawMaze()
        {
            //Console.WriteLine("Draw Maze");
            int w = pictureBox.Width;
            int h = pictureBox.Height;

            if (_doubleBufferImage != null)
            {
                _doubleBufferImage.Dispose();
                GC.Collect(); // prevents memory leaks
            }

            if (w <= 0 || h <= 0)
            {
                Console.WriteLine("Error: PictureBox has invalid dimensions.");
                return;
            }
            _doubleBufferImage = new Bitmap(w, h);
            // Graphics g = Graphics.FromImage(_doubleBufferImage);


            using (Graphics g = Graphics.FromImage(_doubleBufferImage))
            {
                g.Clear(Color.White);

                int minXY = Math.Min(w, h);
                int cellSize = (minXY - 40) / Utils.Size;

                // Draw maze walls

                if (_ownerAgent != null)
                {
                    foreach (var kvp in _ownerAgent.ExplorerPositions)
                    {
                        string agentKey = kvp.Key;        // The agent's identifier (e.g., "Explorer 2")
                        string position = kvp.Value;     // The agent's position (e.g., "0 0")

                        string[] t = position.Split();
                        int x = Convert.ToInt32(t[0]);
                        int y = Convert.ToInt32(t[1]);

                        // Extract the number from the key (e.g., "2" from "Explorer 2")
                        string agentNumber = agentKey.Split(' ').Last();

                        // Draw the agent as a blue circle
                        g.FillEllipse(Brushes.Blue, 20 + y * cellSize + 6, 20 + x * cellSize + 6, cellSize - 12, cellSize - 12);

                        // Draw the agent's number inside the circle
                        var font = new Font("Arial", 12); // Font for the text
                        var brush = Brushes.White;        // Text color
                        var format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };

                        // Calculate the center of the circle
                        int centerX = 20 + y * cellSize + cellSize / 2;
                        int centerY = 20 + x * cellSize + cellSize / 2;

                        // Draw the agent's number
                        g.DrawString(agentNumber, font, brush, new PointF(centerX, centerY), format);
                    }
                }

                for (int r = 0; r < Utils.Size; r++)
                {
                    for (int c = 0; c < Utils.Size; c++)
                    {
                        // Get the current cell
                        Cell cell = maze[r, c];

                        int x = 20 + c * cellSize;
                        int y = 20 + r * cellSize;

                        // Draw the top wall
                        if (cell.Up <= 0)
                            g.DrawLine(Pens.Black, x, y, x + cellSize, y);

                        // Draw the bottom wall
                        if (cell.Down <= 0)
                            g.DrawLine(Pens.Black, x, y + cellSize, x + cellSize, y + cellSize);

                        // Draw the left wall
                        if (cell.Left <= 0)
                            g.DrawLine(Pens.Black, x, y, x, y + cellSize);

                        // Draw the right wall
                        if (cell.Right <= 0)
                            g.DrawLine(Pens.Black, x + cellSize, y, x + cellSize, y + cellSize);

                        // Draw an 'X' if the cell is an exit
                        if (cell.Exit)
                        {
                            g.DrawLine(Pens.Red, x + 5, y + 5, x + cellSize - 5, y + cellSize - 5);
                            g.DrawLine(Pens.Red, x + cellSize - 5, y + 5, x + 5, y + cellSize - 5);
                        }

                        var font = new Font("Arial", 8);
                        var brush = Brushes.Black;
                        var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                        if(debug == true)
                            if(r % 2 < 2 && c % 2 < 2)
                            {
                                // Top weight (closer to the center)
                                g.DrawString(cell.Up.ToString("F2"), font, brush,
                                    new PointF(x + cellSize / 2, y + cellSize / 4), format);

                                // Bottom weight (closer to the center)
                                g.DrawString(cell.Down.ToString("F2"), font, brush,
                                    new PointF(x + cellSize / 2, y + (3 * cellSize) / 4), format);

                                // Left weight (closer to the center)
                                g.DrawString(cell.Left.ToString("F2"), font, brush,
                                    new PointF(x + cellSize / 4, y + cellSize / 2), format);

                                // Right weight (closer to the center)
                                g.DrawString(cell.Right.ToString("F2"), font, brush,
                                    new PointF(x + (3 * cellSize) / 4, y + cellSize / 2), format);

                            }


                    }
                }


                



                Graphics pbg = pictureBox.CreateGraphics();
                pbg.DrawImage(_doubleBufferImage, 0, 0);
            }
                
        }
    }
}
