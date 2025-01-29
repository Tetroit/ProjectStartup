using config;
using events;
using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardWrapper : MonoBehaviour
{
    public event Action<CardWrapper> OnCardStartDragStarted;
    public event Action<CardWrapper> OnCardDragEnded;

    private const float EPS = 0.01f;
    public Material Material { get; private set; }
    public CardEffect CardEffect { get; private set; }

    public float targetRotation;
    public Vector3 targetPosition;
    private Vector3 dragStartPos;
    private Vector3 slotPosition;
    public float targetVerticalDisplacement;

    private AnimationSpeedConfig animationSpeedConfig;
    private EventsConfig eventsConfig;
    private CardWrapper currentDraggedCard;
    private Canvas cardCanvas;

    private bool isDragged;

    public bool isCardPlayed;

    private int xDeg = 40;

    private const string dragLayer = "Drag";

    private void Awake()
    {
        //var renderer = GetComponentInChildren<Renderer>();
        //var mat = renderer.material;
        //Material = new Material(mat);
        //renderer.material = Material;

        CardEffect = GetComponent<CardEffect>();

        cardCanvas = GetComponentInChildren<Canvas>();
        //if (cardCanvas != null)
        //{
        //    cardCanvas.overrideSorting = true;
        //}
    }

    private void Start()
    {

    }

    private void Update()
    {
        if(!isCardPlayed)
        {
            UpdateRotation();
            UpdatePosition();
        }
    }

    public void Initialize(AnimationSpeedConfig animationSpeedConfig, EventsConfig eventsConfig, CardWrapper currentDraggedCard)
    {
        this.animationSpeedConfig = animationSpeedConfig;
        this.eventsConfig = eventsConfig;
        eventsConfig.OnCardPlayed += OnCardPlayed;
        eventsConfig.OnCardRemove += OnCardRemoved;
        this.currentDraggedCard = currentDraggedCard;
    }

    public void OnCardPlayed(CardPlayed card)
    {
        if(card.card != this)
        {
            return;
        }

        eventsConfig.OnCardPlayed -= OnCardPlayed;
        isCardPlayed = true;
    }

    public void OnCardRemoved(CardRemove card)
    {
        if(card.card != this)
        {
            return;
        }

        eventsConfig.OnCardRemove -= OnCardRemoved;
    }

    private void UpdatePosition()
    {
        if(!isDragged)
        {
            Vector3 upDirection = transform.rotation * Vector3.up;
            Vector3 target = targetPosition + upDirection * targetVerticalDisplacement;

            var distance = Vector3.Distance(transform.position, target);
            var repositionSpeed = transform.position.y > target.y || transform.position.y < 0
                ? animationSpeedConfig.releasePosition
                : animationSpeedConfig.position;

            // Whenever I mouserelease a card into the slot I want to reasing the target to that position.
            var lerped = Vector3.Lerp(transform.position, target, repositionSpeed / distance * Time.deltaTime);
            transform.position = lerped;
        } else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var layerMask = LayerMask.GetMask(dragLayer);
            if(Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask))
            {
                transform.position = hit.point + dragStartPos;
            }
        }
    }

    private void UpdateRotation()
    {
        var currentAngle = transform.rotation.eulerAngles.z;
        currentAngle = currentAngle < 0 ? currentAngle + 360 : currentAngle;
        var tempTargetRotation = isDragged ? 0 : targetRotation;
        tempTargetRotation = tempTargetRotation < 0 ? tempTargetRotation + 360 : tempTargetRotation;
        var deltaAngle = Mathf.Abs(currentAngle - tempTargetRotation);
        //if(!(deltaAngle > EPS)) return;

        var adjustedCurrent = deltaAngle > 180 && currentAngle < tempTargetRotation ? currentAngle + 360 : currentAngle;
        var adjustedTarget = deltaAngle > 180 && currentAngle > tempTargetRotation
            ? tempTargetRotation + 360
            : tempTargetRotation;
        var newDelta = Mathf.Abs(adjustedCurrent - adjustedTarget);

        var nextRotation = Mathf.Lerp(adjustedCurrent, adjustedTarget,
            animationSpeedConfig.rotation / newDelta * Time.deltaTime);
        transform.rotation = Quaternion.Euler(xDeg, -2f, nextRotation);
    }

    public void UpdateSortingOrder(int sortingOrder)
    {
        if(cardCanvas != null)
        {
            cardCanvas.sortingOrder = sortingOrder + 1; 
        }
        //Material.renderQueue = sortingOrder;
    }

    private void OnMouseDown()
    {
        
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray destinationRay = Camera.main!.ScreenPointToRay(mousePosition);
            if(Physics.Raycast(destinationRay, out RaycastHit hit, float.MaxValue))
            {
                dragStartPos = transform.position - hit.point;
                isCardPlayed = false;
                isDragged = true;
                OnCardStartDragStarted?.Invoke(this);
                eventsConfig?.RaiseOnCardClick(new CardClick(this));
            }
        }
    }

    private void OnMouseUp()
    {
        isDragged = false;
        OnCardDragEnded?.Invoke(this);
        eventsConfig?.RaiseCardRelease(new CardRelease(this));
    }
}