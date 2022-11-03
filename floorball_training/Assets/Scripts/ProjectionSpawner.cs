using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionSpawner : MonoBehaviour
{
    GameObject floorball_ball;
    GameObject proj;
    GameObject plane;
    LineRenderer[] lines = new LineRenderer[4];
    Ray[] rays = new Ray[4];
    Mesh mesh;
    Mesh plane_mesh;

    public GameObject spawned_proj;
    public GameObject spawned_plane;

    private Quaternion orientation;
    private Vector3 pos;
    private bool proj_has_spawned;
    private bool ball_has_spawned;

    /*
     * Idea: spawn and move around a prefab plane, and determine where it 
     * intersects lines between the ball and the goal, and set those points
     * to the coordinats of a prefab cube
     */
    private void SpawnProjection(Vector3 pos, Quaternion orientation)
    {
        proj = Instantiate(spawned_proj, pos, orientation) as GameObject;
        proj.GetComponent<Renderer>().material.color = Color.blue;

        mesh = proj.GetComponent<MeshFilter>().mesh;

        plane = Instantiate(spawned_plane, pos, orientation) as GameObject;
        //plane.GetComponent<Renderer>().enabled = false;
        plane_mesh = plane.GetComponent<MeshFilter>().mesh;
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

            updateProjPos(true);

            proj_has_spawned = true;

            // render lines:
            
            for (int i=0; i<4; i++)
            {
                //For creating line renderer object
                lines[i] = new GameObject("Line").AddComponent<LineRenderer>();
                lines[i].startColor = Color.black;
                lines[i].endColor = Color.black;
                lines[i].startWidth = 0.01f;
                lines[i].endWidth = 0.01f;
                lines[i].positionCount = 2;
                lines[i].useWorldSpace = true;

                rays[i] = new Ray(floorball_ball.transform.position, (GoalSpawner.corner_positions[i] - floorball_ball.transform.position).normalized);

                //For drawing line in the world space, provide the x,y,z values
                renderLine(floorball_ball.transform.position, GoalSpawner.corner_positions[i], i);
            }

        }
        else if (ball_has_spawned)
        {
            updateProjPos(false);

            for (int i = 0; i<4; i++)
            {
                renderLine(floorball_ball.transform.position, GoalSpawner.corner_positions[i], i);
                // Uncomment to view the problems caused by trying to update the plane (or cube if you want) corner positions
                // Summary of issue: the mesh is not 4 corners, it's n points for... some reason
                // calcVertexPos(i);
            }
        }
        ball_has_spawned = BallSpawner.ball_has_spawned;
    }

    // find where the spawned_plane (plane) intersects with all of the lines
    public void calcVertexPos(int i)
    {
        Vector3[] vertices = plane_mesh.vertices;

        var filter = plane.GetComponent<MeshFilter>();
        Vector3 normal = new Vector3(0, 0, 0);

        if (filter && filter.mesh.normals.Length > 0)
            normal = filter.transform.TransformDirection(filter.mesh.normals[0]);

        var temp_plane = new Plane(normal, transform.position);

        float enter = 0.0f;
        if (temp_plane.Raycast(rays[i], out enter))
        {
            Vector3 hit_point = rays[i].GetPoint(enter);
            if (i == 0) vertices[0] = hit_point;
            if (i == 1) vertices[10] = hit_point;
            if (i == 2) vertices[110] = hit_point;
            if (i == 3) vertices[120] = hit_point;
        }
        plane_mesh.vertices = vertices;
    }

    public void updateProjPos(bool to_spawn)
    {
        // have the projection always in the same orientation as the ball (always facing the goal)
        orientation = floorball_ball.transform.rotation * Quaternion.Euler(90, 180, 0);
        // (current) have projection in 1m in front of ball: to update to put 2m from goal OR 1m from player
        // pos = floorball_ball.transform.position + floorball_ball.transform.TransformDirection(new Vector3(0, 0, 1));
        Vector3 diff_goal_ball = GoalSpawner.center_position - floorball_ball.transform.position;
        float dist = diff_goal_ball.magnitude;

        // (current) have projection in 1m in front of ball: to update to put 2m from goal OR 1m from player
        // pos = floorball_ball.transform.position + floorball_ball.transform.TransformDirection(new Vector3(0, 0, 1));
        pos = floorball_ball.transform.position + floorball_ball.transform.TransformDirection(new Vector3(0.0f, 0.0f, dist - 1.0f));

        if (to_spawn) SpawnProjection(pos, orientation);
        else
        {
            proj.transform.position = pos;
            proj.transform.rotation = orientation;
            //plane.transform.position = pos;
            //plane.transform.rotation = orientation;
        }
    }

    // helper method to update line position
    public void renderLine(Vector3 ball_pos, Vector3 corner_pos, int i)
    {
        //For drawing line in the world space, provide the x,y,z values
        lines[i].SetPosition(0, ball_pos); //x,y and z position of the starting point of the line
        lines[i].SetPosition(1, corner_pos); //x,y and z position of the end point of the line

        rays[i].origin = ball_pos;
        rays[i].direction = (corner_pos - ball_pos).normalized;
    }
}
