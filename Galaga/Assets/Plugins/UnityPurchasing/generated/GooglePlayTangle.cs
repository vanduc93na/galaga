#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("1Wfkx9Xo4+zPY61jEujk5OTg5eb/2Cs7svT3O6nE7ZzHUyx2lyrwkPKGCZZ/Tl/GtA4+cMth2aExVLW/v9XdIz8MYlMxSAoR9XP9LQMh6sRMGaKJd9YBMKsY17q53cJC1rXzXmfk6uXVZ+Tv52fk5OV53gDa18m1Xds6oUI5zHBpqMEToDfapPdnBuiW4iImZu8jmghvvtuzou2BZLY8K7edJuUHvsPVPi4iTZz01WhRT8DsLhYcaSSay5dxYdDK716WlJJ5tMHvGyHS3MRMwFf2bQqcJtPjdmJlNUmX9KV4W5Om3GXr/kaOWSy+0aWz4XhqzUq61NmNw13DMaVeX5JuodA1yrh0KPUm+mDrv2M9/mO0xg2Bcc/W6uyiEPCh2Ofm5OXk");
        private static int[] order = new int[] { 0,4,10,8,8,8,8,13,8,13,11,11,12,13,14 };
        private static int key = 229;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
