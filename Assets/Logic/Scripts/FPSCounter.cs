using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public int maxCount = 200;

    private Text text;
    private float time;
    private int Count;

    private string[] counts;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();

        counts = new string[maxCount];

        for (var i = 0; i < maxCount; i++)
            counts[i] = "FPS: " + i;

        time = Time.time;
        Count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Count++;
        
        if (!(Time.time > time + 1)) return;
        
        time = Time.time;
        text.text = Count < counts.Length ? counts[Count] : "FPS: " + Count;
        Count = 0;
    }
}
