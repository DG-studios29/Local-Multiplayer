using UnityEngine;
using UnityEngine.UI;

public class PunchChargeUI : MonoBehaviour
{
    public Slider chargeSlider;
    public Image fullChargeIcon;
    private void Start()
    {

        Show(false);
    }
    public void UpdateCharge(float current, float max)
    {
        chargeSlider.value = current / max;
        fullChargeIcon.enabled = current >= max;
    }

   
    public void Show(bool visible)
    {
        if (chargeSlider != null)
            chargeSlider.gameObject.SetActive(visible);

        if (fullChargeIcon != null)
            fullChargeIcon.gameObject.SetActive(visible);
    }

    public void Follow(Transform target)
    {
        if (target != null)
        {
            transform.position = target.position + Vector3.up * 2f; 
        }
    }
}
