using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AirForceOne : MonoBehaviour {

    [SerializeField]
    private GameObject model;

    // Use this for initialization
    void Start () {
        
    }

    // Update is called once per frame
    void Update()
    {
        model.gameObject.transform.Rotate(1f, 90 * Time.deltaTime * 0.3f, 180 * Time.deltaTime*0.5f);
    }
}
