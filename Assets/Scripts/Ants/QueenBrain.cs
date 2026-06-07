using UnityEngine;

public class QueenBrain : MonoBehaviour
{
    public enum QueenState { WanderingToFindSpot, DiggingEntrance, DiggingChamber, LayingEggs }
    
    [Header("Current Status")]
    [SerializeField] private QueenState _currentState = QueenState.WanderingToFindSpot;

    [Header("Data & References")]
    [SerializeField] private AntSpeciesData _antSpeciesData;
    [SerializeField] private AntMovement _movement;

    private float _stateTimer = 0f;
    private int _shaftDepthDug = 0;
    private int _targetShaftDepth = 4; // How many blocks straight down

    private void Start()
    {
        ChangeState(QueenState.WanderingToFindSpot);
    }

    private void Update()
    {
        switch (_currentState)
        {
            case QueenState.WanderingToFindSpot:
                HandleWandering();
                break;
            case QueenState.DiggingEntrance:
                HandleDiggingEntrance();
                break;
            case QueenState.DiggingChamber:
                HandleDiggingChamber();
                break;
            case QueenState.LayingEggs:
                // ADD LATER
                break;
        }
    }

    public void ChangeState(QueenState newState)
    {
        _currentState = newState;
        Debug.Log($"Queen changed to state: {_currentState}");

        if (_currentState == QueenState.WanderingToFindSpot)
        {
            // Walk for 2 to 4 seconds looking for a spot
            _stateTimer = Random.Range(2f, 4f); 
        }
        else if (_currentState == QueenState.DiggingEntrance)
        {
            // Stop moving. Start the digging timer based on the species data
            _movement.StopMoving();

            // Rotate her to face down the hole
            _movement.SetVerticalPosture(true);

            _stateTimer = _antSpeciesData != null ? _antSpeciesData.TunnelDigDelay : 1f;
        }
        else if (_currentState == QueenState.DiggingChamber)
        {
            // Stand back up normally
            _movement.SetVerticalPosture(false);
            // Chamber takes longer to dig out
            _stateTimer = (_antSpeciesData != null ? _antSpeciesData.TunnelDigDelay : 1f) * 2f;
        }
    }

    private void HandleWandering()
    {
        _movement.SurfaceWander();
        _stateTimer -= Time.deltaTime;

        if (_stateTimer <= 0f)
        {
            _movement.StopMoving();

            // A spot is found! Dig down
            ChangeState(QueenState.DiggingEntrance);
        }
    }

    private void HandleDiggingEntrance()
    {
        // Crawl down the shaft while waiting for the dig timer
        _movement.ClimbDownwards();
        
        _stateTimer -= Time.deltaTime;

        if (_stateTimer <= 0f)
        {
            // Find the cell slightly below the Queen's feet
            Vector3 targetWorldPos = transform.position + (Vector3.down * 0.5f);
            Vector3Int cellBelow = EnvironmentManager.Instance.GetDirtTilemap().WorldToCell(targetWorldPos);

            // Destroy the dirt (Radius 0 = exactly 1 block)
            EnvironmentManager.Instance.RemoveDirt(cellBelow, 0);

            // Keep track of how deep
            _shaftDepthDug++;

            // Reached the bottom?
            if (_shaftDepthDug >= _targetShaftDepth)
            {
                ChangeState(QueenState.DiggingChamber);
            }
            else
            {
                // Reset timer for the next block
                _stateTimer = _antSpeciesData != null ? _antSpeciesData.TunnelDigDelay : 1f;
            }
        }
    }

    private void HandleDiggingChamber()
    {
        _stateTimer -= Time.deltaTime;

        if (_stateTimer <= 0f)
        {
            // Get the center cell where the queen is currently standing
            Vector3Int centerCell = EnvironmentManager.Instance.GetDirtTilemap().WorldToCell(transform.position);

            // Dig out the room (Radius 1 = 3x3 block area)
            // SWAP TO USE _antSpeciesData.ChamberSize later
            EnvironmentManager.Instance.RemoveDirt(centerCell, 1);

            // Lay eggs
            ChangeState(QueenState.LayingEggs);
        }
    }
}
