using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
  const int PoolSize = 32;

  Queue<AudioSource> Used;
  Queue<AudioSource> Unused;

  void Start()
  {
    Used = new Queue<AudioSource>(PoolSize);
    Unused = new Queue<AudioSource>(PoolSize);

    var pool = new GameObject("Sound Pool");
    pool.transform.parent = gameObject.transform;

    for (int i = 0; i < PoolSize; i++)
    {
      Unused.Enqueue(pool.AddComponent<AudioSource>());
    }
  }

  public void Play(AudioClip clip)
  {
    while (Used.Count > 0 && !Used.Peek().isPlaying)
    {
      Unused.Enqueue(Used.Dequeue());
    }

    var source = (Unused.Count > 0 ? Unused : Used).Dequeue();
    if (source.isPlaying)
    {
      source.Stop();
    }

    source.clip = clip;
    source.Play();

    Used.Enqueue(source);
  }
}
