using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
  const float HopAnimationDuration = 0.667f; // Must match exactly the length of the hop animations.

  public Animator Animator;
  public Renderer Renderer;

  Vector3 DirectionRequest;
  string DirectionRequestName;
  bool IsHopping;
  bool IsHopPausing;
  Vector3 HopDestination;
  float HopDuration;
  float HopPauseDuration;
  float HopPauseTime;

  void Start()
  {
    DirectionRequest = Vector3.zero;
    HopDestination = Vector3.zero;
    IsHopping = false;
    IsHopPausing = false;
    HopDuration = 0.25f;
    HopPauseDuration = 0.05f;

    Animator.Play("IdleRight");
  }

  void Update()
  {
    // Capture user input.
    if (Input.GetKeyDown(KeyCode.UpArrow))
    {
      DirectionRequest = Vector3.up;
      DirectionRequestName = "Up";
    }
    if (Input.GetKeyDown(KeyCode.RightArrow))
    {
      DirectionRequest = Vector3.right;
      DirectionRequestName = "Right";
    }
    if (Input.GetKeyDown(KeyCode.DownArrow))
    {
      DirectionRequest = Vector3.down;
      DirectionRequestName = "Down";
    }
    if (Input.GetKeyDown(KeyCode.LeftArrow))
    {
      DirectionRequest = Vector3.left;
      DirectionRequestName = "Left";
    }

    // Continue an existing hop.
    if (IsHopping)
    {
      var toDestination = (HopDestination - transform.position);
      var direction = toDestination.normalized;
      var distanceLeft = toDestination.magnitude;

      var frameDistance = Time.deltaTime / HopDuration;
      if (frameDistance >= distanceLeft)
      {
        int currX, currY, currZ;
        GetRoundedVector(transform.position, out currX, out currY, out currZ);

        IsHopping = false;
        transform.position = new Vector3(currX, currY, currZ);
        GameStatus.Root.Tiles[currX, currY].Enter();

        IsHopPausing = true;
        HopPauseTime = Time.time;

        Renderer.transform.localPosition = Vector3.zero;
      }
      else
      {
        transform.position += direction * frameDistance;
      }
    }

    // Finished pausing after a hop.
    if (IsHopPausing && Time.time > HopPauseTime + HopPauseDuration)
    {
      IsHopPausing = false;
    }

    // Begin a new hop.
    if (!IsHopping && !IsHopPausing && DirectionRequest != Vector3.zero)
    {
      int currX, currY, currZ;
      GetRoundedVector(transform.position, out currX, out currY, out currZ);
      var currentTile = GameStatus.Root.Tiles[currX, currY];

      int destX, destY, destZ;
      GetRoundedVector(transform.position + DirectionRequest, out destX, out destY, out destZ);
      var destinationTile = GameStatus.Root.Tiles[destX, destY];

      // The player jumps over ritual point tiles completely.
      var ritualTile = destinationTile.IsRitualPoint ? destinationTile : null;
      if (destinationTile.IsRitualPoint)
      {
        GetRoundedVector(transform.position + (DirectionRequest * 2f), out destX, out destY, out destZ);
        destinationTile = GameStatus.Root.Tiles[destX, destY];
      }

      var hopDistance = Mathf.Sqrt(Mathf.Pow(destX - currX, 2) + Mathf.Pow(destY - currY, 2));

      // Leaving the current tile.
      currentTile.Leave();

      // Cancel the hop if the destination tile is an obstacle.
      if (destinationTile.IsObstacle)
      {
        DirectionRequest = Vector3.zero;
        transform.position = new Vector3(currX, currY, currZ);
        Animator.Play("Idle" + DirectionRequestName);
        GameStatus.Root.SoundPlayer.Play(GameStatus.Root.SoundBank.Idle);
      }
      else
      {
        if (ritualTile != null)
        {
          GameStatus.Root.SoundPlayer.Play(GameStatus.Root.SoundBank.AttackShort);
          Renderer.transform.DOLocalPath(new[] { new Vector3(0f, 2f / 3f, 0f), Vector3.zero }, HopDuration * hopDistance).OnComplete(() =>
          {
            GameStatus.Root.SoundPlayer.Play(GameStatus.Root.SoundBank.Impact);
            if (!ritualTile.IsActivated)
            {
              ritualTile.Activate();
            }
          });
        }

        HopDestination = new Vector3(destX, destY, destZ);
        IsHopping = true;
        Animator.Play("Hop" + DirectionRequestName);
        Animator.speed = (HopAnimationDuration / HopDuration) / hopDistance;
        GameStatus.Root.SoundPlayer.Play(GameStatus.Root.SoundBank.Hop);
      }
    }

    // Exit to the main menu.
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      SceneManager.LoadScene("MainMenu");
    }
  }

  static void GetRoundedVector(Vector3 value, out int x, out int y, out int z)
  {
    x = Mathf.RoundToInt(value.x);
    y = Mathf.RoundToInt(value.y);
    z = Mathf.RoundToInt(value.z);
  }
}
