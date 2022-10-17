using System;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{

    [SerializeField] private LayerMask checkLayer = default;
    
    private BoxCollider[] BoxColliders => GetComponentsInChildren<BoxCollider>(true);

    public bool IsColliding()
    {
        if (BoxColliders.Length <= 0)
        {
            Debug.LogError($"CollisionCheck: {transform.name} Have no box colliders to check!");
            return false;
        }
            
        
        int maxColliders = 1;
        Collider[] hitColliders = new Collider[maxColliders];

        foreach (var boxCollider in BoxColliders)
        {
            var boxColliderTf = boxCollider.transform;
            var colliders = Physics.OverlapBoxNonAlloc(boxColliderTf.position + boxCollider.center, 
                 Vector3.Scale(boxCollider.size/2, boxColliderTf.lossyScale), hitColliders, 
                 boxColliderTf.rotation, checkLayer, QueryTriggerInteraction.UseGlobal);

            if (colliders > 0)
                return true;
        }

        return false;
    }

    public bool IsColliding(Vector3 position)
    {
        if (BoxColliders.Length <= 0)
        {
            Debug.LogError($"CollisionCheck: {transform.name} Have no box colliders to check!");
            return false;
        }
        
        int maxColliders = 1;
        Collider[] hitColliders = new Collider[maxColliders];

        foreach (var boxCollider in BoxColliders)
        {
            Transform boxColliderTf = boxCollider.transform;
            Vector3 overlapPosition = (boxColliderTf.position - transform.position + boxCollider.center)+position;
            
            int colliders = Physics.OverlapBoxNonAlloc(overlapPosition,
                Vector3.Scale(boxCollider.size/2, boxColliderTf.lossyScale), hitColliders, 
                boxColliderTf.rotation, checkLayer, QueryTriggerInteraction.UseGlobal);

            if (colliders > 0) return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var boxCollider in BoxColliders)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero + boxCollider.center, boxCollider.size);
        }
    }
}
