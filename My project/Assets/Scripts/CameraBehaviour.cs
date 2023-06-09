using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
  public GameObject player;
  public GameObject crosshair;
  private Vector3 lastAliveLocation;

  public float speed;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    Vector3 targetLocation = (player.transform.position + crosshair.transform.position) / 2.0f;
    if (player.GetComponent<PlayerBehaviour>().alive)
    {
      lastAliveLocation = player.transform.position;
    }
    else
    {
      targetLocation = lastAliveLocation;
    }
    targetLocation.z = -10.0f;
    Vector3 cameraNextFrameLocation = transform.position + (targetLocation - transform.position) * speed * Time.deltaTime;
    transform.position = cameraNextFrameLocation;
  }
}
