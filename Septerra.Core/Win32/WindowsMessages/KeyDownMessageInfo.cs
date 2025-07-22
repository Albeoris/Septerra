using System;
using System.Windows.Forms;

namespace Septerra;

/// <summary>
/// Encapsulates message information for a WM_KEYDOWN message.
/// </summary>
public class KeyDownMessageInfo
{
    private Int32 wParam;
    private Int32 lParam;

    /// <summary>
    /// Initializes a new instance of the KeyDownMessageInfo class.
    /// </summary>
    /// <param name="wParam">The WPARAM value as received with the WM_KEYDOWN message.</param>
    /// <param name="lParam">The LPARAM value as received with the WM_KEYDOWN message.</param>
    public KeyDownMessageInfo(Int32 wParam, Int32 lParam)
    {
        this.wParam = wParam;
        this.lParam = lParam;
    }

    /// <summary>
    /// Gets the virtual-key code of the key that generated the WM_KEYDOWN message.
    /// </summary>
    public Keys VirtualKeyCode => (Keys)wParam;

    /// <summary>
    /// Gets the repeat count for the current message. Indicates how many times the keystroke is repeated as a result of the user holding down the key.
    /// </summary>
    public Int32 RepeatCount => lParam & 0xFFFF;

    /// <summary>
    /// Gets the scan code. The value depends on the OEM.
    /// </summary>
    public Int32 ScanCode => (lParam >> 16) & 0xFF;

    /// <summary>
    /// Gets a value indicating whether the key is an extended key, such as the right-hand ALT and CTRL keys that appear on an enhanced 101- or 102-key keyboard.
    /// </summary>
    public Boolean IsExtendedKey => (lParam & 0x01000000) != 0;

    /// <summary>
    /// Gets a value indicating whether the key was previously down before this message was sent.
    /// </summary>
    public Boolean WasKeyDown => (lParam & 0x40000000) != 0;

    /// <summary>
    /// Gets a value indicating the transition state. The value is always 0 for a WM_KEYDOWN message.
    /// </summary>
    public Boolean IsUp => (lParam & 0x80000000) != 0;
}