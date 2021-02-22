//##############################################
//Cᴏᴘʏʀɪɢʜᴛ 2020 DᴏᴜɢʜᴏᴜᴢLɪɢʜᴛ Codecanyon Item 19703216
//Elin Doughouz >> https://www.facebook.com/Elindoughouz
//====================================================

//For the accuracy of the icon and logo, please use this website " https://appicon.co " and add images according to size in folders " mipmap " 

using System.Collections.Generic;
using WoWonder.Helpers.Model;

namespace WoWonder
{
    internal static class AppSettings
    {
        /// <summary>
        /// Deep Links To App Content
        /// you should add your website without http in the analytic.xml file >> ../values/analytic.xml .. line 5
        /// <string name="ApplicationUrlWeb">demo.wowonder.com</string>
        /// </summary>
        public static string TripleDesAppServiceProvider = "9mzWRy0lKsdQ8KBodhFGRFYBpU1+c9fwo5zNmJ6omqQal77Cp7ifye1ryqkjj1yW0LIC+ezBhB++7ILMo5roJ550mvoJ28+BrbQ3WdbI84gFcCNTZYD0nQhok9+fJuCm6EsVBoQp5C0ytfjGc+o5lvS4Euin8Rdqx0OI1J069EGYUXGu4MhP93tZ8hQSla6ABhAmph9LgOCi75dGOPXL781ZwzgTBLhtdTVqafBxpIjere1FgYrTYUIPQd22OVIf"; 

        //Main Settings >>>>>
        //*********************************************************
        public static string Version = "1.0.5";
        public static string ApplicationName = "The Corner Table Social Media App";
        public static string DatabaseName = "CornerTable"; 

        // Friend system = 0 , follow system = 1
        public static int ConnectivitySystem = 1;

        public static PostButtonSystem PostButton = PostButtonSystem.ReactionDefault;
        public static bool ShowTextShareButton = true;
        public static bool ShowShareButton = true; 

        //Main Colors >>
        //*********************************************************
        public static string MainColor = "#a84849";
        public static string StoryReadColor = "#808080";

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
        public static bool WalkThroughSetFlowAnimation = true;
        public static bool WalkThroughSetZoomAnimation = false;
        public static bool WalkThroughSetSlideOverAnimation = false;
        public static bool WalkThroughSetDepthAnimation = false;
        public static bool WalkThroughSetFadeAnimation = false;

        //Main Messenger settings
        //*********************************************************
        public static bool MessengerIntegration = true;
        public static bool ShowDialogAskOpenMessenger = false;//#New
        public static string MessengerPackageName = "com.cornertable.app"; //APK name on Google Play

        //AdMob >> Please add the code ad in the Here and analytic.xml 
        //*********************************************************
        public static bool ShowAdMobBanner = false;
        public static bool ShowAdMobInterstitial = false;
        public static bool ShowAdMobRewardVideo = false;
        public static bool ShowAdMobNative = false;
        public static bool ShowAdMobNativePost = false;
        public static bool ShowAdMobAppOpen = false; //#New
        public static bool ShowAdMobRewardedInterstitial = false; //#New 

        public static string AdInterstitialKey = "ca-app-pub-5135691635931982/3276797899";
        public static string AdRewardVideoKey = "ca-app-pub-5135691635931982/8193070896";
        public static string AdAdMobNativeKey = "ca-app-pub-5135691635931982/4917290429";
        public static string AdAdMobAppOpenKey = "ca-app-pub-5135691635931982/3449206524"; //#New
        public static string AdRewardedInterstitialKey = "ca-app-pub-5135691635931982/5364486052"; //#New

        //Three times after entering the ad is displayed
        public static int ShowAdMobInterstitialCount = 3;
        public static int ShowAdMobRewardedVideoCount = 3;
        public static int ShowAdMobNativeCount = 40;
        public static int ShowAdMobAppOpenCount = 2; //#New
        public static int ShowAdMobRewardedInterstitialCount = 3; //#New

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
         
        //********************************************************* 
        public static bool EnableRegisterSystem = true;  
        public static bool ShowGenderOnRegister = true;

        //Set Theme Welcome Pages 
        //*********************************************************
        //Types >> Gradient or Video or Image
        /// <summary> 
        /// if Gradient you can change color from  ../values/colors.xml
        /// <color name="background_First_Layout_startColor">#777777</color>
        /// <color name="background_First_Layout_centerColor">#888888</color>
        /// <color name="background_First_Layout_endColor">#444444</color>
        /// </summary>
        public static string BackgroundScreenWelcomeType = "Image";

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
        public static bool ShowAlbum = true;
        public static bool ShowArticles = true;
        public static bool ShowPokes = true;
        public static bool ShowCommunitiesGroups = true;
        public static bool ShowCommunitiesPages = true;
        public static bool ShowMarket = true;
        public static bool ShowPopularPosts = true;
        public static bool ShowMovies = true;
        public static bool ShowNearBy = true;
        public static bool ShowStory = true;
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

        public static bool ShowSuggestedGroup = true;
        public static bool ShowSuggestedUser = true;
         
        //count times after entering the Suggestion is displayed
        public static int ShowSuggestedGroupCount = 70; 
        public static int ShowSuggestedUserCount = 50;

        //allow download or not when share
        public static bool AllowDownloadMedia = true; 

        public static bool ShowAdvertise = false; //New >> Next Version
         
        /// <summary>
        /// https://rapidapi.com/api-sports/api/covid-193
        /// you can get api key and host from here https://prnt.sc/wngxfc 
        /// </summary>
        public static bool ShowInfoCoronaVirus = false;//#New
        public static string KeyCoronaVirus = "164300ef98msh0911b69bed3814bp131c76jsneaca9364e61f";//#New
        public static string HostCoronaVirus = "covid-193.p.rapidapi.com";//#New
         
        public static bool ShowLive = false;  
        public static string AppIdAgoraLive = "c55b9bda665042809b61dfeb3f3832e0"; 

        //Events settings
        //*********************************************************  
        public static bool ShowEvents = true; 
        public static bool ShowEventGoing = true; 
        public static bool ShowEventInvited = true;  
        public static bool ShowEventInterested = true;  
        public static bool ShowEventPast = true; 

        //Set a story duration >> 7 Sec
        public static long StoryDuration = 7000L;
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
        public static bool ShowWithdrawals = true;
        public static bool ShowAddToFamily = true; //#New

        /// <summary>
        /// The default value comes from the site .. in case it is not available, it is taken from these values
        /// </summary>
        public static string WeeklyPrice = "3"; 
        public static string MonthlyPrice = "8"; 
        public static string YearlyPrice = "89"; 
        public static string LifetimePrice = "259"; 

        //Native Post settings
        //*********************************************************
        public static int AvatarPostSize = 60;
        public static int ImagePostSize = 200;
        public static string PostApiLimitOnScroll = "22";

        //Get post in background >> 1 Min = 60 Sec
        public static long RefreshPostSeconds = 60000;  
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

        /// <summary>
        /// Post Privacy
        /// ShowPostPrivacyForAllUser = true : all posts user have icon Privacy 
        /// ShowPostPrivacyForAllUser = false : just my posts have icon Privacy (default)
        /// </summary>
        public static bool ShowPostPrivacyForAllUser = false; 

        public static bool ShowFullScreenVideoPost = true;

        public static bool EnableVideoCompress = true; 
         
        //Trending page
        //*********************************************************   
        public static bool ShowTrendingPage = true;
         
        public static bool ShowProUsersMembers = true;
        public static bool ShowPromotedPages = true;
        public static bool ShowTrendingHashTags = true;
        public static bool ShowLastActivities = true;
        public static bool ShowShortcuts = true;//#New

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
        public static bool ShowExchangeCurrency = false;//#New
        public static string KeyCurrencyApi = "644761ef2ba94ea5aa84767109d6cf7b";//#New
        public static string ExCurrency = "USD"; //#New
        public static string ExCurrencies = "EUR,GBP,TRY";//#New 
        public static readonly List<string> ExCurrenciesIcons = new List<string>() {"€", "£", "₺"};//#New

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

        public static bool ShowAnonymousPrivacyPost = true; 

        //Boost 
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
         
        /// <summary>
        /// if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        /// Just replace it with this 5 lines of code
        /// <uses-permission android:name="android.permission.READ_CONTACTS" />
        /// <uses-permission android:name="android.permission.READ_PHONE_NUMBERS" />
        /// <uses-permission android:name="android.permission.SEND_SMS" />
        /// </summary>
        public static bool InvitationSystem = false;

        public static int LimitGoProPlansCountsTo = 4;

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

        public static bool ImageCropping = true;

        //Set Theme Tab
        //*********************************************************
        public static bool SetTabDarkTheme = false;
        public static MoreTheme MoreTheme = MoreTheme.BeautyTheme; 
        public static bool SetTabOnButton = true;

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
        public static bool ShowInAppBilling = false; 

        public static bool ShowPaypal = true; 
        public static bool ShowBankTransfer = true; 
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
         
        //Chat Window Activity >>
        //*********************************************************
        //if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        //Just replace it with this 5 lines of code
        /*
         <uses-permission android:name="android.permission.READ_CONTACTS" />
         <uses-permission android:name="android.permission.READ_PHONE_NUMBERS" /> 
         <uses-permission android:name="android.permission.GET_ACCOUNTS" />
         <uses-permission android:name="android.permission.SEND_SMS" />
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
        public static bool EnableReplyMessageSystem = false; //#New >> Next Version
        public static bool EnableForwardMessageSystem = true; //#New 
        public static bool EnableFavoriteMessageSystem = true; //#New 
        public static bool EnablePinMessageSystem = true; //#New 

        //List Chat >>
        //*********************************************************
        public static bool EnableChatPage = true;
        public static bool EnableChatGroup = true;

        // Options List Chat
        public static bool EnableChatArchive = true; //#New
        public static bool EnableChatPin = true; //#New
        public static bool EnableChatMute = true; //#New
        public static bool EnableChatMakeAsRead = true; //#New
        public static bool EnableChatReport = false; //#New >> Next Version
         
        // Story >>
        //*********************************************************
        //Set a story duration >> 10 Sec 
        public static bool EnableStorySeenList = false; //#New >> Next Version
        public static bool EnableReplyStory = false; //#New >> Next Version

        // Video/Audio Call Settings >>
        //*********************************************************
        public static bool EnableAudioVideoCall = true;

        public static bool EnableAudioCall = true;
        public static bool EnableVideoCall = true;

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