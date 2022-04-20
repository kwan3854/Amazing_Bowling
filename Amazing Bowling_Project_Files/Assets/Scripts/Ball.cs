using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public LayerMask whatIsProp;
    public ParticleSystem explosionParticle;
    public AudioSource explosionAudio;

    public float maxDamage = 100f;
    public float explosionForce = 1000f;
    public float LifeTime = 10f; // 맵 밖으로 나가면 파괴되지 않는 문제가 생길 수 있으므로 수명을 제한한다.
    public float explosionRadius = 20f;

    void Start()
    {
        Destroy(gameObject, LifeTime);
    }

    private void OnDestroy()
    {
        GameManager.instance.OnBallDestroy();
    }

    void OnTriggerEnter(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, whatIsProp);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            Prop prop = targetRigidbody.GetComponent<Prop>();

            if (!prop)
                continue;

            prop.TakeDamage(CalculateDamage(targetRigidbody.position));
        }

        // 부모 오브젝트를 파괴하면 자식 오브젝트도 파괴된다.
        // 공을 먼저 파괴시키고, 자식 게임오브젝트인 PlasmaExplosionEffect 가 파괴되지 않게 해야 한다.
        // 따라서 공 오브젝트를 파괴시킬 때, PlasmaExplosionEffect 를 Ball 의 자식에서 해제시킨다.
        explosionParticle.transform.parent = null;

        explosionParticle.Play();
        explosionAudio.Play();

        GameManager.instance.OnBallDestroy(); //공이 땅에 닿았을 때 isRoundActive 를 false 로 하게 한다.

        Destroy(explosionParticle.gameObject, explosionParticle.main.duration); // 재생이 다 되면 파괴한다. (메모리 낭비 방지)
        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosiion)
    {
        Vector3 explosionToTarget = targetPosiion - transform.position;
        float distance = explosionToTarget.magnitude;
        float edgeToCenterDistance = explosionRadius - distance;
        float percentage = edgeToCenterDistance/explosionRadius;
        float damage = maxDamage * percentage;
        damage = Mathf.Max(0f, damage);
        return damage;
    }
}
