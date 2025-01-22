using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    public class Sqrt : Function
    {
        public override int Calculate(int a)
        {
            return (int)Mathf.Sqrt(a);
        }
        public override string ToString()
        {
            return "sqrt";
        }
    }
}