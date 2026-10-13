using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    [SerializeField] GameObject _pauseMenu;
    [SerializeField] CanvasGroup _gameOverScreen;

    private bool _playerDead;
    private bool _isGamePaused;
    private Action<bool> onGamePaused;

    private void Awake()
    {
        onGamePaused = PlayerEvents.RaiseGamePaused;
      
    }

    private void Start()
    {
        _gameOverScreen.alpha = 0f;
        _gameOverScreen.interactable = false;
        _gameOverScreen.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        PlayerEvents.onDeath += OnPlayerDeath;

    }

    private void OnDisable()
    {

        PlayerEvents.onDeath -= OnPlayerDeath;

    }

    private void Update()
    {
        PauseMenu();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void PauseMenu()
    {
        if (_playerDead) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            if (!_isGamePaused)
            {
                _isGamePaused = true;
                onGamePaused.Invoke(_isGamePaused);
                _pauseMenu.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                _pauseMenu.SetActive(false);
                _isGamePaused = false;
                onGamePaused.Invoke(_isGamePaused);
                Time.timeScale = 1f;
            }
        }
    }

    void OnPlayerDeath(bool isDead)
    {
        _playerDead = isDead;
        if (isDead)
        {
            _gameOverScreen.interactable = true;
            _gameOverScreen.blocksRaycasts = true;
            _gameOverScreen.DOFade(1, 2f);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
