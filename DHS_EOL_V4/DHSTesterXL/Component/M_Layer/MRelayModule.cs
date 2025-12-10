using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHSTesterXL
{
    public enum MRModuleRY
    {
        LockLampCh1,
        PowerLampCh1,
        TouchRelayCh1,
        CancelRelayCh1,
        LockLampCh2,
        PowerLampCh2,
        TouchRelayCh2,
        CancelRelayCh2,
        Count
    }

    public class MRelayModule : HModbusRTU
    {
        private byte _slaveAddress = 1;
        private ushort _startAddress = 0;
        private bool[] _relays = new bool[(int)MRModuleRY.Count] { false, false, false, false, false, false, false, false };

        public MRelayModule()
        {
        }

        public MRelayModule(byte slaveAddress)
        {
            _slaveAddress = slaveAddress;
        }

        public byte GetSlaveAddress()
        {
            return _slaveAddress;
        }

        public void SetSlaveAddress(byte slaveAddress)
        {
            _slaveAddress = slaveAddress;
        }

        public ushort GetStartAddress()
        {
            return _startAddress;
        }

        public void SetStartAddress(ushort startAddress)
        {
            _startAddress = startAddress;
        }

        public async Task<bool[]> GetRelayStateAsync()
        {
            Task<bool[]> datas = Task.Run(async () => await ReadCoilsAsync(_slaveAddress, 0, (ushort)_relays.Length));
            for (int i = 0; i < _relays.Length; i++)
                _relays[i] = (await datas)[i];
            return _relays;
        }

        public void SetRelayStateAsync(int relayIndex, bool relayState)
        {
            _relays[relayIndex] = relayState;
            Task.Run(async () => await WriteMultipleCoilsAsync(_slaveAddress, 0, _relays));
        }

        public bool GetLockLampStateCh1()
        {
            return _relays[(int)MRModuleRY.LockLampCh1];
        }

        public void SetLockLampStateCh1(bool onOff)
        {
            SetRelayStateAsync((int)MRModuleRY.LockLampCh1, onOff);
        }

        public bool GetPowerLampStateCh1()
        {
            return _relays[(int)MRModuleRY.PowerLampCh1];
        }

        public void SetPowerLampStateCh1(bool onOff)
        {
            SetRelayStateAsync((int)MRModuleRY.PowerLampCh1, onOff);
        }

        public bool GetLockLampStateCh2()
        {
            return _relays[(int)MRModuleRY.LockLampCh2];
        }

        public void SetLockLampStateCh2(bool onOff)
        {
            SetRelayStateAsync((int)MRModuleRY.LockLampCh2, onOff);
        }

        public bool GetPowerLampStateCh2()
        {
            return _relays[(int)MRModuleRY.PowerLampCh2];
        }

        public void SetPowerLampStateCh2(bool onOff)
        {
            SetRelayStateAsync((int)MRModuleRY.PowerLampCh2, onOff);
        }
    }
}
