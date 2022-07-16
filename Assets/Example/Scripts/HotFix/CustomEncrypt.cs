

namespace NewBi
{
    public class NewBiEncrypt : FuXi.BaseEncrypt
    {
        public override byte[] Encrypt(byte[] sourceBytes)
        {
            return sourceBytes;
        }

        public bool IsEncrypt(byte[] sourceBytes)
        {
            throw new System.NotImplementedException();
        }

        public override byte[] EncryptOffset()
        {
            return null;
        }
        
    }
}

public class CustomEncrypt : FuXi.BaseEncrypt
{
    public override byte[] Encrypt(byte[] sourceBytes)
    {
        throw new System.NotImplementedException();
    }

    public bool IsEncrypt(byte[] sourceBytes)
    {
        throw new System.NotImplementedException();
    }

    public override byte[] EncryptOffset()
    {
        throw new System.NotImplementedException();
    }
}