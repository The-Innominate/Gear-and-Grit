using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerMovement : MonoBehaviour
{
	[Header("Movement")]
	private float moveSpeed;
	public float walkSpeed;
	public float sprintSpeed;
	public float slideSpeed;
	public float wallrunSpeed;
	public float climbSpeed;
	public float dashSpeed;
	public float swingSpeed;
	public float dashSpeedChangeFactor;
	public float maxYSpeed;

	private float desiredMoveSpeed;
	private float lastDisiredMoveSpeed;

	public float speedIncreaseMultiplier;
	public float slopeIncreaseMultiplier;

	public float groundDrag;

	private Vector3 currentPostion;
	private Vector3 previousPostion;

	[Header("jumping")]
	public float jumpForce;
	public float jumpCooldown;
	public float airMultiplier;
	bool canJump = true;

	[Header("Crouching")]
	public float crouchSpeed;
	public float crouchYScale;
	float startYScale;

	[Header("Keybinds")]
	public KeyCode jumpKey = KeyCode.Space;
	public KeyCode sprintKey = KeyCode.LeftShift;
	public KeyCode crouchKey = KeyCode.LeftControl;

	[Header("Ground Check")]
	public float playerHeight;
	public LayerMask isGround;
	public bool onGround;

	[Header("Slope Handling")]
	public float maxSlopeAngle;
	private RaycastHit slopeHit;
	private bool exitingSlope;

	[Header("CameraEffects")]
	private PlayerCam cam;
	public CameraChange currentCamera;
	public PlayerCam thirdPersonCam;
	public PlayerCam firstPersonCam;
	public float grappleFOV = 75f;

	[Header("References")]
	public Climbing climbingScript;

	public Transform orientation;

	float horizontalInput;
	float verticalInput;

	public Animator animator;
	public Transform modelTransform;

	Vector3 moveDirection;

	Rigidbody rb;

	public bool sliding;
	public bool wallRunning;
	public bool climbing;
	public bool freeze;
	public bool dashing;
	public bool swinging;
	//public bool unlimited;

	public bool activeGrapple;

	public bool restricted;

	public MovementState state;
	public enum MovementState
	{
		walking,
		sprinting,
		crouching,
		sliding,
		wallRunning,
		climbing,
		freeze,
		unlimited,
		dashing,
		swinging,
		air
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;

		canJump = true;

		startYScale = transform.localScale.y;
	}

	private void FixedUpdate()
	{
		MovePlayer();
	}

	private void Update()
	{
		onGround = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, isGround);

		MyInput();
		SpeedControl();
		stateHandler();

		if (onGround && !dashing && !activeGrapple)
		{
			rb.drag = groundDrag;
		}
		else
		{
			rb.drag = 0;
		}

		if(currentCamera.camMode == 0)
		{
			cam = thirdPersonCam;
		} else
		{
			cam = firstPersonCam;
		}

		if (animator.isActiveAndEnabled)
		{
			animator.SetFloat("YVelocity", rb.velocity.y);
			animator.SetBool("onGround", onGround);
			animator.SetBool("Sliding", sliding);
			animator.SetBool("Swinging", swinging);
		}

		modelTransform.rotation = orientation.rotation;
	}

	private void MyInput()
	{
		horizontalInput = Input.GetAxisRaw("Horizontal");
		verticalInput = Input.GetAxisRaw("Vertical");

		if (Input.GetKey(jumpKey) && canJump && onGround)
		{
			canJump = false;
			Jump();

			Invoke(nameof(ResetJump), jumpCooldown);
		}

		if (Input.GetKeyDown(crouchKey))
		{
			transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
			rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
		}

		if (Input.GetKeyUp(crouchKey))
		{
			transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
		}
	}

	bool keepMomentum;
	private MovementState lastState;
	private void stateHandler()
	{	
		if(dashing)
		{
			state = MovementState.dashing;
			desiredMoveSpeed = dashSpeed;
		} 
		else if (swinging)
		{
			state = MovementState.swinging;
			desiredMoveSpeed = swingSpeed;
		}
		else if (freeze)
		{
			state = MovementState.freeze;
			rb.velocity = Vector3.zero;
			moveSpeed = 0f;
		}
		//else if (unlimited)
		//{
		//	state = MovementState.unlimited;
		//	desiredMoveSpeed = 999f;
		//	return;
		//}
		else if (climbing)
		{
			state = MovementState.climbing;
			desiredMoveSpeed = climbSpeed;
		}
		else if (wallRunning)
		{
			state = MovementState.wallRunning;
			desiredMoveSpeed = wallrunSpeed;
		}
		else if (sliding)
		{
			state = MovementState.sliding;

			if (OnSlope() && rb.velocity.y < 0.1f)
			{
				desiredMoveSpeed = slideSpeed;
				keepMomentum = true;
			}
			else
			{
				desiredMoveSpeed = sprintSpeed;
			}
		}
		else if (Input.GetKey(crouchKey))
		{
			state = MovementState.crouching;
			desiredMoveSpeed = crouchSpeed;
		}
		else if (onGround && Input.GetKey(sprintKey))
		{
			state = MovementState.sprinting;
			desiredMoveSpeed = sprintSpeed;
		}
		else if (onGround)
		{
			state = MovementState.walking;
			desiredMoveSpeed = walkSpeed;
		}
		else
		{
			state = MovementState.air;
			
			if(desiredMoveSpeed < sprintSpeed)
			{
				desiredMoveSpeed = walkSpeed;
			} else
			{
				desiredMoveSpeed = sprintSpeed;
			}
		}

		if (Mathf.Abs(desiredMoveSpeed - lastDisiredMoveSpeed) > 4f && moveSpeed != 0)
		{
			StopAllCoroutines();
			StartCoroutine(SmoothlyLerpMoveSpeed());
		}
		else
		{
			moveSpeed = desiredMoveSpeed;
		}

		bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDisiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
		{
			if(keepMomentum)
			{
				StopAllCoroutines();
				StartCoroutine(SmoothlyLerpMoveSpeed());
			}
			else
			{
				StopAllCoroutines();
				moveSpeed = desiredMoveSpeed;
			}
		}

		lastDisiredMoveSpeed = desiredMoveSpeed;
		lastState = state;

		if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
	}

	private IEnumerator SmoothlyLerpMoveSpeed()
	{
		float time = 0;
		float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
		float startValue = moveSpeed;

		while (time < difference)
		{
			moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
			if (OnSlope())
			{
				float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
				float slopeAngleIncrease = 1 + (slopeAngle / 90f);

				time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
			}
			//else if (dashing)
			//{
			//	time += Time.deltaTime * dashSpeedChangeFactor;
			//}
			else
			{
				time += Time.deltaTime * speedIncreaseMultiplier;
			}

			yield return null;
		}

		moveSpeed = desiredMoveSpeed;
	}

	private void MovePlayer()
	{
		if (restricted || activeGrapple || swinging) return;

		if (state == MovementState.dashing) return;

		if (climbingScript.exitingWall) return;

		moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

		if (OnSlope() && !exitingSlope)
		{
			rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

			if (rb.velocity.y < -0.01)
			{
				rb.AddForce(Vector3.down * 80f, ForceMode.Force);
			}
		}

		if (onGround)
		{
			rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
		}
		else if (!onGround)
		{
			rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
		}

		if(!wallRunning) rb.useGravity = !OnSlope();
	}

	private void SpeedControl()
	{
		if(activeGrapple) return;
		if (OnSlope() && !exitingSlope)
		{
			if (rb.velocity.magnitude > moveSpeed)
			{
				rb.velocity = rb.velocity.normalized * moveSpeed;
			}
		}
		else
		{
			Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

			if (flatVel.magnitude > moveSpeed)
			{
				Vector3 limitedVel = flatVel.normalized * moveSpeed;
				rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
			}
		}

		if(maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
		{
			rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
		}
	}

	private void Jump()
	{
		exitingSlope = true;

		rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

		rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
	}

	private void ResetJump()
	{
		canJump = true;

		exitingSlope = false;
	}

	private bool enableMovemmentOnNextTouch;
	public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
	{
		activeGrapple = true;

		velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);

		Invoke(nameof(SetVelocity), 0.1f);

		Invoke(nameof(ResetRestrictions), 3f);
	}

	private Vector3 velocityToSet;
	private void SetVelocity()
	{
		enableMovemmentOnNextTouch = true;
		rb.velocity = velocityToSet;

		cam.DoFov(grappleFOV);
	}

	public void ResetRestrictions()
	{
		activeGrapple = false;

		cam.DoFov(65f);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(enableMovemmentOnNextTouch)
		{
			enableMovemmentOnNextTouch = false;
			ResetRestrictions();

			GetComponent<Grappling>().StopGrapple();
		}
	}

	public bool OnSlope()
	{
		if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
		{
			float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
			return angle < maxSlopeAngle && angle != 0;
		}

		return false;
	}

	public Vector3 GetSlopeMoveDirection(Vector3 direction)
	{
		return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
	}

	public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
	{
		float gravity = Physics.gravity.y;
		float displacementY = endPoint.y - startPoint.y;
		Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

		Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
		Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

		return velocityXZ + velocityY;
	}
}
