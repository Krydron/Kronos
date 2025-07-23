using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraFov : MonoBehaviour
{
    Camera camera;
    [SerializeField] TextMeshProUGUI text;
    SettimgsTracker gameManager;
    [SerializeField] Slider slider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        gameManager = GameObject.Find("GameManager").GetComponent<SettimgsTracker>();
        camera.fieldOfView = gameManager.cameraFOV;
        slider.value = camera.fieldOfView;
    }

    public void UpdateFOV(float fov)
    {
        camera.fieldOfView = fov;
        gameManager.cameraFOV = fov;
        text.text = fov.ToString();
    }
}
