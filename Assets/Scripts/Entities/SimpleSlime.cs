using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SimpleSlime : MonoBehaviour
{
    NavMeshAgent agent;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (PlayerController.Instance != null)
            agent.SetDestination(PlayerController.Instance.transform.position);
    }
}
