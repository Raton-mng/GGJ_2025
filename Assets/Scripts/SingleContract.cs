using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SingleContract : MonoBehaviour
{
    public enum ContractEffect
    {
        None,
        SlowOpponent,
        BoostPlayer,
        ReverseControls
    }

    public ContractEffect effectType;
    private bool isSelected = false;

    public void SetEffect(ContractEffect effect, Sprite sprite)
    {
        effectType = effect;
        GetComponent<Image>().sprite = sprite;
    }

    public void SelectContract(int playerId)
    {
        isSelected = true;
        print(GetIsSelected());
        StartCoroutine(FadeOutAndDestroy());
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
}
