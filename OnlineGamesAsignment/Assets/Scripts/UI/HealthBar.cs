using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBarImage;

    public int health = 0;
    public int maxHealth = 0;

    private void Start()
    {
        Debug.Log(name + transform.position);
    }

    public void SetHealthBar(int maxHealth)
    {
        health = maxHealth;
        this.maxHealth = maxHealth;
        RestoreHealth();
    }

    private void UpdateHealth()
    {
        healthBarImage.fillAmount = health / (float)maxHealth;
    }

    public void RestoreHealth()
    {
        health = maxHealth;
        UpdateHealth();
    }

    public bool Damage(int amount)
    {
        health -= amount;
        if (health <= 0) return true;

        UpdateHealth();
        return false;
    }

}
