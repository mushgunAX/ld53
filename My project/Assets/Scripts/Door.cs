using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
  public GameObject player;
  public GameObject fader;
  private float faderOpacity;
  public float fadeOutRate;
  public GameObject[] enemiesToActivate;
  public AudioSource doorKickSound;
  public AudioSource toPlayWhenOpening;

  private float flashTime;
  private float flashFrequency = 0.05f;

  private bool doorKicked;

  // Start is called before the first frame update
  void Start()
  {
    faderOpacity = 1.0f;
    doorKicked = false;

    foreach (GameObject enemy in enemiesToActivate)
    {
      enemy.SetActive(false);
    }
    fader.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
  }

  // Update is called once per frame
  void Update()
  {
    if (!doorKicked)
    {
      //Flash
      flashTime -= Time.deltaTime;
      if (flashTime <= 0.0f)
      {
        flashTime += flashFrequency;
        if (GetComponent<SpriteRenderer>().color == Color.red)
          GetComponent<SpriteRenderer>().color = Color.white;
        else
          GetComponent<SpriteRenderer>().color = Color.red;
      }
    }
    else
    {
      GetComponent<SpriteRenderer>().color = Color.red;

      //Fade out
      faderOpacity -= fadeOutRate * Time.deltaTime;
      if (faderOpacity < 0.0f) faderOpacity = 0.0f;
      fader.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, faderOpacity);
    }
  }

  public void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.gameObject == player && doorKicked == false)
    {
      doorKicked = true;
      doorKickSound.Play();
      toPlayWhenOpening.Play();

      //Activate the enemies in the next room
      foreach (GameObject enemy in enemiesToActivate)
      {
        enemy.SetActive(true);
      }

      GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
      GetComponent<Rigidbody2D>().AddForce((transform.position - player.transform.position) * 100.0f);
      GetComponent<BoxCollider2D>().enabled = false;
    }
  }
}
