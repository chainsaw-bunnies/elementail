using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
  public SpriteRenderer Glow;
  public SpriteRenderer Plain;

  float RotationZ;
  float RotationSpeed;

  Vector3 Scale;
  float ScaleSpeed;

  [HideInInspector]
  public bool PortalDone;

  void Start()
  {
    RotationZ = Random.Range(0f, 360f);
    RotationSpeed = 100f;

    Scale = Vector3.one;
    ScaleSpeed = 4f;

    PortalDone = false;
  }

	void Update()
  {
    transform.position = FindObjectOfType<Player>().transform.position;

    RotationZ += RotationSpeed * Time.deltaTime;
    Scale += (Vector3.one*ScaleSpeed) * Time.deltaTime;
    foreach (var renderer in new[] { Plain, Glow })
    {
      renderer.transform.rotation = Quaternion.Euler(0f, 0f, RotationZ);
      renderer.transform.localScale = Scale;
    }

    var c = Glow.color;
    Glow.color = new Color(c.r, c.b, c.g, c.a + 0.002f * Time.fixedTime);
    if (Glow.color.a >= 1f)
    {
      PortalDone = true;
    }
  }
}
