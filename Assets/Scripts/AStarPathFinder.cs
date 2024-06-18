using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AStarPathFinder : MonoBehaviour
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
            //no coins go goal go win babbbbby
            StartNode = currentNode;
            FindPath(FindClosestNode(Goal.position));
        }
    }

    //calculate the distance of the path to the objective node
    float CalculatePathDistance(Node targetNode)
    {
        var openSet = new List<Node> { currentNode };
        var gScore = new Dictionary<Node, float>(); //cost of the neariest way from the begin to every node
        var fScore = new Dictionary<Node, float>(); //cost of the entire way from the begining to the objective

        foreach (var node in FindObjectsOfType<Node>())
        {
            if (node.tag == "NodeAStar")
            {
                gScore[node] = float.MaxValue;
                fScore[node] = float.MaxValue;
            }
        }

        gScore[currentNode] = 0; //initial node cost 0 pesos
        fScore[currentNode] = HeuristicCostEstimate(currentNode, targetNode);

        // using a* to find the shortest path
        while (openSet.Count > 0)
        {
            Node current = GetLowestFScoreNode(openSet, fScore);//finds the neariest node 
            if (current == targetNode)
                return gScore[current]; //restart the cost 

            openSet.Remove(current);

            foreach (var neighbor in current.neighbors)
            {
                if (neighbor.tag == "NodeAStar")
                {
                    float tentativeGScore = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position); //check if there's not a better path
                    if (tentativeGScore < gScore[neighbor])
                    { 
                        //if finds a better path, update the path
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, targetNode);
                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }
        }

        return float.MaxValue;
    }

    float HeuristicCostEstimate(Node start, Node goal)
    {
        //manhattan distance, estimation of the cost from the initial node to the objective
        return Mathf.Abs(start.transform.position.x - goal.transform.position.x) + Mathf.Abs(start.transform.position.z - goal.transform.position.z);
    }

    Node GetLowestFScoreNode(List<Node> openSet, Dictionary<Node, float> fScore)
    {
        //finds the cheapest node
        Node lowest = openSet[0];
        float lowestFScore = fScore[lowest];

        foreach (var node in openSet)
        {
            if (fScore[node] < lowestFScore)
            {
                lowest = node;
                lowestFScore = fScore[node];
            }
        }

        return lowest;
    }

    //find the closest node tho the asigned position
    Node FindClosestNode(Vector3 position)
    {
        Node[] nodes = FindObjectsOfType<Node>();
        float minDistance = float.MaxValue;
        Node closestNode = null;

        foreach (Node node in nodes)
        {
            if (node.tag == "NodeAStar")
            {
                float distance = Vector3.Distance(position, node.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestNode = node;
                }
            }
        }
        return closestNode;
    }

    void FindPath(Node targetNode)
    {
        var openSet = new List<Node> { StartNode };
        var cameFrom = new Dictionary<Node, Node>();

        var gScore = new Dictionary<Node, float>();
        var fScore = new Dictionary<Node, float>();

        foreach (var node in FindObjectsOfType<Node>())
        {
            if (node.tag == "NodeAStar")
            {
                gScore[node] = float.MaxValue;
                fScore[node] = float.MaxValue;
            }
        }

        gScore[StartNode] = 0;
        fScore[StartNode] = HeuristicCostEstimate(StartNode, targetNode);

        //using a* to find the path
        while (openSet.Count > 0)
        {
            Node current = GetLowestFScoreNode(openSet, fScore);

            if (current == targetNode)
            {
                ReconstructPath(cameFrom, current);
                return;
            }

            openSet.Remove(current);

            foreach (var neighbor in current.neighbors)
            {
                if (neighbor.tag == "NodeAStar")
                {
                    float tentativeGScore = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                    //if there's a cheapest path, updates the path 
                    if (tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, targetNode);
                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }
        }
    }

    //rebuild the path from the objective to the begin
    void ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Node> totalPath = new List<Node> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        path = totalPath;
        StartCoroutine(FollowPath());
    }

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
            GameManagerAStar.Instance.ShowGameCompleteAStar();
        }
        else
        {
            FindNextTarget();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CoinAStar"))
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
