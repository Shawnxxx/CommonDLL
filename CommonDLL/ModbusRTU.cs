using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDLL
{
    public class ModbusRTU
    {

        /// <summary>
        /// 检查CRC
        /// </summary>
        /// <param name="response"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        private bool CheckResponse(byte[] response, int Length)
        {
            byte[] CRC = new byte[2];
            GetCRC(response, Length, ref CRC);
            try
            {
                if (CRC[0] == response[Length - 2] && CRC[1] == response[Length - 1])
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 计算CRC
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Length"></param>
        /// <param name="CRC"></param>
        private void GetCRC(byte[] message, int Length, ref byte[] CRC)
        {
            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < Length - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }
        /// <summary>
        /// 读寄存器
        /// </summary>
        /// <param name="funcCode"></param>
        /// <param name="address"></param>
        /// <param name="start"></param>
        /// <param name="Count"></param>
        public byte[] ModbusRTU_Read(byte funcCode, int address, int start, int Count)
        {
            byte[] message = new byte[8];
            byte[] CRC = new byte[2];
            message[0] = (byte)address;
            message[1] = (byte)0x03;        //写寄存器
            message[2] = (byte)(start >> 8);
            message[3] = (byte)(start & 0xFF);
            GetCRC(message, message.Length, ref CRC);
            message[message.Length - 2] = CRC[0];
            message[message.Length - 1] = CRC[1];
            return message;
        }
        /// <summary>
        /// 写寄存器
        /// </summary>
        /// <param name="funcCode"></param>
        /// <param name="address"></param>
        /// <param name="start"></param>
        /// <param name="Count"></param>
        public byte[] ModbusRTU_Write(byte funcCode, int address, int start, int Count)
        {
            byte[] message = new byte[8];
            byte[] CRC = new byte[2];
            message[0] = (byte)address;
            message[1] = (byte)0x06;        //写寄存器
            message[2] = (byte)(start >> 8);
            message[3] = (byte)(start & 0xFF);
            GetCRC(message, message.Length, ref CRC);
            message[message.Length - 2] = CRC[0];
            message[message.Length - 1] = CRC[1];
            return message;
        }
    }
}
