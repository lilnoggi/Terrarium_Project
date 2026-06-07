using UnityEngine;

public class AntBrain : MonoBehaviour
{
    // --- STATE MACHINE ---
    public enum AntState { Idle, Walking, Digging, GoingToSurface, Foraging, ReturningToNest, Stressed }

    [Header("Current Status")]
    [SerializeField] private AntState _currentState = AntState.Idle;
    [SerializeField] private AntSpeciesData _antSpeciesData;

    // LINK UP LATER
    [SerializeField] private AntMovement _movement;
    // private AntAnimation _antAnim;
    // private FormicariumManager _formicarium;

    private float _stateTimer = 0f;

    // -------------------------------------------------------------------------------------------------------

    private void Start()
    {
        // Start in the Idle state
        ChangeState(AntState.Idle);
    }

    private void Update()
    {
        // For now, just a simple state machine with no transitions
        switch (_currentState)
        {
            case AntState.Idle:
                HandleIdleState();
                break;

            case AntState.Walking:
                HandleWalkingState();
                break;

            case AntState.Digging:
                HandleDiggingState();
                break;

            case AntState.GoingToSurface:
                HandleGoingToSurfaceState();
                break;

            case AntState.Foraging:
                HandleForagingState();
                break;

            case AntState.ReturningToNest:
                HandleReturningToNestState();
                break;

            case AntState.Stressed:
                HandleStressedState();
                break;
        }
    }

    // A clean way to switch behaviours
    public void ChangeState(AntState newState)
    {
        _currentState = newState;

        // Give the ant a set amount of time to perform this new action
        if (_currentState == AntState.Idle)
        {
            _stateTimer = Random.Range(1f, 3f); // Stand still for 1 to 3 seconds
        }
        else if (_currentState == AntState.Walking)
        {
            _stateTimer = Random.Range(2f, 5f);
        }

        Debug.Log($"{gameObject.name} changed to state: {_currentState}");
    }

    // --- BEHAVIOUR HANDLERS ---
    private void HandleIdleState()
    {
        _movement.StopMoving();

        // Count down the timer
        _stateTimer -= Time.deltaTime;

        // 1% chance every frame to start wandering
        if (_stateTimer <= 0f)
        {
            // 50% chance to turn around or keep facing original direction
            if (Random.value > 0.5f)
            {
                _movement.FlipSprite();
            }
            
            ChangeState(AntState.Walking);
        }
    }

    private void HandleWalkingState()
    {
        // Tell the legs to Walk
        _movement.SurfaceWander();

        // Count down the timer
        _stateTimer -= Time.deltaTime;

        // 0.5% chance every frame to get bored and stop
        if (_stateTimer <= 0f)
        {
            ChangeState(AntState.Idle);
        }
    }

    private void HandleDiggingState()
    {
        // Implement digging logic
    }

    private void HandleGoingToSurfaceState()
    {
        // Implement logic for going to the surface
    }

    private void HandleForagingState()
    {
        // Implement foraging logic
    }

    private void HandleReturningToNestState()
    {
        // Implement logic for returning to the nest
    }

    private void HandleStressedState()
    {
        // Implement logic for when the ant is stressed
    }
}
