using UnityEngine;
using Photon.Pun;


public class BulletManager : MonoBehaviour
{
    public float bulletSpeed = 1000f;
    Rigidbody bulletRB;

    public float bulletTimeLife = 3f;
    float bulletTimeCount = 0f;
    
    void Start()
    {
        bulletRB = GetComponent<Rigidbody>();
        bulletRB.AddForce(transform.forward * bulletSpeed);
    }

    void Update()
    {
        if(bulletTimeCount >= bulletTimeLife)
        {
            Destroy(this.gameObject);
        }

        bulletTimeCount += Time.deltaTime; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && other.GetComponent<PlayerController>())
        {
            other.GetComponent<PlayerController>().TakeDamage(-10f);
        }
    }
}
