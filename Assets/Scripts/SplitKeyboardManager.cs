using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class SharedDeviceInputManager : PlayerInputManager
{
    private string[] controlScheme = new string[] { "Keyboard1", "Keyboard2" };

    private int playerIndex = 0;
    private int keyboardPlayer = 0;

    public override void JoinPlayerFromActionIfNotAlreadyJoined(InputAction.CallbackContext context)
    {

        if (!CheckIfPlayerCanJoin())
            return;

        var device = context.control.device;

        if(device is not Keyboard)
        {
            if (PlayerInput.FindFirstPairedToDevice(device) != null)
                return;
            JoinPlayer(pairWithDevice: device);
        }
        else
        {
            keyboardPlayer++;
            if(keyboardPlayer > 2)
                return;
            RebindPlayer(JoinPlayer(pairWithDevice: device));
        }
    }

    private void RebindPlayer(PlayerInput obj)
    {
        obj.SwitchCurrentControlScheme(controlScheme[playerIndex], Keyboard.current);
        playerIndex++;
    }
}
