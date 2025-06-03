using UnityEngine;

public class GrapplingDecorator : MovementDecorator
{
    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;
    public float grappleSpeed;
    public float grappleCooldown;
    private float grappleCooldownTimer;

    [Header("References")]
    public Transform gunTip;
    public Transform camera;
    public Transform player;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    private Vector3 grapplePoint;
    private SpringJoint joint;
    private bool isGrappling;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        if (grappleCooldownTimer > 0)
        {
            grappleCooldownTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    public void StartGrapple()
    {
        if (grappleCooldownTimer > 0) return;

        isGrappling = true;
        playerMovement.activeGrapple = true;

        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = camera.position + camera.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        playerMovement.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        isGrappling = false;
        playerMovement.activeGrapple = false;
        grappleCooldownTimer = grappleCooldown;

        lr.enabled = false;
    }

    private void DrawRope()
    {
        if (!isGrappling) return;

        lr.SetPosition(0, gunTip.position);
    }

    public bool IsGrappling()
    {
        return isGrappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
} 