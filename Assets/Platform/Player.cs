using System.Linq.Expressions;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] const float standMoveSpeed = 10.0f;
    [SerializeField] const float crouchMoveSpeed = 6.5f;
    [SerializeField] float moveSpeed = standMoveSpeed;
    [SerializeField] float jumpForce = 10.0f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float airResistance = 0.5f;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isCrouch;

    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    private Vector2 moveInput;

    private SpriteRenderer sr;
    private Transform looks;

    private PlayerInputActions playerInputActions;

    private Vector3 startPosition;

    void Awake()
    {
        startPosition = transform.position;
        playerInputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        looks = transform.Find("Looks");
        sr = transform.Find("Looks").GetComponent<SpriteRenderer>();
        cc = GetComponent<CapsuleCollider2D>();
    }

    void Start()
    {

    }

    void OnEnable()
    {
        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMove;
        playerInputActions.Player.Crouch.started += OnCrouch;
        playerInputActions.Player.Crouch.canceled += EndCrouch;
        playerInputActions.Player.Jump.performed += OnJump;
        playerInputActions.Player.Enable();
    }

    void OnDisable()
    {
        playerInputActions.Player.Move.performed -= OnMove;
        playerInputActions.Player.Move.canceled -= OnMove;
        playerInputActions.Player.Crouch.started -= OnCrouch;
        playerInputActions.Player.Crouch.canceled -= EndCrouch;
        playerInputActions.Player.Jump.performed -= OnJump;
        playerInputActions.Player.Disable();
    }


    void Update()
    {
        if (transform.position.y < -10.0f) 
        {
            Restart();
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y) ;

        if (!isGrounded)
        {
            velocity.x *= 1 - airResistance * Time.fixedDeltaTime;
            velocity.y += gravity * Time.fixedDeltaTime;
        }

        rb.linearVelocity = velocity;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void OnCrouch(InputAction.CallbackContext context)
    {
        cc.size = new Vector2(1,1.2f);
        cc.offset = new Vector2(0, -0.4f);
        looks.localScale = new Vector2(1.0f, 0.6f);
        moveSpeed = crouchMoveSpeed;
        
    }

    private void EndCrouch(InputAction.CallbackContext context)
    {
        cc.size = new Vector2(1,2);
        cc.offset = new Vector2(0, 0);
        looks.localScale = new Vector2(1.0f, 1.0f);
        moveSpeed = standMoveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Platform))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag(Tags.Trap))
        {
            Restart();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Platform)) 
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Collection))
        {
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag(Tags.Goal))
        {
            Debug.Log("GOAL!!");
        }
    }

    private void Restart()
    {
        transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
    }
}
