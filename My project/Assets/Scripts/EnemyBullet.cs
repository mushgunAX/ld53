using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
  private float lifetime;

  // Start is called before the first frame update
  void Start()
  {
    lifetime = 5.0f;
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
    //If the hit is the player, kill him
    if (collision.gameObject.GetComponent<PlayerBehaviour>() != null)
    {
      collision.gameObject.GetComponent<PlayerBehaviour>().alive = false;
    }

    Destroy(gameObject);
  }
}
