using config;
using events;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardWrapper : MonoBehaviour
{
    public event Action<CardWrapper> OnCardStartDragStarted;
    public event Action<CardWrapper> OnCardDragEnded;

    private const float EPS = 0.01f;
    public Material Material { get; private set; }

    public float targetRotation;
    public Vector3 targetPosition;
    private Vector3 dragStartPos;
    public float targetVerticalDisplacement;

    private AnimationSpeedConfig animationSpeedConfig;
    private EventsConfig eventsConfig;
    private CardWrapper currentDraggedCard;

    private bool isDragged;

    private bool isCardPlayed;

    private int xDeg = 75;

    private void Awake()
    {
        var renderer = GetComponent<Renderer>();
        var mat = renderer.material;
        Material = new Material(mat);
        renderer.material = Material;
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
        eventsConfig?.OnCardPlayed.AddListener(OnCardPlayed);
        this.currentDraggedCard = currentDraggedCard;
    }

    private void OnCardPlayed(CardPlayed card)
    {
        if(card.card != this)
        {
            return;
        }

        eventsConfig?.OnCardPlayed.RemoveListener(OnCardPlayed);
        isCardPlayed = true;
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

            var lerped = Vector3.Lerp(transform.position, target,repositionSpeed / distance * Time.deltaTime);
            transform.position = lerped;
        } else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                transform.position = hit.point - dragStartPos;
            }
        }
    }

    private void UpdateRotation()
    {
        var currentAngle = transform.rotation.eulerAngles.z;
        currentAngle = currentAngle < 0 ? currentAngle + 360 : currentAngle;
        var tempTargetRotation = isDragged? 0: targetRotation;
        tempTargetRotation = tempTargetRotation < 0 ? tempTargetRotation + 360 : tempTargetRotation;
        var deltaAngle = Mathf.Abs(currentAngle - tempTargetRotation);
        if(!(deltaAngle > EPS)) return;

        var adjustedCurrent = deltaAngle > 180 && currentAngle < tempTargetRotation ? currentAngle + 360 : currentAngle;
        var adjustedTarget = deltaAngle > 180 && currentAngle > tempTargetRotation
            ? tempTargetRotation + 360
            : tempTargetRotation;
        var newDelta = Mathf.Abs(adjustedCurrent - adjustedTarget);

        var nextRotation = Mathf.Lerp(adjustedCurrent, adjustedTarget,
            animationSpeedConfig.rotation / newDelta * Time.deltaTime);
        transform.rotation = Quaternion.Euler(xDeg, 0, nextRotation);
    }

    private void OnMouseDown()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray destinationRay = Camera.main!.ScreenPointToRay(mousePosition);
            if(Physics.Raycast(destinationRay, out RaycastHit hit, float.MaxValue))
            {
                dragStartPos = hit.point - transform.position;
                isDragged = true;
                OnCardStartDragStarted?.Invoke(this);
                eventsConfig?.OnCardClick?.Invoke(new CardClick(this));
            }
        }
    }

    private void OnMouseUp()
    {
        isDragged = false;
        OnCardDragEnded?.Invoke(this);
        eventsConfig?.OnCardRelease?.Invoke(new CardRelease(this));

        if(currentDraggedCard != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray destinationRay = Camera.main!.ScreenPointToRay(mousePosition);
            if(Physics.Raycast(destinationRay, out RaycastHit hit, float.MaxValue))
            {

            }
        }
    }
}