using UnityEngine;

public class FinishPlatform : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject gameplayUI;

    private bool levelCompleted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (levelCompleted)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            levelCompleted = true;
            Debug.Log("Level Complete!");

            //Stop player movement
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            //Disable player control
            Controller2D controller = other.GetComponent<Controller2D>();
            if (controller != null)
            {
                controller.enabled = false;
            }

            //Disable gameplay UI
            if (gameplayUI != null)
            {
                gameplayUI.SetActive(false);
            }

            //Show level complete screen
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(true);
            }

            //Freeze level
            Time.timeScale = 0f;
        }
    }
}