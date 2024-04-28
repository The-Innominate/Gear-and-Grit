using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class Swinging : MonoBehaviour
{
	[Header("References")]
	public LineRenderer lr;
	public Transform gunTip, cam, player;
	public LayerMask isGrappleable;
	public PlayerMovement pm;


    [Header("Swinging")]
	private float maxSwingDistance = 25f;
	private Vector3 swingPoint;
	private SpringJoint joint;

	[Header("AirGear")]
	public Transform orientation;
	public Rigidbody rb;
	public float horizontalThrustForce;
	public float forwardThrustForce;
	public float extendCableSpeed;

	[Header("Prediction")]
	public RaycastHit predictionHit;
	public float predictionSphereCastRadius;
	public Transform predictionPoint;

	[Header("Input")]
	public KeyCode swingKey = KeyCode.Mouse0;


    private void Update()
	{
		if(!PauseMenu.isPaused)
		{
			if (Input.GetKeyDown(swingKey)) StartSwing();
			if (Input.GetKeyUp(swingKey)) StopSwing();

			CheckForSwingPoints();

			if (joint != null) AirMovement();
		}
	}

	private void LateUpdate()
	{
		DrawRope();
	}

	private void Start()
	{
        lr.enabled = false;
	}

	private void StartSwing()
	{
		if (predictionHit.point == Vector3.zero) return;

		if (GetComponent<Grappling>() != null) GetComponent<Grappling>().StopGrapple();
		pm.ResetRestrictions();

		pm.swinging = true;

		swingPoint = predictionHit.point;
		joint = player.gameObject.AddComponent<SpringJoint>();
		joint.autoConfigureConnectedAnchor = false;
		joint.connectedAnchor = swingPoint;

		float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

		joint.maxDistance = distanceFromPoint * 0.8f;
		joint.minDistance = distanceFromPoint * 0.25f;

		joint.spring = 4.5f;
		joint.damper = 7f;
		joint.massScale = 4.5f;

		lr.positionCount = 2;
		currentGrapplePosition = gunTip.position;

		lr.enabled = true;
	}

	public void StopSwing()
	{
		pm.swinging = false;

		lr.positionCount = 0;
		Destroy(joint);

		lr.enabled = false;
	}

	private Vector3 currentGrapplePosition;

	private void DrawRope()
	{
		if (!joint) return;

		currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

		lr.SetPosition(0, gunTip.position);
		lr.SetPosition(1, swingPoint);
	}

	private void AirMovement()
	{
		if (Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
		if (Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);
		if (Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * forwardThrustForce * Time.deltaTime);

		if (Input.GetKey(KeyCode.Space))
		{
			Vector3 directionToPoint = swingPoint - transform.position;
			rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

			float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

			joint.maxDistance = distanceFromPoint * 0.8f;
			joint.minDistance = distanceFromPoint * 0.25f;
		}

		if (Input.GetKey(KeyCode.S))
		{
			float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

			joint.maxDistance = extendedDistanceFromPoint * 0.8f;
			joint.minDistance = extendedDistanceFromPoint * 0.25f;
		}
	}

	private void CheckForSwingPoints()
	{
		if (joint != null) return;

		RaycastHit sphereCastHit;
		Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out sphereCastHit, maxSwingDistance, isGrappleable);

		RaycastHit raycastHit;
		Physics.Raycast(cam.position, cam.forward, out raycastHit, maxSwingDistance, isGrappleable);

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
