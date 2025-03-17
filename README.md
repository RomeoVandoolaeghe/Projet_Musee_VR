# MUSEUM-VR

## Description
Museum VR est un projet développé sur Unity visant à créer une expérience immersive de musée en réalité virtuelle. L'objectif principal est de permettre aux utilisateurs de découvrir des œuvres d'art de manière innovante en traversant des tableaux 2D pour entrer dans des espaces 3D interactifs.

## Fonctionnalités
### 1. **Exploration du Musée**
- Un musée divisé en deux salles thématiques :
  - **Salle Piet Mondrian** : Présente des œuvres sous forme de puzzle game interactif.
  - **Salle René Magritte** : Offre une expérience d'observation avec des éléments interactifs.
- Une voix d'accueil guide l'utilisateur à travers l'expérience.

### 2. **Tableaux Immersifs**
- Les œuvres 2D peuvent être traversées pour entrer dans une pièce immersive où l’œuvre est reproduite en 3D.
- Les œuvres de Piet Mondrian prennent la forme de puzzles à assembler.
![image](https://github.com/user-attachments/assets/fbf82d64-42d0-4a39-80ef-32267aebd861)

- Les œuvres de René Magritte proposent une interaction pour modifier certains éléments.

### 3. **Intelligence Artificielle**
- Deux tableaux générés dynamiquement grâce à **Stable Diffusion**.
- Un bouton sous chaque tableau permet de générer une nouvelle version de l’image via une requête API.

## Technologies Utilisées
- **Unity** (pour le développement du musée VR)
- **XR Interaction Toolkit** (gestion des interactions VR)
- **Stable Diffusion API** (génération des tableaux IA)
- **Meta Quest 3**

## Futures Améliorations
- Ajout de popups informatifs affichant des descriptions des œuvres lorsqu'elles sont regardées.
- Amélioration des interactions pour une expérience plus immersive.
- Intégration d’un mode multijoueur pour une visite collaborative.

## Installation et Exécution
1. **Cloner le projet**
   ```sh
   git clone https://github.com/votre-repo/museum-vr.git
   ```
2. **Ouvrir sous Unity**
   - Assurez-vous d’avoir **Unity 2022 ou plus récent** installé.
   - Ouvrez le projet avec Unity Hub.
3. **Configurer les API**
   - Récupérez une clé API Stable Diffusion et configurez-la dans Unity.
4. **Lancer le projet**
   - Branchez votre casque VR et lancez la scène principale.

## Licence
Ce projet est sous licence MIT. Voir le fichier [LICENSE](LICENSE) pour plus d’informations.


