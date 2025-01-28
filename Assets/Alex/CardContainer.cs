using System.Collections;
using System.Collections.Generic;
using System.Linq;
using config;
using DefaultNamespace;
using events;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CardContainer : MonoBehaviour
{
    [Header("Alignment")]
    [SerializeField]
    private CardAlignment alignment = CardAlignment.Center;

    [SerializeField]
    private bool allowCardRepositioning = true;

    [SerializeField]
    [Range(-10f, 10f)]
    private float cardSpacing = 2f;

    [SerializeField]
    [Range(-20f, 20f)]
    private float cardScreenYPosition = 0;

    [SerializeField]
    private bool flipCards = false;

    [Header("Rotation")]
    [SerializeField]
    [Range(-90f, 90f)]
    private float maxCardRotation;

    [SerializeField]
    [Range(-1f, 5f)]
    private float maxHeightDisplacement;

    [SerializeField]
    private AnimationSpeedConfig animationSpeedConfig;

    [SerializeField]
    private CardPlayConfig cardPlayConfig;

    [Header("Events")]
    [SerializeField]
    private EventsConfig eventsConfig;

    [Header("Card Prefabs")]
    [SerializeField]
    private List<GameObject> cardPrefabs;

    [SerializeField]
    private Transform cardSpawnPoint;

    private List<CardWrapper> cards = new();

    private CardWrapper currentDraggedCard;

    [SerializeField]
    private GameObject colliderPlane;

    private void Start()
    {
        InitCards();
        UpdateContainerPosition();
    }

    private void InitCards()
    {
        SetUpCards();
    }

    private void OnValidate()
    {
        //temporary, change whenever there is a main menu
        UpdateContainerPosition();
    }

    private void SetCardsRotation()
    {
        for (var i = 0; i < cards.Count; i++)
        {
            cards[i].targetRotation = GetCardRotation(i);
            cards[i].targetVerticalDisplacement = GetCardVerticalDisplacement(i);
        }
    }

    private float GetCardVerticalDisplacement(int index)
    {
        if (cards.Count < 3) return 0;
        // Associate a vertical displacement based on the index in the cards list
        // so that the center card is at max displacement while the edges are at 0 displacement
        return maxHeightDisplacement *
               (1 - Mathf.Pow(index - (cards.Count - 1) / 2f, 2) / Mathf.Pow((cards.Count - 1) / 2f, 2));
    }

    private float GetCardRotation(int index)
    {
        if (cards.Count < 3) return 0;
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
        foreach (Transform card in transform)
        {
            // get an array of card prefabs
            // spawn them dynamically here

            var wrapper = card.GetComponent<CardWrapper>();
            if (wrapper == null)
            {
                wrapper = card.gameObject.AddComponent<CardWrapper>();
            }

            cards.Add(wrapper);

            // Pass child card any extra config it should be aware of
            wrapper.Initialize(animationSpeedConfig, eventsConfig, currentDraggedCard);
            wrapper.OnCardStartDragStarted += OnCardDragStart;
            wrapper.OnCardDragEnded += OnCardDragEnd;
        }
    }

    private void OnCardDragStart(CardWrapper card)
    {
        currentDraggedCard = card;
        colliderPlane.SetActive(true);
    }

    public void OnCardDragEnd(CardWrapper card)
    {
        colliderPlane.SetActive(false);

        if (!cards.Contains(card))
        {
            cards.Add(card);
            card.transform.SetParent(transform, true);
            eventsConfig.OnCardPlayed += card.OnCardPlayed;
            //StartCoroutine(SmoothMoveToHand(card.transform, transform));
        }

        if (currentDraggedCard != card)
        {
            return;
        }

        int slotLayerMask = LayerMask.GetMask("Slot");
        Vector3 mousePosition = Input.mousePosition;
        Ray destinationRay = Camera.main!.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(destinationRay, out RaycastHit hit, float.MaxValue, slotLayerMask))
        {
            int slotChildCount = hit.transform.childCount;
            if (slotChildCount < 1)
            {
                //Relocate RaiseOnCardPlayer to a button so that the effect gets triggered with the End Turn button.
                eventsConfig?.RaiseOnCardPlayed(new CardPlayed(currentDraggedCard));
                currentDraggedCard.transform.SetParent(hit.collider.gameObject.transform, true);
                StartCoroutine(SmoothMoveToSlot(currentDraggedCard.transform, hit.collider.gameObject.transform));
            }
        }

        currentDraggedCard = null;
    }

    private IEnumerator SmoothMoveToSlot(Transform cardTransform, Transform slotTransform)
    {
        float duration = 0.3f;
        float elapsedTime = 0f;

        Vector3 startPos = cardTransform.localPosition;
        Quaternion startRot = cardTransform.localRotation;
        Vector3 startScale = cardTransform.localScale;

        Vector3 endPos = new Vector3(0, 0, -0.1f);
        Quaternion endRot = Quaternion.identity;
        Vector3 endScale = Vector3.one;

        while (elapsedTime <= duration)
        {
            float t = elapsedTime / duration;
            cardTransform.localPosition = Vector3.Lerp(startPos, endPos, t);
            cardTransform.localRotation = Quaternion.Lerp(startRot, endRot, t);
            //cardTransform.localScale = Vector3.Lerp(startScale, endScale, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cardTransform.localPosition = endPos;
        cardTransform.localRotation = endRot;
        //cardTransform.localScale = endScale;
    }

    private IEnumerator SmoothMoveToHand(Transform cardTransform, Transform slotTransform)
    {
        float duration = 0.3f;
        float elapsedTime = 0f;

        Vector3 startPos = cardTransform.localPosition;
        Quaternion startRot = cardTransform.localRotation;
        Vector3 startScale = cardTransform.localScale;

        Vector3 endPos = new Vector3(0, 0, 3);
        Quaternion endRot = Quaternion.Euler(40, 0f, 0f);
        Vector3 endScale = Vector3.one;

        while (elapsedTime <= duration)
        {
            float t = elapsedTime / duration;
            cardTransform.localPosition = Vector3.Lerp(startPos, endPos, t);
            cardTransform.localRotation = Quaternion.Lerp(startRot, endRot, t);
            //cardTransform.localScale = Vector3.Lerp(startScale, endScale, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardTransform.localPosition = endPos;
        cardTransform.localRotation = endRot;
        //cardTransform.localScale = endScale;

        CardWrapper card = cardTransform.GetComponent<CardWrapper>();
        if (card != null)
        {
            card.transform.SetParent(transform, true);
            eventsConfig.OnCardPlayed += card.OnCardPlayed;
        }
    }

    private void UpdateCards()
    {
        if (transform.childCount != cards.Count)
        {
            InitCards();
        }

        if (cards.Count == 0)
        {
            return;
        }

        SetCardsPosition();
        SetCardsRotation();
        SetCardsUILayers();
        UpdateCardOrder();
    }

    private void SetCardsUILayers()
    {

        for (var i = 0; i < cards.Count; i++)
        {
            if (flipCards == true)
            {
                cards[i].Material.renderQueue = (int)RenderQueue.GeometryLast + 100 + i;

            }
            else
            {
                cards[i].Material.renderQueue = (int)RenderQueue.GeometryLast + 100 - i;
            }
        }
    }



    private void UpdateCardOrder()
    {
        //Allows cards to be rearranged in the container.
        if (!allowCardRepositioning || currentDraggedCard == null) return;

        var originalCardIdx = cards.IndexOf(currentDraggedCard);
        if (originalCardIdx == -1) return;

        // Counts how many cards have their x position less than the dragged card.
        var newCardIdx = cards.Count(card => currentDraggedCard.transform.position.x < card.transform.position.x);
        //Finds the current position of the dragged card in the list.
        if (newCardIdx != originalCardIdx)
        {
            cards.RemoveAt(originalCardIdx);
            if (newCardIdx > originalCardIdx && newCardIdx < cards.Count - 1)
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
        var cardsTotalWidth = cards.Sum(card => cardSpacing * card.transform.lossyScale.x);
        DistributeChildrenToFitContainer(cardsTotalWidth);
    }

    private void DistributeChildrenToFitContainer(float totalCardWidth)
    {
        // Get the width of the container
        var scaledWidth = transform.lossyScale.x;

        //if there is only 1 card we prevent division by zero on the line below.
        if (cards.Count <= 1) return;

        // Get the distance between each child
        var distanceBetweenChildren = (scaledWidth - totalCardWidth) / (cards.Count - 1);
        // Set all children's positions to be evenly spaced out
        var currentX = transform.position.x - scaledWidth / 2f;

        const float step = 0.01f;
        float totalOffset = step * cards.Count;
        float currentY = totalOffset / 2f;
        foreach (CardWrapper child in cards)
        {
            var adjustedChildWidth = cardSpacing * child.transform.lossyScale.x;
            child.targetPosition = new Vector3(currentX + adjustedChildWidth / 2, transform.position.y + currentY, cardScreenYPosition);
            currentX += adjustedChildWidth + distanceBetweenChildren;
            currentY -= step;
        }
    }

    private void UpdateContainerPosition()
    {
        switch (alignment)
        {
            case CardAlignment.Left:
                transform.position = new Vector3(-4, transform.position.y, transform.position.z);
                break;

            case CardAlignment.Right:
                transform.position = new Vector3(4, transform.position.y, transform.position.z);
                break;

            case CardAlignment.Center:
                transform.position = new Vector3(0, transform.position.y, transform.position.z);
                break;
        }
    }

    public void RemoveCard(CardWrapper card)
    {
        card.OnCardStartDragStarted -= OnCardDragStart;
        card.OnCardDragEnded -= OnCardDragEnd;
        cards.Remove(card);
    }

    public void DrawCard()
    {
        GameObject randomCardPrefab = cardPrefabs[Random.Range(0, cardPrefabs.Count)];
        GameObject newCard = Instantiate(randomCardPrefab, cardSpawnPoint.position, Quaternion.Euler(75, 0f, 0f), transform);
        CardWrapper wrapper = newCard.AddComponent<CardWrapper>();

        wrapper.Initialize(animationSpeedConfig, eventsConfig, currentDraggedCard);
        wrapper.OnCardStartDragStarted += OnCardDragStart;
        wrapper.OnCardDragEnded += OnCardDragEnd;
        cards.Add(wrapper);
    }

    private bool IsCursorInPlayArea(RectTransform slot)
    {
        if (slot == null) return false;

        var cursorPosition = Input.mousePosition;
        var playArea = slot;
        var playAreaCorners = new Vector3[4];
        playArea.GetWorldCorners(playAreaCorners);
        return cursorPosition.x > playAreaCorners[0].x &&
               cursorPosition.x < playAreaCorners[2].x &&
               cursorPosition.y > playAreaCorners[0].y &&
               cursorPosition.y < playAreaCorners[2].y;

    }
}