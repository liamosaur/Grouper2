﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Grouper2
{
    public class AssessGpp
    {
        private readonly JObject _GPP;

        public AssessGpp(JObject GPP)
        {
            _GPP = GPP;
        }

        public JObject GetAssessed(string assessName)
        {
            //construct the method name based on the assessName and get it using reflection
            MethodInfo mi = this.GetType().GetMethod("GetAssessed" + assessName, BindingFlags.NonPublic | BindingFlags.Instance);
            //TODO check if mi exists, error out if not implemented
            //invoke the found method
            try
            {
                JObject gppToAssess = (JObject)_GPP[assessName];
                if (mi != null)
                {
                    JObject assessedThing = (JObject)mi.Invoke(this, parameters: new object[] { gppToAssess });
                    if (assessedThing != null)
                    {
                        return assessedThing;
                    }
                }
                else
                {
                    Utility.DebugWrite("Failed to find method: GetAssessed" + assessName);
                }
                return null;
            }
            catch (Exception e)
            {
                Utility.DebugWrite(e.ToString());
                return null;
            }
        }

        private JObject GetAssessedDrives(JObject gppCategory)
        {
            JProperty gppDriveProp = new JProperty("Drive", gppCategory["Drive"]);
            JObject assessedGppDrives = new JObject(gppDriveProp);
            return assessedGppDrives;
            //Utility.DebugWrite("gpp is about gppShortcuts");
            //Console.WriteLine(gppShortcuts["Shortcut"]);
        }

        private JObject GetAssessedEnvironmentVariables(JObject gppCategory)
        {
            JProperty gppEVProp = new JProperty("EnvironmentVariable", gppCategory["EnvironmentVariable"]);
            JObject assessedGppEVs = new JObject(gppEVProp);
            return assessedGppEVs;
            //Utility.DebugWrite("gpp is about gppShortcuts");
            //Console.WriteLine(gppShortcuts["Shortcut"]);
        }

        // none of these assess functions do anything but return the values from the gpp yet.
        private JObject GetAssessedShortcuts(JObject gppCategory)
        {
            JProperty gppShortcutProp = new JProperty("Shortcut", gppCategory["Shortcut"]);
            JObject assessedGppShortcuts = new JObject(gppShortcutProp);
            return assessedGppShortcuts;
            //Utility.DebugWrite("gpp is about gppShortcuts");
            //Console.WriteLine(gppShortcuts["Shortcut"]);
        }

        private JObject GetAssessedScheduledTasks(JObject gppCategory)
        {
            JProperty assessedGppSchedTasksTaskProp = new JProperty("Task", gppCategory["Task"]);
            JProperty assessedGppSchedTasksImmediateTaskProp = new JProperty("ImmediateTaskV2", gppCategory["ImmediateTaskV2"]);
            JObject assessedGppSchedTasksAllJson = new JObject(assessedGppSchedTasksTaskProp, assessedGppSchedTasksImmediateTaskProp);
            return assessedGppSchedTasksAllJson;
            //Utility.DebugWrite("gpp is about SchedTasks");
            //Console.WriteLine(gppSchedTasks["Task"]);
            //Console.WriteLine(gppSchedTasks["ImmediateTaskV2"]);
        }

        private JObject GetAssessedRegistrySettings(JObject gppCategory)
        {
            JProperty gppRegSettingsProp = new JProperty("RegSettings", gppCategory["Registry"]);
            if(gppRegSettingsProp == null)
            {
                return null;
            }

            Dictionary<string, Dictionary<string, string>> assessedRegsDict = new Dictionary<string, Dictionary<string, string>>();
            JObject assessedGppRegSettings = new JObject(gppRegSettingsProp);
            foreach (JToken gppRegSettings in assessedGppRegSettings.SelectTokens("RegSettings"))
            {
                foreach (JToken gppRegSetting in gppRegSettings)
                {
                    Dictionary<string, string> assessedRegDict = new Dictionary<string, string> { { "InterestLevel", "3" } };
                    JToken gppRegProps = gppRegSetting["Properties"];
                    string regAction = GetActionString(gppRegProps["@action"].ToString());
                    assessedRegDict.Add("Action", regAction);

                    assessedRegDict.Add("Name", gppRegSetting["@name"].ToString());
                    assessedRegDict.Add("Status", gppRegSetting["@status"].ToString());

                    assessedRegDict.Add("Hive", gppRegProps["@hive"].ToString());
                    assessedRegDict.Add("Key", gppRegProps["@key"].ToString());
                    assessedRegDict.Add("Property name", gppRegProps["@name"].ToString());
                    assessedRegDict.Add("Type", gppRegProps["@type"].ToString());
                    assessedRegDict.Add("Value", gppRegProps["@value"].ToString());

                    //TODO maybe like, check some values against a list of interesting values?

                    assessedRegsDict.Add(gppRegSetting["@uid"].ToString(), assessedRegDict);
                }
            }

            JProperty assessedRegsJson = new JProperty("gpp Registry settings", JToken.FromObject(assessedRegsDict));

            return assessedGppRegSettings;
        }

        private JObject GetAssessedNTServices(JObject gppCategory)
        {
            JProperty ntServiceProp = new JProperty("NTService", gppCategory["NTService"]);
            JObject assessedNtServices = new JObject(ntServiceProp);
            return assessedNtServices;
            //Utility.DebugWrite("gpp is about NTServices");
            //Console.WriteLine(gppNtServices["NTService"]);
        }

        private JObject GetAssessedNetworkOptions(JObject gppCategory)
        {
            JProperty gppNetworkOptionsProp = new JProperty("DUN", gppCategory["DUN"]);
            JObject assessedGppNetworkOptions = new JObject(gppNetworkOptionsProp);
            return assessedGppNetworkOptions;
            //Utility.DebugWrite("gpp is about Network Options");
            //Console.WriteLine(gppNetworkOptions["DUN"]);
        }

        private JObject GetAssessedFolders(JObject gppCategory)
        {
            JProperty gppFoldersProp = new JProperty("Folder", gppCategory["Folder"]);
            JObject assessedGppFolders = new JObject(gppFoldersProp);
            return assessedGppFolders;
            //Utility.DebugWrite("gpp is about Folders");
            //Console.WriteLine(gppFolders["Folder"]);
        }

        private JObject GetAssessedNetworkShareSettings(JObject gppCategory)
        {
            JProperty gppNetSharesProp = new JProperty("NetShare", gppCategory["NetShare"]);
            JObject assessedGppNetShares = new JObject(gppNetSharesProp);
            return assessedGppNetShares;
            //Utility.DebugWrite("gpp is about Network Shares");
            //Console.WriteLine(gppNetShares["NetShare"]);
        }


        private JObject GetAssessedIniFiles(JObject gppCategory)
        {
            //Utility.DebugWrite("gpp is about gppIniFiles");
            JObject assessedGppIniFiles = (JObject)gppCategory["Ini"];
            //Console.WriteLine(AssessedGPPIniFiles.ToString());
            return assessedGppIniFiles;
        }

        private JObject GetAssessedFiles(JObject gppCategory)
        {
            JProperty gppFileProp = new JProperty("File", gppCategory["File"]);
            JObject assessedGppFiles = new JObject(gppFileProp);
            return assessedGppFiles;
            //Utility.DebugWrite("gpp is about Files");
            //Console.WriteLine(gppFiles["File"]);
        }

        private JObject GetAssessedGroups(JObject gppCategory)
        {
            Dictionary<string, Dictionary<string, string>> assessedGroupsDict = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, Dictionary<string, string>> assessedUsersDict = new Dictionary<string, Dictionary<string, string>>();

            foreach (JToken gppUsers in gppCategory.SelectTokens("User"))
            {
                foreach (JToken gppUser in gppUsers) {
                    // dictionary for results from this specific user.
                    Dictionary<string, string> assessedUserDict = new Dictionary<string, string>
                    {
                        {"InterestLevel", "3"}
                    };

                    JToken gppUserProps = gppUser["Properties"];

                    // check what the entry is doing to the user and turn it into real word
                    string userAction = GetActionString(gppUserProps["@action"].ToString());
                    assessedUserDict.Add("Action", userAction);

                    // get the username and a bunch of other details:
                    assessedUserDict.Add("Name", gppUser["@name"].ToString());
                    assessedUserDict.Add("User Name", gppUserProps["@userName"].ToString());
                    assessedUserDict.Add("DateTime Changed", gppUser["@changed"].ToString());
                    assessedUserDict.Add("Account Disabled", gppUserProps["@acctDisabled"].ToString());
                    assessedUserDict.Add("Password Never Expires", gppUserProps["@neverExpires"].ToString());
                    assessedUserDict.Add("Description", gppUserProps["@description"].ToString());
                    assessedUserDict.Add("Full Name", gppUserProps["@fullName"].ToString());
                    assessedUserDict.Add("New Name", gppUserProps["@newName"].ToString());
                    

                    // check for cpasswords
                    string cpassword = gppUserProps["@cpassword"].ToString();
                    if (cpassword.Length > 0)
                    {
                        string decryptedCpassword = "";
                        decryptedCpassword = Utility.DecryptCpassword(cpassword);
                        // if we find one, that's super interesting.
                        assessedUserDict.Add("Cpassword", decryptedCpassword);
                        assessedUserDict["InterestLevel"] = "10";
                    }

                    // add to the output dict with a uid to keep it unique.
                    assessedUsersDict.Add(gppUser["@uid"].ToString(), assessedUserDict);
                }
            }

            // repeat the process for Groups
            foreach (JToken gppGroups in gppCategory.SelectTokens("Group"))
            {
                foreach (JToken gppGroup in gppGroups)
                {
                    //dictionary for results from this specific group
                    Dictionary<string, string> assessedGroupDict = new Dictionary<string, string>
                    {
                        {"InterestLevel", "3"}
                    };

                    JToken gppGroupProps = gppGroup["Properties"];

                    // check what the entry is doing to the group and turn it into real word
                    string groupAction = gppGroupProps["@action"].ToString();
                    groupAction = GetActionString(groupAction);

                    // get the group name and a bunch of other details:
                    assessedGroupDict.Add("Name", gppGroup["@name"].ToString());
                    //assessedGroupDict.Add("User Name", gppGroupProps["@userName"].ToString());
                    assessedGroupDict.Add("DateTime Changed", gppGroup["@changed"].ToString());
                    assessedGroupDict.Add("Description", gppGroupProps["@description"].ToString());
                    assessedGroupDict.Add("New Name", gppGroupProps["@newName"].ToString());
                    assessedGroupDict.Add("Delete All Users", gppGroupProps["@deleteAllUsers"].ToString());
                    assessedGroupDict.Add("Delete All Groups", gppGroupProps["@deleteAllGroups"].ToString());
                    assessedGroupDict.Add("Remove Accounts", gppGroupProps["@removeAccounts"].ToString());
                    assessedGroupDict.Add("Action", groupAction);
                    Utility.DebugWrite("You still need to figure out group members.");
                    //Utility.DebugWrite(gppGroupProps["Members"].ToString());


                    // add to the output dict with a uid to keep it unique.
                    assessedGroupsDict.Add(gppGroup["@uid"].ToString(), assessedGroupDict);
                }
            }

            // cast our Dictionaries back into JObjects
            JProperty assessedUsersJson = new JProperty("gpp User settings", JToken.FromObject(assessedUsersDict));
            JProperty assessedGroupsJson = new JProperty("gpp Group settings", JToken.FromObject(assessedGroupsDict));
            // chuck the users and groups together in one JObject
            JObject assessedGppGroupsJson = new JObject();
            // only want to actually output these things if there's anything useful in them.
            if (assessedUsersDict.Count > 0)
            {
                assessedGppGroupsJson.Add(assessedUsersJson);
            }
            if (assessedGroupsDict.Count > 0)
            {
                assessedGppGroupsJson.Add(assessedGroupsJson);
            }
            return assessedGppGroupsJson;
        }

        private static string GetActionString(string actionChar)
            // shut up, i know it's not really a char.
        {
            string actionString = "";

            switch (actionChar)
            {
                case "U":
                    actionString = "Update";
                    break;
                case "A":
                    actionString = "Add";
                    break;
                case "D":
                    actionString = "Delete";
                    break;
                default:
                    Utility.DebugWrite("oh no this is new");
                    actionString = "Broken";
                    break;
            }

            return actionString;
        }
    }
}
