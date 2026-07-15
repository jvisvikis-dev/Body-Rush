using System;
using System.Collections;
using UnityEngine;

public class Limb : MonoBehaviour
{
    [SerializeField] private NPCBody body;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private int attachableLayer;
    [SerializeField] private float lerpTime;
    [SerializeField] private AnimationCurve lerpCurve;
    private bool attached = true;

    private void OnEnable()
    {
        body.Detach += Detach;
    }

    private void OnDisable()
    {
        body.Detach -= Detach;
    }

    private void Detach(PlayerController player)
    {
        StartCoroutine(WaitToAttachTo(1f, player.gameObject));
    }

    public void AddExplosionForce(Vector3 explosionPosition, float force, float explosionRadius, float upwardsModifier)
    {
        rigidbody.isKinematic = false;
        rigidbody.AddExplosionForce(force, explosionPosition, explosionRadius,upwardsModifier);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject);
        if (collision.gameObject.tag == "Player" && !attached)
        {
            transform.SetParent(collision.gameObject.transform.root);
            rigidbody.isKinematic = true;
            gameObject.tag = "Player";
            attached = true;
        }
    }

    private IEnumerator WaitToAttachTo(float waitTime, GameObject target)
    {
        yield return new WaitForSeconds(waitTime);
        attached = false;
        float timer = 0;
        Vector3 startPos = rigidbody.position;
        gameObject.layer = attachableLayer;
        while(timer < lerpTime && !attached)
        {
            timer += Time.deltaTime;
            rigidbody.position = Vector3.Slerp(startPos, target.transform.position, lerpCurve.Evaluate(timer / lerpTime));
            yield return null;
        }
    }

}
