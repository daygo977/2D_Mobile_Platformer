using UnityEngine;
using UnityEngine.EventSystems;

public class LeftRightButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private Controller2D player;
    [SerializeField] [Range(-1f, 1f)] private float direction = 1f;

    //Start moving when the button is pressed
    public void OnPointerDown(PointerEventData eventData)
    {
        player.SetMoveInput(direction);
    }

    //Stop moving when button is released
    public void OnPointerUp(PointerEventData eventData)
    {
        player.SetMoveInput(0f);
    }

    //Stop moving if finger slides off button
    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerPress == gameObject)
        {
            player.SetMoveInput(0f);
        }
    }
}
