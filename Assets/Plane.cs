using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Plane : MonoBehaviour
{
    [SerializeField] Tile tile;
    [SerializeField] NavMeshAgent agent;

    public int width = 1;
    public int height = 1;
    Dictionary<GameObject, NavMeshBuildSource> lookup = new Dictionary<GameObject, NavMeshBuildSource>();
    Bounds bounds;
    NavMeshData navMesh;
    NavMeshDataInstance navMeshDataInstance;

    public void RemoveFromNavMesh(GameObject gameObject)
    {
        lookup.Remove(gameObject);
        UpdateNavMesh();
    }

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
        bounds = new Bounds();
        navMesh = new NavMeshData();
        navMeshDataInstance = NavMesh.AddNavMeshData(navMesh);

        Vector3 position = Vector3.zero;

        position.x = -(width - 1) / 2.0f;
        for (int x = 0; x < width; ++x)
        {
            position.z = -(height - 1) / 2.0f;
            for (int z = 0; z < height; ++z)
            {
                Tile newTile = Instantiate(tile, position, Quaternion.identity, transform);
                newTile.plane = this;
                MeshFilter meshFilter = newTile.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Renderer renderer = meshFilter.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                    AddToNavMesh(meshFilter);
                }

                position.z++;
            }
            position.x++;
        }

        UpdateNavMesh();

        StartCoroutine(GenerateAgents());
    }

    private void OnDestroy()
    {
        navMeshDataInstance.Remove();
    }

    private void AddToNavMesh(MeshFilter meshFilter)
    {
        var source = new NavMeshBuildSource();
        source.shape = NavMeshBuildSourceShape.Mesh;
        source.sourceObject = meshFilter.sharedMesh;
        source.transform = meshFilter.transform.localToWorldMatrix;
        source.area = 0;
        lookup.Add(meshFilter.gameObject, source);
    }

    private void UpdateNavMesh()
    {
        var sources = new List<NavMeshBuildSource>(lookup.Values);
        NavMeshBuildSettings navMeshBuildSettings = NavMesh.GetSettingsByID(0);
        NavMeshBuilder.UpdateNavMeshData(navMesh, navMeshBuildSettings, sources, bounds);
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
            StartCoroutine(DestroyOnDestinationReached(newAgent));
        }
    }

    IEnumerator DestroyOnDestinationReached(NavMeshAgent navMeshAgent)
    {
        yield return new WaitUntil(() => navMeshAgent.hasPath);
        yield return new WaitUntil(() => navMeshAgent.remainingDistance < 0.1f);
        Destroy(navMeshAgent.gameObject);
    }
}
