using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private void OnMouseUp()
    {
        Destroy(gameObject);
    }
}
