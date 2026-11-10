using TanksGame.GameEntities;

namespace TanksGameTest
{
    /// <summary>
    /// Описывает тестовый набор для проверки логики класса Плитка.
    /// </summary>
    [TestClass]
    public class TileTest
    {
        /// <summary>
        /// Проверяет, что метод возвращает истину, если точка находится строго внутри плитки.
        /// </summary>
        [TestMethod]
        public void Contains_WhenPointIsInsideTile_ShouldReturnTrue()
        {
            // Arrange (Подготовка данных)
            float tileX = 100f;
            float tileY = 100f;
            var tile = new Tile(tileX, tileY, TileType.Slow);

            // Act (Выполнение действия)
            // Плитка имеет размер 40x40. Точка (120, 120) гарантированно лежит внутри.
            bool result = tile.Contains(120f, 120f);

            // Assert (Проверка результата)
            Assert.IsTrue(result, "Метод обязан возвращать true, если переданные координаты находятся внутри плитки!");
        }

        /// <summary>
        /// Проверяет, что метод возвращает ложь, если точка находится за пределами плитки.
        /// </summary>
        [TestMethod]
        public void Contains_WhenPointIsOutsideTile_ShouldReturnFalse()
        {
            // Arrange
            var tile = new Tile(100f, 100f, TileType.Slow);

            // Act
            bool result = tile.Contains(300f, 300f);

            // Assert
            Assert.IsFalse(result, "Метод обязан возвращать false, если координаты лежат вне плитки!");
        }
    }
}
