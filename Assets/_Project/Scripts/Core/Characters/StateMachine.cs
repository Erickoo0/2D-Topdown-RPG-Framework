using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public State CurrentState { get; private set; }
    
    // Starts the initial state and calls it
    public void SetupState(State startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(State newState)
    {
        CurrentState.Exit(); 
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void UpdateState()
    {
        CurrentState.HandleInput();
        CurrentState.Update();
    }

    public void FixedUpdateState()
    {
        CurrentState.PhysicsUpdate();
    }
}
