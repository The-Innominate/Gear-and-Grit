using UnityEngine;

public class SlidingDecorator : MovementDecorator
{
    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;
    public float slideYScale;
    private float startYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        base.Start();
        startYScale = transform.localScale.y;
        Debug.Log("SlidingDecorator initialized with startYScale: " + startYScale);
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0) && playerMovement.onGround)
        {
            Debug.Log("Slide key pressed - Attempting to activate slide");
            Activate();
        }

        if (Input.GetKeyUp(slideKey) && isActive)
        {
            Debug.Log("Slide key released - Deactivating slide");
            Deactivate();
        }

        // Deactivate slide if we're no longer on ground
        if (isActive && !playerMovement.onGround)
        {
            Debug.Log("No longer on ground - Deactivating slide");
            Deactivate();
        }
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            HandleMovement();
        }
    }

    public override void Activate()
    {
        Debug.Log("Activating slide");
        base.Activate();
        playerMovement.sliding = true;

        // Store the current position
        Vector3 currentPosition = transform.position;
        
        // Adjust the scale
        transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        playerMovement.modelTransform.localScale = new Vector3(transform.localScale.x, 2, transform.localScale.z);
        playerMovement.modelTransform.localPosition = new Vector3(0, -2f, 0);
        
        // Add a small upward force to prevent falling through
        rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
        
        // Add the downward force
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
        Debug.Log("Slide activated - Timer set to: " + maxSlideTime);
    }

    public override void Deactivate()
    {
        Debug.Log("Deactivating slide");
        base.Deactivate();
        playerMovement.sliding = false;
        
        // Reset the scale
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        playerMovement.modelTransform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
        playerMovement.modelTransform.localPosition = Vector3.zero;
        
        // Add a small upward force when deactivating to prevent getting stuck
        rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
        
        Debug.Log("Slide deactivated - Scale reset to: " + startYScale);
    }

    public override void HandleMovement()
    {
        if (!playerMovement.onGround)
        {
            Deactivate();
            return;
        }

        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        Debug.Log("Handling slide movement - Input direction: " + inputDirection);

        if (!playerMovement.OnSlope() || rb.velocity.y > -0.01f)
        {
            Debug.Log("Applying normal slide force");
            // Add a small upward force to maintain ground contact
            rb.AddForce(Vector3.up * 0.5f, ForceMode.Force);
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
            Debug.Log("Slide timer remaining: " + slideTimer);
        }
        else
        {
            Debug.Log("Applying slope slide force");
            rb.AddForce(playerMovement.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
        {
            Debug.Log("Slide timer expired - Deactivating slide");
            Deactivate();
        }
    }
} 