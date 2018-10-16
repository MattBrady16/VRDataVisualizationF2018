using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DxR;

namespace DxRR
{
    public class VizController : MonoBehaviour
    {
        public GameObject DxrObj;

        // Use this for initialization
        void Start()
        {
            DxrObj = transform.Find("DxrObj").gameObject;
            if (DxrObj != null)
            {
                Debug.Log("DxrObj found!");
            }
            else
            {
                Debug.Log("Error - DxrObj Not Found.");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown("space"))
            {
                Debug.Log("Next graph requested.");

                Vis VisObj = DxrObj.GetComponent(typeof(Vis)) as Vis;
                if (VisObj != null)
                {
                    Debug.Log("Success!");

                    VisObj.UpdateVisSpecsFromTextSpecs();
                }
                else
                {
                    Debug.Log("Failure...");
                }
            }
        }
    }
}
