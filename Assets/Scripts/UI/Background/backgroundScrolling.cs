using System;
using UnityEngine;
using UnityEngine.UI;

public class backgroundScrolling : MonoBehaviour
{
    [SerializeField]
    private float scrollingSpeed = .69f;
    
    [SerializeField]
    private float zoomFactor = .69f;
    
    [SerializeField]
    private float yMoveFactor = .69f;

    private RawImage background;

    private Camera camera;


    private void Awake()
    {
        background = GetComponent<RawImage>();
        camera = Camera.main;
    }

    private void Update()
    {
        background.uvRect = new(background.uvRect.x + scrollingSpeed * Time.deltaTime, camera.transform.position.y * yMoveFactor,
            camera.orthographicSize * zoomFactor, camera.orthographicSize * zoomFactor);
    }
}
