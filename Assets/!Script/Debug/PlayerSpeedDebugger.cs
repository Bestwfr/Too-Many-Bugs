using FlamingOrange.CoreSystem;
using UnityEngine;

public class PlayerSpeedDebugger : MonoBehaviour
{
    [SerializeField] private Player player;
    
    public Movement Movement { get => movement ?? player.Core.GetCoreComponent(ref movement); }
    private Movement movement;

    [Header("GUI Settings")]
    [SerializeField] private int fontSize = 24;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Vector2 screenPosition = new Vector2(10, 20);

    private GUIStyle style;

    private void OnGUI()
    {
        if (player != null && player.StateMachine != null && player.StateMachine.CurrentState != null)
        {
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.label);
                style.fontSize = fontSize;
                style.normal.textColor = textColor;
            }

            // Update in case you tweak values live in Inspector
            style.fontSize = fontSize;
            style.normal.textColor = textColor;
            
            float factor = 1f / (1f + Time.fixedDeltaTime * Movement.Rb.linearDamping);

            GUI.Label(
                new Rect(screenPosition.x, screenPosition.y, 400, 50),
                $"Speed: {Movement.Rb.linearVelocity.magnitude * factor} m/s",
                style
            );
        }
    }
}