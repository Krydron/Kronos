using UnityEngine;

public class FlashbackQuit : MonoBehaviour
{
    GameObject player;
    GameObject gameManager;
    [SerializeField] float distance;

    private void OnLevelWasLoaded(int level)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = GameObject.Find("GameManager");
    }

    public void Exit()
    {
        //if in range
        player = GameObject.FindGameObjectWithTag("Player");
        if (Vector3.Distance(transform.position, player.transform.position) <= distance)
        {
            //End flashback
            gameManager.GetComponent<SceneLoader>().EndFlashback(0);
        }


    }
}
