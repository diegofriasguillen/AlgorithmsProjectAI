using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public float speed = 3.0f;
    public float detectionRange = 5.0f;
    public Transform player;
    private Node currentNode;
    private Node targetNode;
    private bool isFleeing = false;
    private Rigidbody rb;
    private Queue<Node> fleePath = new Queue<Node>();
    private Queue<Node> movePath = new Queue<Node>();
    private float recalculatePathTime = 10f;  
    private float nextRecalculateTime = 0f;   

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentNode = FindClosestNode(transform.position);
        BuildMovePath();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (!isFleeing)
            {
                isFleeing = true;
                BuildFleePath();
            }
        }
        else
        {
            isFleeing = false;
            if (movePath.Count == 0 || Time.time >= nextRecalculateTime)
            {
                BuildMovePath();
                nextRecalculateTime = Time.time + recalculatePathTime;
            }
        }

        MoveTowardsTarget();
    }

    void MoveTowardsTarget()
    {
        if (targetNode != null)
        {
            Vector3 direction = (targetNode.transform.position - transform.position).normalized;
            rb.velocity = direction * speed;

            if (Vector3.Distance(transform.position, targetNode.transform.position) < 0.1f)
            {
                if (isFleeing && fleePath.Count > 0)
                {
                    targetNode = fleePath.Dequeue();
                }
                else if (!isFleeing && movePath.Count > 0)
                {
                    targetNode = movePath.Dequeue();
                }
            }
        }
    }

    Node FindClosestNode(Vector3 position)
    {
        Node[] nodes = FindObjectsOfType<Node>();
        float minDistance = float.MaxValue;
        Node closestNode = null;

        foreach (Node node in nodes)
        {
            float distance = Vector3.Distance(position, node.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestNode = node;
            }
        }
        return closestNode;
    }

    //builds the path to move 
    void BuildMovePath()
    {
        movePath.Clear();
        Node startNode = currentNode;
        for (int i = 0; i < 10; i++)  
        {
            Node nextNode = GetRandomNeighbor(startNode);
            if (nextNode != null)
            {
                movePath.Enqueue(nextNode);
                startNode = nextNode;
            }
            else
            {
                break;
            }
        }

        if (movePath.Count > 0)
        {
            targetNode = movePath.Dequeue();
        }
    }

    //the same but this time flee 
    void BuildFleePath()
    {
        fleePath.Clear();
        Node startNode = currentNode;
        for (int i = 0; i < 10; i++)  
        {
            Node nextNode = GetFleeTargetNode(startNode);
            if (nextNode != null)
            {
                fleePath.Enqueue(nextNode);
                startNode = nextNode;
            }
            else
            {
                break;
            }
        }

        if (fleePath.Count > 0)
        {
            targetNode = fleePath.Dequeue();
        }
    }

    Node GetRandomNeighbor(Node fromNode)
    {
        if (fromNode.neighbors.Count > 0)
        {
            return fromNode.neighbors[Random.Range(0, fromNode.neighbors.Count)];
        }
        return null;
    }

    Node GetFleeTargetNode(Node fromNode)
    {
        Node bestNode = null;
        float maxDistance = 0;

        //go to the far node
        foreach (var neighbor in fromNode.neighbors)
        {
            float distanceToPlayer = Vector3.Distance(neighbor.transform.position, player.position);
            if (distanceToPlayer > maxDistance)
            {
                maxDistance = distanceToPlayer;
                bestNode = neighbor;
            }
        }

        return bestNode ?? currentNode;  
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
