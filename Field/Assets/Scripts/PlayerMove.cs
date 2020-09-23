using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementStatus
{
    Stand, Run, StrafeLeft, StrafeRight, Jumping
}

public class PlayerMove : MonoBehaviour
{
    Animator ani;

    [SerializeField]
    bool isPlayerControlled = false;
    [SerializeField]
    float jumpSpeed = 4.0f;
    [SerializeField]
    float rotateSpeed = 250.0f;
    [SerializeField]
    float baseGroundCheckDistance = 0.2f;
    float groundCheckDistance = 0.2f;

    Vector3 groundNormal = Vector3.up;
    Vector3 inputVelocity = Vector3.zero;

    Rigidbody unitRigidbody;
    CapsuleCollider unitCollider;

    private GameStatus game_status = null;
    private ItemRoot item_root = null; // ItemRoot 스크립트를 가짐.

    MovementStatus movementStatus;
    bool jumping = false;
    bool grounded = false;
    bool wasGrounded = false;

    private bool isGrounded;
    public bool IsGrounded
    {
        get { return isGrounded; }
        set
        {
            isGrounded = value;
            if (isGrounded)
                ani.SetBool("SetJump", false);
        }
    }

    void Awake()
    {
        unitRigidbody = GetComponent<Rigidbody>();
        unitCollider = GetComponent<CapsuleCollider>();
        ani = GetComponent<Animator>();
        groundCheckDistance = baseGroundCheckDistance;

        this.game_status = GameObject.Find("GameRoot").GetComponent<GameStatus>();
        this.item_root = GameObject.Find("GameRoot").GetComponent<ItemRoot>();
    }

    public void MoveUpdate()
    {
        // Only allow movement and jumps while grounded
        ApplyInputVelocity();

        // Allow turning at anytime. Keep the character facing in the same direction as the Camera if the right mouse button is down.
        ApplyInputRotation();
    }

    public void MoveFixedUpdate()
    {
        unitCollider.radius = 0.2f;

        if (jumping)
        {
            unitRigidbody.velocity = inputVelocity;
            groundCheckDistance = 0.05f;
            jumping = false;
        }
        else if (grounded)
        {
            unitRigidbody.velocity = new Vector3(inputVelocity.x, unitRigidbody.velocity.y, inputVelocity.z);

            if (wasGrounded)
                groundCheckDistance = baseGroundCheckDistance;
        }
        else if (groundCheckDistance < baseGroundCheckDistance)
            groundCheckDistance = unitRigidbody.velocity.y < 0 ? baseGroundCheckDistance : groundCheckDistance + 0.01f;

        CheckGroundStatus();
    }

    void ApplyInputVelocity()
    {
        if (grounded)
        {
            //movedirection
            inputVelocity = new Vector3((Input.GetMouseButton(1) ? Input.GetAxis("Horizontal") : 0), 0, Input.GetAxis("Vertical"));

            //L+R MouseButton Movement
            if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && (Input.GetAxis("Vertical") == 0))
                inputVelocity.z += 1;

            if (inputVelocity.z > 1)
                inputVelocity.z = 1;

            //Strafing move (like Q/E movement    
            inputVelocity.x -= Input.GetAxis("Strafing");

            // if moving forward and to the side at the same time, compensate for distance
            if (Input.GetMouseButton(1) && (Input.GetAxis("Horizontal") != 0) && (Input.GetAxis("Vertical") != 0))
            {
                inputVelocity *= 0.7f;
            }

            // Check roots and apply final move speed
            inputVelocity *= 7f;

            if (inputVelocity.x > 0 || inputVelocity.z > 0 || inputVelocity.x < 0 || inputVelocity.z < 0)
                ani.SetBool("SetMove", true);
            else
                ani.SetBool("SetMove", false);

            // Jump!
            if (Input.GetButton("Jump") || jumping)
            {
                jumping = true;
                inputVelocity.y = jumpSpeed;
                ani.SetBool("SetJump", true);
            }

            //transform direction
            inputVelocity = transform.TransformDirection(inputVelocity);

        }
        else
        {
            ani.SetBool("SetJump", false);
            inputVelocity = Vector3.zero;
            ApplyFlyingAnimations();
        }

    }

    void ApplyInputRotation()
    {
        if (Input.GetMouseButton(1))
        {
            transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            //transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);
        }
        else
        {
            Vector3 vec = new Vector3(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);
            transform.Rotate(vec);
        }
    }

    void CheckGroundStatus()
    {
        wasGrounded = grounded;
        RaycastHit hitInfo;

        if (Physics.Raycast(unitCollider.bounds.center, Vector3.down, out hitInfo, unitCollider.bounds.extents.y +
            baseGroundCheckDistance * 2, 1 << LayerMask.NameToLayer("Ground")))
        {
            var distanceToGround = hitInfo.distance;

            if (distanceToGround > unitCollider.bounds.extents.y + groundCheckDistance)
            {
                if (grounded && inputVelocity.y <= 0)
                {
                    unitRigidbody.AddForce(Vector3.down * unitRigidbody.velocity.magnitude, ForceMode.VelocityChange);
                    groundNormal = hitInfo.normal;
                    grounded = true;
                }
                else
                {
                    grounded = false;
                    groundNormal = Vector3.up;
                }
            }
            else
            {
                groundNormal = hitInfo.normal;
                grounded = true;
            }
        }
        else
        {
            grounded = false;
            groundNormal = Vector3.up;
        }

        IsGrounded = grounded;
    }

    void ApplyFlyingAnimations()
    {
        movementStatus = MovementStatus.Jumping;

    }


}
