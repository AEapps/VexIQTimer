using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace AE_Xamarin.Managers
{
    public class AppManager
    {
        #region Singleton Code
        public static AppManager Instance
        {
            get
            {
                lock (ManagerLock)
                {
                    if (ClassInstance == null)
                    {
                        ClassInstance = new AppManager();
                    }
                    return ClassInstance;
                }
            }
        }

        private static AppManager ClassInstance = null;
        private static readonly object ManagerLock = new object();
        #endregion Singleton Code

        #region Save Data Reflection Code

        /// <summary>
        /// Gets The List Of Objects That Contains The Save Atribute.
        /// </summary>
        /// <param name="atype">The Main Class To Save Items From.</param>
        /// <returns>The List Of Objects To Be Saved.</returns>
        private static Dictionary<string, object> GetSaveItems(object atype)
        {
            //Check That They Pass In An Object
            if (atype == null)
            {
                return new Dictionary<string, object>();
            }

            //Get The Main Class Type
            Type ClassType = atype.GetType();

            //Get All The Fields Of The Class With The Save Attribute
            Dictionary<string, object> TempDictionary = new Dictionary<string, object>();
            FieldInfo[] Members = ClassType.GetFields().Where(prop => Attribute.IsDefined(prop, typeof(SaveItemAttribute))).ToArray();

            //Go Through Them All And Add Them To The Return List
            foreach (MemberInfo Member in Members)
            {
                TempDictionary.Add(((FieldInfo)Member).Name, ((FieldInfo)Member).GetValue(Instance));
            }
            return TempDictionary;
        }
        private Dictionary<string, object> SaveItems;

        /// <summary>
        /// Resets The Values Of Items That Are Tagged With The "SaveItem Attribute" To Their Defaults.
        /// THIS CANNOT BE REVERSED!!
        /// </summary>
        public void ResetSaveData()
        {
            //Try To Get Save Items
            SaveItems = GetSaveItems(this);

            //Check For Any Save Items
            if (SaveItems == null)
            {
                return;
            }

            //Load Variable Values To Their Stored Defaults
            FieldInfo[] Members = typeof(AppManager).GetFields().Where(prop => Attribute.IsDefined(prop, typeof(SaveItemAttribute))).ToArray();
            foreach (FieldInfo Member in Members)
            {
                //Get Default Value As Object
                object DefaultValue = Member.GetCustomAttribute<SaveItemAttribute>().DefaultValue;

                //Set Variable To Default Values
                Member.SetValue(Instance, DefaultValue);
            }

            //Reset The Saved Theme To Default
            AppThemeManager.Instance.SelectTheme(AppThemeManager.Instance.ThemeNames[0]);

            //Save The Default Version
            SaveData();
        }

        /// <summary>
        /// Loads Data That Is Tagged With The "SaveItem Attribute".
        /// </summary>
        public void LoadSavedData()
        {
            //Try To Get Save Items
            SaveItems = GetSaveItems(this);

            //Check For Any Save Items
            if (SaveItems == null)
            {
                return;
            }

            //Load Up The Saved Values
            string[] ItemNames = SaveItems.Keys.ToArray();
            for (int i = 0; i < ItemNames.Length; i++)
            {
                if (Application.Current.Properties.ContainsKey(ItemNames[i]))
                {
                    SaveItems[ItemNames[i]] = Application.Current.Properties[ItemNames[i]];
                }
            }

            //Load Variable Values
            FieldInfo[] Members = typeof(AppManager).GetFields().Where(prop => Attribute.IsDefined(prop, typeof(SaveItemAttribute))).ToArray();
            foreach (FieldInfo Member in Members)
            {
                //Set Variable Values
                Member.SetValue(Instance, SaveItems[Member.Name]);
            }

            //Check If The Theme Has Been Saved Before
            if (Application.Current.Properties.ContainsKey("SavedThemeName"))
            {
                //Set The CurrentTheme
                string ThemeName = Application.Current.Properties["SavedThemeName"].ToString();
                AppThemeManager.Instance.SelectTheme(ThemeName);
            }
            else
            {
                //Set The Theme To Default
                Application.Current.Properties.Add("SavedThemeName", null);
            }
        }

        /// <summary>
        /// Saves The Data That Is Tagged With The "SaveItem Attribute".
        /// </summary>
        public void SaveData()
        {
            //Reload Objects With Their New Values
            SaveItems = GetSaveItems(this);

            //Save The Objects
            string[] ItemNames = SaveItems.Keys.ToArray();
            for (int i = 0; i < ItemNames.Length; i++)
            {
                Application.Current.Properties[ItemNames[i]] = SaveItems[ItemNames[i]];
            }

            //Save The CurrentTheme
            Application.Current.Properties["SavedThemeName"] = AppThemeManager.Instance.CurrentThemeName;

            //Save The Application Properties
            Application.Current.SavePropertiesAsync();
        }

        #endregion Save Data Reflection Code

        private AppManager()
        {
            //Set Themes
            Dictionary<string, AppTheme> Themes = new Dictionary<string, AppTheme>()
            {
                { "Default", new AppTheme("#FFFFFF", "#000000", "#336699", "#424242") }
            };
            AppThemeManager.Create(Themes);
        }
    }
}
