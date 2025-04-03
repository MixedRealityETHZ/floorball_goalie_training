using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class pictureeee : MonoBehaviour
{
    public VideoPanelApp videoPanelApp;
    int runCount = 77;
    public string running = "not set";


    public static Texture2D image;
    GameObject quad;
    Renderer quadRenderer;
    Texture2D imageTMP;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("start pictureeee script");
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quadRenderer = quad.GetComponent<Renderer>() as Renderer;
        quadRenderer.material = new Material(Shader.Find("Unlit/Texture"));
        quad.transform.parent = this.transform;
        quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);
        quad.GetComponent<Renderer>().enabled = false;

        imageTMP = new Texture2D(896, 504);
        image = new Texture2D(896, 504);

        //StartCoroutine(ExampleCoroutine());
    }

    IEnumerator ExampleCoroutine()
    {
        while (true)
        {
            Debug.Log("5!!!!!!!!!!!!!!!!!!!!!!!!!! pictureeee running");
            if (videoPanelApp._latestImageBytes.Length > 0)
            {
                imageTMP.LoadRawTextureData(videoPanelApp._latestImageBytes);
                imageTMP.Apply();
                //imageTMP = VideoPanelApp.image;
                Turn180Degree(imageTMP);
                image = imageTMP;
                quadRenderer.material.mainTexture = image;
                
            }
            //Debug.Log("while true pictureee Script");
            yield return new WaitForSeconds(0.1f);
        }
        
    }

    // Update is called once per frame
    void Update()
    {   
        //public variab to display for running check
        runCount++;
        if(runCount == 999)
        {
            runCount = 0;
        }
        running = runCount.ToString();

        Debug.Log("5!!!!!!!!!!!!!!!!!!!!!!!!!! pictureeee running");
        if (BallTracker.have_tex)
        {
            quadRenderer.material.mainTexture = BallTracker.tex;
        }
        else if (videoPanelApp._latestImageBytes.Length > 0)
        {   
            running = runCount.ToString() + "\n  --> bytes: " + videoPanelApp._latestImageBytes.Length.ToString();
            //imageTMP.LoadRawTextureData(videoPanelApp._latestImageBytes);
            //imageTMP.Apply();
            ////imageTMP = VideoPanelApp.image;
            ////FlipTextureVertically(imageTMP);
            //image = imageTMP;
            //quadRenderer.material.mainTexture = VideoPanelApp.image;

        }

        VideoByteText.running = running;
    }


    public static void Turn180Degree(Texture2D original)
    {
        var originalPixels = original.GetPixels();

        var newPixels = new Color[originalPixels.Length];

        var width = original.width;
        var rows = original.height;

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < rows; y++)
            {
                newPixels[x + y * width] = originalPixels[(width - x - 1) + (rows - y - 1) * width]; //(width - x - 1)
            }
        }

        original.SetPixels(newPixels);
        original.Apply();
    }

    public static void FlipTextureVertically(Texture2D original)
    {
        var originalPixels = original.GetPixels();

        var newPixels = new Color[originalPixels.Length];

        var width = original.width;
        var rows = original.height;

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < rows; y++)
            {
                newPixels[x + y * width] = originalPixels[x + (rows - y - 1) * width];
            }
        }

        original.SetPixels(newPixels);
        original.Apply();
    }
}
