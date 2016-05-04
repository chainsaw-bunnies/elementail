using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
  public static float DampTime = 0.2f;
  public static float ZOffset = -8.33f;

  Vector3 Velocity;

  void Start()
  {
    Velocity = Vector3.zero;
    transform.position = GameStatus.Root.Player.transform.position + (Vector3.forward * ZOffset);
  }

  void LateUpdate()
  {
    var goal = GameStatus.Root.Player.transform.position + (Vector3.forward * ZOffset);
    transform.position = Vector3.SmoothDamp(transform.position, goal, ref Velocity, DampTime, Mathf.Infinity, Time.deltaTime);
  }
}
