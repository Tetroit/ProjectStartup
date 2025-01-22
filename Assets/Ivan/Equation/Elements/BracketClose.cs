using Equation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    [CreateAssetMenu(fileName = "Close Bracket", menuName = "Equation Element/Close Bracket")]
    public class BracketClose : EquationElement
    {
        public BracketClose() : base(Type.BRACKET_CLOSE)
        {
            _priority = 1;
        }
        public BracketOpen pair;

        public override IEnumerable<EquationElement> GetDependencies()
        {
            return pair.GetDependencies();
        }

        public override bool YieldResult()
        {
            stackOverflowLock.IncreaseYieldCount();
            return pair.YieldResult();
        }

        public override string ToString()
        {
            return ")";
        }
    }
}

