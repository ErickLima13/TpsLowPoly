using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollColliders : MonoBehaviour
{
    public List<Collider> colliders  = new();

    private void Start()
    {
        Collider[] rColliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in rColliders)
        {
            colliders.Add(collider);
            collider.enabled = false;
        }
    }

    public void ActiveRagDoll()
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
    }

}
