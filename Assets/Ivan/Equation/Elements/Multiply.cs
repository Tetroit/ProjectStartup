using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    [CreateAssetMenu(fileName = "Multiply", menuName = "Equation Element/Multiply")]
    public class Multiply : Operator
    {
        public Multiply() : base() { }
        public override int Calculate(int a, int b)
        {
            return a * b;
        }
        public override string ToString()
        {
            return "*";
        }
    }
}
