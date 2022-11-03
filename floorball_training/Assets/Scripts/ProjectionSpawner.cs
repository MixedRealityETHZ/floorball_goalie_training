using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionSpawner : MonoBehaviour
{
    GameObject floorball_ball;
    GameObject proj;

    public GameObject spawned_proj;
    private Quaternion orientation;
    private Vector3 pos;
    private bool proj_has_spawned;
    private bool ball_has_spawned;


    private void SpawnProjection(Vector3 pos, Quaternion orientation)
    {
        proj = Instantiate(spawned_proj, pos, orientation) as GameObject;
        proj.GetComponent<Renderer>().material.color = Color.blue;
    }

    // Start is called before the first frame update
    void Start()
    {
        proj_has_spawned = false;
        ball_has_spawned = BallSpawner.ball_has_spawned;
    }
    // Update is called once per frame
    void Update()
    {
        if (ball_has_spawned && !proj_has_spawned)
        {
            floorball_ball = GameObject.Find("floorball_ball");

            // have the projection always in the same orientation as the ball (always facing the goal)
            orientation = floorball_ball.transform.rotation * Quaternion.Euler(90, 180, 0);
            // (current) have projection in 1m in front of ball: to update to put 2m from goal OR 1m from player
            pos = floorball_ball.transform.position + floorball_ball.transform.TransformDirection(new Vector3(0, 0, 1));

            proj_has_spawned = true;
            SpawnProjection(pos, orientation);
        }
        else if (ball_has_spawned)
        {
            // have the projection always in the same orientation as the ball (always facing the goal)
            orientation = floorball_ball.transform.rotation * Quaternion.Euler(90, 180, 0);
            // (current) have projection in 1m in front of ball: to update to put 2m from goal OR 1m from player
            pos = floorball_ball.transform.position + floorball_ball.transform.TransformDirection(new Vector3(0, 0, 1));

            proj.transform.position = pos;
            proj.transform.rotation = orientation;
        }
        ball_has_spawned = BallSpawner.ball_has_spawned;
    }
}
