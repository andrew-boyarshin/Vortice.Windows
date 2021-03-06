// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Vortice.Win32;
using SharpGen.Runtime.Win32;

namespace Vortice.WIC
{
    public partial class IWICStream
    {
        private ComStreamProxy _streamProxy;

        /// <summary>
        /// Initialize stream from file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="access">The <see cref="FileAccess"/> mode.</param>
        public void Initialize(string fileName, FileAccess access)
        {
            DisposeStreamProxy();

            var desiredAccess = access.ToNative();
            InitializeFromFilename(fileName, (int)desiredAccess);
        }

        /// <summary>
        /// Initialize the <see cref="IWICStream"/> from another stream. Access rights are inherited from the underlying stream
        /// </summary>
        /// <param name="comStream"></param>
        public void Initialize(IStream comStream)
        {
            DisposeStreamProxy();
            InitializeFromIStream(comStream);
        }

        /// <summary>
        /// Initialize the <see cref="IWICStream"/> from another stream. Access rights are inherited from the underlying stream
        /// </summary>
        /// <param name="stream">The initialize stream.</param>
        public void Initialize(Stream stream)
        {
            DisposeStreamProxy();
            _streamProxy = new ComStreamProxy(stream);
            InitializeFromIStream(_streamProxy);
        }

        /// <summary>
        /// Initialize the stream from given data.
        /// </summary>
        /// <param name="data">Data to initialize with.</param>
        public unsafe void Initialize(byte[] data)
        {
            DisposeStreamProxy();
            fixed (void* dataPtr = &data[0])
            {
                InitializeFromMemory(new IntPtr(dataPtr), data.Length);
            }
        }

        /// <summary>
        /// Initialize the stream from given data.
        /// </summary>
        /// <param name="data">Data to initialize with.</param>
        public void Initialize<T>(ReadOnlySpan<T> data) where T : unmanaged
        {
            DisposeStreamProxy();
            unsafe
            {
                fixed (void* dataPtr = data)
                {
                    InitializeFromMemory(new IntPtr(dataPtr), data.Length * sizeof(T));
                }
            }
        }

        /// <summary>
        /// Initialize the stream from given data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Data to initialize with.</param>
        public void Initialize<T>(T[] data) where T : unmanaged
        {
            DisposeStreamProxy();
            unsafe
            {
                fixed (void* dataPtr = data)
                {
                    InitializeFromMemory(new IntPtr(dataPtr), data.Length * sizeof(T));
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            DisposeStreamProxy();
        }

        private void DisposeStreamProxy()
        {
            if (_streamProxy != null)
            {
                _streamProxy.Dispose();
                _streamProxy = null;
            }
        }
    }
}
