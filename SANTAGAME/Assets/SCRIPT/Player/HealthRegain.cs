using UnityEngine;

public class HealthRegain : MonoBehaviour
{
    [Header("Health Reference")]
    public Santaplayer player;   // drag Player here

    [Header("Regen Settings")]
    public int regenThreshold = 30;   // regen only below this
    public float regenDelay = 2f;     // seconds between regen
    public int regenAmount = 3;       // health per tick

    private float timer;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (player == null)
            player = GetComponent<Santaplayer>();
    }

    void Update()
    {
        if (player == null) return;

        // check idle animation
        bool isIdle = anim.GetCurrentAnimatorStateInfo(0).IsName("idlep");

        // regen condition
        if (isIdle &&
            player.PlCurrentHealth > 0 &&
            player.PlCurrentHealth < regenThreshold)
        {
            timer += Time.deltaTime;

            if (timer >= regenDelay)
            {
                player.Heal(regenAmount); // ✅ updates slider internally
                timer = 0f;
            }
        }
        else
        {
            timer = 0f; // reset if moving / jumping / above threshold
        }
    }
}
