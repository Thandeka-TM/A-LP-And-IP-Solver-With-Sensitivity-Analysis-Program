using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR381ProjectFinal
{
    public class PrimalSimplex
    {
        public double[,] tableau;
        public int numRows, numCols;

        public PrimalSimplex(double[,] tableau, int numRows, int numCols)
        {
            this.tableau = tableau;
            this.numRows = numRows;
            this.numCols = numCols;
        }

        // This method is now public and can be called from outside the class
        public void Solve()
        {
            while (true)
            {
                int pivotCol = SelectPivotColumn();
                if (pivotCol == -1) break;

                int pivotRow = SelectPivotRow(pivotCol);
                if (pivotRow == -1)
                {
                    Console.WriteLine("Unbounded solution.");
                    return;
                }

                Pivot(pivotRow, pivotCol);
            }

            PrintSolution();
        }

        private int SelectPivotColumn()
        {
            int pivotCol = -1;
            double minValue = 0;

            for (int j = 0; j < numCols - 1; j++)
            {
                if (tableau[numRows - 1, j] < minValue)
                {
                    minValue = tableau[numRows - 1, j];
                    pivotCol = j;
                }
            }

            return pivotCol;
        }

        private int SelectPivotRow(int pivotCol)
        {
            int pivotRow = -1;
            double minRatio = double.PositiveInfinity;

            for (int i = 0; i < numRows - 1; i++)
            {
                if (tableau[i, pivotCol] > 0)
                {
                    double ratio = tableau[i, numCols - 1] / tableau[i, pivotCol];
                    if (ratio < minRatio)
                    {
                        minRatio = ratio;
                        pivotRow = i;
                    }
                }
            }

            return pivotRow;
        }

        private void Pivot(int pivotRow, int pivotCol)
        {
            double pivotValue = tableau[pivotRow, pivotCol];

            for (int j = 0; j < numCols; j++)
            {
                tableau[pivotRow, j] /= pivotValue;
            }

            for (int i = 0; i < numRows; i++)
            {
                if (i != pivotRow)
                {
                    double factor = tableau[i, pivotCol];
                    for (int j = 0; j < numCols; j++)
                    {
                        tableau[i, j] -= factor * tableau[pivotRow, j];
                    }
                }
            }
        }

        private void PrintSolution()
        {
            for (int i = 0; i < numRows - 1; i++)
            {
                Console.WriteLine($"x{i + 1} = {tableau[i, numCols - 1]}");
            }
            Console.WriteLine($"Optimal value: {tableau[numRows - 1, numCols - 1]}");
        }
    }
}
