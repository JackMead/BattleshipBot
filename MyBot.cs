using System.Collections.Generic;
using System.Linq;
using Battleships.Player.Interface;

namespace BattleshipBot
{
    public class MyBot : IBattleshipsBot
    {
        private IGridSquare lastTarget;
        private IGridSquare lastSearchingShot;
        private bool targetAcquired=false;
        private Targeter.Orientation targetOrientation = Targeter.Orientation.Unknown;
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
            while(shotsMade.Contains(nextTarget)|| !targeter.ShotIsValid((GridSquare)nextTarget))
            {
                nextTarget = targeter.GetRandomTarget(shotsAvailable);
            }
            if (!targetAcquired)
            {
                lastSearchingShot = nextTarget;
            }
            lastTarget = nextTarget;
            shotsAvailable.Remove(nextTarget);
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
                lastTarget = lastSearchingShot;
            }

            if (!targetAcquired)
            {
                return targeter.SearchForTarget(lastTarget, shotsMade, shotsAvailable);
            }

            UpdateOrientation();

            return targeter.ContinueAimingAtTarget(shotsMade, targetHits,targetOrientation);
        }

        private void UpdateOrientation()
        {
            if (targetHits.Count < 2)
            {
                targetOrientation= Targeter.Orientation.Unknown;
                return;
            }

            if (targetHits[0].Row == targetHits[1].Row)
            {
                targetOrientation= Targeter.Orientation.Horizontal;
                return;
            }

            targetOrientation = Targeter.Orientation.Vertical;
        }

        private void SetUpShotsAvailableList()
        {
            for (int row = AAsInt; row < AAsInt + 10; row++)
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

            if (targetHits.Count == 5)
            {
                return true;
            }
            
            if (targetHits[0].Row == targetHits[1].Row)
            {
                if (IsHorizontallyDestroyed(targetHits))
                {
                    return true;
                }
            }

            if (targetHits[0].Column == targetHits[1].Column)
            {
                if (IsVerticallyDestroyed(targetHits))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsVerticallyDestroyed(List<IGridSquare> targetHits)
        {
            var lowestShot = targetHits.OrderBy(r => r.Row).ToList()[0];
            var highestShot = targetHits.OrderByDescending(r => r.Row).ToList()[0];

            if (lowestShot.Row != 'A' &&
                shotsAvailable.Contains(new GridSquare((char) (lowestShot.Row - 1), lowestShot.Column)))
            {
                return false;
            }

            if (highestShot.Row != 'J' &&
                shotsAvailable.Contains(new GridSquare((char) (highestShot.Row + 1), highestShot.Column)))
            {
                return false;
            }

            return true;
        }

        private bool IsHorizontallyDestroyed(List<IGridSquare> targetHits)
        {
            var leftMostHit = targetHits.OrderBy(r => r.Column).ToList()[0];
            var rightMostHit = targetHits.OrderByDescending(r => r.Column).ToList()[0];

            if (leftMostHit.Column != 1 &&
                shotsAvailable.Contains(new GridSquare((char)(leftMostHit.Row), leftMostHit.Column-1)))
            {
                return false;
            }

            if (rightMostHit.Column != 10 &&
                shotsAvailable.Contains(new GridSquare((char)(rightMostHit.Row), rightMostHit.Column+1)))
            {
                return false;
            }

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

        public string Name => "Botty McBotFace";
    }
}
