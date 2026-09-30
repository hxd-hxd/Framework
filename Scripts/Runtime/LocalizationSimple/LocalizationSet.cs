using System;
using System.Collections.Generic;
using Framework.Localization;
using LangType = Framework.Localization.Language;

namespace Framework.LocalizationSimple
{
    /// <summary>设置本地化语言</summary>
    public class LocalizationSet : LocalizationSetBase
    {
        public override T GetCurrentLang<T>()
        {
            if (LocalizationCurLanguage.Instance.curLanguage is T lang) return lang;
            return default;
        }

        public override void Set()
        {
            Set(GetCurrentLang<LangType>());
        }

        /// <summary>设置</summary>
        public virtual void Set(LangType lang)
        {
            base.Set(lang);
        }

        public override string LangTypeToString<T>(T lang)
        {
            string langStr = default;
            if (lang is LangType langType)
                langStr = LangTypeToString(langType);
            else langStr = lang.ToString();
            return langStr;
        }

        public virtual string LangTypeToString(LangType lang)
        {
            return lang switch
            {
                LangType.ChineseSimplified => "汉语",
                LangType.ChineseTraditional => "汉语-繁体",
                LangType.English => "英语",
                LangType.Unspecified => null,
                _ => lang.ToString()
            };
        }

        public override void GetAllLangType<T>(ref List<T> list)
        {
            if (default(LangType) is T)
            //if(typeof(T).IsAssignableFrom(typeof(LangType)))
            {
                list ??= TypePool.root.GetList<T>();
                foreach (var item in Enum.GetValues(typeof(LangType)))
                {
                    list.Add((T)item);
                }
            }
        }
    }
}