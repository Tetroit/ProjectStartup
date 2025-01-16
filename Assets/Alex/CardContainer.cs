using System.Collections.Generic;
using System.Linq;
using config;
using DefaultNamespace;
using events;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class CardContainer : MonoBehaviour
{
    [Header("Constraints")]
    private bool preventCardInteraction = false;

    [Header("Alignment")]
    [SerializeField]
    private CardAlignment alignment = CardAlignment.Center;

    [SerializeField]
    private bool allowCardRepositioning = true;

    [Header("Rotation")]
    [SerializeField]
    [Range(-90f, 90f)]
    private float maxCardRotation;

    [SerializeField]
    private float maxHeightDisplacement;

    [SerializeField]
    private ZoomConfig zoomConfig;

    [SerializeField]
    private AnimationSpeedConfig animationSpeedConfig;

    [SerializeField]
    private CardPlayConfig cardPlayConfig;

    [Header("Events")]
    [SerializeField]
    private EventsConfig eventsConfig;

    private List<CardWrapper> cards = new();

    private RectTransform rectTransform;
    private CardWrapper currentDraggedCard;

    private bool forceFitContainer;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        InitCards();
        UpdateContainerPosition();
    }

    private void InitCards()
    {
        SetUpCards();
        SetCardsAnchor();
    }

    private void OnValidate()
    {
        UpdateContainerPosition();
    }

    private void SetCardsRotation()
    {
        for(var i = 0; i < cards.Count; i++)
        {
            cards[i].targetRotation = GetCardRotation(i);
            cards[i].targetVerticalDisplacement = GetCardVerticalDisplacement(i);
        }
    }

    private float GetCardVerticalDisplacement(int index)
    {
        if(cards.Count < 3) return 0;
        // Associate a vertical displacement based on the index in the cards list
        // so that the center card is at max displacement while the edges are at 0 displacement
        return maxHeightDisplacement *
               (1 - Mathf.Pow(index - (cards.Count - 1) / 2f, 2) / Mathf.Pow((cards.Count - 1) / 2f, 2));
    }

    private float GetCardRotation(int index)
    {
        if(cards.Count < 3) return 0;
        // Associate a rotation based on the index in the cards list
        // so that the first and last cards are at max rotation, mirrored around the center
        return -maxCardRotation * (index - (cards.Count - 1) / 2f) / ((cards.Count - 1) / 2f);
    }

    void Update()
    {
        UpdateCards();
    }

    void SetUpCards()
    {
        cards.Clear();
        foreach(Transform card in transform)
        {
            var wrapper = card.GetComponent<CardWrapper>();
            if(wrapper == null)
            {
                wrapper = card.gameObject.AddComponent<CardWrapper>();
            }

            cards.Add(wrapper);

            AddOtherComponentsIfNeeded(wrapper);

            // Pass child card any extra config it should be aware of
            wrapper.zoomConfig = zoomConfig;
            wrapper.animationSpeedConfig = animationSpeedConfig;
            wrapper.eventsConfig = eventsConfig;
            wrapper.preventCardInteraction = preventCardInteraction;
            wrapper.container = this;
        }
    }

    private void AddOtherComponentsIfNeeded(CardWrapper wrapper)
    {
        var canvas = wrapper.GetComponent<Canvas>();
        if(canvas == null)
        {
            canvas = wrapper.gameObject.AddComponent<Canvas>();
        }

        canvas.overrideSorting = true;

        if(wrapper.GetComponent<GraphicRaycaster>() == null)
        {
            wrapper.gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    private void UpdateCards()
    {
        if(transform.childCount != cards.Count)
        {
            InitCards();
        }

        if(cards.Count == 0)
        {
            return;
        }

        SetCardsPosition();
        SetCardsRotation();
        SetCardsUILayers();
        UpdateCardOrder();

        if(!IsCursorInPreviewArea() && currentDraggedCard != null)
        {
            eventsConfig?.OnCardRelease?.Invoke(new CardRelease(currentDraggedCard));
        }
    }

    private void SetCardsUILayers()
    {
        for(var i = 0; i < cards.Count; i++)
        {
            cards[i].uiLayer = zoomConfig.defaultSortOrder + i;
        }
    }

    private void UpdateCardOrder()
    {
        if(!allowCardRepositioning || currentDraggedCard == null) return;

        // Get the index of the dragged card depending on its position
        var newCardIdx = cards.Count(card => currentDraggedCard.transform.position.x > card.transform.position.x);
        var originalCardIdx = cards.IndexOf(currentDraggedCard);
        if(newCardIdx != originalCardIdx)
        {
            cards.RemoveAt(originalCardIdx);
            if(newCardIdx > originalCardIdx && newCardIdx < cards.Count - 1)
            {
                newCardIdx--;
            }

            cards.Insert(newCardIdx, currentDraggedCard);
        }
        // Also reorder in the hierarchy
        currentDraggedCard.transform.SetSiblingIndex(newCardIdx);
    }

    private void SetCardsPosition()
    {
        // Compute the total width of all the cards in global space
        var cardsTotalWidth = cards.Sum(card => card.width * card.transform.lossyScale.x);
        DistributeChildrenToFitContainer(cardsTotalWidth);
    }

    private void DistributeChildrenToFitContainer(float childrenTotalWidth)
    {
        // Get the width of the container
        var width = rectTransform.rect.width * transform.lossyScale.x;
        // Get the distance between each child
        var distanceBetweenChildren = (width - childrenTotalWidth) / (cards.Count - 1);
        // Set all children's positions to be evenly spaced out
        var currentX = transform.position.x - width / 2;

        foreach(CardWrapper child in cards)
        {
            var adjustedChildWidth = child.width * child.transform.lossyScale.x;
            child.targetPosition = new Vector2(currentX + adjustedChildWidth / 2, transform.position.y);
            currentX += adjustedChildWidth + distanceBetweenChildren;
        }

    }

    private void UpdateContainerPosition()
    {
        if(rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        switch(alignment)
        {
            case CardAlignment.Left:
                rectTransform.anchoredPosition = new Vector2(-350, rectTransform.anchoredPosition.y);
                break;

            case CardAlignment.Right:
                rectTransform.anchoredPosition = new Vector2(350, rectTransform.anchoredPosition.y);
                break;

            case CardAlignment.Center:
                rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
                break;
        }
    }

    private void SetCardsAnchor()
    {
        foreach(CardWrapper child in cards)
        {
            child.SetAnchor(new Vector2(0, 0.5f), new Vector2(0, 0.5f));
        }
    }

    public void OnCardDragStart(CardWrapper card)
    {
        currentDraggedCard = card;
    }

    public void OnCardDragEnd()
    {
        // If card is in play area, play it!
        if(IsCursorInPlayArea())
        {
            eventsConfig?.OnCardPlayed?.Invoke(new CardPlayed(currentDraggedCard));
            if(cardPlayConfig.destroyOnPlay)
            {
                DestroyCard(currentDraggedCard);
            }
        }
        currentDraggedCard = null;
    }

    public void DestroyCard(CardWrapper card)
    {
        cards.Remove(card);
        eventsConfig.OnCardDestroy?.Invoke(new CardDestroy(card));
        Destroy(card.gameObject);
    }

    private bool IsCursorInPlayArea()
    {
        if(cardPlayConfig.slot_1 == null) return false;

        var cursorPosition = Input.mousePosition;
        var playArea = cardPlayConfig.slot_1;
        var playAreaCorners = new Vector3[4];
        playArea.GetWorldCorners(playAreaCorners);
        return cursorPosition.x > playAreaCorners[0].x &&
               cursorPosition.x < playAreaCorners[2].x &&
               cursorPosition.y > playAreaCorners[0].y &&
               cursorPosition.y < playAreaCorners[2].y;

    }

    private void CheckPlayArea()
    {
        for (int i = 0; i < cardPlayConfig.playableSlots.Count; i++)
        {
            // if slot[i] is != null - return the cards to the hand
            // else - assign the position of the card to slot

        }
    }

    private bool IsCursorInPreviewArea()
    {
        if (cardPlayConfig.previewArea == null) return false;

        var cursorPosition = Input.mousePosition;
        var playArea = cardPlayConfig.previewArea;
        var playAreaCorners = new Vector3[4];
        playArea.GetWorldCorners(playAreaCorners);
        return cursorPosition.x > playAreaCorners[0].x &&
               cursorPosition.x < playAreaCorners[2].x &&
               cursorPosition.y > playAreaCorners[0].y &&
               cursorPosition.y < playAreaCorners[2].y;

    }
}