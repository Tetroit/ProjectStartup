using System;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
    public enum OperationsNames
    {
        NUMBER,
        PLUS,
        MINUS,
        MULTIPLY,
        DIVIDE,
        SQUARE,
        SQUARE_ROOT,
    }
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
            if (type != null && typeof(EquationElement).IsAssignableFrom(type))
                return Activator.CreateInstance(type, args) as EquationElement;
            else
                throw new Exception("Object is not of EquationElement type");
        }
        public static EquationElement Get(OperationsNames name, params object[] args)
        {
            Type type = EquationElement.toTypes[name];
            if (type != null && typeof(EquationElement).IsAssignableFrom(type))
                return Activator.CreateInstance(type, args) as EquationElement;
            else
                throw new Exception("Object is not of EquationElement type");
        }
    }
    [System.Serializable]
    public abstract class EquationElement
    {
        public static OperationsNames GetName(EquationElement element)
        {
            return toEnums[element.GetType()];
        }
        public OperationsNames GetName()
        {
            return toEnums[GetType()];
        }
        public static Dictionary<System.Type, OperationsNames> toEnums = new Dictionary<System.Type, OperationsNames>
        {
            { typeof(Number), OperationsNames.NUMBER },
            { typeof(Plus), OperationsNames.PLUS},
            { typeof(Minus), OperationsNames.MINUS},
            { typeof(Multiply), OperationsNames.MULTIPLY},
            { typeof(Divide), OperationsNames.DIVIDE},
            { typeof(Sqr), OperationsNames.SQUARE},
            { typeof(Sqrt), OperationsNames.SQUARE_ROOT},
        };
        public static Dictionary<OperationsNames, System.Type> toTypes = new Dictionary<OperationsNames, System.Type>
        {
            { OperationsNames.NUMBER, typeof(Number) },
            { OperationsNames.PLUS, typeof(Plus)},
            { OperationsNames.MINUS, typeof(Minus)},
            { OperationsNames.MULTIPLY, typeof(Multiply)},
            { OperationsNames.DIVIDE, typeof(Divide)},
            { OperationsNames.SQUARE, typeof(Sqr)},
            { OperationsNames.SQUARE_ROOT, typeof(Sqrt)},
        };
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
