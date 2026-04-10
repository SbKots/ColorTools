using System;

namespace ColorTools.Models
{
    /// <summary>
    /// Представляет цвет в модели RGB.
    /// </summary>
    public sealed class RgbColor
    {
        public int R { get; }
        public int G { get; }
        public int B { get; }

        public RgbColor(int r, int g, int b)
        {
            ValidateComponent(r, nameof(r));
            ValidateComponent(g, nameof(g));
            ValidateComponent(b, nameof(b));

            R = r;
            G = g;
            B = b;
        }

        private static void ValidateComponent(int value, string paramName)
        {
            if (value < 0 || value > 255)
            {
                throw new ArgumentOutOfRangeException(
                    paramName,
                    value,
                    "Компонент RGB должен быть в диапазоне от 0 до 255.");
            }
        }

        public override string ToString()
        {
            return $"RGB({R}, {G}, {B})";
        }
    }
}