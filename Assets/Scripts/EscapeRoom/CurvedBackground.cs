using UnityEngine;

public class CurvedBackground : MonoBehaviour 
{
    [SerializeField] private int segments = 20;
    [SerializeField] private float radius = 10f;
    [SerializeField] private float height = 5f;
    [SerializeField] private Material backgroundMaterial;
    [SerializeField] private bool stretchTexture = true;  // Option to stretch or clamp texture

    void Start() 
    {
        CreateCurvedMesh();
    }

    void CreateCurvedMesh() 
    {
        Mesh mesh = new Mesh();
        
        Vector3[] vertices = new Vector3[(segments + 1) * 2];
        Vector2[] uvs = new Vector2[(segments + 1) * 2];
        int[] triangles = new int[segments * 6];

        for (int i = 0; i <= segments; i++) 
        {
            float angle = ((float)i / segments) * Mathf.PI;
            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;

            vertices[i * 2] = new Vector3(x, -height/2, z);
            vertices[i * 2 + 1] = new Vector3(x, height/2, z);

            if (stretchTexture)
            {
                // Stretched UVs
                uvs[i * 2] = new Vector2((float)i / segments, 0);
                uvs[i * 2 + 1] = new Vector2((float)i / segments, 1);
            }
            else
            {
                // Clamped UVs to prevent wrapping
                float u = Mathf.Clamp01((float)i / segments);
                uvs[i * 2] = new Vector2(u, 0);
                uvs[i * 2 + 1] = new Vector2(u, 1);
            }

            if (i < segments) 
            {
                int baseIndex = i * 6;
                int vertIndex = i * 2;
                
                triangles[baseIndex] = vertIndex;
                triangles[baseIndex + 1] = vertIndex + 1;
                triangles[baseIndex + 2] = vertIndex + 2;
                
                triangles[baseIndex + 3] = vertIndex + 1;
                triangles[baseIndex + 4] = vertIndex + 3;
                triangles[baseIndex + 5] = vertIndex + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        
        // Setup material
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = backgroundMaterial;
        
        // Set texture wrap mode to clamp
        if (backgroundMaterial.mainTexture != null)
        {
            backgroundMaterial.mainTexture.wrapMode = TextureWrapMode.Clamp;
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            CreateCurvedMesh();
        }
    }
}