using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCBody : MonoBehaviour
{
    [SerializeField] private List<Limb> limbs;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private ParticleSystem bloodParticles;
    [SerializeField] private float explosionForce;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionUpwardsModifier;
    [SerializeField] private float maxWaitTime = 5f;
    [SerializeField] private float scaredDistance = 10f;
    [SerializeField] private float walkingSpeed = 3f;
    [SerializeField] private float runningSpeed = 7f;
    private float idleTimer;
    private float waitTime;
    private PlayerController player;

    public Action<PlayerController> Detach;
    public enum NPCState {
        Idle,Walking, Running 
    }
    private NPCState state = NPCState.Idle;
    private void Start()
    {
        waitTime = UnityEngine.Random.Range(0, maxWaitTime);
        player = FindFirstObjectByType<PlayerController>();
    }
    private void Update()
    {
        
        switch (state)
        {
            case(NPCState.Idle):
                agent.autoBraking = true;
                if (Vector3.Distance(transform.position, player.transform.position) <= scaredDistance)
                {
                    RunFrom(player.transform);
                    idleTimer = 0;
                    state = NPCState.Running;
                    Debug.Log(state);
                }
                if (idleTimer >= waitTime) {
                    SetValidPath();
                    idleTimer = 0;
                    state = NPCState.Walking;
                    Debug.Log(state);
                }
                else
                    idleTimer += Time.deltaTime;
                break;
            case (NPCState.Walking):
                agent.speed = walkingSpeed;
                agent.autoBraking = true;
                if (Vector3.Distance(transform.position, player.transform.position) <= scaredDistance)
                {
                    RunFrom(player.transform);
                    state = NPCState.Running;
                    Debug.Log(state);
                }
                CheckToIdle();

                break;
            case (NPCState.Running):
                agent.speed = runningSpeed;
                agent.autoBraking = false;
                Vector3 currentPos = transform.position;
                Vector3 destination = agent.destination;
                currentPos.y = 0;
                destination.y = 0;
                if (Vector3.Distance(currentPos, destination) <= 1f && Vector3.Distance(transform.position, player.transform.position) <= scaredDistance)
                    RunFrom(player.transform);
                else if (Vector3.Distance(transform.position, player.transform.position) > scaredDistance)
                    CheckToIdle();
                break;

        }
            
    }
    private void CheckToIdle()
    {
        Vector3 currentPos = transform.position;
        Vector3 destination = agent.destination;
        currentPos.y = 0;
        destination.y = 0;
        if (Vector3.Distance(currentPos, destination) <= 0.1f || agent.isPathStale)
        {
            waitTime = UnityEngine.Random.Range(0, maxWaitTime);

            state = NPCState.Idle;
            Debug.Log(state);
        }
    }
    private void RunFrom(Transform runAwayTarget)
    {
        Vector3 direction = transform.position - runAwayTarget.position;
        direction.Normalize();
        Vector3 newPosition = transform.position + direction * 2f;
        agent.SetDestination(newPosition);
        if(agent.isPathStale)
        {
            Debug.Log("Go right");
            agent.SetDestination(transform.right * 2f);
        }
        if (agent.isPathStale)
        {
            Debug.Log("Go left");
            agent.SetDestination(-transform.right * 2f);
        }
        if (agent.isPathStale)
        {
            SetValidPath();
        }
    }

    private void SetValidPath()
    {
        agent.SetDestination(WorldBounds.Instance.GetRand());
        while (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            agent.SetDestination(WorldBounds.Instance.GetRand());
        }
    }
    public void Explode(PlayerController player)
    {
        foreach (Limb limb in limbs)
        {
            limb.gameObject.SetActive(true);
            limb.transform.SetParent(null);
            limb.AddExplosionForce(player.transform.position, explosionForce,explosionRadius,explosionUpwardsModifier);
            Detach?.Invoke(player);
        }
        if (bloodParticles) 
        {
            bloodParticles.transform.SetParent(null);
            bloodParticles.Play();
        }
        Destroy(gameObject);
    }
}
