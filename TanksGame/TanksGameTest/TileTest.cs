using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using TanksGame.GameEntities;

namespace TanksGameTest
{
    [TestClass]
    public class TileTest
    {
        // Тест метода Contains(x, y) - когда точка находится строго внутри плитки
        [TestMethod]
        public void Contains_WhenPointIsInsideTile_ShouldReturnTrue()
        {
            // Arrange
            float tileX = 100f;
            float tileY = 100f;
            var tile = new Tile(tileX, tileY, TileType.Slow);

            // Act
            bool result = tile.Contains(120f, 120f);

            // Assert
            Assert.IsTrue(result, "Точка должна находиться внутри плитки.");
        }

        // Тест метода Contains(x, y) - когда точка снаружи плитки
        [TestMethod]
        public void Contains_WhenPointIsOutsideTile_ShouldReturnFalse()
        {
            // Arrange
            var tile = new Tile(100f, 100f, TileType.Slow);

            // Act
            bool result = tile.Contains(200f, 200f);

            // Assert
            Assert.IsFalse(result, "Точка должна быть за пределами плитки.");
        }
    }
}
