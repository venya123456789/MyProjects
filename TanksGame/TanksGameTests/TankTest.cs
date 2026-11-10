using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TanksGame.GameEntities;

namespace TanksGameTest
{
    /// <summary>
    /// Описывает тестовый набор для проверки комплексной логики класса Танк.
    /// </summary>
    [TestClass]
    public class TankTest
    {
        private Tank _tank;
        private Tank _enemyTank;
        private bool[] _keysPressed;
        private List<Wall> _walls;
        private List<Tile> _emptyTiles;
        private List<Explosion> _emptyExplosions;

        /// <summary>
        /// Инициализирует начальное состояние окружения перед запуском каждого индивидуального теста.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            // Массив состояний клавиш инициализируется размером 256, как заложено в логике Update
            _keysPressed = new bool[256];
            _walls = new List<Wall>();
            _emptyTiles = new List<Tile>();
            _emptyExplosions = new List<Explosion>();

            // Инициализирует танк игрока
            _tank = new Tank(
                100f,         // x
                100f,         // y
                0f,           // startAngle
                Color.Red,    // color
                Keys.W,       // up
                Keys.S,       // down
                Keys.A,       // left
                Keys.D,       // right
                Keys.Space,   // fire
                Keys.Q,       // switchWeapon
                5             // hp
            );

            // Инициализирует вражеский танк для передачи в метод Update
            _enemyTank = new Tank(500f, 500f, 180f, Color.Blue, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, 5);
        }

        /// <summary>
        /// Проверяет корректность активации модификатора энергетического щита.
        /// </summary>
        [TestMethod]
        public void ApplyBuff_WhenShieldBuff_ShouldEnableShieldProperty()
        {
            // Act
            _tank.ApplyBuff(BuffType.Shield);

            // Assert
            Assert.IsTrue(_tank.HasShield, "После наложения баффа Shield свойство HasShield обязано стать истинным.");
        }

        /// <summary>
        /// Проверяет, что очистка баффов полностью сбрасывает все активные эффекты защиты.
        /// </summary>
        [TestMethod]
        public void ClearBuff_ShouldResetAllBuffProperties()
        {
            // Arrange
            _tank.ApplyBuff(BuffType.Shield);

            // Act
            _tank.ClearBuff();

            // Assert
            Assert.IsFalse(_tank.HasShield, "После вызова метода очистки модификаторов щит должен отключиться.");
        }

        /// <summary>
        /// Проверяет успешную регистрацию урона при контакте со снарядом.
        /// </summary>
        [TestMethod]
        public void IsHitBy_WhenBulletTouchesTank_ShouldReturnTrue()
        {
            // Arrange
            // Согласно формуле (dx^2 + dy^2 < 17 * 17 * 2.8), снаряд на расстоянии 10 пикселей гарантированно попадает в танк
            var enemyBullet = new Bullet(_tank.X + 10f, _tank.Y, 0f, _enemyTank, false);

            // Act
            bool isHit = _tank.IsHitBy(enemyBullet);

            // Assert
            Assert.IsTrue(isHit, "Метод обязан возвращать true, если снаряд находится внутри хитбокса танка.");
        }

        /// <summary>
        /// Проверяет, что при обработке нажатия клавиши движения танк изменяет свои координаты.
        /// </summary>
        [TestMethod]
        public void Update_WhenMoveForwardKeyPressed_ShouldChangeTankCoordinates()
        {
            // Arrange
            _keysPressed[(int)Keys.W] = true; // Зажимаем клавишу "Вперед"
            float initialY = _tank.Y;

            // Нам нужно симулировать несколько тиков, так как скорость танка нарастает плавно:
            // currentSpeed += (target - currentSpeed) * 0.25f;
            // Делаем 5 итераций для преодоления инерции разгона
            for (int i = 0; i < 5; i++)
            {
                _tank.Update(_keysPressed, _walls, _emptyTiles, _enemyTank, _emptyExplosions);
            }

            // Assert
            // При угле 0 градусов движение "Вперед" уменьшает координату Y (Y - Cos(rad) * speed)
            Assert.IsTrue(_tank.Y < initialY, "Координата Y должна уменьшиться, так как при угле 0 танк ускоряется и едет вверх.");
        }

        /// <summary>
        /// Проверяет, что физический движок танка блокирует перемещение сквозь установленные стены.
        /// </summary>
        [TestMethod]
        public void Update_WhenTankHitsWall_ShouldNotPassThroughIt()
        {
            // Arrange
            _keysPressed[(int)Keys.W] = true;

            // В вашем коде IsColliding проверяет стену на расстоянии: TANK_HALF_SIZE * 1.4f (17 * 1.4 = 23.8 пикселей)
            // При угле 0 движение идет ВВЕРХ (в сторону уменьшения Y). Ставим стену прямо над танком (Y = 100 - 20 = 80)
            _walls.Add(new Wall(_tank.X, _tank.Y - 20f, 40f, 40f));
            float initialY = _tank.Y;

            // Act
            for (int i = 0; i < 5; i++)
            {
                _tank.Update(_keysPressed, _walls, _emptyTiles, _enemyTank, _emptyExplosions);
            }

            // Assert
            Assert.AreEqual(initialY, _tank.Y, "Координаты танка должны остаться неизменными, так как движение заблокировано коллизией со стеной.");
        }

        /// <summary>
        /// Проверяет автоматическое уменьшение доступного боезапаса при совершении выстрела в обычном режиме.
        /// </summary>
        [TestMethod]
        public void Update_WhenFireKeyPressed_ShouldReduceRegularAmmo()
        {
            // Arrange
            _keysPressed[(int)Keys.Space] = true; // Симулируем нажатие клавиши атаки (Fire)
            int initialAmmo = _tank.RegularAmmo;

            // Act
            _tank.Update(_keysPressed, _walls, _emptyTiles, _enemyTank, _emptyExplosions);

            // Assert
            Assert.AreEqual(initialAmmo - 1, _tank.RegularAmmo, "Количество обычных патронов в магазине должно уменьшиться на 1 после совершения выстрела.");
        }

        /// <summary>
        /// Проверяет смену режима ведения огня при нажатии на соответствующую клавишу.
        /// </summary>
        [TestMethod]
        public void Update_WhenSwitchWeaponKeyPressed_ShouldToggleMachinegunMode()
        {
            // Arrange
            _keysPressed[(int)Keys.Q] = true; // Нажимаем кнопку смены оружия
            Assert.IsFalse(_tank.IsMachinegunMode, "Изначально танк должен находиться в стандартном режиме стрельбы.");

            // Act
            _tank.Update(_keysPressed, _walls, _emptyTiles, _enemyTank, _emptyExplosions);

            // Assert
            Assert.IsTrue(_tank.IsMachinegunMode, "После обработки нажатия клавиши танк обязан переключиться в режим пулемета.");
        }
    }
}
