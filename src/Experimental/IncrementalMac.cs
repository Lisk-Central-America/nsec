using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NSec.Cryptography
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct IncrementalMac
    {
        private readonly IncrementalMacState _state;
        private readonly MacAlgorithm _algorithm;

        public MacAlgorithm Algorithm => _algorithm;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(
            object objA,
            object objB)
        {
            return object.Equals(objA, objB);
        }

        public static byte[] Finalize(
            ref IncrementalMac state)
        {
            if (state._algorithm == null)
            {
                throw Error.InvalidOperation_UninitializedState();
            }

            try
            {
                byte[] mac = new byte[state._algorithm.MacSize];
                state._algorithm.FinalizeCore(ref Unsafe.AsRef(in state._state), mac);
                return mac;
            }
            finally
            {
                Unsafe.AsRef(in state._algorithm) = null;
            }
        }

        public static void Finalize(
            ref IncrementalMac state,
            Span<byte> mac)
        {
            if (state._algorithm == null)
            {
                throw Error.InvalidOperation_UninitializedState();
            }
            if (mac.Length != state._algorithm.MacSize)
            {
                throw Error.Argument_MacLength(nameof(mac), state._algorithm.MacSize.ToString());
            }

            try
            {
                state._algorithm.FinalizeCore(ref Unsafe.AsRef(in state._state), mac);
            }
            finally
            {
                Unsafe.AsRef(in state._algorithm) = null;
            }
        }

        public static bool FinalizeAndTryVerify(
            ref IncrementalMac state,
            ReadOnlySpan<byte> mac)
        {
            if (state._algorithm == null)
            {
                throw Error.InvalidOperation_UninitializedState();
            }

            try
            {
                return mac.Length == state._algorithm.MacSize && state._algorithm.FinalizeAndTryVerifyCore(ref Unsafe.AsRef(in state._state), mac);
            }
            finally
            {
                Unsafe.AsRef(in state._algorithm) = null;
            }
        }

        public static void FinalizeAndVerify(
            ref IncrementalMac state,
            ReadOnlySpan<byte> mac)
        {
            if (state._algorithm == null)
            {
                throw Error.InvalidOperation_UninitializedState();
            }

            try
            {
                if (!(mac.Length == state._algorithm.MacSize && state._algorithm.FinalizeAndTryVerifyCore(ref Unsafe.AsRef(in state._state), mac)))
                {
                    throw Error.Cryptographic_VerificationFailed();
                }
            }
            finally
            {
                Unsafe.AsRef(in state._algorithm) = null;
            }
        }

        public static void Initialize(
            Key key,
            out IncrementalMac state)
        {
            if (key == null)
            {
                throw Error.ArgumentNull_Key(nameof(key));
            }
            if (!(key.Algorithm is MacAlgorithm algorithm))
            {
                throw Error.Argument_MacKey(nameof(key), key.Algorithm.GetType().FullName, typeof(MacAlgorithm).FullName);
            }

            bool success = false;
            try
            {
                state = default;
                algorithm.InitializeCore(key.Handle, algorithm.MacSize, out Unsafe.AsRef(in state._state));
                Unsafe.AsRef(in state._algorithm) = algorithm;
                success = true;
            }
            finally
            {
                if (!success)
                {
                    state = default;
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool ReferenceEquals(
            object objA,
            object objB)
        {
            return object.ReferenceEquals(objA, objB);
        }

        public static void Update(
            ref IncrementalMac state,
            ReadOnlySpan<byte> data)
        {
            if (state._algorithm == null)
            {
                throw Error.InvalidOperation_UninitializedState();
            }

            state._algorithm.UpdateCore(ref Unsafe.AsRef(in state._state), data);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(
            object obj)
        {
            throw Error.NotSupported_Operation();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            throw Error.NotSupported_Operation();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return typeof(IncrementalMac).ToString();
        }
    }
}