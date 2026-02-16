using UnityEngine;
using UnityEngine.InputSystem;

public class Dream2PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator bodyAnimator;
    [SerializeField] private Animator legsAnimator;

    
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 movement = moveInput * moveSpeed;
        rb.velocity = movement;
    }

    private void Update()
    {
        bool isMoving = moveInput.magnitude > 0.1f;
        bodyAnimator.SetBool("isMoving", isMoving);
        legsAnimator.SetBool("isMoving", isMoving);
    }

    private void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

    public Vector2 MoveInput()
    {
        return moveInput;
    }
}