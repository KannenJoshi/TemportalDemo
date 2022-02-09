using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Firearm : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform holder;
    
    [Header("Basic")]
    [SerializeField] private int damage = 10;
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private float reloadTime = 2.0f; //Seconds
    [SerializeField] private float fireRate = 5.0f; //Shots per second
    [SerializeField] private float adsZoom = 2.0f;

    // TODO: Change to have Firearm as SuperScript, and have a SubScript for each of below
    [Header("Shotgun")]
    [SerializeField] private int bulletsPerClick = 1; //
    [SerializeField] private float spread = 0.2f;

    [Header("Burst")]
    [SerializeField] private int bulletsInSuccession = 1;
    // Needs fireRate between burst shots, and fireDelay between clicks

    [Header("Sniper")]
    // TODO: How to do sniper scopes? PLACEHOLDER TYPE
    [SerializeField] private Texture scopeTexture;

    [Header("Explosive")]
    [SerializeField] private Texture rocketModel;
    [SerializeField] private float radius = 0.0f;
    
    [Header("Advanced")]
    [SerializeField] private float adsFireRate = 5.0f;
    [SerializeField] private bool holdFire = true;
    [SerializeField] private int bulletSpeed = 1000; //ms-1
    [SerializeField] private int bulletRange = 200; //meters
    
    // Recoil Pattern Plugin?

    private int _ammoCount;
    private float _timeBetweenShots;
    // TODO: Set bullet prefab to be default here
    private Texture bulletModel;

    void Start()
    {
        _timeBetweenShots = 1 / fireRate;
        // TODO: if rocketModel set then change bulletModel to be rocket model (DO IN EXPLOSIVE FIREARM CLASS)
    }

    // Update is called once per frame
    void Update()
    {
        if (_ammoCount == 0 && !IsReloading)
        {
            Reload();
        }

        if (IsShooting)
        {
            if (IsReady)
            {
                Fire();
    
                // if tapFire then after shot one bullet must wait until next click
                if (!holdFire)
                {
                    IsShooting = false;
                }
            }
            else
            {
                // Wait for `1/FireRate`seconds then
                //IsReady = true;
            }
        }
    }
    
    /*
     * INPUT ACTIONS
     */
    public void Reload(InputAction.CallbackContext context)
    {
        if (context.performed) Reload();
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed && IsReady && !IsReloading && !IsShooting)
        {
            IsShooting = true;
        }

        if (context.canceled)
        {
            IsShooting = false;
        }
    }

    public void ADS(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _timeBetweenShots = 1 / adsFireRate;
            // Have ADS in Player Controller which uses the gun's aim value
        }
    }
    
    /*
     * PRIVATE METHODS
     */
    private void Reload()
    {
        // TODO: NEED TO CHECK ???
        IsReloading = true;
        StartCoroutine(ReloadDelay());
    }

    private void Fire()
    {
        // TODO: Create Bullet(Vector3 origin, Quaternion direction, int speed, int maxRange): Needs collider, die on collision with any surface, die when past range
        // 
        --_ammoCount;
        // TODO: Apply Recoil
        
        // TODO: NEED TO CHECK ???
        IsReady = false;
        StartCoroutine(StaggerShots());
    }
    
    /*
     * COROUTINES
     */
    IEnumerator StaggerShots()
    {
        yield return new WaitForSeconds(_timeBetweenShots);
        IsReady = true;
    }

    IEnumerator ReloadDelay()
    {
        yield return new WaitForSeconds(reloadTime);
        _ammoCount = magazineSize;
        IsReloading = false;
    }
    
    /*
     * PROPERTIES
     */
    public bool IsReady { get; set; } = false;
    private bool IsShooting { get; set; }
    private bool IsReloading { get; set; }

    public float AdsZoom => adsZoom;
}
