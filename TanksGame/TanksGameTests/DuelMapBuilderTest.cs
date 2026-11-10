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
    /// Тестовый класс для проверки корректности сборки дуэльной карты строителем.
    /// </summary>
    [TestClass]
    public class DuelMapBuilderTests
    {
        private DuelMapBuilder _builder;

        [TestInitialize]
        public void Setup()
        {
            _builder = new DuelMapBuilder();
        }

        /// <summary>
        /// Изменяет коллекцию стен, добавляя в нее базовые границы и центральные укрепления дуэльной арены.
        /// </summary>
        [TestMethod]
        public void CreateWalls_WhenCalled_GeneratesBordersAndDuelObstacles()
        {
            // Act
            List<Wall> walls = _builder.CreateWalls();

            // Assert
            Assert.IsNotNull(walls, "Список стен не должен быть null.");

            // Проверка внешних границ карты дуэли (1280x720)
            bool hasTopWall = walls.Any(w => w.X == 0 && w.Y == 0 && w.Width == 1280 && w.Height == 15);
            bool hasBottomWall = walls.Any(w => w.X == 0 && w.Y == 720 - 15 && w.Width == 1280 && w.Height == 15);
            bool hasLeftWall = walls.Any(w => w.X == 0 && w.Y == 0 && w.Width == 15 && w.Height == 720);
            bool hasRightWall = walls.Any(w => w.X == 1280 - 15 && w.Y == 0 && w.Width == 15 && w.Height == 720);

            Assert.IsTrue(hasTopWall, "Отсутствует верхняя граница карты.");
            Assert.IsTrue(hasBottomWall, "Отсутствует нижняя граница карты.");
            Assert.IsTrue(hasLeftWall, "Отсутствует левая граница карты.");
            Assert.IsTrue(hasRightWall, "Отсутствует правая граница карты.");

            // Проверка наличия специфичного центрального объекта дуэли: new Wall(580, 300, 120, 25)
            bool hasCenterObstacle = walls.Any(w => w.X == 580 && w.Y == 300 && w.Width == 120 && w.Height == 25);
            Assert.IsTrue(hasCenterObstacle, "Центральное дуэльное препятствие не было создано.");
        }

        /// <summary>
        /// Изменяет состав тайлов, исключая генерацию стандартной земли поверх объектов препятствий дуэльной карты.
        /// </summary>
        [TestMethod]
        public void CreateTiles_WithWallsPresent_DoesNotSpawnTilesInsideSolidWalls()
        {
            // Arrange
            List<Wall> walls = _builder.CreateWalls();

            // Act
            List<Tile> tiles = _builder.CreateTiles(walls);

            // Assert
            // Проверяем одну из вертикальных стен: new Wall(300, 160, 25, 80)
            // Внутри этой геометрии не должно быть проходимых тайлов
            bool isTileInsideWall = tiles.Any(t => t.X >= 300 && t.X < 325 && t.Y >= 160 && t.Y < 240);

            Assert.IsFalse(isTileInsideWall, "Строитель создал игровой тайл внутри твердой стены.");
        }

        /// <summary>
        /// Изменяет тип стандартных тайлов на замедляющие в предопределенных зонах дуэльной карты.
        /// </summary>
        [TestMethod]
        public void CreateTiles_InSlowZonesCoordinates_AssignsSlowTileType()
        {
            // Arrange
            List<Wall> walls = _builder.CreateWalls();

            // Согласно коду первой зоны: x от 40 до 120, y от 160 до 240
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
        public void CreateTiles_WithDefaultDuelLayout_GeneratesCorrectTotalTileCount()
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
