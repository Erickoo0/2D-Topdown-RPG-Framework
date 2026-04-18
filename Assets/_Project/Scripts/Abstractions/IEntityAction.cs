
public interface IEntityAction
{
    void EnterAction(EntityController controller);
    void UpdateAction(EntityController controller);
    bool IsFinishedAction(EntityController context);
    void ExitAction(EntityController context);
}
