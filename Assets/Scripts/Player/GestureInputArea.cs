using UnityEngine;
using UnityEngine.EventSystems;

public class GestureInputArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Controller2D player;

    [Header("Timing")]
    [SerializeField] private float holdThreshold = 0.1f;

    [Header("Swipe")]
    [SerializeField] private float swipeThresholdPixels = 90f;

    private bool isTracking;
    private bool swipeTriggered;
    private bool chargeStarted;
    private Vector2 startPos;
    private Vector2 currentPos;
    private float startTime;

    private void Update()
    {
        if (!isTracking || swipeTriggered)
        {
            return;
        }

        float duration = Time.unscaledTime - startTime;
        float distance = Vector2.Distance(currentPos, startPos);

        if (!chargeStarted && duration >= holdThreshold && distance < swipeThresholdPixels)
        {
            chargeStarted = player.BeginCharge();
        }

        if (chargeStarted)
        {
            player.TickCharge(Time.unscaledDeltaTime);
        }

        if (distance >= swipeThresholdPixels)
        {
            if (chargeStarted)
            {
                player.CancelCharge();
            }

            Vector2 direction = (currentPos - startPos).normalized;
            bool dashed = player.TryDash(direction);

            if (dashed)
            {
                swipeTriggered = true;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTracking = true;
        swipeTriggered = false;
        chargeStarted = false;
        startPos = eventData.position;
        currentPos = eventData.position;
        startTime = Time.unscaledTime;
    }

    public void OnDrag(PointerEventData eventData)
    {
        currentPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isTracking)
        {
            return;
        }

        currentPos = eventData.position;

        float duration = Time.unscaledTime - startTime;
        float distance = Vector2.Distance(currentPos, startPos);

        if (!swipeTriggered)
        {
            if (chargeStarted)
            {
                player.ReleaseCharge();
            }
            else if (duration < holdThreshold && distance < swipeThresholdPixels)
            {
                player.TapJump();
            }
        }

        ResetState();
    }

    private void ResetState()
    {
        isTracking = false;
        swipeTriggered = false;
        chargeStarted = false;
    }
}
