using ActressMas;
using Message = ActressMas.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace out_of_the_maze
{
    public class CentralAgent : Agent
    {
        private Maze _formGui;
        public Cell[,] maze { get; set; }
        public Dictionary<string, string> ExplorerPositions { get; set; }
        private Dictionary<string, List<string>> positionMap = new Dictionary<string, List<string>>();
        public LinkedList<string> allAgents { get; set; }
        private int counter = 0;
        private int turn_number = 0;
        private static Boolean visualize = false;


        public CentralAgent()
        {
            ExplorerPositions = new Dictionary<string, string>();
            var generator = new MazeGenerator();
            allAgents = new LinkedList<string>();
            maze = generator.GenerateMaze(Utils.Size, Utils.Size);
            Thread t = new Thread(new ThreadStart(GUIThread));
            t.Start();
        }

        private void GUIThread()
        {
            _formGui = new Maze(this);
            if(visualize)
            {
                _formGui.ShowDialog();
                //_formGui.Load += (sender, args) => _formGui.Hide();
                Application.Run();
            }

            //_formGui.Close();
        }

        public override void Act(Message message)
        {
            // Console.WriteLine("\t[{1} -> {0}]: {2}", this.Name, message.Sender, message.Content);

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
                case "exit":
                    HandleExit(message.Sender, parameters);
                    break;
/*                case "keep_image_alive":
                    Send(message.Sender, "keep_image_alive");
                    break;*/
                default:
                    break; 
            }
            if(visualize)
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

        private void HandleExit(string sender, string position)
        {
            ExplorerPositions[sender] = position;
            //Console.WriteLine("########## Central: Iesirea a fost gasita pe pozitia " + position);
            Console.WriteLine("########## NUMAR_TURE: " + turn_number);

            string fileName;

            // Dynamically create the file name based on the heuristic and other variables
            if (Utils.heuristic)
            {
                fileName = $"C:\\Users\\Gabi\\Desktop\\Facultate\\An 2 - Sem 1 - Master\\SMA\\Proiect\\out-of-the-maze\\out-of-the-maze\\bin\\Debug\\net6.0-windows\\results_Size{Utils.Size}_Explorers{Utils.NoExplorers}.txt";
            }
            else
            {
                fileName = $"C:\\Users\\Gabi\\Desktop\\Facultate\\An 2 - Sem 1 - Master\\SMA\\Proiect\\out-of-the-maze\\out-of-the-maze\\bin\\Debug\\net6.0-windows\\results_Size{Utils.Size}_Explorers{Utils.NoExplorers}_random.txt";
            }

            // Append the data to the dynamically created file
            AppendToFile(fileName, Utils.Str(Utils.Size, Utils.NoExplorers, turn_number));

            foreach (var agent in allAgents)
            {
                Send(agent, Utils.Str("exit", position));
            }

            System.Environment.Exit(0);
            
            


            /*Send(sender, "keep_image_alive");*/
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
                turn_number++;
            }
        }

        public static void AppendToFile(string filePath, string textToAppend)
        {
            try
            {
                // Check if file exists, create if it doesn't
                if (!File.Exists(filePath))
                {
                    using (File.Create(filePath)) { }
                }

                // Append the text to the file
                using (StreamWriter writer = new StreamWriter(filePath, append: true))
                {
                    writer.WriteLine(textToAppend);
                }

                Console.WriteLine("Text appended successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
