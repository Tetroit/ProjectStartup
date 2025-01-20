using System.Collections.Generic;
using System.Linq;
using config;
using DefaultNamespace;
using events;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
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

    [SerializeField]
    private float cardWidth = 4f;

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

    private CardWrapper currentDraggedCard;

    private bool forceFitContainer;

    private void Start()
    {
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
        foreach(Transform card in base.transform)
        {
            // get an array of card prefabs
            // spawn them dynamically here

            var wrapper = card.GetComponent<CardWrapper>();
            if(wrapper == null)
            {
                wrapper = card.gameObject.AddComponent<CardWrapper>();
            }

            cards.Add(wrapper);

            // Pass child card any extra config it should be aware of
            wrapper.Initialize(zoomConfig, animationSpeedConfig, eventsConfig, preventCardInteraction);
            wrapper.OnCardStartDragEvent += OnCardDragStart;
            wrapper.OnCardDragEnded += OnCardDragEnd;
        }
    }

    private void OnCardDragStart(CardWrapper card)
    {
        currentDraggedCard = card;
    }

    public void OnCardDragEnd(CardWrapper card)
    {
        if(currentDraggedCard != card)
        {
            return;
        }

        for(int i = 0; i < cardPlayConfig.playableSlots.Count; i++)
        {
            // if slot[i] is != null - return the cards to the hand
            // else - assign the position of the card to slot

            RectTransform slot = cardPlayConfig.playableSlots[i];
            if(currentDraggedCard != null && IsCursorInPlayArea(slot))
            {
                // assign what slot you have targeted


                // remove card from the container (don't destroy)
                // unparent from card container
                // parent to the slot
                eventsConfig?.OnCardPlayed?.Invoke(new CardPlayed(currentDraggedCard));
                RemoveCard(currentDraggedCard);
                currentDraggedCard.transform.SetParent(slot, false);
            }
        }

        currentDraggedCard = null;
    }

    private void UpdateCards()
    {
        if(base.transform.childCount != cards.Count)
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

        cards.Sort((a, b) =>
        {
            return a.uiLayer.CompareTo(b.uiLayer);
        });

        for(var i = 0; i < cards.Count; i++)
        {
            cards[i].Material.renderQueue = (int)RenderQueue.GeometryLast + 100 - i;
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
        var cardsTotalWidth = cards.Sum(card => cardWidth * card.transform.lossyScale.x);
        DistributeChildrenToFitContainer(cardsTotalWidth);
    }

    private void DistributeChildrenToFitContainer(float totalCardWidth)
    {
        // Get the width of the container
        var scaledWidth = transform.position.x * transform.lossyScale.x;
        // Get the distance between each child
        var distanceBetweenChildren = (scaledWidth - totalCardWidth) / (cards.Count - 1);
        // Set all children's positions to be evenly spaced out
        var currentX = transform.position.x - scaledWidth / 2f;

        foreach(CardWrapper child in cards)
        {
            var adjustedChildWidth = cardWidth * child.transform.lossyScale.x;
            child.targetPosition = new Vector3(currentX + adjustedChildWidth / 2, transform.position.y, transform.position.z);
            currentX += adjustedChildWidth + distanceBetweenChildren;
        }

    }

    private void UpdateContainerPosition()
    {
        switch(alignment)
        {
            case CardAlignment.Left:
                transform.position = new Vector3(-350, transform.position.y, transform.position.z);
                break;

            case CardAlignment.Right:
                transform.position = new Vector3(350, transform.position.y, transform.position.z);
                break;

            case CardAlignment.Center:
                transform.position = new Vector3(0, transform.position.y, transform.position.z);
                break;
        }
    }

    private void SetCardsAnchor()
    {
        //foreach(CardWrapper child in cards)
        //{
        //    child.SetAnchor(new Vector3(0, 0.5f, 0f), new Vector3(0, 0.5f, 0f));
        //}
    }

    public void RemoveCard(CardWrapper card)
    {
        card.OnCardStartDragEvent -= OnCardDragStart;
        card.OnCardDragEnded -= OnCardDragEnd;
        cards.Remove(card);
        //eventsConfig.OnCardDestroy?.Invoke(new CardDestroy(card));
        //Destroy(card.gameObject);
    }

    private bool IsCursorInPlayArea(RectTransform slot)
    {
        if(slot == null) return false;

        var cursorPosition = Input.mousePosition;
        var playArea = slot;
        var playAreaCorners = new Vector3[4];
        playArea.GetWorldCorners(playAreaCorners);
        return cursorPosition.x > playAreaCorners[0].x &&
               cursorPosition.x < playAreaCorners[2].x &&
               cursorPosition.y > playAreaCorners[0].y &&
               cursorPosition.y < playAreaCorners[2].y;

    }

    private bool IsCursorInPreviewArea()
    {
        if(cardPlayConfig.previewArea == null) return false;

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