using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Kamerans target
    public Transform target;

    //Kamerans offset
    private Vector3 offset = new Vector3(0f, 0f, -10f);

    //Kamerans ref velocitet
    private Vector3 velocity = Vector3.zero;

    //Bestämmer hur långt tid kameran tar på sig att röra sig till spelaren
    [Range(0f, 1f)]
    public float smoothTime;

    public ProceduralGeneration generation;

    Vector3 mousePos;

    Vector3 lastPosition;
    void Update()
    {
        //Musens normaliserade position i världen
        mousePos = Camera.main.WorldToScreenPoint(Input.mousePosition).normalized; 

        //Kamerans målposition
        Vector3 targetPosition = target.position + offset + mousePos;

        //Smootha kamerans position till targetPosition under en viss tid
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
