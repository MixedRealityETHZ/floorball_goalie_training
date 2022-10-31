using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameObject Ball class

public class Ball : MonoBehaviour {

    public static Vector3 position;
    public static bool is_seen = true;

    // Use this for initialization
    void Start () {
        // hard coded ball position
        position = new Vector3(0.0f, -10.0f, 3.0f);

        gameObject.GetComponent<Renderer>().material.color = new Color(255, 0, 0); // make the ball red
    }

    // Update is called once per frame
    void Update () {
    
    }
}