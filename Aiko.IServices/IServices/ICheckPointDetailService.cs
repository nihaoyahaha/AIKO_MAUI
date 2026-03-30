using Aiko.SqliteDb;

namespace Aiko.IServices.IServices
{
    public interface ICheckPointDetailService : IServiceBase
    {
        public Task<List<HR01ITEMPINFO>> GetHR01ITEMPINFO(string HR01001, string HR01003, string path);

        public Task<List<HR03SYAS>> GetHR03Pic(string HR01001, string HR01003, string proj);

        public Task<List<HM16SHDIR>> GetHM16List(string HR01001);

        public Task<bool> UpdateHR03(List<HR03SYAS> hr03List, string path);
    }
}
