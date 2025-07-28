using UnityEngine;
using UnityEngine.SceneManagement;

public class FlashbackQuit : Interactable
{
    GameObject player;
    GameObject gameManager;
    [SerializeField] float distance;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = GameObject.Find("GameManager");
    }

    public override bool Interact()
    {
        //if in range
        player = GameObject.FindGameObjectWithTag("Player");
        if (Vector3.Distance(transform.position, player.transform.position) <= distance)
        {
            //End flashback
            gameManager.GetComponent<SceneLoader>().EndFlashback();
            return true;
        }
        return false;
    }
}
