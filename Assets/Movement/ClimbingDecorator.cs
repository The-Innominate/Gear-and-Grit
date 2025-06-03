using UnityEngine;

public class ClimbingDecorator : MovementDecorator
{
    [Header("Climbing")]
    public float climbSpeed = 5f;
    public float maxClimbTime = 2f;
    private float climbTimer;

    [Header("ClimbJumping")]
    public float climbJumpUpForce = 5f;
    public float climbJumpBackForce = 5f;
    public KeyCode jumpKey = KeyCode.Space;
    public int climbJumps = 2;
    private int climbJumpsLeft;

    [Header("Detection")]
    public float detectionLength = 0.7f;
    public float sphereCastRadius = 0.3f;
    public float maxWallLookAngle = 80f;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange = 30f;

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime = 0.5f;
    private float exitWallTimer;

    private void Start()
    {
        base.Start();
        Debug.Log("ClimbingDecorator initialized");
    }

    private void Update()
    {
        WallDetection();
        HandleState();

        if (isActive && !exitingWall)
        {
            HandleMovement();
        }
    }

    private void WallDetection()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, playerMovement.isGround);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if ((wallFront && newWall) || playerMovement.onGround)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
            Debug.Log("Wall detected - Timer and jumps reset");
        }
    }

    public override void HandleState()
    {
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            if (!isActive && climbTimer > 0)
            {
                Debug.Log("Starting climb");
                Activate();
            }

            if (climbTimer > 0)
            {
                climbTimer -= Time.deltaTime;
            }
            if (climbTimer < 0)
            {
                Debug.Log("Climb timer expired");
                Deactivate();
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

        if (wallFront && Input.GetKeyDown(jumpKey) && climbJumpsLeft > 0)
        {
            Debug.Log("Climb jump initiated");
            ClimbJump();
        }
    }

    public override void Activate()
    {
        Debug.Log("Activating climb");
        base.Activate();
        playerMovement.climbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
    }

    public override void Deactivate()
    {
        Debug.Log("Deactivating climb");
        base.Deactivate();
        playerMovement.climbing = false;
    }

    public override void HandleMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    private void ClimbJump()
    {
        if (playerMovement.onGround) return;

        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;
        Debug.Log("Climb jump executed - Jumps remaining: " + climbJumpsLeft);
    }
} 