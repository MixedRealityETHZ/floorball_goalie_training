using UnityEngine;
using System.Collections;

// GameObject Projection class
// Projection is the projection of the 2D goal in front of the goalie

public class Projection : GameObject {

    var center;
    var theta;
    var[] corners;
    
    /* constructor for the projection:
    array corner_positions: contains the positions on a flat plane of the corner positions relative to the center
    var center_position: center x,y,z position relative to Goal frame
    var orientation: represents the orientation (theta) of the (always vertical) projection of the goal 
    */
    public Projection(var[] corner_positions, var center_position, var orientation) {
        self.center = center_position;
        self.theta = orientation;
        self.corners = corner_positions;
    }

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }
}