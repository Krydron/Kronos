using UnityEngine;

public enum Flashback
{
    Office,
    Birthday,
    Warehouse,
    Vault,
    Length
}
public class FlashbackTrigger : MonoBehaviour
{
    [SerializeField] Flashback flashback;
    private GameObject gameManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
    }

    public void Trigger()
    {
        gameManager.GetComponent<SceneLoader>().FlashbackTrigger(flashback);
    }
}
