using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private Entity entity;
    [SerializeField] private bool isEnemyBar = true;
    private Slider slider;

    private float sliderVal;
    private float hpVal;

    void Awake()
    {
        slider = GetComponent<Slider>();
        if (Camera.main != null) cam = Camera.main.transform;
        if (entity == null) entity = GetComponentInParent<Entity>();

        slider.maxValue = entity.HpMax;
        slider.minValue = 0;
    }

    void Update()
    {
        //slider.value = entity.Hp;
        if (!Mathf.Approximately(entity.Hp,slider.value))
        {
            hpVal = entity.Hp;
            sliderVal = slider.value;
        }
        
        slider.value = Mathf.Lerp(sliderVal, hpVal, 10f * Time.unscaledDeltaTime);
        //slider.value = slider.value +  ;
    }
    
    void LateUpdate()
    {
        if (isEnemyBar)
        {
            transform.LookAt(transform.position + cam.forward);
        }
    }
}
