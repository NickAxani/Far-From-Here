using TMPro;
using UnityEngine;

public class DebugState : MonoBehaviour
{

    public TextMeshProUGUI text;
    public PlayerController player;
    private string state;

    // Update is called once per frame
    void Update()
    {
        if (!player.inDebug)
        {
            text.text = "";
            return;
        }
        switch (player.state)
            {
                case PlayerController.MovementState.idle:
                    state = "idle";
                    break;
                case PlayerController.MovementState.walking:
                    state = "walking";
                    break;
                case PlayerController.MovementState.sprinting:
                    state = "sprinting";
                    break;
                case PlayerController.MovementState.crouching:
                    state = "crouching";
                    break;
                case PlayerController.MovementState.crouchwalking:
                    state = "crouch walking";
                    break;
                case PlayerController.MovementState.air:
                    state = "in the air";
                    break;
            }

        text.text = "Player State: " + state;
    }
}
