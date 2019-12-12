using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WeatherWorkStation.Models;
namespace WeatherWorkStation.Repositories
{
    public class NodeDAO:IDisposable
    {
        private WeatherWorkStationEntities db;
        private List<Node> nodeList;
        private bool disposed = false;
        public NodeDAO()
        { 
            db = new WeatherWorkStationEntities();
            nodeList = new List<Node>();
            var queryAllNode = from node in db.Node
                               select node;
            foreach(var entity in queryAllNode)
            {
                nodeList.Add(entity);
            }
        }
        public List<Node> GetNodes()
        {
            return nodeList;
        }
        public Node FindNodeById(String id)
        {
          
            if (0 == nodeList.Capacity)
                return null;
            foreach(Node node in nodeList)
            {
                if(node.NodeId.Equals(id))
                {
                    return node;
                }
            }
            return null;
        }
        public async void SetNodeStatus(bool status,String nodeId)
        {
            var queryAllNode = from obj in db.Node
                               select obj;
            foreach(var obj in queryAllNode)
            {
                if(obj.NodeId == nodeId)
                {
                    obj.Status = status;
                    await db.SaveChangesAsync();
                    break;
                }
            }
        }
        public Boolean isNodeExist(String id)
        {
            if (0 == nodeList.Capacity)
                return false;
            foreach(Node node in nodeList)
            {
                if(node.NodeId.Equals(id))
                {
                    return true;
                }
            }
            return false;
        }
        
        public async Task<Boolean> UpdateOrInsertNode(Node node)
        {
            if (null == node)
                return false;
            var queryAllDetail = from obj in db.Node
                               select obj;
            bool flag = false;
            bool isExisted = false;
            foreach (var entity in queryAllDetail)
            {   
                if(node.NodeId == entity.NodeId)
                {
                    entity.Status = true;
                    isExisted = true;
                }
                if(isExisted && node.NodeLocation != entity.NodeLocation)
                {
                    entity.NodeLocation = node.NodeLocation;
                    entity.Status = true;
                    flag = true;
                    break;                       
                }
            }
            if(flag)
            {
                await db.SaveChangesAsync();
                return true;
            }
            if (isExisted)
            {
                await db.SaveChangesAsync();
                return true;
            }
            db.Node.Add(node);
            await db.SaveChangesAsync();
            return true;
        }
        public List<String> GetAllNodeId()
        {
            var queryAllNodeId = (from obj in db.Node
                                 select obj.NodeId).Distinct();
            List<String> returnList = new List<string>();
            foreach(var obj in queryAllNodeId)
            {
                returnList.Add(obj);

            }
            return returnList;

        }
        ~NodeDAO()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
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