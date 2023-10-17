using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGeneration : MonoBehaviour
{

    public Sprite GrassTile;
    public Sprite DirtTile;
    public Sprite StoneTile;



    public int worldSize;
    public float surfaceValue = 0.2f;
    public float caveFreq = 0.05f;
    public float terrainFreq = 0.05f;
    public float seed;
    public Texture2D noiseTexture;

    public float heightMultiplier = 15f;

    public float heightAddition = 25f;


    void Start()
    {
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
            int height = (int) (Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition);
            for (int y = 0; y < height; y++)
            {
                if (y == height-1)
                {
                    print("huh");
                    SpawnObject(GrassTile, x, y);
                }

                else if (y == height-2)
                {
                    SpawnObject(DirtTile, x, y);
                }

                else
                {
                    SpawnObject(StoneTile,x,y);
                }

            }
        }
    }


    void SpawnObject(Sprite _object, int x, int y)
    {
        if (noiseTexture.GetPixel(x, y).r < surfaceValue)
        {
            GameObject newTile = new GameObject(name = "tile");
            newTile.transform.parent = this.transform;
            newTile.AddComponent<SpriteRenderer>();
            newTile.GetComponent<SpriteRenderer>().sprite = _object;
            newTile.AddComponent<BoxCollider2D>();
            newTile.transform.position = new Vector2(x * 2, y * 2);
        }
    }
}
