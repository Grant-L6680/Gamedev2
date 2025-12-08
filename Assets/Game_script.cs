using UnityEngine;

public class GardenHealth : MonoBehaviour
{
    [Header("Plant Health Settings")]
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;

    private Renderer rend;
    private Color healthyColor = Color.green;
    private Color damagedColor = Color.yellow;
    private Color deadColor = new Color(0.6f, 0.3f, 0.1f); // RGB for a brown-like color

    void Start()
    {
        currentHealth = maxHealth;
        rend = GetComponent<Renderer>();
        UpdateColor();
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        UpdateColor();

        if (currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} has been eaten!");
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (rend == null) return;

        if (currentHealth <= 0)
            rend.material.color = deadColor;
        else if (currentHealth < maxHealth * 0.5f)
            rend.material.color = damagedColor;
        else
            rend.material.color = healthyColor;
    }
}
