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

    public Sprite BrickTile;
    public Sprite tileOutlineSprite;

    [Header("Terrain Controls")]
    public int worldSize;
    public float surfaceValue = 0.2f;
    public float caveFreq = 0.05f;
    public float terrainFreq = 0.05f;
    public float seed;
    public Texture2D noiseTexture;

    public float heightMultiplier = 15f;

    public float heightAddition = 25f;

    public LayerMask isGround;

    private bool shouldPlace;

    private int interactionRange = 12;

    public Transform playerPosition;

    GameObject hoverTile;

    void Start()
    {
        Debug.ClearDeveloperConsole();
        seed = UnityEngine.Random.Range(-10000, 10000);
        GenerateNoiseTexture();
        GenerateTerrain();

        hoverTile = new GameObject(name = "tileOutline");
        hoverTile.transform.parent = this.transform;
        hoverTile.AddComponent<SpriteRenderer>();
        hoverTile.GetComponent<SpriteRenderer>().sprite = tileOutlineSprite;
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


                SpawnBackground(BackgroundTile, x, y);

                if (y == height - 1 && shouldPlace)
                    SpawnObject(GrassTile, x, y);


                else if (y == height - 2 && shouldPlace)
                    SpawnObject(DirtTile, x, y);


                else
                {
                    if (shouldPlace)
                        SpawnObject(StoneTile, x, y);


                    if (noiseTexture.GetPixel(x, y).r > surfaceValue)
                        SpawnBackground(BackgroundTile, x, y);
                }
            }
        }
    }

    void SpawnObject(Sprite _object, float x, float y)
    {
        GameObject newTile = new GameObject(name = "tile");
        newTile.transform.parent = this.transform;
        newTile.transform.position = new Vector2((int)x + 0.5f, (int)y + 0.5f);
        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = _object;
        newTile.AddComponent<BoxCollider2D>();
        newTile.layer = 7;
        newTile.tag = "World";
    }

    void SpawnBackground(Sprite _object, int x, int y)
    {
        GameObject newTile = new GameObject(name = "backgroundTile");
        newTile.transform.parent = this.transform;
        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = _object;
        newTile.transform.position = new Vector2((int)x + 0.5f, (int)y + 0.5f);
        newTile.layer = 7;
        newTile.tag = "World";
    }

    bool hover;

    void SelectedTile()
    {
        if (hover)
            hoverTile.SetActive(true);
        
        else
            hoverTile.SetActive(false);
    }

    bool isWithinRange(Vector3 playerPos, Vector3 mousePos)
    {
        if (Vector3.Distance(playerPos, mousePos) < interactionRange)
            return true;
        
        return false;
    }

    void Update()
    {
        SelectedTile();

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = new Vector3(playerPosition.position.x, playerPosition.position.y);

        if (hit.collider == null)
        {
            hover = false;
            if (Input.GetMouseButtonDown(1) && isWithinRange(playerPos, mousePos))
                SpawnObject(BrickTile, Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));
            
        }

        else if (hit.collider.gameObject.tag == "World")
        {
            if (isWithinRange(playerPos, mousePos))
            {
                hover = true;
                hoverTile.transform.position = new Vector2(Mathf.FloorToInt(mousePos.x) + 0.5f, Mathf.FloorToInt(mousePos.y) + 0.5f);

                if (Input.GetMouseButtonDown(0))
                    Destroy(hit.collider.gameObject);
                
            }
            else
                hover = false;
        }
        else
            hover = false;
        
    }

}
