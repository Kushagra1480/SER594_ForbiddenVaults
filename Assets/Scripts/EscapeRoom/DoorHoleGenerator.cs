using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DoorHoleGenerator : MonoBehaviour
{
    [Header("Door Hole Settings")]
    [SerializeField] private float wallWidth = 10f;
    [SerializeField] private float wallHeight = 8f;
    [SerializeField] private float doorWidth = 2f;
    [SerializeField] private float doorHeight = 3f;
    [SerializeField] private float wallDepth = 0.2f;

    private void Start()
    {
        GenerateWallWithHole();
    }

    private void GenerateWallWithHole()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Calculate vertices for outer and inner rectangles
        Vector3[] vertices = new Vector3[24];  // 4 vertices each for front, back, and 4 inner sides
        Vector2[] uvs = new Vector2[24];
        int[] triangles = new int[72];  // 2 triangles each for 6 quads * 6 faces

        // Wall center offsets
        float halfWidth = wallWidth / 2;
        float halfHeight = wallHeight / 2;
        float halfDoorWidth = doorWidth / 2;
        float halfDoorHeight = doorHeight / 2;
        float halfDepth = wallDepth / 2;

        // Front face vertices (clockwise)
        vertices[0] = new Vector3(-halfWidth, -halfHeight, halfDepth);   // Bottom left
        vertices[1] = new Vector3(halfWidth, -halfHeight, halfDepth);    // Bottom right
        vertices[2] = new Vector3(halfWidth, halfHeight, halfDepth);     // Top right
        vertices[3] = new Vector3(-halfWidth, halfHeight, halfDepth);    // Top left

        // Back face vertices (counter-clockwise)
        vertices[4] = new Vector3(-halfWidth, -halfHeight, -halfDepth);
        vertices[5] = new Vector3(-halfWidth, halfHeight, -halfDepth);
        vertices[6] = new Vector3(halfWidth, halfHeight, -halfDepth);
        vertices[7] = new Vector3(halfWidth, -halfHeight, -halfDepth);

        // Door hole vertices (front face, clockwise)
        vertices[8] = new Vector3(-halfDoorWidth, -halfDoorHeight, halfDepth);
        vertices[9] = new Vector3(halfDoorWidth, -halfDoorHeight, halfDepth);
        vertices[10] = new Vector3(halfDoorWidth, halfDoorHeight, halfDepth);
        vertices[11] = new Vector3(-halfDoorWidth, halfDoorHeight, halfDepth);

        // Door hole vertices (back face, counter-clockwise)
        vertices[12] = new Vector3(-halfDoorWidth, -halfDoorHeight, -halfDepth);
        vertices[13] = new Vector3(-halfDoorWidth, halfDoorHeight, -halfDepth);
        vertices[14] = new Vector3(halfDoorWidth, halfDoorHeight, -halfDepth);
        vertices[15] = new Vector3(halfDoorWidth, -halfDoorHeight, -halfDepth);

        // Generate UVs
        for (int i = 0; i < vertices.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x / wallWidth + 0.5f, vertices[i].y / wallHeight + 0.5f);
        }

        // Generate triangles
        int[] frontFace = new int[] {
            // Bottom section
            0, 8, 1,
            8, 9, 1,
            // Top section
            3, 2, 11,
            11, 2, 10,
            // Left section
            0, 3, 8,
            8, 3, 11,
            // Right section
            9, 10, 1,
            1, 10, 2
        };

        // Copy front face triangles to back face (reversed)
        int[] backFace = new int[frontFace.Length];
        for (int i = 0; i < frontFace.Length; i += 3)
        {
            backFace[i] = frontFace[i] + 4;
            backFace[i + 1] = frontFace[i + 2] + 4;
            backFace[i + 2] = frontFace[i + 1] + 4;
        }

        // Combine all triangles
        System.Array.Copy(frontFace, 0, triangles, 0, frontFace.Length);
        System.Array.Copy(backFace, 0, triangles, frontFace.Length, backFace.Length);

        // Set mesh data
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            GenerateWallWithHole();
        }
    }
}