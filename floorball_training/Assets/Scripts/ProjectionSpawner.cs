using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionSpawner : MonoBehaviour
{
    GameObject floorball_ball;
    // these are all *kinda* redudant (but not really currently)
    GameObject proj;
    GameObject plane;
    GameObject quad;

    LineRenderer[] lines = new LineRenderer[4];
    Ray[] rays = new Ray[4];

    Mesh mesh;
    Mesh plane_mesh;
    Mesh quad_mesh;

    public GameObject spawned_proj;
    public GameObject spawned_plane;
    public GameObject spawned_quad;

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

        // plane GameObject -> can probably remove
        plane = Instantiate(spawned_plane, pos, orientation) as GameObject;
        plane.GetComponent<Renderer>().enabled = false; // makes the larger plane invisible

        plane_mesh = plane.GetComponent<MeshFilter>().mesh;

        quad = Instantiate(spawned_quad, pos, orientation) as GameObject;
        quad.GetComponent<Renderer>().material.color = Color.yellow;

        quad_mesh = quad.GetComponent<MeshFilter>().mesh;
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

                // rays: mathematical lines
                rays[i] = new Ray(floorball_ball.transform.position, (GoalSpawner.corner_positions[i] - floorball_ball.transform.position).normalized);

                //For drawing line in the world space, provide the x,y,z values
                renderLine(floorball_ball.transform.position, GoalSpawner.corner_positions[i], i);
            }

        }
        else if (ball_has_spawned)
        {
            /*Debug.Log(quad_mesh.vertices[0]);
            Debug.Log(quad_mesh.vertices[1]);
            Debug.Log(quad_mesh.vertices[2]);
            Debug.Log(quad_mesh.vertices[3]);*/

            updateProjPos(false);

            for (int i = 0; i<4; i++)
            {
                renderLine(floorball_ball.transform.position, GoalSpawner.corner_positions[i], i);
                // Uncomment to view the problems caused by trying to update the plane (or cube if you want) corner positions
                // Summary of issue: the mesh is not 4 corners, it's n points for... some reason
                Vector3 dist_cc = -floorball_ball.transform.position + GoalSpawner.center_position;
                calcVertexPos(i, dist_cc.normalized);
            }
        }
        ball_has_spawned = BallSpawner.ball_has_spawned;
    }

    // **TODO**
    // find where the spawned_plane (plane) intersects with all of the lines
    // and set the vertices of some square mesh object to those points
    public void calcVertexPos(int i, Vector3 norm)
    {
        Vector3[] vertices = quad_mesh.vertices;
        Vector3[] plane_vertices = plane_mesh.vertices;
        //Debug.Log(quad_mesh.vertices);

        /*Debug.Log(plane_vertices[0]);
        Debug.Log(plane_vertices[10]);
        Debug.Log(plane_vertices[110]);*/

        // Plane struct (not GameObject): purely mathematical to get intersection positions w/ rays
        Plane temp_plane = new Plane(norm, plane_vertices[10]);   

        float enter = 0.0f;

        // Debug.Log(temp_plane);

        if (temp_plane.Raycast(rays[i], out enter)) // not entering here?
        {
            Vector3 hit_point = rays[i].GetPoint(enter);
            vertices[i] = hit_point;
        }
        quad_mesh.vertices = vertices; // this code is correct
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
            plane.transform.position = pos;
            plane.transform.rotation = orientation;
            //quad.transform.position = pos;
            //quad.transform.rotation = orientation;
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

        // Draw rays in (Unity Debug Mode in Scene view)
        // Debug.DrawRay(ball_pos, (corner_pos - ball_pos).normalized * 100f, Color.red, duration: 1.0f / 30.0f);
    }
}
