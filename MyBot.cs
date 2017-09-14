using System.Collections.Generic;
using Battleships.Player.Interface;

namespace BattleshipBot
{
    public class MyBot : IBattleshipsBot
    {
        private IGridSquare lastTarget;
        private bool targetAcquired=false;
        private bool targetVertical = false;
        private List<IGridSquare> targetHits=new List<IGridSquare>();
        private Targeter targeter=new Targeter();
        private List<IGridSquare> shotsMade = new List<IGridSquare>();
        private List<IGridSquare> shotsAvailable = new List<IGridSquare>();
        private int AAsInt = 65;

        public IEnumerable<IShipPosition> GetShipPositions()
        {
            lastTarget = null; // Forget all our history when we start a new game

            return new ShipPositioner().GetRandomShipPositioning();
        }

        public IGridSquare SelectTarget()
        {
            var nextTarget = GetNextTarget();
            lastTarget = nextTarget;
            while (shotsMade.Contains(nextTarget))
            {
                nextTarget = targeter.GetRandomTarget(shotsAvailable);
            }
            shotsMade.Add(nextTarget);
            return nextTarget;
        }

        private IGridSquare GetNextTarget()
        {
            if (lastTarget == null)
            {
                SetUpShotsAvailableList();
                return new GridSquare('F', 2);
            }

            if (IsTargetDestroyed())
            {
                targetAcquired = false;
                targetHits=new List<IGridSquare>();
                lastTarget = targeter.GetRandomTarget(shotsAvailable);
            }

            if (!targetAcquired)
            {
                return targeter.SearchForTarget(lastTarget);
            }

            return targeter.ContinueAimingAtTarget(shotsMade, targetHits, Targeter.Orientation.Unknown);
        }

        private void SetUpShotsAvailableList()
        {
            for (int row = AAsInt+1; row < AAsInt + 11; row++)
            {
                for (int col = 1; col < 11; col++)
                {
                    shotsAvailable.Add(new GridSquare((char) row, col));
                }
            }
        }

        private bool IsTargetDestroyed()
        {
            if (targetHits.Count < 2)
            {
                return false;
            }

            //TODO
            //if on same row
            //do stuff

            //if on same col
            //do stuff

            return true;
        }

        public void HandleShotResult(IGridSquare square, bool wasHit)
        {
            if (wasHit)
            {
                targetHits.Add(square);
                targetAcquired = true;
            }
        }

        public void HandleOpponentsShot(IGridSquare square)
        {
            // Ignore what our opponent does
        }

        public string Name => "Jack M's Initial Bot";
    }
}
