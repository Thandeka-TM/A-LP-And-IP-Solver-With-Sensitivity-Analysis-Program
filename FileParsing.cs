using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LPR381ProjectFinal
{
    public class FileParsing
    {
        //public static void Model()
        //{
        public Model Parse(string filePath)
        {
            var model = new Model();
            var lines = File.ReadAllLines(filePath);

            // Parse the first line (objective function)
            var firstLine = lines[0].Split(' ');
            model.problem = firstLine[0];

            for (int i = 1; i < firstLine.Length - 1; i += 2)
            {
                double coefficient = double.Parse(firstLine[i + 1]);
                // Disable BCC4002
                if (firstLine[i] == "-")
                {
                    coefficient = -coefficient;
                }
                model.objfuncCoef.Add(coefficient);
            }

            // Parse the constraints
            for (int i = 1; i < lines.Length - 1; i++)
            {
                var line = lines[i].Split(' ');
                var coefficients = new List<double>();

                for (int j = 0; j < model.objfuncCoef.Count; j++)
                {
                    double coefficient = double.Parse(line[j * 2 + 1]);
                    if (line[j * 2] == "-")
                    {
                        coefficient = -coefficient;
                    }
                    coefficients.Add(coefficient);
                }

                model.Constraints.Add(coefficients);
                model.ConstraintsRelations.Add(line[model.objfuncCoef.Count * 2]);
                model.rightHandSide.Add(double.Parse(line[model.objfuncCoef.Count * 2 + 1]));
            }

            // Parse sign restrictions
            var signRestrictionsLine = lines[lines.Length - 1].Split(' ');
            foreach (var sign in signRestrictionsLine)
            {
                model.signRestrictions.Add(sign);
            }

            return model;
        }


        //}
    }
}
