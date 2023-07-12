using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider foodSlider;
    public Slider waterSlider;

    public void FoodSliderUpdate(float value)
    {
        foodSlider.value = value;
    }
    public void WaterSliderUpdate(float value)
    {
        waterSlider.value = value;
    }
}
