using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleController : MonoBehaviour {

    Vector2 input;
    Vector3 vec;
    float onePercentHeight, curPercent, totalHeight;
    public GameObject scaleNum;
    private TextMesh scaleTextMesh;


    // Use this for initialization
    void Start () {
        scaleTextMesh = scaleNum.GetComponent<TextMesh>();

        totalHeight = transform.localScale[1];
        onePercentHeight = totalHeight / 100;
		
	}
    

    // Update is called once per frame
    void Update () {
        vec = new Vector3(0, 0, 0);
		if (Input.GetKey(KeyCode.DownArrow))
        {
            vec = new Vector3(0, -onePercentHeight, 0);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            vec = new Vector3(0, onePercentHeight, 0);
        }
        else
        {
            input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

            vec = new Vector3(0, input[1] * onePercentHeight, 0);
        }
       

       
            transform.localScale += vec;
            transform.position += vec;
            curPercent = transform.localScale[1] / 0.7f;
        
        scaleTextMesh.text = String.Format("{0:P0}", curPercent);

    }
}
