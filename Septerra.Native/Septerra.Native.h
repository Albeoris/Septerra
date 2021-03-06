#pragma once

#define LOBYTE(x)  (*(unsigned char *) &(x))

using uint8 = unsigned __int8;

using namespace System;
using namespace System::IO;

namespace SepterraNative {
	public ref class Decompressor
	{
	public:
		
		int __clrcall ReadCompressedFile(int compressedSize, uint8* outputBuffer, Stream^ inputStream)
		{
		  LeftCompressedSize = compressedSize;
		  InputStream = inputStream;

		  int decompressedSize = 0;
		  int readedCompressed = ReadCompressedData();

		  pin_ptr<uint8> compressedBufferPtr = &CompressedBuffer[0];
		  DecompressBuffer(compressedBufferPtr, readedCompressed, outputBuffer, &decompressedSize/*, (int (*)(void))ReadCompressedData*/);
		  
		  return decompressedSize;
		}

	private :

		int LeftCompressedSize;
		Stream^ InputStream;
		array< uint8>^ CompressedBuffer = gcnew array< uint8 >(4000);  

		int ReadCompressedData()
		{
			int desiredSize = LeftCompressedSize;
			if (desiredSize > CompressedBuffer->Length)
				desiredSize = CompressedBuffer->Length;
			
			LeftCompressedSize -= desiredSize;

			if (desiredSize > 0)
			{
				int readSize = InputStream -> Read(CompressedBuffer, 0, desiredSize);
				if (readSize != desiredSize)
					throw gcnew Exception(String::Format("Failed to read {0} bytes from the input stream. Readed: {1}", desiredSize, readSize));
			}
			
			return desiredSize;
		}

		int __clrcall DecompressBuffer(uint8 *compressedDataPtr, int compressedDataSize, uint8 *outputBuffer, signed __int32 *decompressedSize/*, int (*readNextCompressedData)(void)*/)
		{
		  uint8 *v5; // ebp
		  uint8 *v6; // edi
		  uint8 *v7; // eax
		  uint8 v8; // cl
		  uint8 *v9; // esi
		  uint8 *v10; // esi
		  int v11; // ecx
		  //int (*v12)(void); // ebx
		  unsigned int v13; // ecx
		  uint8 *v14; // esi
		  unsigned int v15; // ebp
		  int v16; // ecx
		  uint8 *v17; // esi
		  uint8 *v18; // esi
		  unsigned int v19; // edx
		  unsigned int v20; // ebp
		  int v21; // edx
		  uint8 *v22; // ecx
		  int v23; // eax
		  uint8 *v24; // ecx
		  char v25; // bp
		  int v26; // ebp
		  unsigned int v27; // edx
		  int v28; // ebx
		  uint8 *v29; // ecx
		  unsigned int v30; // ebp
		  uint8 *v31; // ecx
		  uint8 *v32; // ecx
		  uint8 *v33; // ebx
		  int v34; // ecx
		  unsigned int v35; // ebx
		  uint8 *v36; // esi
		  int v37; // edx
		  uint8 *v38; // ebx
		  uint8 *v39; // ecx
		  unsigned int v40; // ebp
		  int v41; // ebx
		  int v42; // ecx
		  unsigned int v43; // ebx
		  uint8 *v44; // esi
		  int v45; // ebx
		  uint8 *v46; // ebx
		  char v48; // [esp+10h] [ebp-8h]
		  int v49; // [esp+14h] [ebp-4h]
		  uint8 *i; // [esp+1Ch] [ebp+4h]
		  uint8 *v51; // [esp+1Ch] [ebp+4h]
		  uint8 *v52; // [esp+1Ch] [ebp+4h]
		 
		  v5 = outputBuffer;
		  v6 = compressedDataPtr;
		  *decompressedSize = 0;
		  v7 = &compressedDataPtr[compressedDataSize];
		  v8 = *compressedDataPtr;
		  i = outputBuffer;
		  v9 = v6;
		  if ( v8 > 0x11u )
		  {
		    v10 = v6 + 1;
		    v11 = v8 - 17;
		    do
		    {
		      *v5++ = *v10++;
		      --v11;
		    }
		    while ( v11 );
		    //v12 = readNextCompressedData;
		    i = v5;
		    goto LABEL_25;
		  }
		LABEL_5:
		  v13 = *v9;
		  v14 = v9 + 1;
		  v15 = v13;
		  if ( v14 < v7 )
		  {
		    //v12 = readNextCompressedData;
		  }
		  else
		  {
		    //v12 = readNextCompressedData;
		    LOBYTE(compressedDataSize) = *(v14 - 2);
		    v48 = *(v14 - 1);
		    v14 = v6;
		    v7 = &v6[ReadCompressedData()];
		  }
		  if ( v15 < 0x10 )
		  {
		    if ( !v15 )
		    {
		      while ( !*v14 )
		      {
		        v15 += 255;
		        if ( ++v14 >= v7 )
		        {
		          LOBYTE(compressedDataSize) = *(v14 - 2);
		          v48 = *(v14 - 1);
		          v14 = v6;
		          v7 = &v6[ReadCompressedData()];
		        }
		      }
		      v16 = *v14++;
		      v15 += v16 + 15;
		      if ( v14 >= v7 )
		      {
		        LOBYTE(compressedDataSize) = *(v14 - 2);
		        v48 = *(v14 - 1);
		        v14 = v6;
		        v7 = &v6[ReadCompressedData()];
		      }
		    }
		    *i = *v14;
		    v17 = v14 + 1;
		    v51 = i + 1;
		    if ( v17 >= v7 )
		    {
		      LOBYTE(compressedDataSize) = *(v17 - 2);
		      v48 = *(v17 - 1);
		      v17 = v6;
		      v7 = &v6[ReadCompressedData()];
		    }
		    *v51 = *v17;
		    v18 = v17 + 1;
		    v52 = v51 + 1;
		    if ( v18 >= v7 )
		    {
		      LOBYTE(compressedDataSize) = *(v18 - 2);
		      v48 = *(v18 - 1);
		      v18 = v6;
		      v7 = &v6[ReadCompressedData()];
		    }
		    *v52 = *v18;
		    v10 = v18 + 1;
		    i = v52 + 1;
		    if ( v10 >= v7 )
		    {
		      LOBYTE(compressedDataSize) = *(v10 - 2);
		      v48 = *(v10 - 1);
		      v10 = v6;
		      v7 = &v6[ReadCompressedData()];
		    }
		    do
		    {
		      *i++ = *v10++;
		      if ( v10 >= v7 )
		      {
		        LOBYTE(compressedDataSize) = *(v10 - 2);
		        v48 = *(v10 - 1);
		        v10 = v6;
		        v7 = &v6[ReadCompressedData()];
		      }
		      --v15;
		    }
		    while ( v15 );
		LABEL_25:
		    v19 = *v10;
		    v14 = v10 + 1;
		    v15 = v19;
		    if ( v14 >= v7 )
		    {
		      LOBYTE(compressedDataSize) = *(v14 - 2);
		      v48 = *(v14 - 1);
		      v14 = v6;
		      v7 = &v6[ReadCompressedData()];
		    }
		    if ( v15 < 0x10 )
		    {
		      v20 = v15 >> 2;
		      v21 = 4 * *v14;
		      v22 = &i[-(int)v20 - (int)v21 - 2049];
		      v9 = v14 + 1;
		      v49 = (int)&i[-(int)v20 - (int)v21 - 2049];
		      if ( v9 >= v7 )
		      {
		        LOBYTE(compressedDataSize) = *(v9 - 2);
		        v48 = *(v9 - 1);
		        v9 = v6;
		        v23 = ReadCompressedData();
		        v22 = (uint8 *)v49;
		        v7 = &v6[v23];
		      }
		      *i = *v22;
		      v24 = v22 + 1;
		      i[1] = *v24;
		      i[2] = v24[1];
		      i += 3;
		      goto LABEL_32;
		    }
		  }
		  while ( 1 )
		  {
		    if ( v15 >= 0x40 )
		    {
		      v28 = (int)&i[-(int)((v15 >> 2) & 7) - 1 + -8 * *v14];
		      v9 = v14 + 1;
		      if ( v9 >= v7 )
		      {
		        LOBYTE(compressedDataSize) = *(v9 - 2);
		        v48 = *(v9 - 1);
		        v9 = v6;
		        v7 = &v6[ReadCompressedData()];
		      }
		      v29 = i;
		      v30 = (v15 >> 5) - 1;
		      goto LABEL_47;
		    }
		    if ( v15 < 0x20 )
		      break;
		    v30 = v15 & 0x1F;
		    if ( !v30 )
		    {
		      while ( !*v14 )
		      {
		        v30 += 255;
		        if ( ++v14 >= v7 )
		        {
		          LOBYTE(compressedDataSize) = *(v14 - 2);
		          v48 = *(v14 - 1);
		          v14 = v6;
		          v7 = &v6[ReadCompressedData()];
		        }
		      }
		      v34 = *v14++;
		      v30 += v34 + 31;
		      if ( v14 >= v7 )
		      {
		        LOBYTE(compressedDataSize) = *(v14 - 2);
		        v48 = *(v14 - 1);
		        v14 = v6;
		        v7 = &v6[ReadCompressedData()];
		      }
		    }

			v35 = (unsigned int)&i[-((unsigned int)*v14 >> 2) - 1];
		    v36 = v14 + 1;
		    if ( v36 >= v7 )
		    {
		      LOBYTE(compressedDataSize) = *(v36 - 2);
		      v48 = *(v36 - 1);
		      v36 = v6;
		      v7 = &v6[ReadCompressedData()];
		    }
		    v28 = v35 - (*v36 << 6);
		    v9 = v36 + 1;
		    if ( v9 >= v7 )
		    {
		      LOBYTE(compressedDataSize) = *(v9 - 2);
		      v48 = *(v9 - 1);
		      v9 = v6;
		      v7 = &v6[ReadCompressedData()];
		    }
		    v29 = i;
		LABEL_63:
		    if ( v30 >= 6 && (signed int)&v29[-v28] >= 4 )
		    {
		      v37 = *(signed int *)v28;
		      v38 = (uint8 *)(v28 + 4);
		      *(signed int *)v29 = v37;
		      v39 = v29 + 4;
		      v40 = v30 - 2;
		      do
		      {
		        v40 -= 4;
		        *(signed int *)v39 = *(signed int *)v38;
		        v39 += 4;
		        v38 += 4;
		      }
		      while ( v40 >= 4 );
		      for ( i = v39; v40; ++i )
		      {
		        *i = *v38++;
		        --v40;
		      }
		      goto LABEL_31;
		    }
		LABEL_47:
		    *v29 = *(uint8 *)v28;
		    v31 = v29 + 1;
		    *v31 = *(uint8 *)(v28 + 1);
		    v32 = v31 + 1;
		    v33 = (uint8 *)(v28 + 2);
		    do
		    {
		      *v32++ = *v33++;
		      --v30;
		    }
		    while ( v30 );
		    i = v32;
		LABEL_31:
		    //v12 = readNextCompressedData;
		LABEL_32:
		    if ( v9 == v6 )
		    {
		      v25 = compressedDataSize;
		    }
		    else if ( v9 == v6 + 1 )
		    {
		      v25 = v48;
		    }
		    else
		    {
		      v25 = *(v9 - 2);
		    }
		    v26 = v25 & 3;
		    if ( !v26 )
		      goto LABEL_5;
		    do
		    {
		      *i++ = *v9++;
		      if ( v9 >= v7 )
		      {
		        LOBYTE(compressedDataSize) = *(v9 - 2);
		        v48 = *(v9 - 1);
		        v9 = v6;
		        v7 = &v6[ReadCompressedData()];
		      }
		      --v26;
		    }
		    while ( v26 );
		    v27 = *v9;
		    v14 = v9 + 1;
		    v15 = v27;
		    if ( v14 >= v7 )
		    {
		      LOBYTE(compressedDataSize) = *(v14 - 2);
		      v48 = *(v14 - 1);
		      v14 = v6;
		      v7 = &v6[ReadCompressedData()];
		    }
		  }
		  if ( v15 < 0x10 )
		  {
			  int offsetBack = -(v15 >> 2);
			  offsetBack -= 1;
			  offsetBack -= 4 * (*v14);
		    v46 = &i[offsetBack];
		    v9 = v14 + 1;
		    if ( v9 >= v7 )
		    {
		      LOBYTE(compressedDataSize) = *(v9 - 2);
		      v48 = *(v9 - 1);
		      v9 = v6;
		      v7 = &v6[ReadCompressedData()];
		    }
		    *i = *v46;
		    i[1] = v46[1];
		    i += 2;
		    goto LABEL_31;
		  }
		  v41 = (int)&i[-2048 * (v15 & 8)];
		  v30 = v15 & 7;
		  if ( !v30 )
		  {
		    while ( !*v14 )
		    {
		      v30 += 255;
		      if ( ++v14 >= v7 )
		      {
		        LOBYTE(compressedDataSize) = *(v14 - 2);
		        v48 = *(v14 - 1);
		        v14 = v6;
		        v7 = &v6[ReadCompressedData()];
		      }
		    }
		    v42 = *v14++;
		    v30 += v42 + 7;
		    if ( v14 >= v7 )
		    {
		      LOBYTE(compressedDataSize) = *(v14 - 2);
		      v48 = *(v14 - 1);
		      v14 = v6;
		      v7 = &v6[ReadCompressedData()];
		    }
		  }
		  v43 = v41 - ((unsigned int)*v14 >> 2);
		  v44 = v14 + 1;
		  if ( v44 >= v7 )
		  {
		    LOBYTE(compressedDataSize) = *(v44 - 2);
		    v48 = *(v44 - 1);
		    v44 = v6;
		    v7 = &v6[ReadCompressedData()];
		  }
		  v45 = v43 - (*v44 << 6);
		  v9 = v44 + 1;
		  if ( v9 >= v7 )
		  {
		    LOBYTE(compressedDataSize) = *(v9 - 2);
		    v48 = *(v9 - 1);
		    v9 = v6;
		    v7 = &v6[ReadCompressedData()];
		  }
		  v29 = i;
		  if ( (uint8 *)v45 != i )
		  {
		    v28 = v45 - 0x4000;
		    goto LABEL_63;
		  }
		  *decompressedSize = i - outputBuffer;
		  return 0;
		}
	};
}
