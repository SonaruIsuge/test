using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class test : MonoBehaviour
{
    [SerializeField]List<TestComponent> components = new List<TestComponent>();
    private void Start()
    {
        components.Add(new Move(20f, this));
        components.Add(new Attack(10f, this));
    }

    private void Update()
    {
        components.ForEach(c => c.Tick());
    }
}

abstract public class TestComponent
{
    protected test Parent { get; private set; }
    public TestComponent(test parent)
    {
        Parent = parent;
    }
    abstract public void Tick();
}



public class Move : TestComponent
{
    public float vel;
    public Move(float vel, test parent) : base(parent)
    {
        this.vel = vel;
    }

    public override void Tick()
    {
        // Debug.Log($"Move Tikc{vel}");
    }
}



public class Attack : TestComponent
{
    public float attack;

    public Attack(float attack, test parent) : base(parent)
    {
        this.attack = attack;
    }

    public override void Tick()
    {
        // Debug.Log($"Attack Tikc{attack}");
    }
}
