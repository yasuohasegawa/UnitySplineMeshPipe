using UnityEngine;

namespace UnityMeshSplinePipe
{
    // 曲線のベースは、以下を流用し、改修、ベクトルを求めるための、微分あたり追加。
    // https://en.wikibooks.org/wiki/Cg_Programming/Unity/Hermite_Curves
    public class Spline
    {
        public int maxPoints = 150; // アンカーポイントの数
        public int numberOfSegments = 10; // アンカーポイントとアンカーポイントの間の、頂点の数
        public int TOTAL;

        public float segMentDIstance = 150.0f;

        //[SerializeField]
        private GameObject[] debugControlPoints;
        private bool isDebug = false;

        public Vector3[] controlPoints { get; set; }// 注意:アンカーポイントの指定は、 断面の数/(アンカーポイントの数-1)で、割り切れる数で、指定
        public Vector3[] positions { get; set; }
        public Vector3[] lookAts { get; set; }
        public Matrix4x4[] mats { get; set; }

        private Vector3 scale = new Vector3(1, 1, 1);

        public Spline()
        {
            TOTAL = maxPoints * numberOfSegments;

            // コントロールポイントの追加
            if (isDebug)
            {
                controlPoints = new Vector3[debugControlPoints.Length];
                for (int i = 0; i < debugControlPoints.Length; i++)
                {
                    controlPoints[i] = debugControlPoints[i].transform.position;
                }
            }
            else
            {
                // プログラム側で直接指定する場合は、ここで、指定

                controlPoints = new Vector3[maxPoints];
                for (int i = 0; i < controlPoints.Length; i++)
                {
                    Vector3 pos = Vector3.zero;
                    pos.z = (float)i * segMentDIstance;
                    //pos.x = UnityEngine.Random.Range(-1.0f, 1.0f);
                    controlPoints[i] = pos;
                }
            }

            int len = TOTAL;
            positions = new Vector3[len];
            lookAts = new Vector3[len];
            mats = new Matrix4x4[len];

            for(int i = 0; i< mats.Length; i++)
            {
                mats[i] = Matrix4x4.identity;
            }

            if (isDebug)
            {
                //CreateAndUpdateHermitecurve(false);
            }
        }

        // 疑似ボーンの更新処理(サブスレッドで、実行) TODO:enumを見て、transformを更新しないときは、更新しない処理
        public void CreateAndUpdateHermitecurve(bool isUpdate = true)
        {
            if (numberOfSegments < 2)
            {
                numberOfSegments = 2;
            }

            if (isDebug)
            {
                for (int i = 0; i < debugControlPoints.Length; i++)
                {
                    controlPoints[i] = debugControlPoints[i].transform.position;
                }
            }

            // loop over segments of spline
            Vector3 p0;
            Vector3 p1;
            Vector3 m0;
            Vector3 m1;

            int last = controlPoints.Length - 1;

            int ind = 0;

            for (int j = 0; j < last; j++)
            {
                // check control points
                if (controlPoints[j] == null ||
                    controlPoints[j + 1] == null ||
                    (j > 0 && controlPoints[j - 1] == null) ||
                    (j < controlPoints.Length - 2 &&
                        controlPoints[j + 2] == null))
                {
                    return;
                }

                // determine control points of segment
                p0 = controlPoints[j];
                p1 = controlPoints[j + 1];
                if (j > 0)
                {
                    m0 = 0.5f * (controlPoints[j + 1] - controlPoints[j - 1]);
                }
                else
                {
                    m0 = controlPoints[j + 1] - controlPoints[j];
                }

                if (j < controlPoints.Length - 2)
                {
                    m1 = 0.5f * (controlPoints[j + 2] - controlPoints[j]);
                }
                else
                {
                    m1 = controlPoints[j + 1] - controlPoints[j];
                }

                // set points of Hermite curve
                Vector3 position;
                Vector3 derivertive;
                float t;
                float pointStep = 1.0f / numberOfSegments;
                if (j == controlPoints.Length - 2)
                {
                    pointStep = 1.0f / (numberOfSegments - 1.0f);
                    // last point of last segment should reach p1
                }

                for (int i = 0; i < numberOfSegments; i++)
                {
                    t = i * pointStep;
                    position = (2.0f * t * t * t - 3.0f * t * t + 1.0f) * p0
                        + (t * t * t - 2.0f * t * t + t) * m0
                        + (-2.0f * t * t * t + 3.0f * t * t) * p1
                        + (t * t * t - t * t) * m1;

                    // first derivertive 上の式をtについて微分(ベクトルを取得するため)
                    derivertive = (6f * t * t - 6f * t) * p0
                        + (3f * t * t - 4f * t + 1f) * m0
                        + (-6f * t * t + 6f * t) * p1
                        + (3f * t * t - 2f * t) * m1;

                    Vector3 velocity = derivertive;
                    Vector3 direction = velocity.normalized;

                    if (ind < positions.Length)
                    {
                        positions[ind] = position;
                        lookAts[ind] = position + direction;

                        Quaternion rotation = Quaternion.LookRotation(lookAts[ind]);
                        mats[ind].SetTRS(position, rotation, scale);
                        //Debug.Log(position+","+t);
                        //Matrix4x4 m = Matrix4x4.LookAt(positions[ind], lookAts[ind], Vector3.forward);
                        //mats[ind] = m;

                        ind++;
                    }
                }
            }
            //Debug.Log(ind);
        }

    }
}