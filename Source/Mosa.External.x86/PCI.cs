//From Cosmos
/*BSD 3-Clause License

Copyright (c) 2021, CosmosOS, COSMOS Project
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.*/

using System.Collections.Generic;

namespace Mosa.External
{
    public enum ClassID
    {
        PCIDevice_2_0 = 0x00,
        MassStorageController = 0x01,
        NetworkController = 0x02,
        DisplayController = 0x03,
        MultimediaDevice = 0x04,
        MemoryController = 0x05,
        BridgeDevice = 0x06,
        SimpleCommController = 0x07,
        BaseSystemPreiph = 0x08,
        InputDevice = 0x09,
        DockingStations = 0x0A,
        Proccesors = 0x0B,
        SerialBusController = 0x0C,
        WirelessController = 0x0D,
        InteligentController = 0x0E,
        SateliteCommController = 0x0F,
        EncryptionController = 0x10,
        SignalProcessingController = 0x11,
        ProcessingAccelerators = 0x12,
        NonEssentialInstsrumentation = 0x13,
        Coprocessor = 0x40,
        Unclassified = 0xFF
    }

    public enum SubclassID
    {
        // MassStorageController: 
        SCSIStorageController = 0x00,
        IDEInterface = 0x01,
        FloppyDiskController = 0x02,
        IPIBusController = 0x03,
        RAIDController = 0x04,
        ATAController = 0x05,
        SATAController = 0x06,
        SASController = 0x07,
        NVMController = 0x08,
        UnknownMassStorage = 0x09,
    }

    public enum ProgramIF
    {
        // MassStorageController:
        SATA_VendorSpecific = 0x00,
        SATA_AHCI = 0x01,
        SATA_SerialStorageBus = 0x02,
        SAS_SerialStorageBus = 0x01,
        NVM_NVMHCI = 0x01,
        NVM_NVMExpress = 0x02
    }

    public enum VendorID
    {
        Intel = 0x8086,
        AMD = 0x1022,
        VMWare = 0x15AD,
        Bochs = 0x1234,
        VirtualBox = 0x80EE
    }

    public enum DeviceID
    {
        SVGAIIAdapter = 0x0405,
        PCNETII = 0x2000,
        BGA = 0x1111,
        VBVGA = 0xBEEF,
        VBoxGuest = 0xCAFE
    }

    public class PCI
    {
        public static List<PCIDevice> Devices;

        public static uint Count
        {
            get { return (uint)Devices.Count; }
        }

        public static void Setup()
        {
            Devices = new List<PCIDevice>();
            if ((PCIDevice.GetHeaderType(0x0, 0x0, 0x0) & 0x80) == 0)
            {
                CheckBus(0x0);
            }
            else
            {
                for (ushort fn = 0; fn < 8; fn++)
                {
                    if (PCIDevice.GetVendorID(0x0, 0x0, fn) != 0xFFFF)
                        break;

                    CheckBus(fn);
                }
            }
        }

        /// <summary>
        /// Check bus.
        /// </summary>
        /// <param name="xBus">A bus to check.</param>
        private static void CheckBus(ushort xBus)
        {
            for (ushort device = 0; device < 32; device++)
            {
                if (PCIDevice.GetVendorID(xBus, device, 0x0) == 0xFFFF)
                    continue;

                CheckFunction(new PCIDevice(xBus, device, 0x0));
                if ((PCIDevice.GetHeaderType(xBus, device, 0x0) & 0x80) != 0)
                {
                    for (ushort fn = 1; fn < 8; fn++)
                    {
                        if (PCIDevice.GetVendorID(xBus, device, fn) != 0xFFFF)
                            CheckFunction(new PCIDevice(xBus, device, fn));
                    }
                }
            }
        }

        private static void CheckFunction(PCIDevice xPCIDevice)
        {
            Devices.Add(xPCIDevice);

            if (xPCIDevice.ClassCode == 0x6 && xPCIDevice.Subclass == 0x4)
                CheckBus(xPCIDevice.SecondaryBusNumber);
        }

        public static bool Exists(PCIDevice pciDevice)
        {
            return GetDevice((VendorID)pciDevice.VendorID, (DeviceID)pciDevice.DeviceID) != null;
        }

        public static bool Exists(VendorID aVendorID, DeviceID aDeviceID)
        {
            return GetDevice(aVendorID, aDeviceID) != null;
        }

        /// <summary>
        /// Get device.
        /// </summary>
        /// <param name="aVendorID">A vendor ID.</param>
        /// <param name="aDeviceID">A device ID.</param>
        /// <returns></returns>
        public static PCIDevice GetDevice(VendorID aVendorID, DeviceID aDeviceID)
        {
            foreach (var xDevice in Devices)
            {
                if ((VendorID)xDevice.VendorID == aVendorID &&
                    (DeviceID)xDevice.DeviceID == aDeviceID)
                {
                    return xDevice;
                }
            }
            return null;
        }

        /// <summary>
        /// Get device.
        /// </summary>
        /// <param name="bus">Bus ID.</param>
        /// <param name="slot">Slot position ID.</param>
        /// <param name="function">Function ID.</param>
        /// <returns></returns>
        public static PCIDevice GetDevice(uint bus, uint slot, uint function)
        {
            foreach (var xDevice in Devices)
            {
                if (xDevice.bus == bus &&
                    xDevice.slot == slot &&
                    xDevice.function == function)
                {
                    return xDevice;
                }
            }
            return null;
        }

        public static PCIDevice GetDeviceClass(ClassID Class, SubclassID SubClass)
        {
            foreach (var xDevice in Devices)
            {
                if ((ClassID)xDevice.ClassCode == Class &&
                    (SubclassID)xDevice.Subclass == SubClass)
                {
                    return xDevice;
                }
            }
            return null;
        }

        public static PCIDevice GetDeviceClass(ClassID aClass, SubclassID aSubClass, ProgramIF aProgIF)
        {
            foreach (var xDevice in Devices)
            {
                if ((ClassID)xDevice.ClassCode == aClass &&
                    (SubclassID)xDevice.Subclass == aSubClass &&
                    (ProgramIF)xDevice.ProgIF == aProgIF)
                {
                    return xDevice;
                }
            }
            return null;
        }
    }
}
