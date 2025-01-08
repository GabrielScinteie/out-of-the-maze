using ActressMas;
using Message = ActressMas.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace out_of_the_maze
{
    public class CentralAgent : Agent
    {
        private Maze _formGui;
        public Cell[,] maze { get; set; }
        public Dictionary<string, string> ExplorerPositions { get; set; }
        private Dictionary<string, List<string>> positionMap = new Dictionary<string, List<string>>();
        private int counter = 0;


        public CentralAgent()
        {
            ExplorerPositions = new Dictionary<string, string>();
            var generator = new MazeGenerator();
            maze = generator.GenerateMaze(Utils.Size, Utils.Size);
            Thread t = new Thread(new ThreadStart(GUIThread));
            t.Start();
        }

        private void GUIThread()
        {
            _formGui = new Maze(this);
            _formGui.ShowDialog();
            Application.Run();
        }

        public override void Act(Message message)
        {
            Console.WriteLine("\t[{1} -> {0}]: {2}", this.Name, message.Sender, message.Content);

            string action; string parameters;
            Utils.ParseMessage(message.Content, out action, out parameters);

            switch (action)
            {
                case "position":
                    HandlePosition(message.Sender, parameters);
                    break;
                case "proposal":
                    HandleProposal(message.Sender, parameters);
                    break;
                case "movement_done":
                    HandleMovementDone(message.Sender, parameters);
                    break;
                default:
                    break; 
            }

            _formGui.UpdateMazeGUI();
        }

        private void HandlePosition(string sender, string position)
        {
            ExplorerPositions.Add(sender, position);
            Send(sender, "propose_move");
        }

        private void HandleMovementDone(string sender, string position)
        {
            ExplorerPositions[sender] = position;
            Send(sender, "propose_move");
        }

        private void HandleProposal(string sender, string position)
        {

            if (!positionMap.ContainsKey(position))
            {
                positionMap[position] = new List<string>(); // Initialize the list if position doesn't exist
            }
            positionMap[position].Add(sender);
            counter += 1;

            // Adaugam la dictionar
            if (counter == ExplorerPositions.Count)
            {
                foreach (var kvp in positionMap)
                {
                    var senders = kvp.Value;
                    if (senders.Count > 1)
                    {
                        // Collision detected: send "confirm" only to the first sender
                        Send(senders[0], "confirm");

                        // Send "deny" to all others
                        for (int i = 1; i < senders.Count; i++)
                        {
                            Send(senders[i], "deny");
                        }
                    } else
                    {
                        // No collision: send "confirm" to the only sender at this position
                        Send(senders[0], "confirm");
                    }

                }

                positionMap.Clear();
                counter = 0;
            }
        }
    }
}
