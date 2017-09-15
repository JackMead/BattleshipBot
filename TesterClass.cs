using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BattleshipBot
{
    [TestFixture]
    class TesterClass
    {

        [Test]
        public void ShipPositioner_OnGetShips_ReturnsValid()
        {
            var shipPositioner = new ShipPositioner();
            var listOfShips = shipPositioner.GetShipPositionsRandomVertical();
            
            for (int i = 0; i < 100; i++)
            {

                listOfShips = shipPositioner.GetShipPositionsRandomVertical();
                Assert.AreEqual(listOfShips.Count(), 5);
            }
        }
    }
}
