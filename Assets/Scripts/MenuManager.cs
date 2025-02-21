using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Cette fonction sera appelée lors du clic sur "Entrer dans le musée"
    public void StartMuseum()
    {
        // Remplace "NomDeVotreSceneDuMusee" par le nom exact de ta scène musée
        SceneManager.LoadScene("Main");
    }

    // Cette fonction sera appelée lors du clic sur "Quitter"
    public void QuitGame()
    {
        Application.Quit();
        // Pour tester dans l'éditeur, tu peux ajouter :
        // #if UNITY_EDITOR
        // UnityEditor.EditorApplication.isPlaying = false;
        // #endif
    }
}
