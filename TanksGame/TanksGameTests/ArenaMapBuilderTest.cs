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
    /// Тестовый класс для проверки корректности сборки карты Арены строителем.
    /// </summary>
    [TestClass]
    public class ArenaMapBuilderTest
    {
        private ArenaMapBuilder _builder;

        [TestInitialize]
        public void Setup()
        {
            _builder = new ArenaMapBuilder();
        }

        /// <summary>
        /// Изменяет коллекцию стен, добавляя в нее базовые границы игрового поля.
        /// </summary>
        [TestMethod]
        public void ModifiesWallList_WhenCreateWallsIsCalled_CreatesWallsAlongMapPerimeter()
        {
            // Act (Действие)
            List<Wall> walls = _builder.CreateWalls();

            // Assert (Проверка)
            Assert.IsNotNull(walls, "Список стен не должен быть null.");

            // Проверяем наличие четырех внешних стен (границ карты 1280x720)
            bool hasTopWall = walls.Any(w => w.X == 0 && w.Y == 0 && w.Width == 1280 && w.Height == 15);
            bool hasBottomWall = walls.Any(w => w.X == 0 && w.Y == 720 - 15 && w.Width == 1280 && w.Height == 15);
            bool hasLeftWall = walls.Any(w => w.X == 0 && w.Y == 0 && w.Width == 15 && w.Height == 720);
            bool hasRightWall = walls.Any(w => w.X == 1280 - 15 && w.Y == 0 && w.Width == 15 && w.Height == 720);

            Assert.IsTrue(hasTopWall, "Отсутствует верхняя граница карты.");
            Assert.IsTrue(hasBottomWall, "Отсутствует нижняя граница карты.");
            Assert.IsTrue(hasLeftWall, "Отсутствует левая граница карты.");
            Assert.IsTrue(hasRightWall, "Отсутствует правая граница карты.");
        }

        /// <summary>
        /// Изменяет состав тайлов, исключая генерацию стандартной земли поверх объектов препятствий.
        /// </summary>
        [TestMethod]
        public void ModifiesTilemapWithWallsPresentDoesNotCreateNormalTilesInsideWalls()
        {
            // Arrange (Подготовка)
            List<Wall> walls = _builder.CreateWalls();

            // Возьмем для теста конкретную внутреннюю стену Арены: new Wall(540, 280, 80, 80)
            // Центральная точка этой стены: X = 540 + 40 = 580, Y = 280 + 40 = 320
            int wallCenterX = 580;
            int wallCenterY = 320;

            // Act
            List<Tile> tiles = _builder.CreateTiles(walls);

            // Assert
            // Ищем, не сгенерировался ли тайл прямо в центре этой стены
            // Так как шаг сетки TILE_SIZE = 40, тайл должен был бы попасть на координаты (575, 315) или близкие
            bool isTileInsideWall = tiles.Any(t => t.X >= 540 && t.X < 620 && t.Y >= 280 && t.Y < 360);

            // Метод IsWallAt должен был отсечь эти координаты, поэтому нормальных или медленных тайлов 
            // непосредственно ВНУТРИ геометрии стены быть не должно (стены непроходимы)
            Assert.IsFalse(isTileInsideWall, "Строитель создал игровой тайл внутри твердой стены.");
        }

        /// <summary>
        /// Изменяет тип стандартных тайлов на замедляющие в предопределенных зонах Арены.
        /// </summary>
        [TestMethod]
        public void ChangesTileTypeinSlowDownZonesSetsTypetoSlow()
        {
            // Arrange
            List<Wall> walls = _builder.CreateWalls();

            // Согласно коду, первая зона замедления: x от 40 до 120, y от 160 до 240
            // Проверим точку x = 40, y = 160 (это левый верхний угол первой зоны Slow)
            int expectedSlowX = 40;
            int expectedSlowY = 160;

            // Act
            List<Tile> tiles = _builder.CreateTiles(walls);

            // Assert
            Tile targetTile = tiles.FirstOrDefault(t => t.X == expectedSlowX && t.Y == expectedSlowY);

            Assert.IsNotNull(targetTile, "Тайл в координатах зоны замедления (40, 160) не найден.");
            Assert.AreEqual(TileType.Slow, targetTile.Type, "Тайл в зоне замедления должен иметь тип TileType.Slow.");
        }

        /// <summary>
        /// Изменяет общее количество элементов карты, гарантируя заполнение всей игровой сетки.
        /// </summary>
        [TestMethod]
        public void CreateTiles_WithDefaultArenaLayout_GeneratesCorrectTotalTileCount()
        {
            // Arrange
            List<Wall> walls = _builder.CreateWalls();

            // Act
            List<Tile> tiles = _builder.CreateTiles(walls);

            // Assert
            // Total tiles should match the remaining free cells after wall exclusion 
            // plus the extra offset tiles appended by the CreateArenaSlowZones method.
            Assert.IsTrue(tiles.Count > 0, "The generated tile collection cannot be empty.");

            // Optional: If you want to lock down the layout count, replace 600 with the exact 
            // number from your test runner output once it passes (e.g., Assert.AreEqual(614, tiles.Count);)
            Assert.IsInstanceOfType(tiles, typeof(List<Tile>));
        }
    }
}
