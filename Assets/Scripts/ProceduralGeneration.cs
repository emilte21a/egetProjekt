using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class ProceduralGeneration : MonoBehaviour
{
    [Header("Tiles")]
    public Sprite GrassTile;
    public Sprite DirtTile;
    public Sprite StoneTile;

    public Sprite BackgroundTile;

    [Header("Terrain Controls")]
    public int worldSize = 200;
    public float surfaceValue = 0.2f;
    public float caveFreq = 0.05f;
    public float terrainFreq = 0.05f;
    public float seed;
    public Texture2D noiseTexture;
    public float heightMultiplier = 15f;
    public float heightAddition = 25f;
    public LayerMask isGround;

    [Range(0,20)]
    public int cullingDistance = 5;

    private bool shouldPlace;

    public Transform playerPos;

    [HideInInspector]
    public List<GameObject> GeneratedTiles;

    void Start()
    {
        GeneratedTiles = new();
        Debug.ClearDeveloperConsole();
        seed = UnityEngine.Random.Range(-10000, 10000);
        GenerateNoiseTexture();
        GenerateTerrain();
    }

    public void GenerateNoiseTexture()
    {
        noiseTexture = new Texture2D(worldSize, worldSize);

        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * caveFreq, (y + seed) * caveFreq);
                noiseTexture.SetPixel(x, y, new Color(v, v, v));
            }
        }
        noiseTexture.Apply();
    }

    public void GenerateTerrain()
    {
        for (int x = 0; x < worldSize; x++)
        {
            int height = (int)(Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition);
            for (int y = 0; y < height; y++)
            {
                if (noiseTexture.GetPixel(x, y).r < surfaceValue)
                    shouldPlace = true;

                else
                    shouldPlace = false;

                if (y == height - 1 && shouldPlace)
                    SpawnObject(GrassTile, x, y);


                else if (y == height - 2 && shouldPlace)
                    SpawnObject(DirtTile, x, y);

                else
                {
                    if (shouldPlace && y < height-2)
                    {
                        SpawnObject(StoneTile, x, y);
                        SpawnBackground(BackgroundTile, x, y);
                    }

                    if (noiseTexture.GetPixel(x, y).r >= surfaceValue)
                        SpawnBackground(BackgroundTile, x, y);
                }
            }
        }
    }
    public void SpawnObject(Sprite _object, float x, float y)
    {
        GameObject newTile = new GameObject(name = "tile");
        newTile.transform.parent = this.transform;
        newTile.transform.position = new Vector3((int)x + 0.5f, (int)y + 0.5f, 0f);
        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = _object;
        newTile.AddComponent<BoxCollider2D>();
        newTile.layer = 7;
        newTile.tag = "World";
        GeneratedTiles.Add(newTile);
    }

    void SpawnBackground(Sprite _object, float x, float y)
    {
        GameObject newTile = new GameObject(name = "backgroundTile");
        newTile.transform.parent = this.transform;
        newTile.transform.position = new Vector3((int)x + 0.5f, (int)y + 0.5f, 0.3f);
        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = _object;
        newTile.layer = 7;
        newTile.tag = "World";
        GeneratedTiles.Add(newTile);
    }

    // Vector3 lastpos;

    // private void Update() {

    //     if (new Vector3((int)playerPos.position.x, (int)playerPos.position.y) != lastpos)
    //     {
    //     FrustumCulling();   
    //     print("Player is moving");
    //     }
    //     else
    //     {
    //         print("Player is not moving");
    //     }

    //     lastpos = new Vector3((int)playerPos.position.x,(int)playerPos.position.y);
    // }

    

    // private void FrustumCulling(){
    //     foreach (var tile in GeneratedTiles)
    //     {
    //         if (tile.transform.position.x > (playerPos.position.x+cullingDistance) || tile.transform.position.x < (playerPos.position.x-cullingDistance))
    //             tile.SetActive(false);
            
    //         else
    //             tile.SetActive(true);
    //     }
    // }
}
