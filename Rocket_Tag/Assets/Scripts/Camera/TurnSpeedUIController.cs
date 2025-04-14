using UnityEngine;
using UnityEngine.UI;

public class TurnSpeedUIController : MonoBehaviour
{
    [SerializeField] private Slider turnSpeedSlider;
    [SerializeField] private TurnSpeedSetting turnSpeedSetting;

    private void Start()
    {
        if(turnSpeedSlider != null)
        {
            turnSpeedSlider.value = turnSpeedSetting.turnSpeed;
            turnSpeedSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    void OnSliderValueChanged(float value)
    {
        turnSpeedSetting.turnSpeed = value;
    }
}
