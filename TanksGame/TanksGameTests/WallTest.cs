using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanksGame.GameEntities;

namespace TanksGameTest
{
    /// <summary>
    /// Описывает тестовый набор для проверки логики класса Стена.
    /// </summary>
    [TestClass]
    public class WallTest
    {
        /// <summary>
        /// Проверяет, что метод обнаруживает столкновение, когда радиус объекта пересекает прямоугольник стены.
        /// </summary>
        [TestMethod]
        public void Contains_WhenObjectCollidesWithWall_ShouldReturnTrue()
        {
            // Arrange
            float wallX = 100f;
            float wallY = 100f;
            float wallWidth = 50f;
            float wallHeight = 50f;
            var wall = new Wall(wallX, wallY, wallWidth, wallHeight);

            // Объект находится слева от стены (X = 95), но при радиусе 10 его правый край (95 + 10 = 105) заходит внутрь стены
            float objectX = 95f;
            float objectY = 125f;
            float radius = 10f;

            // Act
            bool result = wall.Contains(objectX, objectY, radius);

            // Assert
            Assert.IsTrue(result, "Метод должен фиксировать столкновение, если окружность пересекает границы стены.");
        }

        /// <summary>
        /// Проверяет, что метод возвращает ложь, если объект находится на дистанции от стены.
        /// </summary>
        [TestMethod]
        public void Contains_WhenObjectIsFarFromWall_ShouldReturnFalse()
        {
            // Arrange
            var wall = new Wall(100f, 100f, 50f, 50f);

            // Act
            bool result = wall.Contains(500f, 500f, 5f);

            // Assert
            Assert.IsFalse(result, "Столкновение не должно происходить при отсутствии физического контакта.");
        }
    }
}

