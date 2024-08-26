using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR381ProjectFinal
{
    public class Knapsack
    {
        public class KnapsackItem
        {
            public int Weight { get; set; }
            public int Value { get; set; }
            public double ValuePerWeight => Math.Round((double)Value / Weight, 3);
        }
        class KnapsackSolution
        {
            public int TotalValue { get; set; }
            public int TotalWeight { get; set; }
            public List<int> SelectedItems { get; set; }

            public KnapsackSolution()
            {
                SelectedItems = new List<int>();
            }
        }

        class BranchAndBoundKnapsack
        {
            private int Capacity;
            private List<KnapsackItem> Items;
            private KnapsackSolution BestSolution;
            private StreamWriter Writer;
            private string ObjectiveType;
            private int iterationCounter = 0;

            public BranchAndBoundKnapsack(int capacity, List<KnapsackItem> items, StreamWriter writer, string objectiveType)
            {
                Capacity = capacity;
                Items = items.OrderByDescending(i => i.ValuePerWeight).ToList();
                BestSolution = new KnapsackSolution();
                Writer = writer;
                ObjectiveType = objectiveType.ToLower();
            }

            public KnapsackSolution Solve()
            {
                BranchAndBound(0, 0, 0, new List<int>(new int[Items.Count]));
                return BestSolution;
            }

            private void BranchAndBound(int index, int currentWeight, int currentValue, List<int> selectedItems)
            {
                iterationCounter++;
                Writer.WriteLine($"Iteration: {iterationCounter}, Index: {index}, Current Weight: {currentWeight}, Current Value: {currentValue}, Selected Items: {string.Join(", ", selectedItems)}");

                if (index == Items.Count)
                {
                    if ((ObjectiveType == "max" && currentWeight <= Capacity && currentValue > BestSolution.TotalValue) ||
                        (ObjectiveType == "min" && currentWeight <= Capacity && (BestSolution.TotalValue == 0 || currentValue < BestSolution.TotalValue)))
                    {
                        BestSolution.TotalValue = currentValue;
                        BestSolution.TotalWeight = currentWeight;
                        BestSolution.SelectedItems = new List<int>(selectedItems);
                    }
                    return;
                }

                double bound = CalculateBound(index, currentWeight, currentValue);
                Writer.WriteLine($"Bound: {Math.Round(bound, 3)}, Current Best Value: {BestSolution.TotalValue}");

                if ((ObjectiveType == "max" && bound <= BestSolution.TotalValue) ||
                    (ObjectiveType == "min" && bound >= BestSolution.TotalValue && BestSolution.TotalValue != 0))
                {
                    return;
                }

                if (currentWeight + Items[index].Weight <= Capacity)
                {
                    selectedItems[index] = 1;
                    BranchAndBound(index + 1, currentWeight + Items[index].Weight, currentValue + Items[index].Value, selectedItems);
                }

                selectedItems[index] = 0;
                BranchAndBound(index + 1, currentWeight, currentValue, selectedItems);
            }

            private double CalculateBound(int index, int currentWeight, int currentValue)
            {
                double bound = currentValue;
                int totalWeight = currentWeight;

                for (int i = index; i < Items.Count; i++)
                {
                    if (totalWeight + Items[i].Weight <= Capacity)
                    {
                        totalWeight += Items[i].Weight;
                        bound += Items[i].Value;
                    }
                    else
                    {
                        int remainingWeight = Capacity - totalWeight;
                        bound += Items[i].ValuePerWeight * remainingWeight;
                        break;
                    }
                }
                return bound;
            }
        }

        private List<double> objectiveFunction;
        private List<List<double>> constraints;
        private bool isMax;

        public void RunKnapsackMenu(List<double> objectiveFunction, List<List<double>> constraints, bool isMax)
        {

            this.objectiveFunction = objectiveFunction;
            this.constraints = constraints;
            this.isMax = isMax;

            bool continueLoop = true;

            while (continueLoop)
            {
                Console.WriteLine("Please choose an operation:");
                Console.WriteLine("1. Execute Knapsack Problem");
                Console.WriteLine("2. Exit");
                Console.Write("Your selection: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ExecuteKnapsackProblem();
                        break;
                    case "2":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please select again.");
                        break;
                }
            }
        }

        public void ExecuteKnapsackProblem()
        {
            Console.WriteLine("Specify the input file path:");
            string inputFilePath = Console.ReadLine() ?? throw new InvalidOperationException("Input file path cannot be null or empty.");

            Console.WriteLine("Specify the output file path:");
            string outputFilePath = Console.ReadLine() ?? throw new InvalidOperationException("Output file path cannot be null or empty.");

            try
            {
                if (!File.Exists(inputFilePath))
                {
                    Console.WriteLine("The input file does not exist.");
                    return;
                }

                var lines = File.ReadAllLines(inputFilePath);

                // Parse the first line for the objective function and optimization type (max/min)
                var firstLineParts = lines[0].Split(' ');
                isMax = firstLineParts[0].Trim().ToLower() == "max";
                objectiveFunction = firstLineParts.Skip(1).Select(double.Parse).ToList();

                // Parse the second line for the constraints and capacity
                var constraintParts = lines[1].Split(' ');
                var constraintValues = constraintParts.Take(constraintParts.Length - 1).Select(double.Parse).ToList();
                var capacity = int.Parse(constraintParts.Last().Split('=')[1]);

                constraints = new List<List<double>> { constraintValues };

                // Parse the third line for variable types, e.g., "bin bin bin"
                // This example assumes binary variables, and we don't need to store them for now
                var variableTypes = lines[2].Split(' ').ToList();

                if (objectiveFunction == null || constraints == null || constraints.Count == 0)
                {
                    Console.WriteLine("Input data is missing or incorrect.");
                    return;
                }

                List<KnapsackItem> items = new List<KnapsackItem>();

                for (int i = 0; i < objectiveFunction.Count; i++)
                {
                    items.Add(new KnapsackItem
                    {
                        Value = (int)objectiveFunction[i],
                        Weight = (int)constraints[0][i]
                    });
                }

                using (StreamWriter writer = new StreamWriter(outputFilePath))
                {
                    BranchAndBoundKnapsack solver = new BranchAndBoundKnapsack(capacity, items, writer, isMax ? "max" : "min");
                    KnapsackSolution solution = solver.Solve();

                    writer.WriteLine();
                    writer.WriteLine("Best Solution:");
                    writer.WriteLine($"Total Value: {solution.TotalValue}");
                    writer.WriteLine($"Total Weight: {solution.TotalWeight}");
                    writer.WriteLine($"Selected Items: {string.Join(", ", solution.SelectedItems.Select((s, i) => s == 1 ? $"Item {i + 1}" : string.Empty).Where(x => !string.IsNullOrEmpty(x)))}");

                    Console.WriteLine("Solution has been written to the output file.");
                    Console.WriteLine($"Total Value: {solution.TotalValue}");
                    Console.WriteLine($"Total Weight: {solution.TotalWeight}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}
