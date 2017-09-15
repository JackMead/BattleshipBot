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

            //TODO
            //Decide final strategy and implement. For now, patrol boat in corner vertically, others randomly horizontally
            var rowsForShips = new int[5];
            rowsForShips[0] = rand.Next(AAsInt, AAsInt + 10);
            var colsForShips = new int[5];
            colsForShips[0] = rand.Next(1, 5);
            var listOfShipPositions = new List<IShipPosition>();


            for (int i = 1; i < 4; i++)
            {
                rowsForShips[i] = rowsForShips[i - 1] + 2;
                if (rowsForShips[i] > AAsInt + 9)
                {
                    rowsForShips[i] -= 10;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                //TODO sort this out
                //For now, can't touch edges to avoid patrol boat clash
                colsForShips[i] = rand.Next(2, 11 - listOfBoatLengths[i]);
                ShipPosition ship = GetShipPosition((char)rowsForShips[i], colsForShips[i], (char)rowsForShips[i],
                    colsForShips[i] + listOfBoatLengths[i] - 1);
                listOfShipPositions.Add(ship);
            }

            ShipPosition patrolBoat = GetPatrolBoatInCorner();
            listOfShipPositions.Add(patrolBoat);
            return listOfShipPositions;
        }

        private ShipPosition GetPatrolBoatInCorner()
        {
            switch (rand.Next(0, 4))
            {
                case 0:
                    return GetShipPosition('A', 1, 'B', 1);

                case 1:
                    return GetShipPosition('A', 10, 'B', 10);

                case 2:
                    return GetShipPosition('I', 1, 'J', 1);

                case 3:
                    return GetShipPosition('I', 10, 'J', 10);

                    default:
                        return GetShipPosition('A', 1, 'B', 1);
            }
        }

        private ShipPosition GetShipPosition(char startRow, int startColumn, char endRow, int endColumn)
        {
            return new ShipPosition(new GridSquare(startRow, startColumn), new GridSquare(endRow, endColumn));
        }
    }
}


