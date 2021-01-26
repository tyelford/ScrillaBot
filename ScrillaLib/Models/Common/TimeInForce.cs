using System;
using System.Collections.Generic;
using System.Text;

namespace ScrillaLib.Models.Common
{
    public class TimeInForce
    {
        public enum Type
        {
            NONE,
            IOC,  //Immediate or Cancel
            GTC,  //Good till Canceled
            GTD,  //Good till Date
            FOK   //Fill or Kill -- not sure if this is valid
        }
    }
}
