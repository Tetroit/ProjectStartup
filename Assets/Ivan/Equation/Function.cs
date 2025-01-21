using System;
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
        [HideInInspector]
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
            stackOverflowLock.IncreaseYieldCount();
            if (dependency == null) return false;
            if (!dependency.calculated)
                if (!dependency.YieldResult()) return false;
            result = Calculate(dependency.result);
            calculated = true;
            return true;
        }
        protected override void Init()
        {
            _type = Type.FUNCTION;
            _priority = 2;
        }
    }

    [CreateAssetMenu(fileName = "Square", menuName = "Equation Element/Square")]
    public class Sqr : Function
    {
        public override int Calculate(int a)
        {
            return a * a;
        }
        public override string ToString()
        {
            return "sqr";
        }
    }

    [CreateAssetMenu(fileName = "Negate", menuName = "Equation Element/Negate")]
    public class Negate : Function
    {
        public override int Calculate(int a)
        {
            return -a;
        }
        public override string ToString()
        {
            return "-";
        }
    }
}
