
public interface IEntityAction
{
    void Enter(EntityController context);
    void Update(EntityController context);
    bool IsFinished(EntityController context);
    void Exit(EntityController context);
}
