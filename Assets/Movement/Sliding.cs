using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
	[Header("References")]
	public Transform orientation;
	public Transform playerObj;
	public Transform modelTransform;
	private Rigidbody rb;
	private PlayerMovement pm;

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

	private static Sliding instance;

	public static Sliding Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new Sliding();
			}
			return instance;
		}
	}
	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		pm = GetComponent<PlayerMovement>();

		startYScale = playerObj.localScale.y;
	}

	private void Update()
	{
		horizontalInput = Input.GetAxisRaw("Horizontal");
		verticalInput = Input.GetAxisRaw("Vertical");

		if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
		{
			StartSlide();
		}

		if (Input.GetKeyUp(slideKey) && pm.sliding)
		{
			StopSlide();
		}
	}

	private void FixedUpdate()
	{
		if (pm.sliding)
		{
			SlidingMovement();
		}
	}

	private void StartSlide()
	{
		pm.sliding = true;

		playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
		modelTransform.localScale = new Vector3(playerObj.localScale.x, 2, playerObj.localScale.z);
		modelTransform.localPosition = new Vector3(0, -2f, 0);
		rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

		slideTimer = maxSlideTime;
	}

	private void SlidingMovement()
	{
		Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

		if (!pm.OnSlope() || rb.velocity.y > -0.01f)
		{
			rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

			slideTimer -= Time.deltaTime;
		}
		else
		{
			rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
		}

		if (slideTimer <= 0)
		{
			StopSlide();
		}
	}

	private void StopSlide()
	{
		pm.sliding = false;

		playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
		modelTransform.localScale = new Vector3(playerObj.localScale.x, 1, playerObj.localScale.z);
		modelTransform.localPosition = new Vector3(0, -1f, 0);
	}
}
