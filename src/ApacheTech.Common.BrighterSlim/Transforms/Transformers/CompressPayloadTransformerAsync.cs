﻿using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;


/* Unmerged change from project 'ApacheTech.Common.BrighterSlim (net6.0)'
Before:
namespace Paramore.BrighterSlim.Transforms.Transformers
After:
namespace ApacheTech.BrighterSlim.Transforms.Transformers
*/

/* Unmerged change from project 'ApacheTech.Common.BrighterSlim (net7.0)'
Before:
namespace Paramore.BrighterSlim.Transforms.Transformers
After:
namespace ApacheTech.BrighterSlim.Transforms.Transformers
*/

/* Unmerged change from project 'ApacheTech.Common.BrighterSlim (net8.0)'
Before:
namespace Paramore.BrighterSlim.Transforms.Transformers
After:
namespace ApacheTech.BrighterSlim.Transforms.Transformers
*/
namespace ApacheTech.Common.BrighterSlim.Transforms.Transformers
{
    public class CompressPayloadTransformerAsync : IAmAMessageTransformAsync
    {
        private CompressionMethod _compressionMethod = CompressionMethod.GZip;
        private CompressionLevel _compressionLevel = CompressionLevel.Optimal;
        private int _thresholdInBytes;
        private const ushort GZIP_LEAD_BYTES = 0x8b1f;
        private const byte ZLIB_LEAD_BYTE = 0x78;

        /// <summary>Compression method GZip</summary>
        public const string GZIP = "application/gzip";
        /// <summary> Compression method Deflate</summary>
        public const string DEFLATE = "application/deflate";
        /// <summary> Compression method Brotli</summary>
        public const string BROTLI = "application/br";

        /// <summary> Original content type header name</summary>
        public const string ORIGINAL_CONTENTTYPE_HEADER = "originalContentType";

        public void Dispose() { }

        public void InitializeWrapFromAttributeParams(params object[] initializerList)
        {
            _compressionMethod = (CompressionMethod)initializerList[0];
            _compressionLevel = (CompressionLevel)initializerList[1];
            _thresholdInBytes = (int)initializerList[2] * 1024;

        }

        public void InitializeUnwrapFromAttributeParams(params object[] initializerList)
        {
            _compressionMethod = (CompressionMethod)initializerList[0];
        }

        public async Task<Message> WrapAsync(Message message, CancellationToken cancellationToken = default)
        {
            var bytes = message.Body.Bytes;

            //don't transform it too small
            if (bytes.Length < _thresholdInBytes)
                return message;

            using var input = new MemoryStream(bytes);
            using var output = new MemoryStream();

            (Stream compressionStream, string mimeType) = CreateCompressionStream(output);
            await input.CopyToAsync(compressionStream);
            compressionStream.Close();

            message.Header.ContentType = mimeType;
            message.Header.Bag.Add(ORIGINAL_CONTENTTYPE_HEADER, message.Body.ContentType);
            message.Body = new MessageBody(output.ToArray(), mimeType, CharacterEncoding.Raw);

            return message;
        }


        public async Task<Message> UnwrapAsync(Message message, CancellationToken cancellationToken = default)
        {
            if (!IsCompressed(message))
                return message;

            var bytes = message.Body.Bytes;
            using var input = new MemoryStream(bytes);
            using var output = new MemoryStream();

            Stream deCompressionStream = CreateDecompressionStream(input);
            await deCompressionStream.CopyToAsync(output);
            deCompressionStream.Close();

            string contentType = (string)message.Header.Bag[ORIGINAL_CONTENTTYPE_HEADER];
            message.Body = new MessageBody(output.ToArray(), contentType, CharacterEncoding.UTF8);
            message.Header.ContentType = contentType;

            return message;
        }

        private (Stream, string) CreateCompressionStream(MemoryStream uncompressed)
        {
            switch (_compressionMethod)
            {
                case CompressionMethod.GZip:
                    return (new GZipStream(uncompressed, _compressionLevel), GZIP);
#if NETSTANDARD2_0
                case CompressionMethod.Zlib:
                    throw new ArgumentException("Zlib is not supported in nestandard20");
                case CompressionMethod.Brotli:
                    throw new ArgumentException("Brotli is not supported in nestandard20");
#else
                case CompressionMethod.Zlib:
                    return (new ZLibStream(uncompressed, _compressionLevel), DEFLATE);
                case CompressionMethod.Brotli:
                    return (new BrotliStream(uncompressed, _compressionLevel), BROTLI);
#endif
                default:
                    return (uncompressed, "application/json");
            }
        }

        private Stream CreateDecompressionStream(MemoryStream compressed)
        {
            switch (_compressionMethod)
            {
                case CompressionMethod.GZip:
                    return new GZipStream(compressed, CompressionMode.Decompress);

#if NETSTANDARD2_0
                case CompressionMethod.Zlib:
                    throw new ArgumentException("Zlib is not supported in nestandard20");
                case CompressionMethod.Brotli:
                    throw new ArgumentException("Brotli is not supported in nestandard20");
#else
                case CompressionMethod.Zlib:
                    return new ZLibStream(compressed, CompressionMode.Decompress);
                case CompressionMethod.Brotli:
                    return new BrotliStream(compressed, CompressionMode.Decompress);
#endif
                default:
                    return compressed;
            }
        }

        private bool IsCompressed(Message message)
        {
            switch (_compressionMethod)
            {
                case CompressionMethod.GZip:
                    return message.Header.ContentType == "application/gzip" && message.Body.Bytes.Length >= 2 && BitConverter.ToUInt16(message.Body.Bytes, 0) == GZIP_LEAD_BYTES;
                case CompressionMethod.Zlib:
                    return message.Header.ContentType == "application/deflate" && message.Body.Bytes[0] == ZLIB_LEAD_BYTE;
                case CompressionMethod.Brotli:
                    return message.Header.ContentType == "application/br";
                default:
                    return false;

            }
        }

    }
}
