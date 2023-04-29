using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
  //Parameters to tweak
  [Header("Movement")]
  public float moveForce;
  public float horizontalSpeedLimit;
  public float jumpForce;
  public float upwardSpeedLimit;
  public float landingForce;

  [Header("Gun")]
  public float bulletVelocity;
  public float reloadTime;
  public float shotInterval;
  [Tooltip("In degrees")]
  public float deviation;

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

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    handleMovement();
  }
}
