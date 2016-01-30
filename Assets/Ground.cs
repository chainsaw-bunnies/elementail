using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour
{
  const float DustedMinimumDistance = 1.5f;

  public Sprite[] BaseSprites;
  public Sprite[] RuneSprites;
  public SpriteRenderer GroundRenderer;
  public SpriteRenderer DustRenderer;

  void Start()
  {
    GroundRenderer.sprite = BaseSprites[Random.Range(0, BaseSprites.Length)];

    DustRenderer.sprite = RuneSprites[Random.Range(0, RuneSprites.Length)];
    DustRenderer.enabled = false;
  }

  void OnTriggerStay(Collider other)
  {
    var distance = Vector3.Distance(other.transform.position, transform.position);
    if (other.tag == "Player" && !DustRenderer.enabled && distance < DustedMinimumDistance)
    {
      DustRenderer.enabled = true;
    }
  }
}
