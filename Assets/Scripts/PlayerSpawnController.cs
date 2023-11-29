using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawnController : MonoBehaviour
{
    public ProceduralGeneration generation;

    Vector3 spawnPos;

    void Start()
    {
        spawnPos = new Vector2(generation.worldSize / 2, 110);
        transform.position = spawnPos;
    }
}
