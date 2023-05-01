using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletBehaviour : MonoBehaviour
{
  private float lifetime;
  public AudioSource ricochetSound;

  // Start is called before the first frame update
  void Start()
  {
    lifetime = 5.0f;
    ricochetSound = GameObject.Find("RicochetSound").GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update()
  {
    lifetime -= Time.deltaTime;
    if (lifetime <= 0.0f)
      Destroy(gameObject);
  }

  public void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.gameObject.tag != "Player")
    {
      //Play sound
      ricochetSound.pitch = Random.Range(0.8f, 1.2f);
      ricochetSound.Play();

      //TODO particle effects

      if (collision.gameObject.GetComponent<PistolEnemy>() != null)
      {
        collision.gameObject.GetComponent<PistolEnemy>().stunTimeRemaining = collision.gameObject.GetComponent<PistolEnemy>().stunTime;
        collision.gameObject.GetComponent<PistolEnemy>().health -= 1;

        //Push the enemy back
        Vector3 pushDirection = collision.gameObject.transform.position - transform.position;
        pushDirection.z = 0.0f;
        pushDirection.Normalize();
        collision.gameObject.GetComponent<Rigidbody2D>().AddForce(pushDirection * 100.0f);
      }
      Destroy(gameObject);
    }
  }
}
