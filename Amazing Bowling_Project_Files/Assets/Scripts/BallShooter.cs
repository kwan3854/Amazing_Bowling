using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallShooter : MonoBehaviour
{
    public CamFollow cam;
    public Rigidbody ball;
    public Transform firePosition;
    public Slider powerSlider;
    public AudioSource shootingAudio;
    public AudioClip fireClip;
    public AudioClip chargingClip;
    public float minForce = 15f;
    public float maxForce = 30f;
    public float chargingTime = 0.75f;

    private float currentForce;
    private float chargeSpeed;
    private bool fired;
    
    private void OnEnable() // 꺼졌다 켜졌을 때 마다 실행됨
    {
        currentForce = minForce;
        fired = false;
        powerSlider.value = minForce;
    }

    private void Start()
    {
        chargeSpeed = (maxForce - minForce) / chargingTime;
    }

    private void Update()
    {
        if (fired) // 한번이라도 발사하면 동작을 막음
        {
            return;
        }

        powerSlider.value = minForce;

        if (currentForce >= maxForce && !fired) // 힘을 최대로 채웠고, 발사되지 않았다면
        {
            currentForce = maxForce;
            Fire();
        }
        else if (Input.GetButtonDown("Fire1")) // 발사 버튼을 처음 눌렀을 때
        {
            fired = false; // 연사 가능하게 한다.
            currentForce = minForce;

            shootingAudio.clip = chargingClip;
            shootingAudio.Play();
        }
        else if (Input.GetButton("Fire1") && !fired) // 발사 버튼을 누르고 있을 때
        {
            currentForce = currentForce + chargeSpeed * Time.deltaTime;
            powerSlider.value = currentForce;
        }
        else if (Input.GetButtonUp("Fire1") && !fired) // 발사 버튼에서 손을 떼었을 때
        {
            Fire();
        }

    }

    private void Fire()
    {
        fired = true;

        Rigidbody ballInstance = Instantiate(ball, firePosition.position, firePosition.rotation);
        ballInstance.velocity = currentForce * firePosition.forward;

        shootingAudio.clip = fireClip;
        shootingAudio.Play();

        currentForce = minForce;

        cam.SetTarget(ballInstance.transform, CamFollow.State.Tracking);
    }

}
