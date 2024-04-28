using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class LedgeGrabbing : MonoBehaviour
{
	[Header("References")]
	public PlayerMovement pm;
	public Transform orientation;
	public Transform cam;
	public Rigidbody rb;

	[Header("Ledge Grabbing")]
	public float moveToLedgeSpeed;
	public float maxLedgeGrabDistance;

	public float minTimeOnLedge;
	private float timeOnLedge;

	public bool holding;

	[Header("Ledge Jumping")]
	public KeyCode jumpKey = KeyCode.Space;
	public float ledgeJumpForwardForce;
	public float ledgeJumpUpwardForce;

	[Header("Ledge Detection")]
	public float ledgeDetectionLength;
	public float ledgeSphereCastRadius;
	public LayerMask isLedge;

	private Transform lastLedge;
	private Transform currentLedge;

	private RaycastHit ledgeHit;

	[Header("Exiting")]
	public bool exitingLedge;
	public float exitLedgeTime;
	private float exitLedgeTimer;

	private void Update()
	{
		LedgeDetection();
		SubStateMachine();
	}

	private void SubStateMachine()
	{
		float horizontalInput = Input.GetAxisRaw("Horizontal");
		float verticalInput = Input.GetAxisRaw("Vertical");
		bool anyInputKeyPressed = horizontalInput != 0 || verticalInput != 0;

		if (holding)
		{
			FreezeRigidbodyOnLedge();

			timeOnLedge += Time.deltaTime;

			if (timeOnLedge > minTimeOnLedge && anyInputKeyPressed) ExitLedgeHold();

			if (Input.GetKeyDown(jumpKey)) LedgeJump();
		}
		else if (exitingLedge)
        {
			if (exitLedgeTimer > 0) exitLedgeTimer -= Time.deltaTime;
			else exitingLedge = false;
        }
	}

	private void LedgeDetection()
	{
		bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, cam.forward, out ledgeHit, ledgeDetectionLength, isLedge);

		if (!ledgeDetected) return;

		float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.transform.position);

		if (ledgeHit.transform == lastLedge) return;

		if (distanceToLedge < maxLedgeGrabDistance && !holding) EnterLedgeHold();
	}

	private void LedgeJump()
	{
		ExitLedgeHold();

		Invoke(nameof(DelayedJumpForce), 0.05f);
	}

	private void DelayedJumpForce()
	{
		Vector3 forceToAdd = cam.forward * ledgeJumpForwardForce + orientation.up * ledgeJumpUpwardForce;
		rb.velocity = Vector3.zero;
		rb.AddForce(forceToAdd, ForceMode.Impulse);
	}

	private void EnterLedgeHold()
	{
		holding = true;

		currentLedge = ledgeHit.transform;
		lastLedge = ledgeHit.transform;

		rb.useGravity = false;
		rb.velocity = Vector3.zero;

		//pm.unlimited = true;
		pm.restricted = true;
	}

	private void FreezeRigidbodyOnLedge()
	{
		rb.useGravity = false;

		Vector3 directionToLedge = currentLedge.position - transform.position;
		float distacneToLedge = Vector3.Distance(transform.position, currentLedge.position);

		if (distacneToLedge > 1f)
		{
			if (rb.velocity.magnitude > moveToLedgeSpeed)
			{
				rb.AddForce(directionToLedge.normalized * moveToLedgeSpeed * 1000f * Time.deltaTime);
			}
		}
		else
		{
			if (!pm.freeze) pm.freeze = true;
			//if (pm.unlimited) pm.unlimited = false;

			if(distacneToLedge > maxLedgeGrabDistance) ExitLedgeHold();
		}
	}

	private void ExitLedgeHold()
	{
		exitingLedge = true;
		exitLedgeTimer = exitLedgeTime;

		holding = false;
		timeOnLedge = 0;

		pm.restricted = false;
		pm.freeze = false;

		rb.useGravity = true;

		StopAllCoroutines();
		Invoke(nameof(ResetLastLedge), 1f);
	}

	private void ResetLastLedge()
	{
		lastLedge = null;
	}
}
