using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public int maxCount = 150;

    private Text text;
    private float time;
    private int Count;

    private string[] counts;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();

        counts = new string[maxCount];

        for (int i = 0; i < maxCount; i++)
            counts[i] = "FPS\n" + i;

        time = Time.time;
        Count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Count++;
        if (Time.time > time + 1)
        {
            time = Time.time;
            text.text = counts[Count];
            Count = 0;
        }
    }
}
