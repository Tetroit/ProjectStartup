using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    [System.Serializable]
    public class Number : EquationElement
    {
        public Number(int value) : base(Type.NUMBER)
        {
            result = value;
            calculated = true;
            _priority = 1;
        }

        public override IEnumerable<EquationElement> GetDependencies()
        {
            return null;
        }

        public override bool YieldResult()
        {
            return true;
        }
        public override string ToString()
        {
            return result.ToString();
        }
    }
}
