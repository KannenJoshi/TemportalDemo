using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform cam;
    private Entity entity;
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        entity = GetComponentInParent<Entity>();

        slider.maxValue = entity.HpMax;
        slider.minValue = 0;
    }

    void Update()
    {
        slider.value = entity.Hp;
    }
    
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
