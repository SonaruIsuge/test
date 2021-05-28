
abstract public class EnemyAttackBehavior
{
    protected Enemy parent;

    public EnemyAttackBehavior(Enemy parent)
    {
        this.parent = parent;
    }

     abstract public void Attack();
}
