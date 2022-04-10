using UnityEngine;
using UnityEngine.UI;

public class BulletTimeBar : MonoBehaviour
{
    [SerializeField] private Player player;
    
    private Slider slider;

    private float sliderVal;
    private float bulletTimeVal;

    void Awake()
    {
        slider = GetComponent<Slider>();
        if (player == null) player = GameObject.FindWithTag("Player").GetComponent<Player>();

        slider.maxValue = player.BulletTimeResourceMax;
        slider.minValue = 0;
    }

    void Update()
    {
        if (!Mathf.Approximately(player.BulletTimeResource,slider.value))
        {
            bulletTimeVal = player.BulletTimeResource;
            sliderVal = slider.value;
        }
        
        slider.value = Mathf.Lerp(sliderVal, bulletTimeVal, 10f * Time.unscaledDeltaTime);
        //slider.value = slider.value +  ;
    }
}