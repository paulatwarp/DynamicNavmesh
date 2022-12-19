using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Plane plane;

    private void OnMouseUp()
    {
        plane.RemoveFromNavMesh(gameObject);
        Destroy(gameObject);
    }
}
