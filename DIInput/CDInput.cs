using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BASeBlock;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;

namespace DIInput
{

    public class cDInput : iGameInput
    {
        DeviceInstance ourdevice;
        Device gamedevice;
        Thread JoystickPoller = null;
        iGameClient usecli = null;
        /// <summary>
        /// initialize this Input device to use the given GameClient; input should be
        /// sent to that implementation (button down and button up and so forth)
        /// </summary>
        /// <param name="gamecli"></param>
        public string Name { get { return "DirectInput"; } }
        public void Initialize(iGameClient gamecli)
        {
            usecli = gamecli;
            //get list of devices...
            DeviceList GameControllerList = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
            if (GameControllerList.Count > 0)
            {
                //move to first item.
                GameControllerList.MoveNext();

                ourdevice = (DeviceInstance)GameControllerList.Current;

                //create the device.
                gamedevice = new Device(ourdevice.InstanceGuid);
                gamedevice.SetCooperativeLevel(gamecli.Handle, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
                gamedevice.SetDataFormat(DeviceDataFormat.Joystick);
                gamedevice.Acquire();
                //create our polling thread.
                JoystickPoller = new Thread(PollRoutine);
            }
        }
        JoystickState lastJoystate;
        bool stoppollthread = false;
        public void PollRoutine()
        {
            while (!stoppollthread)
            {
                Thread.Sleep(25);
                gamedevice.Poll();

                JoystickState currentstate = gamedevice.CurrentJoystickState;


                //detect changes between the last and the current state.
                byte[] currbuttonstate = currentstate.GetButtons();
                byte[] prevbuttonstate = lastJoystate.GetButtons();
                ButtonConstants[] bc = new ButtonConstants[] { ButtonConstants.Button_A, ButtonConstants.Button_B, ButtonConstants.Button_C, ButtonConstants.Button_D };
                for (int i = 0; i < bc.Length; i++)
                {
                    bool ispressednow = ((currbuttonstate[i] & 0xF0) == 0xF0);
                    bool waspressed = ((prevbuttonstate[i] & 0xF0) == 0xF0);
                    int index = i;
                    if (ispressednow && !waspressed)
                    {
                        //button is now down but wasn't before, fire button down.
                        usecli.Invoke(((MethodInvoker)(() => usecli.InvokeButtonDown(bc[index]))));


                    }
                    else if (!ispressednow && waspressed)
                    {
                        //button was released, fire button up.
                        usecli.Invoke((MethodInvoker)((() => usecli.InvokeButtonUp(bc[index]))));

                    }


                }



                lastJoystate = gamedevice.CurrentJoystickState;










            }



        }
        /// <summary>
        /// called to determine if this implementation has changable settings.
        /// </summary>
        /// <returns></returns>
        public bool hasConfig()
        {
            //not yet...
            return false;
        }

        public void Configure()
        {

        }
    }

}
