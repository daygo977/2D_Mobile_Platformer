using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] private int maxLives = 3;

    [Header("UI")]
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI livesText;

    private int currentLives;
    private bool isDead;

    //Allows other scripts to check player lives
    public int CurrentLives => currentLives;

    private void Awake()
    {
    Time.timeScale = 1f;
    currentLives = maxLives;
    UpdateLivesUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        //Reduce lives by one, and clamp to 0
        currentLives -= damage;
        currentLives = Mathf.Max(currentLives, 0);
        UpdateLivesUI();

        Debug.Log("Player hit! Lives left: " + currentLives);

        //If all lives lost, die
        if (currentLives <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player died!");

        //Disable player control
        Controller2D controller = GetComponent<Controller2D>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        //Stop movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        //Hide gameplay controls
        if (gameplayUI != null)
        {
            gameplayUI.SetActive(false);
        }

        //Show game over screen
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
        livesText.text = "Lives: " + currentLives;
        }
    }
}