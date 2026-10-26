using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonJuice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Idle Pulse")]
    [SerializeField] private bool idlePulse = true;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float pulseAmount = 0.05f; // Small size change

    [Header("Hover Zoom")]
    [SerializeField] private float hoverScale = 1.1f; // Grown size on hover
    [SerializeField] private float transitionSpeed = 12f;

    private Vector3 startScale;
    private Vector3 targetScale;
    private bool isHovered = false;

    void Start()
    {
        startScale = transform.localScale;
        targetScale = startScale;
    }

    void Update()
    {
        if (isHovered)
        {
            // Hover state: quickly scale up
            targetScale = startScale * hoverScale;
        }
        else if (idlePulse)
        {
            // Idle state: gently breathe
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            targetScale = startScale * (1f + pulse);
        }
        else
        {
            targetScale = startScale;
        }

        // Smoothly interpolate to target scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}