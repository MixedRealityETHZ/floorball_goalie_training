using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameObject Projection class
// Projection is the projection of the 2D goal in front of the goalie

public class Projection : MonoBehaviour {

    public Vector3 center;
    public double angle;
    public Vector3[] corners;
    
    /* constructor for the projection:
    array corner_positions: contains the positions on a flat plane of the corner positions relative to the center
    var center_position: center x,y,z position relative to Goal frame
    var orientation: represents the orientation (theta) of the (always vertical) projection of the goal 
    */
    // MAY NOT BE NECESSARY: NEED A WAY TO CHANGE SIZE TO ALWAYS BE 1 METER IN FRONT OF PLAYER
    public Projection(Vector3[] corner_positions, Vector3 center_position, double orientation) {
        center = center_position;
        angle = orientation;
        corners = corner_positions;
    }

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }
}