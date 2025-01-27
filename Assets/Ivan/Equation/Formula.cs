using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Equation
{
    using static UnityEngine.Rendering.DebugUI;
    using ElType = EquationElement.Type;

    [System.Serializable]
    public class Formula
    {
        [SerializeField]
        List<EquationElement> elements = new List<EquationElement>();
        public int size { get { return elements.Count; } }
        List<BracketOpen> bracketOrder = new List<BracketOpen>();

        public StackOverflowLock stackOverflowLock;

        public Formula()
        {
            stackOverflowLock = new StackOverflowLock(this);
        }
        static Dictionary<ElType, List<ElType>> validationTable = new Dictionary<ElType, List<ElType>>()
        {
            {ElType.NONE, new List<ElType> (){ElType.NUMBER, ElType.FUNCTION, ElType.BRACKET_OPEN, ElType.NONE} },
            {ElType.NUMBER, new List<ElType> () {ElType.OPERATOR, ElType.BRACKET_CLOSE, ElType.NONE } },
            {ElType.OPERATOR, new List<ElType> () {ElType.NUMBER, ElType.FUNCTION, ElType.BRACKET_OPEN} },
            {ElType.FUNCTION, new List<ElType> () {ElType.NUMBER, ElType.FUNCTION, ElType.BRACKET_OPEN} },
            {ElType.BRACKET_OPEN, new List<ElType> () {ElType.NUMBER, ElType.FUNCTION, ElType.BRACKET_OPEN} },
            {ElType.BRACKET_CLOSE, new List<ElType> () {ElType.OPERATOR, ElType.BRACKET_CLOSE, ElType.NONE} },

        };

        public bool ValidateCombination(ElType first, ElType second)
        {
            return validationTable[first].Contains(second);
        }
        public void AddElement(EquationElement element)
        {
            element.ID = elements.Count;
            elements.Add(element);
            element.OnFormulaAdded(this);
        }
        public void AddElement(EquationElement element, int id)
        {
            id = Math.Clamp(id, 0, elements.Count);
            for (int i = id; i < elements.Count; i++)
                elements[i].ID++;
            element.ID = id;
            elements.Insert(id, element);
            element.OnFormulaAdded(this);
        }
        public void RemoveElement(EquationElement element)
        {
            bool found = false;
            for (int i=0; i < elements.Count; i++)
            {
                if (found)
                    elements[i].ID--;
                if (element == elements[i])
                {
                    elements.RemoveAt(i);
                    i--;
                    found = true;
                }
            }
        }
        public void Clear()
        {
            elements.Clear();
        }
        public IEnumerable GetElements()
        {
            return elements;
        }
        public bool Validate()
        {
            if (elements.Count == 0) return true;
            for (int i=0; i<elements.Count; i++)
            {
                if (elements[i].ID != i) throw new Exception("ORDER");
            }
            ElType current = elements[0].type;
            ElType previous = ElType.NONE;
            if (!ValidateCombination(previous, current)) return false;

            for (int i=1; i<elements.Count; i++)
            {
                previous = current;
                current = elements[i].type;
                if (!ValidateCombination(previous, current)) return false;
            }

            if (!ValidateCombination(current, ElType.NONE)) return false;
            if (!PairBrackets()) return false;

            return true;
        }
        public int Calculate()
        {
            stackOverflowLock.Reset();

            if (!Validate())
            {
                Debug.LogError("invalid equation");
                return 0;
            }
            foreach (EquationElement element in elements)
            {
                element.calculated = false;
            }
            for (int i=0; i<bracketOrder.Count; i++)
            {
                bracketOrder[i].SetDependency(CalculateDependencies(bracketOrder[i].ID+1, bracketOrder[i].pair.ID));
            }
            EquationElement last = CalculateDependencies(0, elements.Count);
            if (last != null)
            {
                last.YieldResult();
                return last.result;
            }
            return 0;

        }
        EquationElement CalculateDependencies(int startID, int endID)
        {
            if (startID == endID) return null;
            //Debug.Log("Calculating range (" + startID + ", " + endID + ")");
            EquationElement leastPriorityElement = elements[startID];
            int leastPriority = 0;
            for (int i=startID; i<endID; i++)
            {
                switch (elements[i].type)
                {
                    case ElType.NUMBER:
                        if (leastPriority < elements[i].priority)
                        {
                            leastPriorityElement = elements[i];
                            leastPriority = elements[i].priority;
                        }
                        break;
                    case ElType.BRACKET_OPEN:


                        if (leastPriority < elements[i].priority)
                        {
                            leastPriorityElement = elements[i];
                            leastPriority = elements[i].priority;
                        }
                        i = (elements[i] as BracketOpen).pair.ID;
                        break;
                    case ElType.FUNCTION:
                        if (leastPriority < elements[i].priority)

                        {
                            leastPriorityElement = elements[i];
                            leastPriority = elements[i].priority;
                        }
                        (elements[i] as Function).SetDependency(elements[i + 1]);
                        break;
                    case ElType.OPERATOR:

                        if (leastPriority <= elements[i].priority)
                        {
                            leastPriorityElement = elements[i];
                            leastPriority = elements[i].priority;
                        }
                        Operator op = (Operator)elements[i];
                        op.SetDependency(0, GetLeft(i, elements[i].priority));
                        op.SetDependency(1, GetRight(i, elements[i].priority));
                        break;
                }
            }
            return leastPriorityElement;
        }
        EquationElement GetLeft(int ID, int pPriority)
        {
            //we find an operation with the least priority, but more or equally prioritised as target
            EquationElement leastPriorityElement = null;
            int leastPriority = 0;
            for (int i = ID - 1; i >= 0; i--)
            {
                //everything beyond is less priority
                if (elements[i].type == ElType.BRACKET_OPEN)
                    break;

                //consider brackets as one = jump over them
                if (elements[i].type == ElType.BRACKET_CLOSE)
                    i = ((BracketClose)elements[i]).pair.ID;

                int currentPriority = elements[i].priority;

                //less priority = no search further
                if (currentPriority > pPriority)
                    break;

                //found same priority = cannot find lesser priority = answer found
                if (currentPriority == pPriority)
                {
                    //Debug.Log("left of " + ID + ": " + elements[i]);
                    return elements[i];
                }

                //found lesser priority = new potential left argument
                if (leastPriority < currentPriority)
                {
                    leastPriorityElement = elements[i];
                    leastPriority = currentPriority;
                }
            }
            //Debug.Log("left of " + ID + ": " + leastPriorityElement);
            return leastPriorityElement;
        }
        EquationElement GetRight(int ID, int pPriority)
        {
            //we find an operation with the least priority, but more prioritised as target
            EquationElement leastPriorityElement = null;
            int leastPriority = 0;
            for (int i = ID + 1; i < elements.Count; i++)
            {
                int currentPriority = elements[i].priority;

                //everything beyond is lesser priority
                if (elements[i].type == ElType.BRACKET_CLOSE)
                    break;

                //less or equal priority = no search further
                if (currentPriority >= pPriority)
                    break;

                //found lesser or equal priority = new potential left argument
                if (leastPriority <= currentPriority)
                {
                    leastPriorityElement = elements[i];
                    leastPriority = currentPriority;
                }

                //consider brackets as one = jump over them
                if (elements[i].type == ElType.BRACKET_OPEN)
                    i = ((BracketOpen)elements[i]).pair.ID;
            }
            //Debug.Log("right of " + ID + ": " + leastPriorityElement);
            return leastPriorityElement;
        }
        bool PairBrackets()
        {
            bracketOrder.Clear();
            Stack<BracketOpen> order = new Stack<BracketOpen>();
            for (int i=0; i<elements.Count; i++)
            {
                EquationElement element = elements[i];
                if (element.type == ElType.BRACKET_OPEN)
                {
                    BracketOpen bracketOpen = (BracketOpen)element;
                    order.Push(bracketOpen);
                }
                if (element.type == ElType.BRACKET_CLOSE)
                {
                    BracketClose bracketClose = (BracketClose)element;
                    if (order.Count == 0) return false;
                    BracketOpen bracketOpen = order.Pop();
                    bracketOpen.pair = bracketClose;
                    bracketClose.pair = bracketOpen;

                    bracketOrder.Add(bracketOpen);
                }
            }
            if (order.Count != 0) return false;
            return true;
        }

        public override string ToString()
        {
            string res = "= ";
            foreach(EquationElement element in elements)
            {
                res += element.ToString();
            }
            return res;
        }
    }
}
