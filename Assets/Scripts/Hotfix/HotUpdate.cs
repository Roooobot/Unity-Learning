using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotUpdate : MonoBehaviour
{
    Image image;
    bool a = false;
    void Start() {
        image = FindObjectOfType<Image>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            a = !a;
        if (a)
            image.color = Color.green;
        else
            image.color = Color.red;
    }
}


