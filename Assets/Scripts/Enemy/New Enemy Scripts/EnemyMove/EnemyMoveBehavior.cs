
abstract public class EnemyMoveBehavior
{
    protected Enemy parent;
    
    public EnemyMoveBehavior(Enemy parent)
    {
        this.parent = parent;
    }
    abstract public void Move();
}