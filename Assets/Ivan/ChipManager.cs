using config;
using Equation;
using events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.WSA;

public class ChipManager : MonoBehaviour
{
    [HideInInspector]
    public Chip selected;

    [SerializeField]
    GameObject inventoryArea;
    [SerializeField]
    GameObject equationArea;
    [SerializeField]
    ChipsPool pool;
    [SerializeField]
    GameObject chipPrefab;

    [SerializeField]
    private EventsConfig eventConfig;
    public AutoLayout inventoryLayout { get; private set; }
    public AutoLayout equationLayout { get; private set; }

    List<Chip> chips = new List<Chip>();

    static ChipManager _instance;
    public static ChipManager instance => _instance;

    Formula formula = new Formula();
    public int result = 0;

    int numbersPlayed = 0;
    int operationsPlayed = 0;

    bool _allowInteraction = true;
    public bool allowInteraction => _allowInteraction;

    [Serializable]
    public struct EqElToGameObject
    {
        public string equationElement;
        public GameObject gameObject;
    }

    [SerializeField]
    List<EqElToGameObject> _prefabDict;

    public Dictionary<string, GameObject> prefabDict => 
        _prefabDict.ToDictionary(item => item.equationElement, item => item.gameObject);

    void Awake()
    {
        if (instance == null)
        {
            _instance = this;
        }
        else if (instance != this) Destroy(gameObject);
    }

    public IEnumerable<Chip> GetInventoryChips()
    {
        return inventoryLayout.GetObjects<Chip>();
    }
    public IEnumerable<EquationElement> GetInventoryElements()
    {
        Chip[] inventoryChips = inventoryLayout.GetObjects<Chip>().ToArray();
        EquationElement[] inventoryElements = new EquationElement[inventoryChips.Length];
        for (int i=0; i<inventoryElements.Length; i++)
        {
            inventoryElements[i] = inventoryChips[i].element;
        }
        return inventoryElements;
    }
    public IEnumerable<Chip> GetEquationChips()
    {
        return equationLayout.GetObjects<Chip>();
    }
    public GameObject CreateNewChip(EquationElement element, AutoLayout layout = null)
    {
        string id;
        if (element.type == EquationElement.Type.NUMBER)
            id = (element as Number).value.ToString();
        else
            id = element.GetType().Name;

        GameObject instance = Instantiate(prefabDict[id], transform);
        Chip chip = instance.GetComponent<Chip>();
        chip.element = element;

        if (layout != null)
            AddToLayout(chip, layout);

        instance.transform.localRotation = Quaternion.identity;

        return instance;
    }
    public void OnEnable()
    {
        //OnStateChanged(GameManager.instance.currentState);
        //GameManager.instance.OnGameStateChange.AddListener(OnStateChanged);
        //GameManager.instance.OnValidateTurn += ValidateResult;
        eventConfig.OnCardPlayed += OnCardPlayed;
        eventConfig.OnCardRemove += OnCardRemove;
        GameManager.instance.OnTurnPassed += Play;
    }
    public void OnDisable()
    {
        //GameManager.instance.OnGameStateChange.RemoveListener(OnStateChanged);
        //GameManager.instance.OnValidateTurn -= ValidateResult;
        eventConfig.OnCardPlayed -= OnCardPlayed;
        eventConfig.OnCardRemove -= OnCardRemove;
        GameManager.instance.OnTurnPassed -= Play;
    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
        inventoryLayout = inventoryArea.GetComponent<AutoLayout>();
        equationLayout = equationArea.GetComponent<AutoLayout>();
    }
    private void Start()
    {
        foreach (Transform tr in equationArea.transform) 
        {
            Chip chip = tr.GetComponent<Chip>();
            if (chip && chip.element != null)
                formula.AddElement(chip.element);
        }
        for (int i = 0; i < 8; i++)
            CreateNewChip(pool.GetNumber(GetInventoryElements()), inventoryLayout);
        for (int i = 0; i < 8; i++)
            CreateNewChip(pool.GetOperation(GetInventoryElements()), inventoryLayout);

        UpdateEquation();
    }
    private void Update()
    {
        if (selected != null)
        {
            selected.transform.localPosition = Utils.GetMousePos(transform);
            if (Input.GetMouseButtonUp(0))
            {
                if (selected.selfDestructable)
                    DeselectSelfDestructive();
                else
                    Deselect();
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Play();
        }
    }
    [ExecuteAlways]
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
    }
   
    public void AddChip(Chip chip)
    {
        if (!chips.Contains(chip))
        {
            chips.Add(chip);
            if (chip.transform.parent == inventoryArea.transform)
                chip.layout = inventoryLayout;
            if (chip.transform.parent == equationArea.transform)
                chip.layout = equationLayout;
        }
    }
    public void RemoveChip(Chip chip, bool destroy = false)
    {
        if (chips.Contains(chip))
            chips.Remove(chip);

        if (destroy)
            Destroy(chip.gameObject);
    }
    public void Select(Chip chip)
    {
        if (chip.layout)
            chip.layout.RemoveItem(chip.gameObject, true);

        if (!chip.selfDestructable)
            inventoryLayout.EnableShifting();
        equationLayout.EnableShifting();

        selected = chip;
        chip.transform.parent = transform;
    }
    public void DeselectSelfDestructive()
    {
        if (equationLayout.currentShiftID != -1)
        {
            AddToLayout(selected, equationLayout, equationLayout.currentShiftID);
        }
        else
        {
            if (selected.layout == equationLayout)
            {
                equationLayout.RemoveItem(selected.gameObject, true);
                formula.RemoveElement(selected.element);
                UpdateEquation();
            }
            Destroy(selected.gameObject);
        }

        equationLayout.DisableShifting();
        selected = null;
    }
    public void Deselect()
    {

        if (equationLayout.currentShiftID != -1)
        {
            AddToLayout(selected, equationLayout, equationLayout.currentShiftID);
        }
        else if (inventoryLayout.currentShiftID != -1)
        {
            AddToLayout(selected, inventoryLayout, inventoryLayout.currentShiftID);
        }
        else
        {
            AddBack(selected);
        }

        inventoryLayout.DisableShifting();
        equationLayout.DisableShifting();
        selected = null;
    }

    public void AddBack(Chip chip)
    {
        AddToLayout(chip, chip.layout);
    }

    public void AddToLayout(Chip chip, AutoLayout layout, int id = -1)
    {
        if (chip.layout == equationLayout)
        {
            formula.RemoveElement(chip.element);
            UpdateEquation();
        }
        if (layout == equationLayout)
        {   if (id == -1)
                formula.AddElement(chip.element);
            else
                formula.AddElement(chip.element, id);
            UpdateEquation();
        }

        chip.layout = layout;

        if (id == -1)
            layout.AddObject(chip.gameObject, true);
        else
            layout.AddObject(chip.gameObject, id, true);

    }
    void UpdateEquation()
    {
        bool valid = formula.Validate();
        if (valid)
        {
            result = formula.Calculate();
        }
    }
    
    public bool Validate()
    {
        return formula.Validate();
    }
    void OnStateChanged(GameState state)
    {
        Debug.Log("Switching state to " + state);
        if (state.allowChipInteraction && !allowInteraction)
            EnableInteraction();
        if (!state.allowChipInteraction && allowInteraction)
            DisableInteraction();
    }

    public void DisableInteraction()
    {
        _allowInteraction = false;
        if (selected != null)
            AddBack(selected);

        inventoryLayout.DisableShifting();
        equationLayout.DisableShifting();
        selected = null;

        foreach (Chip chip in chips)
        {
            chip.isInteractable = false;
        }
    }

    public void EnableInteraction()
    {
        _allowInteraction = true;
        foreach (Chip chip in chips)
        {
            chip.isInteractable = true;
        }
    }

    public void Play()
    {
        IEnumerable<EquationElement> IE = GetInventoryElements();
        for (int i = 0; i<numbersPlayed; i++)
        {
            EquationElement element = pool.GetNumber(IE);
            CreateNewChip(element, inventoryLayout);
        }

        for (int i = 0; i < operationsPlayed; i++)
        {
            EquationElement element = pool.GetOperation(IE);
            CreateNewChip(element, inventoryLayout);
        }
    }

    public void ReadFormula(Formula formula)
    {
        if (equationLayout.Count != 0)
        {
            foreach (var item in GetEquationChips())
            {
                AddToLayout(item, inventoryLayout);
            }
        }
        var coll = formula.GetElements().ToArray();
        for (int i = 0; i < coll.Length; i++)
        {
            if (coll[i].GetType() == typeof(Number))
                numbersPlayed--;
            else if (
                coll[i].GetType() != typeof(BracketOpen) &&
                coll[i].GetType() != typeof(BracketClose)
                )
                operationsPlayed--;
            CreateNewChip(coll[i], equationLayout);
        }
    }
    public void PushFormula(CardWrapper card)
    {
        card.formula = formula;
        formula = new Formula();

        numbersPlayed = 0;
        operationsPlayed = 0;

        Chip[] equationChips = GetEquationChips().ToArray();
        for (int i = 0; i < equationChips.Length; i++)
        {
            Chip chip = equationChips[i];
            if (!chip.selfDestructable)
            {
                if (chip.element.GetType() == typeof(Number))
                    numbersPlayed++;
                else if (!chip.selfDestructable)
                    operationsPlayed++;
            }
            RemoveChip(chip, true);
            equationLayout.RemoveItem(0);
        }
    }

    public void OnCardPlayed(CardPlayed cardPlayed)
    {
        if (cardPlayed.card.formula == null || cardPlayed.card.formula.size == 0)
        {
            Debug.Log(name + ": Card played");
            PushFormula(cardPlayed.card);
            UpdateEquation();
        }
    }
    public void OnCardRemove(CardRemove cardRemove)
    {
        //formula = cardRemove.card.formula;
        if (cardRemove.card.formula != null && cardRemove.card.formula.size != 0)
        {
            Debug.Log(name + ": Card removed");
            ReadFormula(cardRemove.card.formula);
            cardRemove.card.formula = null;
            UpdateEquation();
        }
    }
}
