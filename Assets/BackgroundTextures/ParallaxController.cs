using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{

    public Transform target;

    [Range(0, 1)]
    public float parallaxFactor; //Bestämmer hur starkt parallax effekten ska vara för varje lager

    public Rigidbody2D playerRB;

    float horizontal;

    void Update()
    {
        //Om spelarens rigidbody inte är 0 så är bestäms horizontal av "Horizontal" input, alltså om spelaren går åt vänster eller höger
        if (playerRB.velocity.x != 0)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            horizontal = 1;
        }
        //En vector som bestämmer vart varje lager av bakgrunden ska vara
        Vector3 followVector = new Vector3(transform.position.x + parallaxFactor / 50 * horizontal, target.position.y, transform.position.z);

        transform.position = followVector;
    }
}