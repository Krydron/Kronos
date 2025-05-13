using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private Slider healthBar;
    private GameObject fill;

    private void Start()
    {
        healthBar = GetComponent<Slider>();
        fill = transform.Find("Fill Area").gameObject;

    }

    public void UpdateHealthBar(float value)
    {
        healthBar.value = value;
        if (value <= 0)
        {
            fill.SetActive(false);
            return;
        }
        fill.SetActive(true);
    }
}
