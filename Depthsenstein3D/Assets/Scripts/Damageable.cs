using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public float MaxHealth = 10.0f;
    public float CurrentHealth = 10.0f;
    public UnityEvent DieEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hurt(float damage) {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0) {
            DieEvent.Invoke();
        }
    }
}
