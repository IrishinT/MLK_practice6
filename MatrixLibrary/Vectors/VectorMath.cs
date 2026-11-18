using System;

namespace MatrixLibrary.Vectors
{
    /// <summary>
    /// Содержит базовые линейно-алгебраические операции над векторами (double[]).
    /// </summary>
    public static class VectorMath
    {
        /// <summary>
        /// Вычисляет скалярное произведение двух векторов (Dot Product): sum(a[i] * b[i]).
        /// </summary>
        public static double Dot(double[] a, double[] b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));
            if (a.Length != b.Length)
                throw new ArgumentException($"Длины векторов не совпадают: {a.Length} и {b.Length}.");

            double sum = 0.0;
            for (int i = 0; i < a.Length; i++)
            {
                sum += a[i] * b[i];
            }
            return sum;
        }

        /// <summary>
        /// Вычисляет евклидову норму (L2-norm, геометрическую длину вектора в пространстве): sqrt(sum(x[i]^2)).
        /// </summary>
        public static double NormL2(double[] vector)
        {
            if (vector == null) throw new ArgumentNullException(nameof(vector));

            double sum = 0.0;
            for (int i = 0; i < vector.Length; i++)
            {
                sum += vector[i] * vector[i];
            }
            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Вычисляет манхэттенскую норму (L1-norm, сумму модулей элементов): sum(|x[i]|).
        /// Часто используется в анализе текстов и L1-регуляризации.
        /// </summary>
        public static double NormL1(double[] vector)
        {
            if (vector == null) throw new ArgumentNullException(nameof(vector));

            double sum = 0.0;
            for (int i = 0; i < vector.Length; i++)
            {
                sum += Math.Abs(vector[i]);
            }
            return sum;
        }

        /// <summary>
        /// Нормализует вектор, делая его длину (L2-норму) равной 1.0.
        /// Возвращает новый вектор единичной длины.
        /// </summary>
        public static double[] Normalize(double[] vector)
        {
            if (vector == null) throw new ArgumentNullException(nameof(vector));

            double norm = NormL2(vector);
            if (norm == 0.0)
                throw new InvalidOperationException("Невозможно нормализовать нулевой вектор.");

            double[] result = new double[vector.Length];
            double invNorm = 1.0 / norm;

            for (int i = 0; i < vector.Length; i++)
            {
                result[i] = vector[i] * invNorm;
            }

            return result;
        }

        /// <summary>
        /// Поэлементное сложение двух векторов: c = a + b.
        /// </summary>
        public static double[] Add(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Длины векторов не совпадают.");

            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i] + b[i];

            return result;
        }

        /// <summary>
        /// Поэлементное вычитание двух векторов: c = a - b.
        /// </summary>
        public static double[] Subtract(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Длины векторов не совпадают.");

            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i] - b[i];

            return result;
        }

        /// <summary>
        /// Умножение вектора на скаляр: c = a * scalar.
        /// </summary>
        public static double[] Scale(double[] vector, double scalar)
        {
            double[] result = new double[vector.Length];
            for (int i = 0; i < vector.Length; i++)
                result[i] = vector[i] * scalar;

            return result;
        }
    }
}