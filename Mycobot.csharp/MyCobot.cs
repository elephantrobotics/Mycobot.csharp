using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace Mycobot.csharp
{
    public class MyCobot
    {
        public string name = "mycobot";
        private static SerialPort _serialPort;

        public MyCobot(string port, int baud = 115200)
        {
            _serialPort = new SerialPort(port, baud) { DtrEnable = true, RtsEnable = true };
        }

        public bool IsOpen => _serialPort != null && _serialPort.IsOpen;

        public bool Open()
        {
            try
            {
                if (_serialPort == null)
                    return this.IsOpen;
                if (_serialPort.IsOpen)
                    this.Close();
                _serialPort.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _serialPort.Close();
            }

            return this.IsOpen;
        }

        public void Close()
        {
            if (this.IsOpen)
                _serialPort.Close();
        }

        /// <summary>
        /// Write byte[] to buffer, when port is open
        /// </summary>
        /// <param name="send">byte[]</param>
        /// <param name="offset">offset of 0</param>
        /// <param name="count">data count</param>
        public void Write(byte[] send, int offset, int count)
        {
            if (this.IsOpen)
            {
                _serialPort.Write(send, offset, count);
            }
        }

        public void Dispose()
        {
            if (_serialPort == null)
                return;
            if (_serialPort.IsOpen)
                this.Close();
            _serialPort.Dispose();
            _serialPort = null;
        }

        private static byte[] Int16ToBytes(int v)
        {
            var res = new byte[2];
            res[0] = (byte)((v >> 8) & 0xff);
            res[1] = (byte)((v) & 0xff);
            return res;
        }

        private static IEnumerable<byte> Int16ArrToBytes(IReadOnlyCollection<int> vs)
        {
            var res = new byte[vs.Count * 2];
            var idx = 0;
            foreach (var t in vs)
            {
                var one = Int16ToBytes(t);
                res[idx++] = one[0];
                res[idx++] = one[1];
            }

            return res;
        }

        private static int getValidIndex(IReadOnlyList<byte> bs)
        {
            for (var i = 0; i < bs.Count; ++i)
            {
                if (bs[i] == 0xfe && bs[i + 1] == 0xfe)
                    return i + 2;
            }

            return -1;
        }

        /// <summary>
        /// arm power on
        /// </summary>
        public void PowerOn()
        {
            byte[] command = { 0xfe, 0xfe, 0x02, 0x10, 0xfa };
            Write(command, 0, 5);
        }

        /// <summary>
        /// arm power off
        /// </summary>
        public void PowerOff()
        {
            byte[] command = { 0xfe, 0xfe, 0x02, 0x11, 0xfa };
            Write(command, 0, 5);
        }

        /// <summary>
        /// Send one angle value
        /// </summary>
        /// <param name="jointNo">joint number: 1 ~ 6</param>
        /// <param name="angle">angle value: -180 ~ 180 </param>
        /// <param name="speed">speed value: 0 ~ 100</param>
        public void SendOneAngle(int jointNo, int angle, int speed)
        {
            int _angle = angle * 100, idx = 0;
            var command = new byte[9];
            // set header
            command[idx++] = 0xfe;
            command[idx++] = 0xfe;
            command[idx++] = 0x06;
            command[idx++] = 0x21;
            // process data
            command[idx++] = (byte)(jointNo);
            command[idx++] = (byte)((_angle >> 8) & 0xff);
            command[idx++] = (byte)(_angle & 0xff);
            command[idx++] = (byte)speed;
            // set footer
            command[idx++] = 0xfa;
            Write(command, 0, command.Length);
        }

        /// <summary>
        /// Send all angles
        /// </summary>
        /// <param name="angles">angles[], length: 6</param>
        /// <param name="speed">speed value: 0 ~ 100</param>
        public void SendAngles(int[] angles, int speed)
        {
            var command = new byte[18];
            var idx = 0;
            command[idx++] = 0xfe;
            command[idx++] = 0xfe;
            command[idx++] = 0x0f;
            command[idx++] = 0x22;
            for (var j = 0; j < angles.Length; ++j)
            {
                angles[j] *= 100;
            }
            var a = Int16ArrToBytes(angles);
            foreach (var t in a)
            {
                command[idx++] = t;
            }

            command[idx++] = (byte)speed;
            command[idx++] = 0xfa;
            Write(command, 0, command.Length);
        }

        /// <summary>
        /// Get all angles
        /// </summary>
        /// <returns>int[], length: 6</returns>
        public int[] GetAngles()
        {
            byte[] command = { 0xfe, 0xfe, 0x02, 0x20, 0xfa };
            Write(command, 0, command.Length);

            Thread.Sleep(200);
            // read data
            // Console.WriteLine(_serialPort.BytesToRead);

            var m_recvBytes = new byte[_serialPort.BytesToRead];
            var result = _serialPort.Read(m_recvBytes, 0, m_recvBytes.Length);
            if (result <= 0)
                return Array.Empty<int>();

            // get valid index
            var idx = getValidIndex(m_recvBytes);
            if (idx == -1)
                return Array.Empty<int>();

            // process data
            var len = (int)m_recvBytes[idx] - 1;
            var res = new int[6];
            var resIdx = 0;
            for (var i = idx + 1; i < idx + len; i += 2)
            {
                res[resIdx++] = BitConverter.ToInt16(m_recvBytes, i) / 100;
            }

            return res;
        }

        /// <summary>
        /// Send one coord
        /// </summary>
        /// <param name="coord">coord No: 1 - 6</param>
        /// <param name="value">coord value</param>
        /// <param name="speed">speed: 0 ~ 100</param>
        public void SendOneCoord(int coord, int value, int speed)
        {
            int idx = 0, _value = coord < 4 ? coord * 10 : coord * 100;
            var command = new byte[9];
            // set header
            command[idx++] = 0xfe;
            command[idx++] = 0xfe;
            command[idx++] = 0x06;
            command[idx++] = 0x24;
            // process data
            command[idx++] = (byte)(coord);
            command[idx++] = (byte)((_value >> 8) & 0xff);
            command[idx++] = (byte)(_value & 0xff);
            command[idx++] = (byte)speed;
            // set footer
            command[idx++] = 0xfa;

            Write(command, 0, command.Length);

        }

        /// <summary>
        /// Send all coords to arm
        /// </summary>
        /// <param name="coords">int[], length: 6</param>
        /// <param name="speed">speed: int, value: 0 ~ 100</param>
        /// <param name="mode">mode:  0 - angular, 1 - linear</param>
        public void SendCoords(int[] coords, int speed, int mode)
        {
            var command = new byte[19];
            var idx = 0;
            // set header
            command[idx++] = 0xfe;
            command[idx++] = 0xfe;
            command[idx++] = 0x10;
            command[idx++] = 0x25;
            // process coords
            for (var i = 0; i < 3; ++i)
            {
                coords[i] *= 10;
            }
            for (var i = 3; i < 6; ++i)
            {
                coords[i] *= 100;
            }
            // append to command
            var a = Int16ArrToBytes(coords);
            foreach (var t in a)
            {
                command[idx++] = t;
            }

            command[idx++] = (byte)speed;
            command[idx++] = (byte)mode;
            // set footer
            command[idx++] = 0xfa;

            Write(command, 0, command.Length);

        }

        /// <summary>
        /// Get all coord
        /// </summary>
        /// <returns>int[], length: 6</returns>
        public int[] GetCoords()
        {
            byte[] command = { 0xfe, 0xfe, 0x02, 0x23, 0xfa };
            Write(command, 0, command.Length);

            Thread.Sleep(200);
            // read data
            var m_recvBytes = new byte[_serialPort.BytesToRead];
            var result = _serialPort.Read(m_recvBytes, 0, m_recvBytes.Length);
            if (result <= 0)
                return Array.Empty<int>();

            // get valid index
            var idx = getValidIndex(m_recvBytes);
            if (idx == -1)
                return Array.Empty<int>();

            // process data
            var len = (int)m_recvBytes[idx] - 1;
            var res = new int[6];
            var resIdx = 0;
            for (var i = idx + 1; i < idx + len; i += 2)
            {
                int v = BitConverter.ToInt16(m_recvBytes, i);
                res[resIdx] = resIdx < 3 ? v / 10 : v / 100;
                resIdx++;
            }

            return res;
        }

        /*
         * set m5 io output 
         * pin_number:io number(2 5 26)
         * signal: High and low level
         */
        public void SetBasicOut(byte pin_number, byte signal)
        {
            byte[] command = {0xfe, 0xfe, 0x04, 0xa0, pin_number, signal, 0xfa};
            Write(command, 0, command.Length);
        }

        /*
         * get basic io input 
         * pin_number:io number(35 36)
         */
        public int GetBasicIn(byte pin_number)
        { 
            byte[] command = {0xfe, 0xfe, 0x03, 0xa1, pin_number, 0xfa};
            Write(command, 0, command.Length);
            Thread.Sleep(200);

            // read data
            var m_recvBytes = new byte[_serialPort.BytesToRead];
            var result = _serialPort.Read(m_recvBytes, 0, m_recvBytes.Length);
            if (result <= 0)
                return -1;

            // get valid index
            var idx = getValidIndex(m_recvBytes);
            if (idx == -1)
                return -1;
            var len = (int)m_recvBytes[idx] - 1;
            int signal = m_recvBytes[idx + len];
            return signal;
        }

        /*
         * set m5 io output 
         * pin_number:io number(23 33)
         * signal: High and low level
         */
        public void SetDigitalOut(byte pin_number, byte signal)
        {
            byte[] command = {0xfe, 0xfe, 0x04, 0x61, pin_number, signal, 0xfa};
            Write(command, 0, command.Length);
        }

        /*
         * get m5 io input 
         * pin_number:io number(22 19)
         */
        public int GetDigitalIn(byte pin_number)
        { 
            byte[] command = {0xfe, 0xfe, 0x03, 0x62, pin_number, 0xfa};
            Write(command, 0, command.Length);
            Thread.Sleep(200);

            // read data
            var m_recvBytes = new byte[_serialPort.BytesToRead];
            var result = _serialPort.Read(m_recvBytes, 0, m_recvBytes.Length);
            if (result <= 0)
                return -1;

            // get valid index
            var idx = getValidIndex(m_recvBytes);

            if (idx == -1)
                return -1;
            var len = (int)m_recvBytes[idx] - 1;
            int signal = m_recvBytes[idx + len];

            return signal;
        }

        public void setGripperValue(byte angle, byte speed)
        {
            byte[] command = { 0xfe, 0xfe, 0x04, 0x67, angle, speed, 0xfa };
            Write(command, 0, command.Length);
        }

        public int getGripperValue()
        {
            byte[] command = { 0xfe, 0xfe, 0x02, 0x65, 0xfa };
            Write(command, 0, command.Length);
            Thread.Sleep(200);

            // read data
            var m_recvBytes = new byte[_serialPort.BytesToRead];
            var result = _serialPort.Read(m_recvBytes, 0, m_recvBytes.Length);
            if (result <= 0)
                return -1;

            // get valid index
            var idx = getValidIndex(m_recvBytes);
            if (idx == -1)
                return -1;
            var len = (int)m_recvBytes[idx] - 1;
            int state = m_recvBytes[idx + len];
           
            return state;
            
        }

        public void setEletricGripper(int state)
        {
            if (state == 0)
            {
                byte[] command = { 0x01, 0x06, 0x01, 0x03, 0x03, 0xE8, 0x78, 0x88};
                Write(command, 0, command.Length);
            } 
            else if (state == 1)
            {
                byte[] command = { 0x01, 0x06, 0x01, 0x03, 0x00, 0x00, 0x78, 0x88};
                Write(command, 0, command.Length);
            }

        }
    }
}
