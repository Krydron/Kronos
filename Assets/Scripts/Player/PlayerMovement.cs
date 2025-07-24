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
using FMODUnity;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] float playerSpeed;
    [SerializeField] float mouseSensitivityX;
    [SerializeField] float mouseSensitivityY;
    private float rotationX;
    private float rotationY;
    Rigidbody rigidbody;
    Vector2 move;
    Vector2 look;
    private Vector3 moveDirection;

    //Camera to update position and manage rotation
    GameObject camera;

    [Header("Sneak Variables")]
    private bool sneaking;
    [SerializeField] float sneakDivider;


    //Added by Kry
    //[Header("Detection Modifiers")]
    //public float normalDetectionRange = 15f;
    //public float sneakDetectionRange = 7f;  // Reduced detection range while sneaking
    //public float normalDetectionTime = 2f;
    //public float sneakDetectionTime = 4f;  // Increased time to be detected

    //public float DetectionRange { get; private set; }
    //public float DetectionTimeModifier { get; private set; }

    [Header("Slope Movement")]
    //For slope code
    RaycastHit hit;
    float angle;
    [SerializeField] float maxAngle;

    [Header("Gun UI")]
    [SerializeField] Animator animator;

    [Header("Crowbar UI")]
    [SerializeField] Animator animatorCrowbar;

    [Header("Sounds")]
    [SerializeField] GameObject steppingSound;
    [SerializeField] StudioEventEmitter walkingSound;
    [SerializeField] StudioEventEmitter crouchingSound;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rigidbody = GetComponent<Rigidbody>();
        camera = GameObject.Find("Main Camera");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set default detection values
        //DetectionRange = normalDetectionRange;
        //DetectionTimeModifier = normalDetectionTime;
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
        if (sneaking) 
        {
            transform.localScale = new Vector3(1, 0.5f, 1);
            rigidbody.AddForce(Vector3.down*80);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        Debug.Log("Sneaking: " + sneaking);

        /*if (sneaking)
        {
            DetectionRange = sneakDetectionRange;
            DetectionTimeModifier = sneakDetectionTime;
        }
        else
        {
            DetectionRange = normalDetectionRange;
            DetectionTimeModifier = normalDetectionTime;
        }*/
    }

    public bool IsSneaking()
    {
        return sneaking;
    }

    private void MovePlayer()
    {
        /*if (!sneaking)
        {
            rigidbody.linearVelocity = transform.TransformDirection(new Vector3(move.x * playerSpeed * Time.deltaTime, rigidbody.linearVelocity.y, move.y * playerSpeed * Time.deltaTime));
        }
        else
        {
            rigidbody.linearVelocity = transform.TransformDirection(new Vector3(move.x * sneakSpeed * Time.deltaTime, rigidbody.linearVelocity.y, move.y * sneakSpeed * Time.deltaTime));
        }*/

        moveDirection.y = rigidbody.linearVelocity.y;
        rigidbody.linearVelocity = moveDirection;
        //rigidbody.linearVelocity
        //rigidbody.AddForce(moveDirection, ForceMode.Acceleration);

        rigidbody.rotation = Quaternion.Euler(0, rotationY, 0);
        camera.transform.position = new Vector3(transform.position.x, transform.position.y+1, transform.position.z);
        camera.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, transform.localScale.y/2+0.8f))
        {
            angle = Vector3.Angle(Vector3.up, hit.normal);
            //Debug.Log("Stand On "+hit.transform.name+" "+ angle);
            return angle < maxAngle && angle != 0;
        }
        return false;
    }

    public void MouseSensitivityX(float x)
    {
        mouseSensitivityX = x;
    }
    public void MouseSensitivityY(float x)
    {
        mouseSensitivityY = x;
    }

    void Update()
    {

        moveDirection = transform.TransformDirection(new Vector3(move.x * playerSpeed, 0, move.y * playerSpeed));
        if (OnSlope()) {
            moveDirection = (Vector3.ProjectOnPlane(moveDirection, hit.normal));
            rigidbody.useGravity = false;
            if (rigidbody.angularVelocity.y < 0)
            {
                rigidbody.AddForce(Vector3.down * 180f, ForceMode.Acceleration);
                //moveDirection.y -= 180f;
                //rigidbody.useGravity = true;
                //moveDirection = Vector3.ProjectOnPlane(new Vector3(move.x* playerSpeed, 0, move.y*playerSpeed).normalized, hit.normal).normalized * playerSpeed;
            }
        }
        else
        {
            rigidbody.useGravity = true;
        }
        if (sneaking) { moveDirection.x /= sneakDivider; moveDirection.z /= 2; }

        if (move.x == 0 && move.y == 0)
        {
            //steppingSound.GetComponent<>
            walkingSound.Stop();
            crouchingSound.Stop();

            if (animator == null) { return; }
            animator.SetBool("Walking", false);
            animatorCrowbar.SetBool("Walking",false);
        }
        else
        {
            if (walkingSound.IsPlaying() || crouchingSound.IsPlaying()) { return; }
            if (sneaking) { crouchingSound.Play(); return; }
            walkingSound.Play();

            if (animator == null) { return; }
            animator.SetBool("Walking", true);
            animatorCrowbar.SetBool("Walking", true);
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
}
