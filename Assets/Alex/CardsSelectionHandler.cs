using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardsSelectionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private float vecticalMoveAmount = 30f;
    [SerializeField] private float moveTime = 0.1f;
    [Range(0f, 2f), SerializeField] private float scaleAmount = 1.1f;

    private Vector3 startPosition;
    private Vector3 startScale;

    private void Start()
    {
        startPosition = transform.position;
        startScale = transform.localScale;
    }

    private IEnumerator MoveCard(bool startAnimation)
    {
        Vector3 endPosition;
        Vector3 endScale;

        float elapsedTime = 0f;

        while(elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime;

            if(startAnimation)
            {
                endPosition = startPosition + new Vector3(0f, vecticalMoveAmount, 0);
                endScale = startScale * scaleAmount;
            }
            else
            {
                endPosition = startPosition;
                endScale = startScale;
            }

            Vector3 lerpedPos = Vector3.Lerp(transform.position, endPosition, (elapsedTime / moveTime));
            Vector3 lerpedScale = Vector3.Lerp(transform.localScale, endScale, (elapsedTime / moveTime));

            transform.position = lerpedPos;
            transform.localScale = lerpedScale;

            yield return null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.selectedObject = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventData.selectedObject = null;
    }

    public void OnSelect(BaseEventData eventData)
    {
        StartCoroutine(MoveCard(true));
    }

    public void OnDeselect(BaseEventData eventData)
    {
        StartCoroutine(MoveCard(false));
    }
}
