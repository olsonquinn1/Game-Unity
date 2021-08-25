using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_movement : MonoBehaviour
{

    public int speed = 50;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertocal");

        Vector2 movement = new Vector2(speed.x * inputX, speed.y * inputY);

        transform.Translate(movement * Time.deltaTime);
    }
}
