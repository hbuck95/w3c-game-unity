using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.IO;
using System.Linq;

[RequireComponent(typeof(VideoPlayer))]
public class PlayVideo : MonoBehaviour {
    private VideoPlayer _player;
    private AudioSource _playerAudio;
    public AudioSource[] noiseSource; //Setup support for multiple noise clips if we decide to go down that route.
    public bool videoFinished;
    public int videoId;

    #region Unity Methods
    private void Awake () {
        _player = GetComponent<VideoPlayer>();
        _playerAudio = GetComponent<AudioSource>();
    }
    #endregion

    /// <summary>
    /// Load a video into the video player.
    /// </summary>
    /// <param name="videoName">The name of the video file to load.</param>
    public void LoadVideo() {
        StartCoroutine(StartVideo());
    }

    public void PlayNoise() {
        StartCoroutine(StartNoise());
    }

    /// <summary>
    /// Plays a video within a seperate thread.
    /// </summary>
    /// <param name="name">The name of the video file to play.</param>
    private IEnumerator StartVideo() {
        videoId = new System.Random().Next(0, 3);
        _player.url = Path.Combine(Application.streamingAssetsPath, string.Format("TimeBasedMedia/Video_{0}.mp4", videoId));
        _player.Prepare();
        _player.audioOutputMode = VideoAudioOutputMode.AudioSource;
        _player.SetTargetAudioSource(0, _playerAudio);
        _player.EnableAudioTrack(0, true);
        Debug.Log("Preparing video...");
        yield return new WaitUntil(() => _player.isPrepared);
        Debug.Log("Video prepared.");
        _player.Play();
        yield return new WaitUntil(() => !_player.isPlaying);
        videoFinished = true;
    }

    /// <summary>
    /// Controls the volume level increase for each noise source.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartNoise() {
        var vol = 0.0f;
        while (vol <= 0.99) {
            foreach (var noise in noiseSource) {
                if (!noise.isPlaying) noise.Play();
                noise.volume = vol;
            }
            yield return new WaitForSecondsRealtime(0.2f);
            vol += 0.02f;
        }
    }

    private IEnumerator _StopNoise() {
        foreach (var noise in noiseSource) {
            while (noise.volume > 0f) {
                noise.volume -= 0.02f;
                yield return new WaitForSecondsRealtime(0.1f);
            }
            noise.Stop();
        }
    }

    /// <summary>
    /// Stop all noise sources.
    /// </summary>
    public void StopNoise() {
        StartCoroutine(_StopNoise());

    }

    /// <summary>
    /// Is a video currently playing?
    /// </summary>
    /// <returns></returns>
    public bool IsPlaying() {
        return _player.isPlaying;
    }

    /// <summary>
    /// Checks to see if any of the audiosources used to play 'noise' currently have their volume at 0.
    /// Returns true if there are still noisy audiosources, false if not.
    /// </summary>
    /// <returns></returns>
    public bool IsNoisy() {
        return noiseSource.Count(x => Mathf.Approximately(x.volume, 0f)) == 0;
    }


}