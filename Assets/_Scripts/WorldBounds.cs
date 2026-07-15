using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WorldBounds : MonoBehaviour
{

    private static WorldBounds instance = null;
    public static WorldBounds Instance => instance;
    [SerializeField] private Vector3 size = new Vector3(10, 10, 10);
    // Start is called before the first frame update
    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, size);
    }
    public bool IsWithinBounds(Vector3 position, bool excludeY = true)
    {
        if(position.x < transform.position.x - size.x/2 || position.x > transform.position.x + size.x / 2)
            return false;
        if ((position.y < transform.position.y - size.y / 2 || position.y > transform.position.y + size.y / 2) && !excludeY)
            return false;
        if (position.z < transform.position.z - size.z / 2 || position.z > transform.position.z + size.y / 2)
            return false;
        return true;
    }
    public Vector3 GetRand()
    {
        return new Vector3(
            Random.Range(transform.position.x - size.x / 2, transform.position.x + size.x / 2),
            Random.Range(transform.position.y - size.y / 2, transform.position.y + size.y / 2),
            Random.Range(transform.position.z - size.z / 2, transform.position.z + size.z / 2)
        );
    }
}

