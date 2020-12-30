public interface ICollisionable : IEnemyCanCollide, IPlayerCanCollide
{

}
public interface IEnemyCanCollide
{
    bool OnEnemyCollision();
}

public interface IPlayerCanCollide
{
    void OnPlayerCollision();
}