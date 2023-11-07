using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{

    public List<GameObject> InventoryGO;



    private void Start()
    {
        InventoryGO = new();
    }

    private void Update()
    {
        if (InventoryGO.Count != 0)
        {
            Debug.Log(InventoryGO[0]);
        }
    }
}
