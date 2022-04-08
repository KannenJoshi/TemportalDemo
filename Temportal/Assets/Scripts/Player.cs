using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Player : Entity
{
    [SerializeField] private Transform head;

    private bool isHealing;
    private List<VisualEffect> healFX;
    
    
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
        base.UpdateBehaviour();
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

}
