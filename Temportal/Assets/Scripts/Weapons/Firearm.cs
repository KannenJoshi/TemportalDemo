using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Firearm : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform barrel;
    [SerializeField] private GameObject projectile;
    
    [Header("Basic")]
    [SerializeField] private int damage = 10;
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private float reloadTime = 2.0f; //Seconds
    [SerializeField] private float fireRate = 5.0f; //Shots per second
    [SerializeField] private float adsZoom = 2.0f;
    [SerializeField] private float recoilForce = 2.0f;
    
    [Header("Advanced")]
    [SerializeField] private float adsFireRate = 5.0f;
    [SerializeField] private bool holdFire = true;
    [SerializeField] private int projectileSpeed = 1000; //ms-1

    // Recoil Pattern Plugin?

    protected int ammoCount;
    private float _timeBetweenShots;
    // TODO: Set bullet prefab to be default here

    void Start()
    {
        _timeBetweenShots = 1 / fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (ammoCount == 0 && !IsReloading)
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

        if (context.canceled)
        {
            _timeBetweenShots = 1 / fireRate;
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

    protected virtual void Fire()
    {
        CreateProjectile();
        
        // 
        --ammoCount;
        // TODO: Apply Recoil
        
        // TODO: NEED TO CHECK ???
        IsReady = false;
        // DONT DO COROUTINE
        StartCoroutine(StaggerShots());
    }

    protected virtual void CreateProjectile()
    {
        //new Bullet(projectileSpeed, projectileRange); //: Needs collider, die on collision with any surface, die when past range
        GameObject newBullet = Instantiate(projectile, barrel.position, barrel.rotation);
        newBullet.GetComponent<Rigidbody>().AddForce(newBullet.transform.forward.normalized * projectileSpeed);
    }
    
    /*
     * COROUTINES
     */
    protected IEnumerator StaggerShots()
    {
        yield return new WaitForSeconds(_timeBetweenShots);
        IsReady = true;
    }

    protected IEnumerator ReloadDelay()
    {
        yield return new WaitForSeconds(reloadTime);
        ammoCount = magazineSize;
        IsReloading = false;
    }
    
    /*
     * PROPERTIES
     */
    protected bool IsReady { get; set; } = false;
    protected bool IsShooting { get; set; }
    protected bool IsReloading { get; set; }

    public float AdsZoom => adsZoom;
}
