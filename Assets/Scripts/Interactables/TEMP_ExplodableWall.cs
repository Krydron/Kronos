using System.Collections;
using UnityEngine;

public class TEMP_ExplodableWall : Interactable
{
    [SerializeField] GameObject bomb;
    [SerializeField] Item bombItem;
    [SerializeField] float time;
    Inventory inventory;
    [SerializeField] GameObject explosion;
    [SerializeField] Transform explosionLocation;

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
        GetComponent<FlashbackTrigger>()?.Trigger();
        Instantiate(explosion,explosionLocation.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    public override bool Interact()
    {
        StartCoroutine(Explode());
        return true;
    }
}
