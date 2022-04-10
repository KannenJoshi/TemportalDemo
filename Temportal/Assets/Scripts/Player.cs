using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Player : Entity
{
    [SerializeField] private Transform head;

    private bool isHealing;
    private List<VisualEffect> healFX;
    [SerializeField] private int bulletTimeMaxDuration = 5;
    [SerializeField] private float bulletTimeResource = 5.0f;
    [SerializeField] private float bulletTimeRegenDelay = 3.0f;
    [SerializeField] private float bulletTimeRegenOverTime = 8.0f;
    public TimeManager timeManager;
    private float _lastEndBulletTime;
    private bool _lastBulletTimeState;
    
    
    void Start()
    {
        healFX = new List<VisualEffect>();
        
        var hfx = GameObject.FindGameObjectsWithTag("Heal FX");
        if (hfx.Length == 0) print("No FX");
        foreach (var fx in hfx)
        {
            healFX.Add(fx.GetComponent<VisualEffect>());
        }
        toggleHealFX(false);
    }

    private void toggleHealFX(bool state)
    {
        healFX.ForEach(e => {if (state) e.Play(); else e.Stop();});
    }

    // Update is called once per frame
    protected override void UpdateBehaviour()
    {
        // Don't change if game paused
        if (Time.timeScale == 0f) return;
        
        // Regen Trigger
        if (!timeManager.IsBulletTime && bulletTimeResource < bulletTimeMaxDuration &&
            Time.time > _lastEndBulletTime + bulletTimeRegenDelay)
        {
            bulletTimeResource += BulletTimeResourceMax * Time.deltaTime / bulletTimeRegenOverTime;
        }
        // If in BT decrease resource meter
        else if (timeManager.IsBulletTime)
        {
            bulletTimeResource -= 1 * Time.unscaledDeltaTime;
        }
        // Ensure within range
        bulletTimeResource = Mathf.Clamp(bulletTimeResource, 0, bulletTimeMaxDuration);

        // If depleted end
        if (bulletTimeResource == 0) timeManager.IsBulletTime = false;
        
        // If got deactivated last Tick, update tracker to delay regen
        if (timeManager.IsBulletTime == false && timeManager.IsBulletTime != _lastBulletTimeState)
        {
            _lastEndBulletTime = Time.time;
        }
        
        // Track last state of BT
        _lastBulletTimeState = timeManager.IsBulletTime;
    }

    protected override void Die()
    {
        
    }
    
    protected override void Heal()
    {
        var oldIsHealing = isHealing;
        isHealing = false;
        if (regenerate && Hp < HpMax && Time.time > _lastHit + healAfterDamageDelay)
        {
            isHealing = true;
            Hp += healAmount * Time.unscaledDeltaTime;
            Hp = Mathf.Min(Hp, HpMax);
        }

        if (isHealing != oldIsHealing)
        {
            toggleHealFX(isHealing);
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
