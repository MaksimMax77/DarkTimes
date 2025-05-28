using Code;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class Bootstrapper : MonoBehaviour
{
    [Inject] private LoadingScreenController _loadingScreen;
    [Inject] private ResourceLoader _resourceLoader;

    private async void Start()
    {
        _loadingScreen.Show();

        await _resourceLoader.LoadAllEssentialAssets(progress =>
        {
            _loadingScreen.SetProgress(progress);
        });

        _loadingScreen.Hide();

        SceneManager.LoadScene("MainMenu");
    }
}
