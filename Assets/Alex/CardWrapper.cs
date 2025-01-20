using config;
using events;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardWrapper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler
{
    public event Action<CardWrapper> OnCardStartDragEvent;
    public event Action<CardWrapper> OnCardDragEnded;

    private const float EPS = 0.01f;
    public Material Material { get; private set; }

    public float targetRotation;
    public Vector3 targetPosition;
    public float targetVerticalDisplacement;
    public int uiLayer;

    private ZoomConfig zoomConfig;
    private AnimationSpeedConfig animationSpeedConfig;

    private bool isHovered;
    private bool isDragged;
    private Vector3 dragStartPos;
    private EventsConfig eventsConfig;
    private bool preventCardInteraction;

    private bool isCardPlayed;

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
            UpdateUILayer();
        }
    }

    public void Initialize(ZoomConfig zoomConfig, AnimationSpeedConfig animationSpeedConfig, EventsConfig eventsConfig, bool preventCardInteraction)
    {
        this.zoomConfig = zoomConfig;
        this.animationSpeedConfig = animationSpeedConfig;
        this.eventsConfig = eventsConfig;
        this.preventCardInteraction = preventCardInteraction;
        eventsConfig?.OnCardPlayed.AddListener(OnCardPlayed);
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

    private void UpdateUILayer()
    {
        if(!isHovered && !isDragged)
        {
            //canvas.sortingOrder = uiLayer;
        }
    }

    private void UpdatePosition()
    {
        if(!isDragged)
        {
            Vector3 upDirection = transform.rotation * Vector3.up;
            Vector3 target = targetPosition + upDirection * targetVerticalDisplacement;

            if(isHovered && zoomConfig.overrideYPosition != -1)
            {
                //target = new Vector3(target.x, zoomConfig.overrideYPosition);
            }

            var distance = Vector3.Distance(transform.position, target);
            var repositionSpeed = transform.position.y > target.y || transform.position.y < 0
                ? animationSpeedConfig.releasePosition
                : animationSpeedConfig.position;

            var lerped = Vector3.Lerp(transform.position, target,
                repositionSpeed / distance * Time.deltaTime);
            transform.position = lerped;
        } else
        {
            var delta = ((Vector3)Input.mousePosition + dragStartPos);
            transform.position = new Vector3(delta.x, delta.y, transform.position.z);
        }
    }

    private void UpdateRotation()
    {
        var crtAngle = transform.rotation.eulerAngles.z;
        // If the angle is negative, add 360 to it to get the positive equivalent
        crtAngle = crtAngle < 0 ? crtAngle + 360 : crtAngle;
        // If the card is hovered and the rotation should be reset, set the target rotation to 0
        var tempTargetRotation = (isHovered || isDragged) && zoomConfig.resetRotationOnZoom
            ? 0
            : targetRotation;
        tempTargetRotation = tempTargetRotation < 0 ? tempTargetRotation + 360 : tempTargetRotation;
        var deltaAngle = Mathf.Abs(crtAngle - tempTargetRotation);
        if(!(deltaAngle > EPS)) return;

        // Adjust the current angle and target angle so that the rotation is done in the shortest direction
        var adjustedCurrent = deltaAngle > 180 && crtAngle < tempTargetRotation ? crtAngle + 360 : crtAngle;
        var adjustedTarget = deltaAngle > 180 && crtAngle > tempTargetRotation
            ? tempTargetRotation + 360
            : tempTargetRotation;
        var newDelta = Mathf.Abs(adjustedCurrent - adjustedTarget);

        var nextRotation = Mathf.Lerp(adjustedCurrent, adjustedTarget,
            animationSpeedConfig.rotation / newDelta * Time.deltaTime);
        transform.rotation = Quaternion.Euler(45, 0, nextRotation);
    }


    public void SetAnchor(Vector3 min, Vector3 max)
    {
        //transform.anchorMin = min;
        //transform.anchorMax = max;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Hover
        if(isDragged)
        {
            // Avoid hover events while dragging
            return;
        }
        if(zoomConfig.bringToFrontOnHover)
        {
            //canvas.sortingOrder = zoomConfig.zoomedSortOrder;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Unhover
        if(isDragged)
        {
            // Avoid hover events while dragging
            return;
        }

        //canvas.sortingOrder = uiLayer;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(preventCardInteraction) return;
        isDragged = true;
        isHovered = true;
        dragStartPos = new Vector3(base.transform.position.x - eventData.position.x,
            base.transform.position.y - eventData.position.y);
        OnCardStartDragEvent?.Invoke(this);
        eventsConfig?.OnCardClkick?.Invoke(new CardClick(this));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragged = false;
        isHovered = false;
        OnCardDragEnded?.Invoke(this);
        eventsConfig?.OnCardRelease?.Invoke(new CardRelease(this));
    }
}