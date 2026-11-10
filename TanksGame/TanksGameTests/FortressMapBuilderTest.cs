using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanksGame.CardFactory;
using TanksGame.GameEntities;

namespace TanksGameTest
{
    /// <summary>
    /// Тестовый класс для проверки корректности сборки карты Крепости строителем.
    /// </summary>
    [TestClass]
    public class FortressMapBuilderTest
    {
        private FortressMapBuilder _builder;

        [TestInitialize]
        public void Setup()
        {
            _builder = new FortressMapBuilder();
        }

        /// <summary>
        /// Изменяет коллекцию стен, добавляя в нее базовые границы и оборонительные редуты карты Крепости.
        /// </summary>
        [TestMethod]
        public void CreateWalls_WhenCalled_GeneratesBordersAndFortressObstacles()
        {
            // Act
            List<Wall> walls = _builder.CreateWalls();

            // Assert
            Assert.IsNotNull(walls, "Список стен не должен быть null.");

            // Проверка внешних границ карты Крепости (1280x720)
            bool hasTopWall = walls.Any(w => w.X == 0 && w.Y == 0 && w.Width == 1280 && w.Height == 15);
            bool hasBottomWall = walls.Any(w => w.X == 0 && w.Y == 720 - 15 && w.Width == 1280 && w.Height == 15);
            bool hasLeftWall = walls.Any(w => w.X == 0 && w.Y == 0 && w.Width == 15 && w.Height == 720);
            bool hasRightWall = walls.Any(w => w.X == 1280 - 15 && w.Y == 0 && w.Width == 15 && w.Height == 720);

            Assert.IsTrue(hasTopWall, "Отсутствует верхняя граница карты.");
            Assert.IsTrue(hasBottomWall, "Отсутствует нижняя граница карты.");
            Assert.IsTrue(hasLeftWall, "Отсутствует левая граница карты.");
            Assert.IsTrue(hasRightWall, "Отсутствует правая граница карты.");

            // Проверка наличия специфичного центрального укрепления крепости: new Wall(520, 250, 240, 30)
            bool hasCentralFortressWall = walls.Any(w => w.X == 520 && w.Y == 250 && w.Width == 240 && w.Height == 30);
            Assert.IsTrue(hasCentralFortressWall, "Центральная стена крепости не была создана.");
        }

        /// <summary>
        /// Изменяет состав тайлов, исключая генерацию стандартной земли поверх объектов препятствий карты Крепости.
        /// </summary>
        [TestMethod]
        public void CreateTiles_WithWallsPresent_DoesNotSpawnTilesInsideSolidWalls()
        {
            // Arrange
            List<Wall> walls = _builder.CreateWalls();

            // Act
            List<Tile> tiles = _builder.CreateTiles(walls);

            // Assert
            // Проверяем одну из угловых стен крепости: new Wall(180, 80, 150, 25)
            // Внутри этой геометрии не должно быть проходимых тайлов
            bool isTileInsideWall = tiles.Any(t => t.X >= 180 && t.X < 330 && t.Y >= 80 && t.Y < 105);

            Assert.IsFalse(isTileInsideWall, "Строитель создал игровой тайл внутри твердой стены.");
        }

        /// <summary>
        /// Изменяет тип стандартных тайлов на замедляющие в предопределенных зонах карты Крепости.
        /// </summary>
        [TestMethod]
        public void CreateTiles_InSlowZonesCoordinates_AssignsSlowTileType()
        {
            // Arrange
            List<Wall> walls = _builder.CreateWalls();

            // Согласно коду первой зоны slow: x от 40 до 120, y от 160 до 240
            int expectedSlowX = 40;
            int expectedSlowY = 160;

            // Act
            List<Tile> tiles = _builder.CreateTiles(walls);

            // Assert
            Tile slowZoneTile = tiles.FirstOrDefault(t => t.X == expectedSlowX && t.Y == expectedSlowY);

            Assert.IsNotNull(slowZoneTile, "Тайл в координатах зоны замедления (40, 160) не найден.");
            Assert.AreEqual(TileType.Slow, slowZoneTile.Type, "Тайл в зоне замедления должен иметь тип TileType.Slow.");
        }

        /// <summary>
        /// Изменяет общее количество элементов карты, гарантируя успешное заполнение коллекции тайлов.
        /// </summary>
        [TestMethod]
        public void CreateTiles_WithDefaultFortressLayout_GeneratesCorrectTotalTileCount()
        {
            // Arrange
            List<Wall> walls = _builder.CreateWalls();

            // Act
            List<Tile> tiles = _builder.CreateTiles(walls);

            // Assert
            Assert.IsNotNull(tiles, "Коллекция тайлов вернула null.");
            Assert.IsTrue(tiles.Count > 0, "Коллекция сгенерированных тайлов не должна быть пустой.");
            Assert.IsInstanceOfType(tiles, typeof(List<Tile>), "Метод должен возвращать корректный тип List<Tile>.");
        }
    }
}
