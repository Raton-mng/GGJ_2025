using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SingleContract : MonoBehaviour
{
    public enum ContractEffect
    {
        None,
        BoostBoost,
        MoreLife,
        ReverseControls,
        PushEveryoneUp,
        ImmediateJump,
        Steal
    }

    public ContractEffect effectType;
    private bool isSelected = false;

    private PlayerManager _playerManager;

    private void Start()
    {
        _playerManager = PlayerManager.Instance;
    }

    public void SetEffect(ContractEffect effect, Sprite sprite)
    {
        effectType = effect;
        GetComponent<Image>().sprite = sprite;
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
            case ContractEffect.PushEveryoneUp :
                PushEveryoneUp(playerID);
                break;
            case ContractEffect.ImmediateJump :
                ImmediateJump(playerID);
                break;
            default :
                Steal(playerID);
                break;
        }
    }

    public bool GetIsSelected()
    {
        return isSelected;
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
        
    }
    
    private void ReverseControls(int playerID)
    {
        
    }
    
    private void PushEveryoneUp(int playerID)
    {
        
    }
    
    private void ImmediateJump(int playerID)
    {
        
    }
    
    private void Steal(int playerID)
    {
        
    }
}
