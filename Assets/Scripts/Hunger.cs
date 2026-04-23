using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Hunger : MonoBehaviour
{
    public Slider HungerSlider;
    public float HungerDrainRate = 0.01f;

    public Transform Respawn;
    public bool isDead;
    public bool beginHunger;

    void Start()
    {
        beginHunger = true;
        HungerSlider.value = HungerSlider.maxValue;
    }

    void Update()
    {
        if (!beginHunger) return;

        HungerSlider.value -= HungerDrainRate * Time.deltaTime;
        HungerSlider.value = Mathf.Clamp(HungerSlider.value, HungerSlider.minValue, HungerSlider.maxValue);

        if (HungerSlider.value <= 0 && !isDead)
        {
            Die();
        }
    }

    public void RestoreHunger(float amount)
    {
        HungerSlider.value += amount;
        HungerSlider.value = Mathf.Clamp(HungerSlider.value, HungerSlider.minValue, HungerSlider.maxValue);
    }

    void Die()
    {
        transform.position = Respawn.position;
        HungerSlider.value = HungerSlider.maxValue;
        isDead = true;

        StartCoroutine(RestartHunger());
    }

    IEnumerator RestartHunger()
    {
        beginHunger = false;

        yield return new WaitForSeconds(2f);

        beginHunger = true;
        isDead = false;
    }
}