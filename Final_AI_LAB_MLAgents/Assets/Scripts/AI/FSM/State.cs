using UnityEngine;

public abstract class State
{
    protected ZombieAI zombie;

    public State(ZombieAI zombie)
    {
        this.zombie = zombie;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
