using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    [System.Serializable]
    public class Number : EquationElement
    {
        [SerializeField]
        int value;
        public Number(int value) : base(Type.NUMBER)
        {
            this.value = value;
            _priority = 1;
        }
        public override IEnumerable<EquationElement> GetDependencies()
        {
            return null;
        }

        public override bool YieldResult()
        {
            stackOverflowLock.IncreaseYieldCount();
            result = value;
            return true;
        }
        public override string ToString()
        {
            return value.ToString();
        }
    }
}
