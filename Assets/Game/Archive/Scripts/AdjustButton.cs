using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdjustButton : MonoBehaviour
{
    private GameObject slider;
    private int oríginalMaxValue;
    private int originalMinValue;
    private int interval;
    
    // Start is called before the first frame update
    void Start()
    {
        slider = transform.parent.GetChild(1).gameObject;
        oríginalMaxValue = (int) slider.GetComponent<Slider>().maxValue;
        originalMinValue = (int) slider.GetComponent<Slider>().minValue;
        interval = oríginalMaxValue - originalMinValue;
    }
    
    public void OnButtonIncrease()
    {
        slider.GetComponent<Slider>().minValue += interval;
        slider.GetComponent<Slider>().maxValue += interval;
    }
    
    public void OnButtonDecrease()
    {
        int maxValue = (int) slider.GetComponent<Slider>().maxValue;
        int minValue = (int) slider.GetComponent<Slider>().minValue;
        
        slider.GetComponent<Slider>().minValue = minValue - interval > originalMinValue ? minValue - interval : originalMinValue ;

        slider.GetComponent<Slider>().maxValue =
            maxValue - interval > oríginalMaxValue ? maxValue - interval : oríginalMaxValue;
    }

    public void Resetbutton()
    {
        slider.GetComponent<Slider>().minValue = originalMinValue;
        slider.GetComponent<Slider>().maxValue = oríginalMaxValue;
    }
    
}
