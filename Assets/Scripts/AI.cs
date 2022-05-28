using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public abstract class AI : MonoBehaviour
{
    [SerializeField] protected float patrolDistance;
    /** <summary>0 => Special ,  1 => walk,  2 => anything but 0 and 1</summary>*/
    [SerializeField] protected string orderOfState = "1210";
    protected float timer = 0;
    protected float wait = 1;
    protected int count = 0;
    protected Animator anim;
    protected NavMeshAgent agent;
    [SerializeField] protected State state;
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        state = State.Idle;
        timer = Time.time;
    }
    void Update()
    {
        anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
        if (state != State.Death)
            States();
    }
    protected abstract void States();
    protected void Idle(State specialState, float specialStateTime, string trigger)
    {
        agent.destination = transform.position;
        if (Time.time - timer <= wait)
            return;
        anim.SetBool(trigger, false);
        timer = Time.time;
        count %= orderOfState.Length;
        switch (orderOfState[count])
        {
            case '0':
                {
                    wait = specialStateTime;
                    state = specialState;
                    break;
                }
            case '1':
                {
                    wait = 1;
                    state = State.Patrol;
                    break;
                }
            default:
                {
                    wait = 3;
                    break;
                }
        }
        count++;
    }
    protected void Walk()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            float yrot = transform.rotation.eulerAngles.y;
            transform.up = Vector3.Lerp(transform.up, hit.normal, .25f);
            transform.Rotate(0, yrot, 0);
        }
        Vector3 des = agent.destination;
        des.y = transform.position.y;
        if (Vector3.Distance(transform.position, des) < .1f)
            state = State.Idle;
    }
    protected Vector3 NextDestination(Vector3 origin, float minRange, float maxRange)
    {
        Vector3 destination = Vector3.zero;
        while (true)
            if (NavMesh.SamplePosition(origin + UnityEngine.Random.insideUnitSphere * maxRange, out NavMeshHit hit, maxRange, 1) &&
            Vector3.Distance(hit.position, origin) >= minRange)
            {
                destination = hit.position;
                break;
            }
        return destination;
    }
    protected enum State { Idle, Eat, Patrol, Walk, Escape, Sleep, Death }
}