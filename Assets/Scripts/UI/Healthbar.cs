using UnityEngine;

enum HealthState
{
    High,
    Medium,
    Low
}

public class Healthbar : MonoBehaviour
{
    //private Slider healthBar;
    //private GameObject fill;

    //[SerializeField, Range(0,100)] int highHealthPercentage = 66;
    [SerializeField, Range(0, 100)] int mediumHealthPercentage = 66;
    [SerializeField, Range(0, 100)] int lowHealthPercentage = 33;
    private Animator animator;
    HealthState healthState;

    private void Start()
    {
        //healthBar = GetComponent<Slider>();
        //fill = transform.Find("Fill Area").gameObject;

        animator = GetComponent<Animator>();
    }

    public void UpdateHealthBar(float value)
    {
        value = value * 100;
        if (value > mediumHealthPercentage)
        {
            //Green Heart
            healthState = HealthState.High;
        }
        else if (value > lowHealthPercentage)
        {
            //Amber Heart
            healthState = HealthState.Medium;
        }
        else
        {
            //Red Heart
            healthState = HealthState.Low;
        }
        //Debug.Log(value+" "+(int)healthState);
        animator.SetInteger("HealthState", (int)healthState);

        /*healthBar.value = value;
        if (value <= 0)
        {
            fill.SetActive(false);
            return;
        }
        fill.SetActive(true);*/
    }
}
