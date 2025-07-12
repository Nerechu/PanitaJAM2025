using UnityEngine;
using UnityEngine.UI;

public class DisplaySlider : MonoBehaviour
{
    public Slider slider;

    public void UpdateUI(int planted, int total)
    {
        slider.maxValue = total;
        slider.value = planted;
    }
}
