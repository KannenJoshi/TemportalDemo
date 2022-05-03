using System.Collections;
using UnityEngine;

namespace Weapons
{
    public class Firearm : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform barrel;
        [SerializeField] private GameObject projectile;
        [SerializeField] private Rigidbody holder;
    
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
        [SerializeField] private float timeToLive = 10.0f;

        // Recoil Pattern Plugin?

        [SerializeField] protected int ammoCount;

        private float _timeBetweenShots;
        // TODO: Set bullet prefab to be default here

        private PlayerHUD hud;

        void Awake()
        {
            holder = transform.root.gameObject.GetComponent<Rigidbody>();
            
            _timeBetweenShots = 1 / fireRate;
        }
    
        void Start()
        {
            ammoCount = magazineSize;

            if (transform.root.CompareTag("Player"))
            {
                hud = GameObject.Find("HUD").GetComponent<PlayerHUD>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            // AMMO
            if (ammoCount <= 0 && !IsReloading)
            {
                Reload();
            }

            // AIMING
            if (IsAiming) _timeBetweenShots = 1 / adsFireRate;
            else _timeBetweenShots = 1 / fireRate;

            // SHOOTING
            if (IsShooting && !IsReloading)
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
    
        private void ADS()
        {
            if (IsAiming) _timeBetweenShots = 1 / adsFireRate;
            else _timeBetweenShots = 1 / fireRate;
        }
    
        /*
     * PRIVATE METHODS
     */

        public void Reload()
        {
            // TODO: NEED TO CHECK ???
            IsReloading = true;
            if (transform.root.CompareTag("Player")) hud.ShowReload(ReloadTime);
            StartCoroutine(ReloadDelay());
        }

        protected virtual void Fire()
        {
            CreateProjectile();
        
            // 
            --ammoCount;
            // TODO: Apply Recoil
            holder.gameObject.GetComponent<Entity>().ApplyRecoilTorque(recoilForce);
        
            // TODO: NEED TO CHECK ???
            IsReady = false;
            // DONT DO COROUTINE
            StartCoroutine(StaggerShots());
        }

        protected virtual void CreateProjectile()
        {
            //new Bullet(projectileSpeed, projectileRange); //: Needs collider, die on collision with any surface, die when past range
            GameObject newBullet = Instantiate(projectile, barrel.position, barrel.rotation);
            newBullet.GetComponent<Bullet>().SetStats(this.damage, transform.root.tag);
        
            Rigidbody brb = newBullet.GetComponent<Rigidbody>();
            brb.AddForce(newBullet.transform.forward.normalized * projectileSpeed * brb.mass / Time.timeScale + barrel.forward);
        
            Destroy(newBullet, timeToLive);
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
            yield return new WaitForSecondsRealtime(reloadTime);
            ammoCount = magazineSize;
            IsReloading = false;
        }
    
        /*
     * PROPERTIES
     */
        public bool IsReady { get; set; } = false;
        public bool IsShooting { get; set; }
        public bool IsReloading { get; set; }
        public bool IsAiming { get; set; }
        public bool IsMagazineFull => ammoCount == magazineSize;
        public int AmmoMax => magazineSize;
        public int AmmoCount => ammoCount;
        public float AdsZoom => adsZoom;
        public float ReloadTime => reloadTime;
        public float FireDelay => _timeBetweenShots;
    }
}
