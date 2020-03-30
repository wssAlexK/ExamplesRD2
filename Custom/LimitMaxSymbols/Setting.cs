using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using WSSC.V4.SYS.DBFramework;

namespace WSSC.V4.DMS.SLD.Controls.LimitMaxSymbols
{
    public class Setting
    {

        private DBItem _Item;

        /// <summary>
        /// Настройка
        /// </summary>
        private XDocument _Xml;


        public Setting(DBItem item)
        {
            _Item = item ?? throw new Exception($"В настройку '{this.GetType().FullName}' не передана карточка");

            _Xml = _Item.Site.ConfigParams.GetXDocument(Consts.ConfigParams.LimitMaxSymbols)
                                 ?? throw new Exception($"Не удалось получить настройку из константы '{Consts.ConfigParams.LimitMaxSymbols}'");
        }

        private bool __init_ThisList = false;
        private XElement _ThisList;
        /// <summary>
        /// Настройка для текущего листа (определяется по карточке)
        /// </summary>
        private XElement ThisList
        {
            get
            {
                if (!__init_ThisList)
                {


                    _ThisList = _Xml.Element("Settings")?
                                    .Elements("Setting")
                                    .FirstOrDefault(el => string.Equals(el.Attribute("listname")?.Value, _Item.List.Name, StringComparison.OrdinalIgnoreCase)
                                                         && string.Equals(el.Attribute("webname")?.Value?.Trim('/'), _Item.List.Web.RelativeUrl.Trim('/'),
                                                            StringComparison.OrdinalIgnoreCase))
                                    ?? throw new Exception($"В настройке '{Consts.ConfigParams.LimitMaxSymbols}' для списка '{_Item.List.Name}' не удалось найти xml-узел");

                    __init_ThisList = true;
                }
                return _ThisList;
            }

        }



        private bool __init_ThisFields = false;
        private IEnumerable<XElement> _ThisFields;
        /// <summary>
        /// Xml-узлы с полями для текущего списка
        /// </summary>
        private IEnumerable<XElement> ThisFields
        {
            get
            {
                if (!__init_ThisFields)
                {
                    _ThisFields = this.ThisList.Elements("Field");
                    if (_ThisFields.Count() == 0)
                        throw new Exception($"В настройке '{Consts.ConfigParams.LimitMaxSymbols}' нет xml-узлов 'Field'");


                    __init_ThisFields = true;
                }
                return _ThisFields;
            }

        }



        private bool __init_FieldsInfo = false;
        private Dictionary<string, int> _FieldsInfo;
        /// <summary>
        /// Название поля : макс символов
        /// </summary>
        public Dictionary<string, int> FieldsInfo
        {
            get
            {
                if (!__init_FieldsInfo)
                {
                    _FieldsInfo = this.ThisFields
                                      .ToDictionary(el =>
                                      {
                                          string fieldname = el.Attribute("name")?.Value;
                                          if (string.IsNullOrEmpty(fieldname))
                                              throw new Exception($"В '{Consts.ConfigParams.LimitMaxSymbols}' в '{el.ToString()}' в атрибуте 'name' не указано поле");

                                          return fieldname;
                                      }
                                      , el =>
                                      {
                                          string maxchars = el.Attribute("maxchars")?.Value;
                                          if (string.IsNullOrEmpty(maxchars))
                                              throw new Exception($"В '{Consts.ConfigParams.LimitMaxSymbols}' в '{el.ToString()}' в атрибуте 'maxchars' не указано макс символов");

                                          if (!int.TryParse(maxchars, out int max) || max < 1)
                                              throw new Exception($"В '{Consts.ConfigParams.LimitMaxSymbols}' в '{el.ToString()}' в атрибуте 'maxchars' некорректные данные");

                                          return max;
                                      }
                                      );

                    __init_FieldsInfo = true;
                }
                return _FieldsInfo;
            }

        }






    }
}
