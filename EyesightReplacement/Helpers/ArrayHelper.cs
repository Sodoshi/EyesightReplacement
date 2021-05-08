using System.Linq;
namespace EyesightReplacement.Helpers
{
    public static class ArrayHelper
    {
        public static float[] ConvertToFloatArray(string Floats)
        {
            return Floats.Split(' ')
                .SelectMany(s => float.TryParse(s, out var i) ? new[] {i} : new float[0])
                .ToArray();
        }
        public static int[] ConvertToIntArray(string Ints)
        {
            return Ints.Split(' ')
                .SelectMany(s => int.TryParse(s, out var i) ? new[] {i} : new int[0])
                .ToArray();
        }
        public static float[] Multiply(float[] a1, float[] a2)
        {
            if (a1.Length != 16 || a2.Length != 16)
            {
                return a2;
            }
            var r = new float[16];
            r[0] = a1[0] * a2[0] + a1[1] * a2[4] + a1[2] * a2[8] + a1[3] * a2[12];
            r[1] = a1[0] * a2[1] + a1[1] * a2[5] + a1[2] * a2[9] + a1[3] * a2[13];
            r[2] = a1[0] * a2[2] + a1[1] * a2[6] + a1[2] * a2[10] + a1[3] * a2[14];
            r[3] = a1[0] * a2[3] + a1[1] * a2[7] + a1[2] * a2[11] + a1[3] * a2[15];
            r[4] = a1[4] * a2[0] + a1[5] * a2[4] + a1[6] * a2[8] + a1[7] * a2[12];
            r[5] = a1[4] * a2[1] + a1[5] * a2[5] + a1[6] * a2[9] + a1[7] * a2[13];
            r[6] = a1[4] * a2[2] + a1[5] * a2[6] + a1[6] * a2[10] + a1[7] * a2[14];
            r[7] = a1[4] * a2[3] + a1[5] * a2[7] + a1[6] * a2[11] + a1[7] * a2[15];
            r[8] = a1[8] * a2[0] + a1[9] * a2[4] + a1[10] * a2[8] + a1[11] * a2[12];
            r[9] = a1[8] * a2[1] + a1[9] * a2[5] + a1[10] * a2[9] + a1[11] * a2[13];
            r[10] = a1[8] * a2[2] + a1[9] * a2[6] + a1[10] * a2[10] + a1[11] * a2[14];
            r[11] = a1[8] * a2[3] + a1[9] * a2[7] + a1[10] * a2[11] + a1[11] * a2[15];
            r[12] = a1[12] * a2[0] + a1[13] * a2[4] + a1[14] * a2[8] + a1[15] * a2[12];
            r[13] = a1[12] * a2[1] + a1[13] * a2[5] + a1[14] * a2[9] + a1[15] * a2[13];
            r[14] = a1[12] * a2[2] + a1[13] * a2[6] + a1[14] * a2[10] + a1[15] * a2[14];
            r[15] = a1[12] * a2[3] + a1[13] * a2[7] + a1[14] * a2[11] + a1[15] * a2[15];
            return r;
        }
    }
}