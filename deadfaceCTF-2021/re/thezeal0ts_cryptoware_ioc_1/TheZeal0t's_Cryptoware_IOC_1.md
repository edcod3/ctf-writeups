# Writeup: TheZeal0t's Cryptoware IOC 1

## Challenge Description

> The Zeal0t's cryptoware has a particular network signature that can be used as an "Indicator of Compromise" (IOC). \
> This indicator is unique to the cryptoware, so it can be used to indicate that a system has been infected by the cryptoware, or the cryptoware attempted to infect it. \
> Enter the IOC as the flag: Example __flag{EXACT-IOC-STRING}__

\
My approach to this challenge (during the ctf) was running the binary in a VM (dynamic analysis). \
__Disclaimer: Do not run untrusted binaries on your host machine!__

After the CTF I found the IOC string by reversing the binary (static analysis). \
\
_The IOC string I found was a url used to fetch the AES key. \
However, there might be other IOCs embedded in the binary._

## Solution: Dynamic Analysis


### Approach 1: Monitoring network traffic with `Wireshark`

We can monitor the network traffic during the execution with `Wireshark` and capture any requests made by the binary.

Running the binary while monitoring the network traffic captures a request made by the binary.

```
GET /zealotcrypt-aes-key.txt HTTP/1.1
Host: insidious.deadface.io
User-Agent: DEADFACE_LLABS_CRYPTOWARE/6.69
Accept-Encoding: gzip

HTTP/1.1 200 OK
Date: Wed, 20 Oct 2021 21:24:15 GMT
Server: Apache/2.4.49 (Unix)
Last-Modified: Sun, 03 Oct 2021 01:04:29 GMT
ETag: "11-5cd6860e8d4e0"
Accept-Ranges: bytes
Content-Length: 17
Content-Type: text/plain

scoobiedoobiedoo
```

The url would be the following `http://insidious.deadface.io/zealotcrypt-aes-key.txt`

### Approach 2: Disabling internet connection

An even easier solution is to disable the internet connection in the VM & run the binary.\
\
This will result in the web request failing, making Go panic & printing out the url in the error message.

```bash
$ ./encryptor
panic: Get "http://insidious.deadface.io/zealotcrypt-aes-key.txt": dial tcp: lookup insidious.deadface.io on 169.169.13.37:53: dial udp 169.169.13.37:53: connect: network is unreachable

goroutine 1 [running]:
main.fetchKey({0x9880240, 0x34})
        /home/dsewell/Dev/go/src/github.com/docsewell/go-ransom/zealotcrypt-02.go:90 +0x354
main.main()
        /home/dsewell/Dev/go/src/github.com/docsewell/go-ransom/zealotcrypt-02.go:138 +0x135
```

## Solution: Static Analysis

When opening the binary in [Ghidra](https://ghidra-sre.org/) & locating the `main.main` function, there is a reference to a global variable `DAT_0828abce` on line 58.

```c
local_108 = &DAT_0828abce;
```

The variable contains a hex string, which can be decoded with [CyberChef](https://gchq.github.io/CyberChef/).

Copying the hex values from Ghidra & pasting them into Cyberchef with the Recipe `From Hex > From Hex > Magic (Intensive mode enabled)` gives us the url.

[CyberChef Recipe](https://gchq.github.io/CyberChef/#recipe=From_Hex('Space')From_Hex('Auto')Magic(3,true,false,'')&input=MzMgMzIgMzIgNjUgMzIgNjUgMzIgNjEgMzYgMzAgMzcgMzUgMzcgMzUgMzMgMzMgMzMgMzQgMzIgMzkgMzMgMzMgMzMgNjUgMzMgMzMgMzMgMzUgMzIgNjYgMzIgMzkgMzcgMzQgMzMgNjUgMzMgNjYgMzMgNjIgMzMgNjUgMzMgNjMgMzMgNjIgMzMgMzkgMzMgNjYgMzcgMzQgMzMgMzMgMzMgMzUgMzcgMzUgMzIgMzAgMzMgNjYgMzMgNjIgMzMgMzYgMzMgMzUgMzIgNjUgMzMgMzkgMzIgMzggMzIgMzMgMzIgNjEgMzIgNjUgMzcgMzcgMzMgNjIgMzMgNjYgMzIgMzkgMzcgMzcgMzMgMzEgMzMgNjYgMzIgMzMgMzcgMzQgMzIgNjUgMzIgMzIgMzIgNjU)


### Now wrap the url in flag{_url_} to get the flag.

## Flag: flag{http://insidious.deadface.io/zealotcrypt-aes-key.txt}