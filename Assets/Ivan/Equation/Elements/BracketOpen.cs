using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Open Bracket", menuName = "Equation Element/Open Bracket")]
    public class BracketOpen : EquationElement
    {
        public BracketOpen() : base(Type.BRACKET_OPEN)
        {
            _priority = 1;
        }
        public BracketClose pair;
        public EquationElement resultRef;

        public override IEnumerable<EquationElement> GetDependencies()
        {
            yield return resultRef;
        }

        public void SetDependency(EquationElement element)
        {
            resultRef = element;
        }

        public override bool YieldResult()
        {
            stackOverflowLock.IncreaseYieldCount();
            if (resultRef == null) return false;
            if (!resultRef.calculated) 
                if (!resultRef.YieldResult()) return false;

            result = resultRef.result;
            pair.result = result;

            calculated = true;
            pair.calculated = true;

            return true;
        }
        public override string ToString()
        {
            return "(";
        }
    }
    
}
