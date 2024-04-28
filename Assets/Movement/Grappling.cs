using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask isGrappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grapplingKey = KeyCode.Mouse1;

	[Header("Prediction")]
	public RaycastHit predictionHit;
	public float predictionSphereCastRadius;
	public Transform predictionPoint;

	private bool grappling;

	private void Start()
	{
		pm = GetComponent<PlayerMovement>();
        lr.enabled = false;
	}

	private void Update()
	{
		if(Input.GetKeyDown(grapplingKey)) StartGrapple();

        if(grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }

        CheckForGrapplePoints();
	}

	private void LateUpdate()
	{
		if(grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
	}

	private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

		if (GetComponent<Swinging>() != null) GetComponent<Swinging>().StopSwing();

        grappling = true;

        pm.freeze = true;

        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, isGrappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        } 
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        pm.freeze = false;

        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }

	private void CheckForGrapplePoints()
	{
		RaycastHit sphereCastHit;
		Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out sphereCastHit, maxGrappleDistance, isGrappleable);

		RaycastHit raycastHit;
		Physics.Raycast(cam.position, cam.forward, out raycastHit, maxGrappleDistance, isGrappleable);

		Vector3 realHitPoint;

		if (raycastHit.point != Vector3.zero)
		{
			realHitPoint = raycastHit.point;
		}
		else if (sphereCastHit.point != Vector3.zero)
		{
			realHitPoint = sphereCastHit.point;
		}
		else
		{
			realHitPoint = Vector3.zero;
		}

		if (realHitPoint != Vector3.zero)
		{
			predictionPoint.gameObject.SetActive(true);
			predictionPoint.position = realHitPoint;
		}
		else
		{
			predictionPoint.gameObject.SetActive(false);
		}

		predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
	}
}
