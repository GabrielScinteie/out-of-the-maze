using ActressMas;

namespace out_of_the_maze
{
    public class Program
    {
        private static void Main(string[] args)
        {
            EnvironmentMas env = new EnvironmentMas(0, 0);

            var centralAgent = new CentralAgent();
            env.Add(centralAgent, "central");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            for (int i = 1; i <= Utils.NoExplorers; i++)
            {
                var explorerAgent = new ExplorerAgent();
                centralAgent.allAgents.AddLast("explorer " + i);
                explorerAgent.maze = centralAgent.maze;
                Cell[,] maze = explorerAgent.maze;
                env.Add(explorerAgent, "explorer " + i);
            }

            Thread.Sleep(1000);

            env.Start();
        }
    }
}