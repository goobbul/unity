using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Weapon : MonoBehaviour
{
    [Header("Gun Sound")]
    public AudioClip gunClip;
    public AudioSource gunSource;
    
    [Header("Reload Sound")]
    public AudioSource reloadSource;
    
    public int damage;

    public Camera camera;

    public float fireRate;

    [Header("VFX")]
    public GameObject hitVFX;
    
    private float nextFire;

    [Header("Ammo")]
    public int mag = 5;

    public int ammo = 30;
    public int magAmmo = 30;

    [Header("UI")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;

    [Header("Animation")]
    public Animation animation;
    public AnimationClip reload;

    [Header("Recoil Settings")]
    //[Range(0, 1)]
    //public float recoilPercent = 0.3f;

    [Range(0, 2)]
    public float recoverPercent = 0.7f;

    [Space]
    public float recoilUp = 1f;

    public float recoilBack = 0f;




    private Vector3 originalPosition;
    private Vector3 recoilVelocity = Vector3.zero;

    private float recoilLength;
    private float recoverLenth;


    private bool recoiling;
    public bool recovering;
    
    private void Start()
    {
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;

        originalPosition = transform.localPosition;


        recoilLength = 0;
        recoverLenth = 1 / fireRate * recoverPercent;
    }


    // Update is called once per frame
    void Update()
    {
        if (nextFire > 0)
            nextFire -= Time.deltaTime;


        if (Input.GetButton("Fire1") && nextFire <= 0 && ammo > 0 && animation.isPlaying == false)
        {
            nextFire = 1 / fireRate;

            ammo--;
            
            magText.text = mag.ToString();
            ammoText.text = ammo + "/" + magAmmo;

            Fire();
        }

        if (Input.GetKeyDown(KeyCode.R) && mag > 0 && animation.isPlaying == false)
        {
            Reload();
        }

        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;

        if (ammo == 0)
        {
            ammoText.text = "총알 없음";
            ammoText.color = Color.red;
        }
        else
        {
            ammoText.color = Color.white;
        }
        
        if (mag == 0)
        {
            magText.text = "탄창 없음";
            magText.color = Color.red;
        }
        else
        {
            magText.color = Color.white;
        }

        if (recoiling)
        {
            Recoil();
        }

        if (recovering)
        {
            Recovering();
        }
    }


    void Reload()
    {
        animation.Play(reload.name);
        ReloadSound();
            
        if (mag > 0)
        {
            mag--;

            ammo = magAmmo;
        }
    }

    void ReloadSound()
    {
        if (reloadSource.isPlaying == false)
        {
            reloadSource.mute = false;
            reloadSource.loop = false;
            reloadSource.playOnAwake = false;
            reloadSource.Play();
        }
    }
    
    void GunSound()
    {
            gunSource.mute = false;
            gunSource.loop = false;
            gunSource.playOnAwake = false;
            gunSource.PlayOneShot(gunClip);
    }
    
    void Fire()
    {
        recoiling = true;
        recovering = false;
        
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f))
        {
            PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);
            
            GunSound();
            
            if (hit.transform.gameObject.GetComponent<Health>().isLocalPlayer)
            {
                return;
            }
            else
            {
                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }
        }
    }


    void Recoil()
    {
        Vector3 finalPosition = new Vector3(originalPosition.x, originalPosition.y + recoilUp, originalPosition.z - recoilBack);


        transform.localPosition =
            Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoilLength);


        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = true;
        }
    }
    
    void Recovering()
    {
        Vector3 finalPosition = originalPosition;


        transform.localPosition =
            Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoverLenth);


        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = false;
        }
    }
}
