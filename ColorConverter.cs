using System;
using System.Globalization;
using ColorTools.Models;

namespace ColorTools
{
    /// <summary>
    /// Набор методов для преобразования между RGB, HEX, HSL
    /// и базовых манипуляций с цветами.
    /// </summary>
    public static class ColorConverter
    {
        /// <summary>
        /// Преобразует HEX-строку (#RRGGBB или #RGB) в RGB.
        /// </summary>
        public static RgbColor HexToRgb(string hex)
        {
            string normalized = NormalizeHex(hex);

            int r = int.Parse(normalized.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            int g = int.Parse(normalized.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            int b = int.Parse(normalized.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

            return new RgbColor(r, g, b);
        }

        /// <summary>
        /// Преобразует RGB в HEX-строку формата #RRGGBB.
        /// </summary>
        public static string RgbToHex(int r, int g, int b)
        {
            var color = new RgbColor(r, g, b);
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        /// <summary>
        /// Преобразует RGB в HSL.
        /// </summary>
        public static HslColor RgbToHsl(int r, int g, int b)
        {
            var color = new RgbColor(r, g, b);

            double rd = color.R / 255.0;
            double gd = color.G / 255.0;
            double bd = color.B / 255.0;

            double max = Math.Max(rd, Math.Max(gd, bd));
            double min = Math.Min(rd, Math.Min(gd, bd));
            double delta = max - min;

            double h = 0;
            double s;
            double l = (max + min) / 2.0;

            if (delta == 0)
            {
                s = 0;
                h = 0;
            }
            else
            {
                s = delta / (1 - Math.Abs(2 * l - 1));

                if (max == rd)
                {
                    h = 60 * (((gd - bd) / delta) % 6);
                }
                else if (max == gd)
                {
                    h = 60 * (((bd - rd) / delta) + 2);
                }
                else
                {
                    h = 60 * (((rd - gd) / delta) + 4);
                }

                if (h < 0)
                    h += 360;
            }

            return new HslColor(h, s * 100, l * 100);
        }

        /// <summary>
        /// Преобразует HSL в RGB.
        /// </summary>
        public static RgbColor HslToRgb(double h, double s, double l)
        {
            var color = new HslColor(h, s, l);

            double hd = color.H / 360.0;
            double sd = color.S / 100.0;
            double ld = color.L / 100.0;

            double r;
            double g;
            double b;

            if (sd == 0)
            {
                r = g = b = ld;
            }
            else
            {
                double q = ld < 0.5
                    ? ld * (1 + sd)
                    : ld + sd - ld * sd;

                double p = 2 * ld - q;

                r = HueToRgb(p, q, hd + 1.0 / 3.0);
                g = HueToRgb(p, q, hd);
                b = HueToRgb(p, q, hd - 1.0 / 3.0);
            }

            return new RgbColor(
                (int)Math.Round(r * 255),
                (int)Math.Round(g * 255),
                (int)Math.Round(b * 255));
        }

        /// <summary>
        /// Осветляет цвет на указанный процент (0..100).
        /// </summary>
        public static RgbColor Lighten(RgbColor color, double percent)
        {
            if (color == null)
                throw new ArgumentNullException(nameof(color), "Цвет не может быть null.");

            ValidatePercent(percent, nameof(percent));

            HslColor hsl = RgbToHsl(color.R, color.G, color.B);
            double newLightness = hsl.L + (100 - hsl.L) * (percent / 100.0);

            return HslToRgb(hsl.H, hsl.S, newLightness);
        }

        /// <summary>
        /// Затемняет цвет на указанный процент (0..100).
        /// </summary>
        public static RgbColor Darken(RgbColor color, double percent)
        {
            if (color == null)
                throw new ArgumentNullException(nameof(color), "Цвет не может быть null.");

            ValidatePercent(percent, nameof(percent));

            HslColor hsl = RgbToHsl(color.R, color.G, color.B);
            double newLightness = hsl.L * (1 - percent / 100.0);

            return HslToRgb(hsl.H, hsl.S, newLightness);
        }

        /// <summary>
        /// Смешивает два цвета.
        /// ratio = 0 -> первый цвет
        /// ratio = 1 -> второй цвет
        /// </summary>
        public static RgbColor Blend(RgbColor first, RgbColor second, double ratio)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first), "Первый цвет не может быть null.");

            if (second == null)
                throw new ArgumentNullException(nameof(second), "Второй цвет не может быть null.");

            if (ratio < 0 || ratio > 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(ratio),
                    ratio,
                    "Коэффициент смешивания должен быть в диапазоне от 0 до 1.");
            }

            int r = (int)Math.Round(first.R + (second.R - first.R) * ratio);
            int g = (int)Math.Round(first.G + (second.G - first.G) * ratio);
            int b = (int)Math.Round(first.B + (second.B - first.B) * ratio);

            return new RgbColor(r, g, b);
        }

        private static string NormalizeHex(string hex)
        {
            if (hex == null)
                throw new ArgumentNullException(nameof(hex), "HEX-строка не может быть null.");

            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentException("HEX-строка не может быть пустой или состоять только из пробелов.", nameof(hex));

            string value = hex.Trim();

            if (value.StartsWith("#"))
                value = value.Substring(1);

            if (value.Length == 3)
            {
                value = string.Concat(
                    value[0], value[0],
                    value[1], value[1],
                    value[2], value[2]);
            }

            if (value.Length != 6)
                throw new FormatException("HEX-строка должна быть в формате #RGB, RGB, #RRGGBB или RRGGBB.");

            if (!IsHexString(value))
                throw new FormatException("HEX-строка содержит недопустимые символы.");

            return value.ToUpperInvariant();
        }

        private static bool IsHexString(string value)
        {
            foreach (char c in value)
            {
                bool isHex =
                    (c >= '0' && c <= '9') ||
                    (c >= 'a' && c <= 'f') ||
                    (c >= 'A' && c <= 'F');

                if (!isHex)
                    return false;
            }

            return true;
        }

        private static void ValidatePercent(double percent, string paramName)
        {
            if (percent < 0 || percent > 100)
            {
                throw new ArgumentOutOfRangeException(
                    paramName,
                    percent,
                    "Процент должен быть в диапазоне от 0 до 100.");
            }
        }

        private static double HueToRgb(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;

            if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
            if (t < 1.0 / 2.0) return q;
            if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;

            return p;
        }
    }
}