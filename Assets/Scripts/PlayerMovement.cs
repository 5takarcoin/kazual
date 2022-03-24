using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public enum PlayerState
    {
        Grounded,
        InAir,
        OnWalls,
        LedgeGrab
    }



    [SerializeField] Text climb;

    private PlayerCollision coli;
    private Rigidbody rb;

    [Header("Physics")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float backwardSpeed;
    [SerializeField] private float inAirControl;

    private float actSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;
    [SerializeField] private float directionalControl;

    private PlayerState currentState;
    public bool dekhoTo;

    private float airTime;
    private float groundTime;
    private float adjustAmount;

    [Header("Turning")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private float turnSpeedInAir;
    [SerializeField] private float turnSpeedOnWalls;
    [SerializeField] private float lookUpSpeed;
    [SerializeField] private Camera head;
    private float yTurn;
    private float xTurn;
    [SerializeField] private float maxLookAngle;
    [SerializeField] private float minLookAngle;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight;

    [Header("Wall Running")]
    [SerializeField] private float wallRunTime;
    private float actWallRunTime;
    [SerializeField] private float timeBeforeWallRun;
    [SerializeField] private float wallRunUpwardsMovement;
    [SerializeField] private float wallRunSpeedAcceleratoin;

    [Header("Crouching")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchHeight;
    private float standingHeight;
    private bool crouch;

    [Header("Sliding")]
    [SerializeField] private float slideAmount;
    [SerializeField] private float slideSpeedLimit;
    [SerializeField] private float slideControl;

    [Header("Wall Grabbing")]
    private Vector3 originalPosition;
    [SerializeField] public Vector3 ledgePosition;

    [Header("FOV")]
    [SerializeField] private float maxFov;
    [SerializeField] private float minFov;
    [SerializeField] private float fovSpeed;

    private float horizontalInput;
    private float verticalInput;

    private float xMouseInput;
    private float yMouseInput;

    private bool jumpInput;
    private bool grabInput;
    private bool crouchInput;

    Vector3 ledgeTemp;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        coli = GetComponent<PlayerCollision>();
        rb = GetComponent<Rigidbody>();
        standingHeight = transform.localScale.y;

        adjustAmount = 1;
    }

    private void Update()
    {
        dekhoTo = currentState == PlayerState.LedgeGrab ? true : false;

        ledgeTemp = coli.CheckLedges();
        climb.gameObject.SetActive(ledgeTemp != Vector3.zero);

        //Debug.Log(currentState);
        GetInput();
        ManageStates();
        PlayerLook();
        //HandleFOV();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        xMouseInput = Input.GetAxisRaw("Mouse X");
        yMouseInput = Input.GetAxisRaw("Mouse Y");

        jumpInput = Input.GetKeyDown(KeyCode.Space);
        grabInput = Input.GetKeyDown(KeyCode.E);
        crouchInput = Input.GetKeyDown(KeyCode.C);
    }

    private void ManageStates()
    {
        switch (currentState)
        {
            case PlayerState.Grounded:
                if (groundTime < 10) groundTime += Time.deltaTime;
                Ground();
                break;
            case PlayerState.InAir:
                InAir();
                break;
            case PlayerState.OnWalls:
                OnWalls();
                break;
            case PlayerState.LedgeGrab:
                Ledge();
                break;
        }
    }

    private void Jump()
    {
        Vector3 temp = rb.velocity;
        temp.y = 0;
        rb.velocity = temp;
        rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
        SetInAir();
    }

    private void OnWalls()
    {
        MoveOnWalls();

        actWallRunTime += Time.deltaTime;

        if (grabInput)
        {
            ledgePosition = coli.CheckLedges();
            if (ledgePosition != Vector3.zero)
            {
                LedgeGrab();
            }
        }

        if(!CheckWalls()){
            SetInAir();
            return;
        }

        if (coli.CheckFloor(-transform.up)) SetOnGround();
    }

    private void InAir()
    {
        MoveInAir();

        if (airTime < 10) airTime += Time.deltaTime;

        if (grabInput)
        {
            ledgePosition = coli.CheckLedges();
            if (ledgePosition != Vector3.zero) LedgeGrab();
        }

        if (CheckWalls() && airTime > timeBeforeWallRun)
        {
            SetOnWalls();
            return;
        }

        if (coli.CheckFloor(-transform.up) && airTime > 0.25f) SetOnGround();
    }

    private void PlayerLook()
    {
        xTurn -= yMouseInput * lookUpSpeed;
        xTurn = Mathf.Clamp(xTurn, minLookAngle, maxLookAngle);
        head.transform.localRotation = Quaternion.Euler(xTurn, 0, 0);

        float turn;
        switch (currentState)
        {
            case PlayerState.Grounded:
                turn = turnSpeed;
                break;
            case PlayerState.InAir:
                turn = turnSpeedInAir;
                break;
            case PlayerState.OnWalls:
                turn = turnSpeedOnWalls;
                break;
            case PlayerState.LedgeGrab:
                turn = 0;
                break;
            default:
                turn = turnSpeed;
                break;
        }

        yTurn += xMouseInput * turn;
        transform.rotation = Quaternion.Euler(0, yTurn, 0);
    }

    private void HandleFOV()
    {
        float magnitude = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
        float fov = Mathf.Lerp(minFov, maxFov, magnitude / fovSpeed);

        head.fieldOfView = Mathf.Lerp(head.fieldOfView, fov, 4 * Time.deltaTime);
    }

    private void MovePlayer()
    {
        Vector3 movementDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
        if (movementDirection == Vector3.zero) movementDirection = rb.velocity.normalized;

        movementDirection *= actSpeed;
        movementDirection.y = rb.velocity.y;

        rb.velocity = Vector3.Lerp(rb.velocity, movementDirection, directionalControl * adjustAmount * Time.deltaTime);
    }

    private void SetInAir()
    {
        StopCrouching();
        groundTime = 0;
        currentState = PlayerState.InAir;
    }

    private void StartCrouching()
    {
        crouch = true;

        Vector3 temp = transform.localScale;
        temp.y = crouchHeight;
        transform.localScale = temp;

        if (actSpeed > slideSpeedLimit) Slide();
    }

    private void StopCrouching()
    {
        crouch = false;
        Vector3 temp = transform.localScale;
        temp.y = standingHeight;
        transform.localScale = temp;
    }

    private void Slide()
    {
        actSpeed = slideSpeedLimit;
        adjustAmount = 0;

        Vector3 direction = rb.velocity.normalized;
        direction.y = 0;
        rb.AddForce(direction * slideAmount, ForceMode.Impulse);
    }

    private void LedgeGrab()
    {
        originalPosition = transform.position;
        rb.velocity = Vector3.zero;
        actSpeed = 0;
        currentState = PlayerState.LedgeGrab;
    }

    private bool CheckWalls()
    {
        if (horizontalInput == 0 && verticalInput == 0) return false;
        if (actWallRunTime >= wallRunTime) return false;

        float clampedVer = Mathf.Clamp(verticalInput, 0, 1);
        Vector3 direction = transform.forward * clampedVer + transform.right * horizontalInput;

        return coli.CheckWall(direction);
    }

    private void SetOnGround()
    {
        actSpeed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
        actWallRunTime = 0;
        airTime = 0;
        currentState = PlayerState.Grounded;
    }

    private void SetOnWalls()
    {
        groundTime = 0;
        airTime = 0;
        currentState = PlayerState.OnWalls;
    }


    private void MoveInAir()
    {
        Vector3 movementDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
        if (movementDirection == Vector3.zero) movementDirection = rb.velocity.normalized;

        movementDirection *= actSpeed;
        movementDirection.y = rb.velocity.y;

        rb.velocity = Vector3.Lerp(rb.velocity, movementDirection, inAirControl * Time.deltaTime);
    }

    private void MoveOnWalls()
    {
        Vector3 movementDirection = transform.up * verticalInput * wallRunUpwardsMovement + transform.forward * actSpeed;
        rb.velocity = Vector3.Lerp(rb.velocity, movementDirection, wallRunSpeedAcceleratoin * Time.deltaTime);
    }

    private void Ground()
    {
        if (jumpInput) Jump();

        else
        {
            float inputMagnitude = new Vector2(horizontalInput, verticalInput).normalized.magnitude;
            float targetSpeed = Mathf.Lerp(backwardSpeed, maxSpeed, verticalInput);

            if (crouch) targetSpeed = crouchSpeed;

            float accel = inputMagnitude == 0 ? decceleration : acceleration;

            actSpeed = Mathf.Lerp(actSpeed, targetSpeed * inputMagnitude, accel * Time.deltaTime);

            MovePlayer();

            if (crouchInput && !crouch) StartCrouching();
            
            else if (crouchInput && crouch && !coli.CheckRoof(transform.up)) StopCrouching();

            if (adjustAmount < 1) adjustAmount += slideControl * Time.deltaTime;
            else adjustAmount = 1;

            if (!coli.CheckFloor(-transform.up))
            {
                if (airTime < 0.2f) airTime += Time.deltaTime;
                else
                {
                    SetInAir();
                    return;
                }
            }
            else airTime = 0;
        }

        if (grabInput)
        {
            ledgePosition = coli.CheckLedges();
            if (ledgePosition != Vector3.zero)
            {
                LedgeGrab();
            }
        }
    }

    private void Ledge()
    {
        if (transform.position.y < ledgePosition.y) rb.velocity = new Vector3(0, 4, 0);
        else if (Vector3.Distance(transform.position, ledgePosition) < 0.1)
        {
            SetOnGround();
            if (horizontalInput == 0 && verticalInput == 0) rb.velocity = Vector3.zero;
        }
        else if (transform.position.y >= ledgePosition.y) rb.velocity = (ledgePosition - transform.position).normalized * 4;
    }

    public void SetState()
    {
        currentState = PlayerState.InAir;
    }

    public void Reset()
    {
        currentState = PlayerState.InAir;
    }
}

