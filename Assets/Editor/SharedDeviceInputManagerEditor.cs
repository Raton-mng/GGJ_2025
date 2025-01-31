using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Editor;

[CustomEditor(typeof(SharedDeviceInputManager))]
public class SharedDeviceInputManagerEditor : UnityEditor.Editor
{
    private SharedDeviceInputManager inputManager;
    private UnityEditor.Editor defaultEditor;

    private void OnEnable()
    {
        inputManager = target as SharedDeviceInputManager;
        defaultEditor = UnityEditor.Editor.CreateEditor(inputManager as PlayerInputManager, typeof(PlayerInputManagerEditor));
    }

    public override void OnInspectorGUI()
    {
        defaultEditor.OnInspectorGUI();
    }
}

