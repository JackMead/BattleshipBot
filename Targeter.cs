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
        private int initialSearchSpacing = 3;
        public IGridSquare SearchForTarget(IGridSquare lastTarget)
        {
            var row = lastTarget.Row;
            var col = lastTarget.Column + initialSearchSpacing;
            
            if (col <= 10)
            {
                return new GridSquare(row, col);
            }

            row = (char)(row + 1);
            if (row > 'J')
            {
                row = 'A';
            }

            col = (row%initialSearchSpacing)+1;
            
            return new GridSquare(row, col);
        }
        public enum Orientation
        {
            Vertical, Horizontal, Unknown
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
                return ShotToSideOfHit(targetHits, shotsMade);
            }

            if (orientation == Orientation.Vertical)
            {
                return ShotVerticalFromHit(targetHits, shotsMade);
            }

            if (orientation == Orientation.Horizontal)
            {
                return ShotHorizontalFromHit(targetHits, shotsMade);
            }

            return shotsMade[0];
        }

        private IGridSquare ShotHorizontalFromHit(List<IGridSquare> targetHits, List<IGridSquare> shotsMade)
        {
            foreach (var shot in shotsMade)
            {
                var shotToLeft = new GridSquare(shot.Row, shot.Column - 1);
                if (!shotsMade.Contains(shotToLeft) && ShotIsValid(shotToLeft))
                {
                    return shotToLeft;
                }
                var shotToRight = new GridSquare(shot.Row, shot.Column + 1);
                if (!shotsMade.Contains(shotToRight) && ShotIsValid(shotToLeft))
                {
                    return shotToRight;
                }
            }
            return shotsMade[0];
        }

        private IGridSquare ShotVerticalFromHit(List<IGridSquare> targetHits, List<IGridSquare> shotsMade)
        {
            foreach (var shot in shotsMade)
            {
                var shotUp = new GridSquare((char) (shot.Row - 1), shot.Column);
                if (!shotsMade.Contains(shotUp) && ShotIsValid(shotUp))
                {
                    return shotUp;
                }
                var shotDown = new GridSquare((char) (shot.Row + 1), shot.Column);
                if (!shotsMade.Contains(shotDown) && ShotIsValid(shotDown))
                {
                    return shotDown;
                }
            }
            return shotsMade[0];
        }

        private IGridSquare ShotToSideOfHit(List<IGridSquare> shotsMade, List<IGridSquare> targetHits)
        {
            foreach (var shot in targetHits)
            {

                var shotToLeft = new GridSquare(shot.Row, shot.Column - 1);
                if (!shotsMade.Contains(shotToLeft) && ShotIsValid(shotToLeft))
                {
                    return shotToLeft;
                }
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
                var shotDown = new GridSquare((char)(shot.Row + 1), shot.Column);
                if (!shotsMade.Contains(shotDown) && ShotIsValid(shotDown))
                {
                    return shotDown;
                }
            }
            return shotsMade[0];

        }

        private bool ShotIsValid(GridSquare shot)
        {
            if (shot.Row < AAsInt || shot.Row > AAsInt+9)
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
