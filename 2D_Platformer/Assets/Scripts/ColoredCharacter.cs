using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ColoredCharacter : MonoBehaviour
{
    [SerializeField] public ElementColor color;

    public void OnValidate()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer && WorldParameters.Instance)
            renderer.color = WorldParameters.Instance.GetColor(color);

#if UNITY_EDITOR
        // Retirer avant d'ajouter : un seul appel en attente, même si OnValidate est appelé plusieurs fois
        EditorApplication.delayCall -= ApplyLayers;
        EditorApplication.delayCall += ApplyLayers;
#endif
    }

#if UNITY_EDITOR
    private void ApplyLayers()
    {
        EditorApplication.delayCall -= ApplyLayers;

        // L'objet a pu être détruit entre-temps (Play mode, rechargement de scène...)
        if (this == null || WorldParameters.Instance == null)
            return;

        gameObject.layer = WorldParameters.Instance.GetColorCharacterMask(color);

        CharacterController2D controller = GetComponent<CharacterController2D>();
        if (controller && controller.groundCheck)
            controller.groundCheck.gameObject.layer = WorldParameters.Instance.GetColorCharacterGroundMask(color);
    }
#endif
}
