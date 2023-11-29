using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InventoryController : MonoBehaviour
{
    [Header("Procedural Generation")]
    //Refererar till terränggenerationen
    public ProceduralGeneration generation;

    [Header("Outline Sprite")]
    //En sprite för outlinen runt block
    public Sprite tileOutlineSprite;

    [Header("Play position")]
    //Spelar positionen i världen
    public Transform playerPosition;

    [Range(0, 12)]
    public int interactionRange = 12;

    //Skapa ett gameobjekt för hövreringsTilen
    GameObject hoverTile;

    //En bool som bestämmer om musen hövrerar
    bool isHovering;

    private void Start()
    {
        //Skapa ett tomt gameobject med namnet tileOutline
        hoverTile = new GameObject(name = "tileOutline");

        //Gameobjectet är ett barn till gameobjectet som har scriptet (Spelaren)
        hoverTile.transform.parent = this.transform;

        //Lägg till en sprite renderer
        hoverTile.AddComponent<SpriteRenderer>();

        //Gör spriten till tileOutlineSprite
        hoverTile.GetComponent<SpriteRenderer>().sprite = tileOutlineSprite;
    }

    //Metod som aktiverar och deaktiverar tileOutline gameobjectet
    void SelectedTile()
    {
        if (isHovering)
            hoverTile.SetActive(true);

        else
            hoverTile.SetActive(false);
    }

    //Metod som tillåter en att placera och ta sönder blocks som är inom interractionRange
    bool isWithinRange(Vector3 playerPos, Vector3 mousePos)
    {
        //Kollar om avståndet mellan spelaren och musens position på skärmen är mindre eller lika med interactionRange
        if ((int)Vector2.Distance(playerPos, mousePos) <= interactionRange)
            return true;

        return false;
    }

    //Skapa en RayCastHit2D
    RaycastHit2D hit;

    void Update()
    {
        //Kollar konstant om musen hövrerar över en tile i griden eller inte
        SelectedTile();

        //En vector3 som konverterar musens position på skärmen till spelet
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Spelarpositionen
        Vector3 playerPos = new Vector3(playerPosition.position.x, playerPosition.position.y);

        //En vector3 som bestämmer vilken cell i griden som musen hövrerar över
        Vector3Int selectedTile = generation.grid.WorldToCell(mousePos);

        //Skickar en raycast från musensposition
        hit = Physics2D.Raycast(mousePos, Vector2.zero);

        //Om tilen på positionen selectedTile är null
        if (generation.grid.GetTile(selectedTile) == null)
        {
            isHovering = false;

            //Om höger musknapp trycks och raycasten är null och inte träffar spelarens collider så placera en brickTile på musens position
            if (Input.GetMouseButtonDown(1) && (hit.collider == null || !hit.collider.gameObject.CompareTag("Player")))
                generation.SpawnTile(new Vector3Int(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y)), generation.brickTile);
        }

        //Annars om tilen på positionen selected tile inte är null
        else if (generation.grid.GetTile(selectedTile) != null)
        {
            //Om musen är inom interactionRange till spelaren
            if (isWithinRange(playerPos, mousePos))
            {
                //Visa tileOutline och uppdatera dess position 
                isHovering = true;
                hoverTile.transform.position = new Vector3(selectedTile.x + 0.5f, selectedTile.y + 0.5f, -0.1f);

                //Ta sönder tilen på muspositionen
                if (Input.GetMouseButtonDown(0)) 
                    generation.DestroyTileAt(selectedTile);
            }
            else
                isHovering = false;
        }
        else
            isHovering = false;
    }
}
