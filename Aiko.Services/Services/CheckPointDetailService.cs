using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using Microsoft.Extensions.Logging;

namespace Aiko.Services.Services
{
    public class CheckPointDetailService : BaseService<CheckPointDetailService>, ICheckPointDetailService
    {
        public CheckPointDetailService(ServiceContext<CheckPointDetailService> context) :
            base(context)
        {

        }

        /// <summary>
        /// aikoApp実行中のアプリケーションコンテキスト
        /// </summary>
        public AikoAppContext AppContext => AikoAppContext;

        public async Task<List<HR01ITEMPINFO>> GetHR01ITEMPINFO(string HR01001, string HR01003, string path)
        {
            HR01ITEM hr01ITEM = new HR01ITEM();
            hr01ITEM.HR01001 = HR01001;
            hr01ITEM.HR01003 = HR01003;

            List<HR01ITEMPINFO> hr01ITEMPINFOList = new List<HR01ITEMPINFO>();
            try
            {
                hr01ITEMPINFOList = await HkksDb.GetHR01ITEMPINFOAsync(hr01ITEM);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

            for (int i = hr01ITEMPINFOList.Count - 1; i >= 0; i--)
            {
                string imageType = hr01ITEMPINFOList[i].HR03017 == 0 ? ".jpg" : ".svg";
                string imagePath = Path.Combine(path, hr01ITEMPINFOList[i].HR03002 + imageType);

                if (!File.Exists(imagePath))
                {
                    hr01ITEMPINFOList.RemoveAt(i);
                }
            }

            return hr01ITEMPINFOList;
        }

        public async Task<List<HR03SYAS>> GetHR03Pic(string HR01001, string HR01003, string proj)
        {
            HR03SYAS hr03SYAS = new HR03SYAS();
            hr03SYAS.HR03001 = HR01001;
            hr03SYAS.HR03003 = HR01003;
            hr03SYAS.HR03004 = proj;

            List<HR03SYAS> hr03PicList = new List<HR03SYAS>();
            try
            {
                hr03PicList = await HkksDb.GetHR03PicAsync(hr03SYAS);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

            return hr03PicList;
        }

        public async Task<List<HM16SHDIR>> GetHM16List(string HR01001)
        {
            HM16SHDIR hm16SHDIR = new HM16SHDIR();
            hm16SHDIR.HM16001 = HR01001;

            List<HM16SHDIR> hm16List = new List<HM16SHDIR>();
            try
            {
                hm16List = await HkksDb.GetHM16ListAsync(hm16SHDIR);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

            return hm16List;
        }

        public async Task<bool> UpdateHR03(List<HR03SYAS> hr03List, string path)
        {
            try
            {
                var hr03ListToUpdate = hr03List.Where(hr03 => hr03.CHANGE == "UPDATE").ToList();
                var hr03ListToDelete = hr03List.Where(hr03 => hr03.CHANGE == "DELETE").ToList();

                List<HR06SYASDEL> hr06List = new List<HR06SYASDEL>();
                foreach (var item in hr03ListToDelete)
                {
                    HR06SYASDEL hr06 = new HR06SYASDEL();
                    hr06.HR06001 = item.HR03001;
                    hr06.HR06002 = item.HR03002;
                    hr06.HR06003 = item.HR03003;
                    hr06.HR06004 = item.HR03004;
                    hr06.HR06005 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    hr06.HR06006 = AppContext.Name;
                    hr06.HR06007 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    hr06.HR06008 = AppContext.Name;
                    hr06.HR06009 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    hr06.HR06010 = AppContext.Name;
                    hr06List.Add(hr06);
                }

                await HkksDb.UpdateTableAsync(new List<HR02KSKK>(), hr03ListToUpdate, hr03ListToDelete, new List<HR04KSHIS>(), hr06List);

                foreach (var hr03 in hr03ListToDelete)
                {
                    string imageType = hr03.HR03017 == 0 ? ".jpg" : ".svg";
                    string imagePath = Path.Combine(path, hr03.HR03002 + imageType);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return false;
            }

            return true;
        }
    }
}