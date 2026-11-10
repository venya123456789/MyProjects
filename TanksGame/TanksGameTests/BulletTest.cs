using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TanksGame.GameEntities;
using TanksGame.Managers;

namespace TanksGameTest
{
    /// <summary>
    /// Описывает тестовый набор для проверки логики класса Снаряд.
    /// </summary>
    [TestClass]
    public class BulletTest
    {
        private Tank _mockOwner;

        /// <summary>
        /// Инициализирует начальное состояние перед каждым тестом.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            // Задаем скорость снарядов в менеджере настроек, чтобы избежать деления на ноль или нулевого смещения
            SettingsManager.BulletSpeed = 5f;

            // Создаем заглушку владельца снаряда
            _mockOwner = new Tank(100f, 100f, 0f, Color.Red, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, 5);
        }

        /// <summary>
        /// Проверяет, что метод изменяет координаты снаряда в полете.
        /// </summary>
        [TestMethod]
        public void Update_ShouldMoveBulletBasedOnSpeedAndAngle()
        {
            // Arrange
            float initialX = 100f;
            float initialY = 100f;
            float angle = 0f; // При угле 0 снаряд летит строго вверх (X не меняется, Y уменьшается)

            var bullet = new Bullet(initialX, initialY, angle, _mockOwner, isMachinegun: false);

            // Act
            bullet.Update();

            // Assert
            // Проверяем, что координата Y уменьшилась, так как вектор направления движения 0° уходит вверх (Y - Cos(rad) * speed)
            Assert.AreEqual(initialX, bullet.X, 0.001f, "При угле 0 градусов смещение по оси X должно отсутствовать.");
            Assert.IsTrue(bullet.Y < initialY, "Координата Y должна уменьшиться, так как снаряд летит вверх.");
        }

        /// <summary>
        /// Проверяет, что снаряд становится уничтоженным (IsDead = true) после преодоления максимальной дистанции.
        /// </summary>
        [TestMethod]
        public void Update_WhenMaxDistanceReached_ShouldSetIsDeadToTrue()
        {
            // Arrange
            var bullet = new Bullet(100f, 100f, 0f, _mockOwner, isMachinegun: false);

            // Act
            // Делаем достаточное количество тактов обновления, чтобы снаряд пролетел больше 1000f (максимальная дистанция)
            // При скорости 5f за 201 такт он пролетит 5 * 201 = 1005f
            for (int i = 0; i < 201; i++)
            {
                bullet.Update();
            }

            // Assert
            Assert.IsTrue(bullet.IsDead, "Снаряд должен получить флаг IsDead = true после превышения максимальной дистанции полета.");
        }

        /// <summary>
        /// Проверяет, что метод возвращает ложь, пока снаряд летит внутри допустимых игровых границ.
        /// </summary>
        [TestMethod]
        public void CheckBounds_WhenInsideMap_ShouldReturnFalse()
        {
            // Arrange
            var bullet = new Bullet(600f, 400f, 0f, _mockOwner, false);

            // Act
            // Передаем границы карты в формате float, соответствующем вашей новой сигнатуре
            bool isOutOfBounds = bullet.CheckBounds(0f, 0f, 1280f, 720f);

            // Assert
            Assert.IsFalse(isOutOfBounds, "Метод должен возвращать false, если снаряд находится внутри игровой карты.");
        }

        /// <summary>
        /// Проверяет, что метод фиксирует вылет снаряда за экран и возвращает истину.
        /// </summary>
        [TestMethod]
        public void CheckBounds_WhenOutsideMap_ShouldReturnTrue()
        {
            // Arrange
            // Смещаем снаряд далеко за правую границу карты (1350 при лимите 1280)
            var bullet = new Bullet(1350f, 400f, 0f, _mockOwner, false);

            // Act
            bool isOutOfBounds = bullet.CheckBounds(0f, 0f, 1280f, 720f);

            // Assert
            Assert.IsTrue(isOutOfBounds, "Метод обязан зафиксировать выход снаряда за правый предел экрана.");
        }
    }
}
