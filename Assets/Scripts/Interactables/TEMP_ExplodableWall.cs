using System.Collections;
using UnityEngine;

public class TEMP_ExplodableWall : Interactable
{
    [SerializeField] GameObject bomb;
    [SerializeField] float time;

    IEnumerator Explode()
    {
        bomb.SetActive(true);
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    public override bool Interact()
    {
        StartCoroutine(Explode());
        return true;
    }
}
