using UnityEngine;

public class CurbManager : MonoBehaviour
{
    [SerializeField] private float speed = 0.01f;
    static public CurbManager m_Instance;
    private void Awake()
    {
        m_Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(isActiveAndEnabled)
            transform.position += (Vector3) Vector2.right * speed * Time.deltaTime;
    }
}
