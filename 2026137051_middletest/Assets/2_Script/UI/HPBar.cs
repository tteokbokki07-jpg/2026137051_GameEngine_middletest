using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Slider hpSlider;

    public float maxHP = 100;
    public float currentHP = 100;

    void Start()
    {
        hpSlider.maxValue = maxHP;
        hpSlider.value = currentHP;
    }
    void Update()
    {
        hpSlider.value = currentHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP < 0f) currentHP = 0f;
        hpSlider.value = currentHP;
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
        hpSlider.value = currentHP;
    }
}