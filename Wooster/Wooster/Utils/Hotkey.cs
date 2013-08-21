using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Wooster.Utils;

namespace MovablePython
{
    public class Hotkey
    {
        #region Interop

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, Keys vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);

        private const uint WmHotkey = 0x312;

        private const uint ModAlt = 0x1;
        private const uint ModControl = 0x2;
        private const uint ModShift = 0x4;
        private const uint ModWin = 0x8;

        private const uint ErrorHotkeyAlreadyRegistered = 1409;

        #endregion

        private static int _currentId;
        private const int MaximumId = 0xBFFF;

        private Keys _keyCode;
        private bool _shift;
        private bool _control;
        private bool _alt;
        private bool _windows;

        [XmlIgnore]
        private int _id;
        [XmlIgnore]
        private bool _registered;

        public event HandledEventHandler Pressed;

        public Hotkey()
            : this(Keys.None, false, false, false, false)
        {
            // No work done here!
        }

        public Hotkey(Keys keyCode, bool shift, bool control, bool alt, bool windows)
        {
            // Assign properties
            this.KeyCode = keyCode;
            this.Shift = shift;
            this.Control = control;
            this.Alt = alt;
            this.Windows = windows;

            // Register us as a message filter
            ComponentDispatcher.ThreadPreprocessMessage += PreFilterMessage;
        }

        private void PreFilterMessage(ref MSG msg, ref bool handled)
        {
            // Only process WM_HOTKEY messages
            if (msg.message != WmHotkey)
            {
                //handled = false;
                return;
            }

            // Check that the ID is our key and we are registerd
            if (this._registered && (msg.wParam.ToInt32() == this._id))
            {
                // Fire the event and pass on the event if our handlers didn't handle it
                handled = this.OnPressed();
            }
            else
            { handled = false; }
        }

        ~Hotkey()
        {
            // Unregister the hotkey if necessary
            if (this.Registered)
            { this.Unregister(); }
        }

        public Hotkey Clone()
        {
            // Clone the whole object
            return new Hotkey(this._keyCode, this._shift, this._control, this._alt, this._windows);
        }

        public bool GetCanRegister()
        {
            // Handle any exceptions: they mean "no, you can't register" :)
            try
            {
                // Attempt to register
                if (!this.Register())
                { return false; }

                // Unregister and say we managed it
                this.Unregister();
                return true;
            }
            catch (Win32Exception)
            { return false; }
            catch (NotSupportedException)
            { return false; }
        }

        public bool Register()
        {
            // Check that we have not registered
            if (this._registered)
            { throw new NotSupportedException("You cannot register a hotkey that is already registered"); }

            // We can't register an empty hotkey
            if (this.Empty)
            { throw new NotSupportedException("You cannot register an empty hotkey"); }

            // Get an ID for the hotkey and increase current ID
            this._id = _currentId;
            _currentId = Hotkey._currentId + 1 % Hotkey.MaximumId;

            // Translate modifier keys into unmanaged version
            uint modifiers = (this.Alt ? ModAlt : 0) | (this.Control ? ModControl : 0) |
                            (this.Shift ? ModShift : 0) | (this.Windows ? ModWin : 0);

            // Register the hotkey
            if (RegisterHotKey(IntPtr.Zero, this._id, modifiers, this._keyCode) == 0)
            {
                // Is the error that the hotkey is registered?
                if (Marshal.GetLastWin32Error() == ErrorHotkeyAlreadyRegistered)
                {
                    return false;
                }

                throw new Win32Exception();
            }

            // Save the control reference and register state
            this._registered = true;

            // We successfully registered
            return true;
        }
        
        public void Unregister()
        {
            // Check that we have registered
            if (!this._registered)
            { throw new NotSupportedException("You cannot unregister a hotkey that is not registered"); }
                        
            try
            {
                // Clean up after ourselves
                UnregisterHotKey(IntPtr.Zero, this._id);
            }
            catch (Exception)
            { /* who cares */ }

            // Clear the control reference and register state
            this._registered = false;
        }

        private void Reregister()
        {
            // Only do something if the key is already registered
            if (!this._registered)
            { return; }
            
            // Unregister and then reregister again
            this.Unregister();
            this.Register();
        }

        private bool OnPressed()
        {
            // Fire the event if we can
            var handledEventArgs = new HandledEventArgs(false);
            if (this.Pressed != null)
            { this.Pressed(this, handledEventArgs); }

            // Return whether we handled the event or not
            return handledEventArgs.Handled;
        }

        public override string ToString()
        {
            // We can be empty
            if (this.Empty)
            { return "(none)"; }

            // Build key name
            string keyName = Enum.GetName(typeof(Keys), this._keyCode);
            switch (this._keyCode)
            {
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    // Strip the first character
                    keyName = keyName.Substring(1);
                    break;
                default:
                    // Leave everything alone
                    break;
            }

            // Build modifiers
            string modifiers = "";
            if (this._shift)
            { modifiers += "Shift+"; }
            if (this._control)
            { modifiers += "Control+"; }
            if (this._alt)
            { modifiers += "Alt+"; }
            if (this._windows)
            { modifiers += "Windows+"; }

            // Return result
            return modifiers + keyName;
        }

        public bool Empty
        {
            get { return this._keyCode == Keys.None; }
        }

        public bool Registered
        {
            get { return this._registered; }
        }

        public Keys KeyCode
        {
            get { return this._keyCode; }
            set
            {
                // Save and reregister
                this._keyCode = value;
                this.Reregister();
            }
        }

        public bool Shift
        {
            get { return this._shift; }
            set
            {
                // Save and reregister
                this._shift = value;
                this.Reregister();
            }
        }

        public bool Control
        {
            get { return this._control; }
            set
            {
                // Save and reregister
                this._control = value;
                this.Reregister();
            }
        }

        public bool Alt
        {
            get { return this._alt; }
            set
            {
                // Save and reregister
                this._alt = value;
                this.Reregister();
            }
        }

        public bool Windows
        {
            get { return this._windows; }
            set
            {
                // Save and reregister
                this._windows = value;
                this.Reregister();
            }
        }
    }
}
