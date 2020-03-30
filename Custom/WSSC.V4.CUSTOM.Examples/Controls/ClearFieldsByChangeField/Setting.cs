using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using WSSC.V4.SYS.DBFramework;

namespace WSSC.V4.DMS.FOS.Controls.ClearFieldsByChangeField
{
    public class Setting
    {
        private DBItem Item { get; set; }
        public Setting(DBItem item) => this.Item = item ?? throw new Exception("Не удалось получить карточку (DBItem = null)");




        private bool __init_Xml;
        private XDocument _Xml;
        /// <summary>
        /// Xml настройки
        /// </summary>
        private XDocument Xml
        {
            get
            {
                if (!__init_Xml)
                {
                    _Xml = this.Item.Site.ConfigParams.GetXDocument(Consts.ConfigParams.ClearFieldsByChangeField)
                           ?? throw new Exception($"Не удалось получить настройку '{Consts.ConfigParams.ClearFieldsByChangeField}'");

                    __init_Xml = true;
                }

                return _Xml;
            }
        }

        private bool __init_ListsSetting;
        private List<XElement> _ListsSetting;
        /// <summary>
        /// Настройка для списков
        /// </summary>
        private List<XElement> ListsSetting
        {
            get
            {
                if (!__init_ListsSetting)
                {
                    _ListsSetting = this.Xml.Element("Setting")?.Elements("List")?.ToList();
                    if (_ListsSetting.Count == 0)
                        throw new Exception($"Ошибка в константе '{Consts.ConfigParams.ClearFieldsByChangeField}' - не удалось получить настройку списков");

                    __init_ListsSetting = true;
                }
                return _ListsSetting;
            }
        }

        private bool __init_ListSetting;
        private XElement _ListSetting;
        /// <summary>
        /// Настройка для текущего списка
        /// </summary>
        private XElement ListSetting
        {
            get
            {
                if (!__init_ListSetting)
                {
                    IEnumerable<XElement> fields = this.ListsSetting.FirstOrDefault(el =>
                                                                     string.Equals(el.Attribute("name")?.Value, this.Item.List.Name, StringComparison.OrdinalIgnoreCase)
                                                                     && string.Equals(el.Attribute("web")?.Value?.Trim('/'),
                                                                        this.Item.List.Web.RelativeUrl.Trim('/'), StringComparison.OrdinalIgnoreCase))
                                                     ?.Elements("Field")
                                                     ?? throw new Exception($"Для списка '{this.Item.List.Name}' не найдена или некорректная настройка");

                    if (fields.ToList().Count > 1)
                        throw new Exception($"Для списка '{this.Item.List.Name}' указано несколько xml-узлов Field, а должен быть только один");
                    _ListSetting = fields.First();


                    __init_ListSetting = true;
                }
                return _ListSetting;
            }
        }

        private bool __init_FieldsAddiction;
        private Dictionary<string, string[]> _FieldsAddiction;
        /// <summary>
        /// Зависимость полей. Key = ключевое поле, Value[] = поля которые следует очищать
        /// Не может быть пустой. И не может быть более 1.
        /// </summary>
        public Dictionary<string, string[]> FieldsAddiction
        {
            get
            {
                if (!__init_FieldsAddiction)
                {

                    string key = this.ListSetting.Attribute("parent")?.Value;
                    if (string.IsNullOrEmpty(key))
                        throw new Exception($"Не заполнен атрибут parent в настройке '{Consts.ConfigParams.ClearFieldsByChangeField}': '{this.ListSetting.ToString()}'");

                    //Получаем значения разделяя их через ; и очищая от лишних пробелов
                    string[] values = this.ListSetting.Attribute("childs")?.Value?
                                                            .Split(new char[] { ';' })
                                                            .Select(value => value.TrimStart(' ').TrimEnd(' '))
                                                            .Where(value => !string.IsNullOrEmpty(value))
                                                            .ToArray();

                    if (values.Length < 1)
                        throw new Exception($"Не заполнен корректно атрибут childs в настройке '{Consts.ConfigParams.ClearFieldsByChangeField}': '{this.ListSetting.ToString()}'");

                    _FieldsAddiction = new Dictionary<string, string[]>();
                    _FieldsAddiction.Add(key, values);

                    __init_FieldsAddiction = true;
                }
                return _FieldsAddiction;
            }
        }

    }
}
