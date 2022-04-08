using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private Transform head;
    // Start is called before the first frame update
    void Start()
    {

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
        if (regenerate && Hp < HpMax && Time.time > _lastHit + healAfterDamageDelay)
        {
            Hp += healAmount * Time.unscaledDeltaTime;
            Hp = Mathf.Min(Hp, HpMax);
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
