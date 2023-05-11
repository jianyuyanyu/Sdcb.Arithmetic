using Sdcb.Arithmetic.Gmp;
using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace Sdcb.Arithmetic.Mpfr.Tests
{
    public class GlobalTest
    {
        private readonly ITestOutputHelper _console;

        public GlobalTest(ITestOutputHelper console)
        {
            _console = console;
        }

        [Fact]
        public void RawVersion()
        {
            IntPtr ver = MpfrLib.mpfr_get_version();
            Assert.NotEqual(IntPtr.Zero, ver);

            string? version = Marshal.PtrToStringAnsi(ver);
            Assert.NotNull(version);

            _console.WriteLine(version); // "4.2.0"
        }

        [Fact]
        public void RawPatch()
        {
            IntPtr ver = MpfrLib.mpfr_get_patches();
            Assert.NotEqual(IntPtr.Zero, ver);

            string? patches = Marshal.PtrToStringAnsi(ver);
            Assert.NotNull(patches);

            _console.WriteLine(patches); // ""
        }

        [Fact]
        public unsafe void CalcE()
        {
#pragma warning disable CA1806 // ��Ҫ���Է������
            // https://www.mpfr.org/sample.html
            Mpfr_t s, t, u;
            MpfrLib.mpfr_init2((IntPtr)(&t), 200);
            MpfrLib.mpfr_set_d((IntPtr)(&t), 1.0, MpfrRounding.ToNegativeInfinity);
            MpfrLib.mpfr_init2((IntPtr)(&s), 200);
            MpfrLib.mpfr_set_d((IntPtr)(&s), 1.0, MpfrRounding.ToNegativeInfinity);
            MpfrLib.mpfr_init2((IntPtr)(&u), 200);

            for (uint i = 1; i <= 100; ++i)
            {
                MpfrLib.mpfr_mul_ui((IntPtr)(&t), (IntPtr)(&t), i, MpfrRounding.ToPositiveInfinity);
                MpfrLib.mpfr_set_d((IntPtr)(&u), 1.0, MpfrRounding.ToNegativeInfinity);
                MpfrLib.mpfr_div((IntPtr)(&u), (IntPtr)(&u), (IntPtr)(&t), MpfrRounding.ToNegativeInfinity);
                MpfrLib.mpfr_add((IntPtr)(&s), (IntPtr)(&s), (IntPtr)(&u), MpfrRounding.ToNegativeInfinity);
            }

            int exp;
            IntPtr strptr = MpfrLib.mpfr_get_str(IntPtr.Zero, (IntPtr)(&exp), 10, 0, (IntPtr)(&s), MpfrRounding.ToNegativeInfinity);
            string str = Marshal.PtrToStringAnsi(strptr)!;
            Assert.Equal("2.7182818284590452353602874713526624977572470936999595749669131", GmpFloat.ToString(str, exp));
            MpfrLib.mpfr_free_str(strptr);
            MpfrLib.mpfr_clear((IntPtr)(&s));
            MpfrLib.mpfr_clear((IntPtr)(&t));
            MpfrLib.mpfr_clear((IntPtr)(&u));
            MpfrLib.mpfr_free_cache();
#pragma warning restore CA1806 // ��Ҫ���Է������
        }
    }
}