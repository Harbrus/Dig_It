using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject InitialSpawnPoint; // use a checkpoint
   
    ///// duration between a death of the main character and its respawn
    //public float RespawnDelay = 2f;
    ///// duration of the initial fade in (in seconds)
    //public float IntroFadeDuration = 1f;
    ///// duration of the fade to black at the end of the level (in seconds)
    //public float OutroFadeDuration = 1f;

    /// the elapsed time since the start of the level
    public System.TimeSpan RunningTime { get { return DateTime.UtcNow - _started; } }

    [Header("Respawn Loop")]
    /// the delay, in seconds, before displaying the death screen once the player is dead
    public float DelayBeforeDeathScreen = 1f;

    // private
    protected DateTime _started;
    protected int _savedPoints;
    protected Collider _collider;
    protected Vector3 _initialSpawnPointPosition;
    protected static LevelManager _instance;

    public GameObject Door;
    public int jewelToWin = 0;
    public int GameDuration = 99;
    protected bool _gameOver = false;
    protected int remainingTime;

    public GameObject countdownGameObj;
    public GameObject jewelCountGameObj;
    private TextMeshProUGUI countdown;
    private TextMeshProUGUI jewelCount;
    public int currentJewelAvailable;
    private bool setGameOverCheck = true;

    /// <summary>
    /// Singleton design pattern
    /// </summary>
    /// <value>The instance.</value>
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    _instance = obj.AddComponent<LevelManager>();
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// On awake, we initialize our instance. Make sure to call base.Awake() in override if you need awake.
    /// </summary>
    protected virtual void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        _instance = this as LevelManager;
        _collider = this.GetComponent<Collider>();
        _initialSpawnPointPosition = (InitialSpawnPoint == null) ? Vector3.zero : InitialSpawnPoint.transform.position;
        countdown = countdownGameObj.GetComponent<TextMeshProUGUI>();
        jewelCount = jewelCountGameObj.GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Initialization();        
    }

    protected virtual void Initialization()
    {
        _savedPoints = GameManager.Instance.Points;
        _started = DateTime.UtcNow;
    }

    /// <summary>
    /// Every frame we check for checkpoint reach
    /// </summary>
    public virtual void Update()
    {
        if (playerPrefab == null)
        {
            return;
        }

        _savedPoints = GameManager.Instance.Points;
        remainingTime = GameDuration - RunningTime.Seconds;
        countdown.SetText(remainingTime.ToString());
        jewelCount.SetText(_savedPoints + "/" + currentJewelAvailable);

        if(_savedPoints >= jewelToWin)
        {
            jewelCount.color = Color.red;
        }
        else
        {
            jewelCount.color = Color.white;
        }
    }

    /// <summary>
    /// Gets the player to the specified level
    /// </summary>
    /// <param name="levelName">Level name.</param>
    public virtual void GotoLevel(string levelName)
    {
        //StartCoroutine(GotoLevelCo(levelName));
    }

    /// <summary>
    /// Waits for a short time and then loads the specified level
    /// </summary>
    /// <returns>The level co.</returns>
    /// <param name="levelName">Level name.</param>
    //protected virtual IEnumerator GotoLevelCo(string levelName)
    //{
    //    //if (Time.timeScale > 0.0f)
    //    //{
    //    //    yield return new WaitForSeconds(OutroFadeDuration);
    //    //}

    //    //if (string.IsNullOrEmpty(levelName))
    //    //{
    //    //    LoadingSceneManager.LoadScene("StartScreen");
    //    //}
    //    //else
    //    //{
    //    //    LoadingSceneManager.LoadScene(levelName);
    //    //}
    //}

    // restart level
    // pause
    // respawn
    // DigItLevelManager from first project
}
