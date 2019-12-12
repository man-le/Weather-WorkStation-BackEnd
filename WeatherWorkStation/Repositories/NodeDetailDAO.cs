using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WeatherWorkStation.Models;
namespace WeatherWorkStation.Repositories
{
    public class NodeDetailDAO:IDisposable
    {
        private WeatherWorkStationEntities db;
        private List<NodeDetail> nodeDetails;
        private bool disposed = false;
        public NodeDetailDAO()
        {
            db = new WeatherWorkStationEntities();
            nodeDetails = new List<NodeDetail>();
            var queryAllDetails = from detail in db.NodeDetail
                                  select detail;
            foreach(var entity in queryAllDetails)
            {
                nodeDetails.Add(entity);
            }
        }
        public List<NodeDetail> GetDetails()
        {
            return nodeDetails;
        }
        public async void InsertNodeDetail(NodeDetail nodeDetail)
        {
            if (null == nodeDetail)
                return;
            nodeDetails.Add(nodeDetail);
            await db.SaveChangesAsync();
        }
        public async Task<Boolean> InsertOrUpdateNodeDetail(NodeDetail nodeDetail)
        {
            if (null == nodeDetail)
                return false;
            var queryAllDetails = from detail in db.NodeDetail
                                  select detail;
            foreach (var detail in queryAllDetails)
            {
                if(detail.Equals(nodeDetail))
                {
                    return false;
                }
            }
            db.NodeDetail.Add(nodeDetail);
            await db.SaveChangesAsync();
            return true;
        }
        public NodeDetail GetDetailByNodeId(String nodeId)
        {

            if (0 == nodeDetails.Capacity)
            {
                return null;
            }
            List<NodeDetail> returnList = new List<NodeDetail>();
            foreach(var item in nodeDetails)
            {
                if(item.NodeId == nodeId)
                {
                    returnList.Add(item);
                }
            }
            return returnList.Last();
        }
        ~NodeDetailDAO(){
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}