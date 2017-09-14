using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleships.Player.Interface;

namespace BattleshipBot
{
    class ShipPositioner
    {
        private Random rand = new Random();
        private int AAsInt = 65;
        private int[] listOfBoatLengths = {5, 4, 3, 3, 2};

        public IEnumerable<IShipPosition> GetRandomShipPositioning()
        {
            //Ship theory: place patrol boat in a corner, then randomize the others?
            //TODO

            //For now, randomly place each on a different horizontal line
            var rowsForShips = new int[5];
            rowsForShips[0] = rand.Next(AAsInt, AAsInt + 10);

            for (int i = 1; i < 5; i++)
            {
                rowsForShips[i] = rowsForShips[i - 1] + 2;
                if (rowsForShips[i] > AAsInt + 9)
                {
                    rowsForShips[i] -= 10;
                }
            }

            var listOfShipPositions = new List<IShipPosition>();

            var colsForShips = new int[5];
            for (int i = 0; i < 5; i++)
            {
                colsForShips[i] = rand.Next(1, 12 - listOfBoatLengths[i]);
                ShipPosition ship = GetShipPosition((char)rowsForShips[i], colsForShips[i], (char)rowsForShips[i],
                    colsForShips[i] + listOfBoatLengths[i] - 1);
                listOfShipPositions.Add(ship);
            }

            return listOfShipPositions;
        }

        private ShipPosition GetShipPosition(char startRow, int startColumn, char endRow, int endColumn)
        {
            return new ShipPosition(new GridSquare(startRow, startColumn), new GridSquare(endRow, endColumn));
        }
    }
}


