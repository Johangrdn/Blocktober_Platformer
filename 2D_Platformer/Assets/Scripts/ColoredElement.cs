using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ColoredElement : MonoBehaviour
{
    [SerializeField] private ElementColor color;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ColoredCharacter character = collision.gameObject.GetComponent<ColoredCharacter>();
        CharacterController2D controller = collision.gameObject.GetComponent<CharacterController2D>();
        if (character && controller && character.color == color)
        {
            controller.Die();
        }
    }

    public void OnValidate()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer && WorldParameters.Instance)
            renderer.color = WorldParameters.Instance.GetColor(color);

#if UNITY_EDITOR
        EditorApplication.delayCall -= ApplyLayer;
        EditorApplication.delayCall += ApplyLayer;
#endif
    }

#if UNITY_EDITOR
    private void ApplyLayer()
    {
        EditorApplication.delayCall -= ApplyLayer;

        if (this == null || WorldParameters.Instance == null)
            return;

        gameObject.layer = WorldParameters.Instance.GetColorGroundMask(color);
    }
#endif
}
