public class TitleSceneManager : MonoBehaviourSingleton<TitleSceneManager>
{
    private const int _playSceneNumber = 1;
    
    public void OnStartButtonClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_playSceneNumber);
    }
}
