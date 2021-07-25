//##############################################
//Cᴏᴘʏʀɪɢʜᴛ 2020 DᴏᴜɢʜᴏᴜᴢLɪɢʜᴛ Codecanyon Item 19703216
//Elin Doughouz >> https://www.facebook.com/Elindoughouz
//====================================================

//For the accuracy of the icon and logo, please use this website " https://appicon.co " and add images according to size in folders " mipmap " 

using System.Collections.Generic;
using WoWonder.Activities.NativePost.Extra;
using WoWonder.Helpers.Model;
using WoWonderClient;

namespace WoWonder
{
    internal static class AppSettings
    {
        /// <summary>
        /// Deep Links To App Content
        /// you should add your website without http in the analytic.xml file >> ../values/analytic.xml .. line 5
        /// <string name="ApplicationUrlWeb">demo.wowonder.com</string>
        /// </summary>
        public static string TripleDesAppServiceProvider = "QYUzMdXjqPIsNPF1Kr0lyvghp9MXnOQsr5Ofq+qfB56iP8lKKtr5IGayCyQBYwNZ8rkY7aHLgWCVRY8pOn1utRPE1Is6tbVxSMJmBPEG7KKhnyJKUJxMqOfwx9w8LGnlk88rJ7Y/bpNzqxWczuMIQyant4ecn1m26r1NrA41i6eKQ9Qc+MA74UQeha1d6yzTiZlRSLXEtSyooCNDJSb9HmQGKRRFBJ2QQ+GGnpSbGVl49oywSW7tDBfZqVvElVwjYuVaeNLtSIKvnRUfFw/Nm8I0VRvLHCUz6f65qdK9tt+PsxkEMZhfL6G6E5ERL079oNBE9MFYUK7b+Z7o15Ln72yNawwomKGIh+kaLIoqmGekBGkwoRmbGKHpdZg3Mzvp/aAxORp0g76OxokEKBD33mGmdWHkSIRRi0C7IJScPKg6V/svKZ2gBe2doEevLuBnD4IE/D+8lvRbhWsMpYryRib65vgFx8AGXY4tvVfMYA4G7GSZNbt3GuIBujUh//abEfnwvTkrOltvVwYWzipAuogQczku8+z5VurJa9KLCuc/ORRbhg/Y+ePM6xa/p6UhMCo373zOdd9G3flvHKaGKBSDL2IcumT/Ja5eDzthVw5MD4X/1ujopiJBRNPQGnuarEfnvi2uBydWnkmPysI70dXVeTFIvK1OIEWiBM8r0dLU2asO6WaIa1oBw0CP21CYnn2FaTDtfv2Dnf6qNsaKLsJJsmzFDpLIiN4LoK70/W6+V1DN7XATHiERF5eA/eOAitGJx0HdLAPBX/dXC71WbXARKN63WOeFMI/QTDu7ZkXh4LkL7IMKS06YLaMOtR1eWZa6gmHY7HQ0q2PEUfzLjLU+rfnzKSN4ERWuFeImMU6mq2yrE54mjRfEviqhHlpp6qEm+t6OigC0TdANT9VVz8rqK3W/YTcLTuLwr7KA4T+ljRrUvZQ7k3x8bguwpm9WcVnE0hzHbiViEgsL/8UV3sO1MaihphULxGZskWZE9LyMeOoPfWa8mdkqiOoeq6KJmYYYANssjwlyYU/k6V45LQYJCxM3urllY93I8Zx3wyZBos7zbFLADpH8wasCb184uyxjL52SUV/POYgkdoF/xhNRyoKNAteWzW9zVq/K9ZgvPrSFTdFufRJWMzrJALB1xIDVxUTW357oJB5sXiOSYotHpvuPnGRzE05Qdm83UuQGn196KmXCugKuI27LFJZ6rc0oEdwq0z70I20qGA5DDwnr0AnE+OSx4neffhtGMsrhjw29DtqPUxLSqRrWxNt/91DsFd0I0CByt3sLlzXzrVyOJmRp4MnusU9ZvJWcsFlOlqKBMkkNg+hVC6fkdJ+DS7fPyaHU7J6GPtD5MDIyqTzxXL54oOiz6KNhA5KCfFYlOD7pMA60nyoKJGCsWibPGNQbOFe47GOGHN8vhNf7Mbia5q+4SjDQOqymt992+19tcrevQ4NUma9dPA9WMH1Z2CjUZxr2DliWCg34UzdmkJjsh6rT/gUC1tWSo7EaQaj7Eas8HF53vbrg/mocYxlITtdnq+6ytoxqwi890ALwm0JWb9kDYT0ZgIzbGXOD8Iz+ItQOEsl/yGuofKTMPj0ZTPi3rEwebmwHPZRRK501bKn4TX7CBNgnFbTCYAqfwE34Uie78N6kNI7Z1I0KS2s4BoI9UsS6ZRLH0BnEqCeHklEH0rOv5sGLLtbSgRdE+nodIRUAWwUT49d4g/MPnknVcbLMUDSuV7O55vmew7PTYQfze31s2iu7xsadrWc26GsENF0Jz/nxJ2dR1NYlV44WZHw7Ki288Anp6rV/jwwuVWidFg5z+2isoNfVjKUNpC0x+X7IIZ3fKJadrjSaN/Oth2Sco5YKlzmpYZx6TipffFv4UIy09maA4f71komooCsq3VywLmj/WvSgmM6dCOMl1mxf47LelPuPlDxYTkKHtVxTqKc4cnjX5WPgwi0DpeO56Be4kfd5S2i/V3+zKxr3xHxbmhf+CDYx2o2BtaDIm+7OPL0RTKYlt7SJlUdLsvmwAZ3j36nzrkK2S0m9rB90n/lciwUFlkIhYLuH7GDo/6Ikg87B8HNSLPTg369S+UKj2ei5CjKatT3AdofZ3ehpEYuPxyqNySWCSOFUx3EBmyo8+yIFfnFf/OhJ/yCnDfoMTrD5FTXfQ/7yvQbsdUOiy6zctFUjAkELgrYryRe8+piKLVUGaq49jgwTdaXkz9GyS6AoBhkgWuaUCikjnvsta9JSxMOHmR5zXe3M0VMby/t+xF6oRAmUM4xrHQKkbu7Me5Zfl9MEqEje6RLCw13jsPALd+7GtPMyxhxjuhhJDQ5D9MlYdjRjrlqLYmlN+SzKRKbfYwhpZg5HZdsqXnzY66A76LYhRKbCNhzmHF4F6oKk6A9I0g9wEKINMA1gAYRFfkAUFeJACUX4N/L8uFjupFEWOvHfmbazvLklLdWFwb4qJ7YYL4oEO2kzShNSxIjpGMZ6K5T+aTKISazNZPTh";

        //Main Settings >>>>>
        //*********************************************************
        public static string Version = "2.9";
        public static string ApplicationName = "The Corner Table Social Network";
        public static string DatabaseName = "CornerTable"; 

        // Friend system = 0 , follow system = 1
        public static int ConnectivitySystem = 1;
         
        //Main Colors >>
        //*********************************************************
        public static string MainColor = "#a84849";
         
        //Language Settings >> http://www.lingoes.net/en/translator/langcode.htm
        //*********************************************************
        public static bool FlowDirectionRightToLeft = false;
        public static string Lang = ""; //Default language ar

        //Set Language User on site from phone 
        public static bool SetLangUser = true; 

        //Notification Settings >>
        //*********************************************************
        public static bool ShowNotification = true;
        public static string OneSignalAppId = "de8377cd-294d-4fcf-a455-4a4237e2e18e";
        public static string MsgOneSignalAppId = "de8377cd-294d-4fcf-a455-4a4237e2e18e";

        // WalkThrough Settings >>
        //*********************************************************
        public static bool ShowWalkTroutPage = true;

        //Main Messenger settings
        //*********************************************************
        public static bool MessengerIntegration = false;
        public static bool ShowDialogAskOpenMessenger = false; 
        public static string MessengerPackageName = "com.cornertable.app"; //APK name on Google Play

        //AdMob >> Please add the code ad in the Here and analytic.xml 
        //*********************************************************
        public static bool ShowAdMobBanner = false;
        public static bool ShowAdMobInterstitial = false;
        public static bool ShowAdMobRewardVideo = false;
        public static bool ShowAdMobNative = false;
        public static bool ShowAdMobNativePost = false;
        public static bool ShowAdMobAppOpen = false;  
        public static bool ShowAdMobRewardedInterstitial = false;  

        public static string AdInterstitialKey = "ca-app-pub-5135691635931982/3584502890";
        public static string AdRewardVideoKey = "ca-app-pub-5135691635931982/2518408206";
        public static string AdAdMobNativeKey = "ca-app-pub-5135691635931982/2280543246";
        public static string AdAdMobAppOpenKey = "ca-app-pub-5135691635931982/2813560515";  
        public static string AdRewardedInterstitialKey = "ca-app-pub-5135691635931982/7842669101";  

        //Three times after entering the ad is displayed
        public static int ShowAdMobInterstitialCount = 3;
        public static int ShowAdMobRewardedVideoCount = 3;
        public static int ShowAdMobNativeCount = 40;
        public static int ShowAdMobAppOpenCount = 2;  
        public static int ShowAdMobRewardedInterstitialCount = 3;  

        //FaceBook Ads >> Please add the code ad in the Here and analytic.xml 
        //*********************************************************
        public static bool ShowFbBannerAds = false; 
        public static bool ShowFbInterstitialAds = false;  
        public static bool ShowFbRewardVideoAds = false; 
        public static bool ShowFbNativeAds = false; 
         
        //YOUR_PLACEMENT_ID
        public static string AdsFbBannerKey = "250485588986218_554026418632132"; 
        public static string AdsFbInterstitialKey = "250485588986218_554026125298828";  
        public static string AdsFbRewardVideoKey = "250485588986218_554072818627492"; 
        public static string AdsFbNativeKey = "250485588986218_554706301897477"; 

        //Three times after entering the ad is displayed
        public static int ShowFbNativeAdsCount = 40;

        //Colony Ads >> Please add the code ad in the Here 
        //*********************************************************  
        public static bool ShowColonyBannerAds = false; //#New
        public static bool ShowColonyInterstitialAds = false; //#New
        public static bool ShowColonyRewardAds = false; //#New

        public static string AdsColonyAppId = "appff22269a7a0a4be8aa"; //#New
        public static string AdsColonyBannerId = "vz85ed7ae2d631414fbd"; //#New
        public static string AdsColonyInterstitialId = "vz39712462b8634df4a8"; //#New
        public static string AdsColonyRewardedId = "vz32ceec7a84aa4d719a"; //#New 
        //********************************************************* 

        public static bool EnableRegisterSystem = true;
        /// <summary>
        /// true => Only over 18 years old
        /// false => all 
        /// </summary>
        public static bool IsUserYearsOld = true; //#New

        //Set Theme Full Screen App
        //*********************************************************
        public static bool EnableFullScreenApp = false;
            
        //Code Time Zone (true => Get from Internet , false => Get From #CodeTimeZone )
        //*********************************************************
        public static bool AutoCodeTimeZone = true;
        public static string CodeTimeZone = "UTC";

        //Error Report Mode
        //*********************************************************
        public static bool SetApisReportMode = false;

        //Social Logins >>
        //If you want login with facebook or google you should change id key in the analytic.xml file 
        //Facebook >> ../values/analytic.xml .. line 10-11 
        //Google >> ../values/analytic.xml .. line 15 
        //*********************************************************
        public static bool ShowFacebookLogin = false;
        public static bool ShowGoogleLogin = false;

        public static readonly string ClientId = "430795656343-679a7fus3pfr1ani0nr0gosotgcvq2s8.apps.googleusercontent.com";

        //########################### 

        //Main Slider settings
        //*********************************************************
        public static PostButtonSystem PostButton = PostButtonSystem.ReactionDefault;
        public static ToastTheme ToastTheme = ToastTheme.Custom;//#New 

        public static BottomNavigationSystem NavigationBottom = BottomNavigationSystem.Default;//#New 

        public static bool ShowBottomAddOnTab = true; //New 
         
        public static bool ShowAlbum = true;
        public static bool ShowArticles = true;
        public static bool ShowPokes = true;
        public static bool ShowCommunitiesGroups = true;
        public static bool ShowCommunitiesPages = true;
        public static bool ShowMarket = true;
        public static bool ShowPopularPosts = true;
        public static bool ShowBoostedPosts = true; //New 
        public static bool ShowBoostedPages = true; //New 
        public static bool ShowMovies = true;
        public static bool ShowNearBy = true;
        public static bool ShowStory = false;
        public static bool ShowSavedPost = true;
        public static bool ShowUserContacts = true; 
        public static bool ShowJobs = true; 
        public static bool ShowCommonThings = true; 
        public static bool ShowFundings = true;
        public static bool ShowMyPhoto = true; 
        public static bool ShowMyVideo = true; 
        public static bool ShowGames = true;
        public static bool ShowMemories = true;  
        public static bool ShowOffers = true;  
        public static bool ShowNearbyShops = true;   

        public static bool ShowSuggestedPage = true;//New 
        public static bool ShowSuggestedGroup = true;
        public static bool ShowSuggestedUser = true;
         
        //count times after entering the Suggestion is displayed
        public static int ShowSuggestedPageCount = 90; //New 
        public static int ShowSuggestedGroupCount = 70; 
        public static int ShowSuggestedUserCount = 50;

        //allow download or not when share
        public static bool AllowDownloadMedia = true; 

        public static bool ShowAdvertise = false; //New  
         
        /// <summary>
        /// https://rapidapi.com/api-sports/api/covid-193
        /// you can get api key and host from here https://prnt.sc/wngxfc 
        /// </summary>
        public static bool ShowInfoCoronaVirus = false;  
        public static string KeyCoronaVirus = "164300ef98msh0911b69bed3814bp131c76jsneaca9364e61f"; 
        public static string HostCoronaVirus = "covid-193.p.rapidapi.com"; 
         
        public static bool ShowLive = false;  
        public static string AppIdAgoraLive = "c55b9bda665042809b61dfeb3f3832e0"; 

        //Events settings
        //*********************************************************  
        public static bool ShowEvents = true; 
        public static bool ShowEventGoing = true; 
        public static bool ShowEventInvited = true;  
        public static bool ShowEventInterested = true;  
        public static bool ShowEventPast = true;

        // Story >>
        //*********************************************************
        //Set a story duration >> 10 Sec
        public static long StoryDuration = 10000L;
        public static bool EnableStorySeenList = true; //#New 
        public static bool EnableReplyStory = true; //#New  

        //*********************************************************

        /// <summary>
        ///  Currency
        /// CurrencyStatic = true : get currency from app not api 
        /// CurrencyStatic = false : get currency from api (default)
        /// </summary>
        public static readonly bool CurrencyStatic = false;
        public static readonly string CurrencyIconStatic = "$";
        public static readonly string CurrencyCodeStatic = "USD";
        public static readonly string CurrencyFundingPriceStatic = "$";

        //Profile settings
        //*********************************************************
        public static bool ShowGift = true;
        public static bool ShowWallet = true; 
        public static bool ShowGoPro = true;  
        public static bool ShowAddToFamily = true;

        public static bool ShowUserGroup = false; //#New
        public static bool ShowUserPage = false; //#New
        public static bool ShowUserImage = true; //#New
        public static bool ShowUserSocialLinks = false; //#New

        public static CoverImageStyle CoverImageStyle = CoverImageStyle.CenterCrop; //#New

        /// <summary>
        /// The default value comes from the site .. in case it is not available, it is taken from these values
        /// </summary>
        public static string WeeklyPrice = "3"; 
        public static string MonthlyPrice = "8"; 
        public static string YearlyPrice = "89"; 
        public static string LifetimePrice = "259";

        //Native Post settings
        //********************************************************* 
        public static bool ShowTextWithSpace = true;//#New

        public static ImagePostStyle ImagePostStyle = ImagePostStyle.FullWidth; //#New

        public static bool ShowTextShareButton = false;
        public static bool ShowShareButton = true;
         
        public static int AvatarPostSize = 60;
        public static int ImagePostSize = 200;
        public static string PostApiLimitOnScroll = "22";

        //Get post in background >> 1 Min = 30 Sec
        public static long RefreshPostSeconds = 30000;  
        public static string PostApiLimitOnBackground = "12"; 

        public static bool AutoPlayVideo = true;
         
        public static bool EmbedPlayTubePostType = true;
        public static bool EmbedDeepSoundPostType = true;
        public static VideoPostTypeSystem EmbedFacebookVideoPostType = VideoPostTypeSystem.EmbedVideo; 
        public static VideoPostTypeSystem EmbedVimeoVideoPostType = VideoPostTypeSystem.EmbedVideo; 
        public static VideoPostTypeSystem EmbedPlayTubeVideoPostType = VideoPostTypeSystem.Link; 
        public static VideoPostTypeSystem EmbedTikTokVideoPostType = VideoPostTypeSystem.Link; 
        public static bool ShowSearchForPosts = true; 
        public static bool EmbedLivePostType = true;
         
        //new posts users have to scroll back to top
        public static bool ShowNewPostOnNewsFeed = true; 
        public static bool ShowAddPostOnNewsFeed = false; 
        public static bool ShowCountSharePost = true;

        public static WRecyclerView.VolumeState DefaultVolumeVideoPost = WRecyclerView.VolumeState.Off;//#New 

        /// <summary>
        /// Post Privacy
        /// ShowPostPrivacyForAllUser = true : all posts user have icon Privacy 
        /// ShowPostPrivacyForAllUser = false : just my posts have icon Privacy (default)
        /// </summary>
        public static bool ShowPostPrivacyForAllUser = false; 

        public static bool ShowFullScreenVideoPost = true;

        public static bool EnableVideoCompress = false; 
         
        //Trending page
        //*********************************************************   
        public static bool ShowTrendingPage = true;
         
        public static bool ShowProUsersMembers = true;
        public static bool ShowPromotedPages = true;
        public static bool ShowTrendingHashTags = true;
        public static bool ShowLastActivities = true;
        public static bool ShowShortcuts = true; 
        public static bool ShowFriendsBirthday = true;//#New
        public static bool ShowAnnouncement = true;//#New

        /// <summary>
        /// https://www.weatherapi.com
        /// </summary>
        public static bool ShowWeather = false;  
        public static string KeyWeatherApi = "e7cffc4d6a9a4a149a1113143201711";

        /// <summary>
        /// https://openexchangerates.org
        /// #Currency >> Your currency
        /// #Currencies >> you can use just 3 from those : USD,EUR,DKK,GBP,SEK,NOK,CAD,JPY,TRY,EGP,SAR,JOD,KWD,IQD,BHD,DZD,LYD,AED,QAR,LBP,OMR,AFN,ALL,ARS,AMD,AUD,BYN,BRL,BGN,CLP,CNY,MYR,MAD,ILS,TND,YER
        /// </summary>
        public static bool ShowExchangeCurrency = false; 
        public static string KeyCurrencyApi = "644761ef2ba94ea5aa84767109d6cf7b"; 
        public static string ExCurrency = "USD";  
        public static string ExCurrencies = "EUR,GBP,TRY"; 
        public static readonly List<string> ExCurrenciesIcons = new List<string> {"€", "£", "₺"}; 

        //********************************************************* 

        public static bool ShowUserPoint = true;

        //Add Post
        public static bool ShowGalleryImage = true;
        public static bool ShowGalleryVideo = true;
        public static bool ShowMention = true;
        public static bool ShowLocation = true;
        public static bool ShowFeelingActivity = true;
        public static bool ShowFeeling = true;
        public static bool ShowListening = true;
        public static bool ShowPlaying = true;
        public static bool ShowWatching = true;
        public static bool ShowTraveling = true;
        public static bool ShowGif = true;
        public static bool ShowFile = true;
        public static bool ShowMusic = true;
        public static bool ShowPolls = true;
        public static bool ShowColor = true;
        public static bool ShowVoiceRecord = true;//#New

        public static bool ShowAnonymousPrivacyPost = true;

        //Advertising 
        public static bool ShowAdvertisingPost = true;  

        //Settings Page >> General Account
        public static bool ShowSettingsGeneralAccount = true;
        public static bool ShowSettingsAccount = true;
        public static bool ShowSettingsSocialLinks = true;
        public static bool ShowSettingsPassword = true;
        public static bool ShowSettingsBlockedUsers = true;
        public static bool ShowSettingsDeleteAccount = true;
        public static bool ShowSettingsTwoFactor = true; 
        public static bool ShowSettingsManageSessions = true;  
        public static bool ShowSettingsVerification = true;
         
        public static bool ShowSettingsSocialLinksFacebook = true; 
        public static bool ShowSettingsSocialLinksTwitter = true; 
        public static bool ShowSettingsSocialLinksGoogle = true; 
        public static bool ShowSettingsSocialLinksVkontakte = true;  
        public static bool ShowSettingsSocialLinksLinkedin = true;  
        public static bool ShowSettingsSocialLinksInstagram = true;  
        public static bool ShowSettingsSocialLinksYouTube = true;  

        //Settings Page >> Privacy
        public static bool ShowSettingsPrivacy = true;
        public static bool ShowSettingsNotification = true;

        //Settings Page >> Tell a Friends (Earnings)
        public static bool ShowSettingsInviteFriends = true;

        public static bool ShowSettingsShare = true;
        public static bool ShowSettingsMyAffiliates = true;
        public static bool ShowWithdrawals = true;

        /// <summary>
        /// if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        /// Just replace it with this 5 lines of code
        /// <uses-permission android:name="android.permission.READ_CONTACTS" />
        /// <uses-permission android:name="android.permission.READ_PHONE_NUMBERS" />
        /// </summary>
        public static bool InvitationSystem = false; 

        //Settings Page >> Help && Support
        public static bool ShowSettingsHelpSupport = true;

        public static bool ShowSettingsHelp = true;
        public static bool ShowSettingsReportProblem = true;
        public static bool ShowSettingsAbout = true;
        public static bool ShowSettingsPrivacyPolicy = true;
        public static bool ShowSettingsTermsOfUse = true;

        public static bool ShowSettingsRateApp = true; 
        public static int ShowRateAppCount = 5; 
         
        public static bool ShowSettingsUpdateManagerApp = false; 

        public static bool ShowSettingsInvitationLinks = true; 
        public static bool ShowSettingsMyInformation = true; 

        public static bool ShowSuggestedUsersOnRegister = true;

        //Set Theme Tab
        //*********************************************************
        public static bool SetTabDarkTheme = false;
        public static MoreTheme MoreTheme = MoreTheme.Card;//#New 

        //Bypass Web Errors  
        //*********************************************************
        public static bool TurnTrustFailureOnWebException = true;
        public static bool TurnSecurityProtocolType3072On = true;

        //*********************************************************
        public static bool RenderPriorityFastPostLoad = false;

        /// <summary>
        /// if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        /// <uses-permission android:name="com.android.vending.BILLING" />
        /// </summary>
        public static bool ShowInAppBilling = true; 

        public static bool ShowPaypal = true; 
        public static bool ShowBankTransfer = false; 
        public static bool ShowCreditCard = true;

        //********************************************************* 
        public static bool ShowCashFree = false;  

        /// <summary>
        /// Currencies : http://prntscr.com/u600ok
        /// </summary>
        public static string CashFreeCurrency = "INR";  

        //********************************************************* 

        /// <summary>
        /// If you want RazorPay you should change id key in the analytic.xml file
        /// razorpay_api_Key >> .. line 24 
        /// </summary>
        public static bool ShowRazorPay = false; 

        /// <summary>
        /// Currencies : https://razorpay.com/accept-international-payments
        /// </summary>
        public static string RazorPayCurrency = "USD";  
         
        public static bool ShowPayStack = false;  
        public static bool ShowPaySera = false;  //#Next Version   

        //********************************************************* 
         
        //Chat
        //*********************************************************  
        public static SystemApiGetLastChat LastChatSystem = SystemApiGetLastChat.Old; //#New 
        public static InitializeWoWonder.ConnectionType ConnectionTypeChat = InitializeWoWonder.ConnectionType.RestApi; //New

        //Chat Window Activity >>
        //*********************************************************
        //if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        //Just replace it with this 5 lines of code
        /*
         <uses-permission android:name="android.permission.READ_CONTACTS" />
         <uses-permission android:name="android.permission.READ_PHONE_NUMBERS" /> 
         */
        public static bool ShowButtonContact = false;
        /////////////////////////////////////

        public static bool ShowButtonCamera = true;
        public static bool ShowButtonImage = true;
        public static bool ShowButtonVideo = true;
        public static bool ShowButtonAttachFile = true;
        public static bool ShowButtonColor = true;
        public static bool ShowButtonStickers = true;
        public static bool ShowButtonMusic = true;
        public static bool ShowButtonGif = true;
        public static bool ShowButtonLocation = true;

        public static bool ShowMusicBar = false;

        public static bool OpenVideoFromApp = true;
        public static bool OpenImageFromApp = true;

        // Stickers Packs Settings >> 
        public static int StickersOnEachRow = 3;
        public static string StickersBarColor = "#efefef";
        public static string StickersBarColorDark = "#282828";

        public static bool ShowStickerStack0 = true;
        public static bool ShowStickerStack1 = true;
        public static bool ShowStickerStack2 = true;
        public static bool ShowStickerStack3 = true;
        public static bool ShowStickerStack4 = true;
        public static bool ShowStickerStack5 = true;
        public static bool ShowStickerStack6 = true;

        //Record Sound Style & Text 
        public static bool ShowButtonRecordSound = true;

        // Options List Message
        public static bool EnableReplyMessageSystem = false; //#New  
        public static bool EnableForwardMessageSystem = true; //#New 
        public static bool EnableFavoriteMessageSystem = true; //#New 
        public static bool EnableReactionMessageSystem = true; //#New 
        public static bool EnablePinMessageSystem = false; //#New >> Next Version

        //List Chat >>
        //*********************************************************
        public static bool EnableChatPage = true;
        public static bool EnableChatGroup = true;

        // Options List Chat
        public static bool EnableChatArchive = true; //#New
        public static bool EnableChatPin = true; //#New
        public static bool EnableChatMute = true; //#New
        public static bool EnableChatMakeAsRead = true; //#New 

        // Video/Audio Call Settings >>
        //*********************************************************
        public static bool EnableAudioVideoCall = false;

        public static bool EnableAudioCall = false;
        public static bool EnableVideoCall = false;

        public static SystemCall UseLibrary = SystemCall.Twilio;

        //Last_Messages Page >>
        ///*********************************************************
        public static bool ShowOnlineOfflineMessage = false;

        public static int RefreshChatActivitiesSeconds = 6000; // 6 Seconds
        public static int MessageRequestSpeed = 3000; // 3 Seconds

        //Options chat heads (Bubbles) 
        //*********************************************************
        //Always , Hide , FullScreen
        public static string DisplayModeSettings = "Always";

        //Default , Left  , Right , Nearest , Fix , Thrown
        public static string MoveDirectionSettings = "Right";

        //Circle , Rectangle
        public static string ShapeSettings = "Circle";

        // Last position
        public static bool IsUseLastPosition = true;


        public static bool ShowSettingsWallpaper = true; //#New

    }
}
