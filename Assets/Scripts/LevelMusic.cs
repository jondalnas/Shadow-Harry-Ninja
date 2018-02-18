using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusic : MonoBehaviour {
	public AudioClip[] levelMusicLoop;
	public AudioClip[] levelMusicBuildup;

	public static int currentLevel;

	private bool playingBuildup;

	private AudioSource aus;

	// Use this for initialization
	void Start () {
		aus = GetComponent<AudioSource>();
		ChangeSong(0);
	}
	
	// Update is called once per frame
	void Update () {
		if (playingBuildup && !aus.isPlaying) {
			aus.clip = levelMusicLoop[currentLevel];
			aus.loop = true;
			aus.Play();
		}
	}

	public void ChangeSong(int level) {
		currentLevel = level;

		playingBuildup = true;

		aus.clip = levelMusicBuildup[level];
		aus.loop = false;
		aus.Play();
	}
}
