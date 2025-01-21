using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New number", menuName = "Equation Element/Number")]
    public class Number : EquationElement
    {
        [SerializeField]
        int value;
        public Number(int value) : base(Type.NUMBER)
        {
            result = value;
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
        protected override void Init()
        {
            _type = Type.NUMBER;
            result = value;
            _priority = 1;
        }
    }
}
