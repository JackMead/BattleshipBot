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
        private int initialSearchSpacing = 4;
        public enum Orientation
        {
            Vertical, Horizontal, Unknown
        }

        public IGridSquare SearchForTarget(IGridSquare lastTarget, List<IGridSquare> shotsMade, List<IGridSquare> shotsAvailable)
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
                return SearchForTarget(newTarget, shotsMade, shotsAvailable);
            }

            return GetRandomTarget(shotsAvailable);
        }

        public IGridSquare GetRandomTarget(List<IGridSquare> shotsAvailable)
        {
            var randGenerator = new Random();
            var index = randGenerator.Next(0, shotsAvailable.Count);

            return shotsAvailable[index];
        }

        public IGridSquare ContinueAimingAtTarget(List<IGridSquare> shotsMade, List<IGridSquare> targetHits, Orientation orientation)
        {
            //TODO
            //Better shooting
            if (orientation == Orientation.Unknown)
            {
                return ShotToSideOfHit(shotsMade, targetHits);
            }

            if (orientation == Orientation.Vertical)
            {
                return ShotVerticalFromHit(targetHits, shotsMade);
            }

            if (orientation == Orientation.Horizontal)
            {
                return ShotHorizontalFromHit(targetHits, shotsMade);
            }

            return targetHits[0];
        }

        private IGridSquare ShotHorizontalFromHit(List<IGridSquare> targetHits, List<IGridSquare> shotsMade)
        {
            var shot = targetHits.OrderBy(r => r.Column).ToList()[0];
            var shotLeft = new GridSquare(shot.Row, shot.Column - 1);
            if (ShotIsValid(shotLeft) && !shotsMade.Contains(shotLeft))
            {
                return shotLeft;
            }
            shot = targetHits.OrderByDescending(r => r.Column).ToList()[0];
            var shotRight = new GridSquare(shot.Row, shot.Column + 1);

            return shotRight;
        }

        private IGridSquare ShotVerticalFromHit(List<IGridSquare> targetHits, List<IGridSquare> shotsMade)
        {
            var shot = targetHits.OrderBy(r => r.Row).ToList()[0];
            var shotUp = new GridSquare((char)(shot.Row - 1), shot.Column);
            if (ShotIsValid(shotUp) && !shotsMade.Contains(shotUp))
            {
                return shotUp;
            }
            shot = targetHits.OrderByDescending(r => r.Row).ToList()[0];
            var shotDown = new GridSquare((char)(shot.Row + 1), shot.Column);

            return shotDown;
        }

        private IGridSquare ShotToSideOfHit(List<IGridSquare> shotsMade, List<IGridSquare> targetHits)
        {
            var shot = targetHits[0];
            var shotToRight = new GridSquare(shot.Row, shot.Column + 1);
            if (!shotsMade.Contains(shotToRight) && ShotIsValid(shotToRight))
            {
                return shotToRight;
            }
            var shotUp = new GridSquare((char)(shot.Row - 1), shot.Column);
            if (!shotsMade.Contains(shotUp) && ShotIsValid(shotUp))
            {
                return shotUp;
            }
            var shotToLeft = new GridSquare(shot.Row, shot.Column - 1);
            if (!shotsMade.Contains(shotToLeft) && ShotIsValid(shotToLeft))
            {
                return shotToLeft;
            }

            var shotDown = new GridSquare((char)(shot.Row + 1), shot.Column);
            if (!shotsMade.Contains(shotDown) && ShotIsValid(shotDown))
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
    }
}
