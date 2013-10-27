namespace RacingGame
{
    using System;

    public class Counter
    {
        protected Action _action;

        public float Value { get; protected set; }

        /// <summary>
        /// Возвращает и устанавливает интервал (мс).
        /// </summary>
        public float Interval { get; set; }

        /// <summary>
        /// Создает таймер.
        /// </summary>
        /// <param name="action">Действие по истечению времени.</param>
        /// <param name="interval">Интервал (мс).</param>
        public Counter(Action action, float interval)
        {
            _action = action;
            Interval = interval;
        }

        public void Update(uint milliseconds)
        {
            if (Interval > 0)
            {
                Value += milliseconds;

                while (Value >= Interval)
                {
                    _action();
                    Value -= Interval;
                }
            }
        }

        public void Reset()
        {
            Value = 0;
        }
    }
}
