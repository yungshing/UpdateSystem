using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateSystem
{
    public class UpdatesContentFormFollow : Follow
    {
        public UpdatesContentFormFollow() { }

        /// <summary>
        /// 设置是更新状态
        /// 正在更新中
        /// 没有任何更新在进行
        /// </summary>
        /// <param name="isUpdating"></param>
        public void SetUpdateState(bool isUpdating)
        {
            GlobalData.IsUpdating = isUpdating;
        }
        public string SetTips()
        {
            string tips = "";
            if (GlobalData.IsFirstUse)
            {
                return "首次使用，需要下载全部文件";
            }
            for (int i = 0; i < GlobalData.webXML.x_Updates.Length; i++)
            {
                tips += GlobalData.webXML.x_Updates[i];
            }
            return tips;
           }
    }
}
