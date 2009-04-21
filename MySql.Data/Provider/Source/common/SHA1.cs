// Copyright � 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;

namespace MySql.Data.Common
{
    class SHA1Hash
    {
        private const int SHA1_HASH_SIZE = 20;          // Hash size in bytes

		// Constants defined in SHA-1
		private static uint[]  K = new uint[4] {
			0x5A827999, 0x6ED9EBA1,	0x8F1BBCDC,	0xCA62C1D6	};

		private static uint[] sha_const_key = new uint[5] {
			0x67452301, 0xEFCDAB89, 0x98BADCFE, 0x10325476, 0xC3D2E1F0 };

        private ulong   length;                        // Message length in bits
        private uint[]  intermediateHash;              // Message Digest
        private bool    computed;                      // Is the digest computed?
//        private bool    corrupted;                     // Is the message digest corrupted?
        private short   messageBlockIndex;             // Index into message block array
        private byte[]  messageBlock;                  // 512-bit message blocks
 
        public SHA1Hash()
        {
            intermediateHash = new uint[SHA1_HASH_SIZE/4];
            messageBlock = new byte[64];
			Reset();
        }

        public void Reset()
        {
/*#ifndef DBUG_OFF
			if (!context)
				return SHA_NULL;
#endif*/

			length		  = 0;
			messageBlockIndex	  = 0;

			intermediateHash[0] = sha_const_key[0];
			intermediateHash[1] = sha_const_key[1];
			intermediateHash[2] = sha_const_key[2];
			intermediateHash[3] = sha_const_key[3];
			intermediateHash[4] = sha_const_key[4];

			computed = false;
//			corrupted  = false;
        }

		public byte[] ComputeHash(byte[] buffer)
		{
			Reset();
			Input(buffer, 0, buffer.Length);
			return Result();
		}

        public void Input(byte[] buffer, int index, int bufLen)
        {
            if (buffer == null || bufLen == 0) return;

            if (index < 0 || index > buffer.Length - 1)
                throw new ArgumentException("Index must be a value between 0 and buffer.Length-1", "index");
            if (bufLen < 0)
                throw new ArgumentException("Length must be a value > 0", "length");
            if ((bufLen+index) > buffer.Length)
                throw new ArgumentException("Length + index would extend past the end of buffer", "length");

/*#ifndef DBUG_OFF
  // We assume client konows what it is doing in non-debug mode 
  if (!context || !message_array)
    return SHA_NULL;
  if (context->Computed)
    return (context->Corrupted= SHA_STATE_ERROR);
  if (context->Corrupted)
    return context->Corrupted;
#endif*/

            while (bufLen-- > 0)
            {
                messageBlock[messageBlockIndex++] = (byte)(buffer[index++] & 0xFF);
                length  += 8;  /* Length is in bits */

/*#ifndef DBUG_OFF
    
    //  Then we're not debugging we assume we never will get message longer
      //2^64 bits.
    
    if (context->Length == 0)
      return (context->Corrupted= 1);	   // Message is too long 
#endif*/

                if (messageBlockIndex == 64)
                    ProcessMessageBlock();
            }
        }

        private void ProcessMessageBlock()
        {
            uint    temp;           // Temporary word value
            uint[]  W;              // Word sequence
            uint    A, B, C, D, E;  // Word buffers

            W = new uint[80];

            //Initialize the first 16 words in the array W
            for (int t = 0; t < 16; t++)
            {
                int index=t*4;
                W[t] = (uint)messageBlock[index] << 24;
                W[t] |= (uint)messageBlock[index + 1] << 16;
                W[t] |= (uint)messageBlock[index + 2] << 8;
                W[t] |= (uint)messageBlock[index + 3];
            }


            for (int t = 16; t < 80; t++)
            {
                W[t] = CircularShift(1, W[t-3] ^ W[t-8] ^ W[t-14] ^ W[t-16]);
            }

            A = intermediateHash[0];
            B = intermediateHash[1];
            C = intermediateHash[2];
            D = intermediateHash[3];
            E = intermediateHash[4];

            for (int t = 0; t < 20; t++)
            {
                temp= CircularShift(5, A) + ((B & C) | ((~B) & D)) + E + W[t] + K[0];
                E = D;
                D = C;
                C = CircularShift(30, B);
                B = A;
                A = temp;
            }

            for (int t = 20; t < 40; t++)
            {
                temp = CircularShift(5,A) + (B ^ C ^ D) + E + W[t] + K[1];
                E = D;
                D = C;
                C = CircularShift(30,B);
                B = A;
                A = temp;
            }

            for (int t = 40; t < 60; t++)
            {
                temp= (CircularShift(5,A) + ((B & C) | (B & D) | (C & D)) + E + W[t] + K[2]);
                E = D;
                D = C;
                C = CircularShift(30,B);
                B = A;
                A = temp;
            }

            for (int t = 60; t < 80; t++)
            {
                temp = CircularShift(5,A) + (B ^ C ^ D) + E + W[t] + K[3];
                E = D;
                D = C;
                C = CircularShift(30,B);
                B = A;
                A = temp;
            }

            intermediateHash[0] += A;
            intermediateHash[1] += B;
            intermediateHash[2] += C;
            intermediateHash[3] += D;
            intermediateHash[4] += E;

            messageBlockIndex = 0;
        }

        private static uint CircularShift(int bits, uint word)
        {
		    return (((word) << (bits)) | ((word) >> (32-(bits))));
        }

        private void PadMessage()
        {
            /*
            Check to see if the current message block is too small to hold
            the initial padding bits and length.  If so, we will pad the
            block, process it, and then continue padding into a second
            block.
            */

            int i = messageBlockIndex;

            if (i > 55)
            {
                messageBlock[i++] = 0x80;
				Array.Clear(messageBlock, i, 64-i);
                //bzero((char*) &context->Message_Block[i], sizeof(messageBlock[0])*(64-i));
                messageBlockIndex = 64;

                /* This function sets messageBlockIndex to zero	*/
                ProcessMessageBlock();

				Array.Clear(messageBlock, 0, 56);
                //bzero((char*) &context->Message_Block[0], sizeof(messageBlock[0])*56);
                messageBlockIndex = 56;
            }
            else
            {
                messageBlock[i++] = 0x80;
				Array.Clear(messageBlock, i, 56-i);
                //bzero((char*) &messageBlock[i], sizeof(messageBlock[0])*(56-i));
                messageBlockIndex = 56;
            }

            // Store the message length as the last 8 octets
            messageBlock[56] = (byte)(length >> 56);
            messageBlock[57] = (byte)(length >> 48);
            messageBlock[58] = (byte)(length >> 40);
            messageBlock[59] = (byte)(length >> 32);
            messageBlock[60] = (byte)(length >> 24);
            messageBlock[61] = (byte)(length >> 16);
            messageBlock[62] = (byte)(length >> 8);
            messageBlock[63] = (byte)length;

            ProcessMessageBlock();        
        }

        public byte[] Result()
        {
/*#ifndef DBUG_OFF
  if (!context || !Message_Digest)
    return SHA_NULL;

  if (context->Corrupted)
    return context->Corrupted;
#endif*/

            if (!computed)
            {
                PadMessage();

                // message may be sensitive, clear it out
				Array.Clear(messageBlock, 0, 64);
                //bzero((char*) messageBlock,64);
                length   = 0;    /* and clear length  */
                computed = true;
            }

            byte[] messageDigest = new byte[SHA1_HASH_SIZE];
            for (int i = 0; i < SHA1_HASH_SIZE; i++)
                messageDigest[i] = (byte)((intermediateHash[i>>2] >> 8 * ( 3 - ( i & 0x03 ) )));
            return messageDigest;
        }

    }
}
