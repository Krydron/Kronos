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

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerSpeed;
    [SerializeField] float mouseSensitivityX;
    [SerializeField] float mouseSensitivityY;
    private float rotationX;
    private float rotationY;
    Rigidbody rigidbody;
    Vector2 move;
    Vector2 look;

    private bool sneaking;
    [SerializeField] float sneakSpeed;


    //Added by Kry
    [Header("Detection Modifiers")]
    public float normalDetectionRange = 15f;
    public float sneakDetectionRange = 7f;  // Reduced detection range while sneaking
    public float normalDetectionTime = 2f;
    public float sneakDetectionTime = 4f;  // Increased time to be detected

    public float DetectionRange { get; private set; }
    public float DetectionTimeModifier { get; private set; }

    GameObject camera;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rigidbody = GetComponent<Rigidbody>();
        camera = GameObject.Find("Main Camera");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set default detection values
        DetectionRange = normalDetectionRange;
        DetectionTimeModifier = normalDetectionTime;
    }

    public void OnMove(InputValue value)
    {
        if (GetComponent<UIInteractions>().MenuOpen()) { return; }
        move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        if (this.GetComponent<UIInteractions>() == null) { return; }
        if (this.GetComponent<UIInteractions>().MenuOpen()) { return; }
        look.x = value.Get<Vector2>().x * Time.deltaTime * mouseSensitivityX;
        look.y = value.Get<Vector2>().y * Time.deltaTime * mouseSensitivityY;
        rotationX = Mathf.Clamp(rotationX - look.y, -90f, 90f);
        rotationY += look.x;
    }

    public void OnCrouch()
    {
        // Toggle sneaking
        sneaking = !sneaking;
        Debug.Log("Sneaking: " + sneaking);

        if (sneaking)
        {
            DetectionRange = sneakDetectionRange;
            DetectionTimeModifier = sneakDetectionTime;
        }
        else
        {
            DetectionRange = normalDetectionRange;
            DetectionTimeModifier = normalDetectionTime;
        }
    }

    public bool IsSneaking()
    {
        return sneaking;
    }

    private void MovePlayer()
    {
        if (!sneaking)
        {
            rigidbody.linearVelocity = transform.TransformDirection(new Vector3(move.x * playerSpeed * Time.deltaTime, rigidbody.linearVelocity.y, move.y * playerSpeed * Time.deltaTime));
        }
        else
        {
            rigidbody.linearVelocity = transform.TransformDirection(new Vector3(move.x * sneakSpeed * Time.deltaTime, rigidbody.linearVelocity.y, move.y * sneakSpeed * Time.deltaTime));
        }

        rigidbody.rotation = Quaternion.Euler(0, rotationY, 0);
        camera.transform.position = new Vector3(transform.position.x, transform.position.y+1, transform.position.z);
        camera.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    }


    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
}
