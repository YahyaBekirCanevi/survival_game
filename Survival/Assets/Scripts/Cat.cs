using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : AI
{
    [SerializeField] private float minDistance;
    [SerializeField] private float sleepDuration;
    protected override void States()
    {
        if ((state == State.Walk && Vector3.Distance(agent.destination, transform.position) < .1f) || state != State.Walk)
            switch (state)
            {
                case State.Idle:
                    {
                        Idle(State.Sleep, sleepDuration, "Sleep");
                        break;
                    }
                case State.Walk:
                    {
                        Walk();
                        break;
                    }
                case State.Sleep:
                    {
                        anim.SetBool("Sleep", true);
                        state = State.Idle;
                        break;
                    }
                case State.Patrol:
                    {
                        agent.destination = NextDestination(transform.position, minDistance, patrolDistance);
                        state = State.Walk;
                        break;
                    }
                default: break;
            }
    }
}
