using JetBrains.Annotations;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
  private bool deadOnce;
  public bool alive;

  //Parameters to tweak
  [Header("Movement")]
  public float moveForce;
  public float horizontalSpeedLimit;
  public float jumpForce;
  public float upwardSpeedLimit;
  public float landingForce;

  [Header("Gun Basics")]
  public GameObject crosshair;
  public GameObject bullet;
  public float bulletVelocity;

  [Header("Deviation")]
  [Tooltip("In degrees")]
  public float maxDeviation;
  public float deviationReductionRate;
  public float deviationPerShot;
  private float currentDeviation;
  
  [Header("Ammo and Reloading")]
  public int rightAmmo;
  public int leftAmmo;
  public float reloadTime;
  private const int maxAmmo = 17;
  private float reloadTimeRemaining;
  private bool presentlyReloading;

  [Header("UI")]
  public TextMeshProUGUI rightAmmoCounter;
  public TextMeshProUGUI leftAmmoCounter;
  public GameObject reloadBar;

  [Header("Audio")]
  public AudioSource shootSound;
  public AudioSource reloadSound;
  public AudioSource reloadedSound;
  public AudioSource dieSound;
  public AudioSource emptySound;

  // Start is called before the first frame update
  void Start()
  {
    leftAmmo = maxAmmo;
    rightAmmo = maxAmmo;
    currentDeviation = 0.0f;
    presentlyReloading = false;
    alive = true;
    deadOnce = false;
  }

  // Update is called once per frame
  void Update()
  {
    handleUI();

    if (alive)
    {
      //Deviation reduction over time
      currentDeviation -= deviationReductionRate * Time.deltaTime;
      if (currentDeviation <= 0.0f)
        currentDeviation = 0.0f;
      if (currentDeviation > maxDeviation)
        currentDeviation = maxDeviation;

      //Reloading
      if (Input.GetKey(KeyCode.R) &&
        (rightAmmo < maxAmmo || leftAmmo < maxAmmo))
      {
        if (!presentlyReloading)
          reloadSound.Play();
        presentlyReloading = true;
      }
      else
      {
        presentlyReloading = false;
      }

      if (!presentlyReloading)
      {
        reloadBar.SetActive(false);
        reloadTimeRemaining = reloadTime;
        handleMovement();
        handleShooting();
        handleLooking();
      }
      else
      {
        reloadBar.SetActive(true);
        handleReloading();
      }
    }
    else
    {
      if (!deadOnce)
        dieSound.Play();
      deadOnce = true;

      //Fall through the earth
      GetComponent<BoxCollider2D>().enabled = false;

      //Prompt to restart the game
      leftAmmoCounter.text = "";
      rightAmmoCounter.text = "Delivery Failed. Press R to restart";

      //Restart the game when R is pressed
      if (Input.GetKey(KeyCode.R))
      {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1.0f;
      }
    }
  }

  public void handleLooking()
  {
    if (crosshair.transform.position.x > transform.position.x)
    {
      GetComponent<SpriteRenderer>().flipX = false;
    }
    else
    {
      GetComponent<SpriteRenderer>().flipX = true;
    }
  }

  public void handleMovement()
  {
    //Horizontal movement
    if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
    {
      GetComponent<Rigidbody2D>().AddForce(new Vector3(moveForce, 0.0f, 0.0f));
    }
    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
    {
      GetComponent<Rigidbody2D>().AddForce(new Vector3(-moveForce, 0.0f, 0.0f));
    }

    //Jump to fly
    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
    {
      GetComponent<Rigidbody2D>().AddForce(new Vector3(0.0f, jumpForce, 0.0f));
    }

    //Speed limiting
    Vector3 currentVelocity = GetComponent<Rigidbody2D>().velocity;
    if (currentVelocity.x > horizontalSpeedLimit)
      currentVelocity.x = horizontalSpeedLimit;
    if (currentVelocity.x < -horizontalSpeedLimit)
      currentVelocity.x = -horizontalSpeedLimit;
    if (currentVelocity.y > upwardSpeedLimit)
      currentVelocity.y = upwardSpeedLimit;
    GetComponent<Rigidbody2D>().velocity = currentVelocity;
  }

  public void handleCrouching()
  {
    //TODO
  }

  public void handleReloading()
  {
    reloadTimeRemaining -= Time.deltaTime;
    reloadBar.transform.localScale = new Vector3(10.0f * (reloadTimeRemaining / reloadTime), reloadBar.transform.localScale.y, reloadBar.transform.localScale.z);
    if (reloadTimeRemaining <= 0.0f)
    {
      //Play reloaded sound
      reloadedSound.Play();

      if (rightAmmo < leftAmmo)
        rightAmmo = maxAmmo;
      else
        leftAmmo = maxAmmo;
      reloadTimeRemaining += reloadTime;
    }
  }

  public void handleUI()
  {
    rightAmmoCounter.text = rightAmmo.ToString();
    leftAmmoCounter.text = leftAmmo.ToString();
  }

  public void gunShot()
  {
    //Play BANG sound
    shootSound.Play();

    //Determine the shot direction
    //https://www.youtube.com/watch?v=HH6JzH5pTGo
    Vector3 baseDirection = crosshair.transform.position - transform.position;
    float rotationAngle = Random.Range(-currentDeviation, currentDeviation);
    Vector3 finalShotDirection = Quaternion.AngleAxis(rotationAngle, Vector3.forward) * baseDirection;
    finalShotDirection.z = 0.0f;
    finalShotDirection.Normalize();

    GameObject newBullet = Instantiate(bullet);
    newBullet.transform.position = GetComponent<Transform>().position + finalShotDirection;
    newBullet.GetComponent<Rigidbody2D>().velocity = finalShotDirection * bulletVelocity;

    //Increase the deviation
    currentDeviation += deviationPerShot;
  }

  public void handleShooting()
  {
    //Left
    if (Input.GetMouseButtonDown(0))
    {
      if (leftAmmo > 0)
      {
        --leftAmmo;
        gunShot();
      }
      else
      {
        //play CHIK sound
        emptySound.Play();
      }
    }
    if (Input.GetMouseButtonDown(1))
    {
      if (rightAmmo > 0)
      {
        --rightAmmo;
        gunShot();
      }
      else
      {
        //play CHIK sound
        emptySound.Play();
      }  
    }
  }
}
