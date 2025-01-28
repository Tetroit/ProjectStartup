using System.Collections.Generic;
using config;
using events;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace demo
{
    /**
     * Offers a preview of a card when hovering over it by cloning the original card and placing it on top of the original.
     * Allows setting the global preview position and scale.
     */
    public class ClonePreviewManager : MonoBehaviour, CardPreviewManager
    {
        [SerializeField]
        private float previewHeightOffset = 1f;
        [SerializeField]
        private float previewScaleMultiplier = 2.5f;
        [SerializeField]
        private float previewForwardOffset = 0.5f;

        [SerializeField]
        private RectTransform previewArea;

        private int xDeg = 40;

        private Dictionary<CardWrapper, GameObject> previews = new();

        private CardWrapper currentCard;

        [SerializeField]
        private EventsConfig eventsConfig;

        private void Awake()
        {
            eventsConfig.OnCardClick += OnCardClick;
            eventsConfig.OnCardRelease += OnCardRelease;
        }

        private void OnDestroy()
        {
            eventsConfig.OnCardClick -= OnCardClick;
            eventsConfig.OnCardRelease -= OnCardRelease;
        }

        private void Update()
        {
            if (currentCard != null)
            {
                bool shouldEnable = IsCursorInPreviewArea();
                GameObject preview = previews[currentCard];
                preview.SetActive(shouldEnable);
                preview.transform.position = currentCard.transform.position + new Vector3(0, previewHeightOffset, previewForwardOffset);
            }
        }

        public void OnCardClick(CardClick cardClick)
        {
            OnCardPreviewStarted(cardClick.card);
        }

        public void OnCardRelease(CardRelease cardRelease)
        {
            OnCardPreviewEnded(cardRelease.card);
        }

        public void OnCardPreviewStarted(CardWrapper card)
        {
            if(!previews.ContainsKey(card))
            {
                CreateCloneForCard(card);
            }

            currentCard = card;
            GameObject currentPreview = previews[currentCard];
            currentPreview.SetActive(true);

            Vector3 newPosition = card.transform.position + new Vector3(0, previewHeightOffset, previewForwardOffset);
            currentPreview.transform.position = newPosition;
            currentPreview.transform.localScale = card.transform.localScale * previewScaleMultiplier;
        }

        private void CreateCloneForCard(CardWrapper card)
        {
            GameObject clone = Instantiate(card.gameObject, transform);
            clone.transform.position = card.transform.position;
            clone.transform.localScale = card.transform.localScale * previewScaleMultiplier;
            clone.transform.rotation = Quaternion.Euler(xDeg, card.transform.localRotation.y, 0f);

            Renderer renderer = clone.GetComponentInChildren<Renderer>();
            renderer.material.renderQueue = (int)RenderQueue.GeometryLast + 200;

            StripCloneComponents(clone);
            previews.Add(card, clone);
        }

        private static void StripCloneComponents(GameObject clone)
        {
            var cloneWrapper = clone.GetComponent<CardWrapper>();
            if(cloneWrapper != null)
            {
                Destroy(cloneWrapper);
            }

            var collider = clone.GetComponent<Collider>();
            if(collider != null)
            {
                Destroy(collider);
            }
        }

        public void OnCardPreviewEnded(CardWrapper card)
        {
            if(previews.ContainsKey(card))
            {
                previews[card].SetActive(false);
            }

            currentCard = null;
        }

        private bool IsCursorInPreviewArea()
        {
            if (previewArea == null)
            {
                Debug.LogError($"{nameof(previewArea)} is not assigned");
                return false;
            }

            var cursorPosition = Input.mousePosition;
            var playAreaCorners = new Vector3[4];
            previewArea.GetWorldCorners(playAreaCorners);
            return cursorPosition.x > playAreaCorners[0].x &&
                   cursorPosition.x < playAreaCorners[2].x &&
                   cursorPosition.y > playAreaCorners[0].y &&
                   cursorPosition.y < playAreaCorners[2].y;
        }
    }
}
