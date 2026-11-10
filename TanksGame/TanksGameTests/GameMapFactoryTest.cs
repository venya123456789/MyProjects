using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanksGame.CardFactory;

namespace TanksGameTest
{
    /// <summary>
    /// Описывает тестовый набор для проверки логики фабрики игровых карт.
    /// </summary>
    [TestClass]
    public class GameMapFactoryTest
    {
        private GameMapFactory _factory;

        /// <summary>
        /// Инициализирует экземпляр фабрики перед запуском каждого теста.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _factory = new GameMapFactory();
        }

        /// <summary>
        /// Проверяет, что при типе карты 1 фабрика возвращает строитель Арены.
        /// </summary>
        [TestMethod]
        public void GetMapBuilder_WhenMapTypeIs1_ShouldReturnArenaMapBuilder()
        {
            // Act
            IMapBuilder builder = _factory.GetMapBuilder(1);

            // Assert
            Assert.IsNotNull(builder, "Фабрика не должна возвращать null для существующего типа карты.");
            Assert.IsInstanceOfType(builder, typeof(ArenaMapBuilder), "При типе 1 должен создаваться экземпляр ArenaMapBuilder.");
        }

        /// <summary>
        /// Проверяет, что при типе карты 2 фабрика возвращает строитель Крепости.
        /// </summary>
        [TestMethod]
        public void GetMapBuilder_WhenMapTypeIs2_ShouldReturnFortressMapBuilder()
        {
            // Act
            IMapBuilder builder = _factory.GetMapBuilder(2);

            // Assert
            Assert.IsNotNull(builder, "Фабрика не должна возвращать null для существующего типа карты.");
            Assert.IsInstanceOfType(builder, typeof(FortressMapBuilder), "При типе 2 должен создаваться экземпляр FortressMapBuilder.");
        }

        /// <summary>
        /// Проверяет, что при типе карты 3 фабрика возвращает строитель Дуэли.
        /// </summary>
        [TestMethod]
        public void GetMapBuilder_WhenMapTypeIs3_ShouldReturnDuelMapBuilder()
        {
            // Act
            IMapBuilder builder = _factory.GetMapBuilder(3);

            // Assert
            Assert.IsNotNull(builder, "Фабрика не должна возвращать null для существующего типа карты.");
            Assert.IsInstanceOfType(builder, typeof(DuelMapBuilder), "При типе 3 должен создаваться экземпляр DuelMapBuilder.");
        }

        /// <summary>
        /// Проверяет, что при передаче неизвестного или некорректного значения типа фабрика возвращает строитель по умолчанию (Арена).
        /// </summary>
        [TestMethod]
        public void GetMapBuilder_WhenMapTypeIsUnknown_ShouldReturnDefaultArenaMapBuilder()
        {
            // Arrange
            int invalidMapType = -999;

            // Act
            IMapBuilder builder = _factory.GetMapBuilder(invalidMapType);

            // Assert
            Assert.IsNotNull(builder, "Фабрика не должна возвращать null при некорректных входных данных.");
            Assert.IsInstanceOfType(builder, typeof(ArenaMapBuilder), "При невалидном типе фабрика обязана вернуть строитель по умолчанию (ArenaMapBuilder).");
        }
    }
}
