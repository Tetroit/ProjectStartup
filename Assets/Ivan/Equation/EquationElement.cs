using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{

    public class StackOverflowLock
    {
        Formula formula;
        int yieldCount = 0;
        public StackOverflowLock(Formula formula)
        {
            this.formula = formula;
        }
        public void Reset()
        {
            yieldCount = 0;
        }
        public void IncreaseYieldCount()
        {
            yieldCount++;
            if (formula.size * 4 < yieldCount)
                throw new System.Exception("Operation caused stack overflow");
        }
    }

    public static class EquationElementFactory
    {
        public static EquationElement Get(string str, params object[] args)
        {
            Type type = Type.GetType("Equation." + str);
/*            Debug.Log(str);
            Debug.Log(type);*/
            if (type != null && typeof(EquationElement).IsAssignableFrom(type))
                return Activator.CreateInstance(type, args) as EquationElement;
            else
                throw new Exception("Object is not of EquationElement type");
        }
    }
    [System.Serializable]
    public abstract class EquationElement
    {
        public enum Type
        {
            NONE,
            NUMBER,
            FUNCTION,
            OPERATOR,
            BRACKET_OPEN,
            BRACKET_CLOSE
        }
        protected Type _type;
        public Type type => _type;

        protected int _priority;
        public virtual int priority => _priority;

        [HideInInspector]
        public int result;
        [HideInInspector]
        public int ID;

        [HideInInspector]
        public bool calculated = false;
        [HideInInspector]
        protected StackOverflowLock stackOverflowLock;
        public abstract IEnumerable<EquationElement> GetDependencies();
        public EquationElement(Type type) { _type = type; }
        public abstract bool YieldResult();
        public void OnFormulaAdded(Formula formula)
        {
            stackOverflowLock = formula.stackOverflowLock;
        }
    }
}
