using System;
using MatrixLibrary.Core;

namespace MatrixLibrary.Search
{
    /// <summary>
    /// Преобразует матрицу частот слов (Term-Document Matrix) в нормализованную матрицу TF-IDF.
    /// Строки матрицы представляют документы, столбцы - слова/термины.
    /// </summary>
    public static class TfIdfTransformer
    {
        /// <summary>
        /// Применяет формулу TF-IDF и выполняет L2-нормализацию строк.
        /// </summary>
        /// <param name="termCountMatrix">Сырая матрица [Документы x Слова].</param>
        /// <returns>Взвешенная разреженная матрица TF-IDF.</returns>
        public static CsrMatrix Transform(CsrMatrix termCountMatrix)
        {
            if (termCountMatrix == null) throw new ArgumentNullException(nameof(termCountMatrix));

            int totalDocs = termCountMatrix.Rows;
            int vocabSize = termCountMatrix.Columns;

            // DF - в скольких документах встретилось слово j
            int[] df = new int[vocabSize];
            for (int i = 0; i < totalDocs; i++)
            {
                foreach (var (wordIdx, _) in termCountMatrix.GetRowElements(i))
                {
                    df[wordIdx]++;
                }
            }

            // IDF со сглаживанием: idf(t) = ln((1 + N) / (1 + df(t))) + 1.0
            double[] idf = new double[vocabSize];
            for (int j = 0; j < vocabSize; j++)
            {
                idf[j] = Math.Log((1.0 + totalDocs) / (1.0 + df[j])) + 1.0;
            }

            // считаем TF-IDF и нормализуем строки (L2), чтобы длина документа не влияла на релевантность
            var builder = new SparseMatrixBuilder(totalDocs, vocabSize, isSymmetric: false);

            for (int i = 0; i < totalDocs; i++)
            {
                double sumSquares = 0.0;
                foreach (var (wordIdx, count) in termCountMatrix.GetRowElements(i))
                {
                    double tfIdfValue = count * idf[wordIdx];
                    sumSquares += tfIdfValue * tfIdfValue;
                }

                double rowNorm = Math.Sqrt(sumSquares);
                if (rowNorm == 0.0) continue;

                double invNorm = 1.0 / rowNorm;

                foreach (var (wordIdx, count) in termCountMatrix.GetRowElements(i))
                {
                    double finalWeight = (count * idf[wordIdx]) * invNorm;
                    builder[i, wordIdx] = finalWeight;
                }
            }

            return builder.Build();
        }
    }
}