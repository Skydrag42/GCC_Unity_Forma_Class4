using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EntitySpawner : MonoBehaviour
{
	public GameObject entityPrefab;
	public Transform entitesParent;
    public float spawnRate = .1f;

	// Be careful where you place your spawner, as sampling a point
	// on the navmesh with more than `samplingDistance` meters of difference between the 
	// spawner and the navmesh will not work correctly
	// https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
	public float spawnRadius = 5;
	private float samplingDistance = 1f;

	public float maxQuantity = 10f;

	private void Start()
	{
		StartCoroutine(SpawnEntity());
	}

	IEnumerator SpawnEntity()
	{
		NavMeshHit hit;
		Vector3 spawnPos;
		while (true)
		{
			yield return new WaitForSeconds(1 / spawnRate);
			if (entitesParent.childCount >= maxQuantity) continue;

			spawnPos = Random.insideUnitCircle * spawnRadius;
            // insideUnitCircle returns a vector2, so we need to switch some values
            spawnPos.z = spawnPos.y;
            spawnPos.y = 0;
            spawnPos += transform.position;
			if (NavMesh.SamplePosition(spawnPos, out hit, samplingDistance, NavMesh.AllAreas))
				spawnPos = hit.position;
			else spawnPos = transform.position;
			Instantiate(entityPrefab, spawnPos, Quaternion.identity, entitesParent);
		}
	}

}
