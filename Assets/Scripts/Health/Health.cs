using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int startingHealth;
    private int currentHealth;

    public int GetStartingHealth => startingHealth;
    public int SetStartingHealth
    {
        set 
        { 
            startingHealth = value;
            currentHealth = value;
        }
    }
}
