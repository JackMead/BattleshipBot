using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleships.Player.Interface;

namespace BattleshipBot
{
    class Targeter
    {
        private int AAsInt = 65;
        public enum Orientation
        {
            Vertical, Horizontal, Unknown
        }

        public IGridSquare SearchForTarget(IGridSquare lastTarget, List<IGridSquare> shotsMade, List<IGridSquare> shotsAvailable)
        {
            return SearchRandomFromEfficientList(lastTarget, shotsMade, shotsAvailable);
        }

        private IGridSquare SearchRandomFromEfficientList(IGridSquare lastTarget, List<IGridSquare> shotsMade, List<IGridSquare> shotsAvailable)
        {
            List<IGridSquare> shotsToChooseFrom = new List<IGridSquare>();

            foreach (var shot in shotsAvailable)
            {
                if ((shot.Column + shot.Row) % 8 == 0)
                {
                    shotsToChooseFrom.Add(shot);
                }
            }

            if (!shotsToChooseFrom.Any())
            {
                foreach (var shot in shotsAvailable)
                {
                    if ((shot.Column + shot.Row) % 4 == 0)
                    {
                        shotsToChooseFrom.Add(shot);
                    }
                }
            }

            if (!shotsToChooseFrom.Any())
            {
                foreach (var shot in shotsAvailable)
                {
                    if ((shot.Column + shot.Row) % 2 == 0)
                    {
                        shotsToChooseFrom.Add(shot);
                    }
                }
            }

            return GetRandomTarget(shotsToChooseFrom);
        }
        
        public IGridSquare GetRandomTarget(List<IGridSquare> shotsAvailable)
        {
            var randGenerator = new Random();
            shotsAvailable= shotsAvailable.OrderBy(r => randGenerator.Next()).ToList();

            //Add bias towards edges factor
            if (randGenerator.Next(0, 4) == 0)
            {
                if (randGenerator.Next(0, 2) == 0)
                {
                shotsAvailable = shotsAvailable.OrderBy(r => r.Row).ToList();

                }
                shotsAvailable = shotsAvailable.OrderByDescending(r => r.Row).ToList();

            }
            else if (randGenerator.Next(0, 4) == 0)
            {
                if (randGenerator.Next(0, 2) == 0)
                {
                    shotsAvailable = shotsAvailable.OrderBy(r => r.Column).ToList();
                }
                shotsAvailable = shotsAvailable.OrderByDescending(r => r.Column).ToList();
            }

            return shotsAvailable[0];
        }

        public IGridSquare ContinueAimingAtTarget(List<IGridSquare> shotsMade, List<IGridSquare> targetHits, Orientation orientation, List<IGridSquare> shotsAvailable)
        {
            if (orientation == Orientation.Unknown)
            {
                return ShotToSideOfHit(shotsMade, targetHits, shotsAvailable);
            }

            if (orientation == Orientation.Vertical)
            {
                return ShotVerticalFromHit(targetHits, shotsMade, shotsAvailable);
            }

            if (orientation == Orientation.Horizontal)
            {
                return ShotHorizontalFromHit(targetHits, shotsMade, shotsAvailable);
            }

            return targetHits[0];
        }

        private IGridSquare ShotHorizontalFromHit(List<IGridSquare> targetHits, List<IGridSquare> shotsMade, List<IGridSquare> shotsAvailable)
        {
            var shot = targetHits.OrderBy(r => r.Column).ToList()[0];
            var shotLeft = new GridSquare(shot.Row, shot.Column - 1);
            if (shotsAvailable.Contains(shotLeft))
            {
                return shotLeft;
            }
            shot = targetHits.OrderByDescending(r => r.Column).ToList()[0];
            var shotRight = new GridSquare(shot.Row, shot.Column + 1);

            return shotRight;
        }

        private IGridSquare ShotVerticalFromHit(List<IGridSquare> targetHits, List<IGridSquare> shotsMade, List<IGridSquare> shotsAvailable)
        {
            var shot = targetHits.OrderBy(r => r.Row).ToList()[0];
            var shotUp = new GridSquare((char)(shot.Row - 1), shot.Column);
            if (shotsAvailable.Contains(shotUp))
            {
                return shotUp;
            }
            shot = targetHits.OrderByDescending(r => r.Row).ToList()[0];
            var shotDown = new GridSquare((char)(shot.Row + 1), shot.Column);

            return shotDown;
        }

        private IGridSquare ShotToSideOfHit(List<IGridSquare> shotsMade, List<IGridSquare> targetHits, List<IGridSquare> shotsAvailable)
        {
            var shot = targetHits[0];
            var shotToRight = new GridSquare(shot.Row, shot.Column + 1);
            if (shotsAvailable.Contains(shotToRight))
            {
                return shotToRight;
            }
            var shotUp = new GridSquare((char)(shot.Row - 1), shot.Column);
            if (shotsAvailable.Contains(shotUp))
            {
                return shotUp;
            }
            var shotToLeft = new GridSquare(shot.Row, shot.Column - 1);
            if (shotsAvailable.Contains(shotToLeft))
            {
                return shotToLeft;
            }

            var shotDown = new GridSquare((char)(shot.Row + 1), shot.Column);
            if (shotsAvailable.Contains(shotDown))
            {
                return shotDown;
            }

            return targetHits[0];

        }

        public bool ShotIsValid(GridSquare shot)
        {
            if (shot.Row < AAsInt || shot.Row > AAsInt + 9)
            {
                return false;
            }
            if (shot.Column < 1 || shot.Column > 10)
            {
                return false;
            }
            return true;
        }

        //Defunct searching methods
        private IGridSquare SearchBySpacing(IGridSquare lastTarget, List<IGridSquare> shotsMade, List<IGridSquare> shotsAvailable, int initialSearchSpacing)
        {
            var row = lastTarget.Row;
            var col = lastTarget.Column + initialSearchSpacing;

            if (col > 10)
            {
                row = (char)(row + 1);
                if (row > 'J')
                {
                    row = 'A';
                }

                col = (row % initialSearchSpacing) + 1;
            }

            var newTarget = new GridSquare(row, col);
            if (!shotsMade.Contains(newTarget))
            {
                return newTarget;
            }

            if (!shotsMade.Contains(SearchForTarget(newTarget, shotsMade, shotsAvailable)))
            {
                return SearchBySpacing(newTarget, shotsMade, shotsAvailable, initialSearchSpacing);
            }

            return GetRandomTarget(shotsAvailable);
        }

        public IGridSquare SearchDiagonals(IGridSquare lastTarget, List<IGridSquare> shotsMade,
            List<IGridSquare> shotsAvailable)
        {

            var row = lastTarget.Row;
            var col = lastTarget.Column;

            row = (char)(row + 2);
            if (row > 'J')
            {
                row = 'A';
            }

            col+=2;
            if (col > 10)
            {
                col = 2;
            }


            var newTarget = new GridSquare(row, col);
            if (!shotsMade.Contains(newTarget))
            {
                return newTarget;
            }
            
            row=(char) (row+3);
            col-=3;
            if (row > 'J')
            {
                row = 'A';
            }
            
            if (col <1)
            {
                col += 10;
            }
            newTarget= new GridSquare(row, col);
            if (!shotsMade.Contains(newTarget))
            {
                return newTarget;
            }

            return GetRandomTarget(shotsAvailable);
        }
    }
}
