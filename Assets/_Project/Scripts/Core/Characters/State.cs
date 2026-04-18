
public abstract class State
{
    protected StateMachine stateMachine; // Reference to the state machine
    
    // Constructor
    protected State(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    // Contracts that each state must fulfill
    public virtual void Enter(){}
    public virtual void Update(){}
    public virtual void PhysicsUpdate(){}
    public virtual void HandleInput() { }
    public virtual void Exit(){}
}

// Generic State
// Other classes which inherit from State<T> must define T, where T must inherit from a BaseEntityController
public abstract class State<T> : State where T : BaseEntityController
{
    // Specialized Reference holds the controller (PlayerController / EntityController)
    protected readonly T controller;

    // Constructor: Pass the specific controller and the stateMachine it belongs to
    protected State(T controller, StateMachine stateMachine) : base(stateMachine)
    {
        this.controller = controller;
    }
}