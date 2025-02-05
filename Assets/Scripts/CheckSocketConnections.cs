using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CheckAllSockets : MonoBehaviour
{
    [Header("Configuration des sockets et objets attendus")]
    [Tooltip("Liste des XR Socket Interactor (7 au total)")]
    public XRSocketInteractor[] sockets;
    
    [Tooltip("Liste des GameObjects attendus dans les sockets dans le même ordre que les sockets. Par exemple, index 0 = premier objet blanc, index 1 = deuxième objet blanc, index 2 = troisième objet blanc, index 3 = objet bleu, index 4 = objet rouge, index 5 = objet noir, index 6 = objet jaune.")]
    public GameObject[] expectedObjects;

    private void OnEnable()
    {
        // S'abonner aux événements de chaque socket
        foreach (XRSocketInteractor socket in sockets)
        {
            socket.selectEntered.AddListener(OnSocketChanged);
            socket.selectExited.AddListener(OnSocketChanged);
        }
    }

    private void OnDisable()
    {
        // Se désabonner des événements
        foreach (XRSocketInteractor socket in sockets)
        {
            socket.selectEntered.RemoveListener(OnSocketChanged);
            socket.selectExited.RemoveListener(OnSocketChanged);
        }
    }

    // Méthode appelée lors de l'insertion ou du retrait d'un objet dans n'importe quel socket
    private void OnSocketChanged(BaseInteractionEventArgs args)
    {
        CheckSockets();
    }

    // Vérifie que chaque socket contient l'objet attendu
    private void CheckSockets()
    {
        // Vérifier que les tableaux ont le même nombre d'éléments
        if (sockets.Length != expectedObjects.Length)
        {
            Debug.LogError("Le nombre de sockets et d'objets attendus doit être identique !");
            return;
        }

        bool allCorrect = true;

        // Parcourir tous les sockets et vérifier le contenu
        for (int i = 0; i < sockets.Length; i++)
        {
            XRSocketInteractor socket = sockets[i];

            // Si le socket est vide, la vérification échoue
            if (socket.interactablesSelected.Count == 0)
            {
                allCorrect = false;
                break;
            }

            // Récupérer l'objet inséré dans le socket
            GameObject insertedObject = socket.interactablesSelected[0].transform.gameObject;

            // Comparer avec l'objet attendu
            if (insertedObject != expectedObjects[i])
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            Debug.Log("bravo");
            // Vous pouvez ici déclencher une UI ou toute autre action.
        }
    }
}