using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class CastleHealth : MonoBehaviour
{
    [FormerlySerializedAs("maximumHealth")]
    [SerializeField, Min(1f)] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image healthFill;
    [SerializeField] private UIManager uiManager;

    private bool isDefeated;

    public float MaximumHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    private void Start()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = maxHealth;

        if (uiManager == null)
            uiManager = UIManager.Instance != null ? UIManager.Instance : FindObjectOfType<UIManager>();

        if (healthSlider == null)
            Debug.LogError("[CastleHealth] Assign UI/TOP_BARPANEL/SLIDER to the Health Slider field.", this);

        RefreshHealthBar();
    }

    public void TakeDamage(int damage)
    {
        TakeDamage((float)damage);
    }

    public void TakeDamage(float damage)
    {
        if (isDefeated || damage <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        RefreshHealthBar();

        if (currentHealth == 0f)
        {
            isDefeated = true;
            if (uiManager == null)
            {
                Debug.LogError("[CastleHealth] Castle reached zero health, but no UIManager is available.", this);
                return;
            }

            uiManager.ToggleLoosePanel();
        }
    }

    [ContextMenu("Test Take 20 Damage")]
    private void TestTake20Damage()
    {
        TakeDamage(20);
    }

    private void RefreshHealthBar()
    {
        float healthRatio = Mathf.Clamp01(currentHealth / maxHealth);
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
            healthSlider.SetValueWithoutNotify(currentHealth);
        }

        Image sliderFill = healthSlider != null && healthSlider.fillRect != null
            ? healthSlider.fillRect.GetComponent<Image>()
            : null;

        if (sliderFill != null && sliderFill.type == Image.Type.Filled)
            sliderFill.fillAmount = healthRatio;

        if (healthFill != null && healthFill.type == Image.Type.Filled)
            healthFill.fillAmount = healthRatio;
    }
}
