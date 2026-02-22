
using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EndGame : MonoBehaviour
{
    [SerializeField] private RawImage blackoutImage;
    [SerializeField] private PlayableDirector winDirector;
    [SerializeField] private PlayableDirector loseDirector;

    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TMP_Text endMessage; 
    [SerializeField] private TMP_Text[] endGameText;
    
    [SerializeField] private UnityEvent onGameEnd;
    
    public void GameWon()
    {
        GameEnd(winDirector, "You ascended the marble stairs and forgot about the purple desert.").Forget();
    }

    public void GameLost()
    {
        GameEnd(loseDirector, "The mysterious wanderer thought you better at the bottom of the stars").Forget();
    }

    async UniTaskVoid GameEnd(PlayableDirector director, string text)
    {
        onGameEnd?.Invoke();
        
        float duration = 1f;
        await Tween.Color(blackoutImage, Color.black, duration);
        
        director.Play();
        
        await Tween.Color(blackoutImage, Color.clear, duration);
        
        await UniTask.WaitForSeconds((float)director.duration);

        print("asjkdfl");
        endMessage.text = text; 
        endGamePanel.SetActive(true);
        foreach (var t in endGameText) 
            Tween.Color(t, Color.white, duration);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && endGamePanel.activeSelf)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
#if UNITY_EDITOR
if (Input.GetKeyDown(KeyCode.Keypad1))
{
    GameWon();
}
if (Input.GetKeyDown(KeyCode.Keypad2))
{
    GameLost();
}
#endif
    }
}