using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;


/*
 * Привет, вот небольшой гайд если заблудился:
 * 
 * 1.1) Для коректной работы скрипта к объекту игрока должен быть прикреплен невидимый объект,
 * по объему больше, чем сам объект "Игрок". Этот объект толжен иметь тег "Looter"
 * 1.2) Объкеты подбираемые игроком должны иметь теги "_Pistol", "_Lasergun", "_Shotgun",
 * "_Flamethrower" и "_Sword" соответсвенно
 * 
 * 2.1) К объектну игрока должен быть прикреплены объекты с тегами "Lasergun", "Pistol", "Shotgun",
 * "Flamehrower", "DisabledSword", "HighTechSword" и "SteamPunkSword" соответсвенно
 * 2.2) Эти объекты С САМОГО НАЧАЛА должны быть выключены!!!!
 *
 * 3.1) Переменная отслеживающая измерение |public int dimension|
 *      Смысл на значение:
 *      1 - high-tech
 *      2 - steampunk
 * 3.2) Оружие отслеживается при помощи |public int currentWeapon|
 *      Смысл на значение:
 *      0 - отсутсвие оружия (в самом начале)
 *      1 - пистолет/дробовик
 *      3 - лазерган/огнемет
 *      5 - меч
 *
 * 4.1) Методы снизу мэйна
 * 4.2) В метод |switchDimension()| добавить реализацию перехода между мирами
 *                  (планово переадресация на метод ветки World)
 * 
 *
 *
 *
 *
 *                                                                                                                                                                                    Если ты это читаешь, вероятно я сплю (*^.^*)  
 */

public class Combat : MonoBehaviour
{
    public GameObject player = GameObject.FindGameObjectWithTag("Player");
    public GameObject prefabSphereMelee;
    public GameObject hand = GameObject.FindGameObjectWithTag("Hand");
    
    public Transform spawnPointSphereMelee;
    
    public string enemyTag = "Enemy";
    public int currentWeapon = 0;
    public int previousWeapon = 0;
    public int dimension = 1;
    public bool isDimensionChanged = false;
    public float timeTemp = -1f;
    public bool isReloading = false;
    public float timeTempReload = -1f
    
    public MouseButton buttonToShoot = MouseButton.Left;
    public KeyCode buttonToSwitchDimension = KeyCode.Tab;
    public KeyCode buttonToMainWeapon = KeyCode.Alpha1;
    public KeyCode buttonToAdditionalWeapon = KeyCode.Alpha2;
    public KeyCode buttonToSword = KeyCode.Alpha3;

    
    public HighTechGun lasergun, pistol;
    public SteamPunkGun shotgun, flamethrower;
    public BladeGun disabledSword, highTechSword, steamPunkSword;

    private List<HighTechGun> guns;
    
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
        public bool isFireGun;
        public float radius;
        public int ammo;

        public SteamPunkGun(float fireRate, GameObject gunObject, bool isAvailable, float distance, int damage,
            bool isFireGun, float radius, int ammo) : base(fireRate, gunObject, isAvailable, distance, damage)
        {
            this.isFireGun = isFireGun;
            lastFireTime = -fireRate;
            this.radius = radius;
            this.ammo = ammo;
        }
    }
    
    public class BladeGun : HighTechGun
    {
        public float abilityRate;
        public float lastAbilityTime;
        public bool dimensionID;
        public float range;

        public BladeGun(float fireRate, GameObject gunObject, bool isAvailable, float distance, int damage,
            float abilityRate, bool dimensionID, float range) : base(fireRate, gunObject, isAvailable, distance, damage)
        {
            this.abilityRate = abilityRate;
            lastAbilityTime = -abilityRate;
            lastFireTime = -fireRate;
            this.dimensionID = dimensionID;
            this.range = range;
        }
    }
    
    /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// ///
     
    void Start()
    {
        lasergun = new HighTechGun(7f, GameObject.FindGameObjectWithTag("Lasergun"),
            false, 100f, 100);
        
        pistol = new HighTechGun(0.5f, GameObject.FindGameObjectWithTag("Pistol"),
            false, 100f, 8);
        
        shotgun = new SteamPunkGun(1f, GameObject.FindGameObjectWithTag("Shotgun"), false,
            12f, 50, false, 3f,  4);
        
        flamethrower = new SteamPunkGun(0.25f, GameObject.FindGameObjectWithTag("Flamehrower"), false,
            7f, 50, true, 3.5f,  10000);
        
        disabledSword = new BladeGun(1f, GameObject.FindGameObjectWithTag("DisabledSword"), false,
            3f, 34, 0.5f, false, 3f);
        
        highTechSword = new BladeGun(1f, GameObject.FindGameObjectWithTag("HighTechSword"), false,
            3f, 50, 0.5f, false, 3f);
        
        steamPunkSword = new BladeGun(1f, GameObject.FindGameObjectWithTag("SteamPunkSword"), false,
            3f, 50, 0.5f, true, 3f);
        
        List<HighTechGun> guns = new List<HighTechGun>{lasergun, pistol, shotgun, flamethrower, disabledSword, highTechSword, steamPunkSword};
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
        
        
            
        
        




        if (!(lasergun.isAvailable && pistol.isAvailable && shotgun.isAvailable && flamethrower.isAvailable &&
              disabledSword.isAvailable && highTechSword.isAvailable && steamPunkSword.isAvailable))
        {

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

    void Reloading()
    {
        
    }

    void EchoInputs()
    {
        if (Input.GetKeyDown(buttonToMainWeapon) && (currentWeapon != 3) && (guns[FindWeaponID()].isAvailable) && !isReloading)
        {
            currentWeapon = 3;
            DisableAllWeapon();
            guns[FindWeaponID()].gunObject.SetActive(true);
        }
        else if (Input.GetKeyDown(buttonToAdditionalWeapon) && (currentWeapon != 1) && (guns[FindWeaponID()].isAvailable) && !isReloading)
        {
            currentWeapon = 1;
            DisableAllWeapon();
            guns[FindWeaponID()].gunObject.SetActive(true);
        }
        else if (Input.GetKeyDown(buttonToSword) && (currentWeapon != 5) && (guns[FindWeaponID()].isAvailable) && !isReloading)
        {
            currentWeapon = 5;
            DisableAllWeapon();
            guns[FindWeaponID()].gunObject.SetActive(true);
        }
        
        if (Input.GetKeyDown(buttonToSwitchDimension))
            SwitchDimension();
        
        if (Input.GetMouseButtonDown((int)buttonToShoot))
            if (!isReloading) Shoot();
        
    }

    void DisableAllWeapon()
    {
        foreach (HighTechGun gun in guns)
        {
            gun.gunObject.SetActive(false);
        }
    }
    
    void Shoot()
    {
        switch (FindWeaponID())    // Сам разбирайся в этой магии, если ты сюда залез xD
        {
            case 1: //пистолет
                if (HighTechAttack(pistol)); //проверки для звука атаки
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

    bool SwordAttack(BladeGun gunObject)
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
        else if (Time.time > gunObject.lastFireTime + gunObject.fireRate)
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
            return true;
        }
        return false;
    }
    
    int FindWeaponID()
    {
        return currentWeapon * dimension;
    }

    void SwitchDimension()
    {
        if (steamPunkSword.isAvailable && highTechSword.isAvailable)
        {
            dimension = (dimension == 1) ? 2 : 1;
            steamPunkSword.gunObject.SetActive(dimension == 1);
            highTechSword.gunObject.SetActive(dimension == 2);
            isDimensionChanged = true;
        }
    }
    
}



