using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    private float speed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    public float crouchCooldown;
    private float startYScale;
    private bool isCrouching;
    private bool canCrouch;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode debugKey = KeyCode.F3;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;
    public bool inDebug;

    Vector3 moveDirection;
    Rigidbody rb;
    float horizInput;
    float vertInput;
    public GameObject[] inventory;
    public GameObject inHand;

    public MovementState state;
    public enum MovementState
    {
        idle,
        walking,
        sprinting,
        crouching,
        crouchwalking,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        inDebug = false;

        readyToJump = true;

        canCrouch = true;
        isCrouching = false;
        startYScale = transform.localScale.y;

        inventory = new GameObject[5];
        for (int i = 0; i < inventory.Length; i++)
        {
            GameObject emptySlot = new GameObject("EmptySlot_" + i);
            emptySlot.transform.SetParent(gameObject.transform);
            emptySlot.AddComponent<nullItem>();
            inventory[i] = emptySlot;
        }


        //temp objects
        GameObject compassObj = new GameObject("Compass");
        compassObj.AddComponent<compass>();
        add_Item(compassObj, 0);

        GameObject gunObj = new GameObject("Gun");
        gunObj.AddComponent<gun>();
        add_Item(gunObj, 1);

        GameObject gpsObj = new GameObject("GPS");
        gpsObj.AddComponent<gps>();
        add_Item(gpsObj, 2);

        GameObject radioObj = new GameObject("Radio");
        radioObj.AddComponent<radio>();
        add_Item(radioObj, 3);

        GameObject foodObj = new GameObject("Food");
        foodObj.AddComponent<food>();
        add_Item(foodObj, 4);

        inHand = inventory[0];
        
    }

    private void Update()
    {

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        GetInput();
        SpeedControl();
        StateHandler();

        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void add_Item(GameObject item, int slot)
    {
        if (inHand == inventory[slot])
        {
            inHand = item;
        }
        Destroy(inventory[slot]);
        inventory[slot] = item;
        item.transform.SetParent(gameObject.transform);
    }

    private void GetInput()
    {
        horizInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey) && canCrouch)
        {
            canCrouch = false;

            if (!isCrouching)
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                rb.AddForce(Vector3.down * 3f, ForceMode.Impulse);
                isCrouching = true;
            }
            else
            {
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                rb.AddForce(Vector3.up * 3f, ForceMode.Impulse);
                isCrouching = false;
            }

            Invoke(nameof(resetCrouch), crouchCooldown);
        }

        if (Input.GetKeyDown(debugKey))
        {
            inDebug = !inDebug;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inHand = inventory[0];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inHand = inventory[1];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            inHand = inventory[2];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            inHand = inventory[3];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            inHand = inventory[4];
        }

        if (inHand != null && Input.GetMouseButtonDown(0))
        {
            inHand.GetComponent<Item>().Left(gameObject);
        }
        else if (inHand != null && Input.GetMouseButtonDown(1))
        {
            inHand.GetComponent<Item>().Right(gameObject);
        }

    }

    private void resetCrouch()
    {
        canCrouch = true;
    }

    private void StateHandler()
    {
        if (grounded)
        {
            if (rb.linearVelocity.x != 0 && rb.linearVelocity.z != 0)
            {
                if (isCrouching)
                {
                    state = MovementState.crouchwalking;
                    speed = crouchSpeed;
                }

                else if (Input.GetKey(sprintKey))
                {
                    state = MovementState.sprinting;
                    speed = sprintSpeed;
                }
                else
                {
                    state = MovementState.walking;
                    speed = walkSpeed;
                }

            }
            else if (isCrouching)
            {
                state = MovementState.crouching;
                speed = crouchSpeed;
            }
            else
            {
                state = MovementState.idle;
                speed = walkSpeed;
            }
            
        }

        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.right * horizInput + orientation.forward * vertInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * speed * 20f, ForceMode.Force);
            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (grounded)
            rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * speed * 10f * airMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {

        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > speed)
                rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
        else
        {
            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVelocity.magnitude > speed)
            {
                Vector3 limitVelocity = flatVelocity.normalized * speed;
                rb.linearVelocity = new Vector3(limitVelocity.x, rb.linearVelocity.y, limitVelocity.z);
            }
        }
        
    }

    private void Jump()
    {
        exitingSlope = true;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public void shoot()
    {
        //raycast
        //on hit
        //create sound collider
        create_sound_collider(100);
    }

    private void create_sound_collider(float size)
    {

        GameObject soundObj = new GameObject("SoundCollider");
        soundObj.transform.position = transform.position;
        soundObj.transform.parent = null;

        SphereCollider col = soundObj.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = size;

        int layer = LayerMask.NameToLayer("Sound");
        if (layer != -1)
            soundObj.layer = layer;

        StartCoroutine(destroy_sound_collider(soundObj));
    }
    
    IEnumerator destroy_sound_collider(GameObject pulse)
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(pulse);
    }
}
