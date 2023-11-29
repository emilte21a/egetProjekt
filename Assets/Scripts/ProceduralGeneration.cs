using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class ProceduralGeneration : MonoBehaviour
{
    //Variabler för olika Tiles
    [Header("Tiles")]
    public Tilemap grid;

    public RuleTile grassTile;
    public RuleTile dirtTile;
    public RuleTile stoneTile;
    public RuleTile brickTile;
    public Sprite BackgroundTile;

    //Variabler som ändrar formen av terrängen
    [Header("Terrain Controls")]
    public int worldSize = 200;

    //Bestämmer en tröskel som bestämmer om ett block bör placeras på en viss position
    public float surfaceThreshold = 0.5f;

    //Bidrar till att skapa grottor i terrängen. Högre värden innebär mindre grottor medans lägre värden innebär större grottor
    public float caveFreq = 0.05f;

    //Bestämmer dynamiken för hur terrängen genereras. Högre värden innebär djupa dalar och höga berg medans lägre värden innebär "mjukare" terräng
    public float terrainFreq = 0.05f;

    //Bestämmer hur terrängen ska se ut. Olika seed världen innebär olika världar
    public float seed;

    public Texture2D noiseTexture;

    //bidrar till att skapa fler dalar och berg desto högre värdet är
    public float heightMultiplier = 15f;

    //Höjer endast terrängen med den mängd som anges
    public float heightAddition = 25f;

    //En layermask för att kolla om något är en del av marken
    public LayerMask isGround;

    //En bool för om en tile bör placeras
    private bool shouldPlace;

    //En lista som lagrar alla tiles i världen
    [HideInInspector]
    public List<RuleTile> GeneratedTiles;

    void Start()
    {
        //Instansiera listan GeneratedTiles
        GeneratedTiles = new();

        //Rensa konsollen
        Debug.ClearDeveloperConsole();

        //Skapa ett random seed. Detta slumpar hur barnan ska se ut
        seed = UnityEngine.Random.Range(-10000, 10000);

        //Skapa en noise texture
        GenerateNoiseTexture();

        //Generera terrängen
        GenerateTerrain();
    }

    //Metod för att generera en noise texture med perlinnoise
    public void GenerateNoiseTexture()
    {
        noiseTexture = new Texture2D(worldSize, worldSize); //Skapa en ny texture2D med bredden och höjden worldsize

        for (int x = 0; x < noiseTexture.width; x++) //Kolla igenom alla världen från 0 till noiseTextures bredd och höjd
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {

                //Skapa en float som använder sig av perlinnoise för varje pixel. V bestämmer en slumpmässig gråskala för varje pixel på bilden
                float v = Mathf.PerlinNoise((x + seed) * caveFreq, (y + seed) * caveFreq);

                //Sätt färgen på pixeln med positionen X och Y till en ny färg med värdena V
                noiseTexture.SetPixel(x, y, new Color(v, v, v));

            }
        }
        noiseTexture.Apply(); //Applicera föregående ändringar när texturen har genererats
    }

    //Metod för att generera terrängen i spelet
    public void GenerateTerrain()
    {
        //Loopa genom varje x och y värde i terrängen
        for (int x = 0; x < worldSize; x++)
        {
            int height = (int)(Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition);
            //Räkna ut höjden för varje x värde med perlinnoise funktionen och andra parametrar

            for (int y = 0; y < height; y++)
            {
                //Kolla om pixelns röda komponent är mindre eller lika med surfaceThreshold
                if (noiseTexture.GetPixel(x, y).r <= surfaceThreshold)
                    shouldPlace = true;

                else
                    shouldPlace = false;

                //Placera ett gräsblock på det högsta y värdet
                if (y == height - 1 && shouldPlace)
                    SpawnTile(new Vector3Int(x, y, 0), grassTile);

                //Placera ett jordblock på det näst högra y värdet
                else if (y == height - 2 && shouldPlace)
                    SpawnTile(new Vector3Int(x, y, 0), dirtTile);

                //Placera ett stenblock och backgroundTiles på resterande y värden
                else
                {
                    if (shouldPlace && y < height - 2)
                    {
                        SpawnTile(new Vector3Int(x, y, 0), stoneTile);
                        SpawnBackground(BackgroundTile, x, y);
                    }

                    if (noiseTexture.GetPixel(x, y).r >= surfaceThreshold)
                        SpawnBackground(BackgroundTile, x, y);
                }
            }
        }
    }

    //Metod för att skapa en bakgrundTile på en specifik position   
    void SpawnBackground(Sprite _object, int x, int y)
    {
        GameObject newTile = new GameObject(name = "backgroundTile"); //Skapa ett nytt tomt gameobject med namnet backgroundtile
        newTile.transform.parent = this.transform; //Gör gameobjectet till ett barn av det tomma gameobjektet som scriptet är lagt på
        newTile.transform.position = new Vector3((int)x + 0.5f, (int)y + 0.5f, 0.3f); //Ändra dess position till en vector3 med parametrarna X och Y
        newTile.AddComponent<SpriteRenderer>(); //Skapa en ny sprite renderer
        newTile.GetComponent<SpriteRenderer>().sprite = _object; //Spriten för objektet är parametern _object
        newTile.layer = 7; //Gameobjectets layer är 7, alltså World
        newTile.tag = "World"; //Gameobjectets tag är också World
    }

    //Metod för att skapa en Tile i griden och vilken sorts tile
    public void SpawnTile(Vector3Int pos, RuleTile tile)
    {
        grid.SetTile(pos, tile); //Placera en ruletile med parametern pos och tile
        GeneratedTiles.Add(tile); //Lägg till ruletilen i listan GeneratedTiles
    }

    //Metod för att ta bort en tile i griden på en specifik position
    public void DestroyTileAt(Vector3Int pos)
    {
        RuleTile ruleTileToRemove = grid.GetTile(pos) as RuleTile; //Hämtar en tile i griden med positionen pos och konverterar den till en ruletile

        if (ruleTileToRemove != null) //Om ruletilen som ska bort inte är null
        {
            grid.SetTile(pos, null); //Gör den till null (ta bort den)
            GeneratedTiles.Remove(ruleTileToRemove); //Ta bort den från listan där alla tiles är lagrade

        }
    }
}
