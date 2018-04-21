#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("IwmycZMqV0GqurbZCGBB/MXbVHh17P5Z3i5ATRlXyVelMcrLBvo1RLqCiP2wDl8D5fVEXnvKAgAG7SBVyU+uNdatWOT9PFWHNKNOMGPzknxrTL+vJmBjrz1QeQhTx7jiA75kBHuPtUZIUNhUw2L5ngiyR3fi9vGh2I02HeNClaQ/jEMuLUlW1kIhZ8pB83BTQXx3eFv3OfeGfHBwcHRxcqFeLOC8YbJu9H8r96lq9yBSmRXl3QNgMezPBzJI8X9q0hrNuCpFMScrQUm3q5j2x6XcnoVh52m5l7V+UPNwfnFB83B7c/NwcHHtSpROQ10hZhKdAuvay1IgmqrkX/VNNaXAISsCdray8nu3Dpz7Kk8nNnkV8CKov1tCfng2hGQ1THNycHFw");
        private static int[] order = new int[] { 6,12,7,5,4,11,11,11,9,11,11,12,12,13,14 };
        private static int key = 113;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
