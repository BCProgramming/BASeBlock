using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.DirectInput;
///some gamepad input classes. 
namespace BASeBlock
{
    public class JoystickDevice
    {
        private Device ourdevice;
        private JoystickState laststate;
        private  bool haspolled=false;
        public JoystickDevice(Device deviceobject)
        {
            ourdevice = deviceobject;
            ourdevice.Acquire();


        }
        public void PollChanges(iGameClient GameCli)
        {
            //polls changes in this controller from the previous poll and raises
            //appropriate events in iGameClient depending on the differences.
            if (!haspolled)
            {
                haspolled=true;
                ourdevice.Poll();
                laststate = ourdevice.CurrentJoystickState;
                return;

            }
            laststate = ourdevice.CurrentJoystickState;
            ourdevice.Poll();


            byte[] prevbuttons = laststate.GetButtons();
            byte[] currbuttons = ourdevice.CurrentJoystickState.GetButtons();
            



        }



    }
    class gamepadinput : iGameInput 
    {

        private iGameClient useClient;

        private List<JoystickDevice> joysticks = new List<JoystickDevice>();
        public void Initialize(iGameClient gamecli)
        {
            useClient = gamecli;
            DeviceList GameControllerList = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
            if (GameControllerList.Count > 0)
            {
                GameControllerList.MoveNext();
                DeviceInstance deviceInstance = (DeviceInstance)
                    GameControllerList.Current;

                // create a device from this controller.

                Device jDevice = new Device(deviceInstance.InstanceGuid);
                jDevice.SetCooperativeLevel(useClient.Handle,
                    CooperativeLevelFlags.Background |
                    CooperativeLevelFlags.NonExclusive);

                joysticks.Add(new JoystickDevice(jDevice));




            }



        }
        //designed to poll all joysticks and report any applicable changes from their previous state
        //via the Input events.
        public void PollChanges()
        {
            foreach (JoystickDevice loopdevice in joysticks)
            {
                loopdevice.PollChanges(useClient);


            }



        }

    }
}
