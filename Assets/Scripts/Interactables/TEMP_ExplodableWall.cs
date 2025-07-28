using System.Collections;
using UnityEngine;

public class TEMP_ExplodableWall : Interactable
{
    [SerializeField] GameObject bomb;
    [SerializeField] Item bombItem;
    [SerializeField] float time;
    [SerializeField] Inventory inventory;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    IEnumerator Explode()
    {
        if (!inventory.HasItem(bombItem)) { yield break; }
        bomb.SetActive(true);
        inventory.RemoveItem(bombItem);
        yield return new WaitForSeconds(time);
        //Destroy(gameObject);
        gameObject.SetActive(false);
        GetComponent<FlashbackTrigger>().Trigger();
    }

    public override bool Interact()
    {
        StartCoroutine(Explode());
        return true;
    }
}
