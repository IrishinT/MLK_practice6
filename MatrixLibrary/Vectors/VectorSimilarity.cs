using System;

namespace MatrixLibrary.Vectors
{
    /// <summary>
    /// Содержит метрики сходства и расстояния между векторами (включая углы).
    /// Фундаментальный инструмент для текстового поиска и сравнения документов.
    /// </summary>
    public static class VectorSimilarity
    {
        /// <summary>
        /// Вычисляет косинусное сходство (Cosine Similarity) между двумя векторами.
        /// Возвращает значение от -1.0 до 1.0 (для документов с неотрицательными весами: от 0.0 до 1.0).
        /// 1.0 - векторы сонаправлены (идентичны по смыслу).
        /// 0.0 - векторы ортогональны (угол 90 градусов, нет ничего общего).
        /// </summary>
        public static double CosineSimilarity(double[] a, double[] b)
        {
            double dot = VectorMath.Dot(a, b);
            double normA = VectorMath.NormL2(a);
            double normB = VectorMath.NormL2(b);

            if (normA == 0.0 || normB == 0.0)
                return 0.0;

            double cos = dot / (normA * normB);

            // из-за погрешностей double может выйти чуть больше 1.0
            return Math.Clamp(cos, -1.0, 1.0);
        }

        /// <summary>
        /// Вычисляет косинусное расстояние: 1.0 - CosineSimilarity.
        /// 0.0 означает, что векторы идентичны.
        /// </summary>
        public static double CosineDistance(double[] a, double[] b)
        {
            return 1.0 - CosineSimilarity(a, b);
        }

        /// <summary>
        /// Вычисляет угол между двумя векторами в радианах (от 0 до Pi).
        /// </summary>
        public static double AngleRadians(double[] a, double[] b)
        {
            double cos = CosineSimilarity(a, b);
            return Math.Acos(cos);
        }

        /// <summary>
        /// Вычисляет угол между двумя векторами в градусах (от 0° до 180°).
        /// </summary>
        public static double AngleDegrees(double[] a, double[] b)
        {
            double radians = AngleRadians(a, b);
            return radians * (180.0 / Math.PI);
        }

        /// <summary>
        /// Вычисляет обычное евклидово расстояние между концами векторов (L2-distance):
        /// sqrt(sum((a[i] - b[i])^2)).
        /// </summary>
        public static double EuclideanDistance(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Длины векторов не совпадают.");

            double sum = 0.0;
            for (int i = 0; i < a.Length; i++)
            {
                double diff = a[i] - b[i];
                sum += diff * diff;
            }
            return Math.Sqrt(sum);
        }
    }
}