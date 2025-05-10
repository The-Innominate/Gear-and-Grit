using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class WallRunning : MonoBehaviour
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
	public float WallCheckDistance;
	public float minJumpHeight;
	private RaycastHit leftWallhit;
	private RaycastHit rightWallhit;
	private bool wallLeft;
	private bool wallRight;

	[Header("Exiting")]
	private bool exitingWall;
	public float exitWallTime;
	private float exitWallTimer;

	[Header("Gravity")]
	public bool useGravity;
	public float gravityCounterForce;

	[Header("References")]
	public Transform orientation;
	public CameraChange currentCamera;
	private PlayerCam cam;
	public PlayerCam thirdPersonCam;
	public PlayerCam firstPersonCam;
	private PlayerMovement pm;
	private LedgeGrabbing lg;
	private Rigidbody rb;

	private static WallRunning instance;

	public static WallRunning Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new WallRunning();
			}
			return instance;
		}
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		pm = GetComponent<PlayerMovement>();
		lg = GetComponent<LedgeGrabbing>();
	}

	private void Update()
	{
		CheckForWall();
		StateMachine();

		if (currentCamera.camMode == 0)
		{
			cam = thirdPersonCam;
		}
		else
		{
			cam = firstPersonCam;
		}
		if (pm.animator.isActiveAndEnabled)
		{
			if (pm.wallRunning)
			{
				pm.animator.SetBool("onRightWall", wallLeft);
				pm.animator.SetBool("onLeftWall", wallRight);
			}
			else
			{
				pm.animator.SetBool("onRightWall", false);
				pm.animator.SetBool("onLeftWall", false);
			}
		}
	}

	private void FixedUpdate()
	{
		if (pm.wallRunning)
		{
			WallRunningMovement();
		}
	}

	private void CheckForWall()
	{
		wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, WallCheckDistance, isWall);
		wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, WallCheckDistance, isWall);
	}

	private bool AboveGround()
	{
		return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, isGround);
	}

	private void StateMachine()
	{
		horizontalInput = Input.GetAxisRaw("Horizontal");
		verticalInput = Input.GetAxisRaw("Vertical");

		upwardsRunning = Input.GetKey(upwardsRunKey);
		downwardsRunning = Input.GetKey(downwardsRunKey);

		if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
		{
			if (!pm.wallRunning)
			{
				StartWallRun();
			}

			if (wallRunTimer > 0)
			{
				wallRunTimer -= Time.deltaTime;
			}

			if (wallRunTimer <= 0 && pm.wallRunning)
			{
				exitingWall = true;
				exitWallTimer = exitWallTime;
			}

			if (Input.GetKey(wallJumpKey))
			{
				WallJump();
			}
		}
		else if (exitingWall)
		{
			if (pm.wallRunning)
			{
				StopWallRun();
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
			if (pm.wallRunning)
			{
				StopWallRun();
			}
		}
	}

	private void StartWallRun()
	{
		//if (pm.activeGrapple)
		//{
		//	return;
		//}
		pm.wallRunning = true;

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

	private void WallRunningMovement()
	{
		if (pm.activeGrapple)
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

	private void StopWallRun()
	{
		pm.wallRunning = false;

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

	private void WallJump()
	{
		//if (lg.holding || lg.exitingLedge) return;

		exitingWall = true;
		exitWallTimer = exitWallTime;

		Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

		Vector3 forceTotApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

		rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
		rb.AddForce(forceTotApply, ForceMode.Impulse);

	}
}
