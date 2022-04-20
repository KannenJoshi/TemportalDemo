using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

public class PlayerHUD : MonoBehaviour
{
    /*
     [Header("Bars")]
    [SerializeField] private float healthCurrent;
    [SerializeField] private float healthMax;
    [SerializeField] private float temporalMeterCurrent;
    [SerializeField] private int temporalMeterMax;
    */
    
    [Header("Numbers")]
    [SerializeField] private TextMeshProUGUI ammoCount;
    [SerializeField] private TextMeshProUGUI ammoMax;
    [SerializeField] private TextMeshProUGUI score;

    [Header("Indicators")]
    [SerializeField] private Image LPortal;
    [SerializeField] private Image RPortal;
    [SerializeField] private Image reloadCircle;
    [SerializeField] private Image primary;
    [SerializeField] private Image secondary;

    [Header("Menus")]
    [SerializeField] private GameObject pauseMenu;
    
    private Portal leftPortal;
    private Portal rightPortal;

    private float reloadTime;

    private GameObject primaryGun;
    private GameObject secondaryGun;

    private void Awake()
    {
        var portals = GameObject.FindGameObjectsWithTag("Portal");
        leftPortal = portals[0].GetComponent<Portal>();
        rightPortal = portals[1].GetComponent<Portal>();

        var player = GameObject.FindGameObjectWithTag("Player");
        var hand = player.transform.GetChild(1).GetChild(0);
        primaryGun = hand.GetChild(0).gameObject;
        secondaryGun = hand.GetChild(1).gameObject;
    }

    private void Update()
    {
        // Portal Indicator
        var tempColour = LPortal.color;
        tempColour.a = leftPortal.IsPlaced ? 1f : 0.2f;
        LPortal.color = tempColour;
        
        tempColour = RPortal.color;
        tempColour.a = rightPortal.IsPlaced ? 1f : 0.2f;
        RPortal.color = tempColour;

        // Reload Circle
        if (reloadCircle.enabled)
        {
            reloadCircle.fillAmount += Time.unscaledDeltaTime / reloadTime;
            if (reloadCircle.fillAmount >= 1.0f) reloadCircle.enabled = false;
        }
        
        // Gun Equipped
        tempColour = primary.color;
        tempColour.a = primaryGun.activeSelf ? 1f : 0.5f;
        primary.color = tempColour;
        
        tempColour = secondary.color;
        tempColour.a = secondaryGun.activeSelf ? 1f : 0.5f;
        secondary.color = tempColour;

        // Score
        score.text = "Score: " + ScoreCounter.Score;
    }

    public void SetAmmo(int count) { ammoCount.text = count.ToString(); }
    public void SetAmmoMax(int max) { ammoMax.text = max.ToString(); }

    public void ShowReload(float reloadTime)
    {
        reloadCircle.enabled = true;
        reloadCircle.fillAmount = 0.0f;
        this.reloadTime = reloadTime;
    }

    public void PauseMenu(bool isPaused)
    {
        pauseMenu.SetActive(isPaused);
        //if (i
    }
}
