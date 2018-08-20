using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityMeshSplinePipe
{
    public class MeshPipe : MonoBehaviour
    {

        [SerializeField]
        private int VerticesCount = 10;

        [SerializeField]
        private float Radius = 1f;

        [SerializeField]
        private List<Vector3> controlls = new List<Vector3>();

        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector3> offsetVertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private List<Vector2> uv = new List<Vector2>();

        private Mesh mesh;
        public Spline sp;

        // Use this for initialization
        void Start()
        {
            mesh = new Mesh();
            sp = new Spline();
            CreateMesh();
            UpdateMesh();
        }

        // Update is called once per frame
        void Update()
        {
            /*
            for (int i = 0; i < sp.controlPoints.Length; i++)
            {
                sp.controlPoints[i] = controlls[i];
            }
            UpdateVerts();
            */
        }

        private void CreateMesh()
        {
            sp.CreateAndUpdateHermitecurve();

            int faceCount = 0;

            float x = 0f;
            float y = 0f;
            float z = 0.1f;

            float centerZ = 0.1f;
            float uvYStep = 1.0f / (float)(sp.TOTAL - sp.numberOfSegments);
            float uvXStep = 1.0f / (float)(this.VerticesCount);

            float xoff = 0f;
            float yoff = 0f;

            for (int i = 0; i < sp.TOTAL - sp.numberOfSegments; i++)
            {
                float additionalRad = Mathf.PerlinNoise(xoff, yoff)*10f;
                for (int j = 0; j < this.VerticesCount; j++)
                {
                    float x2 = 0f;
                    float y2 = 0f;

                    float rad = (90f - (360f / (float)this.VerticesCount) * (j)) * Mathf.Deg2Rad;
                    float rad2 = (90f - (360f / (float)this.VerticesCount) * (j + 1)) * Mathf.Deg2Rad;
                    x = Mathf.Cos(rad) * (Radius+ additionalRad);
                    y = Mathf.Sin(rad) * (Radius+ additionalRad);
                    x2 = Mathf.Cos(rad2) * (Radius+ additionalRad);
                    y2 = Mathf.Sin(rad2) * (Radius+ additionalRad);

                    vertices.Add(new Vector3(x2, y2, z - centerZ));
                    vertices.Add(new Vector3(x, y, z - centerZ));
                    vertices.Add(new Vector3(x, y, z + centerZ));
                    vertices.Add(new Vector3(x2, y2, z + centerZ));

                    offsetVertices.Add(new Vector3(x2, y2, z - centerZ));
                    offsetVertices.Add(new Vector3(x, y, z - centerZ));
                    offsetVertices.Add(new Vector3(x, y, z + centerZ));
                    offsetVertices.Add(new Vector3(x2, y2, z + centerZ));

                    triangles.Add(faceCount * 4); //1
                    triangles.Add(faceCount * 4 + 1); //2
                    triangles.Add(faceCount * 4 + 2); //3
                    triangles.Add(faceCount * 4); //1
                    triangles.Add(faceCount * 4 + 2); //3
                    triangles.Add(faceCount * 4 + 3); //4

                    float uvX1 = (uvXStep * (float)j);
                    float uvX2 = (uvXStep * (float)(j + 1));

                    float uvY1 = (uvYStep * (float)i);
                    float uvY2 = (uvYStep * (float)(i + 1));

                    uv.Add(new Vector2(uvX1, uvY1));
                    uv.Add(new Vector2(uvX2, uvY1));
                    uv.Add(new Vector2(uvX2, uvY2));
                    uv.Add(new Vector2(uvX1, uvY2));

                    faceCount++;

                    xoff += 0.2f;
                    yoff += 0.2f;
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            var filter = GetComponent<MeshFilter>();
            filter.sharedMesh = mesh;
        }

        private void UpdateVerts()
        {
            sp.CreateAndUpdateHermitecurve();
            for (int i = 0; i < sp.TOTAL-sp.numberOfSegments; i++)
            {
                int start = ((this.VerticesCount * 4) * i);
                
                for (int j = 0; j < this.VerticesCount; j++)
                {
                    vertices[start+(j * 4)] = sp.mats[i].MultiplyPoint3x4(offsetVertices[start + (j * 4)]);
                    vertices[start + (j * 4) + 1] = sp.mats[i].MultiplyPoint3x4(offsetVertices[start + (j * 4) + 1]);
                    vertices[start + (j * 4) + 2] = sp.mats[i].MultiplyPoint3x4(offsetVertices[start + (j * 4) + 2]);
                    vertices[start + (j * 4) + 3] = sp.mats[i].MultiplyPoint3x4(offsetVertices[start + (j * 4) + 3]);
                }
            }
            
            // connect between polygons
            for (int i = 0; i < (sp.TOTAL - sp.numberOfSegments) - 1; i++)
            {
                for (int j = 0; j < this.VerticesCount; j++)
                {
                    int prev = ((this.VerticesCount * 4) * i);
                    int now = ((this.VerticesCount * 4) * (i + 1));

                    vertices[now + (j * 4)] = vertices[prev + (j * 4) + 3];
                    vertices[now + (j * 4) + 1] = vertices[prev + (j * 4) + 2];
                }
            }

            mesh.SetVertices(vertices);
            mesh.RecalculateBounds();
        }

        public void UpdateMesh(bool isInit = true)
        {
            float xoff = 0f;
            float yoff = 0f;
            for (int i = 0; i < sp.controlPoints.Length; i++)
            {
                float dx = Mathf.PerlinNoise(xoff, yoff) * (i * 2f);
                if (i < sp.controlPoints.Length/2)
                {
                    dx = dx * 2.0f;
                }

                float dy = Mathf.PerlinNoise(xoff, yoff) * 50f;
                if (i >= 1)
                {
                    sp.controlPoints[i].x = dx;
                    sp.controlPoints[i].y = dy;
                }

                if (isInit)
                {
                    controlls.Add(sp.controlPoints[i]);
                }

                xoff += 0.2f;
                yoff += 0.2f;
            }
            UpdateVerts();
        }

        private float map(float v, float sx, float sn, float dx, float dn)
        {
            return (v - sn) * (dx - dn) / (sx - sn) + dn;
        }

    }
}