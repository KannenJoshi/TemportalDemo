using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VFX;

public class Player : Entity
{
    [SerializeField] private Transform head;
    [SerializeField] private int bulletTimeMaxDuration = 5;
    [SerializeField] private float bulletTimeResource = 5.0f;
    [SerializeField] private float bulletTimeRegenDelay = 3.0f;
    [SerializeField] private float bulletTimeRegenOverTime = 8.0f;

    private bool _isHealing;
    private List<VisualEffect> _healFX;
    private float _lastEndBulletTime;
    private bool _lastBulletTimeState;

    private static GameObject _instance;
    public static GameObject Instance => _instance;

    protected override void Awake()
    {
        base.Awake();
        
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = gameObject;
        }
    }

    void Start()
    {
        _healFX = new List<VisualEffect>();
        
        var hfx = GameObject.FindGameObjectsWithTag("Heal FX");
        if (hfx.Length == 0) print("No FX");
        foreach (var fx in hfx)
        {
            _healFX.Add(fx.GetComponent<VisualEffect>());
        }
        toggleHealFX(false);
    }

    private void toggleHealFX(bool state)
    {
        _healFX.ForEach(e => {if (state) e.Play(); else e.Stop();});
    }

    // Update is called once per frame
    protected override void UpdateBehaviour()
    {
        // Don't change if game paused
        if (Time.timeScale == 0f) return;
        
        // Regen Trigger
        if (!TimeManager.isBulletTime && bulletTimeResource < bulletTimeMaxDuration &&
            Time.time > _lastEndBulletTime + bulletTimeRegenDelay)
        {
            bulletTimeResource += BulletTimeResourceMax * Time.deltaTime / bulletTimeRegenOverTime;
        }
        // If in BT decrease resource meter
        else if (TimeManager.isBulletTime)
        {
            bulletTimeResource -= 1 * Time.unscaledDeltaTime;
        }
        // Ensure within range
        bulletTimeResource = Mathf.Clamp(bulletTimeResource, 0, bulletTimeMaxDuration);

        // If depleted end
        if (bulletTimeResource == 0) TimeManager.isBulletTime = false;
        
        // If got deactivated last Tick, update tracker to delay regen
        if (TimeManager.isBulletTime == false && TimeManager.isBulletTime != _lastBulletTimeState)
        {
            _lastEndBulletTime = Time.time;
        }
        
        // Track last state of BT
        _lastBulletTimeState = TimeManager.isBulletTime;
    }

    protected override void Die()
    {
        GameOverMenu.GameOver();
    }
    
    protected override void Heal()
    {
        var oldIsHealing = _isHealing;
        _isHealing = false;
        if (regenerate && Hp < HpMax && Time.time > _lastHit + healAfterDamageDelay)
        {
            _isHealing = true;
            //Hp += healAmount * Time.unscaledDeltaTime;
            Hp += healAmount * Time.deltaTime;
            Hp = Mathf.Min(Hp, HpMax);
        }

        if (_isHealing != oldIsHealing)
        {
            toggleHealFX(_isHealing);
        }
    }

    public override void ApplyRecoilTorque(float torque)
    {
        //base.ApplyRecoilTorque(torque);
        // TODO: ROTATE OVER TIME?
        //var localRot = head.localRotation.eulerAngles;
        //head.localRotation = Quaternion.Euler(localRot - new Vector3(torque, 0, 0));
    }

    public int BulletTimeResourceMax => bulletTimeMaxDuration;
    public float BulletTimeResource => bulletTimeResource;
}
