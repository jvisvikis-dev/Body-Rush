using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCBody : MonoBehaviour
{
    [SerializeField] private List<Limb> limbs;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject targetDestination;
    [SerializeField] private ParticleSystem bloodParticles;
    [SerializeField] private float explosionForce;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionUpwardsModifier;

    public Action<PlayerController> Detach;
    private void Update()
    {
        agent.destination = targetDestination.transform.position;
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
