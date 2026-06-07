using UnityEngine;

public class AntMovement : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] private float _moveSpeed = 0.5f;
    [SerializeField] private bool _isFacingLeft = true;

    [Header("Sensors")]
    [SerializeField] private Transform _groundCheck; // Point near the front legs
    [SerializeField] private Transform _wallCheck; // Point at the face pointing forward
    [SerializeField] private LayerMask _dirtLayer; 

    private Rigidbody2D _rb;
    private float _flipCooldown = 0f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Count down the flip cooldown if it's above 0
        if (_flipCooldown > 0)
        {
            _flipCooldown -= Time.deltaTime;
        }
    }

    // The Brain will call this method when it wants the ant to wander
    public void SurfaceWander()
    {
        float direction = _isFacingLeft ? -1f : 1f;

        // --- FAKE GRAVITY ---
        // Is the ant standing on anything?
        bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, _dirtLayer);

        if (!isGrounded)
        {
            // Keep forward momentum while falling
            _rb.linearVelocity = new Vector2(direction * _moveSpeed, -2f);
            return;
        }

        // --- WALKING ---
        // Move the ant forward
        _rb.linearVelocity = new Vector2(direction * _moveSpeed, _rb.linearVelocity.y);

        // --- STEPPING UP & WALL DETECTION ---
        Vector2 faceDirection = _isFacingLeft ? Vector2.left : Vector2.right;
        
        // Shoot lasers to check the Environment
        // Laser 1: At face level
        bool isHittingWall = Physics2D.Raycast(_wallCheck.position, faceDirection, 0.1f, _dirtLayer);
        
        // Laser 2: At head level (to see if wall is short enough to climb)
        Vector3 headPosition = _wallCheck.position + new Vector3(0, 0.5f, 0);
        bool isHittingHead = Physics2D.Raycast(headPosition, faceDirection, 0.1f, _dirtLayer);

        if (isHittingWall)
        {
            if (!isHittingHead)
            {
                // It's a 1-block step, add a vertical "hop" to step up onto it
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 2f);
            }
            else if (_flipCooldown <= 0)
            {
                // Both lasers hit something, turn around
                FlipSprite();
            }
        }
    }

    public void StopMoving()
    {
        _rb.linearVelocity = new Vector2(0, 0);
    }

    // --- POSTURE CONTROLS ---
    public void SetVerticalPosture(bool isVertical)
    {
        if (isVertical)
        {
            // Point the ants "nose" straight down into the hole
            // If the ant is facing left, rotate 90 degrees. if right -90 degrees
            float zRotation = _isFacingLeft ? 90f : -90f;
            transform.rotation = Quaternion.Euler(0, 0, zRotation);
        }
        else
        {
            // Stand back up normally
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // --- MANUAL GRAVITY / CLIMBING ---
    public void ClimbDownwards()
    {
        // Is there ground touhcing the ants feet/face
        bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, _dirtLayer);

        if (!isGrounded)
        {
            // Slowly crawl/fall down the shaft
            _rb.linearVelocity = new Vector2(0, -2f);
        }
        else
        {
            // Stop moving if ant hits the bottom of the current hole
            StopMoving();
        }
    }

    // The Brain calls this
    public void FlipSprite()
    {
        _isFacingLeft = !_isFacingLeft;

        // Flips the entire ant object horizontally
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;

        // Start the cooldown so the ant can't flip again for half a second
        _flipCooldown = 0.5f;
    }

    // --- DEBUGGING ---
    private void OnDrawGizmos()
    {
        // Draw the Gravity Sensor (Red Line) 
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.down * 0.8f);

        // Draw the Ground/Edge Check (Green Line)
        if (_groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(_groundCheck.position, Vector2.down * 0.5f);
        }      

        if (_wallCheck != null)
        {
            // Draw the Wall Check (Blue Line)
            Gizmos.color = Color.blue;
            Vector2 faceDirection = _isFacingLeft ? Vector2.left : Vector2.right;
            Gizmos.DrawRay(_wallCheck.position, faceDirection * 0.1f);

            // Draw the Wall Check (Yellow Line)
            Gizmos.color = Color.yellow;
            Vector2 headPosition = _wallCheck.position + new Vector3(0, 0.5f, 0);
            Gizmos.DrawRay(headPosition, faceDirection * 0.1f);
        }
    }
}
