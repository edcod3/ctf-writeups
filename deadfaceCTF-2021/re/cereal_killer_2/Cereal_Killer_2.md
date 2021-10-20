# Writeup: Cereal Killer 2

## Challenge Description

> mort1cia also loves spooky, sugary cereals. She isn't scared of hyperactivity or tooth decay! \
> Download the program and decrypt the output to find out what her favorite cereal is. \
> Enter the answer as  flag __{here-is-the-answer}__.

## Solution

### 1. Analyze the binary
  
Running the `file` command on the binary tells us that the binary is a __.NET__ executable.

```text
re02.exe: PE32 executable (console) Intel 80386 Mono/.Net assembly, for MS Windows
```

Dissassembling .NET binary is trivial with the help of tools like [ILSpy](https://github.com/icsharpcode/ILSpy) or [dnSpy](https://github.com/dnSpy/dnSpy).

### 2. Decompilation with `dnSpy`

Opening the binary in `dnSpy` shows us the C# source code.

dnSPy Assembly Explorer: \
![dnSPy Assembly Explorer](https://raw.githubusercontent.com/edcod3/ctf-writeups/master/deadfaceCTF-2021/re/cereal_killer_2/assembly-explorer.png "dnSPy Assembly Explorer")

SymmetricEncryptor class source code:
```csharp
using System;
using System.Security.Cryptography;
using System.Text;

// Token: 0x02000002 RID: 2
public static class SymmetricEncryptor
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000450
	public static void Main(string[] args)
	{
		Console.WriteLine("hlS4MbOmA+kQX71xXwPs7CsCWp9jQxCPa/oMk2o2bZr+jgweD4b8u80z5LVoBqC7");
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
```

The password sadly isn't the flag because the flag format is incorrect :( \
\
The code of `Main` method only prints out a base64-encoded string which can be assumed is the encrypted flag. \
\
Luckily we can just use the decrypt method `DecryptFromBase64ToString` to decrypt the string ourselves.


### 3. Decrypting the Ciphertext

We can rewrite the `Main` method so that the program prints out the flag.

Rewrite the following line in `Main` from:

```csharp
Console.WriteLine("hlS4MbOmA+kQX71xXwPs7CsCWp9jQxCPa/oMk2o2bZr+jgweD4b8u80z5LVoBqC7");
```

To: 

```csharp
Console.WriteLine(DecryptFromBase64ToString("hlS4MbOmA+kQX71xXwPs7CsCWp9jQxCPa/oMk2o2bZr+jgweD4b8u80z5LVoBqC7"));
```

Now we can compile the C# code and get the flag. \
I used a [C# Online Compiler](https://dotnetfiddle.net/) to compile & run the code. \
\
Running the code will give us the flag:

```
flag{frank3n-berry-goodness-NOM-NOM-NOM}
```

## Flag: flag{frank3n-berry-goodness-NOM-NOM-NOM}