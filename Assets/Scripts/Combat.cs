using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;


public class Combat : MonoBehaviour
{
    public GameObject player;
    public GameObject prefabSphereMelee;

    public static GameObject _Pistol;
    public static GameObject _Lasergun;
    public static GameObject _Shotgun;
    public static GameObject _Flamethrower;
    public static GameObject _DisabledSword;
    public static GameObject _TriggerSword;
    
    private Transform spawnPointSphereMelee;
    
    public string enemyTag = "Enemy";
    public int currentWeapon = 0;
    public int previousWeapon = 0;
    public int dimension = 1;
    public bool isDimensionChanged = false;
    public float lastTravelTime = -1f;
    public float timeTemp = -1f;
    public bool isReloading = false;
    public float timeTempReload = -1f;
    public int fireDelayTemp = -1;
    public bool breakFlag = false;
    public bool canTravel = false;
    
    public MouseButton buttonToShoot = MouseButton.Left;
    public KeyCode buttonToSwitchDimension = KeyCode.Tab;
    public KeyCode buttonToMainWeapon = KeyCode.Alpha1;
    public KeyCode buttonToAdditionalWeapon = KeyCode.Alpha2;
    public KeyCode buttonToSword = KeyCode.Alpha3;
    
    public HighTechGun lasergun, pistol, disabledSword, highTechSword, steamPunkSword;
    public SteamPunkGun shotgun, flamethrower;

    private List<HighTechGun> guns;
    public List<GameObject> loot;
    
    public class HighTechGun
    {
        public float fireRate;
        public float lastFireTime;
        public GameObject gunObject;
        public bool isAvailable;
        public float distance;
        public int damage;

        public HighTechGun(float fireRate, GameObject gunObject, bool isAvailable, float distance, int damage)
        {
            this.fireRate = fireRate;
            lastFireTime = -fireRate;
            this.gunObject = gunObject;
            this.isAvailable = isAvailable;
            this.distance = distance;
            this.damage = damage;
        }
    }

    public class SteamPunkGun : HighTechGun 
    {
        public float radius;
        public int ammo;

        public SteamPunkGun(float fireRate, GameObject gunObject, bool isAvailable, float distance, int damage,
            float radius, int ammo) : base(fireRate, gunObject, isAvailable, distance, damage)
        {
            lastFireTime = -fireRate;
            this.radius = radius;
            this.ammo = ammo;
        }
    }
    
    /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// ///
     
    void Start()
    {
        
        player = GameObject.FindGameObjectWithTag("Player");
        //hand = GameObject.FindGameObjectWithTag("Hand");
        
        _Pistol = GameObject.FindGameObjectWithTag("_Pistol");
        _Lasergun = GameObject.FindGameObjectWithTag("_Lasergun");
        _Shotgun = GameObject.FindGameObjectWithTag("_Shotgun");
        _Flamethrower = GameObject.FindGameObjectWithTag("_Flamethrower");
        _DisabledSword = GameObject.FindGameObjectWithTag("_DisabledSword");
        _TriggerSword = GameObject.FindGameObjectWithTag("_TriggerSword");
        
        
        lasergun = new HighTechGun(7f, GameObject.FindGameObjectWithTag("Lasergun"),
            false, 100f, 100);
        
        pistol = new HighTechGun(0.5f, GameObject.FindGameObjectWithTag("Pistol"),
            false, 100f, 8);
        
        shotgun = new SteamPunkGun(1f, GameObject.FindGameObjectWithTag("Shotgun"), false,
            12f, 50, 3f,  4);
        
        flamethrower = new SteamPunkGun(0.25f, GameObject.FindGameObjectWithTag("Flamethrower"), false,
            7f, 5, 3.5f,  100);
        
        disabledSword = new HighTechGun(1f, GameObject.FindGameObjectWithTag("DisabledSword"), false,
            3f, 34);
        
        highTechSword = new HighTechGun(1f, GameObject.FindGameObjectWithTag("HighTechSword"), false,
            3f, 34);
        
        steamPunkSword = new HighTechGun(1f, GameObject.FindGameObjectWithTag("SteamPunkSword"), false,
            3f, 34);
        
        loot = new List<GameObject>{_Pistol, _Lasergun, _Shotgun, _Flamethrower, _DisabledSword};
        guns = new List<HighTechGun>{pistol, shotgun, lasergun, flamethrower, highTechSword, steamPunkSword, disabledSword};
    }
    
    void Update()
    {
        EchoInputs();
        if ((Time.time > timeTemp + disabledSword.fireRate / 2f) && (timeTemp != -1f))
        {
            Destroy(prefabSphereMelee.gameObject);
            timeTemp = -1f;
        }

        if ((Time.time > timeTempReload + 2f) && (timeTemp != -1f))
        {
            timeTempReload = -1f;
            isReloading = false;
        }

        if ((fireDelayTemp > -1) && (Time.time >= 0.1f + flamethrower.lastFireTime))
        {
            fireDelayTemp--;
            Conecast(flamethrower);
        }
        print(guns[0]);

        if ((!canTravel && !disabledSword.isAvailable) && (Vector3.Distance(player.transform.position, _TriggerSword.transform.position) <= 1.5f))
        {
            canTravel = true;
            disabledSword.isAvailable = false;
            DisableAllWeapon();
            highTechSword.isAvailable = true;
            steamPunkSword.isAvailable = true;
        }
            
        if (!(lasergun.isAvailable && pistol.isAvailable && shotgun.isAvailable && flamethrower.isAvailable &&
              highTechSword.isAvailable && steamPunkSword.isAvailable))
        {
            foreach (GameObject lootable in loot)
            {
                if (Vector3.Distance(player.transform.position, lootable.transform.position) <= 1.5f)
                {
                    foreach (HighTechGun gun in guns)
                    {
                        if ((lootable.tag).Substring(1) == gun.gunObject.tag)
                        {
                            loot.Remove(lootable);
                            gun.isAvailable = true;
                            breakFlag = true;
                            
                            if (gun == disabledSword)
                            
                            break;
                        }
                    }
                    Destroy(lootable);
                }

                if (breakFlag)
                {
                    breakFlag = false;
                    break;
                }
            }
        }

        if ((currentWeapon !=  previousWeapon) || (isDimensionChanged))
        {
            lasergun.lastFireTime = Time.time - lasergun.fireRate + 5.5f;
            pistol.lastFireTime = Time.time - pistol.fireRate + 0.3f;
            shotgun.lastFireTime = Time.time - shotgun.fireRate + 0.6f;
            disabledSword.lastFireTime = Time.time - disabledSword.fireRate + 0.5f;
            highTechSword.lastFireTime = Time.time - highTechSword.fireRate + 0.5f;
            steamPunkSword.lastFireTime = Time.time - steamPunkSword.fireRate + 0.5f;
            previousWeapon = currentWeapon;
            if (isDimensionChanged) isDimensionChanged = false;
        }
        
    }

    /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// ///
    
    void EchoInputs()
    {
        print(guns[FindWeaponID()].gunObject.tag);
        if (Input.GetKeyDown(buttonToMainWeapon) && (currentWeapon != 3) && (guns[FindWeaponID()].isAvailable) && !isReloading && (fireDelayTemp < 3))
        {
            currentWeapon = 3;
            DisableAllWeapon();
            guns[FindWeaponID()].gunObject.GetComponent<MeshRenderer>().enabled = true;
        }
        else if (Input.GetKeyDown(buttonToAdditionalWeapon) && (currentWeapon != 1) && (guns[FindWeaponID()].isAvailable) && !isReloading && (fireDelayTemp < 3))
        {
            currentWeapon = 1;
            DisableAllWeapon();
            guns[FindWeaponID()].gunObject.GetComponent<MeshRenderer>().enabled = true;
        }
        else if (Input.GetKeyDown(buttonToSword) && (currentWeapon != 5) && (guns[FindWeaponID()].isAvailable) && !isReloading && (fireDelayTemp < 3))
        {
            currentWeapon = 5;
            DisableAllWeapon();
            guns[FindWeaponID()].gunObject.GetComponent<MeshRenderer>().enabled = true;
        }

        if (Input.GetKeyDown(buttonToSwitchDimension) && canTravel)
            SwitchDimension();
            
        
        if (Input.GetMouseButtonDown((int)buttonToShoot))
            if (!isReloading) Shoot();
        
    }

    void DisableAllWeapon()
    {
        foreach (HighTechGun gun in guns)
        {
            print(gun.gunObject.tag);
            gun.gunObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    
    void Shoot()
    {
        switch (FindWeaponID())    // Сам разбирайся в этой магии, если ты сюда залез xD
        {
            case 1: //пистолет
                if (HighTechAttack(pistol)); //проверки для звука атаки и эффектов
                break;
            
            case 2: //дробовик
                if (SteamPunkAttack(shotgun));
                break;
            
            case 3: //лазерган
                if (HighTechAttack(lasergun));
                break;
            
            case 6: //огнемет
                if (SteamPunkAttack(flamethrower));
                break;
            
            case 5: //HT меч (или выключенный)
                if (SwordAttack(highTechSword));
                break;
            
            case 10: //SP меч
                if (SwordAttack(steamPunkSword));
                break;
        }
    }

    bool SwordAttack(HighTechGun gunObject)
    {
        if (disabledSword.isAvailable)
        {
            if (Time.time > disabledSword.lastFireTime + disabledSword.fireRate)
            {
                JustSpherecast();
            }
            else if (Time.time > gunObject.lastFireTime + gunObject.fireRate)
            {
                JustSpherecast();
            }
            return true;
        }
        return false;
    }
    
    void JustSpherecast()
    {
        Vector3 spawnPosition = spawnPointSphereMelee.position + spawnPointSphereMelee.forward * 2f;
        GameObject sphereMelee = Instantiate(prefabSphereMelee, spawnPosition, Quaternion.identity);
        sphereMelee.transform.SetParent(player.transform);
        timeTemp = Time.time;
    }

    bool HighTechAttack(HighTechGun gunObject)
    {
        if (Time.time > gunObject.lastFireTime + gunObject.fireRate)
        {
            gunObject.lastFireTime = Time.time;
                    
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, gunObject.distance) && hit.collider.CompareTag(enemyTag))
            {
                
            }
            return true;
        }
        return false;
    }
    
    bool SteamPunkAttack(SteamPunkGun gunObject)
    {
        if (!isReloading && (gunObject.ammo <= 0) && (gunObject == pistol)) // ЗВУК ПЕРЕЗАРЯДКИ ПИСТОЛЯ + АНИМАЦИЯ
        {
            isReloading = true;
            timeTempReload = Time.time;
        }
        else if ((gunObject == flamethrower) && (gunObject.ammo == 0)) // ПАР
        {
            fireDelayTemp = -1;
        }
        else if ((Time.time > gunObject.lastFireTime + gunObject.fireRate) && (gunObject == shotgun))
        {
            gunObject.lastFireTime = Time.time;
            Conecast(gunObject);
            return true;
        }
        else if ((fireDelayTemp == -1) && (gunObject == flamethrower))
        {
            fireDelayTemp = 9;
        }
        return false;
    }

    void Conecast(SteamPunkGun gunObject)
    {
        gunObject.lastFireTime = Time.time;
        
        float coneRadius = gunObject.distance * Mathf.Tan(gunObject.radius * Mathf.Deg2Rad);
        Vector3 coneDirection = Camera.main.transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(Camera.main.transform.position, coneRadius, coneDirection,
            gunObject.distance);

        gunObject.ammo -= 1;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag(enemyTag))
            {

            }
        }
    }
    
    int FindWeaponID()
    {
        switch (currentWeapon * dimension) // Сам разбирайся в этой магии, если ты сюда залез xD
        {
            case 1: //пистолет
                return 0;

            case 2: //дробовик
                return 1;

            case 3: //лазерган
                return 2;

            case 6: //огнемет
                return 3;

            case 5: //HT меч (или выключенный)
                return 4;

            case 10: //SP меч
                return 5;
            
            default:
                return 6;
        }
    }

    void SwitchDimension()
    {
        if ((steamPunkSword.isAvailable && highTechSword.isAvailable) && (Time.time > lastTravelTime + 0.5f))
        {
            dimension = (dimension == 1) ? 2 : 1;
            steamPunkSword.gunObject.SetActive(dimension == 1);
            highTechSword.gunObject.SetActive(dimension == 2);
            isDimensionChanged = true;
        }
    }
}



