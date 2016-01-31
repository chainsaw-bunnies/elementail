using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
  int LeftRunesCount;
  float Speed;
  public float MaxSpeed = 15f;
  public SpriteRenderer SpriteRenderer;

  // Allow player to slightly overlap dangerous tiles since otherwise the player might be overlapping a tile
  // however our art is such that it will still look like the art is not touching the player.
  const float DangerousTileTolerance = 2.75f; 

  CharacterController CharacterController;
  HashSet<Ground> CurrentGrounds;

  float? XGoal;
  float? YGoal;
  Vector3 CurrentGoalVelocity;
  KeyCode CurrentKey;
  Vector3 CurretKeyDir; // Direction based on the last key pressed.

  void Start()
  {
    CharacterController = GetComponent<CharacterController>();
    CurrentGrounds = new HashSet<Ground>();
    CurrentKey = KeyCode.None;
    CurretKeyDir = Vector3.zero;

    XGoal = null;
    YGoal = null;
    CurrentGoalVelocity = Vector3.zero;
  }

  void FixedUpdate()
  {
    Speed = MaxSpeed;
    CharacterController.Move((CurretKeyDir * Speed * Time.fixedDeltaTime));

    // Cause the player to align to a grid on the axis they are not currently running on.
    if (XGoal.HasValue)
    {
      var xGoalDiff = Mathf.Abs(transform.position.x - XGoal.Value);
      if (!Mathf.Approximately(xGoalDiff, 0f))
      {
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(XGoal.Value, transform.position.y, 0f), ref CurrentGoalVelocity, 0.01f, Mathf.Infinity, Time.fixedDeltaTime);
      }
    }
    if (YGoal.HasValue)
    {
      var yGoalDiff = Mathf.Abs(transform.position.y - YGoal.Value);
      if (!Mathf.Approximately(yGoalDiff, 0f))
      {
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x, YGoal.Value, 0f), ref CurrentGoalVelocity, 0.01f, Mathf.Infinity, Time.fixedDeltaTime);
      }
    }


    // Update the direction you are facing.
    // SpriteRenderer.flipX = CurretKeyDir.x > 0;
  }

  void Update()
  {
    if (Input.GetKeyUp(KeyCode.Escape))
    {
      SceneManager.LoadScene("MainMenu");
    }

    KeyCheck(KeyCode.UpArrow,    new Vector3(0f, 1f, 0f),  KeyCode.LeftArrow, KeyCode.RightArrow, transform.position.x, ref XGoal, ref YGoal);
    KeyCheck(KeyCode.DownArrow,  new Vector3(0f, -1f, 0f), KeyCode.LeftArrow, KeyCode.RightArrow, transform.position.x, ref XGoal, ref YGoal);
    KeyCheck(KeyCode.LeftArrow,  new Vector3(-1f, 0f, 0f), KeyCode.DownArrow, KeyCode.UpArrow,    transform.position.y, ref YGoal, ref XGoal);
    KeyCheck(KeyCode.RightArrow, new Vector3(1f, 0f, 0f),  KeyCode.DownArrow, KeyCode.UpArrow,    transform.position.y, ref YGoal, ref XGoal);
  }

  void KeyCheck(KeyCode next, Vector3 nextDir, KeyCode roundDown, KeyCode roundUp, float pos, ref float? Goal, ref float? OtherGoal)
  {
    if (Input.GetKeyUp(next))
    {
      if (ChangedAxis(CurrentKey, next))
      {
        if (CurrentKey == roundDown)
        {
          OtherGoal = null;
          Goal = RoundDown(pos, Map.TileSize);
        }
        if (CurrentKey == roundUp)
        {
          OtherGoal = null;
          Goal = RoundUp(pos, Map.TileSize);
        }
      }
      CurrentKey = next;
      CurretKeyDir = nextDir;
    }
  }

  static List<KeyCode> XAxisKeys = new List<KeyCode> { KeyCode.LeftArrow, KeyCode.RightArrow };
  static List<KeyCode> YAxisKeys = new List<KeyCode> { KeyCode.UpArrow, KeyCode.DownArrow };
  static bool ChangedAxis(KeyCode prev, KeyCode next)
  {
    return (XAxisKeys.Contains(prev) && YAxisKeys.Contains(next)) || (XAxisKeys.Contains(next) && YAxisKeys.Contains(prev));
  }

  static float RoundUp(float value, float multiple)
  {
    if (!Mathf.Approximately(value % multiple, 0f))
      return (value - value % multiple) + multiple;
    return value;

  }

  static float RoundDown(float value, float multiple)
  {
    if (!Mathf.Approximately(value % multiple, 0f))
      return (value - value % multiple);
    return value;
  }

  #region Runes

  public void EnterTile(Ground ground)
  {
    CurrentGrounds.Add(ground);
  }

  public void ExitTile(Ground ground)
  {
    CurrentGrounds.Remove(ground);
  }

  void LateUpdate()
  {
    if (CurrentKey == KeyCode.None && XGoal == null && YGoal == null)
    {
      return;
    }

    Ground closest = null;
    float bestDistance = Mathf.Infinity;

    // Only affect the ground of the tile you overlap with the most.
    foreach (var ground in CurrentGrounds)
    {
      var dist = Vector3.Distance(transform.position, ground.transform.position);

      // Lose
      if (ground.Dangerous && dist < DangerousTileTolerance)
      {
        SceneManager.LoadScene("GameOver");
        return;
      }

      if (dist < bestDistance)
      {
        closest = ground;
        bestDistance = dist;
      }
    }

    if (closest != null && !closest.Runed)
    {
      LeftRunesCount++;
      closest.ActivateRune();
    }
  }

  #endregion
}
