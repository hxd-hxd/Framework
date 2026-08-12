using System;
using System.Collections.Generic;
using Framework.Localization;
using LangType = Framework.Localization.Language;

namespace Framework.LocalizationSimple
{
    /// <summary>设置本地化语言</summary>
    public class LocalizationSet : LocalizationSetBase
    {
        public override void Set()
        {
            Set(LocalizationCurLanguage.Instance.curLanguage);
        }

        /// <summary>设置</summary>
        public virtual void Set(LangType lang)
        {
            base.Set(lang);
        }

        public override string LangTypeToString(object lang)
        {
            string langStr = default;
            if (lang is LangType langType)
                langStr = LangTypeToString(langType);
            return langStr;
        }

        public virtual string LangTypeToString(LangType lang)
        {
            return lang switch
            {
                LangType.ChineseSimplified => "汉语",
                LangType.ChineseTraditional => "汉语-繁体",
                LangType.English => "英文",
                LangType.Unspecified => null,
                _ => lang.ToString()
            };
        }

        public override void GetAllLangType(ref List<object> list)
        {
            list ??= TypePool.root.GetList<object>();
            foreach (var item in Enum.GetValues(typeof(LangType)))
            {
                list.Add(item);
            }
        }
    }
}