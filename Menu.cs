using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LPR381ProjectFinal
{
    public class Menu
    {
        static Model model; //storing the loaded model
        public void menu()
        {
            // Absolute path to the text file
            string filePath = @"lprexamplefile.txt";

            FileParsing fp = new FileParsing();
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Optimization Algorithms Menu");
                Console.WriteLine("============================");
                Console.WriteLine("1. Primal Simplex Algorithm");
                Console.WriteLine("2. Primal Simplex Algorithm");
                Console.WriteLine("3. Branch and Bound Algorithm");
                Console.WriteLine("4. Cutting Plane Algorithm");
                Console.WriteLine("5. Kanpsack Algorithm");
                Console.WriteLine("6. Sensitivity Analysis");
                Console.WriteLine("7. Exit");
                Console.WriteLine("============================");
                Console.Write("Select an option (1-7): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("You selected Primal Simplex Algorithm.");
                        Console.WriteLine("Enter filepath: ");
                        filePath = Console.ReadLine();
                        fp.Parse(filePath).ToString();
                        Start();
                        break;

                    case "2":
                        Console.WriteLine("You selected Revised Simplex Algorithm.");
                        Console.WriteLine("Enter filepath: ");
                        filePath = Console.ReadLine();
                        fp.Parse(filePath).ToString();
                        RunRevisedPrimalSimplex();
                        break;

                    case "3":
                        Console.WriteLine("You selected Branch and Bound Algorithm.");
                        Console.WriteLine("Enter filepath: ");
                        filePath = Console.ReadLine();
                        fp.Parse(filePath).ToString();
                        BranchandBound branchandBound = new BranchandBound();
                        branchandBound.Solve(model);
                        break;

                    case "4":
                        Console.WriteLine("You selected Cutting Plane Algorithm.");
                        Console.WriteLine("Enter filepath: ");
                        filePath = Console.ReadLine();
                        fp.Parse(filePath).ToString();
                        CuttingPlane cuttingPlane = new CuttingPlane();
                        cuttingPlane.Solve(model, outputFilePath: @"cuttingplaneoutput.txt");
                        //cuttingPlane.solve(); NEED HELP HERE
                        break;

                    case "5":
                        Console.WriteLine("You selected Knapsack Algorithm.");
                        //Console.WriteLine("Enter filepath: ");
                        //filePath = Console.ReadLine();
                        //fp.Parse(filePath).ToString();
                        Knapsack knapsack = new Knapsack();
                        knapsack.ExecuteKnapsackProblem();
                        break;

                    case "6":
                        Console.WriteLine("You selected Sensitivity Analysis.");
                        A();
                        // Implement Sensitivity Analysis here
                        break;

                    case "7":
                        Console.WriteLine("Exiting the program.");
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid selection. Please choose a valid option.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("Press any key to return to the menu...");
                    Console.ReadKey();
                }
            }
        }

        public static void Start()
        {
            Console.WriteLine("Primal Simplex Algorithm");
            Console.WriteLine("------------------------");

            // Get the number of rows and columns
            Console.Write("Enter the number of constraints (rows): ");
            int numRows = int.Parse(Console.ReadLine());

            Console.Write("Enter the number of decision variables + 1 (columns): ");
            int numCols = int.Parse(Console.ReadLine());

            // Initialize the tableau
            double[,] tableau = new double[numRows, numCols];

            // Get the tableau values from the user
            Console.WriteLine("Enter the tableau values row by row (space-separated):");
            for (int i = 0; i < numRows; i++)
            {
                Console.WriteLine($"Enter values for row {i + 1}:");
                string[] input = Console.ReadLine().Split(' ');
                for (int j = 0; j < numCols; j++)
                {
                    tableau[i, j] = double.Parse(input[j]);
                }
            }

            // Create an instance of the PrimalSimplex class
            PrimalSimplex simplex = new PrimalSimplex(tableau, numRows, numCols);

            // Solve the simplex algorithm
            simplex.Solve();

            // Wait for user input before closing
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void RunRevisedPrimalSimplex()
        {
            Console.WriteLine("Enter number of rows (constraints): ");
            int numRows = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter number of columns (variables): ");
            int numCols = int.Parse(Console.ReadLine());

            double[,] a = new double[numRows, numCols];
            double[] b = new double[numRows];
            double[] c = new double[numCols];

            Console.WriteLine("Enter the coefficients for matrix A:");
            for (int i = 0; i < numRows; i++)
            {
                Console.WriteLine($"Row {i + 1}:");
                for (int j = 0; j < numCols; j++)
                {
                    Console.Write($"a[{i + 1},{j + 1}]: ");
                    a[i, j] = double.Parse(Console.ReadLine());
                }
            }

            Console.WriteLine("Enter the coefficients for vector b:");
            for (int i = 0; i < numRows; i++)
            {
                Console.Write($"b[{i + 1}]: ");
                b[i] = double.Parse(Console.ReadLine());
            }

            Console.WriteLine("Enter the coefficients for vector c:");
            for (int j = 0; j < numCols; j++)
            {
                Console.Write($"c[{j + 1}]: ");
                c[j] = double.Parse(Console.ReadLine());
            }

            RevisedPrimalSimplex simplex = new RevisedPrimalSimplex();
            simplex.RevisedSimplex(a, b, c);
            simplex.Solve();
        }

        public static void A()
        {
            SensitivityAnalysis analysis = new SensitivityAnalysis();

            while (true)
            {
                Console.WriteLine("----- Linear Programming Sensitivity Analysis -----");
                Console.WriteLine("1. Add Variable");
                Console.WriteLine("2. Set Objective Function");
                Console.WriteLine("3. Add Constraint");
                Console.WriteLine("4. Solve Model");
                Console.WriteLine("5. Display Range of Non-Basic Variable");
                Console.WriteLine("6. Apply Change to Non-Basic Variable");
                Console.WriteLine("7. Display Range of Basic Variable");
                Console.WriteLine("8. Apply Change to Basic Variable");
                Console.WriteLine("9. Display Range of Constraint RHS");
                Console.WriteLine("10. Apply Change to Constraint RHS");
                Console.WriteLine("11. Display Shadow Prices");
                Console.WriteLine("12. Apply Duality");
                Console.WriteLine("13. Solve Dual Programming Model");
                Console.WriteLine("14. Verify Strong or Weak Duality");
                Console.WriteLine("15. Add New Activity");
                Console.WriteLine("16. Add New Constraint");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");
                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter variable name: ");
                        string varName = Console.ReadLine();
                        Console.Write("Enter lower bound: ");
                        double lowerBound = double.Parse(Console.ReadLine());
                        Console.Write("Enter upper bound: ");
                        double upperBound = double.Parse(Console.ReadLine());
                        analysis.AddVariable(varName, lowerBound, upperBound);
                        break;

                    case 2:
                        var coefficients = new Dictionary<string, double>();
                        Console.WriteLine("Enter the coefficients for the objective function:");
                        foreach (var variable in analysis.variables)
                        {
                            Console.Write($"{variable.Name()}: ");
                            double coefficient = double.Parse(Console.ReadLine());
                            coefficients[variable.Name()] = coefficient;
                        }
                        Console.Write("Is this a maximization problem? (yes/no): ");
                        bool isMaximization = Console.ReadLine().ToLower() == "yes";
                        analysis.SetObjective(coefficients, isMaximization);
                        break;

                    case 3:
                        var constraintCoefficients = new Dictionary<string, double>();
                        Console.Write("Enter constraint name: ");
                        string constraintName = Console.ReadLine();
                        Console.WriteLine("Enter the coefficients for the constraint:");
                        foreach (var variable in analysis.variables)
                        {
                            Console.Write($"{variable.Name()}: ");
                            double coefficient = double.Parse(Console.ReadLine());
                            constraintCoefficients[variable.Name()] = coefficient;
                        }
                        Console.Write("Enter lower bound: ");
                        double constraintLowerBound = double.Parse(Console.ReadLine());
                        Console.Write("Enter upper bound: ");
                        double constraintUpperBound = double.Parse(Console.ReadLine());
                        analysis.AddConstraint(constraintCoefficients, constraintLowerBound, constraintUpperBound, constraintName);
                        break;

                    case 4:
                        analysis.SolveModel();
                        break;

                    case 5:
                        Console.Write("Enter Non-Basic Variable name: ");
                        string nonBasicVarName = Console.ReadLine();
                        analysis.DisplayRangeNonBasicVariable(nonBasicVarName);
                        break;

                    case 6:
                        Console.Write("Enter Non-Basic Variable name: ");
                        string nbVarName = Console.ReadLine();
                        Console.Write("Enter new value: ");
                        double newValue = double.Parse(Console.ReadLine());
                        analysis.ApplyChangeNonBasicVariable(nbVarName, newValue);
                        break;

                    case 7:
                        Console.Write("Enter Basic Variable name: ");
                        string basicVarName = Console.ReadLine();
                        analysis.DisplayRangeBasicVariable(basicVarName);
                        break;

                    case 8:
                        Console.Write("Enter Basic Variable name: ");
                        string bVarName = Console.ReadLine();
                        Console.Write("Enter new value: ");
                        double basicNewValue = double.Parse(Console.ReadLine());
                        analysis.ApplyChangeBasicVariable(bVarName, basicNewValue);
                        break;

                    case 9:
                        Console.Write("Enter Constraint name: ");
                        string constraintRHSName = Console.ReadLine();
                        analysis.DisplayRangeConstraintRHS(constraintRHSName);
                        break;

                    case 10:
                        Console.Write("Enter Constraint name: ");
                        string constName = Console.ReadLine();
                        Console.Write("Enter new RHS value: ");
                        double newRHS = double.Parse(Console.ReadLine());
                        analysis.ApplyChangeConstraintRHS(constName, newRHS);
                        break;

                    case 11:
                        analysis.DisplayShadowPrices();
                        break;

                    case 12:
                        analysis.ApplyDuality();
                        break;

                    case 13:
                        analysis.SolveDualModel();
                        break;

                    case 14:
                        analysis.VerifyStrongOrWeakDuality();
                        break;

                    case 15:
                        Console.Write("Enter new activity name: ");
                        string activityName = Console.ReadLine();
                        Console.Write("Enter lower bound: ");
                        double activityLowerBound = double.Parse(Console.ReadLine());
                        Console.Write("Enter upper bound: ");
                        double activityUpperBound = double.Parse(Console.ReadLine());
                        var activityCoefficients = new Dictionary<string, double>();
                        Console.WriteLine("Enter the coefficients for the new activity:");
                        foreach (var variable in analysis.variables)
                        {
                            Console.Write($"{variable.Name()}: ");
                            double activityCoefficient = double.Parse(Console.ReadLine());
                            activityCoefficients[variable.Name()] = activityCoefficient;
                        }
                        analysis.AddNewActivity(activityName, activityLowerBound, activityUpperBound, activityCoefficients);
                        break;

                    case 16:
                        var newConstraintCoefficients = new Dictionary<string, double>();
                        Console.Write("Enter new constraint name: ");
                        string newConstraintName = Console.ReadLine();
                        Console.WriteLine("Enter the coefficients for the new constraint:");
                        foreach (var variable in analysis.variables)
                        {
                            Console.Write($"{variable.Name()}: ");
                            double constraintCoefficient = double.Parse(Console.ReadLine());
                            newConstraintCoefficients[variable.Name()] = constraintCoefficient;
                        }
                        Console.Write("Enter lower bound: ");
                        double newConstraintLowerBound = double.Parse(Console.ReadLine());
                        Console.Write("Enter upper bound: ");
                        double newConstraintUpperBound = double.Parse(Console.ReadLine());
                        analysis.AddNewConstraint(newConstraintCoefficients, newConstraintLowerBound, newConstraintUpperBound, newConstraintName);
                        break;

                    case 0:
                        return;

                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }
            }
        }
    }
}
