using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Spawn a ball when it is seen and track update its position
 * Following this tutorial: https://www.youtube.com/watch?v=Zb9WchxZhvM
 */

public class BallSpawner : MonoBehaviour {

    GameObject ball;
    public GameObject spawned_ball;
    public static bool ball_has_spawned;
    
    private void SpawnBall(Vector3 pos)
    {
        ball = Instantiate(spawned_ball, pos, Quaternion.identity) as GameObject;
        ball.GetComponent<Renderer>().material.color = Color.red;
        ball.name = "floorball_ball";

        if (GoalSpawner.goal_has_spawned)
        {
            ball.transform.LookAt(GameObject.Find("floorball_goal").transform.position);
        }
        // ball.transform.localRotation = Quaternion.Euler(0.0f, ball.transform.localRotation.y, 0.0f);
    }

    void Start()
    {
        ball_has_spawned = false;
    }

    // Update is called once per frame
    void Update()
    {
        // only run is the ball is visible but has not yet been spawned
        if (!ball_has_spawned)
        {
            ball_has_spawned = true;
            // Vector3 position = C++ script finding position

            // hard coded ball position
            SpawnBall(new Vector3(1.0f, 0.0f, -3.0f));
        }
        else if (GoalSpawner.goal_has_spawned)
        {
            ball.transform.LookAt(GameObject.Find("floorball_goal").transform.position);
        }
    }
}
