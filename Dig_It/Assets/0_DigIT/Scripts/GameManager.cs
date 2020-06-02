using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DigItEventType
{
    LevelStart,
    LevelComplete,
    LevelEnd,
    Pause,
    UnPause,
    PlayerDeath,
    RespawnStarted,
    RespawnComplete,
    StarPicked,
    GameOver
}

public class GameManager : MonoBehaviour
{
    /// the maximum amount of lives the character can currently have
    public int MaximumLives = 0;
    /// the current number of lives 
    public int CurrentLives = 0;

    /// the name of the scene to redirect to when all lives are lost
    public string GameOverScene;

    public int Points;

    public bool Paused { get; set; }

    bool coroutineCalled = false;

    /// the current player
    public Vector2 LevelMapPosition { get; set; }
    public GameObject currentPlayer;

    protected static GameManager _instance;
    protected bool _enabled;
    protected int _initialMaximumLives;
    protected int _initialCurrentLives;

    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if(_instance == null)
                {
                    GameObject gameManager = new GameObject();
                    _instance = gameManager.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if(!Application.isPlaying)
        {
            return;
        }

        if (_instance == null)
        {
            //If I am the first instance, make me the Singleton
            _instance = this as GameManager;
            DontDestroyOnLoad(transform.gameObject);
            _enabled = true;
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
            {
                Destroy(this.gameObject);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _initialCurrentLives = CurrentLives;
        _initialMaximumLives = MaximumLives;
    }

    public virtual void Reset()
    {
        Points = 0;
        Time.timeScale = 1f;
        Paused = false;
    }

    public virtual void LoseLife()
    {
        CurrentLives--;
    }

    public virtual void GainLives(int lives)
    {
        CurrentLives += lives;
        if (CurrentLives > MaximumLives)
        {
            CurrentLives = MaximumLives;
        }
    }

    public virtual void AddLives(int lives, bool increaseCurrent)
    {
        MaximumLives += lives;
        if (increaseCurrent)
        {
            CurrentLives += lives;
        }
    }

    /// <summary>
    /// Resets the number of lives to their initial values.
    /// </summary>
    public virtual void ResetLives()
    {
        CurrentLives = _initialCurrentLives;
        MaximumLives = _initialMaximumLives;
    }

    /// <summary>
    /// Adds the points in parameters to the current game points.
    /// </summary>
    /// <param name="pointsToAdd">Points to add.</param>
    public virtual void AddPoints(int pointsToAdd)
    {
        Points += pointsToAdd;
    }

    public virtual void LosePoint(int pointsToRemove)
    {
        Points -= pointsToRemove;
        if(!coroutineCalled)
        {
            StartCoroutine(FlashRed(currentPlayer));
        }
    }

    public IEnumerator FlashRed(GameObject gameObject)
    {
        coroutineCalled = true;
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.3f);
        currentPlayer.GetComponent<SpriteRenderer>().color = Color.white;
        coroutineCalled = false;
        yield return new WaitForSeconds(0.3f);

    }

    /// <summary>
    /// use this to set the current points to the one you pass as a parameter
    /// </summary>
    /// <param name="points">Points.</param>
    public virtual void SetPoints(int points)
    {
        Points = points;
    }

    /// <summary>
    /// Pauses the game or unpauses it depending on the current state
    /// </summary>
    public virtual void Pause()
    {
        // if time is not already stopped		
        if (Time.timeScale > 0.0f)
        {
            Instance.Paused = true;
            //if ((GUIManager.Instance != null))
            //{
            //    GUIManager.Instance.SetPauseScreen(true);
            //    _pauseMenuOpen = true;
            //}
        }
        else
        {
            UnPause();
        }
       // LevelManager.Instance.ToggleCharacterPause();
    }

    /// <summary>
    /// Unpauses the game
    /// </summary>
    public virtual void UnPause()
    {
        Instance.Paused = false;
        //if ((GUIManager.Instance != null))
        //{
        //    //GUIManager.Instance.SetPauseScreen(false);
        //    //_pauseMenuOpen = false;
        //}
    }
}
