using System.Collections.Generic;
using UnityEngine;

// Échange les positions des personnages quand on appuie sur Contrôle.
// Avec 2 personnages : ils échangent leurs places.
// Avec plus de 2 : chaque personnage prend la place du suivant dans la liste (rotation).
public class SwapPositions : MonoBehaviour
{
    [Tooltip("Personnages dont on échange les positions. Si la liste est vide, tous les CharacterController2D de la scène sont utilisés.")]
    [SerializeField] private List<CharacterController2D> characters = new();

    [SerializeField] private KeyCode swapKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode altSwapKey = KeyCode.RightControl;

    private void Start()
    {
        if (characters.Count == 0)
            characters.AddRange(FindObjectsOfType<CharacterController2D>());
    }

    private void Update()
    {
        if (Input.GetKeyDown(swapKey) || Input.GetKeyDown(altSwapKey))
            Swap();
    }

    public void Swap()
    {
        // Ignore les personnages détruits ou désactivés
        characters.RemoveAll(c => c == null);
        List<CharacterController2D> active = characters.FindAll(c => c.gameObject.activeInHierarchy);
        if (active.Count < 2)
            return;

        // On mémorise toutes les positions avant de déplacer qui que ce soit
        Vector3[] positions = new Vector3[active.Count];
        for (int i = 0; i < active.Count; i++)
            positions[i] = active[i].transform.position;

        for (int i = 0; i < active.Count; i++)
        {
            Vector3 target = positions[(i + 1) % active.Count];
            CharacterController2D character = active[i];

            Rigidbody2D rb = character.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.position = target;
                rb.velocity = Vector2.zero; // évite de garder l'élan de l'ancienne position
            }
            character.transform.position = target;
        }

        Physics2D.SyncTransforms();
    }
}
