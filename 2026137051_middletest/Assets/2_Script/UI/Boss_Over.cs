using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Monitors player and boss health and invokes events when either reaches zero.
public class Boss_Over : MonoBehaviour
{
    [Header("Player Health Source")]
    [Tooltip("Assign a PlayerController to read player HP from (preferred).")]
    public PlayerController playerController;
    [Tooltip("Alternative: assign a HPBar (child 'Health') to read player HP from if PlayerController is not used.")]
    public HPBar playerHPBar;

    [Header("Boss Health Source")]
    [Tooltip("Assign the boss HPBar to monitor boss health.")]
    public HPBar bossHPBar;

    [Header("Events")]
    public UnityEvent onPlayerDeath;
    public UnityEvent onBossDeath;

    public GameObject Boss;

    // internal state to ensure events run once
    private bool playerDeathTriggered = false;
    private bool bossDeathTriggered = false;

    void Start()
    {
        // try to auto-assign common references if not set
        if (playerController == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                playerController = p.GetComponent<PlayerController>();
        }

        if (playerHPBar == null && playerController != null)
        {
            var healthT = playerController.transform.Find("Health");
            if (healthT != null)
                playerHPBar = healthT.GetComponent<HPBar>();
            if (playerHPBar == null)
                playerHPBar = playerController.GetComponentInChildren<HPBar>();
        }
    }

    void Update()
    {
        // check player via slider value if available
        if (!playerDeathTriggered)
        {
            Slider pSlider = null;
            if (playerHPBar != null)
                pSlider = playerHPBar.hpSlider;
            else if (playerController != null)
            {
                // try to find Health child
                var ht = playerController.transform.Find("Health");
                if (ht != null)
                {
                    var ph = ht.GetComponent<HPBar>();
                    if (ph != null) pSlider = ph.hpSlider;
                }
            }

            if (pSlider != null && pSlider.value <= 0f)
            {
                playerDeathTriggered = true;
                OverPlayer();
                if (onPlayerDeath != null) onPlayerDeath.Invoke();
            }
        }

        // check boss via slider value
        if (!bossDeathTriggered && bossHPBar != null)
        {
            Slider bSlider = bossHPBar.hpSlider;
            if (bSlider != null && bSlider.value <= 0f)
            {
                bossDeathTriggered = true;
                OverBoss();
                if (onBossDeath != null) onBossDeath.Invoke();
            }
        }
    }

    // Called once when player HP slider reaches zero
    public void OverPlayer()
    {
        SceneManager.LoadScene("Making_5");
    }

    // Called once when boss HP slider reaches zero
    public void OverBoss()
    {
        // Move player to specified coordinates (x=41, y=0). Preserve z.
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 pos = player.transform.position;
            pos.x = 41f;
            pos.y = 0f;
            player.transform.position = pos;
        }
        if (Boss != null)
            Boss.SetActive(false);
    }
}
