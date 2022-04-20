using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public int score = 5;
    public ParticleSystem explosionParticle;
    public float hp = 10f;
    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            ParticleSystem instance = Instantiate(explosionParticle, transform.position, transform.rotation);
            Destroy(instance.gameObject, instance.main.duration); // 재생이 다 되면 파괴한다. (메모리 낭비 방지)

            AudioSource explosionAudio = GetComponent<AudioSource>();
            explosionAudio.Play();
            instance.Play();

            GameManager.instance.AddScore(score);

            // 매 스테이지에 굉장히 많은 프롭들이 필요한데, 매번 새로 생성, 파괴 하면 렉이 많이 걸림.
            // 그래서 화면에 보이지 않게 임시로 꺼 두고, 스테이지 전환시 재사용한다.
            gameObject.SetActive(false);
        }
    }
}
