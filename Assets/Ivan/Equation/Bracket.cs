using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    [System.Serializable]
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
            return pair.YieldResult();
        }

        public override string ToString()
        {
            return ")";
        }
    }
}
