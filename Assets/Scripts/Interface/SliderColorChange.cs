using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderColorChange : MonoBehaviour
{
    private Image sliderHandle;
    private Slider slider;

    void Start()
    {
        slider = transform.Find("Slider").GetComponent<Slider>();
        sliderHandle = transform.Find("Slider").Find("HandleArea").Find("Handle").GetComponent<Image>();
    }

    public void OnSliderClicked()
    {
        if (slider.value == 0)
        {
            slider.value = 1;
            sliderHandle.color = new Color(0, 0.15f, 0);
            UIController.Instance.ChangeAutosell(true);
        }
        else
        {
            slider.value = 0;
            sliderHandle.color = new Color(0.16f, 0, 0);
            UIController.Instance.ChangeAutosell(false);
        }
    }

    public void SetSlider(bool enabled)
    {
        if (slider == null)
            Start();

        if (enabled)
        {
            slider.value = 1;
            sliderHandle.color = new Color(0, 0.15f, 0);
        }
        else
        {
            slider.value = 0;
            sliderHandle.color = new Color(0.16f, 0, 0);
        }
    }
}
