using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Equation;
using System.Linq;

public class EquationTestBehaviour : MonoBehaviour
{
    [SerializeField]
    public Formula equation;
    void Start()
    {
        //sqrt(sqrt((81+19)*100)) = 10
        equation.AddElement(new Sqrt());
        //open bracket omited
        equation.AddElement(new Sqrt());
        equation.AddElement(new BracketOpen());
        equation.AddElement(new BracketOpen());
        equation.AddElement(new Number(81));
        equation.AddElement(new Plus());
        equation.AddElement(new Number(19));
        equation.AddElement(new BracketClose());
        equation.AddElement(new Multiply());
        equation.AddElement(new Number(100));
        equation.AddElement(new BracketClose());
        //close bracket omited

        Debug.Log("Result is " + equation.Calculate());
    }
}
