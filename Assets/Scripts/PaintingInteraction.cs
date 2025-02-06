using UnityEngine;
using UnityEngine.UI;

public class PaintingInteraction : MonoBehaviour
{
    public GameObject[] imagesToDisplay; // Tableau contenant les images
    public Button actionButton; // Le bouton pour changer l'image

    private int currentImageIndex = 0;

    private void Start()
    {
        // Masque toutes les images au début
        foreach (GameObject image in imagesToDisplay)
        {
            image.SetActive(false);
        }

        // Vérifie si un bouton est configuré et ajoute l'écouteur
        if (actionButton != null)
        {
            actionButton.onClick.AddListener(ChangeImage);
        }
        else
        {
            Debug.LogError("Aucun bouton n'est attaché !");
        }
    }

    private void ChangeImage()
    {
        // Masque l'image actuelle
        if (imagesToDisplay[currentImageIndex] != null)
        {
            imagesToDisplay[currentImageIndex].SetActive(false);
        }

        // Passe à l'image suivante
        currentImageIndex = (currentImageIndex + 1) % imagesToDisplay.Length;

        // Affiche la nouvelle image
        if (imagesToDisplay[currentImageIndex] != null)
        {
            imagesToDisplay[currentImageIndex].SetActive(true);
            Debug.Log($"Image affichée : {imagesToDisplay[currentImageIndex].name}");
        }
    }
}
