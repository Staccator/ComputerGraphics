using System;
using System.Windows;
using ShapeDrafter.Models;
using static System.Math;

namespace ShapeDrafter.MathOperations
{
    public class Matrix
    {
        public float[,] matrix = new float[4,4];
        public Matrix(float[] array)
        {
            int ind = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrix[j, i] = array[ind++];
                }
            }
        }
        public Matrix(float[,] array2d)
        {
            matrix = array2d;
        }

        public static Matrix TranslationMatrix(Vector3 v)
        {
            var array = new float[]
            {
                1, 0, 0, v.X,
                0, 1, 0, v.Y,
                0, 0, 1, v.Z,
                0, 0, 0, 1,
            };
            return new Matrix(array);
        }
        
        public static Matrix ScaleMatrix(Vector3 v)
        {
            var array = new float[]
            {
                v.X, 0, 0, 0,
                0, v.Y, 0, 0,
                0, 0, v.Z, 0,
                0, 0, 0, 1,
            };
            return new Matrix(array);
        }
        public static Matrix RotateXMatrix(float a)
        {
            var array = new float[]
            {
                1, 0, 0, 0,
                0, (float)Cos(a), (float)-Sin(a), 0,
                0, (float)Sin(a), (float)Cos(a), 0,
                0, 0, 0, 1,
            };
            return new Matrix(array);
        }
        public static Matrix RotateYMatrix(float a)
        {
            var array = new float[]
            {
                (float)Cos(a), 0, (float)-Sin(a), 0,
                0, 1, 0, 0,
                (float)Sin(a), 0, (float)Cos(a) , 0,
                0, 0, 0, 1,
            };
            return new Matrix(array);
        }
        public static Matrix RotateZMatrix(float a)
        {
            var array = new float[]
            {
                (float)Cos(a), (float)-Sin(a), 0, 0,
                (float)Sin(a), (float)Cos(a), 0 , 0,
                0, 0, 1, 0,
                0, 0, 0, 1,
            };
            return new Matrix(array);
        }
        public static Matrix ProjectionMatrix()
        {
            var array = new float[]
            {
                (float)4/(float)3, 0, 0, 0,
                0, 0, 1, 0,
                0, 0, 2, -3,
                0, 0, 1, 0,
            };
            return new Matrix(array);
        }
        public static Matrix ViewMatrix(float a)
        {
            var result = RotateXMatrix((float) (Cos(a) * 0.3)) * RotateYMatrix((float) (Sin(a) * 0.4));
            result = TranslationMatrix(new Vector3(0, -1, -2)) * result;
            return result;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            float [,] result = new float[4, 4];
            var a = m1.matrix;
            var b = m2.matrix;
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        result[i, j] += a[k, j] * b[i, k];
                    }
                }
            }
            return new Matrix(result);
        }
        
        public static float[] operator *(Matrix m1, float[] vector)
        {
            float [] result = new float[4];
            var a = m1.matrix;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i] += a[j, i] * vector[j];
                }
            }

            return result;
        }
        
        public static Vector4 operator *(Matrix m1, Vector4 v)
        {
            float[] result = m1 * new float[] {v.X, v.Y, v.Z, v.W};
            return new Vector4(result[0],result[1],result[2],result[3]);
        }
        
        public static Vector3 operator *(Matrix m1, Vector3 v)
        {
            float[] result = m1 * new float[] {v.X, v.Y, v.Z, 0};
            return new Vector3(result[0],result[1],result[2]);
        }

        public override string ToString()
        {
            string result = "[";
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    result += $"{matrix[i, j]},";
                }

                result += " | ";
            }

            result += "]";
            return result;
        }
        
        public Matrix Transposed()
        {
            float [,] result = new float[4, 4];
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    result[i, j] = matrix[j, i];
                }
            }
            return new Matrix(result);
        }
        
        public Matrix Inversed()
        {
            float [,] result = new float[4, 4];
            float det = Determinant();
            float multiplier = 1 / det;
            
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    result[j, i] = multiplier * TriDeterminant(i,j) * ((i+j)%2 == 0 ? 1 : -1);
                }
            }
            
            return new Matrix(result);
        }

        public float Determinant()
        {
            return TriDeterminant(0, 0) * matrix[0, 0]
                   - TriDeterminant(0, 1) * matrix[0, 1]
                   + TriDeterminant(0, 2) * matrix[0, 2]
                   - TriDeterminant(0, 3) * matrix[0, 3];
        }

        public float TriDeterminant(int x, int y)
        {
            var tab = new float[9];
            int index = 0;
            for (int j = 0; j < 4; j++)
            {
                if (j == y) continue;
                for (int i = 0; i < 4; i++)
                {
                    if (i == x) continue;
                    tab[index++] = matrix[i, j];
                }
            }

            return tab[0] * tab[4] * tab[8]
                   + tab[1] * tab[5] * tab[6]
                   + tab[2] * tab[3] * tab[7]
                   - tab[2] * tab[4] * tab[6]
                   - tab[5] * tab[7] * tab[0]
                   - tab[8] * tab[3] * tab[1];
        }
    }
}