using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCycleManager : MonoBehaviour {

    enum Shape { Random, Triangle, Square }

    [SerializeField] bool isRunning = true;
    [SerializeField] float tickRate = 0.01f;
    [SerializeField] float pointsPerTick = 100;
    [SerializeField] Shape startingShape;
    [SerializeField] int numOfVertex = 3;
    [SerializeField] float jumpPercent = 0.5f;

    Vector3 centre;
    float radius;
    Vector2[] vertexes;
    Vector2 currentPoint;
    Color32[] colors;

    void Start ()
    {
        centre = AdjustCamera();
        radius = Camera.main.orthographicSize;
        vertexes = GenerateStartingShape();
        currentPoint = GetRandomStartingPoint();
        colors = FillColorArrayRandomly();

        StartCoroutine(Tick());
	}

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            isRunning = !isRunning;
        }
    }

    IEnumerator Tick()
    {
        while (true)
        {
            if (isRunning)
            {
                for (int i = 0; i < pointsPerTick; i++)
                    GenerateNextRandomPoint(GetRandomVertex());
            }
            yield return new WaitForSeconds(tickRate);
        }
    }

    /// <summary>
    /// Position the camera where the bottom left is at x=0 and y=0
    /// </summary>
    /// <returns></returns>
    Vector3 AdjustCamera()
    {
        Camera cam = Camera.main;
        cam.transform.position = new Vector3(cam.aspect * cam.orthographicSize, cam.aspect, 0);
        Vector3 centre = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 1f));
        return centre;
    }

    Color32[] FillColorArrayRandomly()
    {
        Color32[] colors = new Color32[vertexes.Length];

        for (int i = 0; i < colors.Length; i++)
            colors[i] = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);

        return colors;
    }

    Vector2[] GenerateStartingShape()
    {
        switch (startingShape)
        {
            case Shape.Random:
                return GenerateRandomShape(numOfVertex);
            case Shape.Triangle:
                return GeneratePredefinedTriangle();
            case Shape.Square:
                return GeneratePredefinedSquare();
            default:
                return GenerateRandomShape(numOfVertex);
        }
    }

    Vector2[] GenerateRandomShape(int numOfVertex)
    {
        Vector2[] vertexes = new Vector2[numOfVertex];

        for (int i = 0; i < numOfVertex; i++)
        {
            Vector2 point = new Vector2(centre.x, centre.y) + Random.insideUnitCircle.normalized * radius;
            vertexes[i] = point;
        }

        return vertexes;
    }

    Vector2[] GeneratePredefinedTriangle()
    {
        Vector2[] vertexes = new Vector2[3];

        vertexes[0] = new Vector2(centre.x, centre.y + radius);
        vertexes[1] = new Vector2(centre.x + radius, centre.y - radius);
        vertexes[2] = new Vector2(centre.x - radius, centre.y - radius);

        return vertexes;
    }

    Vector2[] GeneratePredefinedSquare()
    {
        Vector2[] vertexes = new Vector2[4];

        vertexes[0] = new Vector2(centre.x - radius, centre.y + radius);
        vertexes[1] = new Vector2(centre.x + radius, centre.y + radius);
        vertexes[2] = new Vector2(centre.x + radius, centre.y - radius);
        vertexes[3] = new Vector2(centre.x - radius, centre.y - radius);

        return vertexes;
    }

    void DisplayPoint(Vector2 point, Color32 color)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(point.x, point.y, 1f);
        sphere.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
        MeshRenderer renderer = sphere.GetComponent<MeshRenderer>();
        renderer.material.color = color;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    Vector2 GetRandomStartingPoint()
    {
        Vector2 point = new Vector2(centre.x, centre.y) + Random.insideUnitCircle * radius;
        return point;
    }

    void GenerateNextRandomPoint(int targetVertex)
    {
        Vector2 nextPoint = Vector2.Lerp(currentPoint, vertexes[targetVertex], jumpPercent);
        DisplayPoint(nextPoint, colors[targetVertex]);
        currentPoint = nextPoint;
    }

    int GetRandomVertex()
    {
        return Random.Range(0, vertexes.Length);
    }
}
