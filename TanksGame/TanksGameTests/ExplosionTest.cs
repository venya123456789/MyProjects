using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanksGame.GameEntities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.AxHost;

namespace TanksGameTest
{
    /// <summary>
    /// Тестовый класс для проверки логики, изменения состояния и жизненного цикла игрового взрыва.
    /// </summary>
    [TestClass]
    public class ExplosionTest
    {
        /// <summary>
        /// Изменяет радиус взрыва в сторону увеличения при каждом обновлении кадра.
        /// </summary>
        [TestMethod]
        public void ModifiesRadius_OnUpdateCall_RadiusIncreasesBySpecifiedStep()
        {
            // Arrange (Подготовка)
            var explosion = new Explosion(100f, 100f);

            // Для проверки изменения радиуса мы используем косвенный признак — переход в состояние IsDead,
            // так как поле radius приватное. Шаг увеличения = 4.2f. Начальный радиус = 6f.
            // Граница смерти > 80f. (80 - 6) / 4.2 = 17.61 кадра. То есть на 18 кадр взрыв должен умереть.

            // Act (Действие)
            for (int i = 0; i < 17; i++)
            {
                explosion.Update();
            }

            // Assert (Проверка)
            // На 17 шаге: 6 + (17 * 4.2) = 77.4f. Взрыв все еще должен быть жив.
            Assert.IsFalse(explosion.IsDead, "Взрыв не должен умереть до достижения радиуса 80f.");

            explosion.Update(); // 18-й шаг: 77.4 + 4.2 = 81.6f.

            // Изменяет состояние IsDead на true
            Assert.IsTrue(explosion.IsDead, "Взрыв должен быть мертв, так как радиус превысил 80f.");
        }

        /// <summary>
        /// Изменяет жизненный статус объекта на "Мертв" сразу после инициализации, если радиус изначально корректен.
        /// </summary>
        [TestMethod]
        public void ChangesLifeStatus_WhenObjectCreated_ExplosionInitiallyAlive()
        {
            // Arrange & Act
            var explosion = new Explosion(0f, 0f);

            // Assert
            Assert.IsFalse(explosion.IsDead, "Новый созданный взрыв должен иметь статус IsDead = false.");
        }

        /// <summary>
        /// Изменяет состояние за фиксированное количество тактов времени.
        /// Тест проверяет математическую точность: за сколько именно шагов взрыв гарантированно исчезнет.
        /// </summary>
        [TestMethod]
        public void ModifiesStateAcrossFrames_PreciseLifecycleCalculation()
        {
            // Arrange
            var explosion = new Explosion(500f, 400f);
            int framesCount = 0;

            // Act
            while (!explosion.IsDead)
            {
                explosion.Update();
                framesCount++;

                // Защита от бесконечного цикла, если вдруг логика Update сломается
                if (framesCount > 100)
                {
                    Assert.Fail("Взрыв не умер за 100 кадров. Логика радиуса нарушена.");
                }
            }

            // Assert
            // Математический расчет: (80 - 6) / 4.2 = 17.61 -> округляем вверх = 18 шагов.
            Assert.AreEqual(18, framesCount, "Жизненный цикл взрыва должен составлять ровно 18 вызовов Update.");
        }
    }
}
