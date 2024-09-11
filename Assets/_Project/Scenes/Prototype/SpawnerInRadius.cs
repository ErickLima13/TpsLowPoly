using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerInRadius : MonoBehaviour
{
    public float radius;
    public GameObject cubeFab;
    public List<Transform> cubeList = new List<Transform>();
    public int countCube;

    public EnemyAI enemy;

    private void Start()
    {
        for (int i = 0; i < countCube; i++)
        {
            Vector3 randomPos = Random.onUnitSphere * radius;
            randomPos.y = transform.position.y;
            GameObject temp = Instantiate(cubeFab, randomPos, Quaternion.identity);
            temp.transform.parent = transform;
            temp.name = "Wp " + i;
            cubeList.Add(temp.transform);
        }

        if (cubeList.Count >= countCube)
        {
            cubeList.AddRange(enemy.waypoints.ToList());
            enemy.waypoints = cubeList.ToArray();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
