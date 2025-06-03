using UnityEngine;

public abstract class MovementDecorator : MonoBehaviour
{
    protected PlayerMovement playerMovement;
    protected Rigidbody rb;
    protected Transform orientation;
    protected bool isActive;

    protected virtual void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        orientation = playerMovement.orientation;
    }

    public virtual void Activate()
    {
        isActive = true;
    }

    public virtual void Deactivate()
    {
        isActive = false;
    }

    public virtual void HandleMovement()
    {
        // Base movement handling
    }

    public virtual void HandleInput()
    {
        // Base input handling
    }

    public virtual void HandleState()
    {
        // Base state handling
    }

    public virtual void HandlePhysics()
    {
        // Base physics handling
    }
} 