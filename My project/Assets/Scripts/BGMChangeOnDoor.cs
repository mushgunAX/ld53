using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMChangeOnDoor : MonoBehaviour
{
  public AudioSource toStop;
  public AudioSource toStart;
  private bool audioChanged;

  // Start is called before the first frame update
  void Start()
  {
    audioChanged = false;
  }

  // Update is called once per frame
  void Update()
  {
    if (audioChanged == false)
    {
      if (!GetComponent<BoxCollider2D>().enabled)
      {
        audioChanged = true;
        if (toStop != null)
          toStop.Stop();
        if (toStart != null)
          toStart.Play();
      }
    }
  }
}
