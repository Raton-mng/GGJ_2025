using UnityEngine;

public class TurboManager : MonoBehaviour
{
    public static TurboManager Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
