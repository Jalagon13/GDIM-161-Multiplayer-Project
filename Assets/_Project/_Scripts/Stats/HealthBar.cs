using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _fillImage;

    public void UpdateFill(int currentHp, int maxHp)
    {
        _fillImage.fillAmount = Mathf.Clamp01(Mathf.InverseLerp(0, maxHp, currentHp));
    }
}
