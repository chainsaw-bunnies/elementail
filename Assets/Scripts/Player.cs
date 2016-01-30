using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{
  public float Speed = 1f;

  CharacterController CharacterController;
  Vector3 MoveDirection;

  void Start()
  {
    CharacterController = GetComponent<CharacterController>();
    MoveDirection = Vector3.zero;
  }

  void FixedUpdate()
  {
    CharacterController.Move(MoveDirection * Speed * Time.fixedDeltaTime);
  }

  void Update ()
  {
    var keys = new[] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
    foreach (var key in keys)
    {
      if (Input.GetKeyDown(key))
      {
        MoveDirection += KeyCodeToDirection(key);
      }
      if (Input.GetKeyUp(key))
      {
        MoveDirection -= KeyCodeToDirection(key);
      }
    }

    if (Input.GetKeyUp(KeyCode.Escape))
    {
      Application.Quit();
    }
  }

  static Vector3 KeyCodeToDirection(KeyCode code)
  {
    switch (code)
    {
      case KeyCode.UpArrow:  return new Vector3(0f, 1f, 0f);
      case KeyCode.DownArrow:  return new Vector3(0f, -1f, 0f);
      case KeyCode.RightArrow: return new Vector3(1f, 0f, 0f);
      case KeyCode.LeftArrow:  return new Vector3(-1f, 0f, 0f);
    }
    return Vector3.zero;
  }
}
