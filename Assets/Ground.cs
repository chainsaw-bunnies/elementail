using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour
{
  const float DustedMinimumDistance = 2f;

  public Sprite[] BaseSprites;
  public Sprite[] RuneSprites;
  public SpriteRenderer GroundRenderer;
  public SpriteRenderer DustRenderer;
  bool Dusted;

  void Start()
  {
    GroundRenderer.sprite = BaseSprites[Random.Range(0, BaseSprites.Length)];

    Dusted = false;
    DustRenderer.sprite = RuneSprites[Random.Range(0, RuneSprites.Length)];
    DustRenderer.enabled = false;
  }

  void OnTriggerStay(Collider other)
  {
    if (other.tag != "Player") { return; }

    if (Dusted)
    {
    }
    else
    {
      var distance = Vector3.Distance(other.transform.position, transform.position);
      if (other.tag == "Player" && distance < DustedMinimumDistance)
      {
        Dusted = true;
        DustRenderer.enabled = true;
      }
    }
  }
}
