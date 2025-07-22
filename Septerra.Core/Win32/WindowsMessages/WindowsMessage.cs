namespace Septerra;

/// <summary>
/// Defines Windows messages commonly used in message handling.
/// </summary>
public enum WindowsMessage
{
    /// <summary>
    /// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem key is pressed.
    /// wParam: The virtual-key code of the nonsystem key.
    /// lParam: The repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag.
    /// </summary>
    WM_KEYDOWN = 0x0100,

    /// <summary>
    /// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem key is released.
    /// wParam: The virtual-key code of the nonsystem key.
    /// lParam: The repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag.
    /// </summary>
    WM_KEYUP = 0x0101,
    
    /// <summary>
    /// The WM_CHAR message is posted to the window with the keyboard focus when a WM_KEYDOWN message is translated by the TranslateMessage function.
    /// wParam: The character code of the key.
    /// lParam: The repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag.
    /// </summary>
    WM_CHAR = 0x0102,

    /// <summary>
    /// The WM_SYSKEYDOWN message is sent to the window with the keyboard focus when the user presses a key that is not a regular key such as the ALT key while holding down the ALT key.
    /// wParam: The virtual-key code of the key.
    /// lParam: The repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag.
    /// </summary>
    WM_SYSKEYDOWN = 0x0104,
    
    /// <summary>
    /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user releases a key that was pressed while the ALT key was held down.
    /// wParam: The virtual-key code of the key.
    /// lParam: The repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag.
    /// </summary>
    WM_SYSKEYUP = 0x0105
}