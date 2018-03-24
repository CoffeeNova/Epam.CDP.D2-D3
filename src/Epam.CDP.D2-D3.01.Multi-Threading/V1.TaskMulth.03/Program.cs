using System;
using System.Threading.Tasks;
using Common;

namespace V1.TaskMulth._03
{
    /// <summary>
    /// Write a program, which multiplies two matrices and uses class Parallel. 
    /// </summary>
    class Program
    {
        private static readonly Random Random = new Random();

        static void Main()
        {
            var r1 = Helper.DigitsInput("Matrix #1 rows count:");
            var c1 = Helper.DigitsInput("Matrix #1 columns count:");
            var matrix1 = BuildRandomIntMatrix(r1, c1);

            var count = 2;
            while (true)
            {
                var r2 = Helper.DigitsInput($"Matrix #{count} rows count:");
                var c2 = Helper.DigitsInput($"Matrix #{count} columns count:");
                var matrix2 = BuildRandomIntMatrix(r2, c2);

                Console.WriteLine("The result of multiplying matrix1:");
                Console.Write(Environment.NewLine);
                PrintMatrixToScreen(matrix1);
                Console.Write(Environment.NewLine);
                Console.Write("by matrix2:");
                Console.Write(Environment.NewLine);
                PrintMatrixToScreen(matrix2);
                Console.Write(Environment.NewLine);
                Console.Write("Is:");
                Console.Write(Environment.NewLine);

                try
                {
                    var result = MultiplyMatrices(matrix1, matrix2);
                    PrintMatrixToScreen(result);
                    matrix1 = result;
                    count++;
                }
                catch
                {
                    var m1Rows = matrix1.GetLength(0);
                    var m1Columns= matrix1.GetLength(1);
                    var m2Rows = matrix2.GetLength(0);
                    var m2Columns = matrix2.GetLength(1);
                    var func = new Func<bool, string>(reverse =>
                    {
                        if (m2Columns < m1Rows)
                            return !reverse ? "rows" : "columns";

                        if (m2Rows < m1Columns)
                            return reverse ? "rows" : "columns";

                        throw new Exception();
                    });

                    Console.Write(
                        $"This is impossible to multipy matrices when matrix #{count} {func(false)} count less then matrix #{count - 1} {func(true)}. " +
                        "(or you have violated another rule or my algorithm too bad)");
                        Console.Write(Environment.NewLine);
                }

                Console.Write("Press ctrl+c or alt+f4 or reset button to stop this madness. Or random key to continue.");
                Console.ReadLine();

            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static int[,] MultiplyMatrices(params int[][,] matrices)
        {
            if (matrices.Length < 2)
                throw new ArgumentException("Should be equal or more than 2 matrices to multiply");

            var matrix1 = matrices[0];
            for (var matrixNumb = 1; matrixNumb < matrices.Length; matrixNumb++)
            {
                var matrix2 = matrices[matrixNumb];
                var result = new int[matrix1.GetLength(0), matrix2.GetLength(1)];

                var tempMatrix1 = matrix1;
                Parallel.For(0, matrix1.GetLength(0), i =>
                {
                    for (var j = 0; j < matrix2.GetLength(1); j++)
                    {
                        var temp = 0;
                        for (var k = 0; k < tempMatrix1.GetLength(1); k++)
                        {
                            temp += tempMatrix1[i, k] * matrix2[k, j];
                        }

                        result[i, j] = temp;
                    }
                });
                matrix1 = result;
            }

            return matrix1;
        }

        private static int[,] BuildRandomIntMatrix(int rows, int columns)
        {
            var result = new int[rows, columns];
            for (var i = 0; i < rows; i++)
                for (var j = 0; j < columns; j++)
                    result[i, j] = Random.Next(10);

            return result;
        }

        private static void PrintMatrixToScreen(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                    Console.Write($"{matrix[i, j]}    ");

                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }
    }
}
