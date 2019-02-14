using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;
namespace HubCore.Infrastructure
{
    public class XMLFileSettingsManager : ISettingsManager
    {
        private readonly string NOT_FOUND = "NotFound";

        public ApplicationInfo _applicationInfo { get; }

        public string GetSetting(string Application, string Section, string Key, string DefaultValue)
        {
            var instanceSettingsRaw = ReadFileContents(_applicationInfo.InstanceSettingsFilePath);
            var instanceSettings = XDocument.Parse(instanceSettingsRaw);
            string retval=DefaultValue;
            var foundInInstance = findInXDoc(instanceSettings,Application,Section,Key, out retval);
            if (!foundInInstance)
            {
                var globalSettingsRaw = ReadFileContents(_applicationInfo.GlobalSettingsFilePath);
                var globalSettings = XDocument.Parse(globalSettingsRaw);
                var foundInGlobal = findInXDoc(globalSettings, Application, Section, Key, out retval);
                if(!foundInGlobal)
                {
                    retval = DefaultValue;
                    SaveSetting(Application, Section, Key, DefaultValue, SettingSaveLocation.Global);
                    SaveSetting(Application, Section, Key, DefaultValue, SettingSaveLocation.Instance);
                }
            }
            return retval;
        }

        private bool findInXDoc(XDocument settings,string Application,string Section,string Key, out string settingValue)
        {
            settingValue = string.Empty;
            bool retval = false;
            var application = settings.Root.Elements("APPLICATION").Where(elm => elm.Attribute("Name").Value == Application).FirstOrDefault();
            if(application!=null)
            {
                var section = application.Elements("SECTION").Where(elm => elm.Attribute("Name").Value == Section).FirstOrDefault();
                if(section!=null)
                {
                    var key = section.Elements("KEY").Where(elm => elm.Attribute("Name").Value == Key).FirstOrDefault();
                    if(key!=null)
                    {
                        retval = true;
                        settingValue = key.Attribute("Value").Value;
                    }
                }
            }
            return retval;
        }

        public string GetSetting(string Application, string Section, string Key)
        {
            return GetSetting(Application, Section, Key, NOT_FOUND);
        }

        public void SaveSetting(string Application, string Section, string Key, string ValueToSave,SettingSaveLocation WhereToSave)
        {
            var filePath = (WhereToSave == SettingSaveLocation.Instance ? _applicationInfo.InstanceSettingsFilePath : _applicationInfo.GlobalSettingsFilePath);
            var raw = ReadFileContents(filePath);
            var doc = XDocument.Parse(raw);
            var app = doc.Root.Elements("APPLICATION").Where(elm => elm.Attribute("Name").Value == Application).FirstOrDefault();
            if(app==null)
            {
                app = new XElement("APPLICATION", new XAttribute("Name", Application));
                doc.Root.Add(app);
            }
            var section = app.Elements("SECTION").Where(elm => elm.Attribute("Name").Value == Section).FirstOrDefault();
            if(section==null)
            {
                section = new XElement("SECTION", new XAttribute("Name", Section));
                app.Add(section);
            }
            var key = section.Elements("KEY").Where(elm => elm.Attribute("Name").Value == Key).FirstOrDefault();
            if (key == null) 
            {
                key= new XElement("KEY", new XAttribute("Name", Key));
                section.Add(key);
            }
            key.SetAttributeValue("Value", ValueToSave);
            WriteFileContents(filePath, doc.ToString());
        }
        public XMLFileSettingsManager(ApplicationInfo applicationInfo)
        {
            _applicationInfo = applicationInfo;
        }
        public virtual string ReadFileContents(string FilePath)
        {
            return File.Exists(FilePath) ? File.ReadAllText(FilePath) : "<SETTINGS/>";
        }
        public virtual void WriteFileContents(string FilePath,string TextToWrite)
        {
            File.WriteAllText(FilePath, TextToWrite);
        }
    }
}