using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighbors;
    [HideInInspector] public Node prev;
    public float CostToReach;

    void OnDrawGizmos()
    {
        if (neighbors == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.color = Color.red;
        foreach (Node neighbor in neighbors)
        {
            if (neighbor != null)
            {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }
    }
}