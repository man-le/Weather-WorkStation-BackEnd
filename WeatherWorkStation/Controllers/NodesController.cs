using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WeatherWorkStation.Models;
using WeatherWorkStation.Utils;
using WeatherWorkStation.Repositories;
using System.Threading;

namespace WeatherWorkStation.Controllers
{
    public class NodesController : ApiController
    {
        private NodeDAO nodeDAO;
        private NodeDetailDAO nodeDetailDAO;
        // GET: api/Nodes
        public List<NodeInfo> GetNode()
        {
            nodeDAO = new NodeDAO();
            nodeDetailDAO = new NodeDetailDAO();
            List<NodeInfo> nodeInfos = new List<NodeInfo>();

            var nodes = nodeDetailDAO.GetDetails();
            foreach(var x in nodes)
            {
                NodeInfo temp = new NodeInfo();
                if(null==nodeDAO.FindNodeById(x.NodeId))
                {

                    return null;
                }
                temp.NodeId = x.NodeId;
                temp.NodeLocation = nodeDAO.FindNodeById(x.NodeId).NodeLocation;
                temp.Humidity = x.Humidity.Value;
                temp.Raining = x.Raining.Value;
                temp.SoilMoisture = x.SoilMoisture.Value;
                temp.Temperature = x.Temperature.Value;
                temp.UpdateTime = x.updateTime;
                nodeInfos.Add(temp);
            }
            return nodeInfos;
        }
        
        
        // GET: api/Nodes/5
        [ResponseType(typeof(Node))]
        public IHttpActionResult GetNode(string id)
        {
            nodeDAO = new NodeDAO();
            nodeDetailDAO = new NodeDetailDAO();
            NodeInfo node = new NodeInfo();
            if (!nodeDAO.isNodeExist(id))
                return NotFound();
            NodeDetail nodeDetail = nodeDetailDAO.GetDetailByNodeId(id);
            if (null == nodeDetail)
                return NotFound();
            node.NodeId = nodeDetail.NodeId;
            node.Humidity = nodeDetail.Humidity.Value;
            node.Raining = nodeDetail.Raining.Value;
            node.SoilMoisture = nodeDetail.SoilMoisture.Value;
            node.Temperature = nodeDetail.Temperature.Value;
            node.UpdateTime = nodeDetail.updateTime;
            node.NodeLocation = nodeDAO.FindNodeById(id).NodeLocation;

            if (node == null)
            {
                return NotFound();
            }

            return Ok(node);
        }
        

        // POST: api/Nodes
        [ResponseType(typeof(PostNode))]
        public async Task<IHttpActionResult> PostNode(PostNode node)
        {
            if (null == node)
                return BadRequest();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            nodeDAO = new NodeDAO();
            nodeDetailDAO = new NodeDetailDAO();

            //<-Generate Node and NodeDetail->
            Node newNode = new Node();
            NodeDetail newDetail = new NodeDetail();


            // <-Get Node Inform from Post Node->
            newNode.NodeId = node.NodeId;
            newNode.NodeLocation = node.NodeLocation;
            newNode.Status = true;

            // <-Get NodeDetail Inform from Post Node->
            newDetail.NodeId = node.NodeId;
            newDetail.Raining = node.Raining;
            newDetail.Humidity = node.Humidity;
            newDetail.SoilMoisture = node.SoilMoisture;
            newDetail.Temperature = node.Temperature;
            var dateTime = DateTime.Now;
            var dateTimeOffset = new DateTimeOffset(dateTime);
            var unixDateTime = dateTimeOffset.ToUnixTimeSeconds();
            newDetail.updateTime = unixDateTime;
            try
            {
                if (!await nodeDAO.UpdateOrInsertNode(newNode))
                {
                    return BadRequest();
                }
                if (!await nodeDetailDAO.InsertOrUpdateNodeDetail(newDetail))
                {
                    return BadRequest();
                }

            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
            return Ok(node);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                nodeDetailDAO.Dispose();
                nodeDAO.Dispose();
            }
            base.Dispose(disposing);
        }

        
    }
}