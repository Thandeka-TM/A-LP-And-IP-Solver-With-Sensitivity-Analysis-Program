using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR381ProjectFinal
{
    public class Solution
    {
        public double ObjectiveValue { get; set; }
        public List<double> VariableValues { get; set; }

        public Solution()
        {
            VariableValues = new List<double>();
        }
    }

    //Represents a node in the tree
    public class Node
    {
        public Model Model { get; set; }
        public Solution Solution { get; set; }
    }

    public class BranchandBound
    {
        public Solution Solve(Model model)
        {
            Queue<Node> nodeQueue = new Queue<Node>();

            // Creating the root node
            Node rootNode = new Node();
            rootNode.Model = model;
            rootNode.Solution = SolveUsingSimplex(rootNode.Model);
            rootNode.Solution = SolveUsingSimplex(rootNode.Model);

            nodeQueue.Enqueue(rootNode);

            Solution bestSolution = null;

            while (nodeQueue.Count > 0)
            {
                Node currentNode = nodeQueue.Dequeue();

                if (IsIntegerSolution(currentNode.Solution))
                {
                    if (bestSolution == null || currentNode.Solution.ObjectiveValue > bestSolution.ObjectiveValue)
                    {
                        bestSolution = currentNode.Solution;
                    }
                }
                else
                {
                    int branchingVariable = SelectBranchingVariable(currentNode.Solution);

                    Node leftNode = CreateBranch(currentNode, branchingVariable, true);
                    Node rightNode = CreateBranch(currentNode, branchingVariable, false);

                    leftNode.Solution = SolveUsingSimplex(leftNode.Model);
                    rightNode.Solution = SolveUsingSimplex(rightNode.Model);

                    nodeQueue.Enqueue(leftNode);
                    nodeQueue.Enqueue(rightNode);
                }
            }

            return bestSolution;
        }

        private Solution SolveUsingSimplex(Model model)
        {
            var (tableau, numRows, numCols) = ConvertModelToTableau(model);

            //Solving using Primal Simplex
            PrimalSimplex simplex = new PrimalSimplex(tableau, numRows, numCols);
            simplex.Solve();

            return ExtractSolutionFromTableau(simplex.tableau, numRows, numCols);
        }

        //Converting LP into tableau
        private (double[,] tableau, int numRows, int numCols) ConvertModelToTableau(Model model)
        {
            int numRows = model.Constraints.Count + 1;
            int numCols = model.objfuncCoef.Count + 1;

            double[,] tableau = new double[numRows, numCols];

            for (int i = 0; i < model.Constraints.Count; i++)
            {
                for (int j = 0; j < model.Constraints[i].Count; j++)
                {
                    tableau[i, j] = model.Constraints[i][j];
                }
                tableau[i, numCols - 1] = model.rightHandSide[i];
            }

            for (int j = 0; j < model.objfuncCoef.Count; j++)
            {
                tableau[numRows - 1, j] = -model.objfuncCoef[j];
            }

            return (tableau, numRows, numCols);
        }

        private Solution ExtractSolutionFromTableau(double[,] tableau, int numRows, int numCols)
        {
            List<double> variableValues = new List<double>();
            for (int i = 0; i < numRows - 1; i++)
            {
                variableValues.Add(tableau[i, numCols - 1]);
            }

            double objectiveValue = tableau[numRows - 1, numCols - 1];

            return new Solution
            {
                ObjectiveValue = objectiveValue,
                VariableValues = variableValues
            };
        }

        private bool IsIntegerSolution(Solution solution)
        {
            foreach (var value in solution.VariableValues)
            {
                if (Math.Floor(value) != value)
                {
                    return false;
                }
            }
            return true;
        }

        private int SelectBranchingVariable(Solution solution)
        {
            for (int i = 0; i < solution.VariableValues.Count; i++)
            {
                if (Math.Floor(solution.VariableValues[i]) != solution.VariableValues[i])
                {
                    return i;
                }
            }
            return -1;
        }

        private Node CreateBranch(Node parentNode, int variableIndex, bool isLeftBranch)
        {
            var newModel = new Model
            {
                problem = parentNode.Model.problem,
                objfuncCoef = new List<double>(parentNode.Model.objfuncCoef),
                Constraints = new List<List<double>>(parentNode.Model.Constraints),
                ConstraintsRelations = new List<string>(parentNode.Model.ConstraintsRelations),
                rightHandSide = new List<double>(parentNode.Model.rightHandSide),
                signRestrictions = new List<string>(parentNode.Model.signRestrictions)
            };

            double branchingValue = isLeftBranch ? Math.Floor(parentNode.Solution.VariableValues[variableIndex])
                                                 : Math.Ceiling(parentNode.Solution.VariableValues[variableIndex]);

            var newConstraint = new List<double>(new double[newModel.objfuncCoef.Count]);
            newConstraint[variableIndex] = 1;
            newModel.Constraints.Add(newConstraint);
            newModel.ConstraintsRelations.Add(isLeftBranch ? "<=" : ">=");
            newModel.rightHandSide.Add(branchingValue);

            return new Node { Model = newModel };
        }
    }
}
