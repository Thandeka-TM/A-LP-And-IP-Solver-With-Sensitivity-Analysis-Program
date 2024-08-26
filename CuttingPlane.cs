using System;
using System.Collections.Generic;
using System.IO;
using Google.OrTools.LinearSolver;

namespace LPR381ProjectFinal
{
    public class CuttingPlane
    {
        public class Model
        {
            public string ProblemType { get; set; }
            public List<double> ObjFuncCoeff { get; set; } = new List<double>();
            public List<List<double>> Constraints { get; set; } = new List<List<double>>();
            public List<string> ConstraintRelations { get; set; } = new List<string>();
            public List<double> RightHandSide { get; set; } = new List<double>();
            public List<string> SignRestrictions { get; set; } = new List<string>();
        }

        public void Solve(Model model, string outputFilePath)
        {
            Solver solver = Solver.CreateSolver("GLOP");

            if (solver == null)
            {
                Console.WriteLine("Could not create solver.");
                return;
            }

            // Create variables
            Variable[] variables = new Variable[model.ObjFuncCoeff.Count];
            for (int i = 0; i < model.ObjFuncCoeff.Count; i++)
            {
                variables[i] = solver.MakeNumVar(0.0, double.PositiveInfinity, $"x{i + 1}");
            }

            // Define the objective function
            Objective objective = solver.Objective();
            for (int i = 0; i < model.ObjFuncCoeff.Count; i++)
            {
                objective.SetCoefficient(variables[i], model.ObjFuncCoeff[i]);
            }
            objective.SetMaximization();

            // Add constraints
            for (int i = 0; i < model.Constraints.Count; i++)
            {
                Constraint constraint = solver.MakeConstraint(0.0, model.RightHandSide[i]);

                for (int j = 0; j < model.Constraints[i].Count; j++)
                {
                    constraint.SetCoefficient(variables[j], model.Constraints[i][j]);
                }
            }

            // Solve the problem
            Solver.ResultStatus resultStatus = solver.Solve();

            // Check the result status
            if (resultStatus != Solver.ResultStatus.OPTIMAL)
            {
                Console.WriteLine("The problem does not have an optimal solution!");
                return;
            }

            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                writer.WriteLine("Initial Solution:");
                for (int i = 0; i < variables.Length; i++)
                {
                    writer.WriteLine($"x{i + 1} = {variables[i].SolutionValue()}");
                }

                // Implement Gomory's Cutting Plane Method
                bool fractionalSolution = true;

                while (fractionalSolution)
                {
                    fractionalSolution = false;

                    // Check for fractional variables
                    foreach (var variable in variables)
                    {
                        double value = variable.SolutionValue();
                        if (value != Math.Floor(value))
                        {
                            fractionalSolution = true;
                            break;
                        }
                    }

                    if (fractionalSolution)
                    {
                        // Generate a Gomory cut
                        writer.WriteLine("Adding a Gomory cut...");
                        Constraint gomoryCut = solver.MakeConstraint(double.NegativeInfinity, 0);

                        for (int i = 0; i < variables.Length; i++)
                        {
                            double value = variables[i].SolutionValue();
                            double fractionalPart = value - Math.Floor(value);
                            gomoryCut.SetCoefficient(variables[i], fractionalPart);
                        }

                        // Re-solve the problem
                        resultStatus = solver.Solve();

                        if (resultStatus != Solver.ResultStatus.OPTIMAL)
                        {
                            writer.WriteLine("The problem does not have an optimal solution!");
                            return;
                        }

                        writer.WriteLine("Solution after adding Gomory cut:");
                        for (int i = 0; i < variables.Length; i++)
                        {
                            writer.WriteLine($"x{i + 1} = {variables[i].SolutionValue()}");
                        }
                    }
                }

                writer.WriteLine("Final integer solution:");
                for (int i = 0; i < variables.Length; i++)
                {
                    writer.WriteLine($"x{i + 1} = {variables[i].SolutionValue()}");
                }
            }

            Console.WriteLine($"Results have been written to {outputFilePath}");
        }

        public static Model LoadModelFromFile(string filePath)
        {
            Model model = new Model();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string[] objectiveLine = reader.ReadLine().Split();
                model.ProblemType = objectiveLine[0];

                for (int i = 1; i < objectiveLine.Length; i++)
                {
                    model.ObjFuncCoeff.Add(double.Parse(objectiveLine[i]));
                }

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("<=") || line.Contains(">=") || line.Contains("="))
                    {
                        string[] parts = line.Split(' ');
                        List<double> constraint = new List<double>();

                        int j = 0;
                        for (; j < parts.Length - 2; j++)
                        {
                            constraint.Add(double.Parse(parts[j]));
                        }

                        model.Constraints.Add(constraint);
                        model.ConstraintRelations.Add(parts[j]);
                        model.RightHandSide.Add(double.Parse(parts[j + 1]));
                    }
                    else
                    {
                        model.SignRestrictions.AddRange(line.Split());
                    }
                }
            }

            return model;
        }

        public static void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("Welcome to the Gomory Cutting Plane Algorithm Solver");
                Console.WriteLine("1. Upload LP Model");
                Console.WriteLine("2. Exit");
                Console.Write("Choose an option: ");
                int choice = int.Parse(Console.ReadLine());

                if (choice == 1)
                {
                    Console.Write("Enter the file path of the LP model: ");
                    string inputFilePath = Console.ReadLine();

                    Console.Write("Enter the file path to save the output: ");
                    string outputFilePath = Console.ReadLine();

                    Model model = LoadModelFromFile(inputFilePath);

                    CuttingPlane cp = new CuttingPlane();
                    cp.Solve(model, outputFilePath);
                }
                else if (choice == 2)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice, please try again.");
                }
            }
        }

        public static void Main(string[] args)
        {
            ShowMenu();
        }
    }
}
