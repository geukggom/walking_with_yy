using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{

    public void OnStartButtonClick()
    {
        //로딩화면 띄우기
        StartGameAsync().Forget();

        async UniTask StartGameAsync()
        {
            await GameDataManager.Instance.LoadGameData();
        }
    }
}
