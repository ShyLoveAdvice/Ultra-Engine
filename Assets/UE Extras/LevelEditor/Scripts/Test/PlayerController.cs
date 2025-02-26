using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D _rb;
    private bool _isGrounded;
    private float _moveInput;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Ground check
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Get movement input
        _moveInput = Input.GetAxisRaw("Horizontal");

        // Jump logic
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
        // Move the player
        _rb.velocity = new Vector2(_moveInput * moveSpeed, _rb.velocity.y);
    }
}

