using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR381ProjectFinal
{
    public class Model
    {
        public string problem;
        public List<double> objfuncCoef;
        public List<List<double>> constraints;
        public List<string> constraintsRelations;
        public List<double> rightHandSide;
        public List<string> signRestrictions;

        public string Problem { get => problem; set => problem = value; }
        public List<double> ObjfuncCoef { get => objfuncCoef; set => objfuncCoef = value; }
        public List<List<double>> Constraints { get => constraints; set => constraints = value; }
        public List<string> ConstraintsRelations { get => constraintsRelations; set => constraintsRelations = value; }
        public List<double> RightHandSide { get => rightHandSide; set => rightHandSide = value; }
        public List<string> SignRestrictions { get => signRestrictions; set => signRestrictions = value; }

        public Model()
        {
            Problem = " ";
            ObjfuncCoef = new List<double>();
            Constraints = new List<List<double>>();
            ConstraintsRelations = new List<string>();
            RightHandSide = new List<double>();
            SignRestrictions = new List<string>();
        }

        public Model(string problemType, List<double> objFuncCoef, List<List<double>> constraints, List<string> constraintsRelations, List<string> signRestrictions, List<double> rightHandSide)
        {
            Problem = problemType;
            ObjfuncCoef = objFuncCoef;
            Constraints = constraints;
            ConstraintsRelations = constraintsRelations;
            RightHandSide = rightHandSide;
            SignRestrictions = signRestrictions;
        }
    }
}

