using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public State CurrentState { get; private set; }
    
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

    public void Update()
    {
        CurrentState.HandleInput();
        CurrentState.Update();
    }

    public void FixedUpdate()
    {
        CurrentState.PhysicsUpdate();
    }
}
