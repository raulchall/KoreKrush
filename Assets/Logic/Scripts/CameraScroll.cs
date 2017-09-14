using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CameraScroll : MonoBehaviour
{
    public float scrollForceMultiplier;

    private Vector2? prev;
    private Vector2? current;
    private bool mouseDown;
    private Rigidbody2D body;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        if (Input.GetMouseButtonDown(1))
        {
            prev = Input.mousePosition;
            mouseDown = true;
        }
        else if (Input.GetMouseButton(1))
            current = Input.mousePosition;
        else if (Input.GetMouseButtonUp(1))
        {
            current = Input.mousePosition;
            mouseDown = false;
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    prev = touch.position;
                    mouseDown = true;
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    current = touch.position;
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    current = touch.position;
                    mouseDown = false;
                    break;
            }
        }
#endif
    }

    void FixedUpdate()
    {
        if (prev.HasValue)
            if (current.HasValue)
                if (mouseDown)
                {
                    body.MovePosition(body.position - (current - prev).Value * scrollForceMultiplier);
                    prev = current;
                }
                else
                {
                    body.velocity = (prev - current).Value * scrollForceMultiplier / Time.deltaTime;
                    prev = current = null;
                }
            else
                body.velocity = Vector2.zero;
    }
}
