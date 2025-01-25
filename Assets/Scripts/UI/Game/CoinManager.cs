using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _coins;
    private int _coinNumber;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _coinNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _coins.text = "X" + _coinNumber;
    }

    public int GetCoins()
    {
        return _coinNumber;
    }

    public void EarnCoins(int value)
    {
        _coinNumber += value;
    }

    public void UseCoins(int value)
    {
        _coinNumber -= value;
    }
}
