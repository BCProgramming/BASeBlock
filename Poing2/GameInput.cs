using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
/*
using Microsoft.DirectX.DirectInput;
using Action=Microsoft.DirectX.DirectInput.Action;

namespace BASeCamp.BASeBlock
{
    //GameInput classes... currently unused.
    class GameInput
    {



    }
    public class DIGamepadEventArgs : EventArgs
    {
        public DirectInputGameInput Sender=null;
        internal JoystickState CurrentState;
        internal JoystickState PreviousState;

        public DIGamepadEventArgs(DirectInputGameInput pSender, JoystickState PrevState, JoystickState CurrState)
        {
            Sender=pSender;
            PreviousState=PrevState;
            CurrentState=CurrState;
            


        }


    }

    public class DirectInputGameInput
    {
        private iGameClient _Owner=null;
        
        private class JoystickHandler
        {
            public event EventHandler<DIGamepadEventArgs> GameInputEvent;
            private Device ourdevice=null;
            private void InvokeEvent(DIGamepadEventArgs eargs)
            {

                GameInputEvent.Invoke(this, eargs);

            }
            private void InvokeEvent(DirectInputGameInput ginput, JoystickState PreviousState, JoystickState currstate)
            {
                GameInputEvent.Invoke(ginput,new DIGamepadEventArgs(ginput, PreviousState, currstate));


            }

            public JoystickHandler(DirectInputGameInput ownerclass, Device usedevice)
            {
                ourdevice=usedevice;
                ourdevice.SetCooperativeLevel(ownerclass._Owner.Handle,
              CooperativeLevelFlags.Background |
              CooperativeLevelFlags.NonExclusive);
                ActionFormat Af = new ActionFormat();
                ourdevice.BuildActionMap(Af, ActionMapControl.Initialize);
                
                

                ourdevice.SetDataFormat(DeviceDataFormat.Joystick);
                ourdevice.Acquire();
              

            }
            private JoystickState jstate;

            private bool testhighbit(byte testthis)
            {
                return ((testthis & 0x80) >> 8) > 0;


            }

            public void Poll()
            {
                
                ourdevice.Poll();
                JoystickState newstate = ourdevice.CurrentJoystickState;
                //check the buttons.
                byte[] currbuttonstate = newstate.GetButtons();
                byte[] prevbuttonstate = jstate.GetButtons();


                for (int i = 0; i < currbuttonstate.Length; i++)
                {




                }





            }

        }

        private JoystickHandler[] AllJoysticks= null;
        private Thread JoystickPollThread = null; //thread used for polling joysticks.
        private bool PollingActive=false;

        private void PollingThread()
        {

            while (PollingActive)
            {
                Thread.Sleep(10); //10 ms currently. Maybe make this configurable...

                //tell each JoystickHandler() to poll.
                foreach (JoystickHandler jh in AllJoysticks)
                {
                    jh.Poll();

                }



            }




        }

        public DirectInputGameInput(iGameClient owner,IWin32Window windowobject)
        {
            //initialize devices.
            DeviceList gameControllerList = Manager.GetDevices(DeviceClass.GameControl,
            EnumDevicesFlags.AttachedOnly);


            // check that we have at least one device.
            List<JoystickHandler> buildhandlers = new List<JoystickHandler>();
            if (gameControllerList.Count > 0)
            {
                // Move to the first device

                gameControllerList.MoveNext();
                DeviceInstance deviceInstance = (DeviceInstance)
                    gameControllerList.Current;

                // create a device from this controller.
                
                Device joystickDevice = new Device(deviceInstance.InstanceGuid);

                buildhandlers.Add(new JoystickHandler(this,joystickDevice));
          
            }

           
            
         


        }



    }

}
*/