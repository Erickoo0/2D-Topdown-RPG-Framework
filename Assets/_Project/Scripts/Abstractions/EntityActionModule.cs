using UnityEngine;

public abstract class EntityActionModule : MonoBehaviour, IEntityAction
{
    public abstract void EnterAction(EntityController controller);
    public abstract void UpdateAction(EntityController controller);
    public abstract bool IsFinishedAction(EntityController context);
    public abstract void ExitAction(EntityController context);
}
