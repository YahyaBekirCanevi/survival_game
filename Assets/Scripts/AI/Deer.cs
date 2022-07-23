using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Damagable))]
public class Deer : AI
{
    [SerializeField] private float escapeDistance;
    private Transform player;
    private Damagable health;
    protected override void Start()
    {
        base.Start();
        health = GetComponent<Damagable>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    protected override void States()
    {
        state = health.canDestroyed ? State.Death : (health.damaged ? State.Escape : state);
        switch (state)
        {
            case State.Idle:
                {
                    Idle(State.Eat, 3, "Eat");
                    break;
                }
            case State.Walk:
                {
                    Walk();
                    break;
                }
            case State.Death:
                {
                    Death();
                    break;
                }
            case State.Eat:
                {
                    anim.SetBool("Eat", true);
                    state = State.Idle;
                    break;
                }
            case State.Patrol:
                {
                    agent.destination = NextDestination(transform.position, escapeDistance, patrolDistance);
                    state = State.Walk;
                    break;
                }
            case State.Escape:
                {
                    count = 0;
                    agent.destination = NextDestination((transform.position - player.position).normalized * escapeDistance, 1, escapeDistance);
                    state = State.Walk;
                    break;
                }
            default: break;
        }
    }
    protected void Death()
    {
        agent.destination = transform.position;
        health.timeToDestroy = 5f;
        GetComponent<Ragdoll>().OnRagdoll(true);
    }
}