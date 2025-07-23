/**************************************************************************************************************
* <Name> Class
*
* The header file for the <Name> class.
* 
* This class 
* 
*
* Created by: <Owen Clifton> 
* Date: <need to add>
*
***************************************************************************************************************/

using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health;
    private int fullHealth;
    private GameObject healthBar;
    private Healthbar healthBarScript;
    private bool dead;

    private void Start()
    {
        dead = false;
        fullHealth = (int)health;
        healthBar = GameObject.Find("HealthBar");
        healthBarScript = healthBar?.GetComponent<Healthbar>();
    }

    public void Health(float value)
    {
        health = Mathf.Clamp(value, 0, fullHealth);
        UpdateHealthBar();
    }

    public float Health()
    {
        return health;
    }

    public void IncrementHealth(float value)
    {
        health += value;
        health = Mathf.Clamp(health, 0, fullHealth);
        UpdateHealthBar();
    }

    IEnumerator Death()
    {
        GameObject.FindGameObjectWithTag("Eyes").GetComponent<Eyes>().ShutEyes();
        yield return new WaitForSeconds(2);
        GameObject.Find("GameManager").GetComponent<LoopTracker>().IncrementLoop();
        GameObject.Find("GameManager").GetComponent<SceneLoader>().ReloadScene();
    }

    public void DecrementHealth(float value)
    {
        health -= value;
        health = Mathf.Clamp(health, 0, fullHealth);
        if (health == 0 && !dead)
        {
            dead=true;
            StartCoroutine(Death());
        }
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        //if (healthBarScript == null) { return; }
        //Debug.Log("Health: "+ health / fullHealth);
        healthBarScript.UpdateHealthBar(health / fullHealth);
    }
}
