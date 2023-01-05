using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Us: 169.254.71.173
// Julia:169.254.219.203

public class ProjectionSpawner : MonoBehaviour
{
    //Enable/Disable Linerendering
    public static bool LineRenderingEnabled = false;

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
    //public GameObject spawned_quad;

    public GameObject floorball_ball;
    public GameObject man_quad;
    public GameObject goal_cube;

    public GameObject debugging1;
    public GameObject debugging2;
    public GameObject debugging3;
    public GameObject debugging4;

    private Quaternion orientation;
    private Vector3 pos;
    private bool proj_has_spawned;
    private Mesh man_mesh;

    private Vector3[] new_vertices = new Vector3[4];

    /*
     * Idea: spawn and move around a prefab plane, and determine where it 
     * intersects lines between the ball and the goal, and set those points
     * to the coordinats of a prefab cube
     */
    private void SpawnProjection(Vector3 pos, Quaternion orientation)
    {
        proj = Instantiate(spawned_proj, pos, orientation) as GameObject;
        proj.GetComponent<Renderer>().material.color = Color.blue;
        proj.GetComponent<Renderer>().enabled = false; // makes the larger plane invisible

        mesh = proj.GetComponent<MeshFilter>().mesh;

        // plane GameObject -> can probably remove
        plane = Instantiate(spawned_plane, pos, orientation) as GameObject;
        plane.GetComponent<Renderer>().enabled = false; // makes the larger plane invisible
        plane.GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.5f);
        plane_mesh = plane.GetComponent<MeshFilter>().mesh;

        //quad = Instantiate(spawned_quad, pos, orientation) as GameObject;
        //quad.GetComponent<Renderer>().material.color = Color.yellow;
        //quad.GetComponent<Renderer>().enabled = false;
        //quad_mesh = quad.GetComponent<MeshFilter>().mesh;


    }

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Debug.Log("START");
        goal_cube.GetComponent<Renderer>().material.color = Color.green;
        UnityEngine.Debug.Log("GOal should be green");
        MeshRenderer meshRenderer = man_quad.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        MeshFilter meshFilter = man_quad.AddComponent<MeshFilter>();
        man_mesh = new Mesh();
        man_quad.GetComponent<Renderer>().material.color = new Color(0.5f, 0.2f, 0.2f, 0.3f);
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0)
        };
        man_mesh.vertices = vertices;
        UnityEngine.Debug.Log("Original vertices set as " + man_mesh.vertices.ToString());
        int[] tris = new int[6]
        {
            // lower left triangle
            2, 1, 0,
            // upper right triangle
            0, 3, 2
        };
        man_mesh.triangles = tris;
        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        man_mesh.normals = normals;
        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        man_mesh.uv = uv;
        meshFilter.mesh = man_mesh;

        man_quad.transform.position= new Vector3(0f, 0f, 0f);

        proj_has_spawned = false;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3[] corner_positions = new Vector3[4];
        //corner_positions[0] = goal_cube.transform.position + new Vector3(1.6f / 2.0f, 1.15f / 2.0f, 0.6f / 2.0f); // ORIGINAL, USE BELOW FOR FLAT
        //corner_positions[1] = goal_cube.transform.position + new Vector3(-1.6f / 2.0f, 1.15f / 2.0f, 0.6f / 2.0f);
        //corner_positions[2] = goal_cube.transform.position + new Vector3(-1.6f / 2.0f, -1.15f / 2.0f, 0.6f / 2.0f);
        //corner_positions[3] = goal_cube.transform.position + new Vector3(1.6f / 2.0f, -1.15f / 2.0f, 0.6f / 2.0f);
        corner_positions[0] = goal_cube.transform.position + new Vector3(1.6f / 2.0f, 1.15f / 2.0f, 0.0f / 2.0f);// goal_cube.transform.TransformPoint(new Vector3(1.6f / 2.0f, 1.15f / 2.0f, 0.0f / 2.0f));
        corner_positions[1] = goal_cube.transform.position + new Vector3(-1.6f / 2.0f, 1.15f / 2.0f, 0.0f / 2.0f);// goal_cube.transform.TransformPoint(new Vector3(-1.6f / 2.0f, 1.15f / 2.0f, 0.0f / 2.0f));
        corner_positions[2] = goal_cube.transform.position + new Vector3(-1.6f / 2.0f, -1.15f / 2.0f, 0.0f / 2.0f);// goal_cube.transform.TransformPoint(new Vector3(-1.6f / 2.0f, -1.15f / 2.0f, 0.0f / 2.0f));
        corner_positions[3] = goal_cube.transform.position + new Vector3(1.6f / 2.0f, -1.15f / 2.0f, 0.0f / 2.0f);// goal_cube.transform.TransformPoint(new Vector3(1.6f / 2.0f, -1.15f / 2.0f, 0.0f / 2.0f));

        if (!proj_has_spawned)
        {
            updateProjPos(true);

            proj_has_spawned = true;

            // render lines:

            for (int i = 0; i < 4; i++)
            {
                //For creating line renderer object
                lines[i] = new GameObject("Line").AddComponent<LineRenderer>();
                lines[i].GetComponent<Renderer>().enabled = LineRenderingEnabled;
                lines[i].material = new Material(Shader.Find("Sprites/Default"));
                lines[i].startColor = Color.yellow;
                lines[i].endColor = Color.yellow;
                lines[i].startWidth = 0.01f;
                lines[i].endWidth = 0.01f;
                lines[i].positionCount = 2;
                lines[i].useWorldSpace = true;

                // rays: mathematical lines
                rays[i] = new Ray(floorball_ball.transform.position, (corner_positions[i] - floorball_ball.transform.position).normalized);

                //For drawing line in the world space, provide the x,y,z values
                renderLine(floorball_ball.transform.position, corner_positions[i], i);
            }

        }
        else
        {
            updateProjPos(false);

            for (int i = 0; i < 4; i++)
            {
                lines[i].GetComponent<Renderer>().enabled = LineRenderingEnabled;

                renderLine(floorball_ball.transform.position, corner_positions[i], i);
                // Uncomment to view the problems caused by trying to update the plane (or cube if you want) corner positions
                // Summary of issue: the mesh is not 4 corners, it's n points for... some reason
                UnityEngine.Debug.Log("Shoulda rendered");
                Vector3 dist_cc = -floorball_ball.transform.position + goal_cube.transform.position;
                UnityEngine.Debug.Log("transformation worked");
                UnityEngine.Debug.Log(dist_cc.normalized);
                //calcVertexPos(i, dist_cc.normalized);

                UnityEngine.Debug.Log("started calcVertexPos");
                UnityEngine.Debug.Log(man_mesh.vertices.ToString());
                Vector3 vertex = man_mesh.vertices[i];

                UnityEngine.Debug.Log("vertices added");
                // Plane struct (not GameObject): purely mathematical to get intersection positions w/ rays
                Plane temp_plane = new Plane(dist_cc.normalized, plane.transform.position);
                float enter = 0.0f;

                UnityEngine.Debug.Log("plane made");
                // Debug.Log(temp_plane);

                if (temp_plane.Raycast(rays[i], out enter)) // not entering here?
                {
                    Vector3 hit_point = rays[i].GetPoint(enter);
                    vertex = hit_point;
                    UnityEngine.Debug.Log("Has Hit " + hit_point.ToString());
                }
                UnityEngine.Debug.Log("Rays cast");
                new_vertices[i] = vertex;

                UnityEngine.Debug.Log(vertex.ToString() + " Vertex: " + new_vertices[i].ToString());

                UnityEngine.Debug.Log("Mesh set");
                UnityEngine.Debug.Log("Vertex Position Calculated!");
            }

            man_mesh.vertices = new_vertices;

            man_mesh.RecalculateTangents();
            man_mesh.RecalculateBounds();

            debugging1.transform.position = man_mesh.vertices[0];
            debugging2.transform.position = man_mesh.vertices[1];
            debugging3.transform.position = man_mesh.vertices[2];
            debugging4.transform.position = man_mesh.vertices[3];
        }

    }

    // find where the spawned_plane (plane) intersects with all of the lines
    // and set the vertices of some square mesh object to those points
    //public void calcVertexPos(int i, Vector3 norm)
    //{
    //    UnityEngine.Debug.Log("started calcVertexPos");
    //    UnityEngine.Debug.Log(man_mesh.vertices.ToString());
    //    Vector3 vertex = man_mesh.vertices[i];

    //    UnityEngine.Debug.Log("vertices added");
    //    // Plane struct (not GameObject): purely mathematical to get intersection positions w/ rays
    //    Plane temp_plane = new Plane(norm, plane.transform.position);   
    //    float enter = 0.0f;

    //    UnityEngine.Debug.Log("plane made");
    //    // Debug.Log(temp_plane);

    //    if (temp_plane.Raycast(rays[i], out enter)) // not entering here?
    //    {
    //        Vector3 hit_point = rays[i].GetPoint(enter);
    //        vertex = hit_point;
    //        UnityEngine.Debug.Log("Has Hit " + hit_point.ToString());
    //    }
    //    UnityEngine.Debug.Log("Rays cast");
    //    man_mesh.vertices[i] = vertex;
    //    man_mesh.vertices[i].x = vertex.x;


    //    UnityEngine.Debug.Log(vertex.ToString() + " Vertex: " + man_mesh.vertices[i].ToString());


    //    UnityEngine.Debug.Log("Mesh set");
    //}

    public void updateProjPos(bool to_spawn)
    { 
        floorball_ball.transform.LookAt(goal_cube.transform.position);
        // have the projection always in the same orientation as the ball (always facing the goal)
        orientation = floorball_ball.transform.rotation * Quaternion.Euler(90, 180, 0);
        // (current) have projection in 1m in front of ball: to update to put 2m from goal OR 1m from player
        // pos = floorball_ball.transform.position + floorball_ball.transform.TransformDirection(new Vector3(0, 0, 1));
        Vector3 diff_goal_ball = goal_cube.transform.position - floorball_ball.transform.position;
        float dist = diff_goal_ball.magnitude;

        // (current) have projection in 1m in front of ball: to update to put 2m from goal OR 1m from player
        // pos = floorball_ball.transform.position + floorball_ball.transform.TransformDirection(new Vector3(0, 0, 1));
        pos = floorball_ball.transform.position + floorball_ball.transform.TransformDirection(new Vector3(0.0f, 0.0f, dist - 2.5f));

        if (to_spawn) SpawnProjection(pos, orientation);
        else
        {
            proj.transform.position = pos;
            proj.transform.rotation = orientation;
            plane.transform.position = pos;
            plane.transform.rotation = orientation;
        }
    }

    // helper method to update line position
    public void renderLine(Vector3 ball_pos, Vector3 corner_pos, int i)
    {
        //For drawing line in the world space, provide the x,y,z values
        lines[i].SetPosition(0, ball_pos); //x,y and z position of the starting point of the line
        lines[i].SetPosition(1, corner_pos); //x,y and z position of the end point of the line

        UnityEngine.Debug.Log("line set");
        UnityEngine.Debug.Log(i.ToString());
        rays[i].origin = ball_pos;
        UnityEngine.Debug.Log("ray origin");
        rays[i].direction = (corner_pos - ball_pos).normalized;
        UnityEngine.Debug.Log("ray set");
    }
}
