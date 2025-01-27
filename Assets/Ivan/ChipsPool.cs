
using Equation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Chip pool", menuName = "Chip Pool")]
public class ChipsPool : ScriptableObject, IChipsPool
{
    //stupid ass unity cannot serialize dictionaries
    //so I have to do it instead
    //:sigh_emoji:

    [System.Serializable]
    public struct ONtoIntPair 
    { 
        public OperationsNames name; 
        public int number;
    }
    [System.Serializable]
    public struct ONtoFloatPair
    {
        public OperationsNames name;
        public float number;
    }

    [SerializeField]
    ONtoIntPair[] _amountLimit;
    [SerializeField]
    ONtoFloatPair[] _spawnProbabilities;

    public Dictionary<OperationsNames, float> spawnProbabilities => 
        _spawnProbabilities.ToDictionary(item => item.name, item=>item.number);
    public Dictionary<OperationsNames, int> amountLimit => 
        _amountLimit.ToDictionary(item => item.name, item => item.number);

    public List<OperationsNames> numberTypes = new List<OperationsNames>() 
    { 
        OperationsNames.NUMBER 
    };
    public List<OperationsNames> operationTypes = new List<OperationsNames>() 
    {
        OperationsNames.PLUS,
        OperationsNames.MINUS,
        OperationsNames.MULTIPLY,
        OperationsNames.DIVIDE,
        OperationsNames.SQUARE,
    };
    public int numberLimit = 3;

    public int maxNumber = 9;
    public int minNumber = 1;
    public EquationElement GetAny(IEnumerable<EquationElement> inventory, float numberProbability)
    {
        float rng = UnityEngine.Random.Range(0f, 1f);
        if (rng < numberProbability)
            return GetNumber(inventory);
        else
            return GetOperation(inventory);
    }

    public EquationElement GetNumber(IEnumerable<EquationElement> inventory)
    {
        Dictionary<int,int> inventoryCount = new Dictionary<int, int>();
        List<int> allowed = new List<int>();

        //count each chip in inventory

        foreach (var chip in inventory)
        {
            //filter numbers
            if (chip.type != EquationElement.Type.NUMBER)
                continue;

            int num = ((Number)chip).value;
            if (inventoryCount.Keys.Contains(num))
                inventoryCount[num]++;
            else
                inventoryCount.Add(num, 1);
        }

        //determine if there are too many chips of 1 type

        for (int i=minNumber; i<=maxNumber; i++)
        {
            if (inventoryCount.ContainsKey(i) && inventoryCount[i] >= numberLimit)
                continue;
            
            allowed.Add(i);
        }
        
        int rng = UnityEngine.Random.Range(0, allowed.Count);
        return EquationElementFactory.Get("Number", allowed[rng]);
    }

    public EquationElement GetOperation(IEnumerable<EquationElement> inventory)
    {
        float totalWieght = 0;
        Dictionary<OperationsNames, int> inventoryCount = new Dictionary<OperationsNames, int>();
        Dictionary<float, OperationsNames> rngPool = new Dictionary<float, OperationsNames>();

        //count each chip in inventory

        foreach (var chip in inventory)
        {
            //filter operations
            if (!operationTypes.Contains(chip.GetName()))
                continue;

            if (inventoryCount.Keys.Contains(chip.GetName()))
                inventoryCount[chip.GetName()]++;
            else
                inventoryCount.Add(chip.GetName(), 1);
        }

        //determine if there are too many chips of 1 type

        foreach (var entry in spawnProbabilities)
        {
            if (inventoryCount.ContainsKey(entry.Key) && inventoryCount[entry.Key] >= amountLimit[entry.Key])
                continue;
            totalWieght += entry.Value;
            rngPool.Add(totalWieght, entry.Key);
        }
        float rng = UnityEngine.Random.Range(0, totalWieght);
        for (int i = 0; i < rngPool.Count; i++)
        {
            (float key, OperationsNames value) = rngPool.ElementAt(i);
            if (rng < key)
                return EquationElementFactory.Get(value);
        }
        return null;
    }
}

public interface IChipsPool
{
    //EquationElement GetAny(IEnumerable<EquationElement> inventory);
    EquationElement GetNumber(IEnumerable<EquationElement> inventory);
    EquationElement GetOperation(IEnumerable<EquationElement> inventory);
}