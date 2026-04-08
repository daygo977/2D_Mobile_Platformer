using UnityEngine;
using UnityEngine.EventSystems;

public class LeftRightButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private Controller2D player;
    [SerializeField] [Range(-1f, 1f)] private float direction = 1f;

    public void OnPointerDown(PointerEventData eventData)
    {
        player.SetMoveInput(direction);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        player.SetMoveInput(0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerPress == gameObject)
        {
            player.SetMoveInput(0f);
        }
    }
}
