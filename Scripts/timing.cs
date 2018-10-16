using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timing : MonoBehaviour {
    float timer;
    // button functionality commented out
    //public Button button;

    void Start() {
        //Button b = button.GetComponent<Button>();
        //b.onClick.AddListener(OnClick);
        timer = 0.0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetKeyDown("space"))
        {
            Debug.Log(timer);
            timer = 0.0f;
        }
    }

    // Listener for button click
    void OnClick() {
        Debug.Log(timer);
        timer = 0.0f;
    }
}
