using System;
using System.Text;
using System.Security.Cryptography;  

class Hash1 {
  public byte[] test1(string data) {
    MD5 hash = MD5.Create();
    hash.Initialize();
    return hash.ComputeHash(Encoding.UTF8.GetBytes(data));
  }

  public byte[] test2(string data) {
    SHA256 hash = SHA256.Create();
    hash.Initialize();
    return hash.ComputeHash(Encoding.UTF8.GetBytes(data));
  }
}
