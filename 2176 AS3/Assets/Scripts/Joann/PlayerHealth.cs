using Unity.Mathematics;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    public UIManager uiManager;

    void Start()
    {
        currentHealth = maxHealth;
        // Tell the UI to set the starting hearts correctly
        if (uiManager != null)
        {
            uiManager.UpdateHealthUI(currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log("Player took damage. Current Health: " + currentHealth);

        // play hurt SFX
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound("Hurt");
        }

        // Tell the UI to update the hearts display
        if (uiManager != null)
        {
            uiManager.UpdateHealthUI(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player has been defeated!");

        GetComponent<PlayerMovement>().enabled = false;

        if (uiManager != null)
        {
            uiManager.ShowGameOverScreen();
        }
    }
}
