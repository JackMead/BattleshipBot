using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Battleships.Player.Interface;

namespace BattleshipBot
{
    class ShipPositioner
    {
        private int AAsInt = 65;
        private int[] listOfBoatLengths = { 5, 4, 3, 3, 2 };
        private List<IShipPosition> listOfShipPositions;
        private bool[,] board = new bool[10, 10];
        private Random rand;

        public IEnumerable<IShipPosition> GetShipPositionsUnknownStrategy(int next)
        {
            rand = new Random();
            listOfShipPositions = new List<IShipPosition>();
            SetBoard();

            switch (next)
            {
                case 0:
                    return GetRandomShipPositioning(rand);

                case 1:
                    return GetHardcodedShipPositions();

                case 3:
                    return GetShipPositionsRandomHorizontal();

                case 4:
                    return GetShipPositionsRandomVertical();

                default:
                    return GetHardcodedShipPositions();
                    
            }
        }

        public IEnumerable<IShipPosition> GetHardcodedShipPositions()
        {
            var random = new Random();
            var setOfpositions = 0;//rand.Next(0, 3);

            if (setOfpositions == 1)
            {
                return new List<IShipPosition>
                {
                    GetShipPosition('A',1,'E',1),
                    GetShipPosition('A',3,'D',3),
                    GetShipPosition('A',10,'C',10),
                    GetShipPosition('D',4,'D',6),
                    GetShipPosition('J',1,'J',2)
                };
            }
            if (setOfpositions == 2)
            {
                return new List<IShipPosition>
                {
                    GetShipPosition('A',6,'A',10),
                    GetShipPosition('G',1,'J',1),
                    GetShipPosition('B',2,'H',4),
                    GetShipPosition('I',8,'I',10),
                    GetShipPosition('E',6,'E',7)
                };
            }
            return new List<IShipPosition>
            {
                GetShipPosition('F',1,'J',1),
                GetShipPosition('F',3,'F',6),
                GetShipPosition('H',3,'H',5),
                GetShipPosition('F',8,'F',10),
                GetShipPosition('J',9,'J',10)
            };
        }
        public IEnumerable<IShipPosition> GetShipPositionsRandomHorizontal()
        {
            listOfShipPositions = new List<IShipPosition>();
            rand = new Random();
            var rowsForShips = new int[5];

            rowsForShips[0] = rand.Next(AAsInt, AAsInt + 10);

            var colsForShips = new int[5];

            colsForShips[0] = rand.Next(1, 6);

            for (int i = 1; i < 5; i++)
            {
                rowsForShips[i] = rowsForShips[i - 1] + 2;
                if (rowsForShips[i] > AAsInt + 9)
                {
                    rowsForShips[i] -= 10;
                }
            }

            
            for (int i = 0; i < 5; i++)
            {
                colsForShips[i] = rand.Next(1, 12 - listOfBoatLengths[i]);
                ShipPosition ship = GetShipPosition((char)rowsForShips[i], colsForShips[i], (char)rowsForShips[i],
                    colsForShips[i] + listOfBoatLengths[i] - 1);
                listOfShipPositions.Add(ship);
            }
            return listOfShipPositions;
        }
        public IEnumerable<IShipPosition> GetShipPositionsRandomVertical()
        {
            listOfShipPositions=new List<IShipPosition>();
            rand = new Random();
            var rowsForShips = new int[5];

            rowsForShips[0] = rand.Next(AAsInt, AAsInt + 5);

            var colsForShips = new int[5];

            colsForShips[0] = rand.Next(1, 11);

            for (int i = 1; i < 5; i++)
            {
                colsForShips[i] = colsForShips[i - 1] + 2;
                if (colsForShips[i] > 10)
                {
                    colsForShips[i] -= 10;
                }
            }


            for (int i = 0; i < 5; i++)
            {
                rowsForShips[i] = rand.Next(AAsInt, AAsInt+11 - listOfBoatLengths[i]);
                Console.WriteLine("Ship begins at: {0},{1} and runs to {2},{3}",(char) rowsForShips[i], colsForShips[i], (char)(rowsForShips[i] + listOfBoatLengths[i] - 1), colsForShips[i]);
                
                ShipPosition ship = GetShipPosition((char)rowsForShips[i], colsForShips[i], (char)(rowsForShips[i] + listOfBoatLengths[i] - 1),
                    colsForShips[i]);
                listOfShipPositions.Add(ship);
            }
            return listOfShipPositions;
        }
        public IEnumerable<IShipPosition> GetRandomShipPositioning(Random rand)
        {
            this.rand = rand;
            listOfShipPositions=new List<IShipPosition>();
            foreach (var length in listOfBoatLengths)
            {
                var position = GetRandomShipPosition(length);
                if (IsValidPosition(position))
                {
                    UpdateBoard(position);
                    listOfShipPositions.Add(position);
                }
            }
            return listOfShipPositions;
        }

        private IShipPosition GetRandomShipPosition(int length)
        {
            //First find all valid placements
            //Then pick one
            var listOfValidPositions = new List<IShipPosition>();

            var orientation = Targeter.Orientation.Horizontal;
            var startRow = 'A';
            var startCol = 1;
            if (rand.Next(0, 2) == 1)
            {
                for (int row = 0; row < 10; row++)
                {
                    for (int col = 0; col < 11 - length; col++)
                    {
                        bool selectionValid = true;
                        for (int boatCols = col; boatCols < col + length; boatCols++)
                        {
                            if (board[row, boatCols] == true)
                            {
                                selectionValid = false;
                                break;
                            }
                        }
                        if (selectionValid)
                        {
                            listOfValidPositions.Add(GetShipPosition((char)(row + AAsInt), col+1, (char)(row + AAsInt), col + length ));
                        }
                    }
                }
                return listOfValidPositions.OrderBy(r => rand.Next()).ToList()[0];

            }

            for (int col = 0; col < 10; col++)
            {
                for (int row = 0; row < 11 - length; row++)
                {
                    bool selectionValid = true;
                    for (int boatRows = row; boatRows < row + length; boatRows++)
                    {
                        if (board[boatRows, col] == true)
                        {
                            selectionValid = false;
                            break;
                        }
                    }
                    if (selectionValid)
                    {
                        listOfValidPositions.Add(GetShipPosition((char)(row + AAsInt), col+1, (char)(row + AAsInt + length - 1), col+1));
                    }
                }
            }
            return listOfValidPositions.OrderBy(r => rand.Next()).ToList()[0];


            if (orientation == Targeter.Orientation.Vertical)
            {
                startRow = (char)rand.Next(AAsInt, AAsInt + 11 - length);
                var endRow = (char)(startRow + length - 1);
                startCol = rand.Next(1, 11);
                return GetShipPosition(startRow, startCol, endRow, startCol);
            }
            else
            {
                startRow = (char)rand.Next(AAsInt, AAsInt + 10);
                startCol = rand.Next(1, 12 - length);
                var endCol = startCol + length - 1;
                return GetShipPosition(startRow, startCol, startRow, endCol);

            }

        }

        private ShipPosition GetShipPosition(char startRow, int startColumn, char endRow, int endColumn)
        {
            return new ShipPosition(new GridSquare(startRow, startColumn), new GridSquare(endRow, endColumn));
        }

        private void SetBoard()
        {
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    board[row, col] = false;
                }
            }
        }

        private void UpdateBoard(IShipPosition position)
        {
            if (position.StartingSquare.Row == position.EndingSquare.Row)
            {
                for (int row = 0; row < 10; row++)
                {
                    for (int col = 0; col < 10; col++)
                    {
                        if (row == (position.StartingSquare.Row - AAsInt) && col < position.EndingSquare.Column &&
                            col >= position.StartingSquare.Column - 1)
                        {
                            board[row, col] = true;
                            SetSurroundingSquaresInvalid(row, col);
                        }
                    }
                }
                return;
            }
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    if (col == (position.StartingSquare.Column - 1) && row <= (position.EndingSquare.Row - AAsInt) &&
                        row >= position.StartingSquare.Row - AAsInt)
                    {
                        board[row, col] = true;
                        SetSurroundingSquaresInvalid(row, col);
                    }
                }
            }
        }
        
        private bool IsValidPosition(IShipPosition position)
        {
            if (position.StartingSquare.Row == position.EndingSquare.Row)
            {

                for (int col = position.StartingSquare.Column - 1; col < position.EndingSquare.Column; col++)
                {
                    if (board[position.StartingSquare.Row - AAsInt, col] == true)
                    {
                        return false;
                    }
                }
                return true;
            }
            for (int row = position.StartingSquare.Row - AAsInt; row < position.EndingSquare.Row + 1 - AAsInt; row++)
            {
                if (board[row, position.StartingSquare.Column - 1] == true)
                {
                    return false;
                }
            }
            return true;
        }

        private void SetSurroundingSquaresInvalid(int row, int col)
        {
            if (row < 9)
            {
                board[row + 1, col] = true;
            }
            if (row > 0)
            {
                board[row - 1, col] = true;
            }
            if (col > 0)
            {
                board[row, col - 1] = true;
            }
            if (col < 9)
            {
                board[row, col + 1] = true;
            }
            if (col < 9 && row < 9)
            {
                board[row + 1, col + 1] = true;
            }
            if (col > 0 && row > 0)
            {
                board[row - 1, col - 1] = true;
            }
            if (col > 0 && row < 9)
            {
                board[row + 1, col - 1] = true;

            }
            if (col < 9 && row > 0)
            {
                board[row - 1, col + 1] = true;

            }
        }
        
    }
}


