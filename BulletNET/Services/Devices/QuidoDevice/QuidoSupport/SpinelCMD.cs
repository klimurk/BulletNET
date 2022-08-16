namespace BulletNET.Services.Devices.QuidoDevice.QuidoSupport
{
    internal class SpinelCMD
    {
        //CMD
        public byte PRE;

        public byte FRM;
        public byte[] NUM;
        public byte ADR;
        public byte SIG;
        public byte INST;
        public byte[] DATA;
        public byte SUMA;
        public byte CR;
        public ushort len;
        public byte[] CMD;
        public string CMDh;

        //ANSW
        public ushort length;

        public ushort lengthD;
        public byte[] ANSW;
        public byte[] ANSWD;

        public SpinelCMD()
        {
        }

        public SpinelCMD(byte _ADR, byte _SIG, byte _INST, byte[] _DATA)
        {
            PRE = 0x2A;
            FRM = 0x61;
            CR = 0x0D;
            ADR = _ADR;
            SIG = _SIG;
            INST = _INST;
            DATA = _DATA;
            len = (ushort)(DATA.Length + 5);
            NUM = BitConverter.GetBytes(len);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(NUM);
            for (int x = 0; x < DATA.Length; x++)
            {
                SUMA += DATA[x];
            }
            SUMA = (byte)(SUMA + PRE + FRM + NUM[1] + NUM[0] + ADR + SIG + INST);
            SUMA = (byte)(255 - SUMA);

            byte[] command = new byte[DATA.Length + 9];
            command[8 + DATA.Length] = CR;
            command[7 + DATA.Length] = SUMA;
            for (int x = 0; x < DATA.Length; x++) { command[7 + x] = DATA[x]; }
            command[6] = INST;
            command[5] = SIG;
            command[4] = ADR;
            command[3] = NUM[1];
            command[2] = NUM[0];
            command[1] = FRM;
            command[0] = PRE;

            CMD = command;
            CMDh = BitConverter.ToString(command);
        }

        //********************************************************************************************************************************************************
        //KONTROLA ODPOVĚDI
        //********************************************************************************************************************************************************

        public int Auditor(byte[] _ANSW)
        {
            byte control_SUM = 0;
            if (_ANSW.Length < 9) return 1;
            if (_ANSW[0] != PRE) return 2;
            if (_ANSW[1] != FRM) return 3;
            length = (ushort)(256 * _ANSW[2] + _ANSW[3]);
            lengthD = (ushort)(length - 5);
            if (_ANSW.Length != length + 4)
            {
                return 4;
            }
            byte[] data = new byte[lengthD];
            if (_ANSW[4] != ADR) return 5;
            if (_ANSW[5] != SIG) return 6;
            if (_ANSW[6] != 0) return _ANSW[6] + 1000;

            //SUMA + fill ANSW
            for (int x = 0; x < lengthD; x++)
            {
                data[x] = _ANSW[7 + x];
                control_SUM += _ANSW[7 + x];
            }
            control_SUM = (byte)(control_SUM + _ANSW[0] + _ANSW[1] + _ANSW[2] + _ANSW[3] + _ANSW[4] + _ANSW[5] + _ANSW[6]);
            control_SUM = (byte)(255 - control_SUM);

            if (_ANSW[7 + lengthD] != control_SUM) return 7;
            if (_ANSW[8 + lengthD] != CR) return 8;

            ANSW = _ANSW;
            ANSWD = data;
            return 0;
        }
    }
}