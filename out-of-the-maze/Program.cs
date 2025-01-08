using ActressMas;

namespace out_of_the_maze
{
    public class Program
    {
        private static void Main(string[] args)
        {
            EnvironmentMas env = new EnvironmentMas(0, 200);

            var centralAgent = new CentralAgent();
            env.Add(centralAgent, "central");

            for (int i = 1; i <= Utils.NoExplorers; i++)
            {
                var explorerAgent = new ExplorerAgent();
                explorerAgent.maze = centralAgent.maze;
                Cell[,] maze = explorerAgent.maze;
                for (int r = 0; r < maze.GetLength(0); r++)
                {
                    for (int c = 0; c < maze.GetLength(1); c++)
                    {
                        // Print the cell's directional weights
                        Cell cell = maze[r, c];
                        Console.Write($"({cell.Up},{cell.Down},{cell.Left},{cell.Right}) ");
                    }
                    Console.WriteLine(); // New line after each row
                }
                env.Add(explorerAgent, "explorer " + i);
            }

            Thread.Sleep(1000);

            env.Start();
        }
    }
}