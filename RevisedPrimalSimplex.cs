using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR381ProjectFinal
{
    class RevisedPrimalSimplex
    {
        private int numVariables;
        private int numConstraints;
        private double[,] tableau;

        public RevisedPrimalSimplex(double[,] constraints, double[] objectiveFunction)
        {
            numConstraints = constraints.GetLength(0);
            numVariables = constraints.GetLength(1);
            tableau = new double[numConstraints + 1, numVariables + numConstraints + 1];

            for (int i = 0; i < numConstraints; i++)
            {
                for (int j = 0; j < numVariables; j++)
                {
                    tableau[i, j] = constraints[i, j];
                }
                tableau[i, numVariables + i] = 1; // Slack variable
                tableau[i, tableau.GetLength(1) - 1] = 0; // Right-hand side
            }

            for (int j = 0; j < numVariables; j++)
            {
                tableau[numConstraints, j] = -objectiveFunction[j];
            }
        }

        public void Solve()
        {
            while (true)
            {
                int pivotColumn = GetPivotColumn();
                if (pivotColumn == -1) break;

                int pivotRow = GetPivotRow(pivotColumn);
                if (pivotRow == -1) throw new Exception("Unbounded solution");

                PerformPivot(pivotRow, pivotColumn);
            }

            PrintSolution();
        }

        private int GetPivotColumn()
        {
            int pivotColumn = -1;
            double minValue = 0;

            for (int j = 0; j < tableau.GetLength(1) - 1; j++)
            {
                if (tableau[tableau.GetLength(0) - 1, j] < minValue)
                {
                    minValue = tableau[tableau.GetLength(0) - 1, j];
                    pivotColumn = j;
                }
            }

            return pivotColumn;
        }

        private int GetPivotRow(int pivotColumn)
        {
            int pivotRow = -1;
            double minRatio = double.MaxValue;

            for (int i = 0; i < numConstraints; i++)
            {
                if (tableau[i, pivotColumn] > 0)
                {
                    double ratio = tableau[i, tableau.GetLength(1) - 1] / tableau[i, pivotColumn];
                    if (ratio < minRatio)
                    {
                        minRatio = ratio;
                        pivotRow = i;
                    }
                }
            }

            return pivotRow;
        }

        private void PerformPivot(int pivotRow, int pivotColumn)
        {
            double pivotValue = tableau[pivotRow, pivotColumn];

            for (int j = 0; j < tableau.GetLength(1); j++)
            {
                tableau[pivotRow, j] /= pivotValue;
            }

            for (int i = 0; i < tableau.GetLength(0); i++)
            {
                if (i != pivotRow)
                {
                    double factor = tableau[i, pivotColumn];
                    for (int j = 0; j < tableau.GetLength(1); j++)
                    {
                        tableau[i, j] -= factor * tableau[pivotRow, j];
                    }
                }
            }
        }

        private void PrintSolution()
        {
            Console.WriteLine("Optimal Solution:");
            for (int j = 0; j < numVariables; j++)
            {
                double value = 0;
                for (int i = 0; i < numConstraints; i++)
                {
                    if (tableau[i, j] == 1)
                    {
                        value = tableau[i, tableau.GetLength(1) - 1];
                        break;
                    }
                }
                Console.WriteLine($"x[{j}] = {value}");
            }
            Console.WriteLine($"Optimal Value = {tableau[tableau.GetLength(0) - 1, tableau.GetLength(1) - 1]}");
        }
    }
}
