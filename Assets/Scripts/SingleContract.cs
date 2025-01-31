using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SingleContract : MonoBehaviour
{
    public enum ContractEffect
    {
        None,
        BoostBoost,
        MoreLife,
        ReverseControls,
        PushEveryoneUp,
        ImmediateJumpUp,
        ImmediateJumpDown
    }

    public ContractEffect effectType;
    private bool isSelected = false;
    private int coast;

    private PlayerManager _playerManager;

    private void Start()
    {
        _playerManager = PlayerManager.Instance;
    }

    public void SetEffect(ContractEffect effect, Sprite sprite, int coast)
    {
        effectType = effect;
        GetComponent<Image>().sprite = sprite;
        this.coast = coast;
    }
    public void SelectContract(int playerID)
    {
        isSelected = true;
        print(GetIsSelected());
        StartCoroutine(FadeOutAndDestroy());
        switch (effectType)
        {
            case ContractEffect.BoostBoost :
                BoostBoost(playerID);
                break;
            case ContractEffect.MoreLife :
                MoreLife(playerID);
                break;
            case ContractEffect.ReverseControls :
                ReverseControls(playerID);
                break;
           /* case ContractEffect.PushEveryoneUp :
                PushEveryoneUp(playerID);
                break;*/
            case ContractEffect.ImmediateJumpUp :
                ImmediateJumpUp(playerID);
                break;
            default :
                ImmediateJumpDown(playerID);
                break;
        }
    }

    public bool GetIsSelected()
    {
        return isSelected;
    }

    public int GetCoast()
    {
        return coast;
    }

    private IEnumerator FadeOutAndDestroy()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        CanvasGroup canvasGroup = gameObject.AddComponent<CanvasGroup>();
        float duration = 0.5f;
        float elapsed = 0f;

        Vector3 originalScale = rectTransform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            rectTransform.localScale = Vector3.Lerp(originalScale, originalScale * 1.2f, t);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void BoostBoost(int playerID)
    {
        PlayerController player =  _playerManager.GetPlayer(playerID);
        StartCoroutine(player.BoostTheBoost(2, 5));
    }
    
    private void MoreLife(int playerID)
    {
        PlayerController player =  _playerManager.GetPlayer(playerID);
        player.GetHealthManager().AddHealth();
    }
    
    private void ReverseControls(int playerID)
    {
        List<PlayerController> otherPlayers = _playerManager.GetOtherPlayers(playerID);
        foreach (PlayerController otherPlayer in otherPlayers)
        {
            StartCoroutine(otherPlayer.InverseControls(6));
        }
    }
    
    /*private void PushEveryoneUp(int playerID)
    {
        List<PlayerController> otherPlayers = _playerManager.GetOtherPlayers(playerID);
        foreach (PlayerController otherPlayer in otherPlayers)
        {
            StartCoroutine(otherPlayer.PushUp(6, 2));
        }
    }*/
    
    private void ImmediateJumpUp(int playerID)
    {
        PlayerController otherPlayer = _playerManager.GetRandomPlayer(playerID);
        otherPlayer.transform.localPosition += 7 * Vector3.up;
    }
    
    private void ImmediateJumpDown(int playerID)
    {
        PlayerController otherPlayer = _playerManager.GetRandomPlayer(playerID);
        otherPlayer.transform.localPosition += 7 * Vector3.down;
    }
}
