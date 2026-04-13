using UnityEngine;
using UnityEngine.UI;

public class Hunger : MonoBehaviour
{
    public Slider HungerSlider;
    public float HungerDrainRate = 5f;

    public Transform Respawn;
    public bool isDead;
    public bool beginHunger;

    void Update()
    {
        if (beginHunger)
        {
            HungerSlider.value -= HungerDrainRate * Time.deltaTime;
        }

        if (HungerSlider.value <= 0)
        {
            Die();
        }
    }

    public void Eat(float amount)
    {
        HungerSlider.value += amount;
        HungerSlider.value = Mathf.Clamp(HungerSlider.value, 0, HungerSlider.maxValue);
    }

    void Die()
    {
        transform.position = Respawn.position;
        beginHunger = false;
        HungerSlider.value = HungerSlider.maxValue;
        isDead = true;
    }
}