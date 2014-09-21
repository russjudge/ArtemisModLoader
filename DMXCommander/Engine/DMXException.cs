using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace DMXCommander.Engine
{

    public class DMXException : Exception
    {
        /*

        FT_INVALID_HANDLE,
        FT_DEVICE_NOT_FOUND,
        FT_DEVICE_NOT_OPENED,
        FT_IO_ERROR,
        FT_INSUFFICIENT_RESOURCES,
        FT_INVALID_PARAMETER,
        FT_INVALID_BAUD_RATE,
        FT_DEVICE_NOT_OPENED_FOR_ERASE,
        FT_DEVICE_NOT_OPENED_FOR_WRITE,
        FT_FAILED_TO_WRITE_DEVICE,
        FT_EEPROM_READ_FAILED,
        FT_EEPROM_WRITE_FAILED,
        FT_EEPROM_ERASE_FAILED,
        FT_EEPROM_NOT_PRESENT,
        FT_EEPROM_NOT_PROGRAMMED,
        FT_INVALID_ARGS,
        FT_OTHER_ERROR

            */
        static string[] FTErrors = { "", "Invalid Handle", "Device Not Found",
                                       "Device Not Opened",
                                       "IO Error",
                                       "Insufficient Resources",
                                       "Invalid Parameter",
                                       "Invalid Baud Rate",
                                       "Device Not Opened for Erase",
                                       "Device Not Opened for Write",
                                       "Failed to Write to Device",
                                       "EEPROM Write Failed",
                                       "EEPROM Write Erase Failed",
                                       "EEPROM Not Present",
                                       "EEPROM Not Programmed",
                                       "Invalid Arguments",
                                       "Other Error" };
        public DMXException(FT_STATUS status)
            : base(status.ToString() + ": " + FTErrors[(int)status])
        {

        }
    }
}
