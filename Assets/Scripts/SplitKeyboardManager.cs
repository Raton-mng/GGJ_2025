using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class SharedDeviceInputManager : PlayerInputManager
{
    private string[] controlScheme = new string[] { "Keyboard1", "Keyboard2" };

    private int playerIndex = 0;

    public override void JoinPlayerFromActionIfNotAlreadyJoined(InputAction.CallbackContext context)
    {
        if (!CheckIfPlayerCanJoin())
            return;

        var device = context.control.device;

        if(device is not Keyboard)
        {
            if (PlayerInput.FindFirstPairedToDevice(device) != null)
                return;
        }
            
        var p = JoinPlayer(pairWithDevice: device);

        if (device is Keyboard)
        {
            RebindPlayer(p);
        }
    }

    private void RebindPlayer(PlayerInput obj)
    {
        obj.SwitchCurrentControlScheme(controlScheme[playerIndex], Keyboard.current);
        playerIndex++;
        Debug.Log(playerIndex);
    }
}
