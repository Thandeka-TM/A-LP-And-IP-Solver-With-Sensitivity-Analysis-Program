using Google.OrTools.LinearSolver;
using System;
using System.Collections.Generic;

namespace LPR381ProjectFinal
{
    class SensitivityAnalysis
    {
        private Solver solver;
        private Objective objective;
        public List<Variable> variables;
        public List<Constraint> constraints;
        public Dictionary<string, double> variableLowerBounds;
        public Dictionary<string, double> variableUpperBounds;
        public Dictionary<string, double> constraintRHS;  // Add this line

        public SensitivityAnalysis()
        {
            solver = Solver.CreateSolver("GLOP");
            variables = new List<Variable>();
            constraints = new List<Constraint>();
            variableLowerBounds = new Dictionary<string, double>();
            variableUpperBounds = new Dictionary<string, double>();
            constraintRHS = new Dictionary<string, double>();  // Add this line
        }

        public void AddVariable(string name, double lowerBound, double upperBound)
        {
            Variable variable = solver.MakeNumVar(lowerBound, upperBound, name);
            variables.Add(variable);
            variableLowerBounds[name] = lowerBound;
            variableUpperBounds[name] = upperBound;
        }

        public void SetObjective(Dictionary<string, double> coefficients, bool isMaximization)
        {
            objective = solver.Objective();
            foreach (var variable in variables)
            {
                if (coefficients.ContainsKey(variable.Name()))
                {
                    objective.SetCoefficient(variable, coefficients[variable.Name()]);
                }
            }
            if (isMaximization)
            {
                objective.SetMaximization();
            }
            else
            {
                objective.SetMinimization();
            }
        }

        public void AddConstraint(Dictionary<string, double> coefficients, double lowerBound, double upperBound, string name)
        {
            Constraint constraint = solver.MakeConstraint(lowerBound, upperBound, name);
            foreach (var variable in variables)
            {
                if (coefficients.ContainsKey(variable.Name()))
                {
                    constraint.SetCoefficient(variable, coefficients[variable.Name()]);
                }
            }
            constraints.Add(constraint);
            constraintRHS[name] = upperBound;  // Store the RHS value
        }

        public void SolveModel()
        {
            Solver.ResultStatus resultStatus = solver.Solve();

            if (resultStatus == Solver.ResultStatus.OPTIMAL)
            {
                Console.WriteLine("Solution:");
                Console.WriteLine("Objective value = " + objective.Value());
                foreach (var variable in variables)
                {
                    Console.WriteLine($"{variable.Name()} = {variable.SolutionValue()}");
                }
            }
            else
            {
                Console.WriteLine("No optimal solution found.");
            }
        }

        // Sensitivity Analysis Functions
        public void DisplayRangeNonBasicVariable(string variableName)
        {
            Variable variable = variables.Find(v => v.Name() == variableName);
            if (variable == null)
            {
                Console.WriteLine($"Variable {variableName} not found.");
                return;
            }

            Console.WriteLine($"Range for Non-Basic Variable {variableName}:");
            Console.WriteLine($"  Reduced Cost: {variable.ReducedCost()}");
        }

        public void ApplyChangeNonBasicVariable(string variableName, double newValue)
        {
            Variable variable = variables.Find(v => v.Name() == variableName);
            if (variable == null)
            {
                Console.WriteLine($"Variable {variableName} not found.");
                return;
            }

            Console.WriteLine($"Changing {variableName} to {newValue}.");
            variable.SetBounds(newValue, newValue);
            SolveModel();
        }

        public void DisplayRangeBasicVariable(string variableName)
        {
            if (!variableLowerBounds.ContainsKey(variableName) || !variableUpperBounds.ContainsKey(variableName))
            {
                Console.WriteLine($"Variable {variableName} not found.");
                return;
            }

            double lowerBound = variableLowerBounds[variableName];
            double upperBound = variableUpperBounds[variableName];

            Variable variable = variables.Find(v => v.Name() == variableName);
            if (variable == null)
            {
                Console.WriteLine($"Variable {variableName} not found.");
                return;
            }

            Console.WriteLine($"Range for Basic Variable {variableName}:");
            Console.WriteLine($"  Solution Value: {variable.SolutionValue()}");
            Console.WriteLine($"  Lower Bound: {lowerBound}");
            Console.WriteLine($"  Upper Bound: {upperBound}");
        }

        public void ApplyChangeBasicVariable(string variableName, double newValue)
        {
            Variable variable = variables.Find(v => v.Name() == variableName);
            if (variable == null)
            {
                Console.WriteLine($"Variable {variableName} not found.");
                return;
            }

            Console.WriteLine($"Changing {variableName} to {newValue}.");
            variable.SetBounds(newValue, newValue);
            SolveModel();
        }

        public void DisplayRangeConstraintRHS(string constraintName)
        {
            Constraint constraint = constraints.Find(c => c.Name() == constraintName);
            if (constraint == null)
            {
                Console.WriteLine($"Constraint {constraintName} not found.");
                return;
            }

            double rhsValue = constraintRHS.ContainsKey(constraintName) ? constraintRHS[constraintName] : double.NaN;

            Console.WriteLine($"Range for Constraint RHS {constraintName}:");
            Console.WriteLine($"  Dual Value: {constraint.DualValue()}");
            Console.WriteLine($"  RHS: {rhsValue}");
        }

        public void ApplyChangeConstraintRHS(string constraintName, double newRHS)
        {
            Constraint constraint = constraints.Find(c => c.Name() == constraintName);
            if (constraint == null)
            {
                Console.WriteLine($"Constraint {constraintName} not found.");
                return;
            }

            Console.WriteLine($"Changing RHS of {constraintName} to {newRHS}.");
            constraint.SetBounds(newRHS, newRHS);
            constraintRHS[constraintName] = newRHS;  // Update the RHS value
            SolveModel();
        }

        public void DisplayShadowPrices()
        {
            Console.WriteLine("Shadow Prices (Dual Values):");
            foreach (var constraint in constraints)
            {
                Console.WriteLine($"{constraint.Name()}: {constraint.DualValue()}");
            }
        }

        // Duality-related Functions
        public void ApplyDuality()
        {
            Console.WriteLine("Applying duality to the model.");
            // Implement the conversion to the dual problem here
        }

        public void SolveDualModel()
        {
            Console.WriteLine("Solving the dual programming model.");
            // Implement the solution of the dual problem here
        }

        public void VerifyStrongOrWeakDuality()
        {
            Console.WriteLine("Verifying if the programming model has strong or weak duality.");
            // Implement the verification of strong or weak duality
        }

        // Add a new activity (variable) to the optimal solution
        public void AddNewActivity(string name, double lowerBound, double upperBound, Dictionary<string, double> coefficients)
        {
            Console.WriteLine($"Adding new activity {name}.");
            AddVariable(name, lowerBound, upperBound);
            SetObjective(coefficients, objective.Maximization());
            SolveModel();
        }

        // Add a new constraint to the optimal solution
        public void AddNewConstraint(Dictionary<string, double> coefficients, double lowerBound, double upperBound, string name)
        {
            Console.WriteLine($"Adding new constraint {name}.");
            AddConstraint(coefficients, lowerBound, upperBound, name);
            SolveModel();
        }
    }
}
