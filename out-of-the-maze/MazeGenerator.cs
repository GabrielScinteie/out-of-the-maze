using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace out_of_the_maze
{
    using System;
    using System.Collections.Generic;

    public class MazeGenerator
    {
        public Cell[,] GenerateMaze(int rows, int cols)
        {
            var rand = Utils.random;

            // Initialize the maze grid with Cell objects
            var maze = new Cell[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    maze[r, c] = new Cell();
                }
            }

            // Directions: {row offset, col offset, direction to current, direction to neighbor}
            var directions = new[]
            {
                (-1, 0, "Up", "Down"),   // North
                (1, 0, "Down", "Up"),   // South
                (0, -1, "Left", "Right"), // West
                (0, 1, "Right", "Left")  // East
            };

            // Visited cells
            var visited = new bool[rows, cols];

            // Start with a random cell
            int startRow = rand.Next(rows);
            int startCol = rand.Next(cols);
            visited[startRow, startCol] = true;

            // List of walls (edges)
            var walls = new List<(int row, int col, int dr, int dc, string current, string neighbor)>();
            foreach (var (dr, dc, current, neighbor) in directions)
            {
                int nr = startRow + dr, nc = startCol + dc;
                if (IsInBounds(nr, nc, rows, cols))
                {
                    walls.Add((startRow, startCol, dr, dc, current, neighbor));
                }
            }

            // Process walls
            while (walls.Count > 0)
            {
                // Pick a random wall
                var index = rand.Next(walls.Count);
                var (row, col, dr, dc, current, neighbor) = walls[index];
                walls.RemoveAt(index);

                int neighborRow = row + dr, neighborCol = col + dc;

                // If the neighbor cell is not visited
                if (IsInBounds(neighborRow, neighborCol, rows, cols) && !visited[neighborRow, neighborCol])
                {
                    // Remove the wall by updating weights
                    UpdateCellWeights(maze[row, col], current, 1);
                    UpdateCellWeights(maze[neighborRow, neighborCol], neighbor, 1);

                    // Mark the neighbor cell as visited
                    visited[neighborRow, neighborCol] = true;

                    // Add the neighbor's walls to the list
                    foreach (var (ndr, ndc, ncurrent, nneighbor) in directions)
                    {
                        int nnr = neighborRow + ndr, nnc = neighborCol + ndc;
                        if (IsInBounds(nnr, nnc, rows, cols) && !visited[nnr, nnc])
                        {
                            walls.Add((neighborRow, neighborCol, ndr, ndc, ncurrent, nneighbor));
                        }
                    }
                }
            }

            generateExit(maze, rows, cols);
            return maze;
        }

        private void UpdateCellWeights(Cell cell, string direction, int value)
        {
            switch (direction)
            {
                case "Up": cell.Up = value; break;
                case "Down": cell.Down = value; break;
                case "Left": cell.Left = value; break;
                case "Right": cell.Right = value; break;
            }
        }

        private bool IsInBounds(int row, int col, int rows, int cols)
        {
            return row >= 0 && row < rows && col >= 0 && col < cols;
        }

        private void generateExit(Cell [,] maze, int rows, int cols)
        {
            Random random = Utils.random;

            int randomRow = random.Next(0, rows);
            int randomCol = random.Next(0, cols);

            maze[randomRow, randomCol].Exit = true;
        }
    }

}
