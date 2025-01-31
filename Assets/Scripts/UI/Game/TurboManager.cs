using UnityEngine;

public class TurboManager : MonoBehaviour
{
    [SerializeField] private GameObject _bar;
    [SerializeField] private float _maxTurbo;
    [SerializeField] private float _consumption;
    private Animator _turboAnimator;
    private float _turbo;
    private RectTransform _anchorMax;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _anchorMax = _bar.GetComponent<RectTransform>();
        _turboAnimator = GetComponentInChildren<Animator>();
        //Debug.Log(_turboAnimator);
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
            //TODO: Add turbo with area
        }
        _anchorMax.anchorMax = new(0.5f, _turbo / _maxTurbo);
    }

    public float GetTurbo()
    {
        return _turbo;
    }

    public void GainTurbo(float t)
    {
        if(_turbo + t > _maxTurbo)
        {
            _turbo = _maxTurbo;
        }
        else
        {
            _turbo += t;
        }

        if (_turbo < _consumption * Time.deltaTime)
        {
            print("isMin");
            _turboAnimator.SetBool("isMin", false);
        }
        if (_turbo == _maxTurbo)
        {
            print("isMax");
            _turboAnimator.SetBool("isMax", true);
        }
    }

    public void UseTurbo()
    {
        if(_turbo == _maxTurbo)
        {print("isMax");
            _turboAnimator.SetBool("isMax", false);
        }
        _turbo -= _consumption * Time.deltaTime;
        if(_turbo < _consumption * Time.deltaTime)
        {print("isMin");
            _turboAnimator.SetBool("isMin", true);
        }
    }
}
