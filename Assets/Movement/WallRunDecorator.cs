using UnityEngine;

public class WallRunDecorator : MovementDecorator
{
    [Header("WallRunning")]
    public LayerMask isWall;
    public LayerMask isGround;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallClimbSpeed;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    public KeyCode wallJumpKey = KeyCode.Space;
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float WallCheckDistance = 0.7f;
    public float minJumpHeight = 1.5f;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime = 0.5f;
    private float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity = false;
    public float gravityCounterForce = 5f;

    [Header("References")]
    private PlayerCam cam;
    public CameraChange currentCamera;
    public PlayerCam thirdPersonCam;
    public PlayerCam firstPersonCam;

    private void Start()
    {
        base.Start();
        Debug.Log("WallRunDecorator initialized");
    }

    private void Update()
    {
        // Always check for walls and handle state
        CheckForWall();
        HandleState();

        // Only handle camera and animation if active
        if (isActive)
        {
            if (currentCamera != null)
            {
                if (currentCamera.camMode == 0)
                {
                    cam = thirdPersonCam;
                }
                else
                {
                    cam = firstPersonCam;
                }
            }

            if (playerMovement.animator != null && playerMovement.animator.isActiveAndEnabled)
            {
                playerMovement.animator.SetBool("onRightWall", wallLeft);
                playerMovement.animator.SetBool("onLeftWall", wallRight);
            }
        }
        else
        {
            if (playerMovement.animator != null && playerMovement.animator.isActiveAndEnabled)
            {
                playerMovement.animator.SetBool("onRightWall", false);
                playerMovement.animator.SetBool("onLeftWall", false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            HandleMovement();
        }
    }

    private void CheckForWall()
    {
        // Debug ray visualization
        Debug.DrawRay(transform.position, orientation.right * WallCheckDistance, Color.red);
        Debug.DrawRay(transform.position, -orientation.right * WallCheckDistance, Color.blue);

        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, WallCheckDistance, isWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, WallCheckDistance, isWall);
        
        if (wallRight || wallLeft)
        {
            Debug.Log($"Wall detected - Left: {wallLeft}, Right: {wallRight}");
            Debug.Log($"Wall distance - Left: {(wallLeft ? leftWallhit.distance : 0)}, Right: {(wallRight ? rightWallhit.distance : 0)}");
            Debug.Log($"Wall normal - Left: {(wallLeft ? leftWallhit.normal : Vector3.zero)}, Right: {(wallRight ? rightWallhit.normal : Vector3.zero)}");
            Debug.Log($"Wall layer - Left: {(wallLeft ? LayerMask.LayerToName(leftWallhit.collider.gameObject.layer) : "none")}, Right: {(wallRight ? LayerMask.LayerToName(rightWallhit.collider.gameObject.layer) : "none")}");
        }
    }

    private bool AboveGround()
    {
        // Debug ray visualization
        Debug.DrawRay(transform.position, Vector3.down * minJumpHeight, Color.green);

        bool above = !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, isGround);
        if (!above)
        {
            Debug.Log($"Not above ground - too close to ground. Height: {minJumpHeight}");
        }
        return above;
    }

    public override void HandleState()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        bool canWallRun = (wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall;
        
        Debug.Log($"Wall run conditions - Wall: {(wallLeft || wallRight)}, Forward: {verticalInput > 0}, Above Ground: {AboveGround()}, Not Exiting: {!exitingWall}");
        Debug.Log($"Input - Horizontal: {horizontalInput}, Vertical: {verticalInput}");
        Debug.Log($"Keys - Up: {upwardsRunning}, Down: {downwardsRunning}");
        
        if (canWallRun)
        {
            Debug.Log("Can wall run - attempting to activate");
            if (!isActive)
            {
                Activate();
            }

            if (wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
                Debug.Log($"Wall run timer: {wallRunTimer}");
            }

            if (wallRunTimer <= 0 && isActive)
            {
                Debug.Log("Wall run timer expired");
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            if (Input.GetKey(wallJumpKey))
            {
                Debug.Log("Wall jump initiated");
                WallJump();
            }
        }
        else if (exitingWall)
        {
            if (isActive)
            {
                Deactivate();
            }

            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }
        else
        {
            if (isActive)
            {
                Deactivate();
            }
        }
    }

    public override void Activate()
    {
        Debug.Log("Activating wall run");
        base.Activate();
        playerMovement.wallRunning = true;

        wallRunTimer = maxWallRunTime;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        cam.DoFov(80f);

        if (currentCamera.camMode == 0)
        {
            if (wallRight) cam.DoTilt(23.064f, 5f);
            if (wallLeft) cam.DoTilt(23.064f, -5f);
        }
        else
        {
            if (wallRight) cam.DoTilt(0f, 5f);
            if (wallLeft) cam.DoTilt(0f, -5f);
        }
    }

    public override void Deactivate()
    {
        Debug.Log("Deactivating wall run");
        base.Deactivate();
        playerMovement.wallRunning = false;

        cam.DoFov(65f);
        if (currentCamera.camMode == 0)
        {
            cam.DoTilt(23.064f, 0f);
        }
        else
        {
            cam.DoTilt(0f, 0f);
        }
    }

    public override void HandleMovement()
    {
        if (playerMovement.activeGrapple)
        {
            rb.useGravity = true;
            return;
        }

        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (upwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        }

        if (downwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        }

        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallForward * 100, ForceMode.Force);
        }

        if (useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }
    }

    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
} 