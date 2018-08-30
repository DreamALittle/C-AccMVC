using Acctrue.CMC.Interface.Menu;
using Acctrue.CMC.Model.Menu;
using Acctrue.Library.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Service.Menu
{
    public class MenuService: BizBase, IMenuService
    {
        public string SearchMenu() {
            Condition where = Condition.Empty;
            MenuInfo[] infos =  dbContext.From<MenuInfo>().Where(where).Select().ToArray();
            Dictionary<string, List<MenuInfo>> dic = new Dictionary<string, List<MenuInfo>>();
            foreach (MenuInfo item in infos)
            {
                if (!item.MenuParentId.HasValue)
                {
                    List<MenuInfo> values = infos.Where(c => c.MenuParentId == item.MenuId).OrderBy(c => c.Seqno).ToList();
                    dic.Add(item.MenuName, values);
                }
            }
            string data = JsonConvert.SerializeObject(dic);
            return data;
        }
    }
}
