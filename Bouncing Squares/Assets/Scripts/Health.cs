using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int health = 5;
    [SerializeField] private Slider slider;

    private void Awake()
    {
        slider.minValue = 0;
        slider.maxValue = health;
        slider.value = health;
    }

    private void Update()
    {
        slider.value = health;
    }

    public void SetValue(int value) => health = value;

    public void SetMaxValue(int value)
    {
        health = value;
        slider.maxValue = value;
    }

    public int GetValue() => health;
    public int GetMaxValue() => (int)slider.maxValue;
}
