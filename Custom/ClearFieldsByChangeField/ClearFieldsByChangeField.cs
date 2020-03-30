using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using WSSC.V4.SYS.DBFramework;

namespace WSSC.V4.DMS.FOS.Controls.ClearFieldsByChangeField
{
    /// <summary>
    /// Контрол Очищение нескольких полей при изменении ключевого поля.
    /// </summary>
	public class ClearFieldsByChangeField : DBListFormWebControl

        protected ClearFieldsByChangeField(DBListFormWebControlMetadata metadata, DBListFormControl listForm)
            : base(metadata, listForm) { }

        /// <summary>
        /// Фабрика для создания контрола.
        /// </summary>
        protected class Factory : DBListFormWebControlFactory
        {
            /// <summary>
            /// Создает экземпляр контрола на форме элемента списка.
            /// </summary>
            /// <param name="metadata">Метаданные контрола.</param>
            /// <param name="listForm">Форма элемента списка.</param>
            /// <returns/>
            protected override DBListFormWebControl CreateListFormWebControl(DBListFormWebControlMetadata metadata, DBListFormControl listForm)
            {
                return new ClearFieldsByChangeField(metadata, listForm);
            }
        }


        private bool __init_Fields = false;
        private KeyValuePair<string, string[]> _Fields;
        /// <summary>
        /// Key - основное поле, Value - зависящие поля
        /// </summary>
        public KeyValuePair<string, string[]> Fields
        {
            get
            {
                if (!__init_Fields)
                {
                    Setting setting = new Setting(this.Item);
                    _Fields = setting.FieldsAddiction.First();
                    __init_Fields = true;
                }
                return _Fields;
            }

        }



        protected override void OnListFormInitCompleted()
        {
            this.AddFieldChangeHandler(this.Fields.Key, "FOS_ClearFieldsByChangeField_Handler");
            this.AppContext.ScriptManager.RegisterResource(@"Controls\ClearFieldsByChangeField\FOS_ClearFieldsByChangeField.js", VersionProvider.ModulePath);
        }

        protected override string ClientInitHandler
        {
            get => "FOS_ClearFieldsByChangeField_Init";
        }

        //Создаём options в js содержащий основное поле и зависимые
        [DataContract]
        private class JSInstanceObject
        {
            [DataMember]
            public string MainField { get; set; }

            [DataMember]
            public string[] ChildFields { get; set; }

            public JSInstanceObject(string mainField, string[] childFields)
            {
                this.MainField = mainField;
                this.ChildFields = childFields;
            }
        }

        protected override object CreateClientInstance()
        {
            return new JSInstanceObject(this.Fields.Key, this.Fields.Value);
        }

        protected override string ClientInstanceName
        {
            get => "FOS_ClearFieldsByChangeField_JSObject";
        }
    }
}