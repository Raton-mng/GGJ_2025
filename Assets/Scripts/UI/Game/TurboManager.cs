using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class TurboManager : MonoBehaviour
{
    [SerializeField] private GameObject _bar;
    [SerializeField] private float _maxTurbo;
    [SerializeField] private float _consumption;
    private float _turbo;
    private RectTransform _anchorMax;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _anchorMax = _bar.GetComponent<RectTransform>();
    }

    private void Start()
    {
        _turbo = _maxTurbo;
    }

    // Update is called once per frame
    void Update()
    {
        if(_turbo < _maxTurbo)
        {
        }
        _anchorMax.anchorMax = new(0.5f, _turbo / _maxTurbo);
    }

    public float GetTurbo()
    {
        return _turbo;
    }

    public void UseTurbo ()
    {
        _turbo -= _consumption * Time.deltaTime;
    }
}
