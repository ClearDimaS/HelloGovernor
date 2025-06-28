public static class AdConfig
{
    public static string AppKey => GetAppKey();
    public static string BannerAdUnitId => GetBannerAdUnitId();
    public static string InterstitalAdUnitId => GetInterstitialAdUnitId();
    public static string RewardedVideoAdUnitId => GetRewardedVideoAdUnitId();

    static string GetAppKey()
    {
        #if UNITY_ANDROID
            return "21fe71be5";
        #elif UNITY_IPHONE
            return "22966076d";
        #else
            return "unexpected_platform";
        #endif
    }

    static string GetBannerAdUnitId()
    {
        #if UNITY_ANDROID
            return "hbwcefwcyb5zuzo8";
        #elif UNITY_IPHONE
            return "0z8skq9vgjb73hry";
        #else
            return "unexpected_platform";
        #endif
    }
    static string GetInterstitialAdUnitId()
    {
#if UNITY_ANDROID
        return "v09u52bhqfxbipom";
#elif UNITY_IPHONE
            return "1yx4xaftg8yaxfba";
#else
            return "unexpected_platform";
        #endif
    }

    static string GetRewardedVideoAdUnitId()
    {
#if UNITY_ANDROID
        return "uti54guhiory2xk9";
#elif UNITY_IPHONE
            return "4uddchai11obhhh8";
#else
            return "unexpected_platform";
        #endif
    }
}
