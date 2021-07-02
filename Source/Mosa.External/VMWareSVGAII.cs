using Mosa.Kernel.x86;
using Mosa.Runtime;

namespace Mosa.External
{
    public class VMWareSVGAII
    {
        public enum Register : ushort
        {
            ID = 0,
            Enable = 1,
            Width = 2,
            Height = 3,
            BitsPerPixel = 7,
            FrameBufferStart = 13,
            VRamSize = 15,
            MemStart = 18,
            MemSize = 19,
            ConfigDone = 20,
            Sync = 21,
            Busy = 22,
            FifoNumRegisters = 293
        }

        private enum ID : uint
        {
            Magic = 0x900000,
            V2 = (Magic << 8) | 2,
        }

        public enum FIFO : uint
        {
            Min = 0,
            Max = 4,
            NextCmd = 8,
            Stop = 12
        }

        private enum FIFOCommand
        {
            Update = 1
        }

        private enum IOPortOffset : byte
        {
            Index = 0,
            Value = 1,
        }

        private ushort IndexPort;
        private ushort ValuePort;
        public MemoryBlock Video_Memory;
        private MemoryBlock FIFO_Memory;
        private PCIDevice device;
        public uint height;
        public uint width;
        public uint depth;

        public VMWareSVGAII()
        {
            device = (PCI.GetDevice(VendorID.VMWare, DeviceID.SVGAIIAdapter));
            device.EnableMemory(true);
            uint basePort = device.BaseAddressBar[0].BaseAddress;
            IndexPort = (ushort)(basePort + (uint)IOPortOffset.Index);
            ValuePort = (ushort)(basePort + (uint)IOPortOffset.Value);
            WriteRegister(Register.ID, (uint)ID.V2);
            if (ReadRegister(Register.ID) != (uint)ID.V2)
                return;
            Video_Memory = Memory.GetPhysicalMemory(new Pointer(ReadRegister(Register.FrameBufferStart)), ReadRegister(Register.VRamSize));
            InitializeFIFO();
        }

        protected void InitializeFIFO()
        {
            FIFO_Memory = Memory.GetPhysicalMemory(new Pointer(ReadRegister(Register.MemStart)), ReadRegister(Register.MemSize));
            FIFO_Memory.Write32((uint)FIFO.Min, (uint)Register.FifoNumRegisters * 4);
            FIFO_Memory.Write32((uint)FIFO.Max, FIFO_Memory.Size);
            FIFO_Memory.Write32((uint)FIFO.NextCmd, (uint)FIFO.Min);
            FIFO_Memory.Write32((uint)FIFO.Stop, FIFO_Memory.Read32((uint)FIFO.Min));
            WriteRegister(Register.ConfigDone, 1);
        }

        public void SetMode(uint width, uint height, uint depth = 32)
        {
            Disable();
            this.depth = (depth / 8);
            this.width = width;
            this.height = height;
            WriteRegister(Register.Width, width);
            WriteRegister(Register.Height, height);
            WriteRegister(Register.BitsPerPixel, depth);
            Enable();
            InitializeFIFO();
        }

        protected void WriteRegister(Register register, uint value)
        {
            IOPort.Out32(IndexPort, (uint)register);
            IOPort.Out32(ValuePort, value);
        }

        protected uint ReadRegister(Register register)
        {
            IOPort.Out32(IndexPort, (uint)register);
            return IOPort.In32(ValuePort);
        }

        protected void SetFIFO(FIFO cmd, uint value)
        {
            FIFO_Memory.Write32((uint)cmd, value);
            return;
        }

        uint nextcmd = 1172;

        public void Update()
        {
            if (nextcmd == 1212) { nextcmd = 1172; }

            SetFIFO((FIFO)(nextcmd), (uint)FIFOCommand.Update);
            SetFIFO(FIFO.NextCmd, nextcmd + 4);
            nextcmd += 4;

            SetFIFO((FIFO)(nextcmd), 0);
            SetFIFO(FIFO.NextCmd, nextcmd + 4);
            nextcmd += 4;

            SetFIFO((FIFO)(nextcmd), 0);
            SetFIFO(FIFO.NextCmd, nextcmd + 4);
            nextcmd += 4;

            SetFIFO((FIFO)(nextcmd), width);
            SetFIFO(FIFO.NextCmd, nextcmd + 4);
            nextcmd += 4;

            SetFIFO((FIFO)(nextcmd), height);
            SetFIFO(FIFO.NextCmd, nextcmd + 4);
            nextcmd += 4;
        }

        public void Enable()
        {
            WriteRegister(Register.Enable, 1);
        }

        public void Disable()
        {
            WriteRegister(Register.Enable, 0);
        }
    }
}
