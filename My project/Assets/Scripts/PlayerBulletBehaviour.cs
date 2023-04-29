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

  public void OnCollisionEnter2D(Collision2D collision)
  {
    Debug.Log(collision.gameObject.name);
    if (collision.gameObject.tag != "Player")
    {
      //TODO play sound
      //TODO particle effects
      Destroy(gameObject);
    }
  }
}
