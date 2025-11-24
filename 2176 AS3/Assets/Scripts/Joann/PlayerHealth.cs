using Unity.Mathematics;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    public UIManager uiManager; // a reference to the ui manager to update the health display

    void Start()
    {
        // set player's health to full at the beginning of the game
        currentHealth = maxHealth;
        // tell the ui manager to set the starting hearts correctly
        if (uiManager != null)
        {
            uiManager.UpdateHealthUI(currentHealth);
        }
    }

    // this public method can be called by other scripts (like the enemy ai) to damage the player
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // ensure health doesn't drop below zero
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log("Player took damage. Current Health: " + currentHealth);

        // play hurt sfx using the audiomanager
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound("Hurt");
        }

        // tell the ui manager to update the hearts display
        if (uiManager != null)
        {
            uiManager.UpdateHealthUI(currentHealth);
        }

        // check if the player has run out of health
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player has been defeated!");

        // disable the player's movement script to stop them from moving after death
        GetComponent<PlayerMovement>().enabled = false;

        // tell the ui manager to show the game over screen
        if (uiManager != null)
        {
            uiManager.ShowGameOverScreen();
        }
    }
}