using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Equation
{

    [System.Serializable]
    public abstract class Operator : EquationElement
    {
        public EquationElement[] dependencies = new EquationElement[2] { null, null };
        public Operator() : base(Type.OPERATOR) 
        {
            _priority = 3;
        }
        abstract public int Calculate(int a, int b);
        public int GetPriority() { return priority; }
        public void SetDependency(int ID, EquationElement element)
        {
            if (ID > 1 || ID < 0) return;
            dependencies[ID] = element;
        }
        public override IEnumerable<EquationElement> GetDependencies()
        {
            return dependencies;
        }
        public override bool YieldResult()
        {
            if (dependencies[0] == null)
            {
                Debug.LogError("left dependency was null");
            }
            if (dependencies[1] == null)
            {
                Debug.LogError("left dependency was null");
            }
            if (!dependencies[0].calculated) 
                if (!dependencies[0].YieldResult()) return false;
            if (!dependencies[1].calculated) 
                if (!dependencies[1].YieldResult()) return false;
            result = Calculate(dependencies[0].result, dependencies[1].result);
            calculated = true;
            return true;
        }
    }

    [System.Serializable]
    public class Plus : Operator
    {
        public Plus() : base()
        {
            _priority = 4;
        }
        public override int Calculate(int a, int b)
        {
            return a + b;
        }
        public override string ToString()
        {
            return "+";
        }
    }

    [System.Serializable]
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
    }

    [System.Serializable]
    public class Multiply : Operator
    {
        public Multiply() : base() { }
        public override int Calculate(int a, int b)
        {
            return a * b;
        }
        public override string ToString()
        {
            return "*";
        }
    }

    [System.Serializable]
    public class Divide : Operator
    {
        public Divide() : base() { }
        public override int Calculate(int a, int b)
        {
            if (b == 0)
                Debug.LogError("division by 0 is prohibited (yet)");
            return a / b;
        }
        public override string ToString()
        {
            return "/";
        }
    }
}
