using UnityEngine;

public class DashDecorator : MovementDecorator
{
    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;
    public float maxDashYSpeed;

    [Header("CameraEffects")]
    private PlayerCam cam;
    public CameraChange currentCam;
    public PlayerCam thirdPersonCam;
    public PlayerCam firstPersonCam;
    public float dashFOV;

    [Header("Settings")]
    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.E;

    private Vector3 delayedForceToApply;

    private void Update()
    {
        if (Input.GetKeyDown(dashKey))
        {
            Activate();
        }

        if (dashCdTimer > 0)
        {
            dashCdTimer -= Time.deltaTime;
        }

        if (currentCam.camMode == 0)
        {
            cam = thirdPersonCam;
        }
        else
        {
            cam = firstPersonCam;
        }
    }

    public override void Activate()
    {
        if (dashCdTimer > 0) return;
        
        base.Activate();
        dashCdTimer = dashCd;

        playerMovement.dashing = true;
        playerMovement.maxYSpeed = maxDashYSpeed;

        cam.DoFov(dashFOV);

        Transform forwardT = useCameraForward ? currentCam.transform : orientation;
        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
        {
            rb.useGravity = false;
        }

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(Deactivate), dashDuration);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        playerMovement.dashing = false;
        playerMovement.maxYSpeed = 0;

        if (disableGravity)
        {
            rb.useGravity = true;
        }

        cam.DoFov(65f);
    }

    private void DelayedDashForce()
    {
        if (resetVel)
        {
            rb.velocity = Vector3.zero;
        }

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirections)
        {
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        }
        else
        {
            direction = forwardT.forward;
        }

        if (direction.magnitude == 0)
        {
            direction = forwardT.forward;
        }

        return direction.normalized;
    }
} 