using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float xPlane = 3f;

    private bool _isGoingUp;

    public int myID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Update());

        IEnumerator Update()
        {
            while(transform.position.x < xPlane)
            {
                transform.position += (Vector3)Vector2.right * Time.deltaTime * speed;
                yield return 0;
            }
        }
    }

    private void Update()
    {
        if (_isGoingUp) transform.position += (Vector3)Vector2.up * Time.deltaTime;
        else transform.position += (Vector3)Vector2.down * Time.deltaTime;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _isGoingUp = context.action.WasPerformedThisFrame();
    }

    public void OnMenuing(InputAction.CallbackContext context)
    {
        Debug.Log(context.action.ReadValue<float>());
    }
}
