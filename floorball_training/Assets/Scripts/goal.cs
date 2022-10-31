using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameObject Goal class

public class Goal : MonoBehaviour {

    public static Vector3 center_position;
    public static Quaternion orientation;
    public static Vector3[] corner_positions;
    public static bool was_seen = false;

    public Goal(Vector3[] corner_positions, int[] corner_ids) {
        /*
         * Goal constructor that takes in the corner positions as seen
         * from the Hololens and calculates the goal center position and angle
         * based on the ids of the seen corners, and defines its corners based on that
         */
    }

    // Use this for initialization
    void Start () {
        gameObject.GetComponent<Renderer>().material.color = new Color(0, 255, 0); // make the goal green

        // hard coded
        center_position = new Vector3(0, 1.15f / 2.0f, 0.0f);
        orientation = Quaternion.identity;

        // calculate corner positions based on center position:
        corner_positions = new Vector3[4];
        corner_positions[0] = center_position + new Vector3(1.6f / 2.0f, 1.15f / 2.0f, 0.6f / 2.0f);
        corner_positions[1] = center_position + new Vector3(-1.6f / 2.0f, 1.15f / 2.0f, 0.6f / 2.0f);
        corner_positions[2] = center_position + new Vector3(-1.6f / 2.0f, -1.15f / 2.0f, 0.6f / 2.0f);
        corner_positions[3] = center_position + new Vector3(-1.6f / 2.0f, -1.15f / 2.0f, 0.6f / 2.0f);
    }

    // Update is called once per frame
    void Update () {
          
    }
}