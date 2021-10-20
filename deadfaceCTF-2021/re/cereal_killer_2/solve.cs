using System;
using System.Security.Cryptography;
using System.Text;

// Token: 0x02000002 RID: 2
public static class SymmetricEncryptor
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000450
	public static void Main(string[] args)
	{
		Console.WriteLine(DecryptFromBase64ToString("hlS4MbOmA+kQX71xXwPs7CsCWp9jQxCPa/oMk2o2bZr+jgweD4b8u80z5LVoBqC7"));
	}

	// Token: 0x06000002 RID: 2 RVA: 0x0000205C File Offset: 0x0000045C
	public static byte[] EncryptString(string toEncrypt)
	{
		byte[] array = new byte[]
		{
			5,
			18,
			61,
			44,
			125,
			34,
			247,
			90,
			155,
			149,
			103,
			142,
			219,
			199,
			5,
			231
		};
		byte[] result;
		using (Aes aes = Aes.Create())
		{
			using (ICryptoTransform cryptoTransform = aes.CreateEncryptor(array, array))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
				result = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
			}
		}
		return result;
	}

	// Token: 0x06000003 RID: 3 RVA: 0x000020E4 File Offset: 0x000004E4
	public static string DecryptFromBase64ToString(string base64Encrypted)
	{
		byte[] encryptedData = Convert.FromBase64String(base64Encrypted);
		return SymmetricEncryptor.DecryptToString(encryptedData);
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00002100 File Offset: 0x00000500
	public static string DecryptToString(byte[] encryptedData)
	{
		byte[] array = new byte[]
		{
			5,
			18,
			61,
			44,
			125,
			34,
			247,
			90,
			155,
			149,
			103,
			142,
			219,
			199,
			5,
			231
		};
		string @string;
		using (Aes aes = Aes.Create())
		{
			using (ICryptoTransform cryptoTransform = aes.CreateDecryptor(array, array))
			{
				byte[] bytes = cryptoTransform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
				@string = Encoding.UTF8.GetString(bytes);
			}
		}
		return @string;
	}

	// Token: 0x06000005 RID: 5 RVA: 0x00002188 File Offset: 0x00000588
	private static byte[] GetKey(string password)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(password);
		byte[] result;
		using (MD5 md = MD5.Create())
		{
			result = md.ComputeHash(bytes);
		}
		return result;
	}

	// Token: 0x04000001 RID: 1
	private static string password = "f1ag{you-didnt-think-it-was-that-easy-did-you}";
}