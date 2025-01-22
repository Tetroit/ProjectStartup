using Equation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    public class Plus : Operator
    {
        public Plus() : base()
        {
            _priority = 4;
        }
        public override int Calculate(int a, int b)
        {
            return a + b;
        }
        public override string ToString()
        {
            return "+";
        }
    }
}
