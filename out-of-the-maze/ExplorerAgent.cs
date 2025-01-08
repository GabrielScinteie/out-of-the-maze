using ActressMas;
using Message = ActressMas.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace out_of_the_maze
{
    public class ExplorerAgent : Agent
    {
        private int _row, _column;
        private int _next_row, _next_column;
        private string last_direction = "Up";
        public Cell[,] maze;
        public override void Setup()
        {
            Random random = Utils.random;

            // Initialize position
            _row = random.Next(0, Utils.Size);
            _column = random.Next(0, Utils.Size);

            // Notify the central agent of the initial position
            Send("central", Utils.Str("position", _row, _column));
        }


        public override void Act(Message message)
        {
            Console.WriteLine("\t[{1} -> {0}]: {2}", this.Name, message.Sender, message.Content);

            string action; string parameters;
            Utils.ParseMessage(message.Content, out action, out parameters);

            switch (action)
            {
                case "propose_move":
                    // Try a Valid Movement
                    TryMove();
                    Send("central", Utils.Str("proposal", _next_row, _next_column));
                    break;
                case "confirm":
                    // Confirm the movement
                    updateMaze();
                    _row = _next_row;
                    _column = _next_column;
                    Send("central", Utils.Str("movement_done", _row, _column));
                    break;
                case "deny":
                    Send("central", Utils.Str("movement_done", _row, _column));
                    break;
                default:
                    break;
            }
           
        }

        private void updateMaze()
        {
            int drow = _next_row - _row;
            int dcol = _next_column - _column;
            double decay = Utils.decay;
            if(drow == 1)
            {
                maze[_row, _column].Down -= decay;
                maze[_row, _column].Down = Math.Max(maze[_row, _column].Down, 0.1);
                last_direction = "Down";
            } else if(drow == -1)
            {
                // sus
                maze[_row, _column].Up -= decay;
                maze[_row, _column].Up = Math.Max(maze[_row, _column].Up, 0.1);
                last_direction = "Up";
            } else
            {
                if(dcol == 1)
                {
                    maze[_row, _column].Right -= decay;
                    maze[_row, _column].Right = Math.Max(maze[_row, _column].Right, 0.1);
                    last_direction = "Right";
                    // dreapta
                } else
                {
                    maze[_row, _column].Left -= decay;
                    maze[_row, _column].Left = Math.Max(maze[_row, _column].Left, 0.1);
                    last_direction = "Left";
                    // stanga
                }
            }

            
        }

        /*private void TryMove()
        {
            var directions = new[]
            {
                (-1, 0, "Up"),   // North
                (1, 0, "Down"),   // South
                (0, -1, "Left"), // West
                (0, 1, "Right")  // East
            };


            //Console.WriteLine(maze[_column, _row]);
            for (int k = 0; k < 4; k++)
            {
                //Console.WriteLine(_column + " " + _row + " " + directions[k].Item1 + " " + directions[k].Item2, directions[k].Item3);
                //Console.WriteLine(isValid(_column, _row, directions[k]));
                if (isValid(_row, _column, directions[k]))
                {
                    _next_row = _row + directions[k].Item1;
                    _next_column = _column + directions[k].Item2;
                    break;
                }
            }

            Console.WriteLine($"Moved to ({_row}, {_column})");
        }
        */

        private void TryMove()
        {
            var directions = new[]
            {
                (-1, 0, "Up"),    // North
                (1, 0, "Down"),   // South
                (0, -1, "Left"),  // West
                (0, 1, "Right")   // East
            };

            var oppositeDirection = new Dictionary<string, string>
            {
                { "Up", "Down" },
                { "Down", "Up" },
                { "Left", "Right" },
                { "Right", "Left" }
            };

            double highestWeight = double.MinValue;
            var bestDirections = new List<(int dx, int dy, string direction)>();

            // Evaluate all directions
            foreach (var direction in directions)
            {
                if (isValid(_row, _column, direction))
                {
                    // Determine the weight for the direction
                    double weight = GetWeightForDirection(_row, _column, direction);

                    if (weight > highestWeight)
                    {
                        // Clear the list and add this direction as it has the new highest weight
                        highestWeight = weight;
                        bestDirections.Clear();
                        bestDirections.Add(direction);
                    }
                    else if (weight == highestWeight)
                    {
                        // Add to the list if it matches the current highest weight
                        bestDirections.Add(direction);
                    }
                }
            }
            // && last_direction != null && direction.Item3 != oppositeDirection[last_direction]
            // Choose randomly from the best directions
            if (bestDirections.Count > 0)
            {
                if(bestDirections.Count > 1)
                {
                    string forbiddenDirection = oppositeDirection.ContainsKey(last_direction) ? oppositeDirection[last_direction] : null;
                    bestDirections.RemoveAll(dir => dir.direction == forbiddenDirection);
                }

                var chosenDirection = bestDirections[Utils.random.Next(bestDirections.Count)];
                _next_row = _row + chosenDirection.dx;
                _next_column = _column + chosenDirection.dy;
                last_direction = chosenDirection.direction;

                Console.WriteLine($"Moved {chosenDirection.direction} to ({_next_row}, {_next_column})");
            }
            else
            {
                Console.WriteLine("No valid moves available.");
            }
        }


        private double GetWeightForDirection(int row, int column, (int dx, int dy, string direction) move)
        {
            Cell currentCell = maze[row, column];

            return move.direction switch
            {
                "Up" => currentCell.Up,
                "Down" => currentCell.Down,
                "Left" => currentCell.Left,
                "Right" => currentCell.Right,
                _ => 0
            };
        }


        private Boolean isValid(int row, int column, (int dx, int dy, string direction) move)
        {
            Cell cell = maze[row, column];
            //Console.WriteLine(x + " " + y + " " + move.dx + " " + move.dy, move.direction);
            switch (move.direction)
            {
                case "Up": return cell.Up > 0;
                case "Down": return cell.Down > 0;
                case "Left": return cell.Left > 0;
                case "Right": return cell.Right > 0;
                default: return false;
            }
        }

    }
}
