using System.Collections.Generic;
using UnityEngine;
using Coco.AI;
using Coco.AI.PathFinding;
using SotomaYorch.Game;
using Unity.VisualScripting;
using System.Linq;
using System.Runtime.ConstrainedExecution;

public class CoinsCollectorAStar : MonoBehaviour
{
    ScrCoin[] coins = null;
    [SerializeField]
    NodesList nodeController;
    AgentController agentController;
    [SerializeField]
    PathFinder<AStar> finder = null;
    [SerializeField]

    List<(Vector3 start, Vector3 end)> positions;

    private void Awake()
    {
        coins = FindObjectsOfType<ScrCoin>();
        agentController = GetComponent<AgentController>();

        finder = new PathFinder<AStar>(nodeController);
    }

    private void OnEnable()
    {
        agentController.OnTargetReached += SetNextTarget;
    }

    private void OnDisable()
    {
        agentController.OnTargetReached -= SetNextTarget;
    }

    private void Start()
    {
        //var final = FindObjectOfType<ScrVictoryGoalMechanic>();
        //FindBestRoute((transform.position, final.transform.position));
        GetCoinsPosition(transform.position);
        (Vector3 start, Vector3 end)[]  posArray;
            
        posArray = positions.ToArray();
        
        FindBestRoute(posArray);
        //finder.Find_BestRouteAsync(FindNewRoutes, positions.ToArray());
    }

    private void GetCoinsPosition(Vector3 start)
    {
        coins = FindObjectsOfType<ScrCoin>();
        positions = new();

        foreach (var coin in coins)
        {
            positions.Add((start, coin.transform.position));
        }
    }

    private void FindBestRoute(params (Vector3 start, Vector3 end)[] pairs)
    {
        finder.OnPathCalculated += SetNextTarget;
        finder.OnPathCalculated += () => FindNextRoute(finder.ShorstestNodeIdx);

        finder.Find_BestRoute(pairs);

        //finder.Find_BestRoute((finder.Start, finder.End));

        //destiny = route.end;

        //RemoveSearchedCoins(route.end);

        //SetNextTarget();
    }

    public void SetNextTarget()
    {
        Vector3? node = null;

        node = finder.GetNextNode();
        
        if (node.HasValue)
        {
            agentController.MoveTo(node.Value);
        }
        else
        {
            agentController.Stop();
            //finder.GoToEnd();
        }
    }

    private void FindNextRoute(int? idx)
    {
        if (!idx.HasValue || positions.Count <= 0) return;

        //finder.OnPathCalculated -= SetNextTarget;

        var newStart = positions[idx.Value].end;
        positions.RemoveAt(idx.Value);

        for (int i = 0; i < positions.Count; i++)
        {
            positions[i] = (newStart, positions[i].end);
        }

        if (positions.Count > 0)
            finder.Find_BestRoute(positions.ToArray());
        else
            finder.GoToEnd(newStart);
    }

    private void RemoveSearchedCoins(Vector3 nearPoint)
    {
        float dis = float.MaxValue;
        int idx = -1;

        for (int i = 0; i < positions.Count; i++)
        {
            if (Vector3.Distance(coins[i].transform.position, nearPoint) is var d && d < dis)
            {
                dis = d;
                idx = i;
            }
        }

        if (idx >= 0)
        {
            positions.RemoveAt(idx);
        }
    }

    private void OnDrawGizmos()
    {
        if (finder == null || finder.paths == null || finder.paths.Count <= 0) return;
        var ar = finder.paths[0].ToArray();
        
        for (int i = 0; i < ar.Length - 1; i++)
        {
            var cur = ar[i];
            Debug.DrawLine(cur.transform.position + Vector3.up * 2, ar[i + 1].transform.position + Vector3.up * 2, Color.black);

        }
    }
}
