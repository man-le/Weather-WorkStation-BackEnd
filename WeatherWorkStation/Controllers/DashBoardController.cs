using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WeatherWorkStation.Utils;
using WeatherWorkStation.Repositories;
using System.Web.Mvc;
using System.Threading.Tasks;
using WeatherWorkStation.Models;

namespace WeatherWorkStation.Controllers
{
    public class DashBoardController : Controller
    {
        private NodeDAO nodeDAO = new NodeDAO();

        [ChildActionOnly]
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.Client, Duration = 2)]
        private async Task<Boolean> CheckStatus()
        {
            WeatherWorkStation.Models.WeatherWorkStationEntities db = new Models.WeatherWorkStationEntities();
            var nodeIdQuery = from obj in db.Node
                              select obj.NodeId;
            foreach (var obj in nodeIdQuery)
            {
                var nodeInfoQuery = (from detail in db.NodeDetail
                                    where detail.NodeId == obj
                                    orderby detail.Id descending
                                    select detail).First();
                var dateTime = DateTime.Now;
                var dateTimeOffset = new DateTimeOffset(dateTime);
                var now = dateTimeOffset.ToUnixTimeSeconds();
                if ((now - nodeInfoQuery.updateTime) > Utils.Variables.TIME_STATUS_UPDATE)
                {
                    var result = db.Node.SingleOrDefault(x => x.NodeId == obj);
                    if (null != result)
                    {
                        result.Status = false;
                    }
                }
            }
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<ActionResult> Refresh()
        {
            await CheckStatus();
            return PartialView("~/Views/Partials/_NodeStatus.cshtml");
        }
        public ActionResult RefreshWeatherTable()
        {
            return PartialView("~/Views/Partials/_DashBoardWeather.cshtml");
        }
        [ChildActionOnly]
        public List<Nodes> GetNodes()
        {
            nodeDAO = new NodeDAO();
            var list = nodeDAO.GetNodes();
            List<Nodes> returnList = new List<Nodes>();
            foreach (var item in list)
            {
                Nodes temp = new Nodes();
                temp.Id = item.Id;
                temp.NodeLocation = item.NodeLocation;
                temp.Status = item.Status.Value;
                temp.NodeId = item.NodeId;
                returnList.Add(temp);
            }
            return returnList;
        }
        public NodeInfo GetNodeInfo(String id)
        {
            NodeDAO nodeDAO = new NodeDAO();
            NodeDetailDAO nodeDetailDAO = new NodeDetailDAO();
            NodeInfo node = new NodeInfo();
            if (!nodeDAO.isNodeExist(id))
                return null;
            NodeDetail nodeDetail = nodeDetailDAO.GetDetailByNodeId(id);
            if (null == nodeDetail)
                return null;
            node.NodeId = nodeDetail.NodeId;
            node.Humidity = nodeDetail.Humidity.Value;
            node.Raining = nodeDetail.Raining.Value;
            node.SoilMoisture = nodeDetail.SoilMoisture.Value;
            node.Temperature = nodeDetail.Temperature.Value;
            node.UpdateTime = nodeDetail.updateTime;
            node.NodeLocation = nodeDAO.FindNodeById(id).NodeLocation;
            return node;
        }

        [ChildActionOnly]
        public List<String> GetNodeId()
        {
            return nodeDAO.GetAllNodeId();
        }
    }
}