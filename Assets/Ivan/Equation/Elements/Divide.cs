using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    [CreateAssetMenu(fileName = "Divide", menuName = "Equation Element/Divide")]
    public class Divide : Operator
    {
        public Divide() : base() { }
        public override int Calculate(int a, int b)
        {
            if (b == 0)
                Debug.LogError("division by 0 is prohibited (yet)");
            return a / b;
        }
        public override string ToString()
        {
            return "/";
        }
    }
}
