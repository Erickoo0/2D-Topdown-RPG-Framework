using UnityEngine;

public abstract class EntityActionModule : MonoBehaviour, IEntityAction
{
    public abstract void Enter(EntityController context);
    public abstract void Update(EntityController context);
    public abstract bool IsFinished(EntityController context);
    public abstract void Exit(EntityController context);
}
