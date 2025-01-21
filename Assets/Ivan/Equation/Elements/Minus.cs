using Equation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Minus", menuName = "Equation Element/Minus")]
    public class Minus : Operator
    {
        public Minus() : base()
        {
            _priority = 4;
        }
        public override int Calculate(int a, int b)
        {
            return a - b;
        }
        public override string ToString()
        {
            return "-";
        }
        protected override void Init()
        {
            base.Init();
            _priority = 4;
        }
    }
}

