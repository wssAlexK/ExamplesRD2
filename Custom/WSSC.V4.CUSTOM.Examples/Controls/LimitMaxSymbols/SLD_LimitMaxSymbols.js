

function SLD_LimitMaxSymbols_Init()
{
    var fieldsInfo = SLD_LimitMaxSymbols_JSObject.FieldsInfo
    for (var index = 0; index < fieldsInfo.length; index++)
    {
        var fieldname = fieldsInfo[index].Key;
        var maxchars = fieldsInfo[index].Value;

        var field = ListForm.GetField(fieldname, true);
        var containerFieldID = field.ContainerID;
        var typedField = field.TypedField;

        if (typedField == null)
            throw new Error('Не удалось получить типизированное поле для ' + fieldname);

        switch (field.Type)
        {
            case 'DBFieldText':
                $('#' + containerFieldID).find('#' + typedField.ContainerID).find('.txt_input').attr('maxlength', maxchars);
                break;            

            case 'DBFieldMultiLineText':
                $('#' + containerFieldID).find('#' + typedField.ControlID).attr('maxlength', maxchars);
                break;

            default:
                throw new Error('Поле ' + fieldname + ' не поддерживается');
        }
    }
}
