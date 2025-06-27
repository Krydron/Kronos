using UnityEngine;
using TMPro;
using System.Collections;

public class Timer : MonoBehaviour
{
    TextMeshProUGUI text;
    int min;

    IEnumerator timePass()
    {
        while (true)
        {
            yield return new WaitForSeconds(60);
            min++;
            text.text = "10:"+min;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        
        text.text = "10:21";
        min = 21;
        StartCoroutine(timePass());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
