using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMeshSplinePipe;
using System.Linq;

public class Main : MonoBehaviour {

    [SerializeField]
    private MeshPipe mpipe;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private GameObject debugObj;

    [SerializeField]
    private GameObject sneakerObj;

    private int frameCountOutter = 0;
    private int frameCount = 0;
    private float startTime = 0f;
    private float time = 0.01f;

    private Texture2D texture;
    private float[] samples = new float[500000];

    private int imageWidth = 256;

    private Material sneakerMat;

    // Use this for initialization
    void Start () {
        startTime = Time.timeSinceLevelLoad;

        texture = new Texture2D(1, imageWidth);
        texture.SetPixels(Enumerable.Range(0, imageWidth).Select(_ => Color.clear).ToArray());
        texture.Apply();

        mpipe.gameObject.GetComponent<MeshRenderer>().materials[0].mainTexture = texture;
        //debugObj.GetComponent<MeshRenderer>().materials[0].mainTexture = texture;

        sneakerMat = sneakerObj.GetComponent<MeshRenderer>().materials[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (mpipe.sp == null)
        {
            return;
        }

        var diff = Time.timeSinceLevelLoad - startTime;
        if(frameCountOutter % 5 == 0)
        {
            if (diff > time)
            {
                frameCount++;
                if (frameCount < mpipe.sp.positions.Length - 1)
                {
                    startTime = Time.timeSinceLevelLoad;
                }
                else
                {
                    frameCount = 0;
                    frameCountOutter = 0;
                    mpipe.UpdateMesh(false);
                }
            }
        }
        frameCountOutter++;

        var rate = diff / time;

        if (frameCount < mpipe.sp.positions.Length - 1)
        {
            Vector3 pos = Vector3.Lerp(mpipe.sp.positions[frameCount], mpipe.sp.positions[frameCount + 1], rate);
            Vector3 lookAt = Vector3.Lerp(mpipe.sp.lookAts[frameCount], mpipe.sp.lookAts[frameCount + 1], rate);

            cam.transform.position = pos;
            cam.transform.LookAt(lookAt);
        }

        if (audioSource.isPlaying)
        {
            audioSource.clip.GetData(samples, audioSource.timeSamples);

            int textureX = 0;
            int skipSamples = 200;
            float maxSample = 0;

            for (int i = 0, l = samples.Length; i < l && textureX < imageWidth; i++)
            {
                maxSample = Mathf.Max(maxSample, samples[i]);

                if (i % skipSamples == 0)
                {
                    texture.SetPixel(0, textureX, new Color(maxSample, 0, 0));
                    sneakerMat.SetFloat("_NoiseVal", maxSample*5.0f);
                    maxSample = 0;
                    textureX++;
                }
            }

            texture.Apply();
        }
    }
}
