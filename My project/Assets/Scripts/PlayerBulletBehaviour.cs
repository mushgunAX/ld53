using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletBehaviour : MonoBehaviour
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
    if (collision.gameObject.tag != "Player")
    {
      //TODO play sound
      //TODO particle effects

      if (collision.gameObject.GetComponent<PistolEnemy>() != null)
      {
        collision.gameObject.GetComponent<PistolEnemy>().stunTimeRemaining = collision.gameObject.GetComponent<PistolEnemy>().stunTime;
        collision.gameObject.GetComponent<PistolEnemy>().health -= 1;
      }
      Destroy(gameObject);
    }
  }
}
