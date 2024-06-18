using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public Node StartNode;
    public List<GameObject> Coins;
    public Transform Goal;
    private Node currentNode;
    private CharacterController controller;
    public List<Node> path = new List<Node>();
    public float speed = 6f;
    public TextMeshProUGUI coinText;
    private int coinsCollected = 0;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentNode = FindClosestNode(transform.position);
        FindNextTarget();
        UpdateCoinText();
    }

    //finding the next coin based on the nodes
    void FindNextTarget()
    {
        if (Coins.Count > 0)
        {
            GameObject nextTarget = Coins[0];
            float minDistance = float.MaxValue;
            Node nextCoinNode = null;

            foreach (var coin in Coins)
            {
                Node coinNode = FindClosestNode(coin.transform.position);
                float distance = CalculatePathDistance(coinNode);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nextTarget = coin;
                    nextCoinNode = coinNode;
                }
            }

            if (nextCoinNode != null)
            {
                StartNode = currentNode;
                FindPath(nextCoinNode);
                Coins.Remove(nextTarget);
            }
        }
        else
        {
            //no coins go goal go win baby
            StartNode = currentNode;
            FindPath(FindClosestNode(Goal.position));
        }
    }

    //calculate the distance of the path to the objective node
    float CalculatePathDistance(Node targetNode)
    {
        var unvisited = new List<Node>(FindObjectsOfType<Node>());
        var distances = new Dictionary<Node, float>();
        foreach (var node in unvisited)
        {
            distances[node] = float.MaxValue;
        }
        distances[currentNode] = 0;

        while (unvisited.Count != 0)
        {
            Node current = null;
            float minDistance = float.MaxValue;
            foreach (var node in unvisited)
            {
                if (distances[node] < minDistance)
                {
                    minDistance = distances[node];
                    current = node;
                }
            }

            unvisited.Remove(current);

            foreach (var neighbor in current.neighbors)
            {
                float alt = distances[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                if (alt < distances[neighbor])
                {
                    distances[neighbor] = alt;
                }
            }
        }

        return distances[targetNode];
    }

    //finding the closest node to the asigned position
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

    
    //dijktras, find the way to the objective, based on the neariest way by the edges
    void FindPath(Node targetNode)
    {
        var unvisited = new List<Node>(FindObjectsOfType<Node>());
        var distances = new Dictionary<Node, float>();
        var previousNodes = new Dictionary<Node, Node>();

        foreach (var node in unvisited)
        {
            distances[node] = float.MaxValue;
            previousNodes[node] = null;
        }
        distances[StartNode] = 0;

        //look for the closest not visited node
        while (unvisited.Count != 0)
        {
            Node current = null;
            float minDistance = float.MaxValue;
            foreach (var node in unvisited)
            {
                if (distances[node] < minDistance)
                {
                    minDistance = distances[node];
                    current = node;
                }
            }

            if (current == targetNode)
                break;

            unvisited.Remove(current);

            foreach (var neighbor in current.neighbors)
            {
                if (neighbor.tag == "NodeDijkstra")
                {
                    float alt = distances[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                    if (alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previousNodes[neighbor] = current;
                    }
                }
            }
        }

        BuildPath(targetNode, previousNodes);
    }

    //build the path starting in the initial node and going to the objective node
    void BuildPath(Node targetNode, Dictionary<Node, Node> previousNodes)
    {
        path.Clear();
        Node current = targetNode;
        while (current != null)
        {
            path.Insert(0, current);
            current = previousNodes[current];
        }

        StartCoroutine(FollowPath());
    }

    //following the path node to node
    System.Collections.IEnumerator FollowPath()
    {
        foreach (Node node in path)
        {
            currentNode = node;
            while (Vector3.Distance(transform.position, node.transform.position) > 0.1f)
            {
                Vector3 moveDirection = Vector3.MoveTowards(transform.position, node.transform.position, Time.deltaTime * speed) - transform.position;
                controller.Move(moveDirection);
                yield return null;
            }
        }

        if (Coins.Count == 0 && Vector3.Distance(transform.position, Goal.position) < 1f)
        {
            GameManagerDijkstra.Instance.ShowGameCompleteDijkstra();
        }
        else
        {
            FindNextTarget();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CoinDijkstra"))
        {
            Coins.Remove(other.gameObject);
            Destroy(other.gameObject);
            coinsCollected++;
            UpdateCoinText();
        }
    }

    void UpdateCoinText()
    {
        coinText.text = "COINS COLLECTED: " + coinsCollected;
    }

}
