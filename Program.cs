using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        string fileName = "file.txt";
        string text = "Sasha borov sapyr @4 kaban";
        byte[] key = Enumerable.Range(0, 32).Select(x => (byte)x).ToArray(); // массив [ 0, 1, 2, ..., 31 ], для примера
        File.WriteAllText(fileName, text);
        Console.WriteLine("Файл создан. Нажми любую клавишу.");
        Console.ReadKey(true);
        EncryptFile(fileName, key);
        Console.WriteLine("Файл зашифрован. Нажми любую клавишу.");
        Console.ReadKey(true);
        DecryptFile(fileName, key);
        Console.WriteLine("Файл расшифрован.");
        Console.WriteLine(File.ReadAllText(fileName));
        Console.ReadKey(true);
    }

    private static void EncryptFile(string path, byte[] key)
    {
        string tmpPath = Path.GetTempFileName();
        using (FileStream fsSrc = File.OpenRead(path))
        using (AesManaged aes = new AesManaged() { Key = key })
        using (FileStream fsDst = File.Create(tmpPath))
        {
            fsDst.Write(aes.IV);
            using (CryptoStream cs = new CryptoStream(fsDst, aes.CreateEncryptor(), CryptoStreamMode.Write, true))
            {
                fsSrc.CopyTo(cs);
            }
        }
        File.Delete(path);
        File.Move(tmpPath, path);
    }

    private static void DecryptFile(string path, byte[] key)
    {
        string tmpPath = Path.GetTempFileName();
        using (FileStream fsSrc = File.OpenRead(path))
        {
            byte[] iv = new byte[16];
            fsSrc.Read(iv);
            using (AesManaged aes = new AesManaged() { Key = key, IV = iv })
            using (CryptoStream cs = new CryptoStream(fsSrc, aes.CreateDecryptor(), CryptoStreamMode.Read, true))
            using (FileStream fsDst = File.Create(tmpPath))
            {
                cs.CopyTo(fsDst);
            }
        }
        File.Delete(path);
        File.Move(tmpPath, path);
    }
}
