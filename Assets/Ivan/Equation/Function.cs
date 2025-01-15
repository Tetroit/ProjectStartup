using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    [System.Serializable]
    public abstract class Function : EquationElement
    {
        public Function() : base(Type.FUNCTION)
        {
            _priority = 2; 
        }
        public EquationElement dependency;
        abstract public int Calculate(int a);
        public void SetDependency(EquationElement element)
        {
            dependency = element;
        }
        public override IEnumerable<EquationElement> GetDependencies()
        {
            yield return dependency;
        }
        
        public override bool YieldResult()
        {
            if (dependency == null) return false;
            if (!dependency.calculated)
                if (!dependency.YieldResult()) return false;
            result = Calculate(dependency.result);
            calculated = true;
            return true;
        }
    }
    public class Sqrt : Function
    {
        public override int Calculate(int a)
        {
            return (int)Mathf.Sqrt(a);
        }
    }
}
