using System.Collections;
using UnityEngine;

[RequireComponent(typeof(IDie))]
public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100; // for customizablility
    [SerializeField] private PlayerData data;
    public float health { get; private set; } // public to get, not public to set

    private IDie _dieScript;
    private bool _isEnemy = true;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        health = maxHealth;
        _dieScript = GetComponent<IDie>();
        _meshRenderer = GetComponent<MeshRenderer>();
        
        if (gameObject.name == "Car") {
          maxHealth =  data.maxHealth;
          _isEnemy = false;
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        health -= amount;

        StartCoroutine(DamageEffect());

        if (health < 0)
        {
            health = 0;
            _dieScript.OnDie();
        }
    }

    private IEnumerator DamageEffect()
    {
        Color originalColor = _meshRenderer.material.color;
        _meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(.1f);
        _meshRenderer.material.color = originalColor;
    }
}
