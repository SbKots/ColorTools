using System;

namespace ColorTools.Models
{
    /// <summary>
    /// Представляет цвет в модели HSL.
    /// H: 0..360, S: 0..100, L: 0..100
    /// </summary>
    public sealed class HslColor
    {
        public double H { get; }
        public double S { get; }
        public double L { get; }

        public HslColor(double h, double s, double l)
        {
            if (h < 0 || h > 360)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(h),
                    h,
                    "Оттенок H должен быть в диапазоне от 0 до 360.");
            }

            if (s < 0 || s > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(s),
                    s,
                    "Насыщенность S должна быть в диапазоне от 0 до 100.");
            }

            if (l < 0 || l > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(l),
                    l,
                    "Светлота L должна быть в диапазоне от 0 до 100.");
            }

            H = h == 360 ? 0 : h;
            S = s;
            L = l;
        }

        public override string ToString()
        {
            return $"HSL({H:0.##}, {S:0.##}%, {L:0.##}%)";
        }
    }
}