using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class CollectibleTile : Tile
{
    public GameObject spawn;

    private void Awake()
    {
        GameManager.Spawn(spawn, transform.GetPosition());
    }
}
