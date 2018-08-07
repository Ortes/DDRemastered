using System;
using System.Text;

namespace DDRemastered_Server
{
    class PacketMaker
    {
        static private byte[] MakePacket(int id, int size)
        {
            byte[] res = new byte[sizeof(int) + size];
            BitConverter.GetBytes(id).CopyTo(res, 0);
            return res;
        }

        static public byte[] MakeReady()
        {
            byte[] res = MakePacket(-1, 0);
            return res;
        }

        static public byte[] MakeInit(int id, String name)
        {
            byte[] res = MakePacket(id, sizeof(int) + name.Length);
            BitConverter.GetBytes(name.Length).CopyTo(res, sizeof(int));
            Encoding.ASCII.GetBytes(name).CopyTo(res, sizeof(int) * 2);
            return res;
        }

        static public byte[] MakeDestroy(int id)
        {
            byte[] res = MakePacket(id, sizeof(byte));
            res[sizeof(int)] = 0xFF;
            return res;
        }

        static public byte[] MakeChCharacter(int id, byte character)
        {
            byte[] res = new byte[sizeof(int) + sizeof(byte)];
            BitConverter.GetBytes(id).CopyTo(res, 0);
            res[sizeof(int)] = character;
            return res;
        }
    }
}
