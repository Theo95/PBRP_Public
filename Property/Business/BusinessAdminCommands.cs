using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using System;

namespace PBRP
{
    class BusinessAdminCommands {

        [Command("aeditbusiness")]
        public void EditBusinessCommand(Client sender, int id = -1, string field = "", string value = "") {
            Player user = Player.PlayerData[sender];
            if (!(user.MasterAccount.AdminLevel >= 4)) { Message.NotAuthorised(sender); return; }
            EditBusiness(sender, id, field, value);
            return;
        }

        [Command("bizztypes")]
        public void BizzTypesCommand(Client sender) {
            Player user = Player.PlayerData[sender];
            if (!(user.MasterAccount.AdminLevel >= 4)) { Message.NotAuthorised(sender); return; }
            Message.Syntax(sender, "Bizz types:");
            Message.Syntax(sender, "-1 = Undefined | 0 = Shop         | 1 = Mechanic/Garage");
            Message.Syntax(sender, " 2 = Bar/Pub   | 3 = Gas Station  | 4 = Clothes Shop");
            Message.Syntax(sender, " 5 = Tattoo    | 6 = Hair Salon   | 7 = Motel/Hotel");
        }

        public async void EditBusiness(Client sender, int id = -1, string field = "", string value = "") {
            Player user = Player.PlayerData[sender];
            if (!(user.MasterAccount.AdminLevel >= 4)) { Message.NotAuthorised(sender); return; }
            if (field == "" && value == "") {
                EditBusinessCommandUsage(sender);
                return;
            }

            Property propertyToEdit = null;
            PropertyManager mng = new PropertyManager();

            if (id == -1) {
                float minRange = 2.5f;
                propertyToEdit = mng.GetClosestPropertyToLocation(sender.position, minRange);
                id = propertyToEdit.Id;
            }  else {
                if (id > PropertyManager.Properties.Count) { API.shared.SendErrorNotification(sender, "Error: Could not find business"); return; }
                propertyToEdit = PropertyManager.Properties[id - 1];
            }

            Business bizzToEdit = null;
            if (bizzToEdit == null) { API.shared.SendErrorNotification(sender, "Error: Could not find business"); return; }

            if (field == "Subtype") {
                if(Int32.Parse(value) >= 0) {
                    bizzToEdit.SubType = (BusinessType)Int32.Parse(value);
                    Message.Info(sender, "You have set " + bizzToEdit.Name + "'s subtype to " + bizzToEdit.SubType);
                }
            }

            await PropertyRepository.UpdateAsync(bizzToEdit);
        }

        private void EditBusinessCommandUsage(Client sender) {
            Message.Syntax(sender, "/editbusiness [ID] [Field] [Value] | (ID should be -1 to edit the business you're on)");
            Message.Syntax(sender, "Fields: Subtype*");
            Message.Syntax(sender, "* - Requires a number Value (See /bizztypes)");
            return;
        }

    }
}
