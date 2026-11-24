using System;
using System.Globalization;
using MatrixLibrary.Core;
using QueueLibrary.Dequeue;

namespace MLK_practice6
{
    /// <summary>
    /// Неизменяемый класс полинома (многочлена) одной переменной с арифметическими операциями.
    /// Индекс массива коэффициентов соответствует степени x: P(x) = sum(coeffs[i] * x^i).
    /// </summary>
    public class Polynomial
    {
        private const double Epsilon = 1e-12;
        private readonly double[] _coefficients;

        public static Polynomial Zero { get; } = new Polynomial(0.0);
        public static Polynomial One { get; } = new Polynomial(1.0);

        /// <summary>
        /// Степень полинома (для нулевого и константного полинома равна 0).
        /// </summary>
        public int Degree => _coefficients.Length - 1;

        /// <summary>
        /// Показывает, является ли полином тождественным нулём.
        /// </summary>
        public bool IsZero => _coefficients.Length == 1 && Math.Abs(_coefficients[0]) < Epsilon;

        /// <summary>
        /// Коэффициент при x^degree.
        /// </summary>
        public double this[int degree]
        {
            get
            {
                if (degree < 0)
                    throw new ArgumentOutOfRangeException(nameof(degree), "Степень не может быть отрицательной.");

                return degree < _coefficients.Length ? _coefficients[degree] : 0.0;
            }
        }

        /// <summary>
        /// Создаёт полином из коэффициентов от младшей степени к старшей: [a0, a1, a2] => a0 + a1*x + a2*x^2.
        /// </summary>
        public Polynomial(params double[] coefficients)
        {
            if (coefficients == null)
                throw new ArgumentNullException(nameof(coefficients));

            int lastNonZero = coefficients.Length - 1;
            while (lastNonZero > 0 && Math.Abs(coefficients[lastNonZero]) < Epsilon)
                lastNonZero--;

            if (coefficients.Length == 0 || (lastNonZero == 0 && Math.Abs(coefficients[0]) < Epsilon))
            {
                _coefficients = new[] { 0.0 };
                return;
            }

            _coefficients = new double[lastNonZero + 1];
            for (int i = 0; i <= lastNonZero; i++)
            {
                _coefficients[i] = Math.Abs(coefficients[i]) < Epsilon ? 0.0 : coefficients[i];
            }
        }

        /// <summary>
        /// Сложение полиномов: A + B.
        /// </summary>
        public Polynomial Add(Polynomial other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            int maxLen = Math.Max(_coefficients.Length, other._coefficients.Length);
            double[] result = new double[maxLen];

            for (int i = 0; i < maxLen; i++)
                result[i] = this[i] + other[i];

            return new Polynomial(result);
        }

        /// <summary>
        /// Вычитание полиномов: A - B.
        /// </summary>
        public Polynomial Subtract(Polynomial other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            int maxLen = Math.Max(_coefficients.Length, other._coefficients.Length);
            double[] result = new double[maxLen];

            for (int i = 0; i < maxLen; i++)
                result[i] = this[i] - other[i];

            return new Polynomial(result);
        }

        /// <summary>
        /// Смена знака всех коэффициентов: -A.
        /// </summary>
        public Polynomial Negate()
        {
            double[] result = new double[_coefficients.Length];
            for (int i = 0; i < _coefficients.Length; i++)
                result[i] = -_coefficients[i];

            return new Polynomial(result);
        }

        /// <summary>
        /// Умножение полиномов: A * B.
        /// </summary>
        public Polynomial Multiply(Polynomial other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (IsZero || other.IsZero) return Zero;

            double[] result = new double[Degree + other.Degree + 1];

            for (int i = 0; i <= Degree; i++)
            {
                if (Math.Abs(_coefficients[i]) < Epsilon) continue;

                for (int j = 0; j <= other.Degree; j++)
                {
                    result[i + j] += _coefficients[i] * other._coefficients[j];
                }
            }

            return new Polynomial(result);
        }

        /// <summary>
        /// Умножение полинома на число.
        /// </summary>
        public Polynomial Multiply(double scalar)
        {
            if (IsZero || Math.Abs(scalar) < Epsilon)
                return Zero;

            double[] result = new double[_coefficients.Length];
            for (int i = 0; i < _coefficients.Length; i++)
                result[i] = _coefficients[i] * scalar;

            return new Polynomial(result);
        }

        /// <summary>
        /// Деление с остатком: возвращает (Quotient, Remainder), такие что A = Quotient * B + Remainder.
        /// </summary>
        public (Polynomial Quotient, Polynomial Remainder) DivideWithRemainder(Polynomial divisor)
        {
            if (divisor == null) throw new ArgumentNullException(nameof(divisor));
            if (divisor.IsZero)
                throw new DivideByZeroException("Деление на нулевой полином невозможно.");

            if (IsZero || Degree < divisor.Degree)
                return (Zero, this);

            int degA = Degree;
            int degB = divisor.Degree;
            double leadB = divisor[degB];

            double[] rem = new double[_coefficients.Length];
            Array.Copy(_coefficients, rem, _coefficients.Length);

            double[] quot = new double[degA - degB + 1];

            for (int k = degA - degB; k >= 0; k--)
            {
                double coeff = rem[k + degB] / leadB;
                if (Math.Abs(coeff) < Epsilon)
                {
                    rem[k + degB] = 0.0;
                    continue;
                }

                quot[k] = coeff;
                for (int j = 0; j <= degB; j++)
                {
                    rem[k + j] -= coeff * divisor[j];
                }
                rem[k + degB] = 0.0;
            }

            return (new Polynomial(quot), new Polynomial(rem));
        }

        /// <summary>
        /// Деление без остатка (возвращает частное от деления).
        /// </summary>
        public Polynomial Divide(Polynomial divisor)
        {
            return DivideWithRemainder(divisor).Quotient;
        }

        /// <summary>
        /// Остаток от деления полинома на divisor.
        /// </summary>
        public Polynomial Modulo(Polynomial divisor)
        {
            return DivideWithRemainder(divisor).Remainder;
        }

        /// <summary>
        /// Точное деление без остатка. Выбрасывает исключение, если полином не делится нацело.
        /// </summary>
        public Polynomial DivideExact(Polynomial divisor)
        {
            var (quotient, remainder) = DivideWithRemainder(divisor);
            if (!remainder.IsZero)
                throw new InvalidOperationException($"Полином ({this}) не делится нацело на ({divisor}). Остаток: {remainder}");

            return quotient;
        }

        /// <summary>
        /// Вычисляет значение полинома в точке x (метод Горнера).
        /// </summary>
        public double Evaluate(double x)
        {
            double result = 0.0;
            for (int i = Degree; i >= 0; i--)
                result = result * x + _coefficients[i];

            return result;
        }

        /// <summary>
        /// Возвращает первую производную полинома.
        /// </summary>
        public Polynomial Derivative()
        {
            if (Degree == 0)
                return Zero;

            double[] result = new double[Degree];
            for (int i = 1; i <= Degree; i++)
                result[i - 1] = _coefficients[i] * i;

            return new Polynomial(result);
        }

        /// <summary>
        /// Преобразует полином в разреженную строку-матрицу [1 x (Degree + 1)] из MatrixLibrary.
        /// </summary>
        public CsrMatrix ToCsrMatrix()
        {
            var builder = new SparseMatrixBuilder(1, _coefficients.Length, isSymmetric: false);
            for (int i = 0; i < _coefficients.Length; i++)
            {
                if (Math.Abs(_coefficients[i]) >= Epsilon)
                    builder[0, i] = _coefficients[i];
            }
            return builder.Build();
        }

        public bool ApproximatelyEquals(Polynomial other, double tolerance = 1e-9)
        {
            if (other == null) return false;

            int maxLen = Math.Max(_coefficients.Length, other._coefficients.Length);
            for (int i = 0; i < maxLen; i++)
            {
                if (Math.Abs(this[i] - other[i]) > tolerance)
                    return false;
            }
            return true;
        }

        public static Polynomial operator +(Polynomial a, Polynomial b) => a.Add(b);
        public static Polynomial operator -(Polynomial a, Polynomial b) => a.Subtract(b);
        public static Polynomial operator -(Polynomial a) => a.Negate();
        public static Polynomial operator *(Polynomial a, Polynomial b) => a.Multiply(b);
        public static Polynomial operator *(Polynomial a, double scalar) => a.Multiply(scalar);
        public static Polynomial operator *(double scalar, Polynomial a) => a.Multiply(scalar);
        public static Polynomial operator /(Polynomial a, Polynomial b) => a.Divide(b);
        public static Polynomial operator %(Polynomial a, Polynomial b) => a.Modulo(b);
        public static Polynomial operator /(Polynomial a, double scalar)
        {
            if (Math.Abs(scalar) < Epsilon)
                throw new DivideByZeroException("Деление полинома на ноль.");
            return a.Multiply(1.0 / scalar);
        }

        public override string ToString()
        {
            if (IsZero)
                return "0";

            var parts = new ArrayDeque<string>(_coefficients.Length);
            bool isFirst = true;

            for (int power = Degree; power >= 0; power--)
            {
                double coeff = _coefficients[power];
                if (Math.Abs(coeff) < Epsilon)
                    continue;

                bool negative = coeff < 0;
                double absCoeff = Math.Abs(coeff);

                string signPrefix = isFirst
                    ? (negative ? "-" : "")
                    : (negative ? " - " : " + ");

                string coeffStr = (power > 0 && Math.Abs(absCoeff - 1.0) < Epsilon)
                    ? ""
                    : absCoeff.ToString("G12", CultureInfo.InvariantCulture);

                string varStr = power switch
                {
                    0 => "",
                    1 => "x",
                    _ => $"x^{power}"
                };

                parts.OfferLast(signPrefix + coeffStr + varStr);
                isFirst = false;
            }

            if (parts.IsEmpty())
                return "0";

            string[] tokens = new string[parts.GetCount()];
            int idx = 0;
            while (parts.HasNext())
                tokens[idx++] = parts.PullFirst();

            return string.Concat(tokens);
        }
    }
}