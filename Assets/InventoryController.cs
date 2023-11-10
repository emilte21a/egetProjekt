using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Procedural Generation")]
    public ProceduralGeneration generation;

    List<GameObject> InventoryGO;

    [Header("Tiles")]
    public Sprite BrickTile;
    public Sprite tileOutlineSprite;

    [Header("Play position")]
    public Transform playerPosition;

    private int interactionRange = 12;

    GameObject hoverTile;

    private void Start()
    {
        InventoryGO = new();

        hoverTile = new GameObject(name = "tileOutline");
        hoverTile.transform.parent = this.transform;
        hoverTile.AddComponent<SpriteRenderer>();
        hoverTile.GetComponent<SpriteRenderer>().sprite = tileOutlineSprite;
    }

    void DestroyObject(GameObject _gameObject)
    {
        InventoryGO.Add(_gameObject);
        Destroy(_gameObject);
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
        if ((int)Vector2.Distance(playerPos, mousePos) <= interactionRange)
            return true;

        return false;
    }

    RaycastHit2D hit;

    void Update()
    {
        SelectedTile();

        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = new Vector3(playerPosition.position.x, playerPosition.position.y);

        if (hit.collider == null)
        {
            hover = false;

            if (Input.GetMouseButtonDown(1) && isWithinRange(playerPos, mousePos))
                generation.SpawnObject(BrickTile, Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));

        }

        else if (hit.collider.gameObject.tag == "World")
        {
            if (isWithinRange(playerPos, mousePos))
            {
                hover = true;
                hoverTile.transform.position = new Vector3(Mathf.FloorToInt(mousePos.x) + 0.5f, Mathf.FloorToInt(mousePos.y) + 0.5f, -0.1f);

                if (Input.GetMouseButtonDown(0))
                {
                    generation.GeneratedTiles.Remove(hit.collider.gameObject);
                    DestroyObject(hit.collider.gameObject);
                }

            }
            else
                hover = false;
        }
        else
            hover = false;

    }
}
