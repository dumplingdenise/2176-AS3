using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public UIManager uiManager;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player has been defeated!");

        // Disable player movement
        GetComponent<PlayerMovement>().enabled = false;

        // Call the UIManager to show the game over screen
        if (uiManager != null)
        {
            uiManager.ShowGameOverScreen();
        }
    }
}