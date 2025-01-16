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
        //7*(6+2*5) 16*7 = 112

        //open bracket omited

        //close bracket omited
        equation.AddElement(new Negate());
        equation.AddElement(new Number(7));
        Debug.Log("Result is " + equation.Calculate());
    }
}
