using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSSC.V4.SYS.DBFramework;
using WSSC.V4.DMS.Workflow;
using WSSC.V4.SYS.Fields.Lookup;

namespace DepartamentsOnStageHandler
{
	/// <summary>
	/// Обработчик на проставления подразделения в зависимости от компании пользователя
	/// </summary>
	public class DepartamentsOnStageHandler : DBItemHandler
	{
		public DepartamentsOnStageHandler(DBItemHandlerDefinition handlerDefinition, DBList list)
			: base(handlerDefinition, list) { }

		protected override void OnBeforeItemUpdate(DBItemOperation operationProperties)
		{
			if (operationProperties == null)
				throw new Exception(string.Format("Ошибка в обработчике '{0}', параметр '{1}'", "DepartamentsOnStageHandler", "DBItemOperation"));

			DBItem item = operationProperties.Item;
			if (item == null)
				throw new Exception(string.Format("Ошибка получения '{0}' в обработчике '{1}'", "item", "DepartamentsOnStageHandler"));

			//только при создании
			if (!item.IsNewOrContextCreated)
				return;

			string currentStage = item.GetStringValue(Consts.Handlers.DepartamentsOnStageHandler.FieldNameStage);
			if (currentStage != Consts.Handlers.DepartamentsOnStageHandler.StagePreparation && currentStage != Consts.Handlers.DepartamentsOnStageHandler.StageAdd)
				return;

			int itemCompanyID = item.GetLookupID(Consts.Handlers.DepartamentsOnStageHandler.FieldNameCompany);
			if (itemCompanyID == 0)
				return;

			//Получаем пользователя
			int userID = item.GetLookupID(Consts.Handlers.DepartamentsOnStageHandler.FieldNameInitor);
			if (userID == 0)
				return;

			DBUser user = item.Site.GetUser(userID);
			if (user == null)
				throw new Exception(string.Format("Ошибка получения '{0}' в обработчике '{1}'", "DBUser", "DepartamentsOnStageHandler"));

			DBItem userItem = user.UserItem;
			if (userItem == null)
				throw new Exception(string.Format("Ошибка получения '{0}' в обработчике '{1}'", "DBUser.DBItem", "DepartamentsOnStageHandler"));

			//Получаем подразделения в карточке пользователя
			List<DBItem> userDepartaments = userItem.GetLookupItems(Consts.Handlers.DepartamentsOnStageHandler.FieldNameDepartamentExtra);
			DBItem userDepartamentItem = userItem.GetLookupItem(Consts.Handlers.DepartamentsOnStageHandler.FieldNameDepartament);
			if (userDepartamentItem != null)
				userDepartaments.Add(userDepartamentItem);

			//Удаляем не актуальные подразделения
			userDepartaments = userDepartaments.Where(t => !t.GetValue<bool>(Consts.Handlers.DepartamentsOnStageHandler.FieldNameDepartamentActual)).ToList();
			if (userDepartaments == null || userDepartaments.Count == 0)
			{
				item.SetValue(Consts.Handlers.DepartamentsOnStageHandler.FieldNameDepartament, null);
				return;
			}

			//Словарь где key это  это ID компании, value это ID подразделения
			Dictionary<int, int[]> userDepartamentInfo = userDepartaments.GroupBy(t => t.GetLookupID(Consts.Handlers.DepartamentsOnStageHandler.FieldNameCompany), t => t.ID)
																		 .ToDictionary(t => t.Key, t => t.ToArray());

			//Если у пользователя нет подразделений для компании из карточки ИЛИ таких подразделений более 1 то поле "Подразделение" очищается
			int[] value;
			if (!userDepartamentInfo.TryGetValue(itemCompanyID, out value) || value.Length > 1)
			{
				item.SetValue(Consts.Handlers.DepartamentsOnStageHandler.FieldNameDepartament, null);
				return;
			}

			//Если одно подразделение по компании из карточки то его и проставляем в поле "Подразделение" 
			item.SetValue(Consts.Handlers.DepartamentsOnStageHandler.FieldNameDepartament, value.First());
		}
	}
}
