using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadowScript : MonoBehaviour
{

    public GameObject shadowPlane;
    public Transform player;
    public LayerMask shadowLayer;
    public float shadowRadius;
    private float radiusCircle { get { return shadowRadius * shadowRadius; } }

    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        Ray r = new Ray(transform.position, player.position - transform.position);
        RaycastHit hit;

        if(Physics.Raycast(r, out hit, 1000, shadowLayer, QueryTriggerInteraction.Collide))
        {
            for(int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = shadowPlane.transform.TransformPoint(vertices[i]);
                float distance = Vector3.SqrMagnitude(v - hit.point);
                if(distance < radiusCircle)
                {
                    float alpha = Mathf.Min(colors[i].a, distance / radiusCircle);
                    colors[i].a = alpha;
                }
            }
            UpdateColors();
        }

    }

    public void Initialize()
    {
        mesh = shadowPlane.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        colors = new Color[vertices.Length];
        for(int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.black;
        }
        UpdateColors();
    }
    public void UpdateColors()
    {
        mesh.colors = colors;
    }
}
