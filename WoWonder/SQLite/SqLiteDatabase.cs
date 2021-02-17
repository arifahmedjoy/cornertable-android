using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using AndroidX.Core.Content;
using FloatingView.Lib;
using Java.Lang;
using Newtonsoft.Json;
using SQLite;
using WoWonder.Activities.Chat.Adapters;
using WoWonder.Activities.Chat.ChatWindow;
using WoWonder.Activities.Chat.Floating;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.Library.OneSignal;
using WoWonderClient;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;
using WoWonderClient.Classes.Movies;
using WoWonderClient.Classes.Posts;
using WoWonderClient.Classes.Product;
using Exception = System.Exception;

namespace WoWonder.SQLite
{
    public class SqLiteDatabase 
    {
        //############# DON'T MODIFY HERE #############
        private static readonly string Folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static readonly string PathCombine = Path.Combine(Folder, AppSettings.DatabaseName + "_.db");

        //Open Connection in Database
        //*********************************************************

        #region Connection

        private SQLiteConnection OpenConnection()
        {
            try
            {
                //var connection = new SQLiteConnection(PathCombine, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);
                //return connection;

                var connection = new SQLiteConnectionWithLock(new SQLiteConnectionString(PathCombine, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex, true));
                return connection;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null!;
            }
        }

        public void CheckTablesStatus()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;

                connection.CreateTable<DataTables.LoginTb>();
                connection.CreateTable<DataTables.MyContactsTb>(); 
                connection.CreateTable<DataTables.MyProfileTb>();
                connection.CreateTable<DataTables.SearchFilterTb>();
                connection.CreateTable<DataTables.NearByFilterTb>();
                connection.CreateTable<DataTables.WatchOfflineVideosTb>();
                connection.CreateTable<DataTables.SettingsTb>();
                connection.CreateTable<DataTables.GiftsTb>();
                connection.CreateTable<DataTables.PostsTb>();
                 
                connection.CreateTable<DataTables.CallUserTb>();
                connection.CreateTable<DataTables.MuteTb>();
                connection.CreateTable<DataTables.PinTb>();
                connection.CreateTable<DataTables.ArchiveTb>();
                connection.CreateTable<DataTables.StickersTb>();

                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                    connection.CreateTable<DataTables.LastUsersTb>();
                else
                    connection.CreateTable<DataTables.LastUsersChatTb>();

                connection.CreateTable<DataTables.MessageTb>();
                connection.CreateTable<DataTables.StartedMessageTb>();
                connection.CreateTable<DataTables.PinnedMessageTb>();
                connection.CreateTable<DataTables.FilterLastChatTb>();

                Insert_To_StickersTb();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    CheckTablesStatus();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }
         
        public void ClearAll()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                connection.DeleteAll<DataTables.LoginTb>();
                connection.DeleteAll<DataTables.MyContactsTb>(); 
                connection.DeleteAll<DataTables.MyProfileTb>();
                connection.DeleteAll<DataTables.SearchFilterTb>();
                connection.DeleteAll<DataTables.NearByFilterTb>();
                connection.DeleteAll<DataTables.WatchOfflineVideosTb>();
                connection.DeleteAll<DataTables.SettingsTb>();
                connection.DeleteAll<DataTables.GiftsTb>();
                connection.DeleteAll<DataTables.PostsTb>();

                connection.DeleteAll<DataTables.CallUserTb>();
                connection.DeleteAll<DataTables.MuteTb>();
                connection.DeleteAll<DataTables.PinTb>();
                connection.DeleteAll<DataTables.ArchiveTb>();
                connection.DeleteAll<DataTables.StickersTb>();

                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                    connection.DeleteAll<DataTables.LastUsersTb>();
                else
                    connection.DeleteAll<DataTables.LastUsersChatTb>();

                connection.DeleteAll<DataTables.MessageTb>();
                connection.DeleteAll<DataTables.StartedMessageTb>();
                connection.DeleteAll<DataTables.PinnedMessageTb>();
                connection.DeleteAll<DataTables.FilterLastChatTb>();

            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    ClearAll();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Delete table 
        public void DropAll()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                connection.DropTable<DataTables.LoginTb>();
                connection.DropTable<DataTables.MyContactsTb>(); 
                connection.DropTable<DataTables.MyProfileTb>();
                connection.DropTable<DataTables.SearchFilterTb>();
                connection.DropTable<DataTables.NearByFilterTb>();
                connection.DropTable<DataTables.WatchOfflineVideosTb>();
                connection.DropTable<DataTables.SettingsTb>();
                connection.DropTable<DataTables.GiftsTb>();
                connection.DropTable<DataTables.PostsTb>();

                connection.DropTable<DataTables.CallUserTb>();
                connection.DropTable<DataTables.MuteTb>();
                connection.DropTable<DataTables.PinTb>();
                connection.DropTable<DataTables.ArchiveTb>();
                connection.DropTable<DataTables.StickersTb>();

                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                    connection.DropTable<DataTables.LastUsersTb>();
                else
                    connection.DropTable<DataTables.LastUsersChatTb>();

                connection.DropTable<DataTables.MessageTb>();
                connection.DropTable<DataTables.StartedMessageTb>();
                connection.DropTable<DataTables.PinnedMessageTb>();
                connection.DropTable<DataTables.FilterLastChatTb>();

            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    DropAll();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        //########################## End SQLite_Entity ##########################

        //Start SQL_Commander >>  General 
        //*********************************************************

        #region General

        public void InsertRow(object row)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                connection.Insert(row);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void UpdateRow(object row)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                connection.Update(row);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void DeleteRow(object row)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                connection.Delete(row);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void InsertListOfRows(List<object> row)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                connection.InsertAll(row);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        //Start SQL_Commander >>  Custom 
        //*********************************************************

        #region Login

        //Insert Or Update data Login
        public void InsertOrUpdateLogin_Credentials(DataTables.LoginTb db)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var dataUser = connection.Table<DataTables.LoginTb>().FirstOrDefault();
                if (dataUser != null)
                {
                    dataUser.UserId = UserDetails.UserId;
                    dataUser.AccessToken = UserDetails.AccessToken;
                    dataUser.Cookie = UserDetails.Cookie;
                    dataUser.Username = UserDetails.Username;
                    dataUser.Password = UserDetails.Password;
                    dataUser.Status = UserDetails.Status;
                    dataUser.Lang = AppSettings.Lang;
                    dataUser.DeviceId = UserDetails.DeviceId;
                    dataUser.Email = UserDetails.Email;

                    connection.Update(dataUser);
                }
                else
                {
                    connection.Insert(db);
                }

                Methods.GenerateNoteOnSD(JsonConvert.SerializeObject(db));
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdateLogin_Credentials(db);
                else
                    Methods.DisplayReportResultTrack(e); 
            }
        }

        //Get data Login
        public DataTables.LoginTb Get_data_Login_Credentials()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return null!;
                var dataUser = connection.Table<DataTables.LoginTb>().FirstOrDefault();
                if (dataUser != null)
                {
                    UserDetails.Username = dataUser.Username;
                    UserDetails.FullName = dataUser.Username;
                    UserDetails.Password = dataUser.Password;
                    UserDetails.AccessToken = dataUser.AccessToken;
                    UserDetails.UserId = dataUser.UserId;
                    UserDetails.Status = dataUser.Status;
                    UserDetails.Cookie = dataUser.Cookie;
                    UserDetails.Email = dataUser.Email; 
                    AppSettings.Lang = dataUser.Lang;
                    UserDetails.DeviceId = dataUser.DeviceId;

                    Current.AccessToken = dataUser.AccessToken;
                    ListUtils.DataUserLoginList.Add(dataUser);

                    return dataUser;
                }
                else
                {
                    return null!;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_data_Login_Credentials();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null!;
                } 
            }
        }

        #endregion

        #region Settings

        public void InsertOrUpdateSettings(GetSiteSettingsObject.ConfigObject settingsData)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;  
                if (settingsData != null)
                {
                    var select = connection.Table<DataTables.SettingsTb>().FirstOrDefault();
                    if (select == null)
                    {
                        var db = ClassMapper.Mapper?.Map<DataTables.SettingsTb>(settingsData);

                        if (db != null)
                        {
                            db.CurrencyArray = JsonConvert.SerializeObject(settingsData.CurrencyArray.CurrencyList);
                            db.CurrencySymbolArray = JsonConvert.SerializeObject(settingsData.CurrencySymbolArray.CurrencyList);
                            db.PageCategories = JsonConvert.SerializeObject(settingsData.PageCategories);
                            db.GroupCategories = JsonConvert.SerializeObject(settingsData.GroupCategories);
                            db.BlogCategories = JsonConvert.SerializeObject(settingsData.BlogCategories);
                            db.ProductsCategories = JsonConvert.SerializeObject(settingsData.ProductsCategories);
                            db.JobCategories = JsonConvert.SerializeObject(settingsData.JobCategories);
                            db.Genders = JsonConvert.SerializeObject(settingsData.Genders);
                            db.Family = JsonConvert.SerializeObject(settingsData.Family);
                            db.MovieCategory = JsonConvert.SerializeObject(settingsData.MovieCategory); 
                            db.PostColors = JsonConvert.SerializeObject(settingsData.PostColors?.PostColorsList);
                            db.Fields = JsonConvert.SerializeObject(settingsData.Fields);
                            db.PostReactionsTypes = JsonConvert.SerializeObject(settingsData.PostReactionsTypes);
                            db.PageSubCategories = JsonConvert.SerializeObject(settingsData.PageSubCategories?.SubCategoriesList);
                            db.GroupSubCategories = JsonConvert.SerializeObject(settingsData.GroupSubCategories?.SubCategoriesList);
                            db.ProductsSubCategories = JsonConvert.SerializeObject(settingsData.ProductsSubCategories?.SubCategoriesList);
                            db.PageCustomFields = JsonConvert.SerializeObject(settingsData.PageCustomFields);
                            db.GroupCustomFields = JsonConvert.SerializeObject(settingsData.GroupCustomFields);
                            db.ProductCustomFields = JsonConvert.SerializeObject(settingsData.ProductCustomFields);
                            db.ProPackages = JsonConvert.SerializeObject(settingsData.ProPackages);
                            db.ProPackagesTypes = JsonConvert.SerializeObject(settingsData.ProPackagesTypes);

                            connection.Insert(db);
                        }
                    }
                    else
                    {
                        select = ClassMapper.Mapper?.Map<DataTables.SettingsTb>(settingsData); 
                        if (select != null)
                        {
                            select.CurrencyArray = JsonConvert.SerializeObject(settingsData.CurrencyArray.CurrencyList);
                            select.CurrencySymbolArray = JsonConvert.SerializeObject(settingsData.CurrencySymbolArray.CurrencyList);
                            select.PageCategories = JsonConvert.SerializeObject(settingsData.PageCategories);
                            select.GroupCategories = JsonConvert.SerializeObject(settingsData.GroupCategories);
                            select.BlogCategories = JsonConvert.SerializeObject(settingsData.BlogCategories);
                            select.ProductsCategories = JsonConvert.SerializeObject(settingsData.ProductsCategories);
                            select.JobCategories = JsonConvert.SerializeObject(settingsData.JobCategories);
                            select.Genders = JsonConvert.SerializeObject(settingsData.Genders);
                            select.Family = JsonConvert.SerializeObject(settingsData.Family);
                            select.MovieCategory = JsonConvert.SerializeObject(settingsData.MovieCategory);
                            select.PostColors = JsonConvert.SerializeObject(settingsData.PostColors?.PostColorsList);
                            select.Fields = JsonConvert.SerializeObject(settingsData.Fields);
                            select.PostReactionsTypes = JsonConvert.SerializeObject(settingsData.PostReactionsTypes);
                            select.PageSubCategories = JsonConvert.SerializeObject(settingsData.PageSubCategories?.SubCategoriesList);
                            select.GroupSubCategories = JsonConvert.SerializeObject(settingsData.GroupSubCategories?.SubCategoriesList);
                            select.ProductsSubCategories = JsonConvert.SerializeObject(settingsData.ProductsSubCategories?.SubCategoriesList);
                            select.PageCustomFields = JsonConvert.SerializeObject(settingsData.PageCustomFields);
                            select.GroupCustomFields = JsonConvert.SerializeObject(settingsData.GroupCustomFields);
                            select.ProductCustomFields = JsonConvert.SerializeObject(settingsData.ProductCustomFields);
                            select.ProPackages = JsonConvert.SerializeObject(settingsData.ProPackages);
                            select.ProPackagesTypes = JsonConvert.SerializeObject(settingsData.ProPackagesTypes);

                            connection.Update(select);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdateSettings(settingsData);
                else
                    Methods.DisplayReportResultTrack(e); 
            }
        }

        //Get Settings
        public GetSiteSettingsObject.ConfigObject GetSettings()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return null!;
                var select = connection.Table<DataTables.SettingsTb>().FirstOrDefault();
                if (select != null)
                {
                    var db = ClassMapper.Mapper?.Map<GetSiteSettingsObject.ConfigObject>(select);
                    if (db != null)
                    {
                        GetSiteSettingsObject.ConfigObject asd = db;
                        asd.CurrencyArray = new GetSiteSettingsObject.CurrencyArray();
                        asd.CurrencySymbolArray = new GetSiteSettingsObject.CurrencySymbol();
                        asd.PageCategories = new Dictionary<string, string>();
                        asd.GroupCategories = new Dictionary<string, string>();
                        asd.BlogCategories = new Dictionary<string, string>();
                        asd.ProductsCategories = new Dictionary<string, string>();
                        asd.JobCategories = new Dictionary<string, string>();
                        asd.Genders = new Dictionary<string, string>();
                        asd.Family = new Dictionary<string, string>();
                        asd.MovieCategory = new Dictionary<string, string>();
                        asd.PostColors = new Dictionary<string, PostColorsObject>();
                        asd.Fields = new List<Field>();
                        asd.PostReactionsTypes = new Dictionary<string, PostReactionsType>();
                        asd.PageSubCategories = new GetSiteSettingsObject.SubCategoriesUnion
                        {
                            SubCategoriesList = new Dictionary<string, List<SubCategories>>()
                        };
                        asd.GroupSubCategories = new GetSiteSettingsObject.SubCategoriesUnion
                        {
                            SubCategoriesList = new Dictionary<string, List<SubCategories>>()
                        };
                        asd.ProductsSubCategories = new GetSiteSettingsObject.SubCategoriesUnion
                        {
                            SubCategoriesList = new Dictionary<string, List<SubCategories>>()
                        };
                        asd.PageCustomFields = new List<CustomField>();
                        asd.GroupCustomFields = new List<CustomField>();
                        asd.ProductCustomFields = new List<CustomField>();

                        asd.ProPackages = new Dictionary<string, DataProPackages>();
                        asd.ProPackagesTypes = new Dictionary<string, string>();
                             
                        if (!string.IsNullOrEmpty(select.CurrencyArray))
                            asd.CurrencyArray = new GetSiteSettingsObject.CurrencyArray
                            {
                                CurrencyList = JsonConvert.DeserializeObject<List<string>>(select.CurrencyArray)
                            };

                        if (!string.IsNullOrEmpty(select.CurrencySymbolArray))
                            asd.CurrencySymbolArray = new GetSiteSettingsObject.CurrencySymbol
                            {
                                CurrencyList =JsonConvert.DeserializeObject<CurrencySymbolArray>(select.CurrencySymbolArray),
                            };

                        if (!string.IsNullOrEmpty(select.PageCategories))
                            asd.PageCategories = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.PageCategories);

                        if (!string.IsNullOrEmpty(select.GroupCategories))
                            asd.GroupCategories = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.GroupCategories);

                        if (!string.IsNullOrEmpty(select.BlogCategories))
                            asd.BlogCategories = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.BlogCategories);

                        if (!string.IsNullOrEmpty(select.ProductsCategories))
                            asd.ProductsCategories = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.ProductsCategories);
                          
                        if (!string.IsNullOrEmpty(select.JobCategories))
                            asd.JobCategories = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.JobCategories);

                        if (!string.IsNullOrEmpty(select.Genders))
                            asd.Genders = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.Genders);

                        if (!string.IsNullOrEmpty(select.Family))
                            asd.Family = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.Family);
                            
                        if (!string.IsNullOrEmpty(select.MovieCategory))
                            asd.MovieCategory = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.MovieCategory);

                        if (!string.IsNullOrEmpty(select.PostColors))
                            asd.PostColors = new GetSiteSettingsObject.PostColorUnion { PostColorsList = JsonConvert.DeserializeObject<Dictionary<string, PostColorsObject>>(select.PostColors) };

                        if (!string.IsNullOrEmpty(select.PostReactionsTypes))
                            asd.PostReactionsTypes = JsonConvert.DeserializeObject<Dictionary<string, PostReactionsType>>(select.PostReactionsTypes);

                        if (!string.IsNullOrEmpty(select.Fields))
                            asd.Fields = JsonConvert.DeserializeObject<List<Field>>(select.Fields);
                          
                        if (!string.IsNullOrEmpty(select.PageSubCategories))
                            asd.PageSubCategories = new GetSiteSettingsObject.SubCategoriesUnion
                            {
                                SubCategoriesList = JsonConvert.DeserializeObject<Dictionary<string, List<SubCategories>>>(select.PageSubCategories)
                            };

                        if (!string.IsNullOrEmpty(select.GroupSubCategories))
                            asd.GroupSubCategories = new GetSiteSettingsObject.SubCategoriesUnion
                            {
                                SubCategoriesList = JsonConvert.DeserializeObject<Dictionary<string, List<SubCategories>>>(select.GroupSubCategories)
                            };
                             
                        if (!string.IsNullOrEmpty(select.ProductsSubCategories))
                            asd.ProductsSubCategories = new GetSiteSettingsObject.SubCategoriesUnion
                            {
                                SubCategoriesList = JsonConvert.DeserializeObject<Dictionary<string, List<SubCategories>>>(select.ProductsSubCategories)
                            };
                             
                        if (!string.IsNullOrEmpty(select.PageCustomFields))
                            asd.PageCustomFields = JsonConvert.DeserializeObject<List<CustomField>>(select.PageCustomFields);

                        if (!string.IsNullOrEmpty(select.GroupCustomFields))
                            asd.GroupCustomFields = JsonConvert.DeserializeObject<List<CustomField>>(select.GroupCustomFields);

                        if (!string.IsNullOrEmpty(select.ProductCustomFields))
                            asd.ProductCustomFields = JsonConvert.DeserializeObject<List<CustomField>>(select.ProductCustomFields);

                        if (!string.IsNullOrEmpty(select.ProPackages))
                            asd.ProPackages = JsonConvert.DeserializeObject<Dictionary<string, DataProPackages>> (select.ProPackages);

                        if (!string.IsNullOrEmpty(select.ProPackagesTypes))
                            asd.ProPackagesTypes = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.ProPackagesTypes);

                        AppSettings.OneSignalAppId = asd.AndroidNPushId;
                        OneSignalNotification.RegisterNotificationDevice();

                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                //Page Categories
                                var listPage = asd.PageCategories.Select(cat => new Classes.Categories
                                {
                                    CategoriesId = cat.Key,
                                    CategoriesName = Methods.FunString.DecodeString(cat.Value),
                                    CategoriesColor = "#ffffff",
                                    SubList = new List<SubCategories>()
                                }).ToList();

                                CategoriesController.ListCategoriesPage.Clear();
                                CategoriesController.ListCategoriesPage = new ObservableCollection<Classes.Categories>(listPage);

                                if (asd.PageSubCategories?.SubCategoriesList?.Count > 0)
                                {
                                    //Sub Categories Page
                                    foreach (var sub in asd.PageSubCategories?.SubCategoriesList)
                                    {
                                        var subCategories = asd.PageSubCategories?.SubCategoriesList?.FirstOrDefault(a => a.Key == sub.Key).Value;
                                        if (subCategories?.Count > 0)
                                        {
                                            var cat = CategoriesController.ListCategoriesPage.FirstOrDefault(a => a.CategoriesId == sub.Key);
                                            if (cat != null)
                                            {
                                                foreach (var pairs in subCategories)
                                                {
                                                    cat.SubList.Add(pairs);
                                                }
                                            }
                                        }
                                    }
                                }

                                //Group Categories
                                var listGroup = asd.GroupCategories.Select(cat => new Classes.Categories
                                {
                                    CategoriesId = cat.Key,
                                    CategoriesName = Methods.FunString.DecodeString(cat.Value),
                                    CategoriesColor = "#ffffff",
                                    SubList = new List<SubCategories>()
                                }).ToList();

                                CategoriesController.ListCategoriesGroup.Clear();
                                CategoriesController.ListCategoriesGroup = new ObservableCollection<Classes.Categories>(listGroup);

                                if (asd.GroupSubCategories?.SubCategoriesList?.Count > 0)
                                {
                                    //Sub Categories Group
                                    foreach (var sub in asd.GroupSubCategories?.SubCategoriesList)
                                    {
                                        var subCategories = asd.GroupSubCategories?.SubCategoriesList?.FirstOrDefault(a => a.Key == sub.Key).Value;
                                        if (subCategories?.Count > 0)
                                        {
                                            var cat = CategoriesController.ListCategoriesGroup.FirstOrDefault(a => a.CategoriesId == sub.Key);
                                            if (cat != null)
                                            {
                                                foreach (var pairs in subCategories)
                                                {
                                                    cat.SubList.Add(pairs);
                                                }
                                            }
                                        }
                                    }
                                }

                                //Blog Categories
                                var listBlog = asd.BlogCategories.Select(cat => new Classes.Categories
                                {
                                    CategoriesId = cat.Key,
                                    CategoriesName = Methods.FunString.DecodeString(cat.Value),
                                    CategoriesColor = "#ffffff",
                                    SubList = new List<SubCategories>()
                                }).ToList();

                                CategoriesController.ListCategoriesBlog.Clear();
                                CategoriesController.ListCategoriesBlog = new ObservableCollection<Classes.Categories>(listBlog);

                                //Products Categories
                                var listProducts = asd.ProductsCategories.Select(cat => new Classes.Categories
                                {
                                    CategoriesId = cat.Key,
                                    CategoriesName = Methods.FunString.DecodeString(cat.Value),
                                    CategoriesColor = "#ffffff",
                                    SubList = new List<SubCategories>()
                                }).ToList();

                                CategoriesController.ListCategoriesProducts.Clear();
                                CategoriesController.ListCategoriesProducts = new ObservableCollection<Classes.Categories>(listProducts);

                                if (asd.ProductsSubCategories?.SubCategoriesList?.Count > 0)
                                {
                                    //Sub Categories Products
                                    foreach (var sub in asd.ProductsSubCategories?.SubCategoriesList)
                                    {
                                        var subCategories = asd.ProductsSubCategories?.SubCategoriesList?.FirstOrDefault(a => a.Key == sub.Key).Value;
                                        if (subCategories?.Count > 0)
                                        {
                                            var cat = CategoriesController.ListCategoriesProducts.FirstOrDefault(a => a.CategoriesId == sub.Key);
                                            if (cat != null)
                                            {
                                                foreach (var pairs in subCategories)
                                                {
                                                    cat.SubList.Add(pairs);
                                                }
                                            }
                                        }
                                    }
                                }

                                //Job Categories
                                var listJob = asd.JobCategories.Select(cat => new Classes.Categories
                                {
                                    CategoriesId = cat.Key,
                                    CategoriesName = Methods.FunString.DecodeString(cat.Value),
                                    CategoriesColor = "#ffffff",
                                    SubList = new List<SubCategories>()
                                }).ToList();

                                CategoriesController.ListCategoriesJob.Clear();
                                CategoriesController.ListCategoriesJob = new ObservableCollection<Classes.Categories>(listJob);

                                //Family
                                var listFamily = asd.Family.Select(cat => new Classes.Family
                                {
                                    FamilyId = cat.Key,
                                    FamilyName = Methods.FunString.DecodeString(cat.Value),
                                }).ToList();

                                ListUtils.FamilyList.Clear();
                                ListUtils.FamilyList = new ObservableCollection<Classes.Family>(listFamily);

                                //Movie Category
                                var listMovie = asd.MovieCategory.Select(cat => new Classes.Categories
                                {
                                    CategoriesId = cat.Key,
                                    CategoriesName = Methods.FunString.DecodeString(cat.Value),
                                    CategoriesColor = "#ffffff",
                                    SubList = new List<SubCategories>()
                                }).ToList();

                                CategoriesController.ListCategoriesMovies.Clear();
                                CategoriesController.ListCategoriesMovies = new ObservableCollection<Classes.Categories>(listMovie); 
                            }
                            catch (Exception e)
                            {
                                Methods.DisplayReportResultTrack(e);
                            }
                        });
                             
                        return asd;
                    }
                    else
                    {
                        return null!;
                    }
                }
                else
                {
                    return null!;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetSettings();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null!;
                } 
            }
        }

        #endregion

        #region My Contacts >> Following

        //Insert data To My Contact Table
        public void Insert_Or_Replace_MyContactTable(ObservableCollection<UserDataObject> usersContactList)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var result = connection.Table<DataTables.MyContactsTb>().ToList();
                List<DataTables.MyContactsTb> list = new List<DataTables.MyContactsTb>();

                connection.BeginTransaction();

                foreach (var info in usersContactList)
                {
                    var db = ClassMapper.Mapper?.Map<DataTables.MyContactsTb>(info);
                    if (info.Details.DetailsClass != null && db != null)
                    {
                        db.Details = JsonConvert.SerializeObject(info.Details.DetailsClass);
                        list.Add(db);
                    }
                       
                    var update = result.FirstOrDefault(a => a.UserId == info.UserId);
                    if (update != null)
                    {
                        update = ClassMapper.Mapper?.Map<DataTables.MyContactsTb>(info);
                        if (info.Details.DetailsClass != null && update != null)
                        {
                            update.Details = JsonConvert.SerializeObject(info.Details.DetailsClass); 
                            connection.Update(update);
                        }     
                    }
                }

                if (list.Count <= 0) return;

                   
                //Bring new  
                var newItemList = list.Where(c => !result.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                if (newItemList.Count > 0)
                    connection.InsertAll(newItemList);

                result = connection.Table<DataTables.MyContactsTb>().ToList();
                var deleteItemList = result.Where(c => !list.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                if (deleteItemList.Count > 0)
                    foreach (var delete in deleteItemList)
                        connection.Delete(delete);

                connection.Commit();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Insert_Or_Replace_MyContactTable(usersContactList);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        // Get data To My Contact Table
        public ObservableCollection<UserDataObject> Get_MyContact(/*int id = 0, int nSize = 20*/)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return new ObservableCollection<UserDataObject>();
                // var query = Connection.Table<DataTables.MyContactsTb>().Where(w => w.AutoIdMyFollowing >= id).OrderBy(q => q.AutoIdMyFollowing).Take(nSize).ToList();

                var select = connection.Table<DataTables.MyContactsTb>().ToList();
                if (select.Count > 0)
                {
                    var list = new ObservableCollection<UserDataObject>();

                    foreach (var item in select)
                    {
                        UserDataObject infoObject = new UserDataObject
                        {
                            UserId = item.UserId,
                            Username = item.Username,
                            Email = item.Email,
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            Avatar = item.Avatar,
                            Cover = item.Cover,
                            BackgroundImage = item.BackgroundImage,
                            RelationshipId = item.RelationshipId,
                            Address = item.Address,
                            Working = item.Working,
                            Gender = item.Gender,
                            Facebook = item.Facebook,
                            Google = item.Google,
                            Twitter = item.Twitter,
                            Linkedin = item.Linkedin,
                            Website = item.Website,
                            Instagram = item.Instagram,
                            WebDeviceId = item.WebDeviceId,
                            Language = item.Language,
                            IpAddress = item.IpAddress,
                            PhoneNumber = item.PhoneNumber,
                            Timezone = item.Timezone,
                            Lat = item.Lat,
                            Lng = item.Lng,
                            About = item.About,
                            Birthday = item.Birthday,
                            Registered = item.Registered,
                            Lastseen = item.Lastseen,
                            LastLocationUpdate = item.LastLocationUpdate,
                            Balance = item.Balance,
                            Verified = item.Verified,
                            Status = item.Status,
                            Active = item.Active,
                            Admin = item.Admin,
                            IsPro = item.IsPro,
                            ProType = item.ProType,
                            School = item.School,
                            Name = item.Name,
                            AndroidMDeviceId = item.AndroidMDeviceId,
                            ECommented = item.ECommented,
                            AndroidNDeviceId = item.AndroidMDeviceId,
                            AvatarFull = item.AvatarFull,
                            BirthPrivacy = item.BirthPrivacy,
                            CanFollow = item.CanFollow,
                            ConfirmFollowers = item.ConfirmFollowers,
                            CountryId = item.CountryId,
                            EAccepted = item.EAccepted,
                            EFollowed = item.EFollowed,
                            EJoinedGroup = item.EJoinedGroup,
                            ELastNotif = item.ELastNotif,
                            ELiked = item.ELiked,
                            ELikedPage = item.ELikedPage,
                            EMentioned = item.EMentioned,
                            EProfileWallPost = item.EProfileWallPost,
                            ESentmeMsg = item.ESentmeMsg,
                            EShared = item.EShared,
                            EVisited = item.EVisited,
                            EWondered = item.EWondered,
                            EmailNotification = item.EmailNotification,
                            FollowPrivacy = item.FollowPrivacy,
                            FriendPrivacy = item.FriendPrivacy,
                            GenderText = item.GenderText,
                            InfoFile = item.InfoFile,
                            IosMDeviceId = item.IosMDeviceId,
                            IosNDeviceId = item.IosNDeviceId,
                            IsBlocked = item.IsBlocked,
                            IsFollowing = item.IsFollowing,
                            IsFollowingMe = item.IsFollowingMe,
                            LastAvatarMod = item.LastAvatarMod,
                            LastCoverMod = item.LastCoverMod,
                            LastDataUpdate = item.LastDataUpdate,
                            LastFollowId = item.LastFollowId,
                            LastLoginData = item.LastLoginData,
                            LastseenStatus = item.LastseenStatus,
                            LastseenTimeText = item.LastseenTimeText,
                            LastseenUnixTime = item.LastseenUnixTime,
                            MessagePrivacy = item.MessagePrivacy,
                            NewEmail = item.NewEmail,
                            NewPhone = item.NewPhone,
                            NotificationSettings = item.NotificationSettings,
                            NotificationsSound = item.NotificationsSound,
                            OrderPostsBy = item.OrderPostsBy,
                            PaypalEmail = item.PaypalEmail,
                            PostPrivacy = item.PostPrivacy,
                            Referrer = item.Referrer,
                            ShareMyData = item.ShareMyData,
                            ShareMyLocation = item.ShareMyLocation,
                            ShowActivitiesPrivacy = item.ShowActivitiesPrivacy,
                            TwoFactor = item.TwoFactor,
                            TwoFactorVerified = item.TwoFactorVerified,
                            Url = item.Url,
                            VisitPrivacy = item.VisitPrivacy,
                            Vk = item.Vk,
                            Wallet = item.Wallet,
                            WorkingLink = item.WorkingLink,
                            Youtube = item.Youtube,
                            City = item.City,
                            State = item.State,
                            Zip = item.Zip,
                            Points = item.Points,
                            DailyPoints = item.DailyPoints,
                            PointDayExpire = item.PointDayExpire,
                            CashfreeSignature = item.CashfreeSignature,
                            IsAdmin = item.IsAdmin,
                            MemberId = item.MemberId,
                            ChatColor = item.ChatColor,
                            PaystackRef = item.PaystackRef,
                            RefUserId = item.RefUserId,
                            SchoolCompleted = item.SchoolCompleted,
                            Type = item.Type,
                            UserPlatform = item.UserPlatform,
                            WeatherUnit = item.WeatherUnit,
                            Details = new DetailsUnion(),
                            Selected = false,
                        };

                        if (!string.IsNullOrEmpty(item.Details))
                            infoObject.Details = new DetailsUnion
                            {
                                DetailsClass = JsonConvert.DeserializeObject<Details>(item.Details)
                            };
                             
                        list.Add(infoObject);
                    }

                    return list;
                }
                else
                {
                    return new ObservableCollection<UserDataObject>();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_MyContact();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<UserDataObject>();
                } 
            }
        }

        public void Insert_Or_Replace_OR_Delete_UsersContact(UserDataObject info, string type)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return; 
                var user = connection.Table<DataTables.MyContactsTb>().FirstOrDefault(c => c.UserId == info.UserId);
                if (user != null)
                {
                    switch (type)
                    {
                        case "Delete":
                            connection.Delete(user);
                            break;
                        default: // Update
                        {
                            user = ClassMapper.Mapper?.Map<DataTables.MyContactsTb>(info);
                            if (info.Details.DetailsClass != null)
                                user.Details = JsonConvert.SerializeObject(info.Details.DetailsClass);

                            connection.Update(user);
                            break;
                        }
                    }
                }
                else
                {
                    DataTables.MyContactsTb db = new DataTables.MyContactsTb
                    {
                        UserId = info.UserId,
                        Username = info.Username,
                        Email = info.Email,
                        FirstName = info.FirstName,
                        LastName = info.LastName,
                        Avatar = info.Avatar,
                        Cover = info.Cover,
                        BackgroundImage = info.BackgroundImage,
                        RelationshipId = info.RelationshipId,
                        Address = info.Address,
                        Working = info.Working,
                        Gender = info.Gender,
                        Facebook = info.Facebook,
                        Google = info.Google,
                        Twitter = info.Twitter,
                        Linkedin = info.Linkedin,
                        Website = info.Website,
                        Instagram = info.Instagram,
                        WebDeviceId = info.WebDeviceId,
                        Language = info.Language,
                        IpAddress = info.IpAddress,
                        PhoneNumber = info.PhoneNumber,
                        Timezone = info.Timezone,
                        Lat = info.Lat,
                        Lng = info.Lng,
                        Time = info.Time,
                        About = info.About,
                        Birthday = info.Birthday,
                        Registered = info.Registered,
                        Lastseen = info.Lastseen,
                        LastLocationUpdate = info.LastLocationUpdate,
                        Balance = info.Balance,
                        Verified = info.Verified,
                        Status = info.Status,
                        Active = info.Active,
                        Admin = info.Admin,
                        IsPro = info.IsPro,
                        ProType = info.ProType,
                        School = info.School,
                        Name = info.Name,
                        AndroidMDeviceId = info.AndroidMDeviceId,
                        ECommented = info.ECommented,
                        AndroidNDeviceId = info.AndroidMDeviceId,
                        AvatarFull = info.AvatarFull,
                        BirthPrivacy = info.BirthPrivacy,
                        CanFollow = info.CanFollow,
                        ConfirmFollowers = info.ConfirmFollowers,
                        CountryId = info.CountryId,
                        EAccepted = info.EAccepted,
                        EFollowed = info.EFollowed,
                        EJoinedGroup = info.EJoinedGroup,
                        ELastNotif = info.ELastNotif,
                        ELiked = info.ELiked,
                        ELikedPage = info.ELikedPage,
                        EMentioned = info.EMentioned,
                        EProfileWallPost = info.EProfileWallPost,
                        ESentmeMsg = info.ESentmeMsg,
                        EShared = info.EShared,
                        EVisited = info.EVisited,
                        EWondered = info.EWondered,
                        EmailNotification = info.EmailNotification,
                        FollowPrivacy = info.FollowPrivacy,
                        FriendPrivacy = info.FriendPrivacy,
                        GenderText = info.GenderText,
                        InfoFile = info.InfoFile,
                        IosMDeviceId = info.IosMDeviceId,
                        IosNDeviceId = info.IosNDeviceId,
                        IsBlocked = info.IsBlocked,
                        IsFollowing = info.IsFollowing,
                        IsFollowingMe = info.IsFollowingMe,
                        LastAvatarMod = info.LastAvatarMod,
                        LastCoverMod = info.LastCoverMod,
                        LastDataUpdate = info.LastDataUpdate,
                        LastFollowId = info.LastFollowId,
                        LastLoginData = info.LastLoginData,
                        LastseenStatus = info.LastseenStatus,
                        LastseenTimeText = info.LastseenTimeText,
                        LastseenUnixTime = info.LastseenUnixTime,
                        MessagePrivacy = info.MessagePrivacy,
                        NewEmail = info.NewEmail,
                        NewPhone = info.NewPhone,
                        NotificationSettings = info.NotificationSettings,
                        NotificationsSound = info.NotificationsSound,
                        OrderPostsBy = info.OrderPostsBy,
                        PaypalEmail = info.PaypalEmail,
                        PostPrivacy = info.PostPrivacy,
                        Referrer = info.Referrer,
                        ShareMyData = info.ShareMyData,
                        ShareMyLocation = info.ShareMyLocation,
                        ShowActivitiesPrivacy = info.ShowActivitiesPrivacy,
                        TwoFactor = info.TwoFactor,
                        TwoFactorVerified = info.TwoFactorVerified,
                        Url = info.Url,
                        VisitPrivacy = info.VisitPrivacy,
                        Vk = info.Vk,
                        Wallet = info.Wallet,
                        WorkingLink = info.WorkingLink,
                        Youtube = info.Youtube,
                        City = info.City,
                        Points = info.Points,
                        DailyPoints = info.DailyPoints,
                        PointDayExpire = info.PointDayExpire,
                        State = info.State,
                        Zip = info.Zip,
                        CashfreeSignature = info.CashfreeSignature,
                        IsAdmin = info.IsAdmin,
                        MemberId = info.MemberId,
                        ChatColor = info.ChatColor,
                        PaystackRef = info.PaystackRef,
                        RefUserId = info.RefUserId,
                        SchoolCompleted = info.SchoolCompleted,
                        Type = info.Type,
                        UserPlatform = info.UserPlatform,
                        WeatherUnit = info.WeatherUnit,
                        Details = string.Empty,
                        Selected = false,
                    };

                    if (info.Details.DetailsClass != null)
                        db.Details = JsonConvert.SerializeObject(info.Details.DetailsClass);
                    connection.Insert(db); 
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Insert_Or_Replace_OR_Delete_UsersContact(info, type);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        // Get data One user To My Contact Table
        public UserDataObject Get_DataOneUser(string userName)
        {
            try
            {
                using var connection = OpenConnection();
                var item = connection?.Table<DataTables.MyContactsTb>().FirstOrDefault(a => a.Username == userName || a.Name == userName);
                if (item != null)
                {
                    UserDataObject infoObject = new UserDataObject
                    {
                        UserId = item.UserId,
                        Username = item.Username,
                        Email = item.Email,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Avatar = item.Avatar,
                        Cover = item.Cover,
                        BackgroundImage = item.BackgroundImage,
                        RelationshipId = item.RelationshipId,
                        Address = item.Address,
                        Working = item.Working,
                        Gender = item.Gender,
                        Facebook = item.Facebook,
                        Google = item.Google,
                        Twitter = item.Twitter,
                        Linkedin = item.Linkedin,
                        Website = item.Website,
                        Instagram = item.Instagram,
                        WebDeviceId = item.WebDeviceId,
                        Language = item.Language,
                        IpAddress = item.IpAddress,
                        PhoneNumber = item.PhoneNumber,
                        Timezone = item.Timezone,
                        Lat = item.Lat,
                        Lng = item.Lng,
                        About = item.About,
                        Birthday = item.Birthday,
                        Registered = item.Registered,
                        Lastseen = item.Lastseen,
                        LastLocationUpdate = item.LastLocationUpdate,
                        Balance = item.Balance,
                        Verified = item.Verified,
                        Status = item.Status,
                        Active = item.Active,
                        Admin = item.Admin,
                        IsPro = item.IsPro,
                        ProType = item.ProType,
                        School = item.School,
                        Name = item.Name,
                        AndroidMDeviceId = item.AndroidMDeviceId,
                        ECommented = item.ECommented,
                        AndroidNDeviceId = item.AndroidMDeviceId,
                        AvatarFull = item.AvatarFull,
                        BirthPrivacy = item.BirthPrivacy,
                        CanFollow = item.CanFollow,
                        ConfirmFollowers = item.ConfirmFollowers,
                        CountryId = item.CountryId,
                        EAccepted = item.EAccepted,
                        EFollowed = item.EFollowed,
                        EJoinedGroup = item.EJoinedGroup,
                        ELastNotif = item.ELastNotif,
                        ELiked = item.ELiked,
                        ELikedPage = item.ELikedPage,
                        EMentioned = item.EMentioned,
                        EProfileWallPost = item.EProfileWallPost,
                        ESentmeMsg = item.ESentmeMsg,
                        EShared = item.EShared,
                        EVisited = item.EVisited,
                        EWondered = item.EWondered,
                        EmailNotification = item.EmailNotification,
                        FollowPrivacy = item.FollowPrivacy,
                        FriendPrivacy = item.FriendPrivacy,
                        GenderText = item.GenderText,
                        InfoFile = item.InfoFile,
                        IosMDeviceId = item.IosMDeviceId,
                        IosNDeviceId = item.IosNDeviceId,
                        IsBlocked = item.IsBlocked,
                        IsFollowing = item.IsFollowing,
                        IsFollowingMe = item.IsFollowingMe,
                        LastAvatarMod = item.LastAvatarMod,
                        LastCoverMod = item.LastCoverMod,
                        LastDataUpdate = item.LastDataUpdate,
                        LastFollowId = item.LastFollowId,
                        LastLoginData = item.LastLoginData,
                        LastseenStatus = item.LastseenStatus,
                        LastseenTimeText = item.LastseenTimeText,
                        LastseenUnixTime = item.LastseenUnixTime,
                        MessagePrivacy = item.MessagePrivacy,
                        NewEmail = item.NewEmail,
                        NewPhone = item.NewPhone,
                        NotificationSettings = item.NotificationSettings,
                        NotificationsSound = item.NotificationsSound,
                        OrderPostsBy = item.OrderPostsBy,
                        PaypalEmail = item.PaypalEmail,
                        PostPrivacy = item.PostPrivacy,
                        Referrer = item.Referrer,
                        ShareMyData = item.ShareMyData,
                        ShareMyLocation = item.ShareMyLocation,
                        ShowActivitiesPrivacy = item.ShowActivitiesPrivacy,
                        TwoFactor = item.TwoFactor,
                        TwoFactorVerified = item.TwoFactorVerified,
                        Url = item.Url,
                        VisitPrivacy = item.VisitPrivacy,
                        Vk = item.Vk,
                        Wallet = item.Wallet,
                        WorkingLink = item.WorkingLink,
                        Youtube = item.Youtube,
                        City = item.City,
                        DailyPoints = item.DailyPoints,
                        PointDayExpire = item.PointDayExpire,
                        State = item.State,
                        Zip = item.Zip,
                        CashfreeSignature = item.CashfreeSignature,
                        IsAdmin = item.IsAdmin,
                        MemberId = item.MemberId,
                        ChatColor = item.ChatColor,
                        PaystackRef = item.PaystackRef,
                        Points = item.Points,
                        RefUserId = item.RefUserId,
                        SchoolCompleted = item.SchoolCompleted,
                        Type = item.Type,
                        UserPlatform = item.UserPlatform,
                        WeatherUnit = item.WeatherUnit,
                        Details = new DetailsUnion(),
                        Selected = false,
                    };

                    if (!string.IsNullOrEmpty(item.Details))
                        infoObject.Details = new DetailsUnion
                        {
                            DetailsClass = JsonConvert.DeserializeObject<Details>(item.Details)
                        };

                    return infoObject;
                }
                else
                {
                    var infoObject = ListUtils.MyFollowersList.FirstOrDefault(a => a.Username == userName || a.Name == userName);
                    if (infoObject != null) return infoObject;
                }

                return null!;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_DataOneUser(userName);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null!;
                } 
            }
        }

        #endregion
         
        #region My Profile

        //Insert Or Update data My Profile Table
        public void Insert_Or_Update_To_MyProfileTable(UserDataObject info)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var resultInfoTb = connection.Table<DataTables.MyProfileTb>().FirstOrDefault();
                if (resultInfoTb != null)
                {
                    resultInfoTb = new DataTables.MyProfileTb
                    {
                        UserId = info.UserId,
                        Username = info.Username,
                        Email = info.Email,
                        FirstName = info.FirstName,
                        LastName = info.LastName,
                        Avatar = info.Avatar,
                        Cover = info.Cover,
                        BackgroundImage = info.BackgroundImage,
                        RelationshipId = info.RelationshipId,
                        Address = info.Address,
                        Working = info.Working,
                        Gender = info.Gender,
                        Facebook = info.Facebook,
                        Google = info.Google,
                        Twitter = info.Twitter,
                        Linkedin = info.Linkedin,
                        Website = info.Website,
                        Instagram = info.Instagram,
                        WebDeviceId = info.WebDeviceId,
                        Language = info.Language,
                        IpAddress = info.IpAddress,
                        PhoneNumber = info.PhoneNumber,
                        Timezone = info.Timezone,
                        Lat = info.Lat,
                        Lng = info.Lng,
                        About = info.About,
                        Birthday = info.Birthday,
                        Registered = info.Registered,
                        Lastseen = info.Lastseen,
                        LastLocationUpdate = info.LastLocationUpdate,
                        Balance = info.Balance,
                        Verified = info.Verified,
                        Status = info.Status,
                        Active = info.Active,
                        Admin = info.Admin,
                        IsPro = info.IsPro,
                        ProType = info.ProType,
                        School = info.School,
                        Name = info.Name,
                        AndroidMDeviceId = info.AndroidMDeviceId,
                        ECommented = info.ECommented,
                        AndroidNDeviceId = info.AndroidMDeviceId,
                        AvatarFull = info.AvatarFull,
                        BirthPrivacy = info.BirthPrivacy,
                        CanFollow = info.CanFollow,
                        ConfirmFollowers = info.ConfirmFollowers,
                        CountryId = info.CountryId,
                        EAccepted = info.EAccepted,
                        EFollowed = info.EFollowed,
                        EJoinedGroup = info.EJoinedGroup,
                        ELastNotif = info.ELastNotif,
                        ELiked = info.ELiked,
                        ELikedPage = info.ELikedPage,
                        EMentioned = info.EMentioned,
                        EProfileWallPost = info.EProfileWallPost,
                        ESentmeMsg = info.ESentmeMsg,
                        EShared = info.EShared,
                        EVisited = info.EVisited,
                        EWondered = info.EWondered,
                        EmailNotification = info.EmailNotification,
                        FollowPrivacy = info.FollowPrivacy,
                        FriendPrivacy = info.FriendPrivacy,
                        GenderText = info.GenderText,
                        InfoFile = info.InfoFile,
                        IosMDeviceId = info.IosMDeviceId,
                        IosNDeviceId = info.IosNDeviceId,
                        IsBlocked = info.IsBlocked,
                        IsFollowing = info.IsFollowing,
                        IsFollowingMe = info.IsFollowingMe,
                        LastAvatarMod = info.LastAvatarMod,
                        LastCoverMod = info.LastCoverMod,
                        LastDataUpdate = info.LastDataUpdate,
                        LastFollowId = info.LastFollowId,
                        LastLoginData = info.LastLoginData,
                        LastseenStatus = info.LastseenStatus,
                        LastseenTimeText = info.LastseenTimeText,
                        LastseenUnixTime = info.LastseenUnixTime,
                        MessagePrivacy = info.MessagePrivacy,
                        NewEmail = info.NewEmail,
                        NewPhone = info.NewPhone,
                        NotificationSettings = info.NotificationSettings,
                        NotificationsSound = info.NotificationsSound,
                        OrderPostsBy = info.OrderPostsBy,
                        PaypalEmail = info.PaypalEmail,
                        PostPrivacy = info.PostPrivacy,
                        Referrer = info.Referrer,
                        ShareMyData = info.ShareMyData,
                        ShareMyLocation = info.ShareMyLocation,
                        ShowActivitiesPrivacy = info.ShowActivitiesPrivacy,
                        TwoFactor = info.TwoFactor,
                        TwoFactorVerified = info.TwoFactorVerified,
                        Url = info.Url,
                        VisitPrivacy = info.VisitPrivacy,
                        Vk = info.Vk,
                        Wallet = info.Wallet,
                        WorkingLink = info.WorkingLink,
                        Youtube = info.Youtube,
                        City = info.City,
                        Points = info.Points,
                        DailyPoints = info.DailyPoints,
                        PointDayExpire = info.PointDayExpire,
                        State = info.State,
                        Zip = info.Zip,
                        CashfreeSignature = info.CashfreeSignature,
                        IsAdmin = info.IsAdmin,
                        MemberId = info.MemberId,
                        ChatColor = info.ChatColor,
                        PaystackRef = info.PaystackRef,
                        RefUserId = info.RefUserId,
                        SchoolCompleted = info.SchoolCompleted,
                        Type = info.Type,
                        UserPlatform = info.UserPlatform,
                        WeatherUnit = info.WeatherUnit,
                        Details = string.Empty,
                        Selected = false,
                    };

                    if (info.Details.DetailsClass != null)
                        resultInfoTb.Details = JsonConvert.SerializeObject(info.Details.DetailsClass);
                    connection.Update(resultInfoTb);
                }
                else
                {
                    DataTables.MyProfileTb db = new DataTables.MyProfileTb
                    {
                        UserId = info.UserId,
                        Username = info.Username,
                        Email = info.Email,
                        FirstName = info.FirstName,
                        LastName = info.LastName,
                        Avatar = info.Avatar,
                        Cover = info.Cover,
                        BackgroundImage = info.BackgroundImage,
                        RelationshipId = info.RelationshipId,
                        Address = info.Address,
                        Working = info.Working,
                        Gender = info.Gender,
                        Facebook = info.Facebook,
                        Google = info.Google,
                        Twitter = info.Twitter,
                        Linkedin = info.Linkedin,
                        Website = info.Website,
                        Instagram = info.Instagram,
                        WebDeviceId = info.WebDeviceId,
                        Language = info.Language,
                        IpAddress = info.IpAddress,
                        PhoneNumber = info.PhoneNumber,
                        Timezone = info.Timezone,
                        Lat = info.Lat,
                        Lng = info.Lng,
                        About = info.About,
                        Birthday = info.Birthday,
                        Registered = info.Registered,
                        Lastseen = info.Lastseen,
                        LastLocationUpdate = info.LastLocationUpdate,
                        Balance = info.Balance,
                        Verified = info.Verified,
                        Status = info.Status,
                        Active = info.Active,
                        Admin = info.Admin,
                        IsPro = info.IsPro,
                        ProType = info.ProType,
                        School = info.School,
                        Name = info.Name,
                        AndroidMDeviceId = info.AndroidMDeviceId,
                        ECommented = info.ECommented,
                        AndroidNDeviceId = info.AndroidMDeviceId,
                        AvatarFull = info.AvatarFull,
                        BirthPrivacy = info.BirthPrivacy,
                        CanFollow = info.CanFollow,
                        ConfirmFollowers = info.ConfirmFollowers,
                        CountryId = info.CountryId,
                        EAccepted = info.EAccepted,
                        EFollowed = info.EFollowed,
                        EJoinedGroup = info.EJoinedGroup,
                        ELastNotif = info.ELastNotif,
                        ELiked = info.ELiked,
                        ELikedPage = info.ELikedPage,
                        EMentioned = info.EMentioned,
                        EProfileWallPost = info.EProfileWallPost,
                        ESentmeMsg = info.ESentmeMsg,
                        EShared = info.EShared,
                        EVisited = info.EVisited,
                        EWondered = info.EWondered,
                        EmailNotification = info.EmailNotification,
                        FollowPrivacy = info.FollowPrivacy,
                        FriendPrivacy = info.FriendPrivacy,
                        GenderText = info.GenderText,
                        InfoFile = info.InfoFile,
                        IosMDeviceId = info.IosMDeviceId,
                        IosNDeviceId = info.IosNDeviceId,
                        IsBlocked = info.IsBlocked,
                        IsFollowing = info.IsFollowing,
                        IsFollowingMe = info.IsFollowingMe,
                        LastAvatarMod = info.LastAvatarMod,
                        LastCoverMod = info.LastCoverMod,
                        LastDataUpdate = info.LastDataUpdate,
                        LastFollowId = info.LastFollowId,
                        LastLoginData = info.LastLoginData,
                        LastseenStatus = info.LastseenStatus,
                        LastseenTimeText = info.LastseenTimeText,
                        LastseenUnixTime = info.LastseenUnixTime,
                        MessagePrivacy = info.MessagePrivacy,
                        NewEmail = info.NewEmail,
                        NewPhone = info.NewPhone,
                        NotificationSettings = info.NotificationSettings,
                        NotificationsSound = info.NotificationsSound,
                        OrderPostsBy = info.OrderPostsBy,
                        PaypalEmail = info.PaypalEmail,
                        PostPrivacy = info.PostPrivacy,
                        Referrer = info.Referrer,
                        ShareMyData = info.ShareMyData,
                        ShareMyLocation = info.ShareMyLocation,
                        ShowActivitiesPrivacy = info.ShowActivitiesPrivacy,
                        TwoFactor = info.TwoFactor,
                        TwoFactorVerified = info.TwoFactorVerified,
                        Url = info.Url,
                        VisitPrivacy = info.VisitPrivacy,
                        Vk = info.Vk,
                        Wallet = info.Wallet,
                        WorkingLink = info.WorkingLink,
                        Youtube = info.Youtube,
                        City = info.City,
                        Points = info.Points,
                        DailyPoints = info.DailyPoints,
                        PointDayExpire = info.PointDayExpire,
                        State = info.State,
                        Zip = info.Zip,
                        CashfreeSignature = info.CashfreeSignature,
                        IsAdmin = info.IsAdmin,
                        MemberId = info.MemberId,
                        ChatColor = info.ChatColor,
                        PaystackRef = info.PaystackRef,
                        RefUserId = info.RefUserId,
                        SchoolCompleted = info.SchoolCompleted,
                        Type = info.Type,
                        UserPlatform = info.UserPlatform,
                        WeatherUnit = info.WeatherUnit, 
                        Details = string.Empty,
                        Selected = false,
                    };

                    if (info.Details.DetailsClass != null)
                        db.Details = JsonConvert.SerializeObject(info.Details.DetailsClass);
                    connection.Insert(db);
                }

                UserDetails.Avatar = info.Avatar;
                UserDetails.Cover = info.Cover;
                UserDetails.Username = info.Username;
                UserDetails.FullName = info.Name;
                UserDetails.Email = info.Email;

                ListUtils.MyProfileList = new ObservableCollection<UserDataObject>();
                ListUtils.MyProfileList?.Clear();
                ListUtils.MyProfileList?.Add(info);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Insert_Or_Update_To_MyProfileTable(info);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        // Get data To My Profile Table
        public UserDataObject Get_MyProfile()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return null!;
                var item = connection.Table<DataTables.MyProfileTb>().FirstOrDefault();
                if (item != null)
                {
                    UserDataObject infoObject = new UserDataObject
                    {
                        UserId = item.UserId,
                        Username = item.Username,
                        Email = item.Email,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Avatar = item.Avatar,
                        Cover = item.Cover,
                        BackgroundImage = item.BackgroundImage,
                        RelationshipId = item.RelationshipId,
                        Address = item.Address,
                        Working = item.Working,
                        Gender = item.Gender,
                        Facebook = item.Facebook,
                        Google = item.Google,
                        Twitter = item.Twitter,
                        Linkedin = item.Linkedin,
                        Website = item.Website,
                        Instagram = item.Instagram,
                        WebDeviceId = item.WebDeviceId,
                        Language = item.Language,
                        IpAddress = item.IpAddress,
                        PhoneNumber = item.PhoneNumber,
                        Timezone = item.Timezone,
                        Lat = item.Lat,
                        Lng = item.Lng,
                        About = item.About,
                        Birthday = item.Birthday,
                        Registered = item.Registered,
                        Lastseen = item.Lastseen,
                        LastLocationUpdate = item.LastLocationUpdate,
                        Balance = item.Balance,
                        Verified = item.Verified,
                        Status = item.Status,
                        Active = item.Active,
                        Admin = item.Admin,
                        IsPro = item.IsPro,
                        ProType = item.ProType,
                        School = item.School,
                        Name = item.Name,
                        AndroidMDeviceId = item.AndroidMDeviceId,
                        ECommented = item.ECommented,
                        AndroidNDeviceId = item.AndroidMDeviceId,
                        AvatarFull = item.AvatarFull,
                        BirthPrivacy = item.BirthPrivacy,
                        CanFollow = item.CanFollow,
                        ConfirmFollowers = item.ConfirmFollowers,
                        CountryId = item.CountryId,
                        EAccepted = item.EAccepted,
                        EFollowed = item.EFollowed,
                        EJoinedGroup = item.EJoinedGroup,
                        ELastNotif = item.ELastNotif,
                        ELiked = item.ELiked,
                        ELikedPage = item.ELikedPage,
                        EMentioned = item.EMentioned,
                        EProfileWallPost = item.EProfileWallPost,
                        ESentmeMsg = item.ESentmeMsg,
                        EShared = item.EShared,
                        EVisited = item.EVisited,
                        EWondered = item.EWondered,
                        EmailNotification = item.EmailNotification,
                        FollowPrivacy = item.FollowPrivacy,
                        FriendPrivacy = item.FriendPrivacy,
                        GenderText = item.GenderText,
                        InfoFile = item.InfoFile,
                        IosMDeviceId = item.IosMDeviceId,
                        IosNDeviceId = item.IosNDeviceId,
                        IsBlocked = item.IsBlocked,
                        IsFollowing = item.IsFollowing,
                        IsFollowingMe = item.IsFollowingMe,
                        LastAvatarMod = item.LastAvatarMod,
                        LastCoverMod = item.LastCoverMod,
                        LastDataUpdate = item.LastDataUpdate,
                        LastFollowId = item.LastFollowId,
                        LastLoginData = item.LastLoginData,
                        LastseenStatus = item.LastseenStatus,
                        LastseenTimeText = item.LastseenTimeText,
                        LastseenUnixTime = item.LastseenUnixTime,
                        MessagePrivacy = item.MessagePrivacy,
                        NewEmail = item.NewEmail,
                        NewPhone = item.NewPhone,
                        NotificationSettings = item.NotificationSettings,
                        NotificationsSound = item.NotificationsSound,
                        OrderPostsBy = item.OrderPostsBy,
                        PaypalEmail = item.PaypalEmail,
                        PostPrivacy = item.PostPrivacy,
                        Referrer = item.Referrer,
                        ShareMyData = item.ShareMyData,
                        ShareMyLocation = item.ShareMyLocation,
                        ShowActivitiesPrivacy = item.ShowActivitiesPrivacy,
                        TwoFactor = item.TwoFactor,
                        TwoFactorVerified = item.TwoFactorVerified,
                        Url = item.Url,
                        VisitPrivacy = item.VisitPrivacy,
                        Vk = item.Vk,
                        Wallet = item.Wallet,
                        WorkingLink = item.WorkingLink,
                        Youtube = item.Youtube,
                        City = item.City,
                        Points = item.Points,
                        DailyPoints = item.DailyPoints,
                        PointDayExpire = item.PointDayExpire,
                        State = item.State,
                        Zip = item.Zip,
                        CashfreeSignature = item.CashfreeSignature,
                        IsAdmin = item.IsAdmin,
                        MemberId = item.MemberId,
                        ChatColor = item.ChatColor,
                        PaystackRef = item.PaystackRef,
                        RefUserId = item.RefUserId,
                        SchoolCompleted = item.SchoolCompleted,
                        Type = item.Type,
                        UserPlatform = item.UserPlatform,
                        WeatherUnit = item.WeatherUnit,
                        Details = new DetailsUnion(),
                        Selected = false,
                    };

                    if (!string.IsNullOrEmpty(item.Details))
                        infoObject.Details = new DetailsUnion
                        {
                            DetailsClass = JsonConvert.DeserializeObject<Details>(item.Details)
                        };
                     
                    UserDetails.Avatar = item.Avatar;
                    UserDetails.Cover = item.Cover;
                    UserDetails.Username = item.Username;
                    UserDetails.FullName = item.Name;
                    UserDetails.Email = item.Email;

                    ListUtils.MyProfileList = new ObservableCollection<UserDataObject> {infoObject};

                    return infoObject;
                }
                else
                {
                    return null!;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_MyProfile();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null!;
                } 
            }
        }

        #endregion

        #region Search Filter 

        public void InsertOrUpdate_SearchFilter(DataTables.SearchFilterTb dataFilter)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var data = connection.Table<DataTables.SearchFilterTb>().FirstOrDefault();
                if (data == null)
                {
                    connection.Insert(dataFilter);
                }
                else
                {
                    data.Gender = dataFilter.Gender;
                    data.Country = dataFilter.Country;
                    data.Status = dataFilter.Status;
                    data.Verified = dataFilter.Verified;
                    data.ProfilePicture = dataFilter.ProfilePicture;
                    data.FilterByAge = dataFilter.FilterByAge;
                    data.AgeFrom = dataFilter.AgeFrom;
                    data.AgeTo = dataFilter.AgeTo;

                    connection.Update(data);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdate_SearchFilter(dataFilter);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        public DataTables.SearchFilterTb GetSearchFilterById()
        {
            try
            {
                using var connection = OpenConnection();
                var data = connection?.Table<DataTables.SearchFilterTb>().FirstOrDefault();
                return data;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetSearchFilterById();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null!;
                }
            }
        }

        #endregion

        #region Near By Filter 

        public void InsertOrUpdate_NearByFilter(DataTables.NearByFilterTb dataFilter)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var data = connection.Table<DataTables.NearByFilterTb>().FirstOrDefault();
                if (data == null)
                {
                    connection.Insert(dataFilter);
                }
                else
                {
                    data.DistanceValue = dataFilter.DistanceValue;
                    data.Gender = dataFilter.Gender;
                    data.Status = dataFilter.Status;
                    data.Relationship = dataFilter.Relationship;

                    connection.Update(data);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdate_NearByFilter(dataFilter);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        public DataTables.NearByFilterTb GetNearByFilterById()
        {
            try
            {
                using var connection = OpenConnection();
                var data = connection?.Table<DataTables.NearByFilterTb>().FirstOrDefault();
                return data;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetNearByFilterById();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null!;
                }
            }
        }

        #endregion

        #region WatchOffline Videos

        //Insert WatchOffline Videos
        public void Insert_WatchOfflineVideos(GetMoviesObject.Movie video)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                if (video != null)
                {
                    var select = connection.Table<DataTables.WatchOfflineVideosTb>().FirstOrDefault(a => a.Id == video.Id);
                    if (select == null)
                    {
                        DataTables.WatchOfflineVideosTb watchOffline = new DataTables.WatchOfflineVideosTb
                        {
                            Id = video.Id,
                            Name = video.Name,
                            Cover = video.Cover,
                            Description = video.Description,
                            Country = video.Country,
                            Duration = video.Duration,
                            Genre = video.Genre,
                            Iframe = video.Iframe,
                            Quality = video.Quality,
                            Producer = video.Producer,
                            Release = video.Release,
                            Source = video.Source,
                            Stars = video.Stars,
                            Url = video.Url,
                            Video = video.Video,
                            Views = video.Views,
                        };

                        connection.Insert(watchOffline);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Insert_WatchOfflineVideos(video);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Remove WatchOffline Videos
        public void Remove_WatchOfflineVideos(string watchOfflineVideosId)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                if (!string.IsNullOrEmpty(watchOfflineVideosId))
                {
                    var select = connection.Table<DataTables.WatchOfflineVideosTb>().FirstOrDefault(a => a.Id == watchOfflineVideosId);
                    if (select != null)
                    {
                        connection.Delete(select);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Remove_WatchOfflineVideos(watchOfflineVideosId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }
         
        //Get WatchOffline Videos
        public ObservableCollection<DataTables.WatchOfflineVideosTb> Get_WatchOfflineVideos()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return new ObservableCollection<DataTables.WatchOfflineVideosTb>();
                var select = connection.Table<DataTables.WatchOfflineVideosTb>().OrderByDescending(a => a.AutoIdWatchOfflineVideos).ToList();
                if (select.Count > 0)
                {
                    return new ObservableCollection<DataTables.WatchOfflineVideosTb>(select);
                }
                else
                {
                    return new ObservableCollection<DataTables.WatchOfflineVideosTb>();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_WatchOfflineVideos();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<DataTables.WatchOfflineVideosTb>();
                } 
            }
        }

        //Get WatchOffline Videos
        public GetMoviesObject.Movie Get_WatchOfflineVideos_ById(string id)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return null!;
                var video = connection.Table<DataTables.WatchOfflineVideosTb>().FirstOrDefault(a => a.Id == id);
                if (video != null)
                {
                    GetMoviesObject.Movie watchOffline = new GetMoviesObject.Movie
                    {
                        Id = video.Id,
                        Name = video.Name,
                        Cover = video.Cover,
                        Description = video.Description,
                        Country = video.Country,
                        Duration = video.Duration,
                        Genre = video.Genre,
                        Iframe = video.Iframe,
                        Quality = video.Quality,
                        Producer = video.Producer,
                        Release = video.Release,
                        Source = video.Source,
                        Stars = video.Stars,
                        Url = video.Url,
                        Video = video.Video,
                        Views = video.Views,
                    };

                    return watchOffline;
                }
                else
                {
                    return null!;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_WatchOfflineVideos_ById(id);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null!;
                } 
            }
        }

        public DataTables.WatchOfflineVideosTb Update_WatchOfflineVideos(string videoId, string videoPath)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return null!;
                var select = connection.Table<DataTables.WatchOfflineVideosTb>().FirstOrDefault(a => a.Id == videoId);
                if (select != null)
                {
                    select.VideoName = videoId + ".mp4";
                    select.VideoSavedPath = videoPath;

                    connection.Update(select);

                    return select;
                }
                else
                {
                    return null!;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Update_WatchOfflineVideos(videoId, videoPath);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null!;
                } 
            }
        }

        #endregion

        #region Gifts

        //Insert data Gifts
        public void InsertAllGifts(ObservableCollection<GiftObject.DataGiftObject> listData)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var result = connection.Table<DataTables.GiftsTb>().ToList();

                List<DataTables.GiftsTb> list = new List<DataTables.GiftsTb>();
                foreach (var info in listData)
                {
                    var gift = new DataTables.GiftsTb
                    {
                        Id = info.Id,
                        MediaFile = info.MediaFile,
                        Name = info.Name,
                        Time = info.Time,
                        TimeText = info.TimeText,
                    };

                    list.Add(gift);

                    var update = result.FirstOrDefault(a => a.Id == info.Id);
                    if (update != null)
                    {
                        update = ClassMapper.Mapper?.Map<DataTables.GiftsTb>(info); 
                        connection.Update(update);
                    }
                }
                     
                if (list.Count <= 0) return;
                connection.BeginTransaction();

                //Bring new  
                var newItemList = list.Where(c => !result.Select(fc => fc.Id).Contains(c.Id)).ToList();
                if (newItemList.Count > 0)
                {
                    connection.InsertAll(newItemList);
                }
                     
                connection.Commit();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertAllGifts(listData);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get List Gifts 
        public ObservableCollection<GiftObject.DataGiftObject> GetGiftsList()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return new ObservableCollection<GiftObject.DataGiftObject>();
                var result = connection.Table<DataTables.GiftsTb>().ToList();
                if (result?.Count > 0)
                {
                    List<GiftObject.DataGiftObject> list = result.Select(gift => new GiftObject.DataGiftObject
                    {
                        Id = gift.Id,
                        MediaFile = gift.MediaFile,
                        Name = gift.Name,
                        Time = gift.Time,
                        TimeText = gift.TimeText,
                    }).ToList();

                    return new ObservableCollection<GiftObject.DataGiftObject>(list);
                }
                else
                {
                    return new ObservableCollection<GiftObject.DataGiftObject>();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetGiftsList();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<GiftObject.DataGiftObject>();
                } 
            }
        }

        #endregion

        #region Post

        //Insert Or Update data Post
        public void InsertOrUpdatePost(string db)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var dataUser = connection.Table<DataTables.PostsTb>().FirstOrDefault();
                if (dataUser != null)
                {
                    dataUser.DataPostJson = db;  
                    connection.Update(dataUser);
                }
                else
                {
                    DataTables.PostsTb postsTb = new DataTables.PostsTb
                    {
                        DataPostJson = db
                    };

                    connection.Insert(postsTb);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdatePost(db);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get data Post
        public string GetDataPost()
        {
            try
            {
                using var connection = OpenConnection();
                var dataPost = connection?.Table<DataTables.PostsTb>().FirstOrDefault();
                return dataPost?.DataPostJson ?? "";
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetDataPost();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null!;
                } 
            }
        }

        public void DeletePost()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                connection.DeleteAll<DataTables.PostsTb>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    DeletePost();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion
         
        #region Last Chat Filter

        public void InsertOrUpdate_FilterLastChat(DataTables.FilterLastChatTb dataFilter)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var data = connection.Table<DataTables.FilterLastChatTb>().FirstOrDefault();
                if (data == null)
                {
                    connection.Insert(dataFilter);
                }
                else
                {
                    data.Status = dataFilter.Status;

                    connection.Update(data);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdate_FilterLastChat(dataFilter);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        public DataTables.FilterLastChatTb GetFilterLastChatById()
        {
            try
            {
                using var connection = OpenConnection();
                var data = connection?.Table<DataTables.FilterLastChatTb>().FirstOrDefault();
                return data;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetFilterLastChatById();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null!;
                }
            }
        }

        #endregion

        #region Message

        //Insert data To Message Table
        public void Insert_Or_Replace_MessagesTable(ObservableCollection<AdapterModelsClassMessage> messageList)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;

                List<DataTables.MessageTb> listOfDatabaseForInsert = new List<DataTables.MessageTb>();

                // get data from database
                var resultMessage = connection.Table<DataTables.MessageTb>().ToList();

                foreach (var messages in messageList)
                {
                    var maTb = ClassMapper.Mapper?.Map<DataTables.MessageTb>(messages.MesData);
                    maTb.SendFile = false;

                    maTb.Product = JsonConvert.SerializeObject(messages.MesData.Product?.ProductClass);

                    maTb.MessageUser = JsonConvert.SerializeObject(messages.MesData.MessageUser?.User);
                    maTb.UserData = JsonConvert.SerializeObject(messages.MesData.UserData);
                    maTb.ToData = JsonConvert.SerializeObject(messages.MesData.ToData);

                    var dataCheck = resultMessage.FirstOrDefault(a => a.Id == messages.MesData.Id);
                    if (dataCheck != null)
                    {
                        var checkForUpdate = resultMessage.FirstOrDefault(a => a.Id == dataCheck.Id);
                        if (checkForUpdate != null)
                        {
                            checkForUpdate = ClassMapper.Mapper?.Map<DataTables.MessageTb>(messages.MesData);
                            checkForUpdate.SendFile = false;
                            checkForUpdate.ChatColor = messages.MesData.ChatColor;

                            checkForUpdate.Product = JsonConvert.SerializeObject(messages.MesData.Product?.ProductClass);

                            checkForUpdate.MessageUser = JsonConvert.SerializeObject(messages.MesData.MessageUser?.User);
                            checkForUpdate.UserData = JsonConvert.SerializeObject(messages.MesData.UserData);
                            checkForUpdate.ToData = JsonConvert.SerializeObject(messages.MesData.ToData);

                            connection.Update(checkForUpdate);

                            var cec = ChatWindowActivity.GetInstance()?.StartedMessageList?.FirstOrDefault(a => a.Id == Long.ParseLong(dataCheck.Id));
                            if (cec != null)
                            {
                                cec.MesData = messages.MesData;
                                cec.MesData.IsStarted = true;
                                cec.TypeView = messages.TypeView;
                            }
                        }
                        else
                        {
                            listOfDatabaseForInsert.Add(maTb);
                        }
                    }
                    else
                    {
                        listOfDatabaseForInsert.Add(maTb);
                    }
                }

                connection.BeginTransaction();

                //Bring new  
                if (listOfDatabaseForInsert.Count > 0)
                {
                    connection.InsertAll(listOfDatabaseForInsert);
                }

                connection.Commit();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Insert_Or_Replace_MessagesTable(messageList);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Update one Messages Table
        public void Insert_Or_Update_To_one_MessagesTable(MessageDataExtra item)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;

                var data = connection.Table<DataTables.MessageTb>().FirstOrDefault(a => a.Id == item.Id);
                if (data != null)
                {
                    data = ClassMapper.Mapper?.Map<DataTables.MessageTb>(item);
                    data.SendFile = false;

                    data.Product = JsonConvert.SerializeObject(item.Product?.ProductClass);

                    data.MessageUser = JsonConvert.SerializeObject(item.MessageUser?.User);
                    data.UserData = JsonConvert.SerializeObject(item.UserData);
                    data.ToData = JsonConvert.SerializeObject(item.ToData);

                    connection.Update(data);
                }
                else
                {
                    var maTb = ClassMapper.Mapper?.Map<DataTables.MessageTb>(item);
                    maTb.SendFile = false;

                    maTb.Product = JsonConvert.SerializeObject(item.Product?.ProductClass);

                    maTb.MessageUser = JsonConvert.SerializeObject(item.MessageUser?.User);
                    maTb.UserData = JsonConvert.SerializeObject(item.UserData);
                    maTb.ToData = JsonConvert.SerializeObject(item.ToData);

                    //Insert  one Messages Table
                    connection.Insert(maTb);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Insert_Or_Update_To_one_MessagesTable(item);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get data To Messages
        public ObservableCollection<AdapterModelsClassMessage> GetMessages_List(string fromId, string toId, string beforeMessageId)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return null;
                var beforeQ = "";
                if (beforeMessageId != "0")
                {
                    beforeQ = "AND Id < " + beforeMessageId + " AND Id <> " + beforeMessageId + " ";
                }

                var query2 = connection.Query<DataTables.MessageTb>("SELECT * FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + ")) " + beforeQ);

                List<DataTables.MessageTb> query = query2.Where(w => w.FromId == fromId && w.ToId == toId || w.ToId == fromId && w.FromId == toId).OrderBy(q => q.Time).TakeLast(50).ToList();
                ObservableCollection<AdapterModelsClassMessage> listMessages = new ObservableCollection<AdapterModelsClassMessage>();
                if (query.Count > 0)
                {
                    foreach (var item in query)
                    {
                        var check = ChatWindowActivity.GetInstance()?.MAdapter.DifferList.FirstOrDefault(a => a.MesData.Id == item.Id);
                        if (check != null)
                            continue;

                        var maTb = ClassMapper.Mapper?.Map<MessageDataExtra>(item);
                        if (maTb != null)
                        {
                            maTb.SendFile = false;
                            maTb.ChatColor = ChatWindowActivity.MainChatColor;

                            maTb.Product = new ProductUnion();
                            maTb.MessageUser = new MessageData.MessageUserUnion();
                            maTb.UserData = new UserDataObject();
                            maTb.ToData = new UserDataObject();

                            if (!string.IsNullOrEmpty(item.Product))
                                maTb.Product = new ProductUnion
                                {
                                    ProductClass = JsonConvert.DeserializeObject<ProductDataObject>(item.Product)
                                };

                            if (!string.IsNullOrEmpty(item.MessageUser))
                                maTb.MessageUser = new MessageData.MessageUserUnion
                                {
                                    User = JsonConvert.DeserializeObject<UserDataObject>(item.MessageUser)
                                };

                            if (!string.IsNullOrEmpty(item.UserData))
                                maTb.UserData = JsonConvert.DeserializeObject<UserDataObject>(item.UserData);

                            if (!string.IsNullOrEmpty(item.ToData))
                                maTb.ToData = JsonConvert.DeserializeObject<UserDataObject>(item.ToData);

                            var type = Holders.GetTypeModel(maTb);
                            if (type == MessageModelType.None)
                                continue;

                            if (beforeMessageId == "0")
                            {
                                listMessages.Add(new AdapterModelsClassMessage
                                {
                                    TypeView = type,
                                    Id = Long.ParseLong(item.Id),
                                    MesData = WoWonderTools.MessageFilter(toId, maTb, type, true)
                                });
                            }
                            else
                            {
                                listMessages.Insert(0, new AdapterModelsClassMessage
                                {
                                    TypeView = type,
                                    Id = Long.ParseLong(item.Id),
                                    MesData = WoWonderTools.MessageFilter(toId, maTb, type, true)
                                });
                            }
                        }
                    }
                    return listMessages;
                }

                return listMessages;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetMessages_List(fromId, toId, beforeMessageId);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<AdapterModelsClassMessage>();
                }
            }
        }

        //Get data To where first Messages >> load more
        public ObservableCollection<DataTables.MessageTb> GetMessageList(string fromId, string toId, string beforeMessageId)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return null;
                var beforeQ = "";
                if (beforeMessageId != "0")
                {
                    beforeQ = "AND Id < " + beforeMessageId + " AND Id <> " + beforeMessageId + " ";
                }

                var query2 = connection.Query<DataTables.MessageTb>("SELECT * FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + ")) " + beforeQ);

                List<DataTables.MessageTb> query = query2.Where(w => w.FromId == fromId && w.ToId == toId || w.ToId == fromId && w.FromId == toId).TakeLast(50).ToList();

                query.Reverse();
                return new ObservableCollection<DataTables.MessageTb>(query);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetMessageList(fromId, toId, beforeMessageId);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<DataTables.MessageTb>();
                }
            }
        }

        //Remove data To Messages Table
        public void Delete_OneMessageUser(string messageId)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var user = connection.Table<DataTables.MessageTb>().FirstOrDefault(c => c.Id == messageId);
                if (user != null)
                {
                    connection.Delete(user);
                }

                Delete_OneStartedMessageUser(messageId);
                Delete_OnePinMessageUser(messageId);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Delete_OneMessageUser(messageId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        public void DeleteAllMessagesUser(string fromId, string toId)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var query = connection.Query<DataTables.MessageTb>("Delete FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + "))");
                Console.WriteLine(query);

                DeleteAllStartedMessagesUser(fromId, toId);
                DeleteAllPinMessagesUser(fromId, toId);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    DeleteAllMessagesUser(fromId, toId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        public void ClearAll_Messages()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                connection.DeleteAll<DataTables.MessageTb>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    ClearAll_Messages();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Call_User

        public void Insert_CallUser(Classes.CallUser user)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var result = connection.Table<DataTables.CallUserTb>().ToList();
                var check = result.FirstOrDefault(a => a.Time == user.Time);
                if (check == null && !string.IsNullOrEmpty(user.Id))
                {
                    DataTables.CallUserTb cv = new DataTables.CallUserTb
                    {
                        CallId = user.Id,
                        UserId = user.UserId,
                        Avatar = user.Avatar,
                        Name = user.Name,
                        AccessToken = user.AccessToken,
                        AccessToken2 = user.AccessToken2,
                        FromId = user.FromId,
                        Active = user.Active,
                        Time = user.Time,
                        Status = user.Status,
                        RoomName = user.RoomName,
                        Type = user.Type,
                        TypeIcon = user.TypeIcon,
                        TypeColor = user.TypeColor
                    };
                    connection.Insert(cv);
                }
                else
                {
                    check = new DataTables.CallUserTb
                    {
                        CallId = user.Id,
                        UserId = user.UserId,
                        Avatar = user.Avatar,
                        Name = user.Name,
                        AccessToken = user.AccessToken,
                        AccessToken2 = user.AccessToken2,
                        FromId = user.FromId,
                        Active = user.Active,
                        Time = user.Time,
                        Status = user.Status,
                        RoomName = user.RoomName,
                        Type = user.Type,
                        TypeIcon = user.TypeIcon,
                        TypeColor = user.TypeColor
                    };

                    connection.Update(check);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Insert_CallUser(user);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        public ObservableCollection<Classes.CallUser> Get_CallUserList()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return new ObservableCollection<Classes.CallUser>();
                var list = new ObservableCollection<Classes.CallUser>();
                var result = connection.Table<DataTables.CallUserTb>().ToList();
                if (result.Count <= 0) return new ObservableCollection<Classes.CallUser>();
                foreach (var cv in result.Select(item => new Classes.CallUser
                {
                    Id = item.CallId,
                    UserId = item.UserId,
                    Avatar = item.Avatar,
                    Name = item.Name,
                    AccessToken = item.AccessToken,
                    AccessToken2 = item.AccessToken2,
                    FromId = item.FromId,
                    Active = item.Active,
                    Time = item.Time,
                    Status = item.Status,
                    RoomName = item.RoomName,
                    Type = item.Type,
                    TypeIcon = item.TypeIcon,
                    TypeColor = item.TypeColor
                }))
                {
                    list.Add(cv);
                }

                return new ObservableCollection<Classes.CallUser>(list.OrderBy(a => a.Time).ToList());
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_CallUserList();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<Classes.CallUser>();
                }
            }
        }

        public void Clear_CallUser_List()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                connection.DeleteAll<DataTables.CallUserTb>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Clear_CallUser_List();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Last User Chat

        //Insert Or Update data To Users Table
        public void Insert_Or_Update_LastUsersChat(Context context, ObservableCollection<ChatObject> chatList, bool showFloating = false)
        {
            try
            {
                using (var connection = OpenConnection())
                {
                    if (connection == null) return;

                    var result = connection.Table<DataTables.LastUsersTb>().ToList();
                    List<DataTables.LastUsersTb> list = new List<DataTables.LastUsersTb>();
                    foreach (var item in chatList)
                    {
                        Classes.LastChatArchive archiveObject = null;
                        switch (item.ChatType)
                        {
                            case "user":
                                item.IsMute = WoWonderTools.CheckMute(item.UserId, "user");
                                item.IsPin = WoWonderTools.CheckPin(item.UserId, "user");
                                archiveObject = WoWonderTools.CheckArchive(item.UserId, "user");
                                item.IsArchive = archiveObject != null;
                                break;
                            case "page":
                                var userAdminPage = item.UserId;
                                if (userAdminPage == item.LastMessage.LastMessageClass.ToData.UserId)
                                {
                                    var userId = item.LastMessage.LastMessageClass.UserData?.UserId;

                                    //var name = item.LastMessage.LastMessageClass.UserData?.Name + "(" + item.PageName + ")";
                                    //Console.WriteLine(name);

                                    //wael change after add in api 
                                    item.IsMute = WoWonderTools.CheckMute(item.PageId + userId, "page");
                                    item.IsPin = WoWonderTools.CheckPin(item.PageId + userId, "page");
                                    archiveObject = WoWonderTools.CheckArchive(item.PageId + userId, "page");
                                    item.IsArchive = archiveObject != null;
                                }
                                else
                                {
                                    var userId = item.LastMessage.LastMessageClass.ToData.UserId;

                                    //var name = item.LastMessage.LastMessageClass.ToData.Name + "(" + item.PageName + ")";
                                    //Console.WriteLine(name);

                                    //wael change after add in api 
                                    item.IsMute = WoWonderTools.CheckMute(item.PageId + userId, "page");
                                    item.IsPin = WoWonderTools.CheckPin(item.PageId + userId, "page");
                                    archiveObject = WoWonderTools.CheckArchive(item.PageId + userId, "page");
                                    item.IsArchive = archiveObject != null;
                                }
                                break;
                            case "group":
                                //wael change after add in api 
                                item.IsMute = WoWonderTools.CheckMute(item.GroupId, "group");
                                item.IsPin = WoWonderTools.CheckPin(item.GroupId, "group");
                                archiveObject = WoWonderTools.CheckArchive(item.GroupId, "group");
                                item.IsArchive = archiveObject != null;
                                break;
                        }

                        DataTables.LastUsersTb db = new DataTables.LastUsersTb
                        {
                            Id = item.Id,
                            UserId = item.UserId,
                            Username = item.Username,
                            Email = item.Email,
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            Avatar = item.Avatar,
                            Cover = item.Cover,
                            BackgroundImage = item.BackgroundImage,
                            RelationshipId = item.RelationshipId,
                            Address = item.Address,
                            Working = item.Working,
                            Gender = item.Gender,
                            Facebook = item.Facebook,
                            Google = item.Google,
                            Twitter = item.Twitter,
                            Linkedin = item.Linkedin,
                            Website = item.Website,
                            Instagram = item.Instagram,
                            WebDeviceId = item.WebDeviceId,
                            Language = item.Language,
                            IpAddress = item.IpAddress,
                            PhoneNumber = item.PhoneNumber,
                            Timezone = item.Timezone,
                            Lat = item.Lat,
                            Lng = item.Lng,
                            About = item.About,
                            Birthday = item.Birthday,
                            Registered = item.Registered,
                            Lastseen = item.Lastseen,
                            LastLocationUpdate = item.LastLocationUpdate,
                            Balance = item.Balance,
                            Verified = item.Verified,
                            Status = item.Status,
                            Active = item.Active,
                            Admin = item.Admin,
                            IsPro = item.IsPro,
                            ProType = item.ProType,
                            School = item.School,
                            Name = item.Name,
                            AndroidMDeviceId = item.AndroidMDeviceId,
                            ECommented = item.ECommented,
                            AndroidNDeviceId = item.AndroidNDeviceId,
                            BirthPrivacy = item.BirthPrivacy,
                            ConfirmFollowers = item.ConfirmFollowers,
                            CountryId = item.CountryId,
                            EAccepted = item.EAccepted,
                            EFollowed = item.EFollowed,
                            EJoinedGroup = item.EJoinedGroup,
                            ELastNotif = item.ELastNotif,
                            ELiked = item.ELiked,
                            ELikedPage = item.ELikedPage,
                            EMentioned = item.EMentioned,
                            EProfileWallPost = item.EProfileWallPost,
                            ESentmeMsg = item.ESentmeMsg,
                            EShared = item.EShared,
                            EVisited = item.EVisited,
                            EWondered = item.EWondered,
                            EmailNotification = item.EmailNotification,
                            FollowPrivacy = item.FollowPrivacy,
                            FriendPrivacy = item.FriendPrivacy,
                            InfoFile = item.InfoFile,
                            IosMDeviceId = item.IosMDeviceId,
                            IosNDeviceId = item.IosNDeviceId,
                            LastAvatarMod = item.LastAvatarMod,
                            LastCoverMod = item.LastCoverMod,
                            LastDataUpdate = item.LastDataUpdate,
                            LastFollowId = item.LastFollowId,
                            LastseenStatus = item.LastseenStatus,
                            LastseenUnixTime = item.LastseenUnixTime,
                            MessagePrivacy = item.MessagePrivacy,
                            NewEmail = item.NewEmail,
                            NewPhone = item.NewPhone,
                            NotificationSettings = item.NotificationSettings,
                            NotificationsSound = item.NotificationsSound,
                            OrderPostsBy = item.OrderPostsBy,
                            PaypalEmail = item.PaypalEmail,
                            PostPrivacy = item.PostPrivacy,
                            Referrer = item.Referrer,
                            ShareMyData = item.ShareMyData,
                            ShareMyLocation = item.ShareMyLocation,
                            ShowActivitiesPrivacy = item.ShowActivitiesPrivacy,
                            TwoFactor = item.TwoFactor,
                            TwoFactorVerified = item.TwoFactorVerified,
                            Url = item.Url,
                            VisitPrivacy = item.VisitPrivacy,
                            Vk = item.Vk,
                            Wallet = item.Wallet,
                            WorkingLink = item.WorkingLink,
                            Youtube = item.Youtube,
                            City = item.City,
                            Points = item.Points,
                            DailyPoints = item.DailyPoints,
                            PointDayExpire = item.PointDayExpire,
                            State = item.State,
                            Zip = item.Zip,
                            AvatarOrg = item.AvatarOrg,
                            BackgroundImageStatus = item.BackgroundImageStatus,
                            Boosted = item.Boosted,
                            CallActionType = item.CallActionType,
                            CallActionTypeUrl = item.CallActionTypeUrl,
                            Category = item.Category,
                            ChatTime = item.ChatTime,
                            ChatType = item.ChatType,
                            Company = item.Company,
                            CoverFull = item.CoverFull,
                            CoverOrg = item.CoverOrg,
                            CssFile = item.CssFile,
                            EmailCode = item.EmailCode,
                            GroupId = item.GroupId,
                            Instgram = item.Instgram,
                            Joined = item.Joined,
                            LastEmailSent = item.LastEmailSent,
                            PageCategory = item.PageCategory,
                            PageDescription = item.PageDescription,
                            PageId = item.PageId,
                            PageName = item.PageName,
                            PageTitle = item.PageTitle,
                            Phone = item.Phone,
                            GroupName = item.GroupName,
                            ProTime = item.ProTime,
                            Rating = item.Rating,
                            RefUserId = item.RefUserId,
                            SchoolCompleted = item.SchoolCompleted,
                            Showlastseen = item.Showlastseen,
                            SidebarData = item.SidebarData,
                            SmsCode = item.SmsCode,
                            SocialLogin = item.SocialLogin,
                            Src = item.Src,
                            StartUp = item.StartUp,
                            StartupFollow = item.StartupFollow,
                            StartupImage = item.StartupImage,
                            StartUpInfo = item.StartUpInfo,
                            Time = item.Time,
                            Type = item.Type,
                            WeatherUnit = item.WeatherUnit,
                            MessageCount = item.MessageCount,
                            AvatarFull = item.AvatarFull,
                            AvatarPostId = item.AvatarPostId,
                            CanFollow = item.CanFollow,
                            CashfreeSignature = item.CashfreeSignature,
                            ChatColor = item.ChatColor,
                            CodeSent = item.CodeSent,
                            CoverPostId = item.CoverPostId,
                            GenderText = item.GenderText,
                            IsAdmin = item.IsAdmin,
                            IsArchive = item.IsArchive,
                            IsBlocked = item.IsBlocked,
                            IsFollowing = item.IsFollowing,
                            IsFollowingMe = item.IsFollowingMe,
                            IsMute = item.IsMute,
                            IsPageOnwer = item.IsPageOnwer,
                            IsPin = item.IsPin,
                            LastLoginData = item.LastLoginData,
                            LastseenTimeText = item.LastseenTimeText,
                            MemberId = item.MemberId,
                            PaystackRef = item.PaystackRef,
                            Selected = item.Selected,
                            UserPlatform = item.UserPlatform,
                            Owner = item.Owner,
                            UserData = JsonConvert.SerializeObject(item.UserData),
                            LastMessage = JsonConvert.SerializeObject(item.LastMessage.LastMessageClass),
                            Parts = JsonConvert.SerializeObject(item.Parts),
                            Details = JsonConvert.SerializeObject(item.Details.DetailsClass),
                        };

                        list.Add(db);

                        var update = result.FirstOrDefault(a => a.UserId == item.UserId);
                        if (update != null)
                        {
                            update = db;
                            update.UserData = JsonConvert.SerializeObject(item.UserData);
                            update.LastMessage = JsonConvert.SerializeObject(item.LastMessage.LastMessageClass);
                            update.Parts = JsonConvert.SerializeObject(item.Parts);
                            update.Details = JsonConvert.SerializeObject(item.Details.DetailsClass);

                            var chk = IdMesgList.FirstOrDefault(a => a == item.LastMessage.LastMessageClass?.Id);

                            if (showFloating && InitFloating.CanDrawOverlays(context) && item.LastMessage.LastMessageClass != null && item.LastMessage.LastMessageClass.Seen == "0" && chk == null && item.LastMessage.LastMessageClass.FromId != UserDetails.UserId && !item.IsMute && Methods.AppLifecycleObserver.AppState == "Background")
                            {
                                var floating = new FloatingObject
                                {
                                    ChatType = item.ChatType,
                                    UserId = item.UserId,
                                    PageId = item.PageId,
                                    GroupId = item.GroupId,
                                    Avatar = item.Avatar,
                                    ChatColor = "",
                                    LastSeen = item.Lastseen,
                                    LastSeenUnixTime = item.LastseenUnixTime,
                                    Name = item.Name,
                                    MessageCount = item.LastMessage.LastMessageClass.MessageCount ?? "1"
                                };

                                switch (item.ChatType)
                                {
                                    case "user":
                                        floating.Name = item.Name;
                                        break;
                                    case "page":
                                        floating.Name = item.PageName;
                                        break;
                                    case "group":
                                        floating.Name = item.GroupName;
                                        break;
                                }

                                IdMesgList.Add(item.LastMessage.LastMessageClass.Id);

                                if (InitFloating.FloatingObject == null && ChatHeadService.RunService)
                                    return;

                                // launch service 
                                Intent intent = new Intent(context, typeof(ChatHeadService));
                                intent.PutExtra(ChatHeadService.ExtraCutoutSafeArea, FloatingViewManager.FindCutoutSafeArea(new Activity()));
                                intent.PutExtra("UserData", JsonConvert.SerializeObject(floating));
                                ContextCompat.StartForegroundService(context, intent);
                            }

                            connection.Update(update);
                        }
                    }

                    if (list.Count <= 0) return;

                    connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                    if (newItemList.Count > 0)
                        connection.InsertAll(newItemList);

                    result = connection.Table<DataTables.LastUsersTb>().ToList();
                    var deleteItemList = result.Where(c => !list.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                    if (deleteItemList.Count > 0)
                        foreach (var delete in deleteItemList)
                            connection.Delete(delete);

                    connection.Commit();
                }

                ListUtils.UserList = chatList;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Insert_Or_Update_LastUsersChat(context, chatList, showFloating);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        // Get data To Users Table
        public ObservableCollection<ChatObject> Get_LastUsersChat_List()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return null;
                var select = connection.Table<DataTables.LastUsersTb>().ToList();
                if (@select.Count > 0)
                {
                    var list = new List<ChatObject>();

                    foreach (DataTables.LastUsersTb item in @select)
                    {
                        Classes.LastChatArchive archiveObject = null;
                        ChatObject db = new ChatObject
                        {
                            Id = item.Id,
                            UserId = item.UserId,
                            Username = item.Username,
                            Email = item.Email,
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            Avatar = item.Avatar,
                            Cover = item.Cover,
                            BackgroundImage = item.BackgroundImage,
                            RelationshipId = item.RelationshipId,
                            Address = item.Address,
                            Working = item.Working,
                            Gender = item.Gender,
                            Facebook = item.Facebook,
                            Google = item.Google,
                            Twitter = item.Twitter,
                            Linkedin = item.Linkedin,
                            Website = item.Website,
                            Instagram = item.Instagram,
                            WebDeviceId = item.WebDeviceId,
                            Language = item.Language,
                            IpAddress = item.IpAddress,
                            PhoneNumber = item.PhoneNumber,
                            Timezone = item.Timezone,
                            Lat = item.Lat,
                            Lng = item.Lng,
                            About = item.About,
                            Birthday = item.Birthday,
                            Registered = item.Registered,
                            Lastseen = item.Lastseen,
                            LastLocationUpdate = item.LastLocationUpdate,
                            Balance = item.Balance,
                            Verified = item.Verified,
                            Status = item.Status,
                            Active = item.Active,
                            Admin = item.Admin,
                            IsPro = item.IsPro,
                            ProType = item.ProType,
                            School = item.School,
                            Name = item.Name,
                            AndroidMDeviceId = item.AndroidMDeviceId,
                            ECommented = item.ECommented,
                            AndroidNDeviceId = item.AndroidNDeviceId,
                            BirthPrivacy = item.BirthPrivacy,
                            ConfirmFollowers = item.ConfirmFollowers,
                            CountryId = item.CountryId,
                            EAccepted = item.EAccepted,
                            EFollowed = item.EFollowed,
                            EJoinedGroup = item.EJoinedGroup,
                            ELastNotif = item.ELastNotif,
                            ELiked = item.ELiked,
                            ELikedPage = item.ELikedPage,
                            EMentioned = item.EMentioned,
                            EProfileWallPost = item.EProfileWallPost,
                            ESentmeMsg = item.ESentmeMsg,
                            EShared = item.EShared,
                            EVisited = item.EVisited,
                            EWondered = item.EWondered,
                            EmailNotification = item.EmailNotification,
                            FollowPrivacy = item.FollowPrivacy,
                            FriendPrivacy = item.FriendPrivacy,
                            InfoFile = item.InfoFile,
                            IosMDeviceId = item.IosMDeviceId,
                            IosNDeviceId = item.IosNDeviceId,
                            LastAvatarMod = item.LastAvatarMod,
                            LastCoverMod = item.LastCoverMod,
                            LastDataUpdate = item.LastDataUpdate,
                            LastFollowId = item.LastFollowId,
                            LastseenStatus = item.LastseenStatus,
                            LastseenUnixTime = item.LastseenUnixTime,
                            MessagePrivacy = item.MessagePrivacy,
                            NewEmail = item.NewEmail,
                            NewPhone = item.NewPhone,
                            NotificationSettings = item.NotificationSettings,
                            NotificationsSound = item.NotificationsSound,
                            OrderPostsBy = item.OrderPostsBy,
                            PaypalEmail = item.PaypalEmail,
                            PostPrivacy = item.PostPrivacy,
                            Referrer = item.Referrer,
                            ShareMyData = item.ShareMyData,
                            ShareMyLocation = item.ShareMyLocation,
                            ShowActivitiesPrivacy = item.ShowActivitiesPrivacy,
                            TwoFactor = item.TwoFactor,
                            TwoFactorVerified = item.TwoFactorVerified,
                            Url = item.Url,
                            VisitPrivacy = item.VisitPrivacy,
                            Vk = item.Vk,
                            Wallet = item.Wallet,
                            WorkingLink = item.WorkingLink,
                            Youtube = item.Youtube,
                            City = item.City,
                            Points = item.Points,
                            DailyPoints = item.DailyPoints,
                            PointDayExpire = item.PointDayExpire,
                            State = item.State,
                            Zip = item.Zip,
                            AvatarOrg = item.AvatarOrg,
                            BackgroundImageStatus = item.BackgroundImageStatus,
                            Boosted = item.Boosted,
                            CallActionType = item.CallActionType,
                            CallActionTypeUrl = item.CallActionTypeUrl,
                            Category = item.Category,
                            ChatTime = item.ChatTime,
                            ChatType = item.ChatType,
                            Company = item.Company,
                            CoverFull = item.CoverFull,
                            CoverOrg = item.CoverOrg,
                            CssFile = item.CssFile,
                            EmailCode = item.EmailCode,
                            GroupId = item.GroupId,
                            Instgram = item.Instgram,
                            Joined = item.Joined,
                            LastEmailSent = item.LastEmailSent,
                            PageCategory = item.PageCategory,
                            PageDescription = item.PageDescription,
                            PageId = item.PageId,
                            PageName = item.PageName,
                            PageTitle = item.PageTitle,
                            Phone = item.Phone,
                            GroupName = item.GroupName,
                            ProTime = item.ProTime,
                            Rating = item.Rating,
                            RefUserId = item.RefUserId,
                            SchoolCompleted = item.SchoolCompleted,
                            Showlastseen = item.Showlastseen,
                            SidebarData = item.SidebarData,
                            SmsCode = item.SmsCode,
                            SocialLogin = item.SocialLogin,
                            Src = item.Src,
                            StartUp = item.StartUp,
                            StartupFollow = item.StartupFollow,
                            StartupImage = item.StartupImage,
                            StartUpInfo = item.StartUpInfo,
                            Time = item.Time,
                            Type = item.Type,
                            WeatherUnit = item.WeatherUnit,
                            MessageCount = item.MessageCount,
                            AvatarFull = item.AvatarFull,
                            AvatarPostId = item.AvatarPostId,
                            CanFollow = item.CanFollow,
                            CashfreeSignature = item.CashfreeSignature,
                            ChatColor = item.ChatColor,
                            CodeSent = item.CodeSent,
                            CoverPostId = item.CoverPostId,
                            GenderText = item.GenderText,
                            IsAdmin = item.IsAdmin,
                            IsArchive = item.IsArchive,
                            IsBlocked = item.IsBlocked,
                            IsFollowing = item.IsFollowing,
                            IsFollowingMe = item.IsFollowingMe,
                            IsMute = item.IsMute,
                            IsPageOnwer = item.IsPageOnwer,
                            IsPin = item.IsPin,
                            LastLoginData = item.LastLoginData,
                            LastseenTimeText = item.LastseenTimeText,
                            MemberId = item.MemberId,
                            PaystackRef = item.PaystackRef,
                            Selected = item.Selected,
                            UserPlatform = item.UserPlatform,
                            LastMessage = new LastMessageUnion
                            {
                                LastMessageClass = new MessageData()
                            },
                            Owner = Convert.ToBoolean(item.Owner),
                            Parts = new List<PartsUnion>(),
                            UserData = new UserDataObject(),
                            Details = new DetailsUnion
                            {
                                DetailsClass = new Details()
                            },
                        };

                        if (!string.IsNullOrEmpty(item.UserData))
                            db.UserData = JsonConvert.DeserializeObject<UserDataObject>(item.UserData);

                        if (!string.IsNullOrEmpty(item.LastMessage))
                        {
                            var sss = JsonConvert.DeserializeObject<MessageData>(item.LastMessage);
                            if (sss != null)
                            {
                                db.LastMessage = new LastMessageUnion
                                {
                                    LastMessageClass = sss
                                };
                            }
                        }

                        if (!string.IsNullOrEmpty(item.Parts))
                            db.Parts = JsonConvert.DeserializeObject<List<PartsUnion>>(item.Parts);

                        switch (item.ChatType)
                        {
                            case "user":
                                db.IsMute = WoWonderTools.CheckMute(db.UserId, "user");
                                db.IsPin = WoWonderTools.CheckPin(db.UserId, "user");
                                archiveObject = WoWonderTools.CheckArchive(db.UserId, "user");
                                db.IsArchive = archiveObject != null;
                                break;
                            case "page":
                                var userAdminPage = db?.UserId;
                                if (userAdminPage == db.LastMessage.LastMessageClass?.ToData?.UserId)
                                {
                                    var userId = db.LastMessage.LastMessageClass?.UserData?.UserId;

                                    //var name = db.LastMessage.LastMessageClass?.UserData?.Name + "(" + db.PageName + ")";
                                    //Console.WriteLine(name);

                                    //wael change after add in api 
                                    db.IsMute = WoWonderTools.CheckMute(db.PageId + userId, "page");
                                    db.IsPin = WoWonderTools.CheckPin(db.PageId + userId, "page");
                                    archiveObject = WoWonderTools.CheckArchive(db.PageId + userId, "page");
                                    db.IsArchive = archiveObject != null;
                                }
                                else
                                {
                                    var userId = db.LastMessage.LastMessageClass?.ToData?.UserId;

                                    //var name = db.LastMessage.LastMessageClass.ToData.Name + "(" + db.PageName + ")";
                                    //Console.WriteLine(name);

                                    //wael change after add in api 
                                    db.IsMute = WoWonderTools.CheckMute(db.PageId + userId, "page");
                                    db.IsPin = WoWonderTools.CheckPin(db.PageId + userId, "page");
                                    archiveObject = WoWonderTools.CheckArchive(db.PageId + userId, "page");
                                    db.IsArchive = archiveObject != null; ;
                                }
                                break;
                            case "group":
                                //wael change after add in api 
                                db.IsMute = WoWonderTools.CheckMute(db.GroupId, "group");
                                db.IsPin = WoWonderTools.CheckPin(db.GroupId, "group");
                                archiveObject = WoWonderTools.CheckArchive(db.GroupId, "group");
                                db.IsArchive = archiveObject != null;
                                break;
                        }

                        if (!string.IsNullOrEmpty(item.Details))
                        {
                            var sss = JsonConvert.DeserializeObject<Details>(item.Details);
                            if (sss != null)
                            {
                                db.Details = new DetailsUnion
                                {
                                    DetailsClass = sss
                                };
                            }
                        }

                        if (db.IsPin)
                        {
                            list.Insert(0, db);
                        }
                        else
                        {
                            list.Add(db);
                        }
                    }

                    return new ObservableCollection<ChatObject>(list);
                }
                else
                {
                    return new ObservableCollection<ChatObject>();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_LastUsersChat_List();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<ChatObject>();
                }
            }
        }

        //Remove data from Users Table
        public void Delete_LastUsersChat(string id, string chatType, string recipientId = "")
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;

                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    switch (chatType)
                    {
                        case "user":
                            {
                                var user = connection.Table<DataTables.LastUsersTb>().FirstOrDefault(c => c.UserId == id && c.ChatType == "user");
                                if (user != null)
                                    connection.Delete(user);
                                break;
                            }
                        case "page":
                            {
                                var page = connection.Table<DataTables.LastUsersTb>().FirstOrDefault(c => c.PageId == id && JsonConvert.DeserializeObject<MessageDataExtra>(c.LastMessage).ToData.UserId == recipientId && c.ChatType == "page");
                                if (page != null)
                                    connection.Delete(page);
                                break;
                            }
                        case "group":
                            {
                                var group = connection.Table<DataTables.LastUsersTb>().FirstOrDefault(c => c.GroupId == id && c.ChatType == "group");
                                if (@group != null)
                                    connection.Delete(@group);
                                break;
                            }
                    }
                }
                else
                {
                    var user = connection.Table<DataTables.LastUsersChatTb>().FirstOrDefault(c => JsonConvert.DeserializeObject<UserDataObject>(c.UserData).UserId == id);
                    if (user != null)
                    {
                        connection.Delete(user);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Delete_LastUsersChat(id, chatType, recipientId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Remove All data To Users Table
        public void ClearAll_LastUsersChat()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    connection.DeleteAll<DataTables.LastUsersTb>();
                }
                else
                {
                    connection.DeleteAll<DataTables.LastUsersChatTb>();
                }

                DeleteAllPin();
                DeleteAllMute();
                DeleteAllArchive();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    ClearAll_LastUsersChat();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Last_User_Chat >> Old

        private static readonly List<string> IdMesgList = new List<string>();
        //Insert Or Update data To Users Table
        public void Insert_Or_Update_LastUsersChat(Context context, ObservableCollection<GetUsersListObject.User> lastUsersList, bool showFloating = false)
        {
            try
            {
                using var connection = OpenConnection();
                var result = connection.Table<DataTables.LastUsersChatTb>().ToList();

                List<DataTables.LastUsersChatTb> list = new List<DataTables.LastUsersChatTb>();
                foreach (var user in lastUsersList)
                {
                    user.IsMute = WoWonderTools.CheckMute(user.UserId, "user");
                    user.IsPin = WoWonderTools.CheckPin(user.UserId, "user");
                    var archiveObject = WoWonderTools.CheckArchive(user.UserId, "user");
                    user.IsArchive = archiveObject != null;

                    var item = new DataTables.LastUsersChatTb
                    {
                        OrderId = user.ChatTime,
                        UserData = JsonConvert.SerializeObject(user),
                        LastMessageData = JsonConvert.SerializeObject(user.LastMessage)
                    };
                    list.Add(item);

                    var update = result.FirstOrDefault(a => a.OrderId == user.ChatTime);
                    if (update != null)
                    {
                        update = item;
                        connection.Update(update);

                        var chk = IdMesgList.FirstOrDefault(a => a == user.LastMessage.Id);
                        if (showFloating && InitFloating.CanDrawOverlays(context) && user.LastMessage != null && user.LastMessage.Seen == "0" && chk == null && user.LastMessage.FromId != UserDetails.UserId && !user.IsMute && Methods.AppLifecycleObserver.AppState == "Background")
                        {
                            var floating = new FloatingObject
                            {
                                ChatType = "user",
                                UserId = user.UserId,
                                PageId = "",
                                GroupId = "",
                                Avatar = user.Avatar,
                                ChatColor = user.ChatColor,
                                LastSeen = user.Lastseen,
                                LastSeenUnixTime = user.LastseenUnixTime,
                                Name = user.Name,
                                MessageCount = "1"
                            };

                            IdMesgList.Add(user.LastMessage.Id);

                            if (InitFloating.FloatingObject == null && ChatHeadService.RunService)
                                return;

                            // launch service 
                            Intent intent = new Intent(context, typeof(ChatHeadService));
                            intent.PutExtra(ChatHeadService.ExtraCutoutSafeArea, FloatingViewManager.FindCutoutSafeArea(new Activity()));
                            intent.PutExtra("UserData", JsonConvert.SerializeObject(floating));
                            ContextCompat.StartForegroundService(context, intent);
                        }
                    }
                }

                if (list.Count <= 0) return;

                connection.BeginTransaction();
                //Bring new  
                var newItemList = list.Where(c => !result.Select(fc => fc.UserData).Contains(c.UserData)).ToList();
                if (newItemList.Count > 0)
                    connection.InsertAll(newItemList);

                result = connection.Table<DataTables.LastUsersChatTb>().ToList();
                var deleteItemList = result.Where(c => !list.Select(fc => fc.UserData).Contains(c.UserData)).ToList();
                if (deleteItemList.Count > 0)
                    foreach (var delete in deleteItemList)
                        connection.Delete(delete);

                connection.Commit();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Insert_Or_Update_LastUsersChat(context, lastUsersList, showFloating);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        // Get data To Last Users Chat
        public ObservableCollection<GetUsersListObject.User> GetLastUsersChatList()
        {
            try
            {
                using var connection = OpenConnection();
                var result = connection.Table<DataTables.LastUsersChatTb>().OrderByDescending(a => a.OrderId).ToList();
                if (result.Count <= 0) return new ObservableCollection<GetUsersListObject.User>();

                var list = new List<GetUsersListObject.User>();
                foreach (var chatUser in from user in result
                                         let userData = JsonConvert.DeserializeObject<GetUsersListObject.User>(user.UserData)
                                         let lastMessageData = JsonConvert.DeserializeObject<MessageData>(user.LastMessageData)
                                         where userData != null && lastMessageData != null
                                         select new GetUsersListObject.User
                                         {
                                             UserId = userData.UserId,
                                             Username = userData.Username,
                                             Avatar = userData.Avatar,
                                             Cover = userData.Cover,
                                             LastseenTimeText = userData.LastseenTimeText,
                                             Lastseen = userData.Lastseen,
                                             Url = userData.Url,
                                             Name = userData.Name,
                                             LastseenUnixTime = userData.LastseenUnixTime,
                                             ChatColor = userData.ChatColor,
                                             Verified = userData.Verified,
                                             LastMessage = lastMessageData,
                                             OldAvatar = userData.OldAvatar,
                                             OldCover = userData.OldCover,
                                             IsMute = WoWonderTools.CheckMute(userData.UserId, "user"),
                                             IsPin = WoWonderTools.CheckPin(userData.UserId, "user"),
                                             IsArchive = WoWonderTools.CheckArchive(userData.UserId, "user") != null ? true : false,
                                         })
                {
                    if (chatUser.IsPin)
                    {
                        list.Insert(0, chatUser);
                    }
                    else
                    {
                        list.Add(chatUser);
                    }
                }

                return new ObservableCollection<GetUsersListObject.User>(list);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetLastUsersChatList();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<GetUsersListObject.User>();
                }
            }
        }

        #endregion

        #region Started Message 

        //Insert Or Delete one StartedMessages Table
        public void Insert_Or_Delete_To_one_StartedMessagesTable(MessageDataExtra item)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;

                var data = connection.Table<DataTables.StartedMessageTb>().FirstOrDefault(a => a.Id == item.Id);
                if (data != null)
                {
                    connection.Delete(data);

                    var cec = ChatWindowActivity.GetInstance()?.StartedMessageList?.FirstOrDefault(a => a.Id == Long.ParseLong(item.Id));
                    if (cec != null)
                    {
                        ChatWindowActivity.GetInstance()?.StartedMessageList.Remove(cec);
                    }
                }
                else
                {
                    item.IsStarted = true;
                    var maTb = ClassMapper.Mapper?.Map<DataTables.StartedMessageTb>(item);
                    maTb.SendFile = false;

                    maTb.Product = JsonConvert.SerializeObject(item.Product?.ProductClass);

                    maTb.MessageUser = JsonConvert.SerializeObject(item.MessageUser?.User);
                    maTb.UserData = JsonConvert.SerializeObject(item.UserData);
                    maTb.ToData = JsonConvert.SerializeObject(item.ToData);

                    //Insert  one Messages Table
                    connection.Insert(maTb);

                    var cec = ChatWindowActivity.GetInstance()?.StartedMessageList?.FirstOrDefault(a => a.Id == Long.ParseLong(item.Id));
                    if (cec == null)
                    {
                        ChatWindowActivity.GetInstance()?.StartedMessageList.Add(new AdapterModelsClassMessage
                        {
                            Id = Long.ParseLong(item.Id),
                            MesData = item,
                            TypeView = item.ModelType
                        });
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Insert_Or_Delete_To_one_StartedMessagesTable(item);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get data To where first StartedMessages  
        public ObservableCollection<AdapterModelsClassMessage> GetStartedMessageList(string fromId, string toId)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return null;
                var query2 = connection.Query<DataTables.StartedMessageTb>("SELECT * FROM StartedMessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + ")) ");

                List<DataTables.StartedMessageTb> query = query2.Where(w => w.FromId == fromId && w.ToId == toId || w.ToId == fromId && w.FromId == toId).ToList();

                var list = new ObservableCollection<AdapterModelsClassMessage>();
                foreach (var item in query)
                {
                    var check = list.FirstOrDefault(a => a.MesData.Id == item.Id);
                    if (check != null)
                        continue;

                    var maTb = ClassMapper.Mapper?.Map<MessageDataExtra>(item);
                    maTb.SendFile = false;

                    maTb.Product = new ProductUnion();
                    maTb.MessageUser = new UserDataObject();
                    maTb.UserData = new UserDataObject();
                    maTb.ToData = new UserDataObject();

                    if (!string.IsNullOrEmpty(item.Product))
                        maTb.Product = new ProductUnion
                        {
                            ProductClass = JsonConvert.DeserializeObject<ProductDataObject>(item.Product)
                        };

                    if (!string.IsNullOrEmpty(item.MessageUser))
                        maTb.MessageUser = new MessageData.MessageUserUnion
                        {
                            User = JsonConvert.DeserializeObject<UserDataObject>(item.MessageUser)
                        };

                    if (!string.IsNullOrEmpty(item.UserData))
                        maTb.UserData = JsonConvert.DeserializeObject<UserDataObject>(item.UserData);

                    if (!string.IsNullOrEmpty(item.ToData))
                        maTb.ToData = JsonConvert.DeserializeObject<UserDataObject>(item.ToData);

                    var type = Holders.GetTypeModel(maTb);
                    if (type == MessageModelType.None)
                        continue;

                    list.Add(new AdapterModelsClassMessage
                    {
                        TypeView = type,
                        Id = Long.ParseLong(item.Id),
                        MesData = WoWonderTools.MessageFilter(toId, maTb, type, true)
                    });
                }

                return list;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetStartedMessageList(fromId, toId);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<AdapterModelsClassMessage>();
                }
            }
        }

        public bool IsStartedMessages(string messageId)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return false;
                return connection.Table<DataTables.StartedMessageTb>()?.FirstOrDefault(a => a.Id == messageId)?.IsStarted ?? false;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return IsStartedMessages(messageId);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return false;
                }
            }
        }

        //Remove data To StartedMessages Table
        public void Delete_OneStartedMessageUser(string messageId)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var user = connection.Table<DataTables.StartedMessageTb>().FirstOrDefault(c => c.Id == messageId);
                if (user != null)
                {
                    connection.Delete(user);

                    var cec = ChatWindowActivity.GetInstance()?.StartedMessageList?.FirstOrDefault(a => a.Id == Long.ParseLong(messageId));
                    if (cec != null)
                    {
                        ChatWindowActivity.GetInstance()?.StartedMessageList.Remove(cec);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Delete_OneStartedMessageUser(messageId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        public void DeleteAllStartedMessagesUser(string fromId, string toId)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var query = connection.Query<DataTables.StartedMessageTb>("Delete FROM StartedMessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + "))");
                Console.WriteLine(query);

                ChatWindowActivity.GetInstance()?.StartedMessageList?.Clear();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    DeleteAllStartedMessagesUser(fromId, toId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Pinned Message 

        //Insert Or Delete one PinnedMessages Table
        public void Insert_Or_Delete_To_one_PinnedMessagesTable(MessageDataExtra item)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;

                var data = connection.Table<DataTables.PinnedMessageTb>().FirstOrDefault(a => a.Id == item.Id);
                if (data != null)
                {
                    connection.Delete(data);

                    var cec = ChatWindowActivity.GetInstance()?.PinnedMessageList?.FirstOrDefault(a => a.Id == Long.ParseLong(item.Id));
                    if (cec != null)
                    {
                        ChatWindowActivity.GetInstance()?.PinnedMessageList.Remove(cec);
                    }
                }
                else
                {
                    item.IsPinned = true;
                    var maTb = ClassMapper.Mapper?.Map<DataTables.PinnedMessageTb>(item);
                    maTb.SendFile = false;

                    maTb.Product = JsonConvert.SerializeObject(item.Product?.ProductClass);

                    maTb.MessageUser = JsonConvert.SerializeObject(item.MessageUser?.User);
                    maTb.UserData = JsonConvert.SerializeObject(item.UserData);
                    maTb.ToData = JsonConvert.SerializeObject(item.ToData);

                    //Insert  one Messages Table
                    connection.Insert(maTb);

                    var cec = ChatWindowActivity.GetInstance()?.PinnedMessageList?.FirstOrDefault(a => a.Id == Long.ParseLong(item.Id));
                    if (cec == null)
                    {
                        ChatWindowActivity.GetInstance()?.PinnedMessageList.Add(new AdapterModelsClassMessage
                        {
                            Id = Long.ParseLong(item.Id),
                            MesData = item,
                            TypeView = item.ModelType
                        });
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Insert_Or_Delete_To_one_PinnedMessagesTable(item);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get data To where first PinMessages  
        public ObservableCollection<AdapterModelsClassMessage> GetPinnedMessageList(string fromId, string toId)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return null;
                var query2 = connection.Query<DataTables.PinnedMessageTb>("SELECT * FROM PinnedMessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + ")) ");

                List<DataTables.PinnedMessageTb> query = query2.Where(w => w.FromId == fromId && w.ToId == toId || w.ToId == fromId && w.FromId == toId).ToList();

                var list = new ObservableCollection<AdapterModelsClassMessage>();
                foreach (var item in query)
                {
                    var check = list.FirstOrDefault(a => a.MesData.Id == item.Id);
                    if (check != null)
                        continue;

                    var maTb = ClassMapper.Mapper?.Map<MessageDataExtra>(item);
                    maTb.SendFile = false;

                    maTb.Product = new ProductUnion();
                    maTb.MessageUser = new UserDataObject();
                    maTb.UserData = new UserDataObject();
                    maTb.ToData = new UserDataObject();

                    if (!string.IsNullOrEmpty(item.Product))
                        maTb.Product = new ProductUnion
                        {
                            ProductClass = JsonConvert.DeserializeObject<ProductDataObject>(item.Product)
                        };

                    if (!string.IsNullOrEmpty(item.MessageUser))
                        maTb.MessageUser = new MessageData.MessageUserUnion
                        {
                            User = JsonConvert.DeserializeObject<UserDataObject>(item.MessageUser)
                        };

                    if (!string.IsNullOrEmpty(item.UserData))
                        maTb.UserData = JsonConvert.DeserializeObject<UserDataObject>(item.UserData);

                    if (!string.IsNullOrEmpty(item.ToData))
                        maTb.ToData = JsonConvert.DeserializeObject<UserDataObject>(item.ToData);

                    var type = Holders.GetTypeModel(maTb);
                    if (type == MessageModelType.None)
                        continue;

                    list.Add(new AdapterModelsClassMessage
                    {
                        TypeView = type,
                        Id = Long.ParseLong(item.Id),
                        MesData = WoWonderTools.MessageFilter(toId, maTb, type, true)
                    });
                }

                return list;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetPinnedMessageList(fromId, toId);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<AdapterModelsClassMessage>();
                }
            }
        }

        public bool IsPinMessages(string messageId)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return false;
                return connection.Table<DataTables.PinnedMessageTb>()?.FirstOrDefault(a => a.Id == messageId)?.IsPinned ?? false;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return IsPinMessages(messageId);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return false;
                }
            }
        }

        //Remove data To PinMessages Table
        public void Delete_OnePinMessageUser(string messageId)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var user = connection.Table<DataTables.PinnedMessageTb>().FirstOrDefault(c => c.Id == messageId);
                if (user != null)
                {
                    connection.Delete(user);

                    var cec = ChatWindowActivity.GetInstance()?.PinnedMessageList?.FirstOrDefault(a => a.Id == Long.ParseLong(messageId));
                    if (cec != null)
                    {
                        ChatWindowActivity.GetInstance()?.PinnedMessageList.Remove(cec);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Delete_OnePinMessageUser(messageId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        public void DeleteAllPinMessagesUser(string fromId, string toId)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var query = connection.Query<DataTables.PinnedMessageTb>("Delete FROM PinnedMessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + "))");
                Console.WriteLine(query);

                ChatWindowActivity.GetInstance()?.PinnedMessageList?.Clear();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    DeleteAllPinMessagesUser(fromId, toId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Mute

        public void InsertORDelete_Mute(Classes.OptionLastChat item)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var result = connection.Table<DataTables.MuteTb>().ToList();
                var check = result.FirstOrDefault(a => a.IdChat == item.IdChat && a.ChatType == item.ChatType);
                if (check == null)
                {
                    DataTables.MuteTb cv = new DataTables.MuteTb
                    {
                        ChatType = item.ChatType,
                        IdChat = item.IdChat,
                        UserId = item.UserId,
                        GroupId = item.GroupId,
                        PageId = item.PageId,
                        Name = item.Name
                    };
                    connection.Insert(cv);
                }
                else
                {
                    connection.Delete(check);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertORDelete_Mute(item);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        public ObservableCollection<Classes.OptionLastChat> Get_MuteList()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return new ObservableCollection<Classes.OptionLastChat>();
                var list = new ObservableCollection<Classes.OptionLastChat>();
                var result = connection.Table<DataTables.MuteTb>().ToList();
                if (result.Count <= 0) return new ObservableCollection<Classes.OptionLastChat>();
                foreach (var cv in result.Select(item => new Classes.OptionLastChat
                {
                    ChatType = item.ChatType,
                    IdChat = item.IdChat,
                    UserId = item.UserId,
                    GroupId = item.GroupId,
                    PageId = item.PageId,
                    Name = item.Name
                }))
                {
                    list.Add(cv);
                }

                return new ObservableCollection<Classes.OptionLastChat>(list);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_MuteList();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<Classes.OptionLastChat>();
                }
            }
        }

        public void DeleteAllMute()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                connection.DeleteAll<DataTables.MuteTb>();

                ListUtils.MuteList?.Clear();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    DeleteAllMute();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Pin

        public void InsertORDelete_Pin(Classes.OptionLastChat item)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var result = connection.Table<DataTables.PinTb>().ToList();
                var check = result.FirstOrDefault(a => a.IdChat == item.IdChat && a.ChatType == item.ChatType);
                if (check == null)
                {
                    DataTables.PinTb cv = new DataTables.PinTb
                    {
                        ChatType = item.ChatType,
                        IdChat = item.IdChat,
                        UserId = item.UserId,
                        GroupId = item.GroupId,
                        PageId = item.PageId,
                        Name = item.Name
                    };
                    connection.Insert(cv);
                }
                else
                {
                    connection.Delete(check);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertORDelete_Pin(item);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        public ObservableCollection<Classes.OptionLastChat> Get_PinList()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return new ObservableCollection<Classes.OptionLastChat>();
                var list = new ObservableCollection<Classes.OptionLastChat>();
                var result = connection.Table<DataTables.PinTb>().ToList();
                if (result.Count <= 0) return new ObservableCollection<Classes.OptionLastChat>();
                foreach (var cv in result.Select(item => new Classes.OptionLastChat
                {
                    ChatType = item.ChatType,
                    IdChat = item.IdChat,
                    UserId = item.UserId,
                    GroupId = item.GroupId,
                    PageId = item.PageId,
                    Name = item.Name
                }))
                {
                    list.Add(cv);
                }

                return new ObservableCollection<Classes.OptionLastChat>(list);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_PinList();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<Classes.OptionLastChat>();
                }
            }
        }

        public void DeleteAllPin()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                connection.DeleteAll<DataTables.PinTb>();

                ListUtils.PinList?.Clear();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    DeleteAllPin();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Archive

        public void InsertORDelete_Archive(Classes.LastChatArchive item)
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                var result = connection.Table<DataTables.ArchiveTb>().ToList();
                var check = result.FirstOrDefault(a => a.IdChat == item.IdChat && a.ChatType == item.ChatType);
                if (check == null)
                {
                    DataTables.ArchiveTb cv = new DataTables.ArchiveTb
                    {
                        ChatType = item.ChatType,
                        IdChat = item.IdChat,
                        UserId = item.UserId,
                        GroupId = item.GroupId,
                        PageId = item.PageId,
                        Name = item.Name,
                        IdLastMessage = item.IdLastMessage,
                        LastMessagesUser = JsonConvert.SerializeObject(item.LastMessagesUser),
                        LastChat = JsonConvert.SerializeObject(item.LastChat),
                        LastChatPage = JsonConvert.SerializeObject(item.LastChatPage),
                    };
                    connection.Insert(cv);
                }
                else
                {
                    connection.Delete(check);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertORDelete_Archive(item);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        public ObservableCollection<Classes.LastChatArchive> Get_ArchiveList()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return new ObservableCollection<Classes.LastChatArchive>();
                var list = new ObservableCollection<Classes.LastChatArchive>();
                var result = connection.Table<DataTables.ArchiveTb>().ToList();
                if (result.Count <= 0) return new ObservableCollection<Classes.LastChatArchive>();
                foreach (var cv in result.Select(item => new Classes.LastChatArchive
                {
                    ChatType = item.ChatType,
                    IdChat = item.IdChat,
                    UserId = item.UserId,
                    GroupId = item.GroupId,
                    PageId = item.PageId,
                    Name = item.Name,
                    IdLastMessage = item.IdLastMessage,
                    LastMessagesUser = !string.IsNullOrEmpty(item.LastMessagesUser) ? JsonConvert.DeserializeObject<GetUsersListObject.User>(item.LastMessagesUser) : new GetUsersListObject.User(),
                    LastChat = !string.IsNullOrEmpty(item.LastChat) ? JsonConvert.DeserializeObject<ChatObject>(item.LastChat) : new ChatObject(),
                    LastChatPage = !string.IsNullOrEmpty(item.LastChatPage) ? JsonConvert.DeserializeObject<PageClass>(item.LastChatPage) : new PageClass(),
                }))
                {
                    list.Add(cv);
                }

                return new ObservableCollection<Classes.LastChatArchive>(list);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_ArchiveList();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<Classes.LastChatArchive>();
                }
            }
        }

        public void DeleteAllArchive()
        {
            try
            {
                using var connection = OpenConnection();
                if (connection == null) return;
                connection.DeleteAll<DataTables.ArchiveTb>();

                ListUtils.PinList?.Clear();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    DeleteAllArchive();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Stickers

        //Insert data To Stickers Table
        public void Insert_To_StickersTb()
        {
            try
            {
                using var connection = OpenConnection();
                var data = connection.Table<DataTables.StickersTb>().ToList()?.Count;
                if (data == 0)
                {
                    var stickersList = new ObservableCollection<DataTables.StickersTb>();
                    DataTables.StickersTb s1 = new DataTables.StickersTb
                    {
                        IdSticker = 1,
                        Name = "Rappit",
                        Visibility = true,
                        Count = Stickers.StickerList1.Count.ToString()
                    };
                    stickersList.Add(s1);

                    DataTables.StickersTb s2 = new DataTables.StickersTb
                    {
                        IdSticker = 2,
                        Name = "Water Drop",
                        Visibility = true,
                        Count = Stickers.StickerList2.Count.ToString()
                    };
                    stickersList.Add(s2);

                    DataTables.StickersTb s3 = new DataTables.StickersTb
                    {
                        IdSticker = 3,
                        Name = "Monster",
                        Visibility = true,
                        Count = Stickers.StickerList3.Count.ToString()
                    };
                    stickersList.Add(s3);

                    DataTables.StickersTb s4 = new DataTables.StickersTb
                    {
                        IdSticker = 4,
                        Name = "NINJA Nyankko",
                        Visibility = true,
                        Count = Stickers.StickerList4.Count.ToString()
                    };
                    stickersList.Add(s4);

                    DataTables.StickersTb s5 = new DataTables.StickersTb
                    {
                        IdSticker = 5,
                        Name = "So Much Love",
                        Visibility = false,
                        Count = Stickers.StickerList5.Count.ToString()
                    };
                    stickersList.Add(s5);

                    DataTables.StickersTb s6 = new DataTables.StickersTb
                    {
                        IdSticker = 6,
                        Name = "Sukkara chan",
                        Visibility = false,
                        Count = Stickers.StickerList6.Count.ToString()
                    };
                    stickersList.Add(s6);

                    DataTables.StickersTb s7 = new DataTables.StickersTb
                    {
                        IdSticker = 7,
                        Name = "Flower Hijab",
                        Visibility = false,
                        Count = Stickers.StickerList7.Count.ToString()
                    };
                    stickersList.Add(s7);

                    DataTables.StickersTb s8 = new DataTables.StickersTb
                    {
                        IdSticker = 8,
                        Name = "Trendy boy",
                        Visibility = false,
                        Count = Stickers.StickerList8.Count.ToString()
                    };
                    stickersList.Add(s8);

                    DataTables.StickersTb s9 = new DataTables.StickersTb
                    {
                        IdSticker = 9,
                        Name = "The stickman",
                        Visibility = false,
                        Count = Stickers.StickerList9.Count.ToString()
                    };
                    stickersList.Add(s9);

                    DataTables.StickersTb s10 = new DataTables.StickersTb
                    {
                        IdSticker = 10,
                        Name = "Chip Dale Animated",
                        Visibility = false,
                        Count = Stickers.StickerList10.Count.ToString()
                    };
                    stickersList.Add(s10);

                    DataTables.StickersTb s11 = new DataTables.StickersTb
                    {
                        IdSticker = 11,
                        Name = AppSettings.ApplicationName + " Stickers",
                        Visibility = false,
                        Count = Stickers.StickerList11.Count.ToString()
                    };
                    stickersList.Add(s11);

                    connection.InsertAll(stickersList);

                    ListUtils.StickersList = new ObservableCollection<DataTables.StickersTb>(stickersList);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("database is locked"))
                    Insert_To_StickersTb();
                else
                    Methods.DisplayReportResultTrack(ex);
            }
        }

        //Get  data To Stickers Table
        public ObservableCollection<DataTables.StickersTb> Get_From_StickersTb()
        {
            try
            {
                using var connection = OpenConnection();
                var stickersList = new ObservableCollection<DataTables.StickersTb>();
                var data = connection.Table<DataTables.StickersTb>().ToList();

                foreach (var s in data.Select(item => new DataTables.StickersTb
                {
                    IdSticker = item.IdSticker,
                    Name = item.Name,
                    Visibility = item.Visibility,
                    Count = item.Count
                }))
                {
                    stickersList.Add(s);
                }

                return stickersList;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_From_StickersTb();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null;
                }
            }
        }

        //Update data To Stickers Table
        public void Update_To_StickersTable(string typeName, bool visibility)
        {
            try
            {
                using var connection = OpenConnection();
                var data = connection.Table<DataTables.StickersTb>().FirstOrDefault(a => a.Name == typeName);
                if (data != null)
                {
                    data.Visibility = visibility;
                }
                connection.Update(data);

                var data2 = ListUtils.StickersList.FirstOrDefault(a => a.Name == typeName);
                if (data2 != null)
                {
                    data2.Visibility = visibility;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("database is locked"))
                    Update_To_StickersTable(typeName, visibility);
                else
                    Methods.DisplayReportResultTrack(ex);
            }
        }

        #endregion

    }
}