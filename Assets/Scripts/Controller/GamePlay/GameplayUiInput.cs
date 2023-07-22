using GamePlayLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class GameplayUiInput : MonoBehaviour
{
    private PlayerController _playerController;

    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private Button _quitButton;
    [SerializeField] private LeaderBordUiHandler _leaderBordUiHandler;
    [SerializeField] private EventSystem _eventSystem;

    private void Awake()
    {
        _playerController = new PlayerController();
        _playerController.UI.Enable();
        _playerController.UI.PauseOptions.performed += ChangePauseMenuState;
        _playerController.UI.Back.performed += QuitGame;
        OnlineGameManager.OnEndGame += OpenLeaderboard;
    }

    private void OnDestroy()
    {
        _playerController.UI.PauseOptions.performed -= ChangePauseMenuState;
        OnlineGameManager.OnEndGame -= OpenLeaderboard;
    }

    private void ChangePauseMenuState(CallbackContext callbackContext)
    {
        if(_pauseMenu.gameObject.activeSelf)
            _pauseMenu.SetActive(false);

        else
        {
            _pauseMenu.SetActive(true);
            _eventSystem.SetSelectedGameObject(_quitButton.gameObject);
        }
    }

    private void OpenLeaderboard()
    {
        _leaderBordUiHandler.gameObject.SetActive(true);
        _leaderBordUiHandler.TurnOn();
        _playerController.UI.Triangular.performed += _leaderBordUiHandler.StartNewGameInput;
    }

    //For leaderboard
    private void QuitGame(CallbackContext callbackContext)
    {
        if (_leaderBordUiHandler.gameObject.activeInHierarchy)
            QuitGame();
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }
        
}
