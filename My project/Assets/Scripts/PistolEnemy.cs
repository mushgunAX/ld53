using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolEnemy : MonoBehaviour
{
  public GameObject playerTarget;
  public GameObject bullet;
  public int maxHealth;
  public int health;
  private bool deadOnce;
  private int lastFrameHealth;
  public float moveSpeed;
  public float moveForce;
  public float stunTime;
  public float stunTimeRemaining;
  private float flashTime;
  private float flashFrequency = 0.05f;

  public float movingTime;
  public float movingTimeVariance;
  private float movingTimeLeft;
  public float burstInterval;
  public float burstIntervalVariance;
  private float timeLeftToBurst;
  public int burstAmount;
  private int burstShotsRemaining;
  public float intraburstInterval;
  public float intraburstIntervalVariance;
  private float timeLeftToShot;

  public float minDistanceBeforeFleeing;

  public float shotMaxDeviation;
  public float shotSpeed;

  private bool isShooting;

  private Rigidbody2D rb;

  [Header("Audio")]
  public AudioSource dieSound;
  public AudioSource hurtSound;
  public AudioSource readySound;
  public AudioSource shootSound;

  // Start is called before the first frame update
  void Start()
  {
    health = maxHealth;
    stunTimeRemaining = 0.0f;
    flashTime = flashFrequency;
    rb = GetComponent<Rigidbody2D>();
    isShooting = false;

    timeLeftToBurst = burstInterval + Random.Range(-burstIntervalVariance, burstIntervalVariance);
    burstShotsRemaining = burstAmount;
    timeLeftToShot = intraburstInterval + Random.Range(-intraburstIntervalVariance, intraburstIntervalVariance);
    movingTimeLeft = movingTime + Random.Range(-movingTimeVariance, movingTimeVariance);

    deadOnce = false;
  }

  // Update is called once per frame
  void Update()
  {
    if (health < lastFrameHealth)
    {
      hurtSound.Play();
    }
    lastFrameHealth = health;

    if (health <= 0)
    {
      if (!deadOnce)
        dieSound.Play();
      deadOnce = true;

      GetComponent<SpriteRenderer>().flipY = true;

      Destroy(gameObject, 5.0f);
      GetComponent<BoxCollider2D>().enabled = false;
      //Alternate blink flashing
      flashTime -= Time.deltaTime;
      if (flashTime <= 0.0f)
      {
        flashTime += flashFrequency;
        GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;
      }
      return;
    }

    if (!(playerTarget.GetComponent<PlayerBehaviour>().alive))
      return;

    if (stunTimeRemaining > 0.0f)
    {
      isShooting = false;
      stunTimeRemaining -= Time.deltaTime;
      //Alternate red flashing
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
      //Stop flashing
      GetComponent<SpriteRenderer>().color = Color.white;
    }

    //Handle logic
    if (stunTimeRemaining <= 0.0f)
    {
      float horizontalDisplacement = playerTarget.transform.position.x - transform.position.x;

      if (!isShooting)
      {
        //Move 
        if (movingTimeLeft > 0.0f)
        {
          movingTimeLeft -= Time.deltaTime;

          //Flee if horizontal displacement too near
          if (Mathf.Abs(horizontalDisplacement) < minDistanceBeforeFleeing)
          {
            if (Mathf.Abs(rb.velocity.x) < moveSpeed)
              rb.AddForce(new Vector2((horizontalDisplacement > 0.0f ? -moveForce : moveForce), 0.0f));

            //Facing
            if (horizontalDisplacement > 0.0f)
              GetComponent<SpriteRenderer>().flipX = true;
            else
              GetComponent<SpriteRenderer>().flipX = false;
          }
          else //Approach
          {
            if (Mathf.Abs(rb.velocity.x) < moveSpeed)
              rb.AddForce(new Vector2((horizontalDisplacement > 0.0f ? moveForce : -moveForce), 0.0f));

            //Facing
            if (horizontalDisplacement > 0.0f)
              GetComponent<SpriteRenderer>().flipX = false;
            else
              GetComponent<SpriteRenderer>().flipX = true;
          }
        }
        else
        {
          movingTimeLeft = movingTime + Random.Range(-movingTimeVariance, movingTimeVariance);
          isShooting = true;
          //Play cocking sound
          readySound.Play();
        }
      }
      else
      {
        //Preparing to shoot
        //Facing
        if (horizontalDisplacement > 0.0f)
        {
          GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
          GetComponent<SpriteRenderer>().flipX = true;
        }
        timeLeftToBurst -= Time.deltaTime;
        if (timeLeftToBurst <= 0.0f)
        {
          if (burstShotsRemaining > 0)
          {
            timeLeftToShot -= Time.deltaTime;
            if (timeLeftToShot <= 0.0f)
            {
              //Shoot

              //Play shoot sound
              shootSound.Play();

              Vector3 baseDirection = playerTarget.transform.position - transform.position;
              float rotationAngle = Random.Range(-shotMaxDeviation, shotMaxDeviation);
              Vector3 finalShotDirection = Quaternion.AngleAxis(rotationAngle, Vector3.forward) * baseDirection;
              finalShotDirection.z = 0.0f;
              finalShotDirection.Normalize();

              GameObject newBullet = Instantiate(bullet);
              newBullet.transform.position = GetComponent<Transform>().position + finalShotDirection;
              newBullet.GetComponent<Rigidbody2D>().velocity = finalShotDirection * shotSpeed;

              timeLeftToShot = intraburstInterval + Random.Range(-intraburstIntervalVariance, intraburstIntervalVariance);
              --burstShotsRemaining;
            }
          }
          else
          {
            timeLeftToBurst = burstInterval + Random.Range(-burstIntervalVariance, burstIntervalVariance);
            timeLeftToShot = intraburstInterval + Random.Range(-intraburstIntervalVariance, intraburstIntervalVariance);
            burstShotsRemaining = burstAmount;
            isShooting = false;
          }
        }
      }
    }
  }
}
