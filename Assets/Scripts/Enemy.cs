using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;

    public void damage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log("Enemy takes " + damageAmount + " damage. Health now: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Implement enemy death logic here
        Debug.Log("Enemy died.");
        Destroy(gameObject);
    }
}
