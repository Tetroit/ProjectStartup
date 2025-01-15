using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Equation
{
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

        public int result;
        public int ID;

        public bool calculated = false;
        public abstract IEnumerable<EquationElement> GetDependencies();
        public EquationElement(Type type) { _type = type; }
        public abstract bool YieldResult();
    }
}
