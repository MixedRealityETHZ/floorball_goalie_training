using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalSpawner : MonoBehaviour {

    public GameObject spawned_goal;
    GameObject goal;

    public static Vector3 center_position;
    public static Quaternion orientation;
    public static Vector3[] corner_positions;

    public static bool goal_has_spawned;

    void SpawnGoal(Vector3 pos, Quaternion orientation)
    {
        goal = Instantiate(spawned_goal, pos, Quaternion.identity) as GameObject;
        goal.GetComponent<Renderer>().material.color = Color.green;
        goal.name = "floorball_goal";
    }

    // Start is called before the first frame update
    void Start()
    {
        goal_has_spawned = false;

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
    void Update()
    {
        // only run is the goal has been seen and has not yet been spawned
        if (!goal_has_spawned)
        {
            goal_has_spawned = true;
            // using hard coded goal position
            SpawnGoal(center_position + new Vector3(0.0f, 0.0f, 0.6f/2.0f), orientation);
        }
    }
}
