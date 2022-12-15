using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Plane : MonoBehaviour
{
    [SerializeField] Transform tile;
    [SerializeField] NavMeshAgent agent;
    public int width = 1;
    public int height = 1;

    float WidthOffset()
    {
        return -(width - 1) / 2.0f;
    }

    float DepthOffset()
    {
        return -(height - 1) / 2.0f;
    }

    private void Start()
    {
        Vector3 position = Vector3.zero;

        position.x = -(width - 1) / 2.0f;
        for (int x = 0; x < width; ++x)
        {
            position.z = -(height - 1) / 2.0f;
            for (int z = 0; z < height; ++z)
            {
                Transform newTile = Instantiate(tile, position, Quaternion.identity, transform);

                position.z++;
            }
            position.x++;
        }

        StartCoroutine(GenerateAgents());
    }

    private IEnumerator GenerateAgents()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Vector3 position = Vector3.zero;
            position.x = WidthOffset() + Random.Range(0.0f, width - 1);
            position.y = 1.0f;
            position.z = DepthOffset();
            NavMeshAgent newAgent = Instantiate<NavMeshAgent>(agent, position, Quaternion.identity, transform);
            position.z += height;
            newAgent.SetDestination(position);
        }
    }
}
