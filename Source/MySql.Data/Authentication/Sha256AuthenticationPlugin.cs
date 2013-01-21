// Copyright © 2012, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.Common;
using MySql.Data.MySqlClient.Properties;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;

namespace MySql.Data.MySqlClient.Authentication
{
  /// <summary>
  /// The implementation of the sha256_password authentication plugin.
  /// </summary>
  public class Sha256AuthenticationPlugin : MySqlAuthenticationPlugin
  {
    private bool hasPublicKey = false;

    private AsymmetricCipherKeyPair publicKey;

    public override string PluginName
    {
      get { return "sha256_password"; }
    }

    protected override byte[] MoreData(byte[] data)
    {
      byte[] passBytes = GetPassword() as byte[];
      byte[] buffer = new byte[ passBytes.Length + 1];
      Array.Copy(passBytes, 0, buffer, 0, passBytes.Length );
      buffer[passBytes.Length] = 0;
      return buffer;
    }

    public override object GetPassword()
    {
#if !CF
      if (Settings.SslMode != MySqlSslMode.None)
      {
        // send as clear text, since the channel is already encrypted
        return Encoding.Default.GetBytes(Settings.Password);
      }
      else
      {
#endif
        // send RSA encrypted, since the channel is not protected
        if (!hasPublicKey) RequestPublicKey();
        byte[] bytes = GetRsaPassword(Settings.Password, AuthenticationData);
        if (bytes != null && bytes.Length == 1 && bytes[0] == 0) return null;
        return bytes;
#if !CF
      }
#endif
    }

    private void RequestPublicKey()
    {
      // send 0x01 packet, get the public key in PEM format (which is not the same than salted seed).
      SendData(new byte[] { 0x01 });
      byte[] rawPubkey = ReadData();
      AsymmetricCipherKeyPair keys = GenerateKeysFromPem( rawPubkey );
      publicKey = keys;
      hasPublicKey = true;
    }

    private AsymmetricCipherKeyPair GenerateKeysFromPem( byte[] rawData )
    {
      PemReader pem = new PemReader(new StreamReader(new MemoryStream( rawData )));
      AsymmetricCipherKeyPair keyPair = (AsymmetricCipherKeyPair)pem.ReadObject();
      return keyPair;
    }

    private byte[] GetRsaPassword(string password, byte[] seedBytes)
    {
      // Obfuscate the plain text password with the session scramble
      byte[] ofuscated = GetXor(Encoding.Default.GetBytes(password), seedBytes);
      // Encrypt the password and send it to the server
      byte[] result = Encrypt(ofuscated, publicKey.Public);
      return result;
    }

    private byte[] GetXor( byte[] src, byte[] pattern )
    {
      byte[] result = new byte[src.Length];
      for (int i = 0; i < src.Length; i++)
      {
        result[ i ] = ( byte )( src[ i ] ^ ( pattern[ i % pattern.Length ] ));
      }
      return result;
    }

    private byte[] Encrypt(byte[] data, AsymmetricKeyParameter key)
    {
      RsaEngine e = new RsaEngine();
      e.Init(true, key);
      int bsize = e.GetInputBlockSize();
      List<byte> output = new List<byte>();
      for (int i = 0; i < data.Length; i += bsize)
      {
        int chunkSize = Math.Min(bsize, data.Length - (i * bsize));
        output.AddRange(e.ProcessBlock(data, i, chunkSize));
      }
      return output.ToArray();
    }
  }
}
