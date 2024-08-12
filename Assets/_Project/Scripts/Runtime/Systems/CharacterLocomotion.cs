using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    private Animator animator;
    private Vector2 input;
    private CharacterController characterController;

    private float movementSpeed;

    private bool isRunning;

    public float incrementSpeed;

    [Header("Jump")]
    public float jumpHeight;
    public float gravity;
    public float stepDown = 0.3f;
    public bool isJumping;
    public float airControl = 2.5f;
    public float groundSpeed;
    public float jumpDamp = 0.5f;

    private Vector3 rootMotion;

    private Vector3 velocity;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        isRunning = true;
    }

    private void Update()
    {
        //trocar para o new input e testar com mobile e navmesh(talvez seja melhor um analogico)

        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = false;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (isRunning && movementSpeed < 1)
        {
            movementSpeed += incrementSpeed * Time.deltaTime;
        }
        else if (!isRunning && movementSpeed > 0)
        {
            movementSpeed -= incrementSpeed * Time.deltaTime;
        }

        animator.SetFloat("InputX", input.x);
        animator.SetFloat("InputY", input.y);
        animator.SetFloat("MovementSpeed", movementSpeed);

       // animator.SetFloat("Velocity", velocity.y);
    }

    private void FixedUpdate()
    {
        if (isJumping) // quando pula
        {
            UpdateInAir();
        }
        else
        {
            UpdateOnGround();
        }
    }

    private void Jump()
    {
        if (!isJumping)
        {
            float jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
            SetInAir(jumpVelocity); 
        }
    }

    private void SetInAir(float jumpVelocity)
    {
        isJumping = true;
        velocity = animator.velocity * jumpDamp * groundSpeed;
        velocity.y = jumpVelocity;
        animator.SetBool("isJumping", true);

        // fazer o blend tree de jump/fall

    }

    private void UpdateOnGround()
    {
        Vector3 stepForwardAmount = rootMotion * groundSpeed;
        Vector3 stepDownAmount = Vector3.down * stepDown;
        characterController.Move(stepForwardAmount + stepDownAmount);
        rootMotion = Vector3.zero;

        if (!characterController.isGrounded)
        {
            SetInAir(0);
        }
    }

    private void UpdateInAir()
    {
        velocity.y -= gravity * Time.fixedDeltaTime;
        Vector3 displacement = velocity * Time.fixedDeltaTime;
        displacement += CalculateInAirControl();
        characterController.Move(displacement);
        isJumping = !characterController.isGrounded;
        rootMotion = Vector3.zero;
        animator.SetBool("isJumping", isJumping);
    }

    private Vector3 CalculateInAirControl()
    {
        return ((transform.forward * input.y)
            + (transform.right * input.x))
            * (airControl / 100);
    }


    private void OnAnimatorMove()
    {
        rootMotion = animator.deltaPosition;
    }
}
